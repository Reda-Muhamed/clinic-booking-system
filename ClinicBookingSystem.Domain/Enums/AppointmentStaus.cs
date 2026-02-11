using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicBookingSystem.Domain.Enums
{
    public enum AppointmentStatus
    {
        Requested = 0,
        Approved = 1,
        Rejected = 2,
        Cancelled = 3,
        Completed = 4
    }
}
