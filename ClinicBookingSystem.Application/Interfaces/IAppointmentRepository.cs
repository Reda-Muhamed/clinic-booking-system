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
        public Task AddAsymc(Appointment appointment);
        public Task<Appointment?> GetByIdAsync(Guid appointmentId);
        public Task<IReadOnlyList<Appointment>> GetByPatientIdAsync(Guid patientId);
        public Task<IReadOnlyList<Appointment>> GetDoctorAppointmentsByDateAsync(Guid doctorId, DateOnly date);
        public Task UpdateAsync(Appointment appointment);

    }
}
