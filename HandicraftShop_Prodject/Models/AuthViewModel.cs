using DTO;

namespace HandicraftShop_Prodject.Models
{
	public class AuthViewModel
	{
		public LoginDTO Login { get; set; } = new();
		public RegisterDTO Register { get; set; } = new();
	}
}
