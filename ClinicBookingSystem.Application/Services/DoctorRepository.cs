using ClinicBookingSystem.Application.Interfaces;
using ClinicBookingSystem.Domain.Entities;
using ClinicBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicBookingSystem.Infrastructure.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly ApplicationDbContext _context;

        public DoctorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Doctor doctor)
        {
            await _context.Doctors.AddAsync(doctor);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Doctors.AnyAsync(d => d.Id == id);
        }

        public async Task<IReadOnlyList<Doctor>> GetAllAsync()
        {
            return await _context.Doctors
                .Include(d => d.Department)
                .ToListAsync();
        }

        public async Task<Doctor?> GetByIdAsync(Guid doctorId)
        {
            // the query filter auto-hide the deleted doctors
            return await _context.Doctors
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.Id == doctorId);
        }

        public async Task<(IEnumerable<Doctor> Doctors, int TotalCount)> GetDoctorsWithFilterAsync(
            string? searchTerm,
            string? departmentName,
            int pageNumber,
            int pageSize)
        {
            var query = _context.Doctors
                .Include(d => d.Department)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(d => d.FullName.Contains(searchTerm,StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(departmentName))
            {
                query = query.Where(d => d.Department.Name.Contains(departmentName, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = await query.CountAsync();

            var doctors = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (doctors, totalCount);
        }

        public async Task<bool> HasDoctorsInDepartmentAsync(Guid departmentId)
        {
            return await _context.Doctors.AnyAsync(d => d.DepartmentId == departmentId);
        }

        public Task UpdateAsync(Doctor doctor)
        {
            _context.Doctors.Update(doctor);
            return Task.CompletedTask;
        }
    }
}