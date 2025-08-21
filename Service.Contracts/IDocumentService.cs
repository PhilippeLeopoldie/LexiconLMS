using LMS.Shared.DTOs.DocumentDtos;
using Microsoft.AspNetCore.Http;

namespace Service.Contracts;
public interface IDocumentService
{
    Task<IEnumerable<DocumentDto>> GetAllAsync();
    Task<DocumentDto> GetByIdAsync(int id);
    Task UpdateAsync(int id, DocumentUpdateDto documentDto);
    Task RestoreAsync(int id, bool restore = false);
    Task DeleteAsync(int id, string userId);
    Task<IEnumerable<DocumentDto>> GetDocumentsByCourseAsync(int courseId);
    Task<IEnumerable<DocumentDto>> GetDocumentsByModuleAsync(int moduleId);
    Task<IEnumerable<DocumentDto>> GetDocumentsByActivityAsync(int activityId);
    Task<IEnumerable<DocumentDto>> GetDocumentsByUserAsync(string userId);
    Task ShareDocumentAsync(int documentId, string sharerUserId, int? courseId, int? moduleId, int? activityId);
    Task<int> UploadAsync(IFormFile file, string webRootPath, string userId, int? courseId, int? moduleId, int? activityId);
    Task<(Stream stream, string fileName, string contentType)> DownloadAsync(int documentId);
}
