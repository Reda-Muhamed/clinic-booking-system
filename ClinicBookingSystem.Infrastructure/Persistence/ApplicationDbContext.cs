using ClinicBookingSystem.Domain.Entities;
using ClinicBookingSystem.Domain.Enums; // Required for Enum conversion
using ClinicBookingSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace ClinicBookingSystem.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Initialize Identity Tables
            base.OnModelCreating(builder);


            builder.Entity<Doctor>().HasQueryFilter(d => !d.IsDeleted);

            // Store Enums as Strings in DB for readability 
            builder.Entity<Appointment>()
                .Property(a => a.Status)
                .HasConversion<string>();


            // Doctor -> Department Relationship
            builder.Entity<Doctor>()
                .HasOne(d => d.Department)
                .WithMany(d => d.Doctors)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Appointment -> Doctor Relationship
            builder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Appointment -> Patient Relationship
            builder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Schedule -> Doctor Relationship
            builder.Entity<Schedule>()
                .HasOne(s => s.Doctor)
                .WithMany(d => d.Schedules)
                .HasForeignKey(s => s.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Doctor>()
                .Property(d => d.Price)
                .HasColumnType("decimal(18,2)");
        }
    }
}