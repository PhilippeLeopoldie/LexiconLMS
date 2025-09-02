using Microsoft.AspNetCore.Identity;

namespace Domain.Models.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName => $"{FirstName} {LastName}".Trim();
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpireTime { get; set; }

    // Student side: each student has one course
    public int? CourseId { get; set; }
    public Course? Course { get; set; }

    //  Teacher side: each teacher can teach many courses
    public ICollection<Course> Courses { get; set; } = [];

    public ICollection<Document> Documents { get; set; } = [];
}
