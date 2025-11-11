using System;
using BussinessObject;

namespace DTO
{
    public class VoucherDTO
    {
        public int VoucherId { get; set; }
        public string? VoucherName { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? MaxReducing { get; set; }
        public int? Quantity { get; set; }
        public int? UsageCount { get; set; }
        public DateOnly? ExpiryDate { get; set; }
        public bool? IsActive { get; set; }
        public decimal? MinOrderValue { get; set; }
        public int? MaxUsagePerUser { get; set; }

        // Tính toán số lượng còn lại
        public int? RemainingQuantity => Quantity.HasValue && UsageCount.HasValue 
            ? Quantity.Value - UsageCount.Value 
            : null;

        // Kiểm tra voucher còn hiệu lực không
        public bool IsValid => IsActive == true 
            && ExpiryDate.HasValue 
            && ExpiryDate.Value >= DateOnly.FromDateTime(DateTime.Now)
            && (Quantity == null || RemainingQuantity > 0);

        // Tính toán số tiền giảm tối đa
        public string DiscountDisplay
        {
            get
            {
                if (DiscountPercentage.HasValue)
                {
                    return $"{DiscountPercentage.Value}%";
                }
                return "N/A";
            }
        }

        // Map từ entity Voucher sang DTO
        public static VoucherDTO FromEntity(Voucher v)
        {
            if (v == null) throw new ArgumentNullException(nameof(v));

            return new VoucherDTO
            {
                VoucherId = v.VoucherId,
                VoucherName = v.VoucherName,
                Code = v.Code,
                Description = v.Description,
                DiscountPercentage = v.DiscountPercentage,
                MaxReducing = v.MaxReducing,
                Quantity = v.Quantity,
                UsageCount = v.UsageCount,
                ExpiryDate = v.ExpiryDate,
                IsActive = v.IsActive,
                MinOrderValue = v.MinOrderValue,
                MaxUsagePerUser = v.MaxUsagePerUser
            };
        }
    }
}

