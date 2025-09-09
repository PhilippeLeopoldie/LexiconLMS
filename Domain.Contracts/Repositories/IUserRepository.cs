using Domain.Models.Entities;
using LMS.Shared.Common;

namespace Domain.Contracts.Repositories;

public interface IUserRepository : IRepositoryBase<ApplicationUser>, IInternalRepositoryBase<ApplicationUser>
{
    Task<PagedList<ApplicationUser>> GetAllUsersAsync(RequestParams requestParams, bool includeDocuments = false, bool trackChanges = false);
    Task<PagedList<ApplicationUser>> GetStudentsByCourseAsync(RequestParams requestParams, int courseId, bool includeDocuments = false, bool trackChanges = false);
    Task<PagedList<ApplicationUser>> GetAllTeachersAsync(RequestParams requestParams, int? courseId, bool includeDocuments = false, bool trackChanges = false);
    Task<ApplicationUser?> GetUserByIdAsync(string id, bool includeDocuments = false, bool trackChanges = false);
}
