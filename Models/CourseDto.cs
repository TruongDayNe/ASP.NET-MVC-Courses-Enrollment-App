using System.ComponentModel.DataAnnotations;

namespace MVCWebApp.Models
{
    public class CourseDto
    {
        [Required]
        public string CourseName { get; set; } = string.Empty;
        [Required]
        public string Lecturer { get; set; } = string.Empty;
        [Required]
        public DateOnly StartDate { get; set; }
        [Required]
        public long TuitionFee { get; set; }
        [Required]
        public int StudentLimit { get; set; }
    }
}
