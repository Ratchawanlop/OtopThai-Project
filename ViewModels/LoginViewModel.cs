namespace WebApplication1.ViewModels;

public class LoginViewModel
{
    public string Phone_number { get; set; } = string.Empty;
    public string Password_hash { get; set; } = string.Empty;
}

public class RegisterViewModel
{
    public string Phone_number { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password_hash { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
