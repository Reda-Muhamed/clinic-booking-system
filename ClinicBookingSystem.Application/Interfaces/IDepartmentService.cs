using ClinicBookingSystem.Application.Common;
using ClinicBookingSystem.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicBookingSystem.Application.Interfaces
{
    public interface IDepartmentService
    {
        Task<Result<IEnumerable<DepartmentDto>>> GetAllDepartmentsAsync();
        Task<Result<DepartmentDto>> GetDepartmentByIdAsync(Guid id);
        Task<Result<Guid>> CreateDepartmentAsync(CreateDepartmentDto dto);
        Task<Result> UpdateDepartmentAsync(UpdateDepartmentDto dto);
        Task<Result> DeleteDepartmentAsync(Guid id);
    }
}
