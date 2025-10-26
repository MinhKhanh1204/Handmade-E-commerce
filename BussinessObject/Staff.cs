using System;
using System.Collections.Generic;

namespace BussinessObject;

public partial class Staff
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

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Account StaffNavigation { get; set; } = null!;
}
