using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System.Diagnostics;
using WebApplication1.Viewmodels;
using WebApplication1.ViewModels;
using WebApplication1.Models.Db;

namespace WebApplication1.Controllers;

public class AccountController : Controller
{
    private readonly Csi402dbContext _db;
    public AccountController(Csi402dbContext db)
    {
        _db = db;
    }


    public IActionResult Login()
    {
        GetCartCount();
        return View();
    }
    [HttpPost]
    public IActionResult Login(LoginViewModel data)
    {
        GetCartCount();
        var user = (from u in _db.Users
                    where u.PhoneNumber == data.Phone_number && u.PasswordHash == data.Password
                    select u).FirstOrDefault();

        if (user != null)
    {
        HttpContext.Session.SetInt32("UserId", user.UserId);
        
        HttpContext.Session.SetString("Username", user.Username ?? "ผู้ใช้งาน"); 
        
        HttpContext.Session.SetInt32("RoleId", user.RoleId);
        HttpContext.Session.SetString("Email", user.Email ?? "ไม่มีอีเมล");

        TempData["Success"] = "ยินดีต้อนรับคุณ " + (user.Username ?? "ผู้ใช้งาน") + " เข้าสู่ระบบ";
        return RedirectToAction("Homepage", "Home");
    }
        else
        {
            ViewBag.Error = "เบอร์โทรศัพท์หรือรหัสผ่านไม่ถูกต้อง";
            return View();
        }
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Homepage", "Home");
    }

    public IActionResult Register()
    {
        GetCartCount();
        return View();
    }
    [HttpPost]
    public IActionResult Register(RegisterViewModel data)
    {
        var existingUser = _db.Users.FirstOrDefault(u => u.PhoneNumber == data.Phone_number);

        if (existingUser != null)
        {
            TempData["Error"] = "เบอร์โทรศัพท์นี้ถูกใช้งานแล้ว กรุณาใช้เบอร์อื่น";
            return View(data);
        }

        var u = new User();
        u.UserId = data.UserId;
        u.RoleId = data.RoleId;
        u.PhoneNumber = data.Phone_number;
        u.PasswordHash = data.Password_hash;
        u.Email = data.Email;
        u.Username = data.Username;
        u.AddressMain = data.AddressMain;
        u.CreateAt = DateTime.Now;

        _db.Users.Add(u);
        _db.SaveChanges();

        TempData["Success"] = "สมัครสมาชิกเรียบร้อยแล้ว กรุณาเข้าสู่ระบบ";
        return RedirectToAction("Login", "Account");
    }

    public IActionResult Userlist()
    {
        var user = _db.Users.ToList();
        return View(user);
    }

    public IActionResult AddUser()
    {
        return View();
    }
    [HttpPost]
    public IActionResult AddUser(RegisterViewModel data)
    {
        var isDuplicate = _db.Users.Any(u => u.PhoneNumber == data.Phone_number);

        if (isDuplicate)
        {
            TempData["Error"] = "เบอร์โทรศัพท์นี้มีในระบบแล้ว ไม่สามารถเพิ่มซ้ำได้";
            return View(data);
        }

        try
        {
            var u = new User
            {
                Username = data.Username,
                PhoneNumber = data.Phone_number,
                PasswordHash = data.Password_hash,
                Email = data.Email,
                AddressMain = data.AddressMain,
                RoleId = data.RoleId,
                CreateAt = DateTime.Now
            };

            _db.Users.Add(u);
            _db.SaveChanges();

            TempData["Success"] = "เพิ่มผู้ใช้งาน " + data.Username + " สำเร็จ";
            return RedirectToAction("UserList");
        }
        catch (Exception ex)
        {
            TempData["Error"] = "เกิดข้อผิดพลาด: " + ex.Message;
            return View(data);
        }
    }

    public IActionResult EditUser(int id)
    {
        var user = (from us in _db.Users
                    where us.UserId == id
                    select new User
                    {
                        UserId = us.UserId,
                        RoleId = us.RoleId,
                        PhoneNumber = us.PhoneNumber,
                        PasswordHash = us.PasswordHash,
                        Email = us.Email,
                        Username = us.Username,
                        AddressMain = us.AddressMain
                    }).FirstOrDefault();


        return View(user);
    }
    [HttpPost]
    public IActionResult EditUser(User data)
    {
        var duplicatePhone = (from u in _db.Users
                              where u.PhoneNumber == data.PhoneNumber && u.UserId != data.UserId
                              select u).FirstOrDefault();

        if (duplicatePhone != null)
        {
            TempData["Error"] = "เบอร์โทรศัพท์นี้ถูกใช้งานแล้ว กรุณาใช้เบอร์อื่น";
            return RedirectToAction("Userlist", "Account");
        }

        var user = (from u in _db.Users where u.UserId == data.UserId select u).FirstOrDefault();

        if (user != null)
    {
        user.Username = data.Username;
        user.PhoneNumber = data.PhoneNumber;
        user.Email = data.Email;
        user.AddressMain = data.AddressMain;
        user.RoleId = data.RoleId;

        // ถ้า data.PasswordHash ไม่ใช่ค่าว่าง (คือแอดมินพิมพ์รหัสใหม่) ค่อยเอาไปเซฟทับ
        if (!string.IsNullOrEmpty(data.PasswordHash))
        {
            user.PasswordHash = data.PasswordHash;
        }

        _db.Update(user); 
        _db.SaveChanges();
    }

        return RedirectToAction("UserList", "Account");
    }

    public IActionResult DeleteUser(int id)
    {
        var user = (from u in _db.Users where u.UserId == id select u).FirstOrDefault();
        _db.Users.Remove(user);
        _db.SaveChanges();
        return RedirectToAction("UserList", "Account");
    }

    private void GetCartCount()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        int cartCount = 0;

        if (userId != null)
        {
            var order = _db.Orders.OrderByDescending(o => o.OrderId).FirstOrDefault(o => o.UserId == userId && o.TrackingStatus == "Pending");
            if (order != null)
            {
                cartCount = _db.OrderItems.Where(i => i.OrderId == order.OrderId).Sum(i => i.Quantity);
            }
        }
        ViewBag.CartCount = cartCount;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
