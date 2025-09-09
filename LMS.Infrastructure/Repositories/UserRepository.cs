using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infrastructure.Data;
using LMS.Shared.Common;
using LMS.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context) : RepositoryBase<ApplicationUser>(context), IUserRepository
{
    public async Task<PagedList<ApplicationUser>> GetAllUsersAsync(RequestParams requestParams, bool includeDocuments = false, bool trackChanges = false)
    {
        var query = FindByCondition(u => u.UserName != null
                                    && (requestParams.SearchTerm == null
                                        || u.UserName.ToLower().Contains(requestParams.SearchTerm.ToLower())),
                                    trackChanges);

        query = ApplyOrdering(query, requestParams);
        query = includeDocuments ? query.Include(u => u.Documents) : query;
        return await PagedList<ApplicationUser>.CreateAsync(query, requestParams.Page, requestParams.PageSize);
    }

    public async Task<PagedList<ApplicationUser>> GetStudentsByCourseAsync(RequestParams requestParams, int courseId, bool includeDocuments = false, bool trackChanges = false)
    {
        var query = FindByCondition(u => u.UserName != null
                                    && (requestParams.SearchTerm == null
                                        || u.UserName.ToLower().Contains(requestParams.SearchTerm.ToLower()))
                                    && u.CourseId == courseId,
                                    trackChanges);

        query = ApplyOrdering(query, requestParams);
        query = includeDocuments ? query.Include(u => u.Documents) : query;
        return await PagedList<ApplicationUser>.CreateAsync(query, requestParams.Page, requestParams.PageSize);
    }

    public async Task<PagedList<ApplicationUser>> GetAllTeachersAsync(RequestParams requestParams, int? courseId, bool includeDocuments = false, bool trackChanges = false)
    {
        var query = FindByCondition(u => u.UserName != null
                                    && (requestParams.SearchTerm == null
                                        || u.UserName.ToLower().Contains(requestParams.SearchTerm.ToLower()))
                                    && (courseId == null || u.Courses.Any(c => c.Id == courseId)),
                                    trackChanges);

        query = ApplyOrdering(query, requestParams);
        query = includeDocuments ? query.Include(u => u.Documents) : query;
        return await PagedList<ApplicationUser>.CreateAsync(query, requestParams.Page, requestParams.PageSize);
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(string id, bool includeDocuments = false, bool trackChanges = false) =>
                await FindByCondition(u => u.Id == id, trackChanges)
                     .FirstOrDefaultAsync();

    private static IQueryable<ApplicationUser> ApplyOrdering(IQueryable<ApplicationUser> users, RequestParams requestParams)
    {
        if (requestParams.OrderBy == null) return users.OrderBy(u => u.UserName);

        return requestParams.OrderBy switch
        {
            OrderByParams.NameAsc => users.OrderBy(u => u.UserName),
            OrderByParams.NameDesc => users.OrderByDescending(u => u.UserName),
            _ => users
        };
    }
}
