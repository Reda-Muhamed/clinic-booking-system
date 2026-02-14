using ClinicBookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicBookingSystem.Application.Interfaces
{
    public interface IAppointmentRepository
    {
        public Task AddAsync(Appointment appointment);
        public Task<Appointment?> GetByIdAsync(Guid appointmentId);
        Task<Appointment?> GetByIdWithDetailsAsync(Guid appointmentId);
        Task<bool> HasAppointmentsInScheduleAsync(Guid scheduleId);
        Task<bool> HasActiveAppointmentsAsync(Guid doctorId);
        public Task<IReadOnlyList<Appointment>> GetByPatientIdAsync(Guid patientId);
        public Task<IReadOnlyList<Appointment>> GetByDoctorAndDateAsync(Guid doctorId, DateOnly date);
        public Task<IReadOnlyList<Appointment>> GetByDoctorIdAsync(Guid doctorId);
        public Task UpdateAsync(Appointment appointment);

    }
}
