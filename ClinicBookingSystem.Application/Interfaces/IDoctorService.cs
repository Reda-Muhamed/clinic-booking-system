using ClinicBookingSystem.Application.Common;
using ClinicBookingSystem.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicBookingSystem.Application.Interfaces
{
    public interface IDoctorService
    {
        public Task<Result<PaginatedList<DoctorDto>>> GetAllDoctorsAsync(string? searchTerm,
            string? department,
            int pageNumber = 1,
            int pageSize = 10);
        public Task<Result<DoctorDto>> GetDoctorByIdAsync(Guid id);
    }
}
