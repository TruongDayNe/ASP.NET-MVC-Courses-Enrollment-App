namespace MVCWebApp.Models
{
    public class Course
    {
        public Guid CourseId { get; set; } = Guid.NewGuid();
        public string CourseName { get; set; } = string.Empty;
        public string Lecturer { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public long TuitionFee { get; set; }
        public int StudentLimit { get; set; }
    }
}
