using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infrastructure.Data;
using LMS.Shared.Common;
using LMS.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;
public class DocumentRepository(ApplicationDbContext context) : RepositoryBase<Document>(context), IDocumentRepository
{
    public async Task<PagedList<Document>> GetAllAsync(RequestParams requestParams, bool trackChanges = false)
    {
        var documents = FindByCondition(d => d.DeletedAt == null
                                     && (requestParams.SearchTerm == null
                                     || d.Name.ToLower().Contains(requestParams.SearchTerm.ToLower())),
                                     trackChanges);
        documents = ApplyOrdering(documents, requestParams)
                    .Include(d => d.User)
                    .Include(d => d.Course)
                    .Include(d => d.Module)
                    .Include(d => d.Activity);

        return await PagedList<Document>.CreateAsync(documents, requestParams.Page, requestParams.PageSize);
    }

    public async Task<Document?> GetByIdAsync(int id, bool trackChanges = false)
    {
        return await FindByCondition(d => d.Id == id && d.DeletedAt == null, trackChanges)
                    .Include(d => d.User)
                    .Include(d => d.Course)
                    .Include(d => d.Module)
                    .Include(d => d.Activity)
                    .FirstOrDefaultAsync();
    }

    public async Task<Document?> GetDeletedByIdAsync(int id, bool trackChanges = false)
    {
        return await FindByCondition(d => d.Id == id && d.DeletedAt != null, trackChanges)
                    .Include(d => d.User)
                    .Include(d => d.Course)
                    .Include(d => d.Module)
                    .Include(d => d.Activity)
                    .FirstOrDefaultAsync();
    }

    public async Task<PagedList<Document>> GetDocumentsByCourseAsync(RequestParams requestParams, int courseId, bool trackChanges = false)
    {
        var documents = FindByCondition(d => d.CourseId == courseId
                                     && d.DeletedAt == null
                                     && (requestParams.SearchTerm == null
                                     || d.Name.ToLower().Contains(requestParams.SearchTerm.ToLower())),
                                     trackChanges);
        documents = ApplyOrdering(documents, requestParams)
                    .Include(d => d.User)
                    .Include(d => d.Course);

        return await PagedList<Document>.CreateAsync(documents, requestParams.Page, requestParams.PageSize);
    }

    public async Task<PagedList<Document>> GetDocumentsByModuleAsync(RequestParams requestParams, int moduleId, bool trackChanges = false)
    {
        var documents = FindByCondition(d => d.ModuleId == moduleId
                                     && d.DeletedAt == null
                                     && (requestParams.SearchTerm == null
                                     || d.Name.ToLower().Contains(requestParams.SearchTerm.ToLower())),
                                     trackChanges);
        documents = ApplyOrdering(documents, requestParams)
                    .Include(d => d.User)
                    .Include(d => d.Module);

        return await PagedList<Document>.CreateAsync(documents, requestParams.Page, requestParams.PageSize);
    }

    public async Task<PagedList<Document>> GetDocumentsByActivityAsync(RequestParams requestParams, int activityId, bool trackChanges = false)
    {
        var documents = FindByCondition(d => d.ActivityId == activityId
                                     && d.DeletedAt == null
                                     && (requestParams.SearchTerm == null
                                     || d.Name.ToLower().Contains(requestParams.SearchTerm.ToLower())),
                                     trackChanges);
        documents = ApplyOrdering(documents, requestParams)
                    .Include(d => d.User)
                    .Include(d => d.Activity);

        return await PagedList<Document>.CreateAsync(documents, requestParams.Page, requestParams.PageSize);
    }

    public async Task<PagedList<Document>> GetDocumentsByUserAsync(RequestParams requestParams, string userId, bool trackChanges = false)
    {
        var documents = FindByCondition(d => d.UploadedByUserId == userId
                                     && d.DeletedAt == null
                                     && (requestParams.SearchTerm == null
                                     || d.Name.ToLower().Contains(requestParams.SearchTerm.ToLower())),
                                     trackChanges);
        documents = ApplyOrdering(documents, requestParams)
                    .Include(d => d.Course)
                    .Include(d => d.Module)
                    .Include(d => d.Activity);
        return await PagedList<Document>.CreateAsync(documents, requestParams.Page, requestParams.PageSize);
    }

    public async Task<Document?> GetDocumentForActivityAndUserAsync(int activityId, string studentUserId) =>
        await FindByCondition(d => d.ActivityId == activityId
                                && d.UploadedByUserId.Equals(studentUserId)
                                && d.DeletedAt == null)
            .OrderByDescending(d => d.UploadedAt)
            .FirstOrDefaultAsync();

    private static IQueryable<Document> ApplyOrdering(IQueryable<Document> documents, RequestParams requestParams)
    {
        if (requestParams.OrderBy == null) return documents.OrderBy(d => d.CreatedAt).ThenBy(d => d.Name);

        return requestParams.OrderBy switch
        {
            OrderByParams.NameAsc => documents.OrderBy(t => t.Name),
            OrderByParams.NameDesc => documents.OrderByDescending(t => t.Name),
            OrderByParams.DateAsc => documents.OrderBy(t => t.CreatedAt).ThenBy(d => d.Name),
            OrderByParams.DateDesc => documents.OrderByDescending(t => t.CreatedAt).ThenBy(d => d.Name),
            _ => documents
        };
    }
}
