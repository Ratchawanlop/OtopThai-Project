using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels;

public class LoginViewModel
{
    public string Phone_number { get; set; }
    public string Password { get; set; }
    public bool RememberMe { get; set; }
}

public class RegisterViewModel
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public string Phone_number { get; set; }
    public string Password_hash { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string AddressMain { get; set; }
}
