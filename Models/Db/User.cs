using System;
using System.Collections.Generic;

namespace WebApplication1.Models.Db;

public partial class User
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public string? PhoneNumber { get; set; }
    public string? PasswordHash { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? AddressMain { get; set; }
    public DateTime? CreateAt { get; set; }
}
