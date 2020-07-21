using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeatherProjection.Models;

namespace WeatherProjection.Controllers
{
    public class WeatherDeviceController : Controller
    {
        private int maxWeatherDevicesCollected = 10;

        private ApplicationDbContext _context;

        // GET: WeatherDevice
        public ActionResult Index()
        {
            using (_context = new ApplicationDbContext())
            {
                List<WeatherDevice> allWeatherDevices = _context.WeatherDevices.Take(maxWeatherDevicesCollected).ToList();

                return View(allWeatherDevices);
            }
        }

        // GET: Create
        [Authorize]
        public ActionResult Create()
        {
            WeatherDevice device = new WeatherDevice();

            return View(device);
        }

        // POST: Create
        [HttpPost]
        public ActionResult Create(WeatherDevice device)
        {
            using (_context = new ApplicationDbContext())
            {
                //Add weather device to the database and save
                _context.WeatherDevices.Add(device);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
        }

        // GET: Details
        public ActionResult Details(string Id, WeatherDisplay displayType = 0)
        {
            using (_context = new ApplicationDbContext())
            {
                //Find the weather device in the database
                WeatherDevice weatherDevice = _context.WeatherDevices.First(wd => wd.Id == Id);

                //If the device has not been found
                if (weatherDevice == null)
                    weatherDevice = new WeatherDevice();

                List<WeatherUpdate> weatherUpdates = null;
                DateTime displayTime = DateTime.Now;

                //Get the weather updates that correlates with the current Date and Time the user selects
                switch (displayType)
                {
                    case WeatherDisplay.OneHour:
                        displayTime = DateTime.Now.AddHours(-1);
                        weatherUpdates = _context.WeatherUpdates.Where(wu => wu.TimeUpdateSent >= displayTime && wu.ParentDeviceID == Id).ToList();
                        break;
                    case WeatherDisplay.SixHours:
                        displayTime = DateTime.Now.AddHours(-6);
                        weatherUpdates = _context.WeatherUpdates.Where(wu => wu.TimeUpdateSent >= displayTime && wu.ParentDeviceID == Id).ToList();
                        break;
                    case WeatherDisplay.TwentyFourHours:
                        displayTime = DateTime.Now.AddDays(-1);
                        weatherUpdates = _context.WeatherUpdates.Where(wu => wu.TimeUpdateSent >= displayTime && wu.ParentDeviceID == Id).ToList();
                        break;
                    case WeatherDisplay.PastSevenDays:
                        displayTime = DateTime.Now.AddDays(-7);
                        weatherUpdates = _context.WeatherUpdates.Where(wu => wu.TimeUpdateSent >= displayTime && wu.ParentDeviceID == Id).ToList();
                        break;
                    case WeatherDisplay.PastThirtyDays:
                        displayTime = DateTime.Now.AddDays(-30);
                        weatherUpdates = _context.WeatherUpdates.Where(wu => wu.TimeUpdateSent >= displayTime && wu.ParentDeviceID == Id).ToList();
                        break;
                    default:
                        break;
                }

                weatherUpdates = weatherUpdates.OrderBy(time => time.TimeUpdateSent.TimeOfDay).ToList();

                DetailsWeatherDevice currentView = new DetailsWeatherDevice
                {
                    CurrentDevice = weatherDevice,
                    WeatherUpdates = weatherUpdates
                };

                return View(currentView);
            }
        }

        // GET: Edit
        [Authorize]
        public ActionResult Edit(string Id)
        {
            using (_context = new ApplicationDbContext())
            {
                WeatherDevice weatherDevice = _context.WeatherDevices.First(wd => wd.Id == Id);

                if (weatherDevice == null)
                    return HttpNotFound();

                return View(weatherDevice);
            }
        }

        // POST: Edit
        [HttpPost]
        public ActionResult Edit(WeatherDevice weatherDevice)
        {
            using (_context = new ApplicationDbContext())
            {
                var deviceToEdit = _context.WeatherDevices.First(wd => wd.Id == weatherDevice.Id);

                if (deviceToEdit == null)
                    return HttpNotFound();

                deviceToEdit.Name = weatherDevice.Name;
                deviceToEdit.Location = weatherDevice.Location;
                deviceToEdit.LatestWeatherUpdate = weatherDevice.LatestWeatherUpdate;
                deviceToEdit.NextMaintenanceDate = weatherDevice.NextMaintenanceDate;
                deviceToEdit.ActiveDevice = weatherDevice.ActiveDevice;
                deviceToEdit.APIKey = weatherDevice.APIKey;

                _context.SaveChanges();

                return RedirectToAction("Index");
            }
        }

        // POST: Search
        [HttpPost]
        public ActionResult Search(WeatherDeviceSearch search)
        {
            using (_context = new ApplicationDbContext())
            {
                List<WeatherDevice> searchedDevices;
                if (string.IsNullOrEmpty(search.Id) && string.IsNullOrEmpty(search.Name) && string.IsNullOrEmpty(search.Location))
                {
                    searchedDevices = _context.WeatherDevices.ToList();
                }
                else
                {
                    searchedDevices = _context.WeatherDevices.Where(wd => wd.Id.Contains(search.Id) || wd.Name.Contains(search.Name ?? string.Empty) || wd.Location.Contains(search.Location ?? string.Empty)).ToList();
                }

                return View("Index", searchedDevices);
            }
        }

        // POST: ChangeAPIKey
        public ActionResult ChangeDeviceAPIKey(string Id)
        {
            using(_context = new ApplicationDbContext())
            {
                //Get the device to change the APIKey
                WeatherDevice deviceToChange = _context.WeatherDevices.FirstOrDefault(wd => wd.Id == Id);

                //If the device doesn't exist
                if (deviceToChange == null)
                    return HttpNotFound();

                //Get all current weather updates from the database
                List<WeatherUpdate> deviceUpdates = _context.WeatherUpdates.Where(wu => wu.ParentDeviceID == deviceToChange.Id).ToList();

                //Create a new APIKey and replace it with the current device APIKey
                string newAPIKey = GetNewDeviceAPIKey();
                deviceToChange.APIKey = newAPIKey;

                //Save the changes to the database
                _context.SaveChanges();

                //Redirect the user back to edit page
                return RedirectToAction("Edit", new { Id = deviceToChange.Id });
            }
        }

        //This function will create a new device APIKey
        public string GetNewDeviceAPIKey()
        {
            //Intialise vairables
            string newApiKey = "";
            bool duplicateKey = true;
            using(_context = new ApplicationDbContext())
            {
                //While the database has not found a new key for the device
                while(duplicateKey)
                {
                    newApiKey = Guid.NewGuid().ToString();
                    WeatherDevice deviceSearch = _context.WeatherDevices.FirstOrDefault(wd => wd.APIKey == newApiKey);

                    if (deviceSearch != null)
                        continue;

                    break;
                }
            }

            //Return the new api key that was created
            return newApiKey;
        }
    }

    public enum WeatherDisplay : int
    {
        OneHour = 0,
        SixHours,
        TwentyFourHours,
        PastSevenDays,
        PastThirtyDays
    }
}