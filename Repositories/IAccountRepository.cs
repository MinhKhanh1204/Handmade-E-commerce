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

    }
}
