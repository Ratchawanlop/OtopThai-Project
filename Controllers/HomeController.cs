using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System.Diagnostics;

namespace WebApplication1.Controllers;

public class HomeController : Controller
{
    public IActionResult SPU()
    {
        string name = "ราชวัลลภ นาว์เพ็ชร์";
        string room = "903";
        int year = 3;
        string favLang = "Javascript";

        int s1 = 4;
        int s2 = 5;
        int s3 = 3;
        int s4 = 5;
        int s5 = 4;
        int s6 = 5;
        int s7 = 5;
        int s8 = 5;
        int s9 = 5;
        int s10 = 5;

        int totalScore = s1 + s2 + s3 + s4 + s5 + s6 + s7 + s8 + s9 + s10;

        string grade = "";
        if (totalScore >= 80) grade = "A";
        else if (totalScore >= 76) grade = "B+";
        else if (totalScore >= 70) grade = "B";
        else if (totalScore >= 66) grade = "C+";
        else if (totalScore >= 60) grade = "C";
        else if (totalScore >= 56) grade = "D+";
        else if (totalScore >= 50) grade = "D";
        else grade = "F";

        string lessthanfive = "";
        if (grade == "F")
        {
            string list = "";
            if (s1 < 5) list += "งานที่ 1 ";
            if (s2 < 5) list += "งานที่ 2 ";
            if (s3 < 5) list += "งานที่ 3 ";
            if (s4 < 5) list += "งานที่ 4 ";
            if (s5 < 5) list += "งานที่ 5 ";
            if (s6 < 5) list += "งานที่ 6 ";
            if (s7 < 5) list += "งานที่ 7 ";
            if (s8 < 5) list += "งานที่ 8 ";
            if (s9 < 5) list += "งานที่ 9 ";
            if (s10 < 5) list += "งานที่ 10 ";

            lessthanfive = "ชิ้นงานที่น้อยกว่า 5: " + list;
        }
        ViewBag.StudentDetails = "ชื่อ: " + name + " ห้อง: " + room + " ชั้นปี: " + year + " ภาษาที่ถนัด: " + favLang;
        ViewBag.Total = totalScore;
        ViewBag.Grade = grade;
        ViewBag.Comment = lessthanfive;

        return View();
    }

    public IActionResult Homepage(string Phone_number, string Password_hash)
    {
        ViewBag.Phone_number = Phone_number;
        ViewBag.Password_hash = Password_hash;
        return View();
    }

    public IActionResult Login()
    {
        return View();
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
