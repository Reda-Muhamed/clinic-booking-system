using Microsoft.AspNetCore.Mvc;

namespace ClinicBookingSystem.Web.Controllers
{
    public class AppointmentController:Controller
    {
        // show the Available Slot
        public IActionResult Book()
        {
            return View();
        }
        public IActionResult Request()
        {
            return View();
        }
        public IActionResult MyAppointments()
        {
            return View();
        }
        public IActionResult Cancel()
        {
            return View();
        }
        public IActionResult Approve()
        {
            return View();
        }
        public IActionResult Reject()
        {
            return View();
        }
        public IActionResult Complete()
        {
            return View();
        }


    }
}
