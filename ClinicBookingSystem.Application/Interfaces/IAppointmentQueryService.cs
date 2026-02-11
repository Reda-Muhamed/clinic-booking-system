using ClinicBookingSystem.Application.Common;
using ClinicBookingSystem.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicBookingSystem.Application.Interfaces
{
    public interface IAppointmentQueryService
    {
        public Task<Result<AppointmentDto>> GetAppointmentByIdAsync(Guid appointmentId);
        public Task<Result<IReadOnlyList<AppointmentDto>>> GetAppointmentsByPatientIdAsync(Guid patientId);
        public Task<Result<IReadOnlyList<AppointmentDto>>> GetAppointmentsByDoctorIdAsync(Guid doctorId);
        public Task<Result<IReadOnlyList<AvailableTimeSlotDto>>> GetAvailableTimeSlotsAsync(Guid doctorId , DateOnly date);
    }
}
