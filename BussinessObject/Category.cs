using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BussinessObject;

public partial class Category
{
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Category name is required")]
    [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
    [Display(Name = "Category Name")]
    public string? CategoryName { get; set; }

    [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Status is required")]
    [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
    public string? Status { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
