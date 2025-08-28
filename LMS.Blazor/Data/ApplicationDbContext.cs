
using Domain.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LMS.Blazor.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<ApplicationUser>().ToTable("ApplicationUser");
        builder.Entity<ActivityType>()
                .HasMany(at => at.Activities)
                .WithOne(a => a.Type)
                .HasForeignKey(a => a.ActivityTypeId);

        builder.Entity<ApplicationUser>()
            .HasOne(u => u.Course)
            .WithMany(c => c.Students)
            .HasForeignKey(u => u.CourseId)
            .IsRequired(false);

        builder.Entity<ApplicationUser>()
            .HasMany(u => u.Courses)
            .WithMany(c => c.Teachers);

        /*builder.Entity<Course>()
            .HasOne(c => c.Teacher)
            .WithMany(u => u.Courses)
            .HasForeignKey(c => c.TeacherId);*/

        builder.Entity<Document>()
            .HasOne(d => d.Course)
            .WithMany(c => c.Documents)
            .HasForeignKey(d => d.CourseId);

        builder.Entity<Document>()
            .HasOne(d => d.Module)
            .WithMany(m => m.Documents)
            .HasForeignKey(d => d.ModuleId);

        builder.Entity<Document>()
            .HasOne(d => d.Activity)
            .WithMany(a => a.Documents)
            .HasForeignKey(d => d.ActivityId);

        builder.Entity<Document>()
            .HasOne(d => d.User)
            .WithMany(u => u.Documents)
            .HasForeignKey(d => d.UploadedByUserId);
    }
}
