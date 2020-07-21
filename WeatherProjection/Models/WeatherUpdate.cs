using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeatherProjection.Models
{
    public class WeatherUpdate
    {
        public string Id { get; set; }
        public string ParentDeviceID { get; set; }

        //DH-11 Sensor Values
        public float Temperature { get; set; }
        public float Humidity { get; set; }
        public float HeatIndex { get; set; }
        public float DewPoint { get; set; }

        //MQ-2 Sensor Values
        public float LPG { get; set; }
        public float CarbonEmissions { get; set; }
        public float Smoke { get; set; }

        public DateTime TimeUpdateSent { get; set; }

        public WeatherUpdate()
        {
            //Creat a new Id for the weather update and attach a time stamp
            this.Id = Guid.NewGuid().ToString();
            this.TimeUpdateSent = DateTime.Now;
        }

        //This function takes the current temp and humditity and calculates the dew point
        public void CalculateDewPoint()
        {
            this.DewPoint = Temperature - ((100 - Humidity) / 5);
        }
    }
}