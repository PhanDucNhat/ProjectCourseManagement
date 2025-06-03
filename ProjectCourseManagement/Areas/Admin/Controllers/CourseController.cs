using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProjectCourseManagement.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ProjectCourseManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CourseController : Controller
    {
        private readonly DataContext _context;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "CourseDictionary";
        private const string CourseNameCacheKey = "CourseNameDictionary";
        private const string InstructorCacheKey = "InstructorDictionary";

        public CourseController(DataContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public IActionResult Index()
        {
            if (!_cache.TryGetValue(CacheKey, out Dictionary<int, Course> courseDictionary))
            {
                courseDictionary = _context.Course
                    .Include(c => c.Instructor)
                    .Take(20)
                    .ToDictionary(m => m.CourseID);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                _cache.Set(CacheKey, courseDictionary, cacheEntryOptions);
            }

            if (!_cache.TryGetValue(CourseNameCacheKey, out Dictionary<string, Course> courseNameDictionary))
            {
                courseNameDictionary = _context.Course
                    .Include(c => c.Instructor)
                    .AsEnumerable()
                    .GroupBy(c => c.CourseName, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                _cache.Set(CourseNameCacheKey, courseNameDictionary, cacheEntryOptions);
            }

            ViewBag.CourseDictionary = courseDictionary;
            ViewBag.CourseNameDictionary = courseNameDictionary;

            return View();
        }

        public IActionResult Create()
        {
            ViewBag.Instructors = _context.Instructor
                .Select(i => new { i.InstructorId, i.InstructorName })
                .ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Course course)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(course.CourseName))
                {
                    ModelState.AddModelError("CourseName", "Tên khóa học không được để trống.");
                    ViewBag.Instructors = _context.Instructor
                        .Select(i => new { i.InstructorId, i.InstructorName })
                        .ToList();
                    return View(course);
                }

                _context.Course.Add(course);
                _context.SaveChanges();
                

                return RedirectToAction("Index");
            }

            ViewBag.Instructors = _context.Instructor
                .Select(i => new { i.InstructorId, i.InstructorName })
                .ToList();
            return View(course);
        }

        public IActionResult Edit(int id)
        {
            var course = _context.Course
                .Include(c => c.Instructor)
                .FirstOrDefault(c => c.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }
            ViewBag.Instructors = _context.Instructor
                .Select(i => new { i.InstructorId, i.InstructorName })
                .ToList();
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Course course)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(course.CourseName))
                {
                    ModelState.AddModelError("CourseName", "Tên khóa học không được để trống.");
                    ViewBag.Instructors = _context.Instructor
                        .Select(i => new { i.InstructorId, i.InstructorName })
                        .ToList();
                    return View(course);
                }

                _context.Course.Update(course);
                _context.SaveChanges();


                return RedirectToAction("Index");
            }
            ViewBag.Instructors = _context.Instructor
                .Select(i => new { i.InstructorId, i.InstructorName })
                .ToList();
            return View(course);
        }

        public IActionResult Delete(int id)
        {
            var course = _context.Course
                .Include(c => c.Instructor)
                .FirstOrDefault(c => c.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var course = _context.Course.Find(id);
            if (course != null)
            {
                _context.Course.Remove(course);
                _context.SaveChanges();

            }
            return RedirectToAction("Index");
        }

        private void UpdateCache()
        {
            var courseDictionary = _context.Course
                .Include(c => c.Instructor)
                .ToDictionary(m => m.CourseID);

            var courseNameDictionary = _context.Course
                .Include(c => c.Instructor)
                .AsEnumerable()
                .GroupBy(c => c.CourseName, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var instructorDictionary = _context.Course
                .Include(c => c.Instructor)
                .Where(c => c.Instructor != null)
                .AsEnumerable()
                .GroupBy(c => c.Instructor.InstructorName, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10));

            _cache.Set(CacheKey, courseDictionary, cacheEntryOptions);
            _cache.Set(CourseNameCacheKey, courseNameDictionary, cacheEntryOptions);
            _cache.Set(InstructorCacheKey, instructorDictionary, cacheEntryOptions);
        }

        public IActionResult Search(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return RedirectToAction("Index");
            }

            var courseList = _context.Course
                .Include(c => c.Instructor)
                .ToList();
            var stopwatchList = Stopwatch.StartNew();
            var listResult = courseList
                .Where(c => c.CourseName.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
            stopwatchList.Stop();
            var listTime = stopwatchList.ElapsedMilliseconds;

            if (!_cache.TryGetValue(CourseNameCacheKey, out Dictionary<string, Course> courseNameDict))
            {
                courseNameDict = _context.Course
                    .Include(c => c.Instructor)
                    .AsEnumerable()
                    .GroupBy(c => c.CourseName, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

                _cache.Set(CourseNameCacheKey, courseNameDict, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                });
            }
            var stopwatchDict = Stopwatch.StartNew();
            var dictResult = courseNameDict
                .Where(kv => kv.Key.Contains(search, StringComparison.OrdinalIgnoreCase))
                .Select(kv => kv.Value)
                .ToList();
            stopwatchDict.Stop();
            var dictTime = stopwatchDict.ElapsedMilliseconds;

            var result = dictResult.ToDictionary(c => c.CourseID);

            ViewBag.SearchTerm = search;
            ViewBag.ListTime = listTime;
            ViewBag.DictTime = dictTime;
            ViewBag.SearchType = "Tên khóa học";

            return View("SearchResult", result);
        }

        public IActionResult SearchByInstructor(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return RedirectToAction("Index");
            }

            var courseList = _context.Course
                .Include(c => c.Instructor)
                .ToList();
            var stopwatchList = Stopwatch.StartNew();
            var listResult = courseList
                .Where(c => c.Instructor != null &&
                            c.Instructor.InstructorName.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
            stopwatchList.Stop();
            var listTime = stopwatchList.ElapsedMilliseconds;

            if (!_cache.TryGetValue(InstructorCacheKey, out Dictionary<string, List<Course>> instructorDict))
            {
                instructorDict = _context.Course
                    .Include(c => c.Instructor)
                    .Where(c => c.Instructor != null)
                    .AsEnumerable()
                    .GroupBy(c => c.Instructor.InstructorName, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

                _cache.Set(InstructorCacheKey, instructorDict, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                });
            }

            var stopwatchDict = Stopwatch.StartNew();
            var dictResult = instructorDict
                .Where(kv => kv.Key.Contains(search, StringComparison.OrdinalIgnoreCase))
                .SelectMany(kv => kv.Value)
                .ToList();
            stopwatchDict.Stop();
            var dictTime = stopwatchDict.ElapsedMilliseconds;

            var result = dictResult
                .GroupBy(c => c.CourseID)
                .Select(g => g.First())
                .ToDictionary(c => c.CourseID);

            ViewBag.SearchTerm = search;
            ViewBag.ListTime = listTime;
            ViewBag.DictTime = dictTime;
            ViewBag.SearchType = "Tên giảng viên";

            return View("SearchResult", result);
        }
    }
}
