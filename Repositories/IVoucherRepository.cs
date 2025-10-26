using BussinessObject;
using BussinessAccessObject;

namespace Repositories
{
    public interface IVoucherRepository : IGenericRepository<Voucher>
    {
        Task<(IEnumerable<Voucher> Items, int Total)> SearchAsync(string? q, bool? isActive, DateTime? expireFrom, DateTime? expireTo, decimal? minOrderFrom, decimal? minOrderTo, int page, int pageSize, string? sortBy, bool desc);
        Task<IEnumerable<Voucher>> GetActiveVouchersAsync();
        Task<Voucher?> GetByCodeAsync(string code);
        Task<bool> ExistsByCodeAsync(string code);
    }
}
