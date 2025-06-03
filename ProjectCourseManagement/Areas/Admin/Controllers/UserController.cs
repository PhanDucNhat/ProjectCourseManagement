using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectCourseManagement.Models;
using ProjectCourseManagement.Utilities;

namespace ProjectCourseManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly DataContext _context;
        public UserController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchString)
        {
            var usList = from u in _context.User
                         select u;

            if (!string.IsNullOrEmpty(searchString))
            {
                usList = usList.Where(u =>
                    u.FullName.Contains(searchString) ||
                    u.Email.Contains(searchString) ||
                    u.PhoneNumber.Contains(searchString) ||
                    u.Role.Contains(searchString) ||
                    u.Status.Contains(searchString));
            }

            ViewBag.CurrentFilter = searchString;
            return View(usList.OrderBy(u => u.UserId).ToList());
        }

        public IActionResult Create()
        {
            var usList = (from m in _context.User
                          select new SelectListItem()
                          {
                              Text = m.FullName,
                              Value = m.UserId.ToString(),
                          }).ToList();
            usList.Insert(0, new SelectListItem()
            {
                Text = "-----Select-----",
                Value = "0"
            });
            ViewBag.usList = usList;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Users us)
        {
            if (ModelState.IsValid)
            {
                _context.User.Add(us);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(us);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var mn = _context.User.Find(id);
            if (mn == null)
            {
                return NotFound();
            }
            var usList = (from m in _context.User
                          select new SelectListItem()
                          {
                              Text = m.FullName,
                              Value = m.UserId.ToString(),
                          }).ToList();
            usList.Insert(0, new SelectListItem()
            {
                Text = "----Select----",
                Value = string.Empty
            });
            ViewBag.usList = usList;
            return View(mn);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Users mn)
        {
            if (ModelState.IsValid)
            {
                _context.User.Update(mn);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mn);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var mn = _context.User.Find(id);

            if (mn == null)
            {
                return NotFound();
            }
            return View(mn);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var deleUser = _context.User.Find(id);
            if (deleUser == null)
            {
                return NotFound();
            }
            _context.User.Remove(deleUser);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            if (string.IsNullOrEmpty(CurrentPassword) || string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(ConfirmPassword))
            {
                TempData["ErrorMessage"] = "Vui lòng điền đầy đủ các trường";
                return View();
            }

            if (NewPassword != ConfirmPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu xác nhận không khớp";
                return View();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập lại";
                return View();
            }

            var user = _context.User.FirstOrDefault(u => u.UserId == int.Parse(userId));
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng";
                return View();
            }

            if (user.PasswordHash != Functions.MD5Password(CurrentPassword))
            {
                TempData["ErrorMessage"] = "Mật khẩu hiện tại không đúng";
                return View();
            }

            user.PasswordHash = Functions.MD5Password(NewPassword);
            user.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Đổi mật khẩu thành công";
            return RedirectToAction("ChangePassword");
        }
    }
}
