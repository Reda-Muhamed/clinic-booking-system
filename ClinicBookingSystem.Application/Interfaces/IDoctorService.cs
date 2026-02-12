using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicBookingSystem.Application.Interfaces
{
    public interface IDoctorService
    {
        public Task GetAllDoctorsAsync();
        public Task GetDoctorByIdAsync();
    }
}
