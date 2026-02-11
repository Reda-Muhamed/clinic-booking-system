using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicBookingSystem.Domain.Entities
{
    public class TimeSlot
    {
        public Guid DoctorId { get; private set; }
        public DateTimeOffset StartTime { get; private set; }
        public DateTimeOffset EndTime { get; private set; }

        
        private TimeSlot() { }
        public TimeSlot(Guid doctorId, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            if (endTime <= startTime)
                throw new ArgumentException("End time must be greater than start time.");
            DoctorId = doctorId;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
