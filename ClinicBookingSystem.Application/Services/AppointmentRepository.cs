using ClinicBookingSystem.Application.Interfaces;
using ClinicBookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicBookingSystem.Application.Services
{
    public class AppointmentRepository : IAppointmentRepository
    {
        public Task AddAsync(Appointment appointment)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Appointment>> GetByDoctorAndDateAsync(Guid doctorId, DateOnly date)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Appointment>> GetByDoctorIdAsync(Guid doctorId)
        {
            throw new NotImplementedException();
        }

        public Task<Appointment?> GetByIdAsync(Guid appointmentId)
        {
            throw new NotImplementedException();
        }

        public Task<Appointment?> GetByIdWithDetailsAsync(Guid appointmentId)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Appointment>> GetByPatientIdAsync(Guid patientId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasActiveAppointmentsAsync(Guid doctorId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasAppointmentsInScheduleAsync(Guid scheduleId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Appointment appointment)
        {
            throw new NotImplementedException();
        }
    }
}
