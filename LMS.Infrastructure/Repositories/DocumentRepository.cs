using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;
public class DocumentRepository(ApplicationDbContext context) : RepositoryBase<Document>(context), IDocumentRepository
{
    public async Task<IEnumerable<Document>> GetAllAsync(bool trackChanges = false)
    {
        return await FindAll(trackChanges)
                    .Include(d => d.User)
                    .Include(d => d.Course)
                    .Include(d => d.Module)
                    .Include(d => d.Activity)
                    .ToListAsync();
    }

    public async Task<Document?> GetByIdAsync(int id, bool trackChanges = false)
    {
        return await FindByCondition(d => d.Id == id, trackChanges)
                    .Include(d => d.User)
                    .Include(d => d.Course)
                    .Include(d => d.Module)
                    .Include(d => d.Activity)
                    .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Document>> GetDocumentsByCourseAsync(int courseId, bool trackChanges = false)
    {
        return await FindByCondition(d => d.CourseId == courseId, trackChanges)
                    .Include(d => d.User)
                    .Include(d => d.Course)
                    .ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetDocumentsByModuleAsync(int moduleId, bool trackChanges = false)
    {
        return await FindByCondition(d => d.ModuleId == moduleId, trackChanges)
                    .Include(d => d.User)
                    .Include(d => d.Module)
                    .ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetDocumentsByActivityAsync(int activityId, bool trackChanges = false)
    {
        return await FindByCondition(d => d.ActivityId == activityId, trackChanges)
                    .Include(d => d.User)
                    .Include(d => d.Activity)
                    .ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetDocumentsByUserAsync(string userId, bool trackChanges = false)
    {
        return await FindByCondition(d => d.UploadedByUserId == userId, trackChanges)
                    .Include(d => d.Course)
                    .Include(d => d.Module)
                    .Include(d => d.Activity)
                    .ToListAsync();
    }

}
