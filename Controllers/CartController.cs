using Bookstore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json; // Thêm thư viện này

public class CartController : Controller
{
    public IActionResult Index()
    {
        // 1. Lấy chuỗi JSON từ Session bằng HttpContext.Session
        var sessionData = HttpContext.Session.GetString("Cart");

        List<Shoppingcart> cartItems;

        if (string.IsNullOrEmpty(sessionData))
        {
            cartItems = new List<Shoppingcart>();
        }
        else
        {
            // 2. Giải mã (Deserialize) chuỗi JSON thành List object
            cartItems = JsonSerializer.Deserialize<List<Shoppingcart>>(sessionData);
        }

        // 3. Khởi tạo ViewModel
        var viewModel = new CartViewModel
        {
            CartItems = cartItems
        };

        return View(viewModel);
    }
}