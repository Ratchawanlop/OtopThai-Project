using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System.Diagnostics;
using WebApplication1.Viewmodels;
using WebApplication1.ViewModels;
using WebApplication1.Models.Db;
using Microsoft.EntityFrameworkCore;


namespace WebApplication1.Controllers;

public class AdminController : Controller
{
    private readonly Csi402dbContext _db;
    public AdminController(Csi402dbContext db)
    {
        _db = db;
    }

    public IActionResult Dashboard()
    {
        var products = _db.Products.ToList();
        return View(products);
    }

    public IActionResult ProductList(string categoryFilter, string statusFilter)
    {
        var products = _db.Products.AsQueryable();

        if (!string.IsNullOrEmpty(categoryFilter))
        {
            products = products.Where(p => p.CategoryName == categoryFilter);
        }

        if (!string.IsNullOrEmpty(statusFilter))
        {
            if (statusFilter == "Out of Stock")
            {
                products = products.Where(p => p.StockQuantity == 0);
            }
            else if (statusFilter == "In Stock")
            {
                products = products.Where(p => p.StockQuantity > 0);
            }
            else if (statusFilter == "Low Stock")
            {
                products = products.Where(p => p.StockQuantity > 0 && p.StockQuantity <= 10);
            }
        }

        ViewBag.CategoryList = _db.Products.Select(p => p.CategoryName).Distinct().ToList();

        ViewBag.SelectedCategory = categoryFilter;
        ViewBag.SelectedStatus = statusFilter;

        return View(products.ToList());
    }
        public IActionResult EditProduct(int id)
        {
            var product = _db.Products.Find(id);
            if (product == null)
            {
                TempData["Error"] = "ไม่พบข้อมูลสินค้าที่ต้องการแก้ไข";
                return RedirectToAction("ProductList");
            }
            return View(product);
        }

        [HttpPost]
        public IActionResult EditProduct(int ProductId, string ProductName, string CategoryName, int StockQuantity, decimal ProductPrice, IFormFile? ProductImage)
        {
            var product = _db.Products.Find(ProductId);
            
            if (product != null)
            {
                product.ProductName = ProductName;
                product.CategoryName = CategoryName;
                product.StockQuantity = StockQuantity;
                product.ProductPrice = ProductPrice;

                if (ProductImage != null && ProductImage.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProductImage.FileName);
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        ProductImage.CopyTo(stream);
                    }

                    product.ImageUrl = "/images/" + fileName; 
                }

                _db.SaveChanges();
                TempData["Success"] = "อัปเดตข้อมูลสินค้าเรียบร้อยแล้ว!";
            }

            // เซฟเสร็จให้กลับไปหน้าตารางสินค้า
            return RedirectToAction("ProductList");
        }
    public IActionResult CreateProduct()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateProduct(WebApplication1.Models.Db.Product model)
    {
        if (ModelState.IsValid)
        {
            model.RatingScore = 0;
            model.ReviewCount = 0;

            _db.Products.Add(model);
            _db.SaveChanges();

            return RedirectToAction("ProductList");
        }

        return View(model);
    }

public IActionResult PromotionList()
{
    var promotions = _db.Promotions.ToList();
    return View(promotions);
}

public IActionResult CreatePromotion()
{
    return View();
}

[HttpPost]
public IActionResult CreatePromotion(Promotion data)
{
    if (ModelState.IsValid)
    {
        _db.Promotions.Add(data);
        _db.SaveChanges();
        TempData["Success"] = "เพิ่มโปรโมชั่นเรียบร้อยแล้ว!";
        return RedirectToAction("PromotionList");
    }
    return View(data);
}

public IActionResult DeletePromotion(int id)
{
    var promo = _db.Promotions.Find(id);
    if (promo != null)
    {
        _db.Promotions.Remove(promo);
        _db.SaveChanges();
        TempData["Success"] = "ลบโปรโมชั่นสำเร็จ";
    }
    return RedirectToAction("PromotionList");
}

    [HttpPost]
    public IActionResult ApproveOrder(int orderId)
    {
        var order = _db.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefault(o => o.OrderId == orderId);

        if (order != null && order.TrackingStatus == "WaitingForReview")
        {
            order.TrackingStatus = "Paid";

            if (order.OrderItems != null)
            {
                foreach (var item in order.OrderItems)
                {
                    if (item.Product != null)
                    {
                        item.Product.StockQuantity -= item.Quantity;

                        if (item.Product.StockQuantity < 0)
                        {
                            item.Product.StockQuantity = 0;
                        }
                    }
                }
            }

            _db.SaveChanges();
        }

        return RedirectToAction("OrderList");
    }

    public IActionResult OrderList()
    {
        var orders = _db.Orders
            .Include(o => o.User)
            .Where(o => o.TrackingStatus != "Pending")
            .OrderByDescending(o => o.OrderDatetime)
            .ToList();

        return View(orders);
    }

    public IActionResult OrderDetail(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var order = _db.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefault(o => o.OrderId == id);  

        if (order == null)
        {
            TempData["Error"] = "ไม่พบข้อมูลคำสั่งซื้อ";
            return RedirectToAction("OrderList");
        }

        return View(order);
    }

    [HttpPost]
    public IActionResult UpdateOrderStatus(int orderId, string newStatus)
    {
        var order = _db.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefault(o => o.OrderId == orderId);

        if (order != null)
        {
            if (newStatus == "Paid" && order.TrackingStatus != "Paid")
            {
                if (order.OrderItems != null)
                {
                    foreach (var item in order.OrderItems)
                    {
                        if (item.Product != null)
                        {
                            item.Product.StockQuantity -= item.Quantity;
                            if (item.Product.StockQuantity < 0) item.Product.StockQuantity = 0;
                        }
                    }
                }
            }

            order.TrackingStatus = newStatus;
            _db.SaveChanges();
        }
        return RedirectToAction("OrderList");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
