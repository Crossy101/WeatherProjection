using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WeatherProjection.Models
{
    public class WeatherDevice
    {
        public string Id { get; set; }
        [Required]
        [DisplayName("Device Name")]
        public string Name { get; set; }
        [Required]
        [DisplayName("Device Location")]
        public string Location { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime InstalledDate { get; set; }
        public DateTime LatestWeatherUpdate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime NextMaintenanceDate { get; set; }
        public bool ActiveDevice { get; set; }

        //Key used to send information about this device
        public string APIKey { get; set; }

        public WeatherDevice()
        {
            //Create a new Device ID and API Key for communicating with the device
            this.Id = Guid.NewGuid().ToString();
            this.APIKey = Guid.NewGuid().ToString();

            //Create time stamps for the device so we know when certain events happened
            this.InstalledDate = DateTime.Now;
            this.LatestWeatherUpdate = DateTime.Now;
            this.NextMaintenanceDate = DateTime.Now.AddYears(1);

            //Set the device active state
            this.ActiveDevice = true;
        }
    }
}