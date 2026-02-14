using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicBookingSystem.Application.Dtos
{
    public class CreateDoctorDto
    {
        public string FullName { get; set; }
        public Guid DepartmentId { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}
