using Bookstore.Data;
using Bookstore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Bookstore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // DANH SÁCH GIẢ (KHÔNG DB)
        private static List<User> Users = FakeData.Users;

        public IActionResult ManageUsers()
        {
            return View(Users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateRole(int userId, string newRole)
        {
            var user = Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.Role = newRole;
                TempData["Message"] = "Cập nhật quyền hạn thành công!";
            }
            return RedirectToAction("ManageUsers");
        }
    }
}
