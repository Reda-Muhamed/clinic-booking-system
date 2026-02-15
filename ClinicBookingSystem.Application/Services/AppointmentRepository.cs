using ClinicBookingSystem.Application.Interfaces;
using ClinicBookingSystem.Domain.Entities;
using ClinicBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace ClinicBookingSystem.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
        }

        public async Task<IReadOnlyList<Appointment>> GetByDoctorAndDateAsync(Guid doctorId, DateOnly date)
        {
            var startOfDay = date.ToDateTime(TimeOnly.MinValue);
            var endOfDay = startOfDay.AddDays(1);

            var appointments = await _context.Appointments
                .Where(a => a.DoctorId == doctorId &&
                            a.StartTime >= startOfDay &&
                            a.StartTime < endOfDay)
                .ToListAsync(); 

            return appointments.AsReadOnly();
        }

        public async Task<IReadOnlyList<Appointment>> GetByDoctorIdAsync(Guid doctorId)
        {
            var appointments = await _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .ToListAsync();

            return appointments.AsReadOnly();
        }

        public async Task<Appointment?> GetByIdAsync(Guid appointmentId)
        {
            return await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == appointmentId);
        }

        public async Task<Appointment?> GetByIdWithDetailsAsync(Guid appointmentId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);
        }

        public async Task<IReadOnlyList<Appointment>> GetByPatientIdAsync(Guid patientId)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.StartTime) 
                .ToListAsync();

            return appointments.AsReadOnly();
        }

        public async Task<bool> HasActiveAppointmentsAsync(Guid doctorId)
        {
            return await _context.Appointments
                .AnyAsync(a => a.DoctorId == doctorId &&
                               a.StartTime > DateTime.UtcNow &&
                               a.Status != Domain.Enums.AppointmentStatus.Cancelled);
        }

        public async Task<bool> HasAppointmentsInScheduleAsync(Guid scheduleId)
        {
            var schedule = await _context.Schedules.FindAsync(scheduleId);
            if (schedule == null) return false;

            return await _context.Appointments
                .AnyAsync(a => a.DoctorId == schedule.DoctorId &&
                                (int)a.StartTime.DayOfWeek == (int)schedule.DayOfWeek &&
                                
                               a.Status != Domain.Enums.AppointmentStatus.Cancelled &&
                               a.StartTime.TimeOfDay < schedule.EndTime.ToTimeSpan() &&
                               a.EndTime.TimeOfDay > schedule.StartTime.ToTimeSpan());
        }

        public Task UpdateAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            return Task.CompletedTask;
        }
    }
}