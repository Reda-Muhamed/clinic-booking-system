using ClinicBookingSystem.Application.Common;
using ClinicBookingSystem.Application.Dtos;
using ClinicBookingSystem.Application.Interfaces;
using ClinicBookingSystem.Domain.Entities;
using ClinicBookingSystem.Domain.Enums;

namespace ClinicBookingSystem.Application.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public ScheduleService(
            IUnitOfWork unitOfWork,
            IScheduleRepository scheduleRepository,
            IDoctorRepository doctorRepository,
            IAppointmentRepository appointmentRepository)
        {
            _unitOfWork = unitOfWork;
            _scheduleRepository = scheduleRepository;
            _doctorRepository = doctorRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<Result> CreateScheduleAsync(CreateScheduleDto dto, Guid actorId, Guid doctorId, UserRole role)
        {
            if (dto is null)
                return Result.Failure("Input data is required.");

            var isAuthorized = role switch
            {
                UserRole.Admin => true,
                UserRole.Doctor => actorId == doctorId,
                _ => false
            };

            if (!isAuthorized)
                return Result.Failure("You are not allowed to create a schedule for this doctor.");

            // ensure the time window is divisible by the slot duration
            var totalMinutes = (dto.EndTime - dto.StartTime).TotalMinutes;
            if (totalMinutes % dto.SlotDuration != 0)
            {
                return Result.Failure($"The time range ({totalMinutes} mins) is not perfectly divisible by the slot duration ({dto.SlotDuration} mins).");
            }

            // 3. Doctor Existence
            var doctorExists = await _doctorRepository.ExistsAsync(doctorId); 
            if (!doctorExists)
                return Result.Failure("Doctor not found.");

            
            var daySchedules = await _scheduleRepository.GetSchedulesByDoctorAndDayAsync(doctorId, dto.DayOfWeek);

            foreach (var existing in daySchedules)
            {
                if (dto.StartTime < existing.EndTime && dto.EndTime > existing.StartTime)
                {
                    return Result.Failure($"Schedule overlaps with an existing block on {dto.DayOfWeek}.");
                }
            }

            try
            {
                var newSchedule = new Schedule(
                    doctorId,
                    dto.StartTime,
                    dto.EndTime,
                    dto.SlotDuration,
                    dto.DayOfWeek);

                await _scheduleRepository.AddAsync(newSchedule);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success();
            }
            catch (ArgumentException ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result> DeleteScheduleAsync(Guid scheduleId, Guid actorId, UserRole role)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);

            if (schedule is null)
                return Result.Failure("Schedule not found.");

            var isAuthorized = role switch
            {
                UserRole.Admin => true,
                UserRole.Doctor => schedule.DoctorId == actorId,
                _ => false
            };

            if (!isAuthorized)
                return Result.Failure("You are not allowed to delete this schedule.");

            
            // You cannot delete a schedule if patients have already booked slots in it
            bool hasAppointments = await _appointmentRepository.HasAppointmentsInScheduleAsync(scheduleId);
            if (hasAppointments)
            {
                return Result.Failure("Cannot delete this schedule because there are active appointments booked. Cancel the appointments first.");
            }

            await _scheduleRepository.DeleteAsync(schedule);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<IReadOnlyList<ScheduleDto>>> GetSchedulesByDoctorAsync(Guid doctorId)
        {
            var doctorExists = await _doctorRepository.ExistsAsync(doctorId);
            if (!doctorExists)
                return Result<IReadOnlyList<ScheduleDto>>.Failure("Doctor does not exist.");

            var schedules = await _scheduleRepository.GetByDoctorIdAsync(doctorId);

            var result = schedules
                .Select(MapToDto)
                .ToList()
                .AsReadOnly();

            return Result<IReadOnlyList<ScheduleDto>>.Success(result);
        }

        private static ScheduleDto MapToDto(Schedule schedule)
        {
            return new ScheduleDto
            {
                Id = schedule.Id,
                DoctorId = schedule.DoctorId,
                DayOfWeek = schedule.DayOfWeek,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                SlotDuration = schedule.SlotDurationInMinutes
            };
        }
    }
}