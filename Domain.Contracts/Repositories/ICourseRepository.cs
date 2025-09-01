using Domain.Models.Entities;
using LMS.Shared.Common;
using LMS.Shared.Enums;

namespace Domain.Contracts.Repositories;

public interface ICourseRepository : IRepositoryBase<Course>, IInternalRepositoryBase<Course>
{
    Task<Course?> GetCourseByIdAsync(int id, bool trackChanges = false);
    Task<Course?> GetCourseByIdByUserRoleAsync(int courseId, UserRole userRole, bool trackChanges = false);
    Task<PagedList<Course>> GetAllCoursesAsync(
        UserRole? includeUsers = null,
        bool includeModules = false,
        bool includeActivities = false,
        RequestParams requestParams = null!,
        bool trackChanges = false);
    Task<Course?> GetByStudentIdAsync(string studentUserId);
}
