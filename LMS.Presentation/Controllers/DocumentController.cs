using LMS.Shared.DTOs.DocumentDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace LMS.Presentation.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DocumentsController(IServiceManager serviceManager) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Teacher, Student")]
    [SwaggerOperation(Summary = "Get all documents", Description = "Retrieves a list of all documents.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Documents retrieved successfully", typeof(IEnumerable<DocumentDto>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request parameters")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<ActionResult<IEnumerable<DocumentDto>>> GetDocuments()
    {
        var documents = await serviceManager.DocumentService.GetAllAsync();
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
    public async Task<ActionResult<IEnumerable<DocumentDto>>> GetDocumentsByCourse(int courseId)
    {
        var documents = await serviceManager.DocumentService.GetDocumentsByCourseAsync(courseId);
        return Ok(documents);
    }

    [HttpGet("module/{moduleId}")]
    [Authorize(Roles = "Teacher, Student")]
    [SwaggerOperation(Summary = "Get documents by module", Description = "Retrieves all documents associated with a specific module.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Documents retrieved successfully", typeof(IEnumerable<DocumentDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<ActionResult<IEnumerable<DocumentDto>>> GetDocumentsByModule(int moduleId)
    {
        var documents = await serviceManager.DocumentService.GetDocumentsByModuleAsync(moduleId);
        return Ok(documents);
    }

    [HttpGet("activity/{activityId}")]
    [Authorize(Roles = "Teacher, Student")]
    [SwaggerOperation(Summary = "Get documents by activity", Description = "Retrieves all documents associated with a specific activity.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Documents retrieved successfully", typeof(IEnumerable<DocumentDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<ActionResult<IEnumerable<DocumentDto>>> GetDocumentsByActivity(int activityId)
    {
        var documents = await serviceManager.DocumentService.GetDocumentsByActivityAsync(activityId);
        return Ok(documents);
    }

    [HttpGet("user/{userId}")]
    [Authorize(Roles = "Teacher, Student")]
    [SwaggerOperation(Summary = "Get documents by user", Description = "Retrieves all documents uploaded by a specific user.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Documents retrieved successfully", typeof(IEnumerable<DocumentDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<ActionResult<IEnumerable<DocumentDto>>> GetDocumentsByUser(string userId)
    {
        var documents = await serviceManager.DocumentService.GetDocumentsByUserAsync(userId);
        return Ok(documents);
    }

    [HttpPost]
    [Authorize(Roles = "Teacher")]
    [SwaggerOperation(Summary = "Create document", Description = "Creates a new document.")]
    [SwaggerResponse(StatusCodes.Status201Created, "Document created successfully", typeof(DocumentDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid document data")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<ActionResult<DocumentDto>> PostDocument(DocumentManipulationDto document)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in token");

        var createdDocument = await serviceManager.DocumentService.CreateAsync(document, userId);
        return CreatedAtAction(nameof(GetDocument), new { id = createdDocument.Id }, createdDocument);
    }

    [HttpPost("share")]
    [Authorize(Roles = "Student")]
    [SwaggerOperation(Summary = "Share document", Description = "Shares a document with a course, module, or activity.")]
    [SwaggerResponse(StatusCodes.Status201Created, "Document shared successfully", typeof(DocumentDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid document data")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<ActionResult<DocumentDto>> ShareDocument(DocumentManipulationDto document, int courseId, int? moduleId = null, int? activityId = null)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in token");

        var sharedDocument = await serviceManager.DocumentService.ShareDocumentAsync(document, userId, courseId, moduleId, activityId);
        return CreatedAtAction(nameof(GetDocument), new { id = sharedDocument.Id }, sharedDocument);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Teacher")]
    [SwaggerOperation(Summary = "Update document", Description = "Updates an existing document.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Document updated successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid document data")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Document not found")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<IActionResult> PutDocument(int id, DocumentManipulationDto document)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in token");

        await serviceManager.DocumentService.UpdateAsync(id, document, userId);
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
}
