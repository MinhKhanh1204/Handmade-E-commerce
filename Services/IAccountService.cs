using BussinessObject;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IAccountService
    {
        Account Login(LoginDTO account);
		bool Register(RegisterDTO account);
        Account GetAccountByID(string id);
        void UpdateProfile(Account account);
		Account GetAccountByEmail(string email);
		bool ChangePassword(string accountId, ChangePasswordDTO changePasswordDto);
		Task<bool> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDto);
		bool ResetPassword(ResetPasswordDTO resetPasswordDto);
    }
}
