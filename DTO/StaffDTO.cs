using System;

namespace DTO
{
    public class StaffDTO
    {
        public string StaffId { get; set; } = null!;
        public string? FullName { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Phone { get; set; }
        public string? CitizenId { get; set; }
        public string? Address { get; set; }
        public DateOnly? HireDate { get; set; }
        public string? Status { get; set; }

        // Thêm các trường Account
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }

        // Thêm CreatedAt để hiển thị ngày tạo
        public DateTime? CreatedAt { get; set; }
    }
}
