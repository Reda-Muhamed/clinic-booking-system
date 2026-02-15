using ClinicBookingSystem.Application.Common;
using ClinicBookingSystem.Application.Dtos;
using ClinicBookingSystem.Application.Interfaces;
using ClinicBookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ClinicBookingSystem.Application.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IDoctorRepository _doctorRepository;

        public DoctorService(IAppointmentRepository appointmentRepository,IUnitOfWork unitOfWork,IDepartmentRepository departmentRepository,IDoctorRepository doctorRepository)
        {
            this._appointmentRepository = appointmentRepository;
            this._unitOfWork = unitOfWork;
            this._departmentRepository = departmentRepository;
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
        private static DoctorDto MapToDto(Doctor doctor )
        {
            return new DoctorDto
            {
                Id = doctor.Id,
                ConsultationFee = doctor.Price,
                DepartmentName = doctor.Department.Name,
                FullName = doctor.FullName,
                ImageUrl = doctor.ImageUrl

            };
        }

        public async Task<Result<Guid>> CreateDoctorAsync(CreateDoctorDto dto)
        {
            var department = await _departmentRepository.GetByIdAsync(dto.DepartmentId);
            if (department is null)
                return Result<Guid>.Failure("Department not found.");


            var doctor = new Doctor(
                dto.FullName,
                dto.DepartmentId,
                dto.Price);

            doctor.UpdateProfile(dto.FullName, dto.Description, dto.ImageUrl, dto.Price);

            await _doctorRepository.AddAsync(doctor);
            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Success(doctor.Id);
        }

        public async Task<Result> UpdateDoctorAsync(UpdateDoctorDto dto)
        {
            // valide the existense of the doctor
            var doctor = await _doctorRepository.GetByIdAsync(dto.Id);
            if (doctor is null)
                return Result.Failure("Doctor not fount.");

            doctor.UpdateProfile(dto.FullName, dto.Description, dto.ImageUrl, dto.Price);

            if(dto.DepatmentId != dto.DepatmentId)
            {
                // validate the department
                var deptExists = await _departmentRepository.ExistsAsync(dto.DepatmentId);
                if (!deptExists) return Result.Failure("New Department does not exist.");
                doctor.ChageDepartment(dto.DepatmentId);
            }
            await _doctorRepository.UpdateAsync(doctor);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();


        }

        public async Task<Result> DeleteDoctorAsync(Guid doctorId)
        {
            var doctor = await _doctorRepository.GetByIdAsync(doctorId);
            if (doctor is null || doctor.IsDeleted)
                return Result.Failure("Doctor not found");
            var hasFutureAppointments = await _appointmentRepository.HasActiveAppointmentsAsync(doctorId);
            if (hasFutureAppointments)
                return Result.Failure("Cannot delete doctor. They have upcoming appointments that must be cancelled or reassigned first.");
            doctor.Delete();
            await _doctorRepository.UpdateAsync(doctor);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
    }
}
