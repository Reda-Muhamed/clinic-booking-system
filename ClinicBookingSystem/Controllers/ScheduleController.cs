using Microsoft.AspNetCore.Mvc;

namespace ClinicBookingSystem.Web.Controllers
{
    public class ScheduleController:Controller
    {
        // doctor schedule
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Delete()
        {
            return View();
        }

        

    }
}
