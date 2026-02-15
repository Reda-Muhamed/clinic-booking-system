using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicBookingSystem.Infrastructure.Identity
{
    public class ApplicationUser:IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        
        // if the user is a doctor
        public Guid? DoctorId { get; set; }

        // if the user is a patient
        public Guid? PatientId { get; set; }
    }
}
