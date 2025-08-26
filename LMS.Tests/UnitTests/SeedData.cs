using Domain.Models.Entities;
using LMS.Shared.DTOs.ModuleDtos;


namespace LMS.Tests.UnitTests;

public class SeedData
{
    public static Course GetCourse()
    {
        // Activity Types
        var lectureType = new ActivityType
        {
            Id = 1,
            Name = "Lecture",
            Description = "Instructor-led classroom lecture"
        };
        var labType = new ActivityType
        {
            Id = 2,
            Name = "Lab",
            Description = "Hands-on coding lab"
        };
        var assignmentType = new ActivityType
        {
            Id = 3,
            Name = "Assignment",
            Description = "Homework or coding project"
        };

        // Users (teachers and students)
        var teacher1 = new ApplicationUser
        {
            Id = "t1",
            UserName = "alice.dev",
            Email = "alice@school.com",
            RefreshToken = "token1",
            RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7)
        };

        var teacher2 = new ApplicationUser
        {
            Id = "t2",
            UserName = "bob.dev",
            Email = "bob@school.com",
            RefreshToken = "token2",
            RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7)
        };

        var student1 = new ApplicationUser
        {
            Id = "s1",
            UserName = "charlie.s",
            Email = "charlie@student.com",
            RefreshToken = "token3",
            RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7)
        };

        var student2 = new ApplicationUser
        {
            Id = "s2",
            UserName = "diana.s",
            Email = "diana@student.com",
            RefreshToken = "token4",
            RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7)
        };

        var student3 = new ApplicationUser
        {
            Id = "s3",
            UserName = "eric.s",
            Email = "eric@student.com",
            RefreshToken = "token5",
            RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7)
        };

        // Course
        var course = new Course
        {
            Id = 1,
            Name = "Software Development",
            Description = "A complete program covering full-stack software engineering",
            Starts = DateTime.UtcNow,
            Ends = DateTime.UtcNow.AddMonths(6),
            Teachers = new List<ApplicationUser> { teacher1, teacher2 },
            Students = new List<ApplicationUser> { student1, student2, student3 }
        };

        // assign reverse relation
        foreach (var student in course.Students)
            student.Course = course;

        foreach (var teacher in course.Teachers)
            teacher.Courses.Add(course);

        // Modules
        var module1 = new Module
        {
            Id = 1,
            Name = "C# Fundamentals",
            Description = "Learn object-oriented programming with C#",
            StartsAt = DateTime.UtcNow,
            EndsAt = DateTime.UtcNow.AddDays(14),
            Course = course,
            CourseId = course.Id
        };

        var module2 = new Module
        {
            Id = 2,
            Name = "Web Development with React",
            Description = "Frontend development using React and TypeScript",
            StartsAt = DateTime.UtcNow.AddDays(15),
            EndsAt = DateTime.UtcNow.AddDays(30),
            Course = course,
            CourseId = course.Id
        };

        course.Modules = new List<Module> { module1, module2 };

        // Activities for Module 1 (C# Fundamentals)
        var activity1 = new Activity
        {
            Id = 1,
            Name = "Intro to C#",
            Description = "Lecture: Basics of syntax and types",
            StartsAt = DateTime.UtcNow,
            EndsAt = DateTime.UtcNow.AddHours(2),
            Type = lectureType,
            ActivityTypeId = lectureType.Id,
            Module = module1,
            ModuleId = module1.Id
        };

        var activity2 = new Activity
        {
            Id = 2,
            Name = "C# Lab 1",
            Description = "Hands-on: Writing your first C# program",
            StartsAt = DateTime.UtcNow.AddDays(1),
            EndsAt = DateTime.UtcNow.AddDays(1).AddHours(3),
            Type = labType,
            ActivityTypeId = labType.Id,
            Module = module1,
            ModuleId = module1.Id
        };

        module1.Activities = new List<Activity> { activity1, activity2 };

        // Activities for Module 2 (React)
        var activity3 = new Activity
        {
            Id = 3,
            Name = "Intro to React",
            Description = "Lecture: Components, props, and state",
            StartsAt = DateTime.UtcNow.AddDays(15),
            EndsAt = DateTime.UtcNow.AddDays(15).AddHours(2),
            Type = lectureType,
            ActivityTypeId = lectureType.Id,
            Module = module2,
            ModuleId = module2.Id
        };

        var activity4 = new Activity
        {
            Id = 4,
            Name = "React Lab",
            Description = "Hands-on: Build a todo app in React",
            StartsAt = DateTime.UtcNow.AddDays(16),
            EndsAt = DateTime.UtcNow.AddDays(16).AddHours(3),
            Type = labType,
            ActivityTypeId = labType.Id,
            Module = module2,
            ModuleId = module2.Id
        };

        module2.Activities = new List<Activity> { activity3, activity4 };

        // Documents
        var doc1 = new Document
        {
            Id = 1,
            Name = "C# Intro Slides",
            Description = "Lecture slides for Intro to C#",
            CreatedAt = DateTime.UtcNow,
            UploadedAt = DateTime.UtcNow,
            StoragePath = "/storage/csharp-intro.pdf",
            Size = 10240,
            FileType = "pdf",
            UploadedByUserId = teacher1.Id,
            User = teacher1,
            Course = course,
            CourseId = course.Id,
            Module = module1,
            ModuleId = module1.Id,
            Activity = activity1,
            ActivityId = activity1.Id
        };

        activity1.Documents = new List<Document> { doc1 };
        module1.Documents = new List<Document> { doc1 };
        course.Documents = new List<Document> { doc1 };
        teacher1.Documents = new List<Document> { doc1 };

        return course;
    }

    public static List<ModuleDto> GetModuleDtos()
    {
        var course = GetCourse();

        return new List<ModuleDto>
            {
                  new ModuleDto
        {
            Id = 1,
            Name = "C# Fundamentals",
            Description = "Learn object-oriented programming with C#",
            StartsAt = DateTime.UtcNow,
            EndsAt = DateTime.UtcNow.AddDays(14),
            CourseId = course.Id
        },

        new ModuleDto
        {
            Id = 2,
            Name = "Web Development with React",
            Description = "Frontend development using React and TypeScript",
            StartsAt = DateTime.UtcNow.AddDays(15),
            EndsAt = DateTime.UtcNow.AddDays(30),
            CourseId = course.Id
        }
            };
    }

    public static ModuleCreateDto GetModuleCreateDto()
    {
        var course = GetCourse();
        return new ModuleCreateDto
        {
            Name = "New Module",
            Description = "New Module description",
            StartsAt = DateTime.UtcNow.AddDays(31),
            EndsAt = DateTime.UtcNow.AddDays(46),
        };
    }

    


}
