using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
	public class ResetPasswordDTO
	{
		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid email format")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Reset code is required")]
		public string ResetCode { get; set; }

		[Required(ErrorMessage = "Password is required")]
		[DataType(DataType.Password)]
		[MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
		public string NewPassword { get; set; }

		[Required(ErrorMessage = "Confirm password is required")]
		[DataType(DataType.Password)]
		[Compare("NewPassword", ErrorMessage = "Password and confirm password do not match")]
		public string ConfirmPassword { get; set; }
	}
}

