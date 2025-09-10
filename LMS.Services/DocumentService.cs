using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.Common;
using LMS.Shared.DTOs.DocumentDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Service.Contracts;

namespace LMS.Services;
public class DocumentService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager) : IDocumentService
{
    public async Task<(IEnumerable<DocumentDto>, MetaData metaData)> GetAllAsync(RequestParams requestParams, bool trackChanges = false)
    {
        ArgumentNullException.ThrowIfNull(requestParams, nameof(requestParams));
        var documents = await unitOfWork.DocumentRepository.GetAllAsync(requestParams, trackChanges);
        var mappedDocuments = mapper.Map<IEnumerable<DocumentDto>>(documents.Items);
        return (mappedDocuments, documents.MetaData);
    }
    public async Task<DocumentDto> GetByIdAsync(int id)
    {
        var document = await unitOfWork.DocumentRepository.GetByIdAsync(id);
        return document == null ? throw new NotFoundException($"Document with ID {id} not found")
                                : mapper.Map<DocumentDto>(document);
    }

    public async Task<(IEnumerable<DocumentDto>, MetaData metaData)> GetDocumentsByCourseAsync(int courseId, RequestParams requestParams, bool trackChanges = false)
    {
        var documents = await unitOfWork.DocumentRepository.GetDocumentsByCourseAsync(requestParams, courseId, trackChanges);
        var mappedDocuments = mapper.Map<IEnumerable<DocumentDto>>(documents.Items);
        return (mappedDocuments, documents.MetaData);
    }

    public async Task<(IEnumerable<DocumentDto>, MetaData metaData)> GetDocumentsByModuleAsync(int moduleId, RequestParams requestParams, bool trackChanges = false)
    {
        ArgumentNullException.ThrowIfNull(requestParams, nameof(requestParams));
        var documents = await unitOfWork.DocumentRepository.GetDocumentsByModuleAsync(requestParams, moduleId, trackChanges);
        var mappedDocuments = mapper.Map<IEnumerable<DocumentDto>>(documents.Items);
        return (mappedDocuments, documents.MetaData);
    }

    public async Task<(IEnumerable<DocumentDto>, MetaData metaData)> GetDocumentsByActivityAsync(int activityId, RequestParams requestParams, bool trackChanges = false)
    {
        ArgumentNullException.ThrowIfNull(requestParams, nameof(requestParams));
        var documents = await unitOfWork.DocumentRepository.GetDocumentsByActivityAsync(requestParams, activityId, trackChanges);
        var mappedDocuments = mapper.Map<IEnumerable<DocumentDto>>(documents.Items);
        return (mappedDocuments, documents.MetaData);
    }

    public async Task<(IEnumerable<DocumentDto>, MetaData metaData)> GetDocumentsByUserAsync(string userId, RequestParams requestParams, bool trackChanges = false)
    {
        ArgumentNullException.ThrowIfNull(requestParams, nameof(requestParams));
        var documents = await unitOfWork.DocumentRepository.GetDocumentsByUserAsync(requestParams, userId, trackChanges);
        var mappedDocuments = mapper.Map<IEnumerable<DocumentDto>>(documents.Items);
        return (mappedDocuments, documents.MetaData);
    }

    public async Task<(IEnumerable<DocumentDto>, MetaData metaData)> GetSubmissionsForActivityAsync(int activityId, RequestParams requestParams, bool trackChanges = false)
    {
        ArgumentNullException.ThrowIfNull(requestParams, nameof(requestParams));
        var submissions = await unitOfWork.DocumentRepository.GetDocumentsByActivityAsync(requestParams, activityId, trackChanges);
        var mappedDocuments = mapper.Map<IEnumerable<DocumentDto>>(submissions.Items);
        return (mappedDocuments, submissions.MetaData);
    }

    public async Task ShareDocumentAsync(int documentId, string sharerUserId, int? courseId, int? moduleId, int? activityId)
    {
        var document = await unitOfWork.DocumentRepository.GetByIdAsync(documentId, true)
            ?? throw new NotFoundException($"Document with ID {documentId} not found.");

        if (document.DeletedAt != null)
            throw new BadRequestException("Cannot share a deleted document.");

        var sharerUser = await userManager.FindByIdAsync(sharerUserId);
        if (sharerUser == null || (!document.UploadedByUserId.Equals(sharerUserId) && !await userManager.IsInRoleAsync(sharerUser, "Teacher")))
            throw new BadRequestException("You do not have permission to share this document.");

        if (courseId == null && moduleId == null && activityId == null)
            throw new BadRequestException("Must specify a course, module, or activity to share the document with.");

        if ((courseId != null && moduleId != null) || (courseId != null && activityId != null) || (moduleId != null && activityId != null))
            throw new BadRequestException("Document can only be shared with one entity at a time.");

        document.CourseId = courseId;
        document.ModuleId = moduleId;
        document.ActivityId = activityId;

        await unitOfWork.CompleteAsync();
    }

    public async Task<int> UploadAsync(IFormFile file, string webRootPath, string userId, int? courseId, int? moduleId, int? activityId)
    {
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException($"User with ID {userId} not found");

        if (file == null || file.Length == 0)
            throw new BadRequestException("No file selected for upload.");

        var uploadsFolder = Path.Combine(webRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        await using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        var document = new Document
        {
            UploadedByUserId = userId,
            Name = file.FileName,
            StoragePath = filePath,
            Size = (int)file.Length,
            FileType = file.ContentType,
            UploadedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CourseId = courseId,
            ModuleId = moduleId,
            ActivityId = activityId,
            User = user
        };

        unitOfWork.DocumentRepository.Create(document);
        await unitOfWork.CompleteAsync();

        return document.Id;
    }

    public async Task<(Stream stream, string fileName, string contentType)> DownloadAsync(int documentId)
    {
        var document = await unitOfWork.DocumentRepository.GetByIdAsync(documentId, false)
            ?? throw new NotFoundException($"Document with ID {documentId} not found.");

        var filePath = Path.Combine(document.StoragePath);

        if (!File.Exists(filePath))
            throw new NotFoundException("File not found on server.");

        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

        return (fileStream, Path.GetFileName(document.Name), document.FileType ?? "application/octet-stream");
    }

    public async Task UpdateAsync(int id, DocumentUpdateDto documentDto)
    {
        var existingDocument = await unitOfWork.DocumentRepository.GetByIdAsync(id, true)
            ?? throw new NotFoundException($"Document with ID {id} not found");

        mapper.Map(documentDto, existingDocument);
        await unitOfWork.CompleteAsync();
    }

    public async Task RestoreAsync(int id, bool restore = false)
    {
        var existingDocument = await unitOfWork.DocumentRepository.GetDeletedByIdAsync(id, true)
            ?? throw new NotFoundException($"Document with ID {id} not found");

        if (restore)
            existingDocument.DeletedAt = null;

        await unitOfWork.CompleteAsync();
    }

    public async Task DeleteAsync(int id, string userId)
    {
        var document = await unitOfWork.DocumentRepository.GetByIdAsync(id, true)
            ?? throw new NotFoundException($"Document with ID {id} not found");

        if (document.UploadedByUserId != userId)
            throw new BadRequestException("You can only delete documents you uploaded");

        document.DeletedAt = DateTime.UtcNow;
        await unitOfWork.CompleteAsync();
    }
}
