using System.ComponentModel.DataAnnotations;

namespace HandicraftShop_Prodject.Models
{
	public class ProfileViewModel
	{
		public string AccountId { get; set; }

		[Required(ErrorMessage = "Username is required")]
		public string Username { get; set; }

		// Thêm thông tin khách hàng cơ bản
		[Required(ErrorMessage = "FullName is required")]
		public string FullName { get; set; }

		[Required(ErrorMessage = "Date of birth is required")]
		[DataType(DataType.Date)]
		public DateOnly DateOfBirth { get; set; }

		[Required(ErrorMessage = "Gender is required")]
		public string Gender { get; set; } // "Male" / "Female" / "Other"

		public string? Avatar { get; set; }

		[Required(ErrorMessage = "Phone is required")]
		[RegularExpression(@"^(0[1-9][0-9]{8,9})$", ErrorMessage = "Invalid phone number")]
		public string Phone { get; set; }

		[Required(ErrorMessage = "Address is required")]
		public string Address { get; set; }
	}
}
