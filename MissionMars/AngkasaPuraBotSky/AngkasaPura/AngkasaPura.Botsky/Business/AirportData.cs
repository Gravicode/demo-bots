using AngkasaPura.Botsky.Dialogs;
using AngkasaPura.Botsky.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AngkasaPura.Botsky.Business
{
    public class AirportData
    {
        static RedisDB Data;
        public static RedisDB Context
        {
            get
            {
                if (Data == null)
                {
                    Data = new RedisDB();
                }
                return Data;
            }
        }
        #region Data Query
        public static List<Flight> GetFlightByCode(string Code)
        {
            var xx = Context.GetAllData<Flight>();
            var data = from c in xx
                   where c.FLIGHT_NO == Code
                   select c;
            return data.ToList();
        }
        public static List<Flight> GetFlightByAirline(string Airline)
        {
            //var data = Context.GetDataByQuery<Flight>("Flights", $"SELECT * FROM Flights WHERE CONTAINS(LOWER(Flights.AIRLINE_NAME),'{Airline.ToLower()}')");
            //return data;
            var xx = Context.GetAllData<Flight>();
            var data = from c in xx
                       where c.AIRLINE_NAME.ToLower().Contains(Airline.ToLower())
                       select c;
            return data.ToList();
        }
        public static List<Facility> GetFacilityByCategory(string Category, string Name = null)
        {
            //var query = Name == null ? $"SELECT * FROM C WHERE C.CATEGORY_NAME_ENG = '{Category}' " : $"SELECT * FROM C WHERE C.CATEGORY_NAME_ENG = '{Category}' AND CONTAINS(LOWER(C.OBJECT_NAME),'{Name.ToLower()}')";
            //var data = Context.GetDataByQuery<Facility>("Facilities", query);
            var xx = Context.GetAllData<Facility>();
            if (Name == null)
            {
                var data = from c in xx
                           where c.CATEGORY_NAME_ENG== Category
                           select c;
                return data.ToList();
               
            }
            else
            {
                var data = from c in xx
                           where c.CATEGORY_NAME_ENG == Category && c.OBJECT_NAME.ToLower().Contains(Name.ToLower())
                           select c;
                return data.ToList();
            }
        }
        public static List<Facility> GetImportantNumber(string Name = null)
        {
            //var query = Name == null ? $"SELECT * FROM c where c.CATEGORY_NAME_ENG <> 'Dine' AND c.CATEGORY_NAME_ENG <> 'Taxi' AND c.CATEGORY_NAME_ENG <> 'Hotel' AND c.CATEGORY_NAME_ENG <> 'Shop'" : $"SELECT * FROM c where c.CATEGORY_NAME_ENG <> 'Dine' AND c.CATEGORY_NAME_ENG <> 'Taxi' AND c.CATEGORY_NAME_ENG <> 'Hotel' AND c.CATEGORY_NAME_ENG <> 'Shop' AND CONTAINS(LOWER(c.OBJECT_NAME),'{Name.ToLower()}')";
            //var data = Context.GetDataByQuery<Facility>("Facilities", query);
            var xx = Context.GetAllData<Facility>();
            if (Name == null)
            {
                var data = from c in xx
                           where c.CATEGORY_NAME_ENG !="Dine" && c.CATEGORY_NAME_ENG != "Taxi" && c.CATEGORY_NAME_ENG != "Hotel" && c.CATEGORY_NAME_ENG != "Shop"
                           select c;
                return data.ToList();

            }
            else
            {
                var data = from c in xx
                           where c.CATEGORY_NAME_ENG != "Dine" && c.CATEGORY_NAME_ENG != "Taxi" && c.CATEGORY_NAME_ENG != "Hotel" && c.CATEGORY_NAME_ENG != "Shop"
                          && c.OBJECT_NAME.ToLower().Contains(Name.ToLower())
                           select c;
                return data.ToList();
            }
        }
        public static List<Luggage> GetLuggages(string Airline, string FlightNo = null)
        {
            //var query = $"SELECT * FROM C WHERE CONTAINS(LOWER(C.LONG_NAME),'{Airline.ToLower()}') AND CONTAINS(LOWER(C.FLIGHT_NUM),'{FlightNo.ToLower()}')";
            //var data = Context.GetDataByQuery<Luggage>("Luggages", query);
            var xx = Context.GetAllData<Luggage>();
            var data = from c in xx
                       where c.LONG_NAME.ToLower().Contains(Airline.ToLower()) && c.FLIGHT_NUM.ToLower().Contains(FlightNo.ToLower())
                       select c;
            return data.ToList();
       
        }
        public static List<APTV> GetAPTV()
        {
            //var query = $"SELECT * FROM C";
            //var data = Context.GetDataByQuery<APTV>("APTV", query);
            var xx = Context.GetAllData<APTV>();
            var data = from c in xx
                    select c;
            return data.ToList();
        }
        public static List<News> GetLatestNews()
        {
            //var query = $"SELECT TOP 10 * FROM C ORDER BY C.ARTICLE_ID DESC";
            //var data = Context.GetDataByQuery<News>("News", query);
            var xx = Context.GetAllData<News>();
            var data = from c in xx
                       orderby c.ARTICLE_ID descending
                       select c;
            return data.Take(10).ToList();
        }
        public static List<Report> GetReportByDate(DateTime StartDate, DateTime EndDate)
        {

            if (EndDate < StartDate)
            {
                var temp = EndDate;
                EndDate = StartDate;
                StartDate = temp;
            }
            //var query = $"SELECT * FROM C WHERE C.REPORT_DATE >= '{StartDate.ToString("yyyy-MM-dd")}' AND C.REPORT_DATE <= '{EndDate.ToString("yyyy-MM-dd")}'";
            //Console.WriteLine(query);
            //var data = Context.GetDataByQuery<Report>("Reports", query);
            //return data;

            var xx = Context.GetAllData<Report>();
            var data = from c in xx
                       where Convert.ToDateTime(c.REPORT_DATE) >= StartDate && Convert.ToDateTime(c.REPORT_DATE) <= EndDate
                       select c;
            return data.ToList();
        }

        public static async Task<bool> InsertComplain(Complain data)
        {

            var hasil = await Context.InsertDoc<Complain>("Complains", data);
            return await Task.FromResult(hasil);
        }

        /*
        static CosmosDB Data;
        public static CosmosDB Context{
            get
            {
                if (Data == null)
                {
                    Data = new CosmosDB();
                }
                return Data;
            }
        }
        #region Data Query
        public static List<Flight> GetFlightByCode(string Code)
        {
            var data = Context.GetDataByQuery<Flight>("Flights",$"SELECT * FROM Flights WHERE Flights.FLIGHT_NO ='{Code}'");
            return data;
        }
        public static List<Flight> GetFlightByAirline(string Airline)
        {
            var data = Context.GetDataByQuery<Flight>("Flights", $"SELECT * FROM Flights WHERE CONTAINS(LOWER(Flights.AIRLINE_NAME),'{Airline.ToLower()}')");
            return data;
        }
        public static List<Facility> GetFacilityByCategory(string Category,string Name=null)
        {
            var query = Name == null ? $"SELECT * FROM C WHERE C.CATEGORY_NAME_ENG = '{Category}' " : $"SELECT * FROM C WHERE C.CATEGORY_NAME_ENG = '{Category}' AND CONTAINS(LOWER(C.OBJECT_NAME),'{Name.ToLower()}')";
            var data = Context.GetDataByQuery<Facility>("Facilities", query);
            return data;
        }
        public static List<Facility> GetImportantNumber(string Name = null)
        {
            var query = Name == null ? $"SELECT * FROM c where c.CATEGORY_NAME_ENG <> 'Dine' AND c.CATEGORY_NAME_ENG <> 'Taxi' AND c.CATEGORY_NAME_ENG <> 'Hotel' AND c.CATEGORY_NAME_ENG <> 'Shop'" : $"SELECT * FROM c where c.CATEGORY_NAME_ENG <> 'Dine' AND c.CATEGORY_NAME_ENG <> 'Taxi' AND c.CATEGORY_NAME_ENG <> 'Hotel' AND c.CATEGORY_NAME_ENG <> 'Shop' AND CONTAINS(LOWER(c.OBJECT_NAME),'{Name.ToLower()}')";
            var data = Context.GetDataByQuery<Facility>("Facilities", query);
            return data;
        }
        public static List<Luggage> GetLuggages(string Airline, string FlightNo = null)
        {
            var query = $"SELECT * FROM C WHERE CONTAINS(LOWER(C.LONG_NAME),'{Airline.ToLower()}') AND CONTAINS(LOWER(C.FLIGHT_NUM),'{FlightNo.ToLower()}')";
            var data = Context.GetDataByQuery<Luggage>("Luggages", query);
            return data;
        }
        public static List<APTV> GetAPTV()
        {
            var query = $"SELECT * FROM C";
            var data = Context.GetDataByQuery<APTV>("APTV", query);
            return data;
        }
        public static List<News> GetLatestNews()
        {
            var query = $"SELECT TOP 10 * FROM C ORDER BY C.ARTICLE_ID DESC";
            var data = Context.GetDataByQuery<News>("News", query);
            return data;
        }
        public static List<Report> GetReportByDate(DateTime StartDate,DateTime EndDate)
        {
            
            if (EndDate < StartDate)
            {
                var temp = EndDate;
                EndDate = StartDate;
                StartDate = temp;
            }
            var query = $"SELECT * FROM C WHERE C.REPORT_DATE >= '{StartDate.ToString("yyyy-MM-dd")}' AND C.REPORT_DATE <= '{EndDate.ToString("yyyy-MM-dd")}'";
            Console.WriteLine(query);
            var data = Context.GetDataByQuery<Report >("Reports", query);
            return data;
        }

        public static async Task<bool> InsertComplain(Complain data)
        {

            var hasil = await Context.InsertDoc<Complain>("Complains",data);
            return await Task.FromResult(hasil);
        }
        */
        //

        #endregion
    }

    #region entities
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
    public class Complain
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }
        public string NoLaporan { set; get; }
        public DateTime TglLaporan { set; get; }
        public string Nama { set; get; }

        public string Telpon { set; get; }

        public string Email { set; get; }

        public TipeLaporan TipeLaporan { set; get; }

        public string Keterangan { set; get; }

        public string Lokasi { set; get; }

        public DateTime Waktu { set; get; }

        public int SkalaPrioritas { set; get; }
    }
    #endregion
}