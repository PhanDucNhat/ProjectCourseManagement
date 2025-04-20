using Microsoft.AspNetCore.Mvc;
using ProjectCourseManagement.Areas.Admin.Models;
using ProjectCourseManagement.Models;
using ProjectCourseManagement.Utilities;

namespace ProjectCourseManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RegisterController : Controller
    {
        private readonly DataContext _context;

        public RegisterController(DataContext context)
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

            // Kiểm tra sự tồn tại của email trong CSDL
            var check = _context.User.Where(m => m.Email == user.Email).FirstOrDefault();
            if (check != null)
            {
                // Hiển thị thông báo, có thể làm cách khác
                Functions._MessageEmail = "Duplicate Email!";
                return RedirectToAction("Index", "Register");
            }

            // Nếu không có thì thêm vào CSDL
            Functions._MessageEmail = string.Empty;
            user.PasswordHash = Functions.MD5Password(user.PasswordHash);
            _context.User.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Index", "Login");
        }
    }
}
