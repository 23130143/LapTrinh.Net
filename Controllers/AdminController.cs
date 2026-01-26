using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bookstore.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Bookstore.Controllers
{
    [Authorize(Roles = "Admin")] // Chỉ tài khoản có Role là Admin mới vào được
    public class AdminController : Controller
    {
        private readonly QuanlybansachContext _context;

        public AdminController(QuanlybansachContext context)
        {
            _context = context;
        }

        // Trang hiển thị danh sách người dùng
        public IActionResult ManageUsers()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        // Xử lý thay đổi Role
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRole(int userId, string newRole)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Role = newRole;
                _context.Update(user);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Cập nhật quyền hạn thành công!";
            }
            return RedirectToAction("ManageUsers");
        }

        public IActionResult Books()
        {
            return View();
        }
    }
}