using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infrastructure.Data;
using LMS.Shared.Common;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class CourseRepository(ApplicationDbContext context) : RepositoryBase<Course>(context), ICourseRepository
{
    public async Task<Course?> GetCourseByIdAsync(int id, bool trackChanges = false) =>
        await FindByCondition(course => course.Id.Equals(id), trackChanges).FirstOrDefaultAsync();

    public async Task<PagedList<Course>> GetAllCoursesAsync(RequestParams requestParams, bool trackChanges = false)
    {
        var query = FindAll(trackChanges);

        query = ApplyOrdering(query, requestParams);

        return await PagedList<Course>.CreateAsync(query, requestParams.Page, requestParams.PageSize);
    }
        
    private static IQueryable<Course> ApplyOrdering(IQueryable<Course> courses, RequestParams requestParams)
    {
        if (string.IsNullOrEmpty(requestParams.OrderBy)) return courses
            .OrderBy(c => c.Starts)
            .ThenBy(c => c.Name);

        return requestParams.OrderBy.ToLower() switch
        {
            "name" => courses.OrderBy(c => c.Name),
            "startdate" => courses.OrderBy(c => c.Starts).ThenBy(c => c.Name),
            _ => courses
        };
    }
}
