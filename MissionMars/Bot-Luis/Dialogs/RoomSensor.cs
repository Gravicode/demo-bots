using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bot_Luis
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


    public class CctvObject
    {
        public int id { get; set; }
        public string camName { get; set; }
        public string description { get; set; }
        public DateTime tanggal { get; set; }
        public string imageUrl { get; set; }
    }

}