using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ClinicBookingSystem.Application.Dtos.Auth
{
    public class RegisterDoctorDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public Guid DepartmentId { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal ConsultationFee { get; set; }
    }
}
