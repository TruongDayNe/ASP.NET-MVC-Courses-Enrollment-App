using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MVCWebApp.Data;
using MVCWebApp.Models;
using Microsoft.Identity.Client;
using Microsoft.Build.Framework;
using Microsoft.AspNetCore.Identity;

namespace MVCWebApp.Controllers
{
    public class EnrollmentsController : Controller
    {
        public readonly AppDbContext context;
        public readonly UserManager<AppUser> userManager;
        public EnrollmentsController(AppDbContext _context, UserManager<AppUser> _userManager)
        {
            context = _context;
            userManager = _userManager;
        }
        [Authorize(Roles ="Student")]
        public IActionResult Index()
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(studentId))
            {
                return Unauthorized();
            }
            // Get enrollments for the current student and join with courses
            var enrollmentsQuery = from e in context.Enrollments
                                   where e.StudentId == Guid.Parse(studentId)
                                   join c in context.Courses on e.CourseId equals c.CourseId
                                   select new { Enrollment = e, Course = c };

            var enrollmentsList = enrollmentsQuery.ToList();

            if (enrollmentsList == null || !enrollmentsList.Any())
            {
                TempData["NotificationMessage"] = "You are not enrolled in any courses.";
                TempData["NotificationType"] = "info";
                return View(new List<Enrollment>());
            }

            // Create a dictionary of Course objects keyed by CourseId
            var courses = enrollmentsList.ToDictionary(
                e => e.Enrollment.CourseId,
                e => e.Course
            );

            // Store Course dictionary in ViewBag
            ViewBag.Courses = courses;

            // Pass the list of Enrollment objects as the model
            var enrollments = enrollmentsList.Select(e => e.Enrollment).ToList();

            return View(enrollments);
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        public IActionResult Enroll(Guid courseId)
        {
            var course = context.Courses.Find(courseId);
            if (course == null)
            {
                return NotFound();
            }

            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine($"Student ID: {studentId}");

            if (string.IsNullOrEmpty(studentId))
            {
                return Unauthorized();
            }

            if (!Guid.TryParse(studentId, out var studentGuid))
            {
                return BadRequest("Invalid student ID format.");
            }

            var enrollment = new Enrollment
            {
                CourseId = course.CourseId,
                StudentId = studentGuid,
            };

            //course reached student limit
            if (course.StudentLimit > 0 && context.Enrollments.Count(e => e.CourseId == courseId) >= course.StudentLimit)
            {
                TempData["NotificationMessage"] = "This course has reached its student limit.";
                TempData["NotificationType"] = "error";
                return RedirectToAction("Index", "Courses");
            }

            //already enrolled
            var existingEnrollment = context.Enrollments
                .FirstOrDefault(e => e.CourseId == courseId && e.StudentId == studentGuid);
            if (existingEnrollment != null)
            {
                TempData["NotificationMessage"] = "You are already enrolled in this course.";
                TempData["NotificationType"] = "error";
                return RedirectToAction("Index", "Courses");
            }

            context.Enrollments.Add(enrollment);
            context.SaveChanges();

            TempData["NotificationMessage"] = "You have successfully enrolled in the course!";
            TempData["NotificationType"] = "success";

            return RedirectToAction("Index", "Courses");
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        public IActionResult Unenroll(Guid enrollmentId)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(studentId))
            {
                return Unauthorized();
            }

            if (!Guid.TryParse(studentId, out var studentGuid))
            {
                return BadRequest("Invalid student ID format.");
            }

            var enrollment = context.Enrollments
                .FirstOrDefault(e => e.EnrollmentId == enrollmentId && e.StudentId == studentGuid);
            if (enrollment == null)
            {
                TempData["NotificationMessage"] = "You are not enrolled in this course.";
                TempData["NotificationType"] = "error";
                return RedirectToAction("Index");
            }

            context.Enrollments.Remove(enrollment);
            context.SaveChanges();

            TempData["NotificationMessage"] = "You have successfully unenrolled from the course!";
            TempData["NotificationType"] = "success";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> Edit(Guid courseId)
        {
            var enrollments = context.Enrollments
                .Where(e => e.CourseId == courseId)
                .ToList();

            if (enrollments == null || !enrollments.Any())
            {
                TempData["NotificationMessage"] = "No students are enrolled in this course.";
                TempData["NotificationType"] = "info";
                return View(new List<Enrollment>());
            }

            var course = context.Courses.Find(courseId);
            if (course == null)
            {
                return NotFound();
            }

            var userNames = new Dictionary<Guid, string>();
            foreach (var enrollment in enrollments)
            {
                var user = await userManager.FindByIdAsync(enrollment.StudentId.ToString());
                if (user != null)
                {
                    userNames[enrollment.StudentId] = $"{user.FirstName} {user.LastName}";
                }
                else
                {
                    userNames[enrollment.StudentId] = "Unknown";
                }
            }

            ViewBag.UserNames = userNames;
            ViewBag.CourseName = course.CourseName;

            return View(enrollments); // Render the Edit.cshtml view
        }

        [HttpPost]
        [Authorize(Roles = "Moderator")]
        public IActionResult Delete(Guid id)
        {
            var enrollment = context.Enrollments.Find(id);
            if (enrollment == null)
            {
                TempData["NotificationMessage"] = "Enrollment not found.";
                TempData["NotificationType"] = "error";
                return RedirectToAction("Index", "Courses");
            }

            var courseId = enrollment.CourseId;
            context.Enrollments.Remove(enrollment);
            context.SaveChanges();

            TempData["NotificationMessage"] = "Student has been removed from the course.";
            TempData["NotificationType"] = "success";
            return RedirectToAction("Edit", new { courseId });
        }

        [HttpGet]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> Statistics(int? year = null)
        {
            // Use the current year if no year is specified
            int selectedYear = year ?? DateTime.Now.Year;

            // Total Students (users in the "Student" role)
            var students = await userManager.GetUsersInRoleAsync("Student");
            ViewBag.TotalStudents = students.Count;

            // Total Courses
            var courses = context.Courses
                .Where(c => c.StartDate.Year == selectedYear)
                .ToList();
            ViewBag.TotalCourses = context.Courses.Count(); // Still show total courses across all years
            ViewBag.SelectedYear = selectedYear;

            // Total Enrollments
            ViewBag.TotalEnrollments = context.Enrollments.Count();

            // Enrollments Per Course
            var enrollmentsPerCourseQuery = from e in context.Enrollments
                                            join c in context.Courses on e.CourseId equals c.CourseId
                                            group e by new { c.CourseId, c.CourseName } into g
                                            select new { CourseName = g.Key.CourseName, Count = g.Count() };
            ViewBag.EnrollmentsPerCourse = enrollmentsPerCourseQuery.AsEnumerable()
                .Select(x => new Dictionary<string, object>
                {
            { "CourseName", x.CourseName },
            { "Count", x.Count }
                })
                .ToList();

            // Recent Enrollments (last 10, ordered by EnrollmentDate)
            var recentEnrollments = context.Enrollments
                .OrderByDescending(e => e.EnrollmentDate)
                .Take(10)
                .ToList();
            ViewBag.RecentEnrollments = recentEnrollments;

            // Pass user names for recent enrollments to the view
            var userNames = new Dictionary<Guid, string>();
            foreach (var enrollment in recentEnrollments)
            {
                var user = await userManager.FindByIdAsync(enrollment.StudentId.ToString());
                userNames[enrollment.StudentId] = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown";
            }
            ViewBag.UserNames = userNames;

            // Pass course names for recent enrollments
            var courseNames = context.Courses.ToDictionary(c => c.CourseId, c => c.CourseName);
            ViewBag.Courses = courseNames;

            // Courses Started Per Month (for the selected year)
            int[] coursesStartedPerMonth = new int[12];
            foreach (var course in courses)
            {
                int monthIndex = course.StartDate.Month - 1;
                coursesStartedPerMonth[monthIndex]++;
            }
            ViewBag.CoursesStartedPerMonth = coursesStartedPerMonth;

            return View();
        }
    }
}
