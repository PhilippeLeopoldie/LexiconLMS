using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs.DocumentDtos;
using Microsoft.AspNetCore.Identity;
using Service.Contracts;

namespace LMS.Services;
public class DocumentService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager) : IDocumentService
{
    public async Task<IEnumerable<DocumentDto>> GetAllAsync()
    {
        var documents = await unitOfWork.DocumentRepository.GetAllAsync();
        return mapper.Map<IEnumerable<DocumentDto>>(documents);
    }
    public async Task<DocumentDto> GetByIdAsync(int id)
    {
        var document = await unitOfWork.DocumentRepository.GetByIdAsync(id);
        if (document == null)
            throw new NotFoundException($"Document with ID {id} not found");

        return mapper.Map<DocumentDto>(document);
    }

    public async Task<DocumentDto> CreateAsync(DocumentManipulationDto documentDto, string userId)
    {
        if (string.IsNullOrWhiteSpace(documentDto.Name))
            throw new BadRequestException("Document name is required");

        if (string.IsNullOrWhiteSpace(documentDto.StoragePath))
            throw new BadRequestException("Storage path is required");

        if (documentDto.Size <= 0)
            throw new BadRequestException("Document size must be greater than 0");

        if (!documentDto.CourseId.HasValue && !documentDto.ModuleId.HasValue && !documentDto.ActivityId.HasValue)
            throw new BadRequestException("Document must be associated with a course, module, or activity");

        var user = await userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException($"User with ID {userId} not found");

        var document = new Document
        {
            Name = documentDto.Name,
            Description = documentDto.Description,
            StoragePath = documentDto.StoragePath,
            Size = documentDto.Size,
            FileType = documentDto.FileType,
            UploadedAt = DateTime.UtcNow,
            UploadedByUserId = userId,
            User = user,
            CourseId = documentDto.CourseId,
            ModuleId = documentDto.ModuleId,
            ActivityId = documentDto.ActivityId
        };

        unitOfWork.DocumentRepository.Create(document);
        await unitOfWork.CompleteAsync();

        return mapper.Map<DocumentDto>(document);
    }

    public async Task UpdateAsync(int id, DocumentManipulationDto documentDto, string userId)
    {
        var existingDocument = await unitOfWork.DocumentRepository.GetByIdAsync(id, true)
            ?? throw new NotFoundException($"Document with ID {id} not found");

        if (existingDocument.UploadedByUserId != userId)
            throw new BadRequestException("You can only update documents you uploaded");

        if (string.IsNullOrWhiteSpace(documentDto.Name))
            throw new BadRequestException("Document name is required");

        if (string.IsNullOrWhiteSpace(documentDto.StoragePath))
            throw new BadRequestException("Storage path is required");

        if (documentDto.Size <= 0)
            throw new BadRequestException("Document size must be greater than 0");

        existingDocument.Name = documentDto.Name;
        existingDocument.Description = documentDto.Description;
        existingDocument.StoragePath = documentDto.StoragePath;
        existingDocument.Size = documentDto.Size;
        existingDocument.FileType = documentDto.FileType;

        unitOfWork.DocumentRepository.Create(existingDocument);
        await unitOfWork.CompleteAsync();
    }

    public async Task DeleteAsync(int id, string userId)
    {
        var document = await unitOfWork.DocumentRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Document with ID {id} not found");

        if (document.UploadedByUserId != userId)
            throw new BadRequestException("You can only delete documents you uploaded");

        unitOfWork.DocumentRepository.Delete(document);
        await unitOfWork.CompleteAsync();
    }

    public async Task<IEnumerable<DocumentDto>> GetDocumentsByCourseAsync(int courseId)
    {
        var documents = await unitOfWork.DocumentRepository.GetDocumentsByCourseAsync(courseId);
        return mapper.Map<IEnumerable<DocumentDto>>(documents);
    }

    public async Task<IEnumerable<DocumentDto>> GetDocumentsByModuleAsync(int moduleId)
    {
        var documents = await unitOfWork.DocumentRepository.GetDocumentsByModuleAsync(moduleId);
        return mapper.Map<IEnumerable<DocumentDto>>(documents);
    }

    public async Task<IEnumerable<DocumentDto>> GetDocumentsByActivityAsync(int activityId)
    {
        var documents = await unitOfWork.DocumentRepository.GetDocumentsByActivityAsync(activityId);
        return mapper.Map<IEnumerable<DocumentDto>>(documents);
    }

    public async Task<IEnumerable<DocumentDto>> GetDocumentsByUserAsync(string userId)
    {
        var documents = await unitOfWork.DocumentRepository.GetDocumentsByUserAsync(userId);
        return mapper.Map<IEnumerable<DocumentDto>>(documents);
    }

    public async Task<DocumentDto> ShareDocumentAsync(DocumentManipulationDto documentDto, string userId, int courseId, int? moduleId = null, int? activityId = null)
    {
        if (string.IsNullOrWhiteSpace(documentDto.Name))
            throw new BadRequestException("Document name is required");

        if (string.IsNullOrWhiteSpace(documentDto.StoragePath))
            throw new BadRequestException("Storage path is required");

        if (documentDto.Size <= 0)
            throw new BadRequestException("Document size must be greater than 0");

        var user = await userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException($"User with ID {userId} not found");

        var document = new Document
        {
            Name = documentDto.Name,
            Description = documentDto.Description,
            StoragePath = documentDto.StoragePath,
            Size = documentDto.Size,
            FileType = documentDto.FileType,
            UploadedAt = DateTime.UtcNow,
            UploadedByUserId = userId,
            User = user,
            CourseId = courseId,
            ModuleId = moduleId,
            ActivityId = activityId
        };

        unitOfWork.DocumentRepository.Create(document);
        await unitOfWork.CompleteAsync();

        return mapper.Map<DocumentDto>(document);
    }
}
