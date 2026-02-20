using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System.Diagnostics;
using WebApplication1.Viewmodels;

namespace WebApplication1.Controllers;

public class AccountController : Controller
{

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
        // var User = new LabUserViewModel();
        // User.UserID = "Ask";
        // User.Name = "Aomsin";
        // User.Lastname = "Kub";
        // User.Address = "164/17 naja";
        // User.Weight = 95;
        // User.Height = 175;
        // User.Age = 22; 
        // var User = new List<LabUserViewModel>
        // {
        //   new LabUserViewModel {UserID = "Ask" , Name = "Aomsin" , Lastname = "Kub" , Address = "123" , Weight = 75 , Height = 175 , Age = 20},
        //   new LabUserViewModel {UserID = "Bok", Name = "Meme" , Lastname = "Ka" , Address = "123" , Weight = 150 , Height = 500 , Age = 230},
        //   new LabUserViewModel {UserID = "Custom", Name = "Aroi" , Lastname = "Mak" , Address = "123" , Weight = 500 , Height = 1200 , Age = 1000}
        // };
        return View();
    }
    // [HttpPost]
    // public IActionResult Lab5(LabUserViewModel data)
    // {
    //     string a, b, c;
    //     a = data.UserID;
    //     b = data.Name;
    //     c = data.Lastname;
    //     ViewBag.UserID = a;
    //     ViewBag.Name = b;
    //     ViewBag.Lastname = c;
    //     // return View();
    //     return RedirectToAction("Lab52", "Account", new { UserID = data.UserID, Name = data.Name, Lastname = data.Lastname });
    // }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(LabUserViewModel data)
    {
        string a, b;
        a = data.Phone_number;
        b = data.Password_hash;
        ViewBag.Phone_number = a;
        ViewBag.Password_hash = b;
        return RedirectToAction("Homepage", "Home", new { Phone_number = data.Phone_number, Password_hash = data.Password_hash });
    }


    public IActionResult Register()
    {
        return View();
    }

    public IActionResult Userlist()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
