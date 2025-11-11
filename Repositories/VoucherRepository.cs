using BussinessObject;
using DataAccessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

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
    }
}

