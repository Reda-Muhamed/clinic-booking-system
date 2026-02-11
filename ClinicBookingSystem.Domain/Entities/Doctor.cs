using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicBookingSystem.Domain.Entities
{
    public class Doctor
    {
        public Guid Id { get; private set; }

        public string FullName { get; private set; } = null!;

        public Guid DepartmentId { get; private set; }

        private Doctor() { }

        public Doctor(string fullName, Guid departmentId)
        {
            Id = Guid.NewGuid();
            FullName = fullName;
            DepartmentId = departmentId;
        }
    }
}
