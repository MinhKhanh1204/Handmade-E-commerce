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

		public void Add(Account account)
		{
			account.AccountId = GenerateNewAccountId();
			_context.Accounts.Add(account);
		}

		private string GenerateNewAccountId()
		{
			// Lấy tất cả AccountId -> Cắt phần số -> Ép về int
			var lastNumber = _context.Accounts
		.AsEnumerable() // CHUYỂN sang xử lý bằng LINQ to Objects
		.Select(a => a.AccountId)
		.Where(id => id.StartsWith("CUS"))
		.Select(id =>
		{
			int number;
			return int.TryParse(id.Substring(3), out number) ? number : 0;
		})
		.DefaultIfEmpty(0)
		.Max();

			int newNumber = lastNumber + 1;

			// Gộp lại, định dạng 3 chữ số
			return $"CUS{newNumber:D3}";
		}

		public Account GetByEmail(string email)
		{
			return _context.Accounts
			   .Include(a => a.UserRoles)
			   .ThenInclude(ur => ur.Role)
			   .FirstOrDefault(a => a.Email == email && a.Status == "Active");
		}

		public void SaveChanges()
		{
			_context.SaveChanges();
		}

		public void AddCustomer(Customer customer)
		{
			_context.Customers.Add(customer);
		}
	}
}
