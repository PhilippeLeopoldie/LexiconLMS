using LMS.Shared.DTOs.DocumentDtos;

namespace Service.Contracts;
public interface IDocumentService
{
    Task<IEnumerable<DocumentDto>> GetAllAsync();
    Task<DocumentDto> GetByIdAsync(int id);
    Task<DocumentDto> CreateAsync(DocumentManipulationDto documentDto, string userId);
    Task UpdateAsync(int id, DocumentManipulationDto documentDto, string userId);
    Task DeleteAsync(int id, string userId);
    Task<IEnumerable<DocumentDto>> GetDocumentsByCourseAsync(int courseId);
    Task<IEnumerable<DocumentDto>> GetDocumentsByModuleAsync(int moduleId);
    Task<IEnumerable<DocumentDto>> GetDocumentsByActivityAsync(int activityId);
    Task<IEnumerable<DocumentDto>> GetDocumentsByUserAsync(string userId);
    Task<DocumentDto> ShareDocumentAsync(DocumentManipulationDto documentDto, string userId, int courseId, int? moduleId = null, int? activityId = null);

}
