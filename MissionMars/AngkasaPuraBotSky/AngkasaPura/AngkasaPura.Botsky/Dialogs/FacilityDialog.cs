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
    public class FacilityDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var OrderFormDialog = FormDialog.FromForm<FacilityQuery>(FacilityQuery.BuildForm, FormOptions.PromptInStart);
            context.Call(OrderFormDialog, this.ResumeAfterOrderFormDialog);
        }

        private async Task ResumeAfterOrderFormDialog(IDialogContext context, IAwaitable<FacilityQuery> result)
        {
            try
            {
                var hasil = await result;
                if (hasil.Results != null)
                {
                    
                    Activity replyToConversation = context.MakeMessage() as Activity; //message.CreateReply("Should go to conversation, in list format");
                    replyToConversation.AttachmentLayout = AttachmentLayoutTypes.List;
                    replyToConversation.Attachments = new List<Attachment>();
                    //replyToConversation.ReplyToId = context.Activity.ReplyToId;
                    Dictionary<string, string> cardContentList = new Dictionary<string, string>();
                    foreach (var item in hasil.Results)
                    {
                        List<CardImage> cardImages = new List<CardImage>();
                        cardImages.Add(new CardImage(url: item.OBJECT_IMAGE));

                        List<CardAction> cardButtons = new List<CardAction>();

                        CardAction plButton = new CardAction()
                        {
                            Value = $"https://google.com?q={item.OBJECT_NAME}",
                            Type = "openUrl",
                            Title = "Search"
                        };

                        cardButtons.Add(plButton);

                        ThumbnailCard plCard = new ThumbnailCard()
                        {
                            Title = $"{Tools.StripHTML(item.OBJECT_NAME)}",
                            Subtitle = $"Address : { Tools.StripHTML( item.OBJECT_ADDRESS)}, Phone : {Tools.StripHTML(item.OBJECT_PHONE)}",

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

    [Serializable]
    public enum FacilityTypes
    {
        [Terms("Dine")]
        Dine=1,
        [Terms("Taxi")]
        Taxi,
        [Terms("Hotel")]
        Hotel,
        [Terms("Shop")]
        Shop
    }
    [Serializable]
    //[Template(TemplateUsage.NotUnderstood, "Ane ga paham \"{0}\".", "Coba lagi ya, ane tidak dapat nilai \"{0}\".")]
    public class FacilityQuery
    {
        public DateTime QueryDate;
        public List<Facility> Results;
        [Prompt("What are you looking for ? {||}")]
        public FacilityTypes FacilityType;
        [Prompt("What is the name or left blank if you don't remember ? {||}")]
        public string Name;

        public static IForm<FacilityQuery> BuildForm()
        {

            OnCompletionAsyncDelegate<FacilityQuery> processOrder = async (context, state) =>
            {
                await Task.Run(() =>
                {
               
                    state.QueryDate = DateTime.Now;
                    var data = AirportData.GetFacilityByCategory(state.FacilityType.ToString(), string.IsNullOrEmpty(state.Name)? null : state.Name);
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
            var builder = new FormBuilder<FacilityQuery>(false);
            var form = builder
                        .Field(nameof(FacilityType))
                        .Field(nameof(Name))
                        .OnCompletion(processOrder)
                        .Build();
            return form;
        }
    }
}