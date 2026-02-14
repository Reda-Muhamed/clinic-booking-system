using ClinicBookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicBookingSystem.Application.Interfaces
{
    public interface IDoctorRepository
    {
        public Task<Doctor?> GetByIdAsync(Guid doctorId);
        public Task<IReadOnlyList<Doctor>> GetAllAsync();
        Task<bool> ExistsAsync(Guid id);
        Task<(IEnumerable<Doctor> Doctors, int TotalCount)> GetDoctorsWithFilterAsync(
            string? searchTerm,
            string? department,
            int pageNumber,
            int pageSize);

    }
}
