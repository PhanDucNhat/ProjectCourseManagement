using Microsoft.AspNetCore.Mvc;
using ProjectCourseManagement.Models;

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

        public IActionResult Index()
        {
            var mnList = _context.User.OrderBy(m => m.UserId).ToList();
            return View(mnList);
        }
    }
}
