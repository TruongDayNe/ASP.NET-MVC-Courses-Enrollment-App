using Microsoft.AspNetCore.Identity;

namespace MVCWebApp.Models
{
    public class AppUser: IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public DateOnly CreateAt { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    }
}
