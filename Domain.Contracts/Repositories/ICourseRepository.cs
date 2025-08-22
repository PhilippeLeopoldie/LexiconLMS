using Domain.Models.Entities;
using LMS.Shared.Common;

namespace Domain.Contracts.Repositories;

public interface ICourseRepository : IRepositoryBase<Course>, IInternalRepositoryBase<Course>
{
    Task<Course?> GetCourseByIdAsync(int id, bool trackChanges = false);
    Task<PagedList<Course>> GetAllCoursesAsync(RequestParams requestParams, bool trackChanges = false);
}
