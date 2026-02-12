using System.Diagnostics;
using ClinicBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClinicBookingSystem.Controllers
{
    public class HomeController : Controller
    {
        // return the landing page
        public IActionResult Index()
        {
           
            return View();
        }     
    }
}
