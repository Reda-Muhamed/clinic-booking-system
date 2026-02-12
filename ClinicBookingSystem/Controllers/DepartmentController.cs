using Microsoft.AspNetCore.Mvc;

namespace ClinicBookingSystem.Web.Controllers
{
    public class DepartmentController:Controller
    {
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
