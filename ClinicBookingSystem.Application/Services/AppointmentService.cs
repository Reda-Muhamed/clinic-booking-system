using ClinicBookingSystem.Application.Common;
using ClinicBookingSystem.Application.Dtos;
using ClinicBookingSystem.Application.Interfaces;
using ClinicBookingSystem.Domain.Entities;
using ClinicBookingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

public class AppointmentService : IAppointmentCommandService, IAppointmentQueryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IScheduleRepository _scheduleRepository;

    public AppointmentService(
        IUnitOfWork unitOfWork,
        IAppointmentRepository appointmentRepository,
        IDoctorRepository doctorRepository,
        IPatientRepository patientRepository,
        IScheduleRepository scheduleRepository)
    {
        _unitOfWork = unitOfWork;
        _appointmentRepository = appointmentRepository;
        _doctorRepository = doctorRepository;
        _patientRepository = patientRepository;
        _scheduleRepository = scheduleRepository;
    }

    public async Task<Result> RequestAppointmentAsync(Guid patientId, Guid doctorId, DateTimeOffset startTime)
    {
        if (startTime <= DateTimeOffset.UtcNow)
            return Result.Failure("Appointment time must be in the future.");

       
        if (!await _doctorRepository.ExistsAsync(doctorId))
            return Result.Failure("Doctor not found.");

        if (!await _patientRepository.ExistsAsync(patientId))
            return Result.Failure("Patient not found.");

        
        var dayOfWeek = startTime.DayOfWeek;
        var daySchedules = await _scheduleRepository.GetSchedulesByDoctorAndDayAsync(doctorId, dayOfWeek);

        if (!daySchedules.Any())
            return Result.Failure("Doctor is not working on this day.");

        var startTimeOfDay = TimeOnly.FromTimeSpan(startTime.TimeOfDay);

        var matchingSchedule = daySchedules.FirstOrDefault(s =>
            startTimeOfDay >= s.StartTime &&
            startTimeOfDay.AddMinutes(s.SlotDurationInMinutes) <= s.EndTime);

        if (matchingSchedule is null)
            return Result.Failure("Selected time is outside working hours.");

        // 4. Validate Slot Alignment (Modulus Check)
        var timeFromStart = startTimeOfDay - matchingSchedule.StartTime;
        if (timeFromStart.TotalMinutes % matchingSchedule.SlotDurationInMinutes != 0)
            return Result.Failure($"Appointments must align with the {matchingSchedule.SlotDurationInMinutes}-minute slots.");

        var endTime = startTime.AddMinutes(matchingSchedule.SlotDurationInMinutes);
        var appointmentDate = DateOnly.FromDateTime(startTime.Date);

        var existingAppointments = await _appointmentRepository.GetByDoctorAndDateAsync(doctorId, appointmentDate);

        bool isOverlapping = existingAppointments.Any(existing =>
            existing.Status != AppointmentStatus.Cancelled &&
            existing.Status != AppointmentStatus.Rejected &&
            startTime < existing.EndTime &&
            endTime > existing.StartTime); 

        if (isOverlapping)
            return Result.Failure("This time slot is already reserved.");

        var appointment = new Appointment(patientId, doctorId, startTime, endTime);

        try
        {
            await _appointmentRepository.AddAsync(appointment);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbException)
        {
            return Result.Failure("This time slot was just reserved by another patient. Please try again.");
        }

        return Result.Success();
    }

    public async Task<Result<IReadOnlyList<AvailableTimeSlotDto>>> GetAvailableTimeSlotsAsync(Guid doctorId, DateOnly date)
    {
        if (!await _doctorRepository.ExistsAsync(doctorId))
            return Result<IReadOnlyList<AvailableTimeSlotDto>>.Failure("Doctor not found.");

        var daySchedules = await _scheduleRepository.GetSchedulesByDoctorAndDayAsync(doctorId, date.DayOfWeek);

        if (!daySchedules.Any())
            return Result<IReadOnlyList<AvailableTimeSlotDto>>.Success(new List<AvailableTimeSlotDto>());

        var appointments = await _appointmentRepository.GetByDoctorAndDateAsync(doctorId, date);

        // list of the busy intervales
        var busyIntervals = appointments
            .Where(a => a.Status != AppointmentStatus.Cancelled && a.Status != AppointmentStatus.Rejected)
            .Select(a => new { Start = a.StartTime, End = a.EndTime })
            .ToList();

        var availableSlots = new List<AvailableTimeSlotDto>();

        foreach (var schedule in daySchedules)
        {
            var currentSlotStart = schedule.StartTime;

            while (currentSlotStart.AddMinutes(schedule.SlotDurationInMinutes) <= schedule.EndTime)
            {
                
                var slotStartDto = new DateTimeOffset(date.ToDateTime(currentSlotStart), TimeSpan.Zero);
                var slotEndDto = slotStartDto.AddMinutes(schedule.SlotDurationInMinutes);

                // Check Overlap against Busy Intervals
                bool isTaken = busyIntervals.Any(busy =>
                    slotStartDto < busy.End && slotEndDto > busy.Start);

                if (!isTaken)
                {
                    availableSlots.Add(new AvailableTimeSlotDto
                    {
                        StartTime = slotStartDto,
                        EndTime = slotEndDto
                    });
                }

                currentSlotStart = currentSlotStart.AddMinutes(schedule.SlotDurationInMinutes);
            }
        }

        return Result<IReadOnlyList<AvailableTimeSlotDto>>.Success(availableSlots);
    }


    public async Task<Result> ApproveAppointmentAsync(Guid appointmentId, Guid doctorId)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
        if (appointment is null) return Result.Failure("Appointment not found.");

        if (appointment.DoctorId != doctorId)
            return Result.Failure("Unauthorized.");

        try
        {
            appointment.Approve();
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (InvalidOperationException ex) { return Result.Failure(ex.Message); }
    }

    public async Task<Result> CancelAppointmentAsync(Guid appointmentId, Guid actorId, UserRole role)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
        if (appointment is null) return Result.Failure("Appointment not found.");

        bool isAuthorized = role switch
        {
            UserRole.Patient => appointment.PatientId == actorId,
            UserRole.Doctor => appointment.DoctorId == actorId,
            UserRole.Admin => true,
            _ => false
        };

        if (!isAuthorized) return Result.Failure("Unauthorized.");

        try
        {
            appointment.Cancel();
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (InvalidOperationException ex) { return Result.Failure(ex.Message); }
    }

    public async Task<Result> CompleteAppointmentAsync(Guid appointmentId, Guid actorId, UserRole role)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
        if (appointment is null) return Result.Failure("Appointment not found.");

        if (role != UserRole.Admin && appointment.DoctorId != actorId)
            return Result.Failure("Unauthorized.");

        try
        {
            appointment.Complete();
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (InvalidOperationException ex) { return Result.Failure(ex.Message); }
    }

    public async Task<Result> RejectAppointmentAsync(Guid appointmentId, Guid actorId, UserRole role)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
        if (appointment is null) return Result.Failure("Appointment not found.");

        if (role != UserRole.Admin && appointment.DoctorId != actorId)
            return Result.Failure("Unauthorized.");

        try
        {
            appointment.Reject();
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (InvalidOperationException ex) { return Result.Failure(ex.Message); }
    }

    public async Task<Result<AppointmentDto>> GetAppointmentByIdAsync(Guid appointmentId)
    {
        var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(appointmentId);
        if (appointment is null) return Result<AppointmentDto>.Failure("Not found.");
        return Result<AppointmentDto>.Success(MapToDto(appointment));
    }

    public async Task<Result<IReadOnlyList<AppointmentDto>>> GetAppointmentsByDoctorIdAsync(Guid doctorId)
    {
        if (!await _doctorRepository.ExistsAsync(doctorId))
            return Result<IReadOnlyList<AppointmentDto>>.Failure("Doctor not found.");

        var appointments = await _appointmentRepository.GetByDoctorIdAsync(doctorId);
        return Result<IReadOnlyList<AppointmentDto>>.Success(appointments.Select(MapToDto).ToList().AsReadOnly());
    }

    public async Task<Result<IReadOnlyList<AppointmentDto>>> GetAppointmentsByPatientIdAsync(Guid patientId)
    {
        if (!await _patientRepository.ExistsAsync(patientId))
            return Result<IReadOnlyList<AppointmentDto>>.Failure("Patient not found.");

        var appointments = await _appointmentRepository.GetByPatientIdAsync(patientId);
        return Result<IReadOnlyList<AppointmentDto>>.Success(appointments.Select(MapToDto).ToList().AsReadOnly());
    }

    private static AppointmentDto MapToDto(Appointment appointment)
    {
        return new AppointmentDto
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId,
            StartTime = appointment.StartTime,
            EndTime = appointment.EndTime,
            Status = appointment.Status,
            DoctorName = appointment.Doctor?.FullName,
            PatientName = appointment.Patient?.FullName 
        };
    }
}