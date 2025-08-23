using LMS.Shared.Common;
using LMS.Shared.DTOs.DocumentDtos;
using Microsoft.AspNetCore.Http;

namespace Service.Contracts;
public interface IDocumentService
{
    Task<(IEnumerable<DocumentDto>, MetaData metaData)> GetAllAsync(RequestParams requestParams, bool trackChanges = false);
    Task<DocumentDto> GetByIdAsync(int id);
    Task UpdateAsync(int id, DocumentUpdateDto documentDto);
    Task RestoreAsync(int id, bool restore = false);
    Task DeleteAsync(int id, string userId);
    Task<(IEnumerable<DocumentDto>, MetaData metaData)> GetDocumentsByCourseAsync(int courseId, RequestParams requestParams, bool trackChanges = false);
    Task<(IEnumerable<DocumentDto>, MetaData metaData)> GetDocumentsByModuleAsync(int moduleId, RequestParams requestParams, bool trackChanges = false);
    Task<(IEnumerable<DocumentDto>, MetaData metaData)> GetDocumentsByActivityAsync(int activityId, RequestParams requestParams, bool trackChanges = false);
    Task<(IEnumerable<DocumentDto>, MetaData metaData)> GetDocumentsByUserAsync(string userId, RequestParams requestParams, bool trackChanges = false);
    Task<(IEnumerable<DocumentDto>, MetaData metaData)> GetSubmissionsForActivityAsync(int activityId, RequestParams requestParams, bool trackChanges = false);
    Task ShareDocumentAsync(int documentId, string sharerUserId, int? courseId, int? moduleId, int? activityId);
    Task<int> UploadAsync(IFormFile file, string webRootPath, string userId, int? courseId, int? moduleId, int? activityId);
    Task<(Stream stream, string fileName, string contentType)> DownloadAsync(int documentId);
}
