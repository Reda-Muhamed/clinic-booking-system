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
            var totalMinutes = (endTime - startTime).TotalMinutes;
            if (endTime <= startTime || slotDurationInMinutes <= 0 || totalMinutes % slotDurationInMinutes != 0)
                throw new ArgumentException("End time must be greater than start time and the duration must be divisible by slot duration.");
            Id = Guid.NewGuid();
            DoctorId = doctorId;
            StartTime = startTime;
            EndTime = endTime;
            SlotDurationInMinutes = slotDurationInMinutes;
            DayOfWeek = dayOfWeek;
        }
    }
}
