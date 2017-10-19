using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ADD THIS PART TO YOUR CODE
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System.IO;
using ServiceStack.Redis;

namespace DataService
{
    class Program
    { // ADD THIS PART TO YOUR CODE
        private const string EndpointUrl = "https://angkasapura.documents.azure.com:443/";
        private const string PrimaryKey = "e3nPUDGW6n52kt1XNvQPZ2PTKgRfnNzEZqfufLNdt1dfZkA0wCpTdgiVAjOT8fI4u2QVZvQN3D7ydpJA337aTg==";
        string ConStr = "vFfVFMQI5xC/Q4Ib4Y08mcrup6hNixMV8zYu7lqte4g=@redis-murahaje.redis.cache.windows.net:6379";
        //private DocumentClient client;
        static void Main(string[] args)
        { // ADD THIS PART TO YOUR CODE
            //try
            //{
                Program p = new Program();
                p.GetStartedDemo().Wait();
            //}
            /*
            catch (DocumentClientException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();
            }*/
        }

        // ADD THIS PART TO YOUR CODE
        private async Task GetStartedDemo()
        {

            /*
            this.client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);// ADD THIS PART TO YOUR CODE
            await this.client.CreateDatabaseIfNotExistsAsync(new Database { Id = "AngkasaPuraDB" });         
            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("AngkasaPuraDB"), new DocumentCollection { Id = "Flights" }); // ADD THIS PART TO YOUR CODE
            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("AngkasaPuraDB"), new DocumentCollection { Id = "Facilities" });            // ADD THIS PART TO YOUR CODE
            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("AngkasaPuraDB"), new DocumentCollection { Id = "News" });
            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("AngkasaPuraDB"), new DocumentCollection { Id = "Luggages" });
            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("AngkasaPuraDB"), new DocumentCollection { Id = "APTV" });
            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("AngkasaPuraDB"), new DocumentCollection { Id = "Reports" });
            */
            //insert flights
            /*
            try
            {
                using (var redisManager = new PooledRedisClientManager(3, ConStr))
                using (var redis = redisManager.GetClient())
                {

                    var items = GetFlights();
                    var redisTodos = redis.As<Flight>();
                    foreach (var item in items)
                    {
                        item.Id = redisTodos.GetNextSequence();
                        redisTodos.Store(item);
                        //await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri("AngkasaPuraDB", "Flights"), item);
                        this.WriteToConsoleAndPromptToContinue("Created item {0}", item.Id);

                    }
                }
            }
            catch (Exception de)
            {
               this.WriteToConsoleAndPromptToContinue("Error : {0}", de.Message);
            }
            */
            /*
            //dine
            
             using (var redisManager = new PooledRedisClientManager(3, ConStr))
                using (var redis = redisManager.GetClient())
                {

                    var items = GetFacility();
                    var redisTodos = redis.As<Facility>();
                    foreach (var item in items)
                    {
                        item.Id = redisTodos.GetNextSequence();
                        redisTodos.Store(item);
                        //await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri("AngkasaPuraDB", "Flights"), item);
                        this.WriteToConsoleAndPromptToContinue("Created item {0}", item.Id);

                    }
                }
                */
             /*
            try
            {
                var items = GetFacility();
                foreach (var item in items)
                {
                    await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri("AngkasaPuraDB", "Facilities"), item);
                    this.WriteToConsoleAndPromptToContinue("Created item {0}", item.Id);

                }
            }
            catch (DocumentClientException de)
            {
                this.WriteToConsoleAndPromptToContinue("Error : {0}", de.Message);
            }
            */
            /*
            //news
            using (var redisManager = new PooledRedisClientManager(3, ConStr))
                using (var redis = redisManager.GetClient())
                {

                    var items = GetNews();
                    var redisTodos = redis.As<News>();
                    foreach (var item in items)
                    {
                        item.Id = redisTodos.GetNextSequence();
                        redisTodos.Store(item);
                        //await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri("AngkasaPuraDB", "Flights"), item);
                        this.WriteToConsoleAndPromptToContinue("Created item {0}", item.Id);

                    }
                }
                */
                /*
            try
            {
                var items = GetNews();
                foreach (var item in items)
                {
                    await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri("AngkasaPuraDB", "News"), item);
                    this.WriteToConsoleAndPromptToContinue("Created item {0}", item.Id);

                }
            }
            catch (DocumentClientException de)
            {
                this.WriteToConsoleAndPromptToContinue("Error : {0}", de.Message);
            }
            */
            /*
            //luggage
            using (var redisManager = new PooledRedisClientManager(3, ConStr))
            using (var redis = redisManager.GetClient())
            {

                var items = GetLuggage();
                var redisTodos = redis.As<Luggage>();
                foreach (var item in items)
                {
                    item.Id = redisTodos.GetNextSequence();
                    redisTodos.Store(item);
                    //await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri("AngkasaPuraDB", "Flights"), item);
                    this.WriteToConsoleAndPromptToContinue("Created item {0}", item.Id);

                }
            }
            
            try
            {
                var items = GetLuggage();
                foreach (var item in items)
                {
                    await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri("AngkasaPuraDB", "Luggages"), item);
                    this.WriteToConsoleAndPromptToContinue("Created item {0}", item.Id);

                }
            }
            catch (DocumentClientException de)
            {
                this.WriteToConsoleAndPromptToContinue("Error : {0}", de.Message);
            }
            
            //aptv
            using (var redisManager = new PooledRedisClientManager(3, ConStr))
            using (var redis = redisManager.GetClient())
            {

                var items = GetAPTV();
                var redisTodos = redis.As<APTV>();
                foreach (var item in items)
                {
                    item.Id = redisTodos.GetNextSequence();
                    redisTodos.Store(item);
                    //await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri("AngkasaPuraDB", "Flights"), item);
                    this.WriteToConsoleAndPromptToContinue("Created item {0}", item.Id);

                }
            }
            
            try
            {
                var items = GetAPTV();
                foreach (var item in items)
                {
                    await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri("AngkasaPuraDB", "APTV"), item);
                    this.WriteToConsoleAndPromptToContinue("Created item {0}", item.Id);

                }
            }
            catch (DocumentClientException de)
            {
                this.WriteToConsoleAndPromptToContinue("Error : {0}", de.Message);
            }
            */
            //report
            using (var redisManager = new PooledRedisClientManager(3, ConStr))
            using (var redis = redisManager.GetClient())
            {

                var items = GetReport();
                var redisTodos = redis.As<Report>();
                foreach (var item in items)
                {
                    item.Id = redisTodos.GetNextSequence();
                    redisTodos.Store(item);
                    //await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri("AngkasaPuraDB", "Flights"), item);
                    this.WriteToConsoleAndPromptToContinue("Created item {0}", item.Id);

                }
            }
            /*
            try
            {
                var items = GetReport();
                foreach (var item in items)
                {
                    await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri("AngkasaPuraDB", "Reports"), item);
                    this.WriteToConsoleAndPromptToContinue("Created item {0}", item.Id);

                }
            }
            catch (DocumentClientException de)
            {
                this.WriteToConsoleAndPromptToContinue("Error : {0}", de.Message);
            }*/
        }// ADD THIS PART TO YOUR CODE
        private void WriteToConsoleAndPromptToContinue(string format, params object[] args)
        {
            Console.WriteLine(format, args);
            Console.WriteLine("Press any key to continue ...");
            //Console.ReadKey();
        }

        private List<Flight> GetFlights()
        {
            string path = @"C:\experiment\Github\AngkasaPuraBotSky\AngkasaPura\angkasa-data\flight.json";
            var datas = JsonConvert.DeserializeObject<List<Flight>>(File.ReadAllText(path));
            /*
            foreach(var item in datas)
            {
                item.Id = Guid.NewGuid().ToString();
            }*/
            return datas;
        }
        private List<Facility> GetFacility()
        {
            string path = @"C:\experiment\Github\AngkasaPuraBotSky\AngkasaPura\angkasa-data\important.json";
            var datas = JsonConvert.DeserializeObject<List<Facility>>(File.ReadAllText(path));
            /*
            foreach (var item in datas)
            {
                item.Id = Guid.NewGuid().ToString();
            }*/
            return datas;
        }

        private List<News> GetNews()
        {
            string path = @"C:\experiment\Github\AngkasaPuraBotSky\AngkasaPura\angkasa-data\news.json";
            var datas = JsonConvert.DeserializeObject<List<News>>(File.ReadAllText(path));
            /*
            foreach (var item in datas)
            {
                item.Id = Guid.NewGuid().ToString();
            }*/
            return datas;
        }
        private List<Luggage> GetLuggage()
        {
            string path = @"C:\experiment\Github\AngkasaPuraBotSky\AngkasaPura\angkasa-data\luggage.json";
            var datas = JsonConvert.DeserializeObject<List<Luggage>>(File.ReadAllText(path));
            /*
            foreach (var item in datas)
            {
                item.Id = Guid.NewGuid().ToString();
            }*/
            return datas;
        }
        private List<APTV> GetAPTV()
        {
            string path = @"C:\experiment\Github\AngkasaPuraBotSky\AngkasaPura\angkasa-data\aptv.json";
            var datas = JsonConvert.DeserializeObject<List<APTV>>(File.ReadAllText(path));
            /*
            foreach (var item in datas)
            {
                item.Id = Guid.NewGuid().ToString();
            }*/
            return datas;
        }
        private List<Report> GetReport()
        {
            string path = @"C:\experiment\Github\AngkasaPuraBotSky\AngkasaPura\angkasa-data\report.json";
            var datas = JsonConvert.DeserializeObject<List<Report>>(File.ReadAllText(path));
            /*
            foreach (var item in datas)
            {
                item.Id = Guid.NewGuid().ToString();
            }*/
            return datas;
        }
    }
    public class Report
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }
        public string MS_REPORT_CATEGORY_NAME { get; set; }
        public string MS_SUB_REPORT_CATEGORY_NAME { get; set; }
        public string MS_DETAIL_REPORT_CATEGORY_NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string REPORT_DATE { get; set; }
        public string BRANCH_CODE { get; set; }
    }
    public class APTV
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }
        public string potrait { get; set; }
        public string content { get; set; }
        public string TITTLE { get; set; }
        public string DESCRIPTION { get; set; }
        public string cover { get; set; }
        public string NAME { get; set; }
        public string BRANCH_NAME { get; set; }
    }
    public class Luggage
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }
        public string LONG_NAME { get; set; }
        public string AIRLINE_CODE { get; set; }
        public string FLIGHT_NUM { get; set; }
        public string CodeShare1 { get; set; }
        public string CodeShare2 { get; set; }
        public string CodeShare3 { get; set; }
        public object CodeShare4 { get; set; }
        public object CodeShare5 { get; set; }
        public string EffectiveDate { get; set; }
        public string ETA_TIME_STAMP { get; set; }
        public string STA_TIME_STAMP { get; set; }
        public string AGATE { get; set; }
        public string ARR_SHORT_REM { get; set; }
        public string Purpose { get; set; }
        public string PARKING_STAND { get; set; }
        public string INT_OR_DOM { get; set; }
        public string TERMINAL { get; set; }
        public string ARR_PLANE_NUM { get; set; }
        public string DEP_PLANE_NUM { get; set; }
        public string AIRCRAFT_TYPE { get; set; }
        public string GROUND_HANDLER_CODE { get; set; }
        public string UPLINE_CITY1 { get; set; }
        public string UPLINE_CITY2 { get; set; }
        public string UPLINE_CITY3 { get; set; }
        public string UPLINE_CITY4 { get; set; }
        public string LONG_NAME1 { get; set; }
        public string LONG_NAME2 { get; set; }
        public string LONG_NAME3 { get; set; }
        public object LONG_NAME4 { get; set; }
        public string ACTUAL_CLAIM { get; set; }
        public string STA { get; set; }
        public string RECLAIM_FIRST_BAG_TIME { get; set; }
    }
    public class News
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }
        public int ARTICLE_ID { get; set; }
        public string TITLE_IND { get; set; }
        public string TITLE_ENG { get; set; }
        public string CONTENT_IND { get; set; }
        public string CONTENT_ENG { get; set; }
        public string EVENT_START_DATE { get; set; }
        public string EVENT_END_DATE { get; set; }
        public string BRANCH { get; set; }
        public string CATEGORY_GROUP { get; set; }
        public string DATE_PUBLISH { get; set; }
        public int CATEGORY_ID { get; set; }
        public string CREATED_BY { get; set; }
        public string IMAGES { get; set; }
        public string ATTACHMENT { get; set; }
    }
    public class Facility
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }
        public string CATEGORY_NAME_ENG { get; set; }
        public string OBJECT_NAME { get; set; }
        public string OBJECT_PIC { get; set; }
        public string OBJECT_PHONE { get; set; }
        public string OBJECT_HP { get; set; }
        public string OBJECT_ADDRESS { get; set; }
        public string OBJECT_EMAIL { get; set; }
        public string OBJECT_IMAGE { get; set; }
        public string OBJECT_IMAGE1 { get; set; }
        public string OBJECT_DESC_ENG { get; set; }
        public string OBJECT_DESC { get; set; }
    }
    public class Flight
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }
        public string AFSKEY { get; set; }
        public string FLIGHT_NO { get; set; }
        public string LEG { get; set; }
        public string LEG_DESCRIPTION { get; set; }
        public string SCHEDULED { get; set; }
        public string ESTIMATED { get; set; }
        public string ACTUAL { get; set; }
        public string CATEGORY_CODE { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string REMARK_CODE { get; set; }
        public string REMARK_DESC_ENG { get; set; }
        public string REMARK_DESC_IND { get; set; }
        public string TERMINAL_ID { get; set; }
        public string GATE_CODE { get; set; }
        public string GATE_OPEN_TIME { get; set; }
        public string GATE_CLOSE_TIME { get; set; }
        public string BAGGAGE_CLAIM_NO { get; set; }
        public string BAGGAGE_CLAIM_OPEN_TIME { get; set; }
        public object BAGGAGE_CLAIM_CLOSE_TIME { get; set; }
        public string STATION1 { get; set; }
        public string STATION1_DESC { get; set; }
        public string STATION2 { get; set; }
        public string STATION2_DESC { get; set; }
        public string STATION3 { get; set; }
        public string STATION3_DESC { get; set; }
        public string STATION4 { get; set; }
        public string STATION4_DESC { get; set; }
        public string STATION5 { get; set; }
        public string STATION5_DESC { get; set; }
        public object STATION6 { get; set; }
        public object STATION6_DESC { get; set; }
        public string AIRLINE_CODE { get; set; }
        public string AIRLINE_NAME { get; set; }
        public string BRANCH_CODE { get; set; }
        public string FR { get; set; }
    }
}
