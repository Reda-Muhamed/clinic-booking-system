using ClinicBookingSystem.Application.Common;
using ClinicBookingSystem.Application.Dtos;
using ClinicBookingSystem.Application.Interfaces;
using ClinicBookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicBookingSystem.Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDepartmentRepository _departmentRepository;
        // We need this to check if doctors are assigned before deleting
        private readonly IDoctorRepository _doctorRepository;

        public DepartmentService(
            IUnitOfWork unitOfWork,
            IDepartmentRepository departmentRepository,
            IDoctorRepository doctorRepository)
        {
            _unitOfWork = unitOfWork;
            _departmentRepository = departmentRepository;
            _doctorRepository = doctorRepository;
        }

        public async Task<Result<IEnumerable<DepartmentDto>>> GetAllDepartmentsAsync()
        {
            var departments = await _departmentRepository.GetAllAsync();

            var dtos = departments
                .Select(d => new DepartmentDto { Id = d.Id, Name = d.Name })
                .ToList();

            return Result<IEnumerable<DepartmentDto>>.Success(dtos);
        }

        public async Task<Result<DepartmentDto>> GetDepartmentByIdAsync(Guid id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);

            if (department is null)
                return Result<DepartmentDto>.Failure("Department not found.");

            return Result<DepartmentDto>.Success(new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name
            });
        }

        public async Task<Result<Guid>> CreateDepartmentAsync(CreateDepartmentDto dto)
        {
            bool exists = await _departmentRepository.ExistsByNameAsync(dto.Name);
            if (exists)
                return Result<Guid>.Failure($"Department '{dto.Name}' already exists.");

            var department = new Department(dto.Name);

            await _departmentRepository.AddAsync(department);
            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Success(department.Id);
        }

        public async Task<Result> UpdateDepartmentAsync(UpdateDepartmentDto dto)
        {
            var department = await _departmentRepository.GetByIdAsync(dto.Id);

            if (department is null)
                return Result.Failure("Department not found.");

            
            department.UpdateName(dto.Name); 
            
            await _departmentRepository.UpdateAsync(department);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> DeleteDepartmentAsync(Guid id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department is null)
                return Result.Failure("Department not found.");

            
            bool hasDoctors = await _doctorRepository.HasDoctorsInDepartmentAsync(id);

            if (hasDoctors)
            {
                return Result.Failure("Cannot delete this department because there are doctors assigned to it. Please reassign or remove the doctors first.");
            }

            await _departmentRepository.DeleteAsync(department);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
    }
}