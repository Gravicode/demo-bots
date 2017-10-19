using System;
using Microsoft.Bot.Builder.FormFlow;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System.Configuration;
using Microsoft.Bot.Builder.Dialogs;
using AngkasaPura.Botsky.Helpers;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using AngkasaPura.Botsky.Business;

namespace AngkasaPura.Botsky.Dialogs
{
    [Serializable]
    public class ShoppingDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("welcome to angkasa pura shopping center, these are our products ?");
            Activity replyToConversation = context.MakeMessage() as Activity;

            replyToConversation.Attachments = new List<Attachment>();
            /*
            List<CardImage> cardImages = new List<CardImage>();
            cardImages.Add(new CardImage(url: "https://<imageUrl1>"));

            List<CardAction> cardButtons = new List<CardAction>();

            CardAction plButton = new CardAction()
            {
                Value = $"https://en.wikipedia.org/wiki/PigLatin",
                Type = "openUrl",
                Title = "WikiPedia Page"
            };

            cardButtons.Add(plButton);
            */

            List<ReceiptItem> receiptList = new List<ReceiptItem>();

            foreach (var item in AirportProduct.GetProducts())
            {
                ReceiptItem lineItem = new ReceiptItem()
                {
                    Title = $"{item.Name}",
                    Subtitle = $"Product ID: { item.IDProduct}",
                    Text = $"{item.Description}",
                    Image = new CardImage(url: $"{item.UrlImage}"),
                    Price = $"Rp.{item.Harga.ToString("{C:0}")}",
                    Quantity = $"{item.Stock}",
                    Tap = null
                };
                receiptList.Add(lineItem);
            }

            ReceiptCard plCard = new ReceiptCard()
            {
                Title = "these are our merchant products",
                //Buttons = cardButtons,
                Items = receiptList,

            };

            Attachment plAttachment = plCard.ToAttachment();
            replyToConversation.Attachments.Add(plAttachment);
            //send product list
            await context.PostAsync(replyToConversation);
            //send confirmation
            PromptDialog.Confirm(
               context,
               AfterShoppingConfirmation,
               "Do you want to order?",
               "Oups, I don't understand!",
               promptStyle: PromptStyle.None);


        }

        public async Task AfterShoppingConfirmation(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                var ShoppingFormDialog = FormDialog.FromForm<ShoppingQuery>(ShoppingQuery.BuildForm, FormOptions.PromptInStart);
                context.Call(ShoppingFormDialog, this.ResumeAfterShoppingFormDialog);
            }
            else
            {
                await context.PostAsync("Ok, thank you.");
            }
            //context.Wait(MessageReceivedAsync);
        }

        private async Task ResumeAfterShoppingFormDialog(IDialogContext context, IAwaitable<ShoppingQuery> result)
        {
            try
            {
                var hasil = await result;

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
    public class ShoppingQuery
    {
        public DateTime TanggalShopping;
        public string NoShopping;
        public string ProductName;
        public double Price;
        public int Stock;
        [Prompt("What's your name ? {||}")]
        public string Nama;

        [Prompt("Your address ? {||}")]
        public string Alamat;

        [Prompt("Your phone number ? {||}")]
        public string Telpon;

        [Prompt("Your email ? {||}")]
        public string Email;

        [Prompt("Type product ID ? {||}")]
        public string KodeProduk;

        [Prompt("How many item do you want to purchase ? {||}")]
        public int Qty;


        public static IForm<ShoppingQuery> BuildForm()
        {

            OnCompletionAsyncDelegate<ShoppingQuery> processShopping = async (context, state) =>
            {
                await Task.Run(async () =>
                {
                    state.NoShopping = $"SP-{DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss")}";
                    state.TanggalShopping = DateTime.Now;
                    StateClient stateClient = context.Activity.GetStateClient();
                    BotData botData = await stateClient.BotState.GetUserDataAsync(context.Activity.ChannelId, context.Activity.From.Id);
                    var myUserData = botData.GetProperty<Cart>("MyCart");
                    if (myUserData == null)
                    {
                        myUserData = new Cart();
                        myUserData.Nama = state.Nama;
                        myUserData.NoShopping = state.NoShopping;
                        myUserData.TanggalShopping = state.TanggalShopping;
                        myUserData.Telpon = state.Telpon;
                        myUserData.Total = 0;
                        myUserData.Tax = 0;
                    }

                    if (myUserData.Items == null)
                    {
                        myUserData.Items = new List<CartItem>();
                    }

                    myUserData.Items.Add(new CartItem() { KodeProduk = state.KodeProduk, Price = state.Price, ProductName = state.ProductName, Qty = state.Qty, Total = state.Qty * state.Price });
                    double TotAll = 0;
                    //calculate total and tax
                    foreach(var item in myUserData.Items)
                    {
                        TotAll += item.Total;
                    }
                    myUserData.Total = TotAll;
                    myUserData.Tax = TotAll * 0.1;

                    botData.SetProperty<Cart>("MyCart", myUserData);
                    await stateClient.BotState.SetUserDataAsync(context.Activity.ChannelId, context.Activity.From.Id, botData);
                    await AirportData.InsertShoppingOrder(myUserData);
                }
                 );
            };
            var builder = new FormBuilder<ShoppingQuery>(false);
            var form = builder
                        .Field(nameof(Nama))
                        .Field(nameof(Alamat))
                        .Field(nameof(Telpon))
                        .Field(nameof(Email))
                         .Field(nameof(KodeProduk), validate:
                            async (state, value) =>
                            {
                                var result = new ValidateResult { IsValid = true, Value = value, Feedback = "product ready" };
                                var p = AirportProduct.GetItemByID(value.ToString());
                                if (p != null)
                                {
                                    state.Price = p.Harga;
                                    state.ProductName = p.Name;
                                    state.Stock = p.Stock;
                                    result.Feedback = $"product is available.";
                                    result.IsValid = true;

                                }
                                else
                                {
                                    result.Feedback = $"product is not available.";
                                    result.IsValid = false;

                                }
                                return result;
                            })
                        .Field(nameof(Qty), validate:
                            async (state, value) =>
                            {
                                var result = new ValidateResult { IsValid = true, Value = value, Feedback = "product ready" };
                                var ok = int.TryParse(value.ToString(), out int Request);
                                /*
                                if (ok && Request >= state.Stock && Request > 0)
                                {
                                    result.Feedback = $"quantity is ok.";
                                    result.IsValid = true;

                                }
                                else
                                {
                                    result.Feedback = $"stock is not ready, fill with lower quantity.";
                                    result.IsValid = false;

                                }*/
                                return result;
                            })
                        .Confirm(async (state) =>
                        {
                            var pesan = $"your order is {state.Qty} items of {state.ProductName}, is it pk ?";
                            return new PromptAttribute(pesan);
                        })
                        .Message("Thanks, we will proceed your order!")
                        .OnCompletion(processShopping)
                        .Build();
            return form;
        }
    }
}