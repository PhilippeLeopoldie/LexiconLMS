using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infrastructure.Data;
using LMS.Shared.Common;
using LMS.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class CourseRepository(ApplicationDbContext context) : RepositoryBase<Course>(context), ICourseRepository
{
    public async Task<Course?> GetCourseByIdAsync(int id, bool trackChanges = false) =>
        await FindByCondition(course => course.Id.Equals(id), trackChanges)
        .FirstOrDefaultAsync();

    public  async Task<Course?> GetCourseByIdByUserRoleAsync(int courseId, UserRole userRole ,bool trackChanges = false)
    {
        var query = FindByCondition(course => course.Id.Equals(courseId), trackChanges);

        if (userRole.Equals(UserRole.Student))
            query = query.Include(c => c.Students);

        if (userRole.Equals(UserRole.Teacher))
            query = query.Include(c => c.Teachers);

        return await query.FirstOrDefaultAsync();
    }
        

    public async Task<PagedList<Course>> GetAllCoursesAsync(
        UserRole? includeUsers = null,
        bool includeModules = false,
        bool includeActivities = false,
        RequestParams requestParams = null!,
        bool trackChanges = false)
    {
        var query = FindAll(trackChanges);


        if (includeUsers == UserRole.Student)
            query = query.Include(course => course.Students);

        if (includeUsers == UserRole.Teacher)
            query = query.Include(course => course.Teachers);

        if (includeUsers == UserRole.All)
        {
            query = query.Include(course => course.Students);
            query = query.Include(course => course.Teachers);
        }


        if (includeModules)
        {
            query = query.Include(c => c.Modules);
            if (includeActivities)
                query = query.Include(c => c.Modules).ThenInclude(m => m.Activities);
        }
        query = ApplyOrdering(query, requestParams);

        return await PagedList<Course>.CreateAsync(query, requestParams.Page, requestParams.PageSize);
    }

    public async Task<Course?> GetByStudentIdAsync(string studentUserId) =>
        await FindByCondition(c => c.Students.Any(student => student.Id.Equals(studentUserId))).FirstOrDefaultAsync();

    private static IQueryable<Course> ApplyOrdering(IQueryable<Course> courses, RequestParams requestParams)
    {
        if (requestParams.OrderBy == null) return courses
            .OrderBy(c => c.Starts)
            .ThenBy(c => c.Name);

        return requestParams.OrderBy switch
        {
            OrderByParams.NameAsc => courses.OrderBy(c => c.Name),
            OrderByParams.NameDesc => courses.OrderByDescending(c => c.Name),
            OrderByParams.DateAsc => courses.OrderBy(c => c.Starts).ThenBy(c => c.Name),
            OrderByParams.DateDesc => courses.OrderByDescending(c => c.Starts).ThenBy(c => c.Name),
            _ => courses
        };
    }
}
