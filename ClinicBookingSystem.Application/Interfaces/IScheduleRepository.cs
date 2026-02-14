using ClinicBookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicBookingSystem.Application.Interfaces
{
    public interface IScheduleRepository
    {
        public Task<IReadOnlyList<Schedule>> GetByDoctorIdAndDateAsync(Guid doctorId, DateOnly date);
        public Task<IReadOnlyList<Schedule>> GetByDoctorIdAsync(Guid doctorId);
        public Task<Schedule?> GetByIdAsync(Guid id);
        Task<IEnumerable<Schedule>> GetSchedulesByDoctorAndDayAsync(Guid doctorId, DayOfWeek day);
        public Task AddAsync(Schedule schedule);
        public Task DeleteAsync(Schedule schedule);
        public Task UpdateAsync(Schedule schedule);
    }
}
