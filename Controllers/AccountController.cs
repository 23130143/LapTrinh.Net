
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

        // ===================================================================
        // ĐĂNG NHẬP
        // ===================================================================

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

                // Kiểm tra Email và Password (Plain Text - không mã hóa)
                if (user != null && user.Password == model.Password)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    // ĐIỀU HƯỚNG DỰA TRÊN VAI TRÒ
                    if (user.Role == "Admin")
                    {
                        return RedirectToAction("Index", "AdminDashboard");
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Email hoặc mật khẩu không chính xác");
            }
            return View(model);
        }

        // ===================================================================
        // ĐĂNG KÝ
        // ===================================================================

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra Email đã tồn tại chưa
                var existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                    return View(model);
                }

                // Tạo User mới với password plain text
                var newUser = new User
                {
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    Password = model.Password,  // Lưu plain text
                    Role = "Customer"           // Mặc định là khách hàng
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            return View(model);
        }

        // ===================================================================
        // ĐĂNG XUẤT
        // ===================================================================

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // ===================================================================
        // TỪ CHỐI TRUY CẬP
        // ===================================================================

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // ===================================================================
        // HỒ SƠ CÁ NHÂN
        // ===================================================================

        [Authorize]
        public IActionResult Profile()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userProfile = _context.Users
                .Include(u => u.Deliveryinformations)
                .FirstOrDefault(u => u.Email == userEmail);

            if (userProfile == null) return NotFound();

            return View(userProfile);
        }

        // ===================================================================
        // CHỈNH SỬA HỒ SƠ
        // ===================================================================

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

                _context.Update(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("Profile");
            }
            return View(model);
        }

        // ===================================================================
        // ĐỔI MẬT KHẨU
        // ===================================================================

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
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);

                if (user != null)
                {
                    // Kiểm tra mật khẩu cũ (plain text)
                    if (user.Password == model.OldPassword)
                    {
                        // Lưu mật khẩu mới (plain text)
                        user.Password = model.NewPassword;
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

        // ===================================================================
        // QUẢN LÝ ĐỊA CHỈ GIAO HÀNG
        // ===================================================================

        [Authorize]
        public IActionResult Addresses()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null) return NotFound();

            var addresses = _context.Deliveryinformations
                                    .Where(d => d.UserId == user.Id)
                                    .ToList();
            return View(addresses);
        }

        [HttpGet]
        [Authorize]
        public IActionResult AddAddress() => View();

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAddress(Deliveryinformation model)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);

            if (user != null && ModelState.IsValid)
            {
                model.UserId = user.Id;
                _context.Deliveryinformations.Add(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Thêm địa chỉ thành công!";
                return RedirectToAction("Addresses");
            }
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditAddress(int id)
        {
            var address = await _context.Deliveryinformations.FindAsync(id);
            if (address == null) return NotFound();
            return View(address);
        }

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

        // Giao diện trang Quên mật khẩu
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // Xử lý gửi mail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Bước A: Kiểm tra email có tồn tại trong DB không
                // var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

                // Giả sử tìm thấy user:
                bool userExists = true; // Thay bằng logic DB của nhóm bạn

                if (userExists)
                {
                    // Bước B: Logic gửi Email (Sử dụng MailKit hoặc một Service gửi mail)
                    // await _emailService.SendEmailAsync(model.Email, "Reset Password", "Mã xác nhận của bạn là: 123456");

                    TempData["SuccessMessage"] = "Một hướng dẫn đã được gửi đến Email của bạn.";
                    return RedirectToAction("ForgotPassword");
                }

                ModelState.AddModelError("", "Email không tồn tại trong hệ thống.");
            }
            return View(model);
        }
    }
}