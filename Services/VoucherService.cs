using BussinessObject;
using Repositories;

namespace Services
{
    public class VoucherService : IVoucherService
    {
        private readonly IVoucherRepository _voucherRepository;

        public VoucherService(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        public async Task<(IEnumerable<Voucher> Items, int Total)> SearchAsync(string? q, bool? isActive, DateTime? expireFrom, DateTime? expireTo, decimal? minOrderFrom, decimal? minOrderTo, int page, int pageSize, string? sortBy, bool desc)
        {
            return await _voucherRepository.SearchAsync(q, isActive, expireFrom, expireTo, minOrderFrom, minOrderTo, page, pageSize, sortBy, desc);
        }

        public async Task<IEnumerable<Voucher>> GetActiveVouchersAsync()
        {
            return await _voucherRepository.GetActiveVouchersAsync();
        }

        public async Task<Voucher?> GetByIdAsync(int id)
        {
            return await _voucherRepository.GetByIdAsync(id);
        }

        public async Task<Voucher> CreateAsync(Voucher voucher)
        {
            // Business rules validation
            ValidateVoucher(voucher);
            
            // Check if code is taken
            if (await IsCodeTakenAsync(voucher.Code!))
            {
                throw new InvalidOperationException($"Mã voucher '{voucher.Code}' đã tồn tại.");
            }

            return await _voucherRepository.CreateAsync(voucher);
        }

        public async Task<Voucher> UpdateAsync(Voucher voucher)
        {
            // Business rules validation
            ValidateVoucher(voucher);
            
            // Check if code is unique (excluding current voucher)
            if (!await IsCodeUniqueAsync(voucher.Code!, voucher.VoucherId))
            {
                throw new InvalidOperationException($"Voucher code '{voucher.Code}' already exists.");
            }

            return await _voucherRepository.UpdateAsync(voucher);
        }

        public async Task DeleteAsync(int id)
        {
            await _voucherRepository.DeleteAsync(id);
        }

        public async Task<Voucher?> GetByCodeAsync(string code)
        {
            return await _voucherRepository.GetByCodeAsync(code);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _voucherRepository.ExistsAsync(id);
        }

        public async Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null)
        {
            var existingVoucher = await _voucherRepository.GetByCodeAsync(code);
            
            if (existingVoucher == null)
                return true;
                
            // If excludeId is provided, check if it's the same voucher
            if (excludeId.HasValue && existingVoucher.VoucherId == excludeId.Value)
                return true;
                
            return false;
        }

        public async Task<bool> IsCodeTakenAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;

            return await _voucherRepository.ExistsByCodeAsync(code);
        }

        private void ValidateVoucher(Voucher voucher)
        {
            if (voucher.DiscountPercentage < 0 || voucher.DiscountPercentage > 100)
            {
                throw new ArgumentException("Discount percentage must be between 0 and 100.");
            }

            if (voucher.MaxReducing < 0)
            {
                throw new ArgumentException("Max reducing amount must be greater than or equal to 0.");
            }

            if (voucher.MinOrderValue < 0)
            {
                throw new ArgumentException("Min order value must be greater than or equal to 0.");
            }

            if (voucher.Quantity < 0)
            {
                throw new ArgumentException("Quantity must be greater than or equal to 0.");
            }

            if (voucher.MaxUsagePerUser < 0)
            {
                throw new ArgumentException("Max usage per user must be greater than or equal to 0.");
            }

            if (voucher.IsActive == true && voucher.ExpiryDate <= DateOnly.FromDateTime(DateTime.Now))
            {
                throw new ArgumentException("Active vouchers must have expiry date in the future.");
            }

            if (string.IsNullOrWhiteSpace(voucher.Code))
            {
                throw new ArgumentException("Voucher code is required.");
            }

            if (string.IsNullOrWhiteSpace(voucher.VoucherName))
            {
                throw new ArgumentException("Voucher name is required.");
            }
        }
    }
}
