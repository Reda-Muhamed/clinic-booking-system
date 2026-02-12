using ClinicBookingSystem.Application.Common;
using ClinicBookingSystem.Domain.Enums;


namespace ClinicBookingSystem.Application.Interfaces
{
    public interface IAppointmentCommandService
    {
        public Task<Result> RequestAppointmentAsync(Guid patientId, Guid doctorId, DateTimeOffset startTime);
        public Task<Result> ApproveAppointmentAsync(Guid appointmentId,Guid doctorId);
        public Task<Result> RejectAppointmentAsync(Guid appointmentId, Guid actorId, UserRole role);
        public Task<Result> CancelAppointmentAsync(Guid appointmentId , Guid actorId , UserRole userRole);
        public Task<Result> CompleteAppointmentAsync(Guid appointmentId, Guid actorId, UserRole role);

    }
}
