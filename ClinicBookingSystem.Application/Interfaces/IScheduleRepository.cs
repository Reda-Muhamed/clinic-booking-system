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
        public Task<IReadOnlyList<Schedule?>> GetByDoctorIdAndDateAsync(Guid doctorId, DateOnly date);
        public Task<IReadOnlyList<Schedule?>> GetByDoctorIdAsync(Guid doctorId);
        public Task AddAsync(Schedule schedule);
        public Task UpdateAsync(Schedule schedule);
    }
}
