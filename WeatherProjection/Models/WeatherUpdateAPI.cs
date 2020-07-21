using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeatherProjection.Models
{
    public class WeatherUpdateAPI
    {
        public string APIKey { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }
        public float HeatIndex { get; set; }

        public float LPG { get; set; }
        public float CarbonEmissions { get; set; }
        public float Smoke { get; set; }
    }
}