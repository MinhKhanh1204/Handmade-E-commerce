using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
	public class RegisterDTO
	{
		[Required(ErrorMessage = "Username is required")]
		public string Username { get; set; }

		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid email format")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Password is required")]
		[DataType(DataType.Password)]
		[MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Confirm password is required")]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "Passwords do not match")]
		public string ConfirmPassword { get; set; }

        // Thêm thông tin khách hàng cơ bản
        [Required(ErrorMessage = "FullName is required")]
        public string FullName { get; set; }

		[Required(ErrorMessage = "Date of birth is required")]
		[DataType(DataType.Date)]
		public DateOnly DateOfBirth { get; set; }

		[Required(ErrorMessage = "Gender is required")]
		public string Gender { get; set; } // "Male" / "Female" / "Other"

		[Required(ErrorMessage = "Phone is required")]
        [RegularExpression(@"^(0[1-9][0-9]{8,9})$", ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; }

		[Required(ErrorMessage = "Address is required")]
		public string Address { get; set; }
	}
}
