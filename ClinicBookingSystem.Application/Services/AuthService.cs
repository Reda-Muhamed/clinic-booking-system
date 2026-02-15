using ClinicBookingSystem.Application.Common;
using ClinicBookingSystem.Application.Dtos.Auth;
using ClinicBookingSystem.Application.Interfaces;
using ClinicBookingSystem.Domain.Entities;
using ClinicBookingSystem.Domain.Enums;
using ClinicBookingSystem.Infrastructure.Identity; 
using Microsoft.AspNetCore.Identity;

namespace ClinicBookingSystem.Application.Services 
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IDepartmentRepository _departmentRepository; 
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IPatientRepository patientRepository,
            IDoctorRepository doctorRepository,
            IDepartmentRepository departmentRepository,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _departmentRepository = departmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null)
                return Result.Failure("Invalid email or password");

            //  they are banned for 5 mins if the fail 5 times
            var result = await _signInManager.PasswordSignInAsync(user, dto.Password, dto.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded) return Result.Success();
            if (result.IsLockedOut) return Result.Failure("Account is locked due to too many failed attempts.");

            return Result.Failure("Invalid email or password.");
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<Result> RegisterDoctorAsync(RegisterDoctorDto dto)
        {
            // Validate Department 
            bool deptExists = await _departmentRepository.ExistsAsync(dto.DepartmentId);
            if (!deptExists)
                return Result.Failure("The selected department does not exist.");

            var user = new ApplicationUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = dto.Email,
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result.Failure($"Failed to register: {errors}");
            }

            //Create Domain Entity with Rollback Safety
            try
            {
                await EnsureRoleExistsAsync(UserRole.Doctor.ToString());
                await _userManager.AddToRoleAsync(user, UserRole.Doctor.ToString());

                var doctor = new Doctor($"{dto.FirstName} {dto.LastName}", dto.DepartmentId, dto.ConsultationFee);

                await _doctorRepository.AddAsync(doctor);
                await _unitOfWork.SaveChangesAsync();

                user.DoctorId = doctor.Id;
                await _userManager.UpdateAsync(user);
                await _signInManager.SignInAsync(user, isPersistent: false);

            }
            catch (Exception)
            {
                
                await _userManager.DeleteAsync(user);
                return Result.Failure("An error occurred while creating the doctor profile. Registration cancelled.");
            }

            return Result.Success();
        }

        public async Task<Result> RegisterPatientAsync(RegisterPatientDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.Phone,
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result.Failure($"Failed to register: {errors}");
            }

            try
            {
                await EnsureRoleExistsAsync(UserRole.Patient.ToString());
                await _userManager.AddToRoleAsync(user, UserRole.Patient.ToString());

                var patient = new Patient($"{dto.FirstName} {dto.LastName}", dto.DateOfBirth);
                await _patientRepository.AddAsync(patient);
                await _unitOfWork.SaveChangesAsync();

                user.PatientId = patient.Id;
                await _userManager.UpdateAsync(user);

                await _signInManager.SignInAsync(user, isPersistent: false);
            }
            catch (Exception)
            {
                await _userManager.DeleteAsync(user);
                return Result.Failure("An error occurred while saving patient data.");
            }

            return Result.Success();
        }

        private async Task EnsureRoleExistsAsync(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}