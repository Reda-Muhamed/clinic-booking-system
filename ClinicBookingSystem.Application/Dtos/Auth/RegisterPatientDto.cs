using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ClinicBookingSystem.Application.Dtos.Auth
{
    public class RegisterPatientDto
    {
        [Required,EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required,MinLength(8)]
        public string Password { get; set; }= string.Empty;
        [Required,Compare("Password")]
        public string ConfirmPassword { get; set; }=string.Empty;
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public DateOnly DateOfBirth { get; set; }
        [Phone]
        public string Phone { get; set; } = string.Empty;
    }
}
