using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicBookingSystem.Application.Dtos
{
    public class CreateScheduleDto
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly  StartTime{ get; set; }
        public TimeOnly  EndTime{ get; set; }
        public int SlotDuration{ get; set; }

    }
}
