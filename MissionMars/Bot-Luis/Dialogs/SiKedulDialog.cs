using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System.Configuration;
using ServiceStack.Redis;
using System.Net.Http;
using Newtonsoft.Json;

namespace Bot_Luis
{
    [LuisModel("06ac8925-f726-436c-8ddb-944700ca938c", "0d97be5a9b63419b884977611ccdba1f",LuisApiVersion.V2,domain: "westus.api.cognitive.microsoft.com", Staging =false)]
    [Serializable]
    public class SiKedulDialog : LuisDialog<object>
    {
        static MqttEngine _mqtt;
        public MqttEngine mqtt
        {
            get
            {
                if (_mqtt == null) _mqtt = new MqttEngine();
                return _mqtt;
            }
        }

        /*
        
         */

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry I did not understand: " + string.Join(", ", result.Intents.Select(i => i.Intent));
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }
        [LuisIntent("cctv.control")]
        public async Task CCTVControl(IDialogContext context, LuisResult result)
        {
            var pesan = "gagal query data cctv.";

            var url = "http://gravicodeabsensiweb.azurewebsites.net/api/CCTVs";
            HttpClient client = new HttpClient();
            var hasil = await client.GetAsync(url);
            if (hasil.IsSuccessStatusCode)
            {
                var datas = JsonConvert.DeserializeObject<List<CctvObject>>(await hasil.Content.ReadAsStringAsync());
                var item = datas[0];
                Activity replyToConversation = context.MakeMessage() as Activity; //message.CreateReply("Should go to conversation, in list format");
                replyToConversation.AttachmentLayout = AttachmentLayoutTypes.List;
                replyToConversation.Attachments = new List<Attachment>();
                //replyToConversation.ReplyToId = context.Activity.ReplyToId;
                Dictionary<string, string> cardContentList = new Dictionary<string, string>();

                List<CardImage> cardImages = new List<CardImage>();
                cardImages.Add(new CardImage(url: item.imageUrl));

                List<CardAction> cardButtons = new List<CardAction>();
                CardAction plButton = new CardAction()
                {
                    Value = $"{item.imageUrl}",
                    Type = "openUrl",
                    Title = "Open Image"
                };

                cardButtons.Add(plButton);
                ThumbnailCard plCard = new ThumbnailCard()
                {
                    Title = $"{item.camName}",
                    Text = $"{item.tanggal.ToString("dd-MMM-yyyy HH:mm")} - {item.description}",
                    Images = cardImages,Buttons=cardButtons
                };

                Attachment plAttachment = plCard.ToAttachment();
                replyToConversation.Attachments.Add(plAttachment);

                await context.PostAsync(replyToConversation);
            }else
                await context.PostAsync(pesan);
            context.Done<string>(null);

        }

        [LuisIntent("room.sensor")]
        public async Task RoomSensor(IDialogContext context, LuisResult result)
        {
            var pesan = "gagal baca data sensor.";
            using (var redisManager = new PooledRedisClientManager(7,ConfigurationManager.AppSettings["RedisCon"]))
            using (var redis = redisManager.GetClient())
            {
                var redisTodos = redis.As<RoomSensor>();

                var data = (from c in redisTodos.GetAll()
                           orderby c.Id descending
                           select c).Take(10).ToList();
                pesan = $"temperatur ruangan {data[0].Temp} C, kelembapan : {data[0].Humid} %, cahaya : {data[0].Light}, gas : {data[0].Gas}";
            }

            await context.PostAsync(pesan);
            context.Done<string>(null);

        }
        [LuisIntent("find.food")]
        public async Task FindFood(IDialogContext context, LuisResult result)
        {
            EntityRecommendation title;
            if (result.TryFindEntity("foodcategory", out title))
            {
                var selfood  = title.Entity;
                PromptDialog.Confirm(context, AfterConfirming_FoodSelect, $"Apakah Anda mencari makanan {selfood} ?", promptStyle: PromptStyle.None);
                context.UserData.SetValue<string>("SelFood",selfood);
                //await context.PostAsync();
            }
            else
            {
                await context.PostAsync("tidak ditemukan jenis makanan");
                context.Wait(MessageReceived);
            }
           

        }
        protected override async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            _message = (Activity)await item;
            context.UserData.SetValue<Activity>("Activity", _message);
            await base.MessageReceived(context, item);
        }

        [field: NonSerialized()]
        private Activity _message;
        public async Task AfterConfirming_FoodSelect(IDialogContext context, IAwaitable<bool> confirmation)
        {
            if (await confirmation)
            {
                var selfood = string.Empty;
                context.UserData.TryGetValue<string>("SelFood", out selfood);
                var hasil = await BingImagesHelper.Search(selfood + " food");
                if (hasil != null)
                {
                    context.UserData.TryGetValue<Activity>("Activity",out _message);
                    ConnectorClient connector = new ConnectorClient(new Uri(_message.ServiceUrl));
                    Activity replyToConversation = _message.CreateReply($"Daftar Makanan {selfood}");
                    replyToConversation.Recipient = _message.From;
                    replyToConversation.Type = "message";
                    replyToConversation.Attachments = new List<Attachment>();
                    List<CardImage> cardImages = new List<CardImage>();

                    foreach (var x in hasil.value)
                    {
                        cardImages.Add(new CardImage(url: x.thumbnailUrl));
                    }
                  
                 
                    List<CardAction> cardButtons = new List<CardAction>();
                    CardAction plButton = new CardAction()
                    {
                        Value = $"https://www.google.co.id/?q={selfood}",
                        Type = "openUrl",
                        Title = "Cari"
                    };
                    cardButtons.Add(plButton);
                    HeroCard plCard = new HeroCard()
                    {
                        Title = $"Makanan {selfood}",
                        Subtitle = "Cari lebih lanjut...",
                        Images = cardImages,
                        Buttons = cardButtons
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);
                    var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);
                }
              
                //await context.PostAsync($"Ok, mencari makanan {selfood}");
            }
            else
            {
                await context.PostAsync("Ok! tidak jadi mencari makanan!");
            }
            context.Wait(MessageReceived);
        }
        [LuisIntent("find.image")]
        public async Task FindImage(IDialogContext context, LuisResult result)
        {
            EntityRecommendation title;
            if (result.TryFindEntity("pictureobject", out title))
            {
                var selImg = title.Entity;
                var hasil = await BingImagesHelper.Search(selImg);
                if (hasil != null)
                {
                    context.UserData.TryGetValue<Activity>("Activity", out _message);
                    ConnectorClient connector = new ConnectorClient(new Uri(_message.ServiceUrl));
                    Activity replyToConversation = _message.CreateReply($"Gambar: {selImg}");
                    replyToConversation.Recipient = _message.From;
                    replyToConversation.Type = "message";
                    replyToConversation.Attachments = new List<Attachment>();
                    List<CardImage> cardImages = new List<CardImage>();

                    foreach (var x in hasil.value)
                    {
                        cardImages.Add(new CardImage(url: x.thumbnailUrl));
                    }
                  
                    List<CardAction> cardButtons = new List<CardAction>();
                   
                    CardAction plButton = new CardAction()
                    {
                        Value = $"https://www.google.co.id/?q={selImg}",
                        Type = "openUrl",
                        Title = "Cari"
                    };
                    cardButtons.Add(plButton);
                    ThumbnailCard plCard = new ThumbnailCard()
                    {
                        Title = $"Hasil pencarian {selImg}",
                        Subtitle = "Cari lebih lanjut..",
                        Images = cardImages,
                        Buttons = cardButtons
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);
                    var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);
                }

                //await context.PostAsync($"Mencari gambar {selImg}");
            }
            else
            {
                await context.PostAsync("tidak ditemukan jenis gambar");
            }
            context.Wait(MessageReceived);

        }
        [LuisIntent("control.light")]
        public async Task ControlLight(IDialogContext context, LuisResult result)
        {
            EntityRecommendation title;
            string room=null, state=null;
            if (result.TryFindEntity("room", out title))
            {
                room = title.Entity;
            }

            if (result.TryFindEntity("state", out title))
            {
                state = title.Entity;
            }

            if(!string.IsNullOrEmpty(room) && !string.IsNullOrEmpty(state))
            {
                await context.PostAsync($"Light is switch to {state} in {room}");
                mqtt.SendMessage("LIGHT_"+state.ToUpper());
            }
            else
            {
                await context.PostAsync("perintah tidak dikenal");
            }
            context.Wait(MessageReceived);

        }
    }
   
}