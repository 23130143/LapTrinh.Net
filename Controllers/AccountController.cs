using BCrypt.Net;
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
        // ===================== LOGIN =====================

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

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

        // ===================== REGISTER =====================

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
                ModelState.AddModelError("Email", "Email đã tồn tại");
                return View(model);
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

        // ===================== LOGOUT =====================

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );
            return RedirectToAction("Login");
        }

        // ===================== PROFILE =====================

        [Authorize]
        public IActionResult Profile()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = FakeData.Users.FirstOrDefault(u => u.Email == email);
            if (user == null) return NotFound();
            return View(user);
        }

        // ===================== EDIT PROFILE =====================

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
                TempData["Success"] = "Cập nhật thành công!";
                return RedirectToAction("Profile");
            }

            return View(model);
        }

        // ===================== CHANGE PASSWORD =====================

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
                user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                TempData["Success"] = "Đổi mật khẩu thành công!";
                return RedirectToAction("Profile");
            }

            ModelState.AddModelError("OldPassword", "Mật khẩu cũ không đúng");
            return View(model);
        }

        // ===================== ADDRESS =====================

        [Authorize]
        public IActionResult Addresses()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = FakeData.Users.FirstOrDefault(u => u.Email == email);
            return View(user?.Deliveryinformations ?? new List<Deliveryinformation>());
        }

        [Authorize]
        [HttpGet]
        public IActionResult AddAddress() => View();

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddAddress(Deliveryinformation model)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = FakeData.Users.FirstOrDefault(u => u.Email == email);

            if (user != null)
            {
                model.Id = user.Deliveryinformations.Count + 1;
                model.UserId = user.Id;
                user.Deliveryinformations.Add(model);
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
