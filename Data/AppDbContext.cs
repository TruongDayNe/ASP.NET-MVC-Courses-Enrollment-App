using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MVCWebApp.Models;
using Microsoft.AspNetCore.Identity;

namespace MVCWebApp.Data
{
    public class AppDbContext: IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var moderator = new IdentityRole
            {
                Id = "1",
                Name = "Moderator",
                NormalizedName = "Moderator"
            };

            var student = new IdentityRole
            {
                Id = "2",
                Name = "Student",
                NormalizedName = "Student"
            };
            modelBuilder.Entity<IdentityRole>().HasData(moderator, student);
        }
    }
}
