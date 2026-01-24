using BCrypt.Net;
using Bookstore.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Bookstore.Controllers
{
    public class AccountController : Controller
    {
        private readonly QuanlybansachContext _context;

        public AccountController(QuanlybansachContext context)
        {
            _context = context;
        }

        // ĐĂNG NHẬP

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Tìm user dựa trên Email
                var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

                // Kiểm tra mật khẩu bằng BCrypt
                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role) // nạp role từ db vào claim
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    // Phân quyền điều hướng
                    if (user.Role == "Admin")
                        return RedirectToAction("Index", "AdminDashboard");

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Email hoặc mật khẩu không chính xác");
            }
            return View(model);
        }

        // ĐĂNG KÝ

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Kiểm tra Email đã tồn tại chưa
                var existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                    return View(model);
                }

                // 2. Mã hóa mật khẩu bằng BCrypt
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                // 3. Tạo thực thể User mới
                var newUser = new User
                {
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    Password = hashedPassword, // Lưu mật khẩu đã mã hóa
                    Role = "Customer"          // Mặc định là khách hàng
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            return View(model);
        }

        // ĐĂNG XUẤT

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // TỪ CHỐI TRUY CẬP

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // QUẢN LÝ TÀI KHOẢN
        [Authorize]
        public IActionResult Profile()
        {
            // Lấy Email của người đang đăng nhập từ Claim
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            // Truy vấn thông tin User kèm theo danh sách địa chỉ giao hàng
            var userProfile = _context.Users
                .Include(u => u.Deliveryinformations)
                .FirstOrDefault(u => u.Email == userEmail);

            if (userProfile == null) return NotFound();

            return View(userProfile);
        }

        // CHỈNH SỬA HỒ SƠ
        [HttpGet]
        [Authorize]
        public IActionResult EditProfile()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(User model)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);

            if (user != null)
            {
                user.Name = model.Name;
                user.Phone = model.Phone;
        // Không cho phép đổi Email ở đây nếu bạn dùng Email làm định danh
        
        _context.Update(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("Profile");
            }
            return View(model);
        }

        // ĐỔI MẬT KHẨU
        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword() => View();

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Lấy Email người dùng hiện tại từ Claims
                var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);

                if (user != null)
                {
                    // 2. Kiểm tra mật khẩu cũ có khớp với mật khẩu đã băm trong DB không
                    if (BCrypt.Net.BCrypt.Verify(model.OldPassword, user.Password))
                    {
                        // 3. Nếu đúng, băm mật khẩu mới và lưu lại
                        user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                        _context.Update(user);
                        await _context.SaveChangesAsync();

                        TempData["Success"] = "Đổi mật khẩu thành công!";
                        return RedirectToAction("Profile");
                    }
                    else
                    {
                        ModelState.AddModelError("OldPassword", "Mật khẩu hiện tại không chính xác.");
                    }
                }
            }
            return View(model);
        }



        // HIỂN THỊ DANH SÁCH
        [Authorize]
        public IActionResult Addresses()
        {
            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null) return NotFound();

            var addresses = _context.Deliveryinformations
                                    .Where(d => d.UserId == user.Id)
                                    .ToList();
            return View(addresses);
        }

        // FORM THÊM MỚI (GET)
        [HttpGet]
        [Authorize]
        public IActionResult AddAddress() => View();

        // XỬ LÝ LƯU THÊM MỚI (POST)
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAddress(Deliveryinformation model)
        {
            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);

            if (user != null)
            {
                model.UserId = user.Id; // QUAN TRỌNG: Gán ID người dùng
                _context.Deliveryinformations.Add(model); // Phải dùng .Add cho thêm mới
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm địa chỉ giao hàng thành công!";
                return RedirectToAction("Addresses");
            }
            return View(model);
        }

        // FORM CHỈNH SỬA (GET)
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditAddress(int id)
        {
            var address = await _context.Deliveryinformations.FindAsync(id);
            if (address == null) return NotFound();
            return View(address);
        }

        // XỬ LÝ LƯU CHỈNH SỬA (POST)
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAddress(Deliveryinformation model)
        {
            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật địa chỉ thành công!";
                return RedirectToAction("Addresses");
            }
            return View(model);
        }

        // XÓA (DELETE)
        [Authorize]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var address = await _context.Deliveryinformations.FindAsync(id);
            if (address != null)
            {
                _context.Deliveryinformations.Remove(address);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xóa địa chỉ thành công!";
            }
            return RedirectToAction("Addresses");
        }
    }
}