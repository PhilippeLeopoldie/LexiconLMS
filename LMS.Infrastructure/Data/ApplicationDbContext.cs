using Domain.Models.Entities;
using LMS.Infrastructure.Data.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace LMS.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivityType> ActivityTypes { get; set; }
        public DbSet<Module> Modules { get; set; } = default!;
        public DbSet<Document> Documents { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new ApplicationUserConfigurations());

            builder.Entity<ActivityType>()
                .HasMany(at => at.Activities)
                .WithOne(a => a.Type)
                .HasForeignKey(a => a.ActivityTypeId);

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Course)
                .WithMany(c => c.Students)
                .HasForeignKey(u => u.CourseId);

            /*builder.Entity<ApplicationUser>()
                .HasMany(u => u.Courses)
                .WithMany(c => c.Teachers);*/

            builder.Entity<Course>()
            .HasOne(c => c.Teacher)
            .WithMany(u => u.Courses)
            .HasForeignKey(c => c.TeacherId);

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
}
