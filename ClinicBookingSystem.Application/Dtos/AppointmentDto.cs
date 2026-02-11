using ClinicBookingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicBookingSystem.Application.Dtos
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }
        public string? PatientName { get; set; }

        public Guid DoctorId { get; set; }
        public string? DoctorName { get; set; }

        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }

        public AppointmentStatus Status { get; set; }
        
    }
}
