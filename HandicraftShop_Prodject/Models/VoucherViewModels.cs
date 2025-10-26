using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace HandicraftShop_Prodject.Models
{
    public class VoucherSearchVm
    {
        public string? Q { get; set; }               // tìm theo Name/Code/Description
        public bool? IsActive { get; set; }
        public DateTime? ExpireFrom { get; set; }
        public DateTime? ExpireTo { get; set; }
        public decimal? MinOrderFrom { get; set; }
        public decimal? MinOrderTo { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }          // Name | Code | ExpiryDate | DiscountPercentage | IsActive
    }

    public class VoucherCreateVm : IValidatableObject
    {
        [Required(ErrorMessage = "Tên voucher là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên voucher không được vượt quá 100 ký tự")]
        public string VoucherName { get; set; } = null!;

        [Required(ErrorMessage = "Mã voucher là bắt buộc")]
        [StringLength(40, ErrorMessage = "Mã voucher không được vượt quá 40 ký tự")]
        [RegularExpression(@"^[A-Z0-9_-]+$", ErrorMessage = "Mã voucher chỉ được chứa chữ cái A-Z, số 0-9, dấu gạch ngang (-) hoặc gạch dưới (_)")]
        // [Remote(areaName: "Admin", controller: "Voucher", action: "IsCodeAvailable", HttpMethod = "GET", ErrorMessage = "Mã voucher này đã tồn tại")]
        public string Code { get; set; } = null!;

        [StringLength(255, ErrorMessage = "Mô tả không được vượt quá 255 ký tự")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Phần trăm giảm giá là bắt buộc")]
        [Range(0, 100, ErrorMessage = "Phần trăm giảm giá phải từ 0 đến 100")]
        public decimal DiscountPercentage { get; set; }

        [Required(ErrorMessage = "Số tiền giảm tối đa là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Số tiền giảm tối đa phải lớn hơn hoặc bằng 0")]
        public decimal MaxReducing { get; set; }

        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Ngày hết hạn là bắt buộc")]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; } = DateTime.Now.AddDays(1);

        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "Giá trị đơn hàng tối thiểu là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá trị đơn hàng tối thiểu phải lớn hơn hoặc bằng 0")]
        public decimal MinOrderValue { get; set; }

        [Required(ErrorMessage = "Số lần sử dụng tối đa mỗi người dùng là bắt buộc")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lần sử dụng tối đa mỗi người dùng phải lớn hơn hoặc bằng 0")]
        public int MaxUsagePerUser { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ExpiryDate.Date <= DateTime.Today)
                yield return new ValidationResult("Ngày hết hạn phải lớn hơn ngày hiện tại.", new[] { nameof(ExpiryDate) });
        }
    }

    public class VoucherEditVm : IValidatableObject
    {
        public int VoucherId { get; set; }

        [Required(ErrorMessage = "Tên voucher là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên voucher không được vượt quá 100 ký tự")]
        public string VoucherName { get; set; } = null!;

        [Required(ErrorMessage = "Mã voucher là bắt buộc")]
        [StringLength(40, ErrorMessage = "Mã voucher không được vượt quá 40 ký tự")]
        [RegularExpression(@"^[A-Z0-9_-]+$", ErrorMessage = "Mã voucher chỉ được chứa chữ cái A-Z, số 0-9, dấu gạch ngang (-) hoặc gạch dưới (_)")]
        public string Code { get; set; } = null!;

        [StringLength(255, ErrorMessage = "Mô tả không được vượt quá 255 ký tự")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Phần trăm giảm giá là bắt buộc")]
        [Range(0, 100, ErrorMessage = "Phần trăm giảm giá phải từ 0 đến 100")]
        public decimal DiscountPercentage { get; set; }

        [Required(ErrorMessage = "Số tiền giảm tối đa là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Số tiền giảm tối đa phải lớn hơn hoặc bằng 0")]
        public decimal MaxReducing { get; set; }

        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Ngày hết hạn là bắt buộc")]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }

        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Giá trị đơn hàng tối thiểu là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá trị đơn hàng tối thiểu phải lớn hơn hoặc bằng 0")]
        public decimal MinOrderValue { get; set; }

        [Required(ErrorMessage = "Số lần sử dụng tối đa mỗi người dùng là bắt buộc")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lần sử dụng tối đa mỗi người dùng phải lớn hơn hoặc bằng 0")]
        public int MaxUsagePerUser { get; set; }

        public int UsageCount { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ExpiryDate.Date <= DateTime.Today)
                yield return new ValidationResult("Ngày hết hạn phải lớn hơn ngày hiện tại.", new[] { nameof(ExpiryDate) });
        }
    }

    public class VoucherDetailsVm
    {
        public int VoucherId { get; set; }
        public string VoucherName { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal MaxReducing { get; set; }
        public int Quantity { get; set; }
        public int UsageCount { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public decimal MinOrderValue { get; set; }
        public int MaxUsagePerUser { get; set; }
        public bool IsExpired => ExpiryDate < DateTime.Now;
    }

    public class VoucherDeleteVm
    {
        public int VoucherId { get; set; }
        public string VoucherName { get; set; } = null!;
        public string Code { get; set; } = null!;
        public decimal DiscountPercentage { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsExpired => ExpiryDate < DateTime.Now;
    }

    public class PagedList<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)Total / PageSize);
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
    }
}
