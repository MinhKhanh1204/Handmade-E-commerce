using BussinessObject;

namespace Services
{
    public interface IVoucherService
    {
        Task<(IEnumerable<Voucher> Items, int Total)> SearchAsync(string? q, bool? isActive, DateTime? expireFrom, DateTime? expireTo, decimal? minOrderFrom, decimal? minOrderTo, int page, int pageSize, string? sortBy, bool desc);
        Task<IEnumerable<Voucher>> GetActiveVouchersAsync();
        Task<Voucher?> GetByIdAsync(int id);
        Task<Voucher> CreateAsync(Voucher voucher);
        Task<Voucher> UpdateAsync(Voucher voucher);
        Task DeleteAsync(int id);
        Task<Voucher?> GetByCodeAsync(string code);
        Task<bool> ExistsAsync(int id);
        Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null);
        Task<bool> IsCodeTakenAsync(string code);
    }
}
