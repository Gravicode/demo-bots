using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Diagnostics;
using Newtonsoft.Json;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using GT = GHIElectronics.UWP.GadgeteerCore;
using GTMB = GHIElectronics.UWP.Gadgeteer.Mainboards;
using GTMO = GHIElectronics.UWP.Gadgeteer.Modules;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HomeController
{
  public class RoomSensor
    {
        public long Id { get; set; }
        public double Temp { get; set; }
        public double Humid { get; set; }
        public double Light { get; set; }
        public double Gas { get; set; }
        public DateTime Tanggal { get; set; }

    }
    public class ControlDevice
    {
        public string request { get; set; }
        public string reference { get; set; }
        public string value { get; set; }
        public string device_id { get; set; }
    }

    public sealed partial class MainPage : Page
    {
        private GTMB.FEZCream mainboard;
        private GTMO.TempHumidSI70 temphumid;
        private GTMO.GasSense gas;
        private GTMO.LightSense light;
        private GTMO.RelayX1 relay;
        private DispatcherTimer timer;
        public MainPage()
        {
            this.InitializeComponent();

            this.Setup();
        }

        private async void Setup()
        {
            this.mainboard = await GT.Module.CreateAsync<GTMB.FEZCream>();
            this.temphumid = await GT.Module.CreateAsync<GTMO.TempHumidSI70>(this.mainboard.GetProvidedSocket(3));
            this.gas = await GT.Module.CreateAsync<GTMO.GasSense>(this.mainboard.GetProvidedSocket(6));
            this.light = await GT.Module.CreateAsync<GTMO.LightSense>(this.mainboard.GetProvidedSocket(5));
            this.relay = await GT.Module.CreateAsync<GTMO.RelayX1>(this.mainboard.GetProvidedSocket(4));
            //mqtt
            if (client == null)
            {
                // create client instance 
                MQTT_BROKER_ADDRESS = "gravicodeservices.cloudapp.net";
                client = new MqttClient(MQTT_BROKER_ADDRESS);
                string clientId = Guid.NewGuid().ToString();
                client.Connect(clientId, "mifmasterz", "123qweasd");
                SubscribeMessage();

            }

            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromMilliseconds(2000);
            this.timer.Tick += this.OnTick;
            this.timer.Start();
        }

        private void OnTick(object sender, object e)
        {
            var sensor = new RoomSensor() { Tanggal=DateTime.Now, Gas = gas.ReadProportion(), Humid = temphumid.TakeMeasurement().RelativeHumidity, Light = light.GetReading(), Temp = temphumid.TakeMeasurement().Temperature };
            var data = JsonConvert.SerializeObject(sensor);
            client.Publish("mifmasterz/assistant/data", Encoding.UTF8.GetBytes(data), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);

        }
        public MqttClient client { set; get; }

        public string MQTT_BROKER_ADDRESS
        {
            set; get;
        }

        void SubscribeMessage()
        {

            // register to message received 

            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            client.Subscribe(new string[] { "mifmasterz/assistant/data", "mifmasterz/assistant/control" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });

        }

        async void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)

        {

            string Pesan = Encoding.UTF8.GetString(e.Message);

            switch (e.Topic)

            {

                case "mifmasterz/assistant/data":
                    var data = JsonConvert.DeserializeObject<RoomSensor>(Pesan);
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
                    {
                        TxtLight.Text = data.Light.ToString();
                        TxtTemp.Text = $"{data.Temp} C / {data.Humid} %";
                        TxtGas.Text = data.Gas.ToString();
                    }));

                    break;

                case "mifmasterz/assistant/control":

                    switch (Pesan)
                    {
                        case "LIGHT_ON":
                            this.relay.TurnOn();
                            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
                            {
                                TxtRelay.Text = "Light On";
                            }));
                            break;
                        case "LIGHT_OFF":
                            this.relay.TurnOff();

                            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
                            {
                                TxtRelay.Text = "Light Off";
                            }));
                  
                            break;
                        default:
                            /*
                            var data = JsonConvert.DeserializeObject<ControlDevice>(Pesan);
                            if (data.value == "1")
                            {
                                this.hat.D2.Color = GIS.FEZHAT.Color.Blue;
                                this.hat.D3.Color = GIS.FEZHAT.Color.Blue;
                                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
                                {
                                    TxtStatus.Text = "Light On";
                                }));
                            }
                            else
                            {
                                this.hat.D2.Color = GIS.FEZHAT.Color.Black;
                                this.hat.D3.Color = GIS.FEZHAT.Color.Black;
                                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
                                {
                                    TxtStatus.Text = "Light Off";
                                }));
                            }*/
                            break;
                    }
                    break;
            }
            Debug.WriteLine(e.Topic + ":" + Pesan);
        }
    }
}
