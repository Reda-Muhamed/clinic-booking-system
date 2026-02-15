using ClinicBookingSystem.Application.Interfaces;
using ClinicBookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicBookingSystem.Application.Services
{
    public class ScheduleRepository : IScheduleRepository
    {
        public Task AddAsync(Schedule schedule)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Schedule schedule)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Schedule>> GetByDoctorIdAndDateAsync(Guid doctorId, DateOnly date)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Schedule>> GetByDoctorIdAsync(Guid doctorId)
        {
            throw new NotImplementedException();
        }

        public Task<Schedule?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Schedule>> GetSchedulesByDoctorAndDayAsync(Guid doctorId, DayOfWeek day)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Schedule schedule)
        {
            throw new NotImplementedException();
        }
    }
}
