using LMS.Shared.Common;
using LMS.Shared.DTOs.DocumentDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using System.Text.Json;
using IFormFile = Microsoft.AspNetCore.Http.IFormFile;

namespace LMS.Presentation.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DocumentsController(IServiceManager serviceManager, IWebHostEnvironment webHostEnvironment) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Teacher, Student")]
    [SwaggerOperation(Summary = "Get all documents", Description = "Retrieves a list of all documents.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Documents retrieved successfully", typeof(IEnumerable<DocumentDto>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request parameters")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<ActionResult<IEnumerable<DocumentDto>>> GetDocuments([FromQuery] RequestParams parameter)
    {
        var (documents, metaData) = await serviceManager.DocumentService.GetAllAsync(parameter);
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metaData));
        return Ok(documents);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Teacher, Student")]
    [SwaggerOperation(Summary = "Get document by ID", Description = "Retrieves a specific document by its ID.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Document retrieved successfully", typeof(DocumentDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Document not found")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<ActionResult<DocumentDto>> GetDocument(int id)
    {
        var document = await serviceManager.DocumentService.GetByIdAsync(id);
        return Ok(document);
    }

    [HttpGet("course/{courseId}")]
    [Authorize(Roles = "Teacher, Student")]
    [SwaggerOperation(Summary = "Get documents by course", Description = "Retrieves all documents associated with a specific course.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Documents retrieved successfully", typeof(IEnumerable<DocumentDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<ActionResult<IEnumerable<DocumentDto>>> GetDocumentsByCourse(int courseId, [FromQuery] RequestParams parameter)
    {
        var (documents, metaData) = await serviceManager.DocumentService.GetDocumentsByCourseAsync(courseId, parameter);
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metaData));
        return Ok(documents);
    }

    [HttpGet("module/{moduleId}")]
    [Authorize(Roles = "Teacher, Student")]
    [SwaggerOperation(Summary = "Get documents by module", Description = "Retrieves all documents associated with a specific module.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Documents retrieved successfully", typeof(IEnumerable<DocumentDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<ActionResult<IEnumerable<DocumentDto>>> GetDocumentsByModule(int moduleId, [FromQuery] RequestParams parameter)
    {
        var (documents, metaData) = await serviceManager.DocumentService.GetDocumentsByModuleAsync(moduleId, parameter);
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metaData));
        return Ok(documents);
    }

    [HttpGet("activity/{activityId:int}")]
    [Authorize(Roles = "Teacher, Student")]
    [SwaggerOperation(Summary = "Get documents by activity", Description = "Retrieves all documents associated with a specific activity.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Documents retrieved successfully", typeof(IEnumerable<DocumentDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<ActionResult<IEnumerable<DocumentDto>>> GetDocumentsByActivity(int activityId, [FromQuery] RequestParams parameter)
    {
        var (documents, metaData) = await serviceManager.DocumentService.GetDocumentsByActivityAsync(activityId, parameter);
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metaData));
        return Ok(documents);
    }

    [HttpGet("submissions/{activityId:int}")]
    [Authorize(Roles = "Teacher")]
    [SwaggerOperation(Summary = "Get submissions for activity", Description = "Retrieves all document submissions for a specific activity.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Submissions retrieved successfully", typeof(IEnumerable<DocumentDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<ActionResult<IEnumerable<DocumentDto>>> GetSubmissionsForActivity(int activityId, [FromQuery] RequestParams parameter)
    {
        var (documents, metaData) = await serviceManager.DocumentService.GetSubmissionsForActivityAsync(activityId, parameter);
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metaData));
        return Ok(documents);
    }

    [HttpGet("user/{userId}")]
    [Authorize(Roles = "Teacher, Student")]
    [SwaggerOperation(Summary = "Get documents by user", Description = "Retrieves all documents uploaded by a specific user.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Documents retrieved successfully", typeof(IEnumerable<DocumentDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<ActionResult<IEnumerable<DocumentDto>>> GetDocumentsByUser(string userId, [FromQuery] RequestParams parameter)
    {
        var (documents, metaData) = await serviceManager.DocumentService.GetDocumentsByUserAsync(userId, parameter);
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metaData));
        return Ok(documents);
    }

    [HttpPost("{id:int}/share")]
    [Authorize(Roles = "Student, Teacher")]
    [SwaggerOperation(Summary = "Share document", Description = "Shares a document with a course, module, or activity.")]
    [SwaggerResponse(StatusCodes.Status201Created, "Document shared successfully", typeof(DocumentDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid document data")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<ActionResult<DocumentDto>> ShareDocument(int id, int? courseId, int? moduleId, int? activityId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in token");

        await serviceManager.DocumentService.ShareDocumentAsync(id, userId, courseId, moduleId, activityId);
        return CreatedAtAction(nameof(GetDocument), new { id }, null);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Teacher")]
    [SwaggerOperation(Summary = "Update document", Description = "Updates an existing document.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Document updated successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid document data")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Document not found")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<IActionResult> PutDocument(int id, DocumentUpdateDto document)
    {
        await serviceManager.DocumentService.UpdateAsync(id, document);
        return NoContent();
    }

    [HttpPut("{id}/restore")]
    [Authorize(Roles = "Teacher")]
    [SwaggerOperation(Summary = "Restore document", Description = "Restores a deleted document.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Document restored successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid restore request")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Document not found")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<IActionResult> RestoreDocument(int id, bool restore = false)
    {
        await serviceManager.DocumentService.RestoreAsync(id, restore);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Teacher")]
    [SwaggerOperation(Summary = "Delete document", Description = "Deletes an existing document.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Document deleted successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Document not found")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<IActionResult> DeleteDocument(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in token");

        await serviceManager.DocumentService.DeleteAsync(id, userId);
        return NoContent();
    }

    [HttpPost("upload")]
    [Authorize(Roles = "Teacher")]
    [SwaggerOperation(Summary = "Upload document", Description = "Uploads a document file to the server.")]
    [SwaggerResponse(StatusCodes.Status201Created, "Document uploaded successfully", typeof(int))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid file or parameters")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<ActionResult<int>> UploadDocument(IFormFile file, int? courseId, int? moduleId, int? activityId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in token");
        var webRootPath = webHostEnvironment.WebRootPath;
        var documentId = await serviceManager.DocumentService.UploadAsync(file, webRootPath, userId, courseId, moduleId, activityId);
        return CreatedAtAction(nameof(GetDocument), new { id = documentId }, documentId);
    }

    [HttpGet("{id:int}/download")]
    [Authorize(Roles = "Teacher, Student")]
    [SwaggerOperation(Summary = "Download document", Description = "Downloads a document file from the server.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Document downloaded successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Document not found")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<IActionResult> DownloadDocument(int id)
    {
        var (stream, fileName, contentType) = await serviceManager.DocumentService.DownloadAsync(id);
        return File(stream, contentType, fileName);
    }
}
