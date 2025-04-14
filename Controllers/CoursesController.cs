using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCWebApp.Data;
using MVCWebApp.Models;

namespace MVCWebApp.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        public readonly AppDbContext context;
        public CoursesController(AppDbContext _context)
        {
            context = _context;
        }
        public IActionResult Index(string searchString, string sortBy)
        {
            var courses = context.Courses.ToList();
            if (!string.IsNullOrEmpty(searchString) && sortBy == "CourseName")
            {
                courses = courses.Where(c => c.CourseName.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            else if (!string.IsNullOrEmpty(searchString) && sortBy == "Lecturer")
            {
                courses = courses.Where(c => c.Lecturer.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            foreach (var course in courses)
            {
                course.StudentLimit = course.StudentLimit - context.Enrollments.Count(e => e.CourseId == course.CourseId);
                if (course.StartDate < DateOnly.FromDateTime(DateTime.Now))
                {
                    courses.Remove(course);
                }
            }
            return View(courses);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CourseDto courseDto)
        {
            if (ModelState.IsValid)
            {
                var course = new Course
                {
                    CourseId = Guid.NewGuid(),
                    CourseName = courseDto.CourseName,
                    Lecturer = courseDto.Lecturer,
                    StartDate = courseDto.StartDate,
                    TuitionFee = courseDto.TuitionFee,
                    StudentLimit = courseDto.StudentLimit
                };
                context.Courses.Add(course);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(courseDto);
        }

        public IActionResult Edit(Guid id)
        {
            var course = context.Courses.Find(id);
            if (course == null)
            {
                return NotFound();
            }
            var courseDto = new CourseDto
            {
                CourseName = course.CourseName,
                Lecturer = course.Lecturer,
                StartDate = course.StartDate,
                TuitionFee = course.TuitionFee,
                StudentLimit = course.StudentLimit
            };

            ViewData["CourseId"] = course.CourseId;

            return View(courseDto);
        }

        [HttpPost]
        public IActionResult Edit(Guid id, CourseDto courseDto)
        {
            if (ModelState.IsValid)
            {
                var course = context.Courses.Find(id);
                if (course == null)
                {
                    return NotFound();
                }
                course.CourseName = courseDto.CourseName;
                course.Lecturer = courseDto.Lecturer;
                course.StartDate = courseDto.StartDate;
                course.TuitionFee = courseDto.TuitionFee;
                course.StudentLimit = courseDto.StudentLimit;
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(courseDto);
        }

        public IActionResult Delete(Guid id)
        {
            var course = context.Courses.Find(id);
            if (course == null)
            {
                return NotFound();
            }
            context.Courses.Remove(course);
            context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
