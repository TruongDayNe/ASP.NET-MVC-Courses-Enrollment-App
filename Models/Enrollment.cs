using MVCWebApp.Models;

namespace MVCWebApp.Models
{
    public class Enrollment
    {
        public Guid EnrollmentId { get; set; }
        public Guid CourseId { get; set; }
        public Guid StudentId { get; set; }
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;
    }
}
