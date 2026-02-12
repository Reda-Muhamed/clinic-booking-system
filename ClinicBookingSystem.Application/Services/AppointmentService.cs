using ClinicBookingSystem.Application.Common;
using ClinicBookingSystem.Application.Dtos;
using ClinicBookingSystem.Application.Interfaces;
using ClinicBookingSystem.Domain.Entities;
using ClinicBookingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicBookingSystem.Application.Services
{
    public class AppointmentService : IAppointmentCommandService, IAppointmentQueryService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IAppointmentRepository appointmentRepository;
        private readonly IDoctorRepository doctorRepository;
        private readonly IPatientRepository patientRepository;
        private readonly IScheduleRepository scheduleRepository;

        public AppointmentService(IUnitOfWork unitOfWork,IAppointmentRepository appointmentRepository,IDoctorRepository doctorRepository , IPatientRepository patientRepository,IScheduleRepository scheduleRepository)
        {
            this.unitOfWork = unitOfWork;
            this.appointmentRepository = appointmentRepository;
            this.doctorRepository = doctorRepository;
            this.patientRepository = patientRepository;
            this.scheduleRepository = scheduleRepository;
        }

        public async Task<Result> RequestAppointmentAsync(Guid patientId, Guid doctorId, DateTimeOffset startTime)
        {
            if (startTime <= DateTimeOffset.UtcNow)
                return Result.Failure("Appointment time must be in the future.");

            var date = DateOnly.FromDateTime(startTime.UtcDateTime);

            var doctor = await doctorRepository.GetByIdAsync(doctorId);
            if (doctor is null)
                return Result.Failure("Doctor not found.");

            var patient = await patientRepository.GetByIdAsync(patientId);
            if (patient is null)
                return Result.Failure("Patient not found.");

            var schedules = await scheduleRepository.GetByDoctorIdAsync(doctorId);

            var schedule = schedules
                .FirstOrDefault(s => s?.DayOfWeek == startTime.DayOfWeek);

            if (schedule is null)
                return Result.Failure("Doctor is not working on this day.");


            var startTimeOfDay = TimeOnly.FromDateTime(startTime.UtcDateTime);
            var endTimeOfDay = startTimeOfDay.AddMinutes(schedule.SlotDurationInMinutes);

            if (startTimeOfDay < schedule.StartTime ||
                endTimeOfDay > schedule.EndTime)
                return Result.Failure("Selected time is outside working hours.");


            var minutesFromStart = (startTimeOfDay.ToTimeSpan() - schedule.StartTime.ToTimeSpan()).TotalMinutes;

            if (minutesFromStart % schedule.SlotDurationInMinutes != 0)
                return Result.Failure("Invalid time slot selection.");

            var endTime = startTime.AddMinutes(schedule.SlotDurationInMinutes);

            var existingAppointments =
            await appointmentRepository.GetByDoctorAndDateAsync(doctorId, date);
            // check overlaps ... if appointment exist 9 -> 10, and i need to reserve 9:10 -> 9:50
            foreach (var existing in existingAppointments)
            {
                if (existing.Status is AppointmentStatus.Cancelled ||
                    existing.Status is AppointmentStatus.Rejected)
                    continue;

                var overlap =
                    startTime < existing.EndTime &&
                    endTime > existing.StartTime;

                if (overlap)
                    return Result.Failure("This time slot is already reserved.");
            }
            var appointment = new Appointment(patientId, doctorId, startTime, endTime);

            try
            {
                await appointmentRepository.AddAsymc(appointment);
                await unitOfWork.SaveChangesAsync(); 
            }
            catch (DbException )
            {
                // Concurrency safety 
                return Result.Failure("This time slot was just reserved. Please try another.");
            }

            return Result.Success();

        }



        public async Task<Result> ApproveAppointmentAsync(Guid appointmentId , Guid doctorId)
        {
            var appointment = await appointmentRepository.GetByIdAsync(appointmentId);
            
            if (appointment is null)
            {
                return Result.Failure("The appointment does not exist");
            }
            if (appointment.DoctorId != doctorId)
                return Result.Failure("You are not allowed to approve this appointment.");

            //  the status of it was checked internally in the Approve()
           
            try
            {
                appointment.Approve();
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(ex.Message);
            }
            await unitOfWork.SaveChangesAsync();
            return Result.Success();
        }


        public async Task<Result> CancelAppointmentAsync(Guid appointmentId,Guid actorId, UserRole role)
        {
            var appointment = await appointmentRepository.GetByIdAsync(appointmentId);

            if (appointment is null)
                return Result.Failure("The appointment does not exist.");

            var isAuthorized = role switch
            {
                UserRole.Patient => appointment.PatientId == actorId,
                UserRole.Doctor => appointment.DoctorId == actorId,
                UserRole.Admin => true,
                _ => false
            };

            if (!isAuthorized)
                return Result.Failure("You are not allowed to cancel this appointment.");

            try
            {
                appointment.Cancel();
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(ex.Message);
            }

            await unitOfWork.SaveChangesAsync();

            return Result.Success();
        }


        public async Task<Result> CompleteAppointmentAsync ( Guid appointmentId,Guid actorId,UserRole role)
        {
            var appointment = await appointmentRepository.GetByIdAsync(appointmentId);

            if (appointment is null)
                return Result.Failure("The appointment does not exist.");

            var isAuthorized = role switch
            {
                UserRole.Doctor => appointment.DoctorId == actorId,
                UserRole.Admin => true,
                _ => false
            };

            if (!isAuthorized)
                return Result.Failure("You are not allowed to complete this appointment.");

            try
            {
                appointment.Complete();
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(ex.Message);
            }

            await unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> RejectAppointmentAsync(Guid appointmentId,Guid actorId,UserRole role)
        {
            var appointment = await appointmentRepository.GetByIdAsync(appointmentId);

            if (appointment is null)
                return Result.Failure("The appointment does not exist.");

            var isAuthorized = role switch
            {
                UserRole.Doctor => appointment.DoctorId == actorId,
                UserRole.Admin => true,
                _ => false
            };

            if (!isAuthorized)
                return Result.Failure("You are not allowed to reject this appointment.");

            try
            {
                appointment.Reject();
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(ex.Message);
            }

            await unitOfWork.SaveChangesAsync();

            return Result.Success();
        }


        public async Task<Result<AppointmentDto>> GetAppointmentByIdAsync(Guid appointmentId)
        {
            var appointment = await appointmentRepository.GetByIdWithDetailsAsync(appointmentId);

            if (appointment is null)
                return Result<AppointmentDto>.Failure("The appointment does not exist.");
            // get the doctor and patient name 
            
            var dto = MapToDto(appointment);

            return Result<AppointmentDto>.Success(dto);
        }




        public async Task<Result<IReadOnlyList<AppointmentDto>>>GetAppointmentsByDoctorIdAsync(Guid doctorId)
        {
            // validate the doctor 
            var doctor = await doctorRepository.GetByIdAsync(doctorId);
            if (doctor is null)
                return Result<IReadOnlyList<AppointmentDto>>.Failure("The doctor was not found");
            var appointments = await appointmentRepository.GetByDoctorIdAsync(doctorId);

            var result = appointments
                .Select(a => MapToDto(a))
                .ToList()
                .AsReadOnly();

            return Result<IReadOnlyList<AppointmentDto>>.Success(result);
        }


        public async Task<Result<IReadOnlyList<AppointmentDto>>> GetAppointmentsByPatientIdAsync(Guid patientId)
        {
            // validate the patient
            var patient = await patientRepository.GetByIdAsync(patientId);
            if (patient is null)
                return Result<IReadOnlyList<AppointmentDto>>.Failure("The Patient was not found");
            var appointments = await appointmentRepository.GetByPatientIdAsync(patientId);

            var result = appointments
                .Select(a => MapToDto(a))
                .ToList()
                .AsReadOnly();

            return Result<IReadOnlyList<AppointmentDto>>.Success(result);

        }

        public async Task<Result<IReadOnlyList<AvailableTimeSlotDto>>>GetAvailableTimeSlotsAsync(Guid doctorId, DateOnly date)
        {
            var doctor = await doctorRepository.GetByIdAsync(doctorId);
            if (doctor is null)
                return Result<IReadOnlyList<AvailableTimeSlotDto>>.Failure("Doctor not found.");

            var dayOfWeek = date.DayOfWeek;

            // Load schedules
            var schedules = await scheduleRepository.GetByDoctorIdAsync(doctorId);

            var schedule = schedules
                .FirstOrDefault(s => s.DayOfWeek == dayOfWeek);

            if (schedule is null)
                return Result<IReadOnlyList<AvailableTimeSlotDto>>
                    .Success(new List<AvailableTimeSlotDto>());

            var slots = new List<AvailableTimeSlotDto>();

            var current = schedule.StartTime;

            while (current.AddMinutes(schedule.SlotDurationInMinutes) <= schedule.EndTime)
            {
                var start = new DateTimeOffset(
                    date.ToDateTime(current),
                    TimeSpan.Zero);
                var end = start.AddMinutes(schedule.SlotDurationInMinutes);

                slots.Add(new AvailableTimeSlotDto
                {
                    StartTime = start,
                    EndTime = end
                });

                current = current.AddMinutes(schedule.SlotDurationInMinutes);
            }

            //Load existing appointments
            var appointments = await appointmentRepository
                .GetByDoctorAndDateAsync(doctorId, date);

            //Remove reserved slots
            var reserved = appointments
                .Where(a => a.Status == AppointmentStatus.Requested ||
                            a.Status == AppointmentStatus.Approved)
                .Select(a => a.StartTime)
                .ToHashSet();

            var availableSlots = slots
                .Where(s => !reserved.Contains(s.StartTime))
                .ToList();

            return Result<IReadOnlyList<AvailableTimeSlotDto>>
                .Success(availableSlots);
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
                DoctorName = appointment?.Doctor?.FullName,
                PatientName = appointment?.Patient?.FullName,
            };
        }

    }
}
