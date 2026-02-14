using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicBookingSystem.Application.Dtos
{
    public class DoctorDto
    {
        public Guid Id {set;get;}
        public string FullName {set;get;}
        public string DepartmentName {set;get;}
        public string ImageUrl {set;get;}
        public decimal ConsultationFee {set;get;}

    }
}
