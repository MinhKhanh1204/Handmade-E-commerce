using BussinessObject;
using Repositories;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;
using Microsoft.EntityFrameworkCore;
using BussinessObject.Models;
using Microsoft.Identity.Client;

namespace Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
		private readonly IEmailService _emailService;

        public AccountService(IAccountRepository accountRepository, IEmailService emailService)
        {
            _accountRepository = accountRepository;
			_emailService = emailService;
        }

		public Account GetAccountByEmail(string email)
		{
			return _accountRepository.GetAccountByEmail(email);
		}

		public Account GetAccountByID(string id)
        {
            return _accountRepository.GetAccountByID(id);
        }

        public Account Login(LoginDTO acc)
        {
            var account = _accountRepository.GetByEmail(acc.Email);
            if (account == null) return null;

            // Giả sử password trong DB được mã hoá SHA256
            var hashedPassword = HashPassword(acc.Password);
            return account.Password == hashedPassword ? account : null;
        }

		public bool Register(RegisterDTO registerDto)
		{
			var existing = _accountRepository.GetByEmail(registerDto.Email);
			if (existing != null)
				return false;

			var account = new Account
			{
				Username = registerDto.Username,
				Email = registerDto.Email,
				Status = "Active",
				CreatedAt = DateTime.Now
			};

			account.Password = HashPassword(registerDto.Password);
            account.UserRoles.Add(new UserRole
            {
				Account = account,
                RoleId = 3,
                Status = "Active"
            });

			_accountRepository.Add(account);

			var customer = new Customer
			{
				CustomerId = account.AccountId,
				FullName = registerDto.FullName,
                DateOfBirth = registerDto.DateOfBirth,
                Gender = registerDto.Gender,
				Phone = registerDto.Phone,
                Address = registerDto.Address,
				Status = "Active"
            };

			_accountRepository.AddCustomer(customer);
			_accountRepository.SaveChanges();

			return true;
		}

        public void UpdateProfile(Account account)
        {
            _accountRepository.UpdateProfile(account);
            _accountRepository.SaveChanges();
        }

		public bool ChangePassword(string accountId, ChangePasswordDTO changePasswordDto)
		{
			var account = _accountRepository.GetAccountByID(accountId);
			if (account == null)
				return false;

			// Verify current password
			var hashedCurrentPassword = HashPassword(changePasswordDto.CurrentPassword);
			if (account.Password != hashedCurrentPassword)
				return false;

			// Hash new password and update
			var hashedNewPassword = HashPassword(changePasswordDto.NewPassword);
			_accountRepository.UpdatePassword(accountId, hashedNewPassword);
			_accountRepository.SaveChanges();

			return true;
		}

		public async Task<bool> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDto)
		{
			try
			{
				// Check if account exists
				var account = _accountRepository.GetByEmail(forgotPasswordDto.Email);
				if (account == null)
					return false;

				// Generate 6-digit reset code
				var resetCode = GenerateResetCode();

				// Set expiry time (15 minutes from now)
				var expiry = DateTime.Now.AddMinutes(15);

				// Save token to database
				_accountRepository.UpdatePasswordResetToken(forgotPasswordDto.Email, resetCode, expiry);
				_accountRepository.SaveChanges();

				// Send email with reset code
				await _emailService.SendPasswordResetEmailAsync(forgotPasswordDto.Email, resetCode);

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool ResetPassword(ResetPasswordDTO resetPasswordDto)
		{
			// Verify token
			var account = _accountRepository.GetAccountByResetToken(resetPasswordDto.Email, resetPasswordDto.ResetCode);
			if (account == null)
				return false;

			// Hash new password and update
			var hashedNewPassword = HashPassword(resetPasswordDto.NewPassword);
			_accountRepository.UpdatePassword(account.AccountId, hashedNewPassword);

			// Clear reset token
			_accountRepository.ClearPasswordResetToken(resetPasswordDto.Email);
			_accountRepository.SaveChanges();

			return true;
		}

		private string GenerateResetCode()
		{
			// Generate random 6-digit code
			var random = new Random();
			return random.Next(100000, 999999).ToString();
		}

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
    }
}
