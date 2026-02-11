using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicBookingSystem.Domain.Entities
{
    public class Patient
    {
        public Guid Id { get; private set; }
        public string FullName { get; private set; } = null!;
        public DateOnly DateOfBirth { get; private set; }

        private Patient() { }
        public Patient( string fullName, DateOnly dateOfBirth)
        {
            Id = Guid.NewGuid();
            FullName = fullName;
            DateOfBirth = dateOfBirth;
        }
    }
}
