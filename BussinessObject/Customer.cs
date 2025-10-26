using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BussinessObject;

public partial class Customer
{
    [Required(ErrorMessage = "Customer ID is required")]
    public string CustomerId { get; set; } = null!;

    [Required(ErrorMessage = "Full name is required")]
    [StringLength(32, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 32 characters")]
    public string? FullName { get; set; }

    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [Range(typeof(DateTime), "1900-01-01", "2024-12-31", ErrorMessage = "Date of birth must be between 1900 and 2024")]
    public DateTime? DateOfBirth { get; set; }

    [StringLength(10, ErrorMessage = "Gender cannot exceed 10 characters")]
    [RegularExpression(@"^(Male|Female|Other)$", ErrorMessage = "Gender must be Male, Female, or Other")]
    public string? Gender { get; set; }

    [Required(ErrorMessage = "Phone number is required")]
    [StringLength(15, MinimumLength = 10, ErrorMessage = "Phone number must be between 10 and 15 characters")]
    [RegularExpression(@"^[0-9+\-\s\(\)]+$", ErrorMessage = "Phone number can only contain numbers, +, -, spaces, and parentheses")]
    public string? Phone { get; set; }

    [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters")]
    [RegularExpression(@"^[a-zA-Z0-9À-ỹ\s\.,\-/]+$", ErrorMessage = "Address contains invalid characters")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "Status is required")]
    [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
    [RegularExpression(@"^(Active|Inactive)$", ErrorMessage = "Status must be Active or Inactive")]
    public string? Status { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Account? CustomerNavigation { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
