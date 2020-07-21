using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeatherProjection.Models;

namespace WeatherProjection.Controllers.Api
{
    public class WeatherDeviceController : ApiController
    {
        private ApplicationDbContext _context;

        [HttpPost]
        public bool WeatherUpdate(WeatherUpdateAPI weatherUpdate)
        {
            using (_context = new ApplicationDbContext())
            {
                //Get device that sent the request with the current APIKey
                if (!ModelState.IsValid)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);

                WeatherDevice currentDevice = _context.WeatherDevices.First(wd => wd.APIKey == weatherUpdate.APIKey);

                //Check if the device is currently null
                if (currentDevice == null)
                    return false;

                //Create a new weather update with the associated values sent to the server
                WeatherUpdate newWeatherUpdate = new WeatherUpdate
                {
                    ParentDeviceID = currentDevice.Id,
                    Temperature = weatherUpdate.Temperature,
                    Humidity = weatherUpdate.Humidity,
                    HeatIndex = weatherUpdate.HeatIndex,
                    LPG = weatherUpdate.LPG,
                    CarbonEmissions = weatherUpdate.CarbonEmissions,
                    Smoke = weatherUpdate.Smoke
                };
                newWeatherUpdate.CalculateDewPoint();
                currentDevice.LatestWeatherUpdate = DateTime.Now;


                //Add the new weather update to the Database and Save
                _context.WeatherUpdates.Add(newWeatherUpdate);
                _context.SaveChanges();

                return true;
            }
        }
    }
}
