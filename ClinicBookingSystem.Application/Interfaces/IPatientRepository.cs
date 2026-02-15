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
        Task<bool> ExistsAsync(Guid id);
        public Task AddAsync(Patient patient);


    }
}
