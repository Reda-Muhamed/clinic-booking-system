using ClinicBookingSystem.Application.Common;
using ClinicBookingSystem.Application.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicBookingSystem.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result>RegisterPatientAsync(RegisterPatientDto dto);
        Task<Result>RegisterDoctorAsync(RegisterDoctorDto dto);
        Task<Result>LoginAsync(LoginDto dto);
        Task LogoutAsync();


    }
}
