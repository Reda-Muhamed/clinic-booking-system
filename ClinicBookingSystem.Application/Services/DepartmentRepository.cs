using ClinicBookingSystem.Application.Interfaces;
using ClinicBookingSystem.Domain.Entities;
using ClinicBookingSystem.Infrastructure.Persistence; 
using Microsoft.EntityFrameworkCore; 

namespace ClinicBookingSystem.Infrastructure.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _context;

        public DepartmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Department department)
        {
            await _context.Departments.AddAsync(department);
        }

        public Task DeleteAsync(Department department)
        {
            _context.Departments.Remove(department);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Departments.AnyAsync(d => d.Id == id);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Departments.AnyAsync(d => d.Name == name);
        }

        public async Task<IReadOnlyList<Department>> GetAllAsync()
        {
            var departments = await _context.Departments
                .AsNoTracking() 
                .ToListAsync();

            return departments.AsReadOnly();
        }

        public async Task<Department?> GetByIdAsync(Guid departmentId)
        {
            return await _context.Departments.FindAsync(departmentId);
        }

        public Task UpdateAsync(Department department)
        {
            _context.Departments.Update(department);
            return Task.CompletedTask;
        }
    }
}