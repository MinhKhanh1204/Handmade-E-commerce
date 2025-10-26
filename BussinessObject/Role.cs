using System;
using System.Collections.Generic;

namespace BussinessObject;

public partial class Role
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public string? Description { get; set; }
}
