using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DTO
{
    public class StaffDTO
    {
        public string StaffId { get; set; } = null!;

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-ZÀ-ỹ\s]+$", ErrorMessage = "Full name cannot contain special characters or numbers.")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        [AgeLimit(18, ErrorMessage = "Staff must be at least 18 years old.")]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must start with 0 and have 10 digits.")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Citizen ID is required.")]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "Citizen ID must have 12 digits.")]
        public string? CitizenId { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters.")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Hire date is required.")]
        [HireDateValid(ErrorMessage = "Hire date cannot be in the future or before the date of birth.")]
        public DateTime? HireDate { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public string? Status { get; set; }

        // --- Authentication fields ---
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Username must be 3–30 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_.-]+$", ErrorMessage = "Username cannot contain special characters.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "Confirm password does not match.")]
        public string? ConfirmPassword { get; set; }

        // --- Avatar upload ---
        public IFormFile? Avatar { get; set; } // file upload
        public string? AvatarUrl { get; set; } // đường dẫn lưu DB

        public DateTime? CreatedAt { get; set; }

        public List<string>? Roles { get; set; }

        public string? RoleName { get; set; }
    }

    public class AgeLimitAttribute : ValidationAttribute
    {
        private readonly int _minAge;

        public AgeLimitAttribute(int minAge)
        {
            _minAge = minAge;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dob)
            {
                var today = DateTime.Today;
                int age = today.Year - dob.Year;
                if (dob.Date > today.AddYears(-age)) age--;

                if (age < _minAge)
                    return new ValidationResult($"Staff must be at least {_minAge} years old.");
            }

            return ValidationResult.Success;
        }
    }

    public class HireDateValidAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime hireDate)
            {
                var staff = (StaffDTO)validationContext.ObjectInstance;
                var today = DateTime.Today;

                if (hireDate.Date > today)
                    return new ValidationResult("Hire date cannot be in the future.");

                if (staff.DateOfBirth.HasValue && hireDate.Date < staff.DateOfBirth.Value.Date)
                    return new ValidationResult("Hire date cannot be earlier than the date of birth.");
            }

            return ValidationResult.Success;
        }
    }
}
