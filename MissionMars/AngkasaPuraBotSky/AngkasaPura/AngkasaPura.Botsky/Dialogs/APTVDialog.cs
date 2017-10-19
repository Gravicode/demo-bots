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

namespace AngkasaPura.Botsky.Dialogs
{
    [Serializable]
    public class APTVDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            try
            {
                var hasil = AirportData.GetAPTV();
                if (hasil != null)
                {

                    Activity replyToConversation = context.MakeMessage() as Activity; //message.CreateReply("Should go to conversation, in list format");
                    replyToConversation.AttachmentLayout = AttachmentLayoutTypes.List;
                    replyToConversation.Attachments = new List<Attachment>();
                    //replyToConversation.ReplyToId = context.Activity.ReplyToId;
                    Dictionary<string, string> cardContentList = new Dictionary<string, string>();
                    foreach (var item in hasil)
                    {
                        List<CardImage> cardImages = new List<CardImage>();
                        cardImages.Add(new CardImage(url: item.potrait));

                        List<CardAction> cardButtons = new List<CardAction>();

                        CardAction plButton = new CardAction()
                        {
                            Value = $"{item.content}",
                            Type = "playVideo",
                            Title = "Play Video"
                        };

                        cardButtons.Add(plButton);

                        ThumbnailCard plCard = new ThumbnailCard()
                        {
                            Title = $"{Tools.StripHTML(item.TITTLE)}",
                            Text = $"{Tools.StripHTML(item.DESCRIPTION)}",
                            Images = cardImages,
                            Buttons = cardButtons
                        };

                        Attachment plAttachment = plCard.ToAttachment();
                        replyToConversation.Attachments.Add(plAttachment);
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