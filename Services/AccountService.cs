using BussinessObject;
using Repositories;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public Account Login(string email, string password)
        {
            var account = _accountRepository.GetByEmail(email);
            if (account == null) return null;

            // Giả sử password trong DB được mã hoá SHA256
            var hashedPassword = HashPassword(password);
            return account.Password == hashedPassword ? account : null;
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
