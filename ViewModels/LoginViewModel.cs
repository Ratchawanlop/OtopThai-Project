using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels;

public class LoginViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "กรุณากรอกเบอร์โทรศัพท์")]
        [RegularExpression(@"^0[0-9]{9}$", ErrorMessage = "รูปแบบเบอร์โทรศัพท์ไม่ถูกต้อง (เช่น 0812345678)")]
        [Display(Name = "เบอร์โทรศัพท์")]
        public string Phone_number { get; set; }

        [Required(ErrorMessage = "กรุณากรอกรหัสผ่าน")]
        [DataType(DataType.Password)]
        [Display(Name = "รหัสผ่าน")]
        // ใช้ชื่อตัวแปร Password ตรงๆ ใน Form ได้เลยครับ ตอนเซฟ/เช็คใน Controller ค่อยเอาไปเทียบกับ Password_hash ใน DB
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}

public class RegisterViewModel
{
    public string Phone_number { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password_hash { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
