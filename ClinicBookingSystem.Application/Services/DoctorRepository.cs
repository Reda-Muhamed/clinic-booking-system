using ClinicBookingSystem.Application.Interfaces;
using ClinicBookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicBookingSystem.Application.Services
{
    public class DoctorRepository : IDoctorRepository
    {
        public Task AddAsync(Doctor doctor)
        {
            throw new NotImplementedException();
        }

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

        public Task<(IEnumerable<Doctor> Doctors, int TotalCount)> GetDoctorsWithFilterAsync(string? searchTerm, string? department, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasDoctorsInDepartmentAsync(Guid departmentId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Doctor doctor)
        {
            throw new NotImplementedException();
        }
    }
}
