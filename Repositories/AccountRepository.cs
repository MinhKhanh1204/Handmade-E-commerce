using BussinessObject;
using DataAccessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly MyStoreContext _context;

        public AccountRepository(MyStoreContext context)
        {
            _context = context;
        }
        public Account GetByEmail(string email)
        {
            return _context.Accounts
               .Include(a => a.UserRoles)
               .ThenInclude(ur => ur.Role)
               .FirstOrDefault(a => a.Email == email && a.Status == "Active");
        }
    }
}
