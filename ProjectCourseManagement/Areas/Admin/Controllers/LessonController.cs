using Microsoft.AspNetCore.Mvc;
using ProjectCourseManagement.Models;

namespace ProjectCourseManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LessonController : Controller
    {
        private readonly DataContext _context;

        public LessonController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Details(int id)
        {
            var course = _context.Course
                //.Include(c => c.Instructor)
                .FirstOrDefault(c => c.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }

            var lessons = _context.Lesson
                .Where(l => l.CourseID == id)
                .ToList();

            ViewBag.Course = course;
            ViewBag.Lessons = lessons;
            return View();
        }

        public IActionResult Create(int courseId)
        {
            var course = _context.Course.Find(courseId);
            if (course == null)
            {
                return NotFound();
            }

            ViewBag.CourseName = course.CourseName;
            return View(new Lesson { CourseID = courseId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(lesson.LessonName))
                {
                    ModelState.AddModelError("LessonName", "Tên bài học không được để trống.");
                    var course = _context.Course.Find(lesson.CourseID);
                    ViewBag.CourseName = course?.CourseName;
                    return View(lesson);
                }

                _context.Lesson.Add(lesson);
                _context.SaveChanges();
                return RedirectToAction("Details", new { id = lesson.CourseID });
            }

            var courseForError = _context.Course.Find(lesson.CourseID);
            ViewBag.CourseName = courseForError?.CourseName;
            return View(lesson);
        }

        public IActionResult Edit(int id)
        {
            var lesson = _context.Lesson
                //.Include(l => l.Course)
                .FirstOrDefault(l => l.LessonID == id);
            if (lesson == null)
            {
                return NotFound();
            }

            ViewBag.CourseName = lesson.Course?.CourseName;
            return View(lesson);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(lesson.LessonName))
                {
                    ModelState.AddModelError("LessonName", "Tên bài học không được để trống.");
                    var course = _context.Course.Find(lesson.CourseID);
                    ViewBag.CourseName = course?.CourseName;
                    return View(lesson);
                }

                _context.Lesson.Update(lesson);
                _context.SaveChanges();
                return RedirectToAction("Details", new { id = lesson.CourseID });
            }

            var courseForError = _context.Course.Find(lesson.CourseID);
            ViewBag.CourseName = courseForError?.CourseName;
            return View(lesson);
        }

        public IActionResult Delete(int id)
        {
            var lesson = _context.Lesson
                //.Include(l => l.Course)
                .FirstOrDefault(l => l.LessonID == id);
            if (lesson == null)
            {
                return NotFound();
            }

            ViewBag.CourseName = lesson.Course?.CourseName;
            return View(lesson);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var lesson = _context.Lesson.Find(id);
            if (lesson != null)
            {
                var courseId = lesson.CourseID;
                _context.Lesson.Remove(lesson);
                _context.SaveChanges();
                return RedirectToAction("Details", new { id = courseId });
            }
            return NotFound();
        }
    }
}
