using ClinicBookingSystem.Application.Common;
using ClinicBookingSystem.Application.Dtos;
using ClinicBookingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicBookingSystem.Application.Interfaces
{
    public interface IScheduleService
    {
        public Task<Result> CreateScheduleAsync(CreateScheduleDto dto, Guid actorId, Guid doctorId, UserRole role);
        public Task<Result> DeleteScheduleAsync(Guid scheduleId, Guid actorId, UserRole role);
        public Task<Result<IReadOnlyList<ScheduleDto>>> GetSchedulesByDoctorAsync(Guid doctorId);
    }
}
