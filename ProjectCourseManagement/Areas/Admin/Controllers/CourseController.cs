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

        public CourseController(DataContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public IActionResult Index()
        {
            // Lấy khóa học theo CourseID từ cache
            if (!_cache.TryGetValue(CacheKey, out Dictionary<int, Course> courseDictionary))
            {
                courseDictionary = _context.Course
                    .Take(20)
                    .ToDictionary(m => m.CourseID);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                _cache.Set(CacheKey, courseDictionary, cacheEntryOptions);
            }

            // Lấy khóa học theo CourseName từ cache
            if (!_cache.TryGetValue(CourseNameCacheKey, out Dictionary<string, Course> courseNameDictionary))
            {
                courseNameDictionary = _context.Course
                .AsEnumerable() // tránh lỗi từ EF Core khi dùng GroupBy với StringComparer
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
                    return View(course);
                }

                _context.Course.Add(course);
                _context.SaveChanges();
                UpdateCache();

                return RedirectToAction("Index");
            }
            return View(course);
        }

        public IActionResult Edit(int id)
        {
            var course = _context.Course.Find(id);
            if (course == null)
            {
                return NotFound();
            }
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
                    return View(course);
                }

                _context.Course.Update(course);
                _context.SaveChanges();
                UpdateCache();

                return RedirectToAction("Index");
            }
            return View(course);
        }

        public IActionResult Delete(int id)
        {
            var course = _context.Course.Find(id);
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
                UpdateCache();
            }
            return RedirectToAction("Index");
        }

        private void UpdateCache()
        {
            var courseDictionary = _context.Course
                .OrderBy(m => m.CourseName)
                .ToDictionary(m => m.CourseID);

            var courseNameDictionary = _context.Course
                .ToDictionary(c => c.CourseName, c => c, StringComparer.OrdinalIgnoreCase);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10));

            _cache.Set(CacheKey, courseDictionary, cacheEntryOptions);
            _cache.Set(CourseNameCacheKey, courseNameDictionary, cacheEntryOptions);
        }

        public IActionResult Search(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return RedirectToAction("Index");
            }
            // 1. Truy vấn bằng danh sách (List)
            var courseList = _context.Course.ToList();
            var stopwatchList = Stopwatch.StartNew();
            var listResult = courseList
                .Where(c => c.CourseName.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
            stopwatchList.Stop();
            var listTime = stopwatchList.ElapsedMilliseconds;
            // 2. Truy vấn bằng Dictionary (HashTable)
            if (!_cache.TryGetValue(CourseNameCacheKey, out Dictionary<string, Course> courseNameDict))
            {
                courseNameDict = _context.Course
                    .ToDictionary(c => c.CourseName, c => c, StringComparer.OrdinalIgnoreCase);

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

            return View("SearchResult", result);
        }
    }
}
