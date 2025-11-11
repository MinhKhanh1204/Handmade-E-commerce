using BussinessObject;
using DataAccessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly MyStoreContext _context;

        public VoucherRepository(MyStoreContext context)
        {
            _context = context;
        }

        public IQueryable<Voucher> GetAllVouchers()
        {
            return _context.Vouchers.AsQueryable();
        }

        public Voucher? GetVoucherById(int voucherId)
        {
            return _context.Vouchers
                .FirstOrDefault(v => v.VoucherId == voucherId);
        }

        public List<Voucher> GetActiveVouchers()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            return _context.Vouchers
                .Where(v => v.IsActive == true 
                    && v.ExpiryDate >= today
                    && (v.Quantity == null || v.UsageCount < v.Quantity))
                .OrderByDescending(v => v.ExpiryDate)
                .ToList();
        }

		public async Task<(IEnumerable<Voucher> Items, int Total)> SearchAsync(string? q, bool? isActive, DateTime? expireFrom, DateTime? expireTo, decimal? minOrderFrom, decimal? minOrderTo, int page, int pageSize, string? sortBy, bool desc)
		{
			var query = _context.Vouchers.AsNoTracking().AsQueryable();

			if (!string.IsNullOrEmpty(q))
			{
				query = query.Where(v => v.VoucherName!.Contains(q) ||
										 v.Code!.Contains(q) ||
										 v.Description!.Contains(q));
			}

			if (isActive.HasValue)
			{
				query = query.Where(v => v.IsActive == isActive.Value);
			}

			if (expireFrom.HasValue)
			{
				query = query.Where(v => v.ExpiryDate >= DateOnly.FromDateTime(expireFrom.Value));
			}

			if (expireTo.HasValue)
			{
				query = query.Where(v => v.ExpiryDate <= DateOnly.FromDateTime(expireTo.Value));
			}

			if (minOrderFrom.HasValue)
			{
				query = query.Where(v => v.MinOrderValue >= minOrderFrom.Value);
			}

			if (minOrderTo.HasValue)
			{
				query = query.Where(v => v.MinOrderValue <= minOrderTo.Value);
			}

			query = (sortBy ?? string.Empty).ToLower() switch
			{
				"name" => desc ? query.OrderByDescending(v => v.VoucherName) : query.OrderBy(v => v.VoucherName),
				"code" => desc ? query.OrderByDescending(v => v.Code) : query.OrderBy(v => v.Code),
				"expirydate" => desc ? query.OrderByDescending(v => v.ExpiryDate) : query.OrderBy(v => v.ExpiryDate),
				"discountpercentage" => desc ? query.OrderByDescending(v => v.DiscountPercentage) : query.OrderBy(v => v.DiscountPercentage),
				"isactive" => desc ? query.OrderByDescending(v => v.IsActive) : query.OrderBy(v => v.IsActive),
				_ => query.OrderBy(v => v.VoucherId)
			};

			var total = await query.CountAsync();

			var items = await query
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return (items, total);
		}

		public async Task<IEnumerable<Voucher>> GetActiveVouchersAsync()
		{
			return await _context.Vouchers.AsNoTracking()
				.Where(v => v.IsActive == true && v.ExpiryDate > DateOnly.FromDateTime(DateTime.Now))
				.OrderBy(v => v.VoucherName)
				.ToListAsync();
		}

		public async Task<Voucher?> GetByIdAsync(int id)
		{
			return await _context.Vouchers.FindAsync(id);
		}

		public async Task<Voucher> CreateAsync(Voucher entity)
		{
			_context.Vouchers.Add(entity);
			await _context.SaveChangesAsync();
			return entity;
		}

		public async Task<Voucher> UpdateAsync(Voucher entity)
		{
			_context.Vouchers.Update(entity);
			await _context.SaveChangesAsync();
			return entity;
		}

		public async Task DeleteAsync(int id)
		{
			var entity = await _context.Vouchers.FindAsync(id);
			if (entity != null)
			{
				_context.Vouchers.Remove(entity);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<bool> ExistsAsync(int id)
		{
			return await _context.Vouchers.AnyAsync(v => v.VoucherId == id);
		}

		public async Task<Voucher?> GetByCodeAsync(string code)
		{
			return await _context.Vouchers.AsNoTracking().FirstOrDefaultAsync(v => v.Code == code);
		}

		public async Task<bool> ExistsByCodeAsync(string code)
		{
			if (string.IsNullOrWhiteSpace(code)) return false;
			return await _context.Vouchers.AsNoTracking().AnyAsync(v => v.Code == code);
		}
    }
}

