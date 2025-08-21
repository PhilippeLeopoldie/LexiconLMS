
using Domain.Models.Entities;

namespace Domain.Contracts.Repositories;
public interface IDocumentRepository : IRepositoryBase<Document>, IInternalRepositoryBase<Document>
{
    Task<IEnumerable<Document>> GetAllAsync(bool trackChanges = false);
    Task<Document?> GetByIdAsync(int id, bool trackChanges = false);
    Task<Document?> GetDeletedByIdAsync(int id, bool trackChanges = false);

    Task<IEnumerable<Document>> GetDocumentsByCourseAsync(int courseId, bool trackChanges = false);
    Task<IEnumerable<Document>> GetDocumentsByModuleAsync(int moduleId, bool trackChanges = false);
    Task<IEnumerable<Document>> GetDocumentsByActivityAsync(int activityId, bool trackChanges = false);
    Task<IEnumerable<Document>> GetDocumentsByUserAsync(string userId, bool trackChanges = false);
}
