using Microsoft.AspNetCore.Mvc;

namespace ClinicBookingSystem.Web.Controllers
{
    public class DoctorController:Controller
    {
        // list all doctor
        public IActionResult Index()
        {
            return View();
        }
        // list details of doctor 
        public IActionResult Details()
        {
            return View();
        }
        // Dashboard for doctor using appointments
        public IActionResult Dasboard()
        {
            return View();
        }


    }
}
