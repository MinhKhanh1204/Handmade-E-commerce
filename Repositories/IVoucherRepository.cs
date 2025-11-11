using BussinessObject;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IVoucherRepository
    {
        IQueryable<Voucher> GetAllVouchers();
        Voucher? GetVoucherById(int voucherId);
        List<Voucher> GetActiveVouchers();

		// Async APIs for Admin Voucher management
		Task<(IEnumerable<Voucher> Items, int Total)> SearchAsync(
			string? q, bool? isActive, System.DateTime? expireFrom, System.DateTime? expireTo,
			decimal? minOrderFrom, decimal? minOrderTo, int page, int pageSize, string? sortBy, bool desc);
		Task<IEnumerable<Voucher>> GetActiveVouchersAsync();
		Task<Voucher?> GetByIdAsync(int id);
		Task<Voucher> CreateAsync(Voucher voucher);
		Task<Voucher> UpdateAsync(Voucher voucher);
		Task DeleteAsync(int id);
		Task<bool> ExistsAsync(int id);
		Task<Voucher?> GetByCodeAsync(string code);
		Task<bool> ExistsByCodeAsync(string code);
    }
}

