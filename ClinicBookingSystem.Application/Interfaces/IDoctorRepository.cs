using ClinicBookingSystem.Domain.Entities;
namespace ClinicBookingSystem.Application.Interfaces
{
    public interface IDoctorRepository
    {
        public Task<Doctor?> GetByIdAsync(Guid doctorId);
        public Task<IReadOnlyList<Doctor>> GetAllAsync();
        public Task AddAsync(Doctor doctor);
        public Task<bool> HasDoctorsInDepartmentAsync(Guid departmentId);

        public Task UpdateAsync(Doctor doctor);
        Task<bool> ExistsAsync(Guid id);
        Task<(IEnumerable<Doctor> Doctors, int TotalCount)> GetDoctorsWithFilterAsync(
            string? searchTerm,
            string? department,
            int pageNumber,
            int pageSize);

    }
}
