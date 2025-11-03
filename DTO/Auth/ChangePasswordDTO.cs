using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
	public class ChangePasswordDTO
	{
		[Required(ErrorMessage = "Current password is required")]
		[DataType(DataType.Password)]
		public string CurrentPassword { get; set; }

		[Required(ErrorMessage = "New password is required")]
		[DataType(DataType.Password)]
		[MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
		public string NewPassword { get; set; }

		[Required(ErrorMessage = "Confirm password is required")]
		[DataType(DataType.Password)]
		[Compare("NewPassword", ErrorMessage = "New password and confirm password do not match")]
		public string ConfirmPassword { get; set; }
	}
}

