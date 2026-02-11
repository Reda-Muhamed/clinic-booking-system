using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicBookingSystem.Domain.Entities
{
    public class Department
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        private Department() { }
        public Department(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }
    }
}
