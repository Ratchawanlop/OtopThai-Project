using System;
using System.Collections.Generic;

namespace WebApplication1.Models.Db;

public partial class Role
{
    public int RoleId { get; set; }

    public string? RoleName { get; set; }

    public string? Permission { get; set; }
}
