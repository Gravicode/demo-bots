using System;
using Microsoft.Bot.Builder.FormFlow;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System.Configuration;
using Microsoft.Bot.Builder.Dialogs;
using AngkasaPura.Botsky.Business;
using System.Collections.Generic;
using Microsoft.Bot.Connector;
using AngkasaPura.Botsky.Helpers;
using AdaptiveCards;

namespace AngkasaPura.Botsky.Dialogs
{
    [Serializable]
    public class ReportAPDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Please input report range date.");
            var OrderFormDialog = FormDialog.FromForm<ReportAPQuery>(ReportAPQuery.BuildForm, FormOptions.PromptInStart);
            context.Call(OrderFormDialog, this.ResumeAfterOrderFormDialog);
        }

        private async Task ResumeAfterOrderFormDialog(IDialogContext context, IAwaitable<ReportAPQuery> result)
        {
            try
            {
                var hasil = await result;
                if (hasil.Results != null)
                {

                    Activity replyToConversation = context.MakeMessage() as Activity; //message.CreateReply("Should go to conversation, in list format");
                    replyToConversation.Attachments = new List<Attachment>();


                    foreach (var item in hasil.Results)
                    {
                        AdaptiveCard card = new AdaptiveCard();

                        // Specify speech for the card.
                        card.Speak = $"<s>this report published at {item.REPORT_DATE}, it's about {item.MS_DETAIL_REPORT_CATEGORY_NAME}</s>";

                        // Add text to the card.
                        card.Body.Add(new TextBlock()
                        {
                            Text = $"Report - {item.MS_REPORT_CATEGORY_NAME}",
                            Size = TextSize.Medium,
                            Weight = TextWeight.Bolder,
                            Wrap = true
                        });

                        // Add text to the card.
                        card.Body.Add(new TextBlock()
                        {
                            Text = $"{item.MS_SUB_REPORT_CATEGORY_NAME} - {item.MS_DETAIL_REPORT_CATEGORY_NAME}",
                            Size = TextSize.Normal,
                            Weight = TextWeight.Bolder,
                            Wrap = true
                        });
                        // Add text to the card.
                        card.Body.Add(new TextBlock()
                        {
                            Text = $"REPORT DATE : {item.REPORT_DATE} "
                        }); // Add text to the card.
                        card.Body.Add(new TextBlock()
                        {
                            Text = $"BRANCH CODE : {item.BRANCH_CODE} "
                        });
                        // Add text to the card.
                        card.Body.Add(new TextBlock()
                        {
                            Text = $"DESCRIPTION : {item.DESCRIPTION} "
                        });
                      
                        
                        /*
                        card.Actions.Add(new HttpAction()
                        {
                            Url = "http://foo.com",
                            Title = "Snooze"
                        });
                        */
                        // Create the attachment.
                        Attachment attachment = new Attachment()
                        {
                            ContentType = AdaptiveCard.ContentType,
                            Content = card
                        };
                        replyToConversation.Attachments.Add(attachment);
                    }
                    await context.PostAsync(replyToConversation);
                }
                else
                {
                    await context.PostAsync("No result..");
                }
            }
            catch (FormCanceledException ex)
            {
                string reply;

                if (ex.InnerException == null)
                {
                    reply = MESSAGESINFO.CANCEL_DIALOG;
                }
                else
                {
                    reply = $"{MESSAGESINFO.ERROR_INFO} Detail: {ex.InnerException.Message}";
                }

                await context.PostAsync(reply);
            }
            finally
            {
                context.Done<object>(null);
            }
        }

    }

    [Serializable]
    //[Template(TemplateUsage.NotUnderstood, "Ane ga paham \"{0}\".", "Coba lagi ya, ane tidak dapat nilai \"{0}\".")]
    public class ReportAPQuery
    {
        public DateTime QueryDate;
        public List<Report> Results;

        [Prompt("Please input the start date (eg. 2016-12-20) ?")]
        public DateTime StartDate;

        [Prompt("Please input the end date (eg. 2016-12-20) ?")]
        public DateTime EndDate;
        public static IForm<ReportAPQuery> BuildForm()
        {

            OnCompletionAsyncDelegate<ReportAPQuery> processOrder = async (context, state) =>
            {
                await Task.Run(() =>
                {

                    state.QueryDate = DateTime.Now;
                    var data = AirportData.GetReportByDate(state.StartDate,state.EndDate);
                    if (data != null && data.Count > 0)
                    {
                        state.Results = data;
                    }
                    else
                    {
                        state.Results = null;
                    }
                    
                }
                 );
            };
            var builder = new FormBuilder<ReportAPQuery>(false);
            var form = builder
                        .Field(nameof(StartDate))
                        .Field(nameof(EndDate))
                        .OnCompletion(processOrder)
                        .Build();
            return form;
        }
    }
}