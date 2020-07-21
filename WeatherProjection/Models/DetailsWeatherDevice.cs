using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeatherProjection.Models
{
    public class DetailsWeatherDevice
    {
        public WeatherDevice CurrentDevice { get; set; }
        public List<WeatherUpdate> WeatherUpdates { get; set; }
    }
}