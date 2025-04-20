using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCourseManagement.Areas.Admin.Models;
using ProjectCourseManagement.Models;
using ProjectCourseManagement.Utilities;

namespace ProjectCourseManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoginController : Controller
    {
        private readonly DataContext _context;
        public LoginController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Users user)
        {
            if (user == null)
            {
                return NotFound();
            }

            // Mã hóa mật khẩu trước khi kiểm tra
            string pw = Functions.MD5Password(user.PasswordHash);
            // Kiểm tra sự tồn tại của email trong CSDL
            var check = _context.User.Where(m => (m.Email == user.Email) && (m.PasswordHash == pw)).FirstOrDefault();
            if (check == null)
            {
                // Hiển thị thông báo có thể làm cách khác
                Functions._Message = "Invalid UserName or Password!";
                return RedirectToAction("Index", "Login");
            }
            // Vào trang Admin nếu Username và password
            Functions._Message = string.Empty;
            Functions._UserId = check.UserId;
            Functions._FullName = string.IsNullOrEmpty(check.FullName) ? string.Empty : check.FullName;
            Functions._Email = string.IsNullOrEmpty(check.Email) ? string.Empty : check.Email;
            return RedirectToAction("Index", "Home");
        }
    }
}
