using ClinicBookingSystem.Application.Interfaces;
using ClinicBookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicBookingSystem.Application.Services
{
    public class PatientRepository : IPatientRepository
    {
        public Task<bool> ExistsAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Doctor>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Doctor?> GetByIdAsync(Guid doctorId)
        {
            throw new NotImplementedException();
        }
    }
}
