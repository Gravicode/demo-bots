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
    public class LuggageDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var OrderFormDialog = FormDialog.FromForm<LuggageQuery>(LuggageQuery.BuildForm, FormOptions.PromptInStart);
            context.Call(OrderFormDialog, this.ResumeAfterOrderFormDialog);
        }

        private async Task ResumeAfterOrderFormDialog(IDialogContext context, IAwaitable<LuggageQuery> result)
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
                        card.Speak = $"<s>Your luggage is at GATE {item.AGATE} in Terminal {item.TERMINAL}, your air line is {item.LONG_NAME} FROM {item.LONG_NAME1} ARRIVED AT {Convert.ToDateTime(item.STA_TIME_STAMP).ToString("dd MMMM yyyy HH:mm")}</s>";

                        // Add text to the card.
                        card.Body.Add(new TextBlock()
                        {
                            Text = "LUGGAGE INFO",
                            Size = TextSize.Large,
                            Weight = TextWeight.Bolder
                        });

                        // Add text to the card.
                        card.Body.Add(new TextBlock()
                        {
                            Text = $"AIRLINE : {item.LONG_NAME} - {item.FLIGHT_NUM} "
                        });

                        // Add text to the card.
                        card.Body.Add(new TextBlock()
                        {
                            Text = $"TERMINAL : {item.TERMINAL} "
                        });
                        // Add text to the card.
                        card.Body.Add(new TextBlock()
                        {
                            Text = $"A GATE : {item.AGATE} "
                        });
                        // Add text to the card.
                        card.Body.Add(new TextBlock()
                        {
                            Text = $"ETA : {item.ETA_TIME_STAMP} "
                        });
                        // Add text to the card.
                        card.Body.Add(new TextBlock()
                        {
                            Text = $"ACTUAL CLAIM : {item.ACTUAL_CLAIM} "
                        });
                        card.Body.Add(new TextBlock()
                        {
                            Text = $"STA : {item.STA} "
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
    public class LuggageQuery
    {
        public DateTime QueryDate;
        public List<Luggage> Results;

        [Prompt("What is the airline name (not necessary to use full name) ?")]
        public string Airline;

        [Prompt("What is the flight number (check your boarding pass) ?")]
        public string FlightNo;
        public static IForm<LuggageQuery> BuildForm()
        {

            OnCompletionAsyncDelegate<LuggageQuery> processOrder = async (context, state) =>
            {
                await Task.Run(() =>
                {

                    state.QueryDate = DateTime.Now;
                    var data = AirportData.GetLuggages(state.Airline,state.FlightNo);
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
            var builder = new FormBuilder<LuggageQuery>(false);
            var form = builder
                        .Field(nameof(Airline))
                        .Field(nameof(FlightNo))
                        .OnCompletion(processOrder)
                        .Build();
            return form;
        }
    }
}