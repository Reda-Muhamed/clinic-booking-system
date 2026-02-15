using ClinicBookingSystem.Application.Interfaces;
using ClinicBookingSystem.Domain.Entities;
using ClinicBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace ClinicBookingSystem.Infrastructure.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly ApplicationDbContext _context;

        public ScheduleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Schedule schedule)
        {
            await _context.Schedules.AddAsync(schedule);
        }

        public Task DeleteAsync(Schedule schedule)
        {
            _context.Schedules.Remove(schedule);
            return Task.CompletedTask;
        }

        public async Task<IReadOnlyList<Schedule>> GetByDoctorIdAndDateAsync(Guid doctorId, DateOnly date)
        {
            
            var dayOfWeek = date.DayOfWeek;

            return await _context.Schedules
                .Where(s => s.DoctorId == doctorId && s.DayOfWeek == dayOfWeek)
                .OrderBy(s => s.StartTime) 
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Schedule>> GetByDoctorIdAsync(Guid doctorId)
        {
            return await _context.Schedules
                .Where(s => s.DoctorId == doctorId)
                .OrderBy(s => s.DayOfWeek)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<Schedule?> GetByIdAsync(Guid id)
        {
            return await _context.Schedules.FindAsync(id);
        }

        public async Task<IEnumerable<Schedule>> GetSchedulesByDoctorAndDayAsync(Guid doctorId, DayOfWeek day)
        {
            return await _context.Schedules
                .Where(s => s.DoctorId == doctorId && s.DayOfWeek == day)
                .ToListAsync();
        }

        public Task UpdateAsync(Schedule schedule)
        {
            _context.Schedules.Update(schedule);
            return Task.CompletedTask;
        }
    }
}