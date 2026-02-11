using ClinicBookingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicBookingSystem.Application.Dtos
{
    public class AppointmentDetailsDto
    {
        public Guid Id { get; set; }

        public string? PatientName { get; set; }
        public string? DoctorName { get; set; }
        public string? DepartmentName { get; set; }

        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }

        public AppointmentStatus Status { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
