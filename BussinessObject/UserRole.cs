using System;
using System.Collections.Generic;

namespace BussinessObject.Models;

public partial class UserRole
{
    public string AccountId { get; set; } = null!;

    public int RoleId { get; set; }

    public string? Status { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
