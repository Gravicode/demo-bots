using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Newtonsoft.Json;
using ServiceStack.Redis;
using System.Configuration;

namespace PushDataToStorage
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        public static PooledRedisClientManager redisManager { set; get; }
        public static MqttClient client { set; get; }
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            var config = new JobHostConfiguration();

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            var host = new JobHost();
            IoTHub();
            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
        }

        static void IoTHub()
        {
            if (redisManager == null)
            {
                redisManager = new PooledRedisClientManager(7, ConfigurationManager.AppSettings["RedisCon"]);
            }
            if (client == null)
            {
                // create client instance 
                string MQTT_BROKER_ADDRESS = "gravicodeservices.cloudapp.net";
                client = new MqttClient(MQTT_BROKER_ADDRESS);
                string clientId = Guid.NewGuid().ToString();
                client.Connect(clientId, "mifmasterz", "123qweasd");
                SubscribeMessage();

            }

           


        }
        static void SubscribeMessage()
        {

            // register to message received 

            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            client.Subscribe(new string[] { "mifmasterz/assistant/data", "mifmasterz/assistant/control" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });

        }

        static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)

        {

            string Pesan = Encoding.UTF8.GetString(e.Message);

            switch (e.Topic)

            {

                case "mifmasterz/assistant/data":
                    var data = JsonConvert.DeserializeObject<RoomSensor>(Pesan);

                   
                    using (var redis = redisManager.GetClient())
                    {
                        var redisTodos = redis.As<RoomSensor>();
                        data.Id = redisTodos.GetNextSequence();
                      
                        redisTodos.Store(data);
                        
                    }
                    break;

                case "mifmasterz/assistant/control":

                   //do nothing
                    break;
            }
        }
    }

    public class RoomSensor
    {
        public long Id { get; set; }
        public double Temp { get; set; }
        public double Humid { get; set; }
        public double Light { get; set; }
        public double Gas { get; set; }
        public DateTime Tanggal { get; set; }

    }
}
