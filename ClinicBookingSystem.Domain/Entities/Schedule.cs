using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicBookingSystem.Domain.Entities
{
    public class Schedule
    {
        public Guid Id { get; private set; }
        public Guid DoctorId { get; private set; }
        public TimeOnly StartTime { get; private set; }
        public TimeOnly EndTime { get; private set; }
        public int SlotDurationInMinutes { get; private set; }
        public DayOfWeek DayOfWeek { get; private set; }
        public Doctor? Doctor { get; private set; }
        private Schedule() { }
        public Schedule(Guid doctorId, TimeOnly startTime, TimeOnly endTime, int slotDurationInMinutes, DayOfWeek dayOfWeek)
        {
            if (endTime <= startTime)
                throw new ArgumentException("End time must be after start time.");

            if (slotDurationInMinutes <= 0)
                throw new ArgumentException("Slot duration must be positive.");

            var duration = endTime - startTime;
            if (duration.TotalMinutes % slotDurationInMinutes != 0)
                throw new ArgumentException($"The shift duration ({duration.TotalMinutes} mins) is not divisible by the slot size ({slotDurationInMinutes} mins).");

            Id = Guid.NewGuid();
            DoctorId = doctorId;
            StartTime = startTime;
            EndTime = endTime;
            SlotDurationInMinutes = slotDurationInMinutes;
            DayOfWeek = dayOfWeek;
        }
    }
}
