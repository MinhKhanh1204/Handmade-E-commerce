using System;
using System.ComponentModel.DataAnnotations;

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
        public DateOnly? DateOfBirth { get; set; }

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
        public DateOnly? HireDate { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public string? Status { get; set; }

        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public DateTime? CreatedAt { get; set; }
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
            if (value is DateOnly dob)
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                int age = today.Year - dob.Year;
                if (dob.AddYears(age) > today)
                    age--;

                if (age < _minAge)
                {
                    return new ValidationResult($"Staff must be at least {_minAge} years old.");
                }
            }

            return ValidationResult.Success;
        }
    }

    public class HireDateValidAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not DateOnly hireDate)
                return ValidationResult.Success;

            var staff = (StaffDTO)validationContext.ObjectInstance;

            var today = DateOnly.FromDateTime(DateTime.Today);
            if (hireDate > today)
                return new ValidationResult("Hire date cannot be in the future.");

            if (staff.DateOfBirth.HasValue && hireDate < staff.DateOfBirth.Value)
                return new ValidationResult("Hire date cannot be earlier than the date of birth.");

            return ValidationResult.Success;
        }
    }
}
