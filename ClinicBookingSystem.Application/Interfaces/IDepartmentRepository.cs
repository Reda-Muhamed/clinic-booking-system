using ClinicBookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicBookingSystem.Application.Interfaces
{
    public interface IDepartmentRepository
    {
       public Task<IReadOnlyList<Department>> GetAllAsync();
       public Task<Department?> GetByIdAsync(Guid departmentId);
       public Task AddAsync(Department department);
       public Task DeleteAsync(Department department);
       public Task<bool> ExistsAsync(Guid id);

    }
}
