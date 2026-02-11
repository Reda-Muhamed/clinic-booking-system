using ClinicBookingSystem.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicBookingSystem.Application.Interfaces
{
    public interface IAppointmentCommandService
    {
        public Task<Result> RequestAppointmentAsync(Guid patientId, Guid doctorId, DateTimeOffset startTime);
        public Task<Result> ApproveAppointmentAsync(Guid appointmentId);
        public Task<Result> RejectAppointmentAsync(Guid appointmentId);
        public Task<Result> CancelAppointmentAsync(Guid appointmentId);
        public Task<Result> CompleteAppointmentAsync(Guid appointmentId);

    }
}
