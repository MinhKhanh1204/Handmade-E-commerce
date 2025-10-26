using BussinessObject.Models;
using System;
using System.Collections.Generic;

namespace BussinessObject;

public partial class Account
{
    public string AccountId { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? PasswordRecoveryToken { get; set; }

    public DateTime? TokenExpiry { get; set; }

    public string? Avatar { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Status { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Staff? Staff { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
