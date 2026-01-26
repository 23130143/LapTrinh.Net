
using Bookstore.Models;
using Bookstore.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
                // Tìm user dựa trên Email
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

                    // Phân quyền điều hướng
                    if (user.Role == "Admin")
                        return RedirectToAction("Index", "AdminDashboard");

            var user = FakeData.Users.FirstOrDefault(u => u.Email == model.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role ?? "Customer")
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity)
            );

            return RedirectToAction("Index", "Home");
        }

        // ===================================================================
        // ĐĂNG KÝ
        // ===================================================================

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (FakeData.Users.Any(u => u.Email == model.Email))
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

            var newUser = new User
            {
                Id = FakeData.Users.Count + 1,
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = "Customer",
                Deliveryinformations = new List<Deliveryinformation>()
            };

            FakeData.Users.Add(newUser);

            TempData["Success"] = "Đăng ký thành công!";
            return RedirectToAction("Login");
        }

        // ===================================================================
        // ĐĂNG XUẤT
        // ===================================================================

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );
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
        [HttpGet]
        public IActionResult EditProfile()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = FakeData.Users.FirstOrDefault(u => u.Email == email);
            if (user == null) return NotFound();
            return View(user);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(User model)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = FakeData.Users.FirstOrDefault(u => u.Email == email);

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
        [HttpGet]
        public IActionResult ChangePassword() => View();

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = FakeData.Users.FirstOrDefault(u => u.Email == email);

            if (user != null && BCrypt.Net.BCrypt.Verify(model.OldPassword, user.Password))
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

            ModelState.AddModelError("OldPassword", "Mật khẩu cũ không đúng");
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
        [HttpGet]
        public IActionResult AddAddress() => View();

        [HttpPost]
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddAddress(Deliveryinformation model)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);

            if (user != null)
            {
                model.UserId = user.Id;
                _context.Deliveryinformations.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm địa chỉ giao hàng thành công!";
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
        public IActionResult DeleteAddress(int id)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = FakeData.Users.FirstOrDefault(u => u.Email == email);

            var address = user?.Deliveryinformations.FirstOrDefault(d => d.Id == id);
            if (address != null)
            {
                user.Deliveryinformations.Remove(address);
            }

            return RedirectToAction("Addresses");
        }
    }
}
