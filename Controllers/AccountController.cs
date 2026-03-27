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

    public IActionResult Test1()
    {
        return View();
    }

    public IActionResult Test2()
    {
        return View();
    }

    public IActionResult Lab4()
    {
        return View();
    }

    public IActionResult Lab5()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(LoginViewModel data)
    {
        var user = (from u in _db.Users
                    where u.PhoneNumber == data.Phone_number && u.PasswordHash == data.Password
                    select u).FirstOrDefault();

        if (user != null)
        {
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetInt32("RoleId", user.RoleId);

            TempData["Success"] = "เข้าสู่ระบบสำเร็จ!";
            return RedirectToAction("Homepage", "Home");
        }
        else
        {
            TempData["Error"] = "เบอร์โทรศัพท์หรือรหัสผ่านไม่ถูกต้อง";
            return View();
        }
    }

    // สร้างฟังก์ชัน Logout เพิ่มเข้ามา
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Homepage", "Home");
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(RegisterViewModel data)
    {
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
        return RedirectToAction("Userlist", "Account");
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
            user.PasswordHash = data.PasswordHash;
            user.Email = data.Email;
            user.AddressMain = data.AddressMain;
            user.RoleId = data.RoleId;


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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
