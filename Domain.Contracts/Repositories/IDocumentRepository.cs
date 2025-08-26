
using Domain.Models.Entities;
using LMS.Shared.Common;

namespace Domain.Contracts.Repositories;
public interface IDocumentRepository : IRepositoryBase<Document>, IInternalRepositoryBase<Document>
{
    Task<PagedList<Document>> GetAllAsync(RequestParams requestParams, bool trackChanges = false);
    Task<Document?> GetByIdAsync(int id, bool trackChanges = false);
    Task<Document?> GetDeletedByIdAsync(int id, bool trackChanges = false);

    Task<PagedList<Document>> GetDocumentsByCourseAsync(RequestParams requestParams, int courseId, bool trackChanges = false);
    Task<PagedList<Document>> GetDocumentsByModuleAsync(RequestParams requestParams, int moduleId, bool trackChanges = false);
    Task<PagedList<Document>> GetDocumentsByActivityAsync(RequestParams requestParams, int activityId, bool trackChanges = false);
    Task<PagedList<Document>> GetDocumentsByUserAsync(RequestParams requestParams, string userId, bool trackChanges = false);
    Task<Document?> GetDocumentForActivityAndUserAsync(int activityId, string studentUserId);
}
