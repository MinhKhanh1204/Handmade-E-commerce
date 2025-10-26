using BussinessObject;
using BussinessAccessObject;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class VoucherRepository : GenericRepository<Voucher>, IVoucherRepository
    {
        public VoucherRepository(MyStoreContext context) : base(context)
        {
        }

        public async Task<(IEnumerable<Voucher> Items, int Total)> SearchAsync(string? q, bool? isActive, DateTime? expireFrom, DateTime? expireTo, decimal? minOrderFrom, decimal? minOrderTo, int page, int pageSize, string? sortBy, bool desc)
        {
            var query = _dbSet.AsNoTracking().AsQueryable();

            // Apply filters
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

            // Apply sorting
            query = sortBy?.ToLower() switch
            {
                "name" => desc ? query.OrderByDescending(v => v.VoucherName) : query.OrderBy(v => v.VoucherName),
                "code" => desc ? query.OrderByDescending(v => v.Code) : query.OrderBy(v => v.Code),
                "expirydate" => desc ? query.OrderByDescending(v => v.ExpiryDate) : query.OrderBy(v => v.ExpiryDate),
                "discountpercentage" => desc ? query.OrderByDescending(v => v.DiscountPercentage) : query.OrderBy(v => v.DiscountPercentage),
                "isactive" => desc ? query.OrderByDescending(v => v.IsActive) : query.OrderBy(v => v.IsActive),
                _ => query.OrderBy(v => v.VoucherId)
            };

            // Get total count
            var total = await query.CountAsync();

            // Apply pagination
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<IEnumerable<Voucher>> GetActiveVouchersAsync()
        {
            return await _dbSet.AsNoTracking()
                .Where(v => v.IsActive == true && v.ExpiryDate > DateOnly.FromDateTime(DateTime.Now))
                .OrderBy(v => v.VoucherName)
                .ToListAsync();
        }

        public async Task<Voucher?> GetByCodeAsync(string code)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(v => v.Code == code);
        }

        public async Task<bool> ExistsByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                Console.WriteLine("Code is null or empty, returning false");
                return false;
            }

            try
            {
                Console.WriteLine($"Repository: Checking if code '{code}' exists...");
                
                // First, let's see if we can query the table at all
                var totalCount = await _dbSet.AsNoTracking().CountAsync();
                Console.WriteLine($"Repository: Total vouchers in database: {totalCount}");
                
                var result = await _dbSet.AsNoTracking()
                    .AnyAsync(v => v.Code == code);
                
                Console.WriteLine($"Repository check: Code='{code}', Exists={result}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Repository error checking code '{code}': {ex.Message}");
                Console.WriteLine($"Repository error stack trace: {ex.StackTrace}");
                return false;
            }
        }
    }
}
