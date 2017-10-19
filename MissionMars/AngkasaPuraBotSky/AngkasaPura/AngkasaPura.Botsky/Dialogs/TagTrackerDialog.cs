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
    public class TagTrackerDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var OrderFormDialog = FormDialog.FromForm<TagTrackerQuery>(TagTrackerQuery.BuildForm, FormOptions.PromptInStart);
            context.Call(OrderFormDialog, this.ResumeAfterOrderFormDialog);
            
        }

        private async Task ResumeAfterOrderFormDialog(IDialogContext context, IAwaitable<TagTrackerQuery> result)
        {
            try
            {
                var hasil = await result;
                if (hasil.Results != null)
                {

                    Activity replyToConversation = context.MakeMessage() as Activity; //message.CreateReply("Should go to conversation, in list format");
                    replyToConversation.Attachments = new List<Attachment>();


                    if (hasil.Results != null)
                    {
                        AdaptiveCard card = new AdaptiveCard();

                        // Specify speech for the card.
                        card.Speak = $"<s>your tag is at {hasil.Results.Location} </s>";

                        // Add text to the card.
                        card.Body.Add(new TextBlock()
                        {
                            Text = "TAG INFO",
                            Size = TextSize.Large,
                            Weight = TextWeight.Bolder
                        });

                        // Add text to the card.
                        card.Body.Add(new TextBlock()
                        {
                            Text = $"TAG ID : {hasil.Results.TagCode} "

                        });
                        card.Body.Add(new TextBlock()
                        {
                            Text = $"Name : {hasil.Results.Name} "

                        });
                        card.Body.Add(new TextBlock()
                        {
                            Text = $"TAG TYPE : {hasil.Results.TagType.ToString()} "

                        });
                        card.Body.Add(new TextBlock()
                        {
                            Text = $"TAG LOCATION : {hasil.Results.Location} "

                        });

                        // Create the attachment.
                        Attachment attachment = new Attachment()
                        {
                            ContentType = AdaptiveCard.ContentType,
                            Content = card
                        };
                        replyToConversation.Attachments.Add(attachment);

                    }
                    else
                    {
                        replyToConversation.Text = "Tag is not found...";
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
    public class TagTrackerQuery
    {
        public DateTime QueryDate;
        public TagData Results;

        [Prompt("Please type your tag ID?")]
        public string TagCode;



        public static IForm<TagTrackerQuery> BuildForm()
        {

            OnCompletionAsyncDelegate<TagTrackerQuery> processOrder = async (context, state) =>
            {
                await Task.Run(() =>
                {

                    state.QueryDate = DateTime.Now;
                    var data = SampleData.GetTagByCode(state.TagCode);
                    if (data != null )
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
            var builder = new FormBuilder<TagTrackerQuery>(false);
            var form = builder
                        .Field(nameof(TagCode))
                        .OnCompletion(processOrder)
                        .Build();
            return form;
        }
    }
}