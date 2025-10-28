using BussinessObject;
using BussinessObject.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Data;
using System.Security.Claims;

namespace HandicraftShop_Prodject.Utils
{
    public class AccountUtils
    {
        public static ClaimsPrincipal CreatePrincipal(Account account)
        {
            var claims = new List<Claim>
            {
                new Claim(nameof(account.AccountId), account.AccountId),
                new Claim(nameof(account.Username), account.Username ?? ""),
                new Claim(nameof(account.Email), account.Email ?? "")
            };

			var roles = account.UserRoles?
			.Where(ur => ur.Role != null)
			.Select(ur => ur.Role.RoleName)
			.ToList() ?? new List<string>();

			// Thêm role vào claims
			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            return principal;
        }

        public static Account? GetUserData(ClaimsPrincipal principal)
        {
            try
            {
                if (principal == null || principal.Identity == null || !principal.Identity.IsAuthenticated)
                    return default;

                var userData = new Account
                {
                    AccountId = principal.FindFirstValue(nameof(Account.AccountId)) ?? "",
                    Username = principal.FindFirstValue(nameof(Account.Username)) ?? "",
                    Email = principal.FindFirstValue(nameof(Account.Email)) ?? "",
                    Avatar = principal.FindFirstValue(nameof(Account.Avatar)) ?? ""
                };

				var roles = principal.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();

				userData.UserRoles = roles.Select(r => new UserRole
				{
					Role = new Role { RoleName = r }
				}).ToList();
				return userData;
            }
            catch
            {
                return default;
            }
        }
    }
}
