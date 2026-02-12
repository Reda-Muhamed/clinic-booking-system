using ClinicBookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicBookingSystem.Application.Interfaces
{
    public interface IPatientRepository
    {
        public Task<Doctor?> GetByIdAsync(Guid doctorId);
        public Task<IReadOnlyList<Doctor>> GetAllAsync();

    }
}
