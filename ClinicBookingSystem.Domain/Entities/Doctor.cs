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

        public decimal Price { get; private set; }
        public string? ImageUrl { get; private set; }

        public string? Description { get; private set; }
        public bool  IsDeleted { get; private set; } = false;
        public Guid DepartmentId { get; private set; }
        public Department? Department { get; private set; }
        public ICollection<Schedule> Schedules { get; private set; } = new List<Schedule>();
        public ICollection<Appointment> Appointments { get; private set; } = new List<Appointment>();
        private Doctor() { }

        public Doctor(string fullName, Guid departmentId, decimal price)
        {
            Id = Guid.NewGuid();
            FullName = fullName;
            DepartmentId = departmentId;
            Price = price;
        }
        public void UpdateProfile(string fullName, string description, string imageUrl, decimal price)
        {
            FullName= fullName;
            Description= description;
            ImageUrl = imageUrl;
            Price = price;


        }
        public void ChageDepartment(Guid newId)
        {
            DepartmentId = newId;
        }
        public void Delete()
        {
            IsDeleted = true;
        }
    }

}
