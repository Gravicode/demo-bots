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
    public class NewsDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            try
            {
                var hasil = AirportData.GetLatestNews();
                if (hasil != null)
                {

                    Activity replyToConversation = context.MakeMessage() as Activity; //message.CreateReply("Should go to conversation, in list format");
                    replyToConversation.Attachments = new List<Attachment>();


                    foreach (var item in hasil)
                    {
                        AdaptiveCard card = new AdaptiveCard();
                        
                        // Specify speech for the card.
                        card.Speak = $"<s>{Tools.StripHTML( item.CONTENT_ENG)}</s>";

                        // Add text to the card.
                        card.Body.Add(new TextBlock()
                        {
                            Text = $"{item.TITLE_ENG}",
                            Size = TextSize.Medium,
                            Weight = TextWeight.Normal,
                            Wrap=true
                        });
                        card.Body.Add(new TextBlock()
                        {
                            Text = $"published at {item.DATE_PUBLISH.Replace(":00:000AM",string.Empty).Replace(":00:000PM", string.Empty)} by {item.CREATED_BY}",
                            Size = TextSize.Normal,
                            Weight = TextWeight.Lighter
                        });

                        card.Body.Add(new Image() { Url = $"{item.IMAGES}", Size=ImageSize.Auto });
                        // Add text to the card.
                        card.Body.Add(new TextBlock()
                        {
                            Wrap= true,
                            Text = $"{Tools.StripHTML(item.CONTENT_ENG)}"
                        });
                        
                        
                        card.Actions.Add(new HttpAction()
                        {
                            Url = $"{item.ATTACHMENT}",
                            Title = "Open"
                        });
                        
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
    
}