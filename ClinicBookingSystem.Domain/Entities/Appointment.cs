using ClinicBookingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicBookingSystem.Domain.Entities
{
    public class Appointment
    {
        public Guid Id { get; private set; }

        public Guid PatientId { get; private set; }
        public Patient? Patient { get; private set; }

        public Guid DoctorId { get; private set; }
        public Doctor? Doctor { get; private set; }
        public DateTimeOffset StartTime { get; private set; }
        public DateTimeOffset EndTime { get; private set; }


        public AppointmentStatus Status { get; private set; }

        public DateTimeOffset CreatedAt { get;  private set; }

        private Appointment() { }

        public Appointment(Guid patientId, Guid doctorId ,DateTimeOffset startTime,DateTimeOffset endTime)
        {
            if(endTime <= startTime)
                throw new ArgumentException("End time must be greater than start time.");
            
            Id = Guid.NewGuid();
            PatientId = patientId;
            DoctorId = doctorId;
            Status = AppointmentStatus.Requested;
            CreatedAt = DateTimeOffset.UtcNow;
            StartTime = startTime;
            EndTime = endTime;
        }

        public void Approve()
        {
            if (Status != AppointmentStatus.Requested)
                throw new InvalidOperationException("Only requested appointments can be approved.");

            Status = AppointmentStatus.Approved;
        }

        public void Reject()
        {
            if (Status != AppointmentStatus.Requested)
                throw new InvalidOperationException("Only requested appointments can be rejected.");

            Status = AppointmentStatus.Rejected;
        }

        public void Cancel()
        {
            if (Status == AppointmentStatus.Completed)
                throw new InvalidOperationException("Completed appointment cannot be cancelled.");
            if(Status == AppointmentStatus.Cancelled)
                throw new InvalidOperationException("Appointment is already cancelled.");
            if(Status == AppointmentStatus.Rejected)
                throw new InvalidOperationException("Rejected appointment cannot be cancelled.");
            Status = AppointmentStatus.Cancelled;
        }

        public void Complete()
        {
            if (Status != AppointmentStatus.Approved)
                throw new InvalidOperationException("Only approved appointments can be completed.");

            Status = AppointmentStatus.Completed;
        }
       
    }
}
