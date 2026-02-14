using ClinicBookingSystem.Application.Common;
using ClinicBookingSystem.Application.Dtos;
using ClinicBookingSystem.Application.Interfaces;
using ClinicBookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicBookingSystem.Application.Services
{
    public class DoctorServices : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;

        public DoctorServices(IDoctorRepository doctorRepository)
        {
            this._doctorRepository = doctorRepository;
        }
        public async Task<Result<PaginatedList<DoctorDto>>> GetAllDoctorsAsync(
            string?searchTerm,
            string?department,
            int pageNumber = 1,
            int pageSize = 10)
        {
            if(pageNumber <= 1) 
                pageNumber = 1;
            if(pageSize < 1)
                pageSize = 10;
            var (doctors, totalCount) = await _doctorRepository.GetDoctorsWithFilterAsync(
                 searchTerm,
                 department,
                 pageNumber,
                 pageSize );
            var doctorDtos = doctors.Select(MapToDto).ToList().AsReadOnly();
            var result = new PaginatedList<DoctorDto>(doctorDtos , totalCount , pageNumber , pageSize);
            return Result<PaginatedList<DoctorDto>>.Success(result);
        }

        public async Task<Result<DoctorDto>> GetDoctorByIdAsync(Guid docId)
        {
            var doctor = await _doctorRepository.GetByIdAsync(docId);
            if(doctor is null)
               return Result<DoctorDto>.Failure("Doctor not found.");
            return Result<DoctorDto>.Success(MapToDto(doctor));

            
        }
        private static DoctorDto MapToDto(Doctor doctor)
        {
            return new DoctorDto
            {
                Id = doctor.Id,
                ConsultationFee = doctor.Price,
                DepartmentName = doctor.DepartmentName,
                FullName = doctor.FullName,
                ImageUrl = doctor.ImageUrl

            };
        }
    }
}
