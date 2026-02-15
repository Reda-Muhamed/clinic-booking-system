using ClinicBookingSystem.Application.Interfaces;
using ClinicBookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicBookingSystem.Application.Services
{
    public class DepartmentRepository : IDepartmentRepository
    {
        public Task AddAsync(Department department)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Department department)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Department>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Department?> GetByIdAsync(Guid departmentId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Department department)
        {
            throw new NotImplementedException();
        }
    }
}
