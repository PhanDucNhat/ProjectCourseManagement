using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCourseManagement.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectCourseManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class InstructorController : Controller
    {
        private readonly DataContext _context;

        public InstructorController(DataContext context)
        {
            _context = context;
        }

        // GET: Admin/Instructor
        public async Task<IActionResult> Index()
        {
            var instructors = await _context.Instructor.ToListAsync();
            return View(instructors);
        }

        // GET: Admin/Instructor/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Instructor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Instructor instructor)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(instructor.InstructorName))
                {
                    ModelState.AddModelError("InstructorName", "Tên giảng viên không được để trống.");
                    return View(instructor);
                }

                if (string.IsNullOrEmpty(instructor.Email))
                {
                    ModelState.AddModelError("Email", "Email không được để trống.");
                    return View(instructor);
                }

                if (string.IsNullOrEmpty(instructor.Password))
                {
                    ModelState.AddModelError("Password", "Mật khẩu không được để trống.");
                    return View(instructor);
                }

                instructor.CreatedAt = DateTime.Now;
                _context.Instructor.Add(instructor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(instructor);
        }

        // GET: Admin/Instructor/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var instructor = await _context.Instructor.FindAsync(id);
            if (instructor == null)
            {
                return NotFound();
            }
            return View(instructor);
        }

        // POST: Admin/Instructor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Instructor instructor)
        {
            if (id != instructor.InstructorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(instructor.InstructorName))
                {
                    ModelState.AddModelError("InstructorName", "Tên giảng viên không được để trống.");
                    return View(instructor);
                }

                if (string.IsNullOrEmpty(instructor.Email))
                {
                    ModelState.AddModelError("Email", "Email không được để trống.");
                    return View(instructor);
                }

                if (string.IsNullOrEmpty(instructor.Password))
                {
                    ModelState.AddModelError("Password", "Mật khẩu không được để trống.");
                    return View(instructor);
                }

                try
                {
                    _context.Update(instructor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstructorExists(instructor.InstructorId))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(instructor);
        }

        // GET: Admin/Instructor/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var instructor = await _context.Instructor.FindAsync(id);
            if (instructor == null)
            {
                return NotFound();
            }
            return View(instructor);
        }

        // POST: Admin/Instructor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instructor = await _context.Instructor.FindAsync(id);
            if (instructor != null)
            {
                // Check if instructor is referenced by any course
                var hasCourses = await _context.Course.AnyAsync(c => c.InstructorId == id);
                if (hasCourses)
                {
                    ModelState.AddModelError("", "Không thể xóa giảng viên này vì họ đang được liên kết với một hoặc nhiều khóa học.");
                    return View(instructor);
                }

                _context.Instructor.Remove(instructor);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool InstructorExists(int id)
        {
            return _context.Instructor.Any(e => e.InstructorId == id);
        }
    }
}