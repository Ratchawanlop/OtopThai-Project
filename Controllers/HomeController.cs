using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Xml;
using WebApplication1.Models;
using WebApplication1.Models.Db;
using WebApplication1.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Controllers;

public class HomeController : Controller
{
    private readonly Csi402dbContext _db;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public HomeController(Csi402dbContext db, IWebHostEnvironment webHostEnvironment)
    {
        _db = db;

        _webHostEnvironment = webHostEnvironment;
    }

    [HttpPost]
    public IActionResult AddToCart(int productId, int quantity)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            TempData["Error"] = "กรุณาเข้าสู่ระบบก่อนหยิบสินค้าใส่ตะกร้า";
            return RedirectToAction("Login", "Home");
        }

        // หาตะกร้าปัจจุบัน (ที่สถานะ Pending) หรือ สร้างใหม่ถ้ายังไม่มี
        var order = _db.Orders.OrderByDescending(o => o.OrderId).FirstOrDefault(o => o.UserId == userId && o.TrackingStatus == "Pending");
        if (order == null)
        {
            order = new Order
            {
                UserId = userId.Value,
                OrderDatetime = DateTime.Now,
                Subtotal = 0,
                NetAmount = 0,
                TrackingStatus = "Pending"
            };
            _db.Orders.Add(order);
            _db.SaveChanges();
        }

        var product = _db.Products.Find(productId);
        if (product != null)
        {
            if (product.StockQuantity <= 0)
            {
                TempData["Error"] = $"ขออภัยครับ {product.ProductName} สินค้าหมดชั่วคราว";
                return RedirectToAction("Product");
            }

            var existingItem = _db.OrderItems.FirstOrDefault(i => i.OrderId == order.OrderId && i.ProductId == productId);

            int currentQtyInCart = existingItem != null ? existingItem.Quantity : 0;

            if (currentQtyInCart + quantity > product.StockQuantity)
            {
                TempData["Error"] = $"ขออภัยครับ สินค้าชิ้นนี้มีเหลือเพียง {product.StockQuantity} ชิ้น";
                return RedirectToAction("Product");
            }

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var newItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = product.ProductPrice
                };
                _db.OrderItems.Add(newItem);
            }

            order.Subtotal += (product.ProductPrice * quantity);
            order.NetAmount = order.Subtotal;

            _db.SaveChanges();
            TempData["Success"] = $"เพิ่ม {product.ProductName} ลงตะกร้าแล้ว";
        }

        return RedirectToAction("Product");
    }

    public IActionResult Checkout()
    {
        GetCartCount();
        var currentUserId = HttpContext.Session.GetInt32("UserId");
        if (currentUserId == null) return RedirectToAction("Login", "Account");

        var myOrder = _db.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefault(o => o.UserId == currentUserId && o.TrackingStatus == "Pending");

        if (myOrder == null || myOrder.OrderItems == null || !myOrder.OrderItems.Any())
        {
            return RedirectToAction("Cart");
        }

        var currentUser = _db.Users.FirstOrDefault(u => u.UserId == currentUserId);
        ViewBag.UserAddress = currentUser?.AddressMain;

        return View(myOrder);
    }
    public IActionResult CheckoutSuccess()
    {
        GetCartCount();
        return View();
    }

    public IActionResult OrderHistory()
    {
        GetCartCount();
        var currentUserId = HttpContext.Session.GetInt32("UserId");
        if (currentUserId == null) return RedirectToAction("Login", "Account");

        var myOrders = _db.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Include(o => o.ProductReviews)
            .Where(o => o.UserId == currentUserId && o.TrackingStatus != "Pending")
            .OrderByDescending(o => o.OrderDatetime)
            .ToList();

        return View(myOrders);
    }

    public IActionResult OrderDetail(int orderId)
    {
        GetCartCount();
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Account"); 
        }

        var order = _db.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefault(o => o.OrderId == orderId && o.UserId == userId);

        if (order == null)
        {
            TempData["Error"] = "ไม่พบข้อมูลคำสั่งซื้อ";
            return RedirectToAction("OrderHistory");
        }

        return View(order);
    }

    public IActionResult Cart()
    {
        GetCartCount();
        var currentUserId = HttpContext.Session.GetInt32("UserId");
        if (currentUserId == null)
        {
            return RedirectToAction("Login", "Account");
        }
        var myCart = _db.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.Product)
            .OrderByDescending(o => o.OrderId)
            .FirstOrDefault(o => o.UserId == currentUserId && o.TrackingStatus == "Pending");
        if (myCart == null || myCart.OrderItems == null || !myCart.OrderItems.Any())
        {
            return View(myCart);
        }
        decimal subTotal = (decimal)myCart.OrderItems.Sum(x => x.UnitPrice * x.Quantity);
        decimal basketryTotal = (decimal)myCart.OrderItems.Where(x => x.Product.CategoryName == "งานสาน").Sum(x => x.UnitPrice * x.Quantity);
        decimal textileTotal = (decimal)myCart.OrderItems.Where(x => x.Product.CategoryName == "ผ้าทอมือ").Sum(x => x.UnitPrice * x.Quantity);
        decimal souvenirTotal = (decimal)myCart.OrderItems.Where(x => x.Product.CategoryName == "ของที่ระลึก").Sum(x => x.UnitPrice * x.Quantity);
        var activePromos = _db.Promotions
            .Where(p => p.IsActive &&
                        (!p.StartDate.HasValue || p.StartDate <= DateTime.Now) &&
                        (!p.EndDate.HasValue || p.EndDate >= DateTime.Now))
            .ToList();
        decimal totalDiscount = 0;
        decimal discountBasketry = 0;
        var promoBasketry = activePromos
            .Where(p => p.PromotionType == "CategoryDiscount" && p.TargetCategoryName == "งานสาน")
            .OrderByDescending(p => p.MinPurchaseAmount)
            .FirstOrDefault(p => basketryTotal >= p.MinPurchaseAmount);
        if (promoBasketry != null)
        {
            discountBasketry = promoBasketry.IsPercentage
                ? basketryTotal * (promoBasketry.DiscountValue / 100m)
                : promoBasketry.DiscountValue;
        }
        decimal discountTextile = 0;
        var promoTextile = activePromos
            .Where(p => p.PromotionType == "CategoryDiscount" && p.TargetCategoryName == "ผ้าทอมือ")
            .OrderByDescending(p => p.MinPurchaseAmount)
            .FirstOrDefault(p => textileTotal >= p.MinPurchaseAmount);
        if (promoTextile != null)
        {
            discountTextile = promoTextile.IsPercentage
                ? textileTotal * (promoTextile.DiscountValue / 100m)
                : promoTextile.DiscountValue;
        }
        decimal discountSouvenir = 0;
        var promoSouvenir = activePromos
            .Where(p => p.PromotionType == "CategoryDiscount" && p.TargetCategoryName == "ของที่ระลึก")
            .OrderByDescending(p => p.MinPurchaseAmount)
            .FirstOrDefault(p => souvenirTotal >= p.MinPurchaseAmount);
        if (promoSouvenir != null)
        {
            discountSouvenir = promoSouvenir.IsPercentage
                ? souvenirTotal * (promoSouvenir.DiscountValue / 100m)
                : promoSouvenir.DiscountValue;
        }
        decimal bundleDiscount = 0;
        Promotion? promoBundle = null;

        if (basketryTotal > 0 && souvenirTotal > 0)
        {
            decimal combinedTotal = basketryTotal + souvenirTotal;
            promoBundle = activePromos
                .Where(p => p.PromotionType == "BundleDiscount"
                         && p.TargetCategoryName == "งานสาน"
                         && p.BundleCategoryName == "ของที่ระลึก")
                .OrderByDescending(p => p.MinPurchaseAmount)
                .FirstOrDefault(p => combinedTotal >= p.MinPurchaseAmount);

            if (promoBundle != null)
            {
                bundleDiscount = promoBundle.IsPercentage
                    ? combinedTotal * (promoBundle.DiscountValue / 100m)
                    : promoBundle.DiscountValue;
            }
        }
        decimal bestTextileSouvenirDiscount = Math.Max((discountTextile + discountSouvenir), bundleDiscount);
        totalDiscount = bestTextileSouvenirDiscount + discountBasketry;
        List<string> appliedPromos = new List<string>();
        if (discountBasketry > 0 && promoBasketry != null)
        {
            appliedPromos.Add(promoBasketry.PromotionName);
        }
        if (bundleDiscount > (discountTextile + discountSouvenir) && promoBundle != null)
        {
            appliedPromos.Add(promoBundle.PromotionName);
        }
        else
        {
            if (discountTextile > 0 && promoTextile != null)
                appliedPromos.Add(promoTextile.PromotionName);

            if (discountSouvenir > 0 && promoSouvenir != null)
                appliedPromos.Add(promoSouvenir.PromotionName);
        }

        if (appliedPromos.Any())
        {
            ViewBag.PromotionName = string.Join(" + ", appliedPromos);
        }
        var freeShippingPromo = activePromos
        .FirstOrDefault(p => p.PromotionType == "FreeShipping" && subTotal >= p.MinPurchaseAmount);
        decimal shippingFee = (freeShippingPromo != null) ? 0 : 50m;
        decimal netAmount = subTotal - totalDiscount + shippingFee;

        myCart.Subtotal = subTotal;
        myCart.DiscountAmount = totalDiscount;
        myCart.ShippingCost = shippingFee;
        myCart.NetAmount = netAmount;
        _db.SaveChanges();
        return View(myCart);
    }

    [HttpPost]
    public IActionResult RemoveFromCart(int productId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToAction("Login", "Home");

        var order = _db.Orders.FirstOrDefault(o => o.UserId == userId && o.TrackingStatus == "Pending");
        if (order != null)
        {
            var item = _db.OrderItems.FirstOrDefault(i => i.OrderId == order.OrderId && i.ProductId == productId);
            if (item != null)
            {
                _db.OrderItems.Remove(item);
                _db.SaveChanges();

                order.Subtotal = _db.OrderItems.Where(i => i.OrderId == order.OrderId).Sum(i => i.Quantity * i.UnitPrice);
                order.NetAmount = order.Subtotal;
                _db.SaveChanges();
            }
        }
        return RedirectToAction("Cart");
    }

    [HttpPost]
    public IActionResult UpdateCartQuantity(int productId, int quantity)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToAction("Login", "Home");

        var order = _db.Orders.FirstOrDefault(o => o.UserId == userId && o.TrackingStatus == "Pending");
        var product = _db.Products.Find(productId);

        if (order != null && product != null)
        {
            var item = _db.OrderItems.FirstOrDefault(i => i.OrderId == order.OrderId && i.ProductId == productId);
            if (item != null)
            {
                if (quantity > 0)
                {
                    if (quantity > product.StockQuantity)
                    {
                        TempData["Error"] = $"ขออภัยครับ สินค้าชิ้นนี้มีเหลือเพียง {product.StockQuantity} ชิ้น";
                        return RedirectToAction("Cart");
                    }

                    item.Quantity = quantity;
                }
                else
                {
                    _db.OrderItems.Remove(item);
                }
                _db.SaveChanges();

                order.Subtotal = _db.OrderItems.Where(i => i.OrderId == order.OrderId).Sum(i => i.Quantity * i.UnitPrice);
                order.NetAmount = order.Subtotal;
                _db.SaveChanges();
            }
        }
        return RedirectToAction("Cart");
    }

    public IActionResult ProductDetail(int id)
    {
        GetCartCount();
        var product = _db.Products.FirstOrDefault(p => p.ProductId == id);
        if (product == null)
        {
            return NotFound();
        }

        var reviews = _db.ProductReviews
                              .Where(r => r.ProductId == id)
                              .OrderByDescending(r => r.ReviewDate)
                              .ToList();

        ViewBag.Reviews = reviews;

        return View(product);
    }

    [HttpPost]
public async Task<IActionResult> SubmitCheckout(string address, IFormFile? slipImage, string shippingOption, string paymentMethod)
{
    GetCartCount();
    var currentUserId = HttpContext.Session.GetInt32("UserId");
    if (currentUserId == null) return RedirectToAction("Login", "Account");

    var myOrder = _db.Orders
        .Include(o => o.OrderItems)
        .ThenInclude(oi => oi.Product)
        .FirstOrDefault(o => o.UserId == currentUserId && o.TrackingStatus == "Pending");

    if (myOrder == null) return RedirectToAction("Cart");

    if (shippingOption == "default")
    {
        var user = _db.Users.FirstOrDefault(u => u.UserId == currentUserId);
        myOrder.ShippingAddress = user?.AddressMain;
    }
    else
    {
        myOrder.ShippingAddress = address;
    }

    string? savedSlipUrl = null;

    if (paymentMethod == "เก็บเงินปลายทาง")
    {
        myOrder.PaymentSlipUrl = null;
        myOrder.TrackingStatus = "Preparing";

        if (myOrder.OrderItems != null)
        {
            foreach (var item in myOrder.OrderItems)
            {
                if (item.Product != null)
                {
                    item.Product.StockQuantity -= item.Quantity; // ตัดสต๊อก
                    if (item.Product.StockQuantity < 0) 
                    {
                        item.Product.StockQuantity = 0;
                    }
                }
            }
        }
    }
    else
    {
        if (slipImage != null && slipImage.Length > 0)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "slips");
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(slipImage.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await slipImage.CopyToAsync(fileStream);
            }

            savedSlipUrl = "/uploads/slips/" + uniqueFileName;
            myOrder.PaymentSlipUrl = savedSlipUrl;
        }
        myOrder.TrackingStatus = "WaitingForReview";
    }

    myOrder.OrderDatetime = DateTime.Now;

    var paymentRecord = new Payment
    {
        OrderId = myOrder.OrderId,
        PaymentMethod = paymentMethod, 
        Amount = myOrder.NetAmount, 
        ReceiptImageUrl = savedSlipUrl,
        Paymentdate = DateTime.Now,
        PaymentStatus = (paymentMethod == "เก็บเงินปลายทาง") ? 0 : 1 
    };

    _db.Payments.Add(paymentRecord);
    _db.SaveChanges();

    return RedirectToAction("CheckoutSuccess");
    }

    [HttpPost]
    public IActionResult ConfirmReceiveOrder(int orderId)
    {
        var order = _db.Orders.Find(orderId);

        if (order != null && (order.TrackingStatus == "Shipped" || order.TrackingStatus == "กำลังจัดส่งสินค้า"))
        {
            order.TrackingStatus = "Completed";
            _db.SaveChanges();
        }

        return RedirectToAction("OrderHistory");
    }

    [HttpPost]
    public IActionResult AddReview(int productId, int orderId, int starScore, string? comment)
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            TempData["Error"] = "กรุณาล็อกอินเข้าสู่ระบบก่อนร่วมแสดงความคิดเห็นครับ/ค่ะ";
            return RedirectToAction("ProductReview", new { orderId = orderId });
        }

        var user = _db.Users.FirstOrDefault(u => u.UserId == userId);
        string loggedInName = user != null ? user.Username : "สมาชิกไม่ทราบชื่อ";

        if (starScore < 1 || starScore > 5)
        {
            TempData["Error"] = "กรุณาให้คะแนน 1-5 ดาว";
            return RedirectToAction("ProductReview", new { orderId = orderId });
        }

        var newReview = new ProductReview
        {
            ProductId = productId,
            OrderId = orderId,
            UserId = userId,
            ReviewerName = loggedInName,
            StarScore = starScore,
            Comment = string.IsNullOrWhiteSpace(comment) ? "ไม่ได้ระบุความคิดเห็น" : comment,
            ReviewDate = DateTime.Now
        };

        _db.ProductReviews.Add(newReview);
        _db.SaveChanges();

        var allReviews = _db.ProductReviews.Where(r => r.ProductId == productId).ToList();
        var product = _db.Products.Find(productId);

        if (product != null && allReviews.Any())
        {
            product.RatingScore = (decimal)allReviews.Average(r => r.StarScore);
            product.ReviewCount = allReviews.Count;
            _db.SaveChanges();
        }

        int totalItems = _db.OrderItems.Count(i => i.OrderId == orderId);
        int reviewedItems = _db.ProductReviews.Count(r => r.OrderId == orderId);

        if (reviewedItems >= totalItems)
        {
            TempData["ReviewSuccess"] = "รีวิวสินค้าในคำสั่งซื้อนี้เรียบร้อยแล้ว ขอบคุณครับ/ค่ะ!";
            return RedirectToAction("OrderHistory");
        }

        TempData["Success"] = "บันทึกรีวิวสำเร็จ!";
        return RedirectToAction("ProductReview", new { orderId = orderId });
    }
    [HttpPost]
    public IActionResult SkipReview(int productId, int orderId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var user = _db.Users.FirstOrDefault(u => u.UserId == userId);
        string loggedInName = user != null ? user.Username : "ไม่ทราบชื่อ";

        var skipReview = new ProductReview
        {
            ProductId = productId,
            OrderId = orderId,
            UserId = userId,
            ReviewerName = loggedInName,
            StarScore = 1,
            Comment = "ไม่ได้ระบุความคิดเห็น",
            ReviewDate = DateTime.Now
        };

        _db.ProductReviews.Add(skipReview);
        _db.SaveChanges();

        int totalItems = _db.OrderItems.Count(i => i.OrderId == orderId);
        int reviewedItems = _db.ProductReviews.Count(r => r.OrderId == orderId);

        if (reviewedItems >= totalItems)
        {
            TempData["ReviewSuccess"] = "จัดการรีวิวสินค้าในคำสั่งซื้อนี้เรียบร้อยแล้ว ขอบคุณครับ!";
            return RedirectToAction("OrderHistory");
        }

        TempData["Success"] = "ข้ามการรีวิวสินค้าเรียบร้อย";
        return RedirectToAction("ProductReview", new { orderId = orderId });
    }

    public IActionResult ProductReview(int orderId)
    {
        GetCartCount();
        var order = _db.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Include(o => o.ProductReviews)
            .FirstOrDefault(o => o.OrderId == orderId);

        if (order == null)
        {
            return RedirectToAction("OrderHistory");
        }

        return View(order);
    }

    public IActionResult Product(string category)
    {
        GetCartCount();
        var products = _db.Products.AsQueryable();

        if (!string.IsNullOrEmpty(category))
        {
            products = products.Where(p => p.CategoryName == category);

            ViewBag.CurrentCategory = category;
        }
        else
        {
            ViewBag.CurrentCategory = "สินค้าทั้งหมด";
        }
        return View(products.ToList());
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

    public IActionResult Homepage()
    {
        GetCartCount();
        var popularProducts = _db.Products.Take(4).ToList();
        return View(popularProducts);
    }

public IActionResult ProfileSettings()
{
    GetCartCount();
    var userId = HttpContext.Session.GetInt32("UserId");
    if (userId == null) return RedirectToAction("Login", "Home");

    var user = _db.Users.Find(userId);
    if (user == null) return RedirectToAction("Login", "Home");

    return View(user);
}

[HttpPost]
public IActionResult ProfileSettings(string name, string email, string address)
{
    var userId = HttpContext.Session.GetInt32("UserId");
    if (userId == null) return RedirectToAction("Login", "Home");

    var user = _db.Users.Find(userId);
    if (user != null)
    {
        user.Username = name;
        user.Email = email;
        user.AddressMain = address;

        _db.SaveChanges();
        TempData["Success"] = "อัปเดตข้อมูลโปรไฟล์เรียบร้อยแล้ว!";

        HttpContext.Session.SetString("Username", user.Username ?? "ผู้ใช้งาน");
    }

    return RedirectToAction("ProfileSettings");
}

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}