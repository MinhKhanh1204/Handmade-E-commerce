using BussinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IAccountRepository
    {
        Account GetByEmail(string email);
		void Add(Account account);
		void AddCustomer(Customer customer);
		void SaveChanges();
        Account GetAccountByID(string id);
        void UpdateProfile(Account account);
		void UpdatePassword(string accountId, string newPassword);
		void UpdatePasswordResetToken(string email, string token, DateTime expiry);
		Account GetAccountByResetToken(string email, string token);
		void ClearPasswordResetToken(string email);
    }
}
