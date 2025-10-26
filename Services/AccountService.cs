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

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
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
