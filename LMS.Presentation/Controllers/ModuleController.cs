using LMS.Shared.Common;
using LMS.Shared.DTOs.ModuleDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;

namespace LMS.Presentation.Controllers;

[Route("api/courses/{courseId}:int/[controller]")]
[ApiController]
[Authorize]
public class ModuleController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public ModuleController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager ??
            throw new ArgumentNullException(nameof(serviceManager));
    }

    [HttpGet]
    [Authorize(Roles = "Teacher, Student")]
    [SwaggerOperation(Summary = "Get all modules", Description = "Retrieves a list of all modules for a specific course.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Activities retrieved successfully", typeof(IEnumerable<ModuleDto>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request parameters")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<ActionResult<IEnumerable<ModuleDto>>> GetModules(int courseId, [FromQuery] ModuleRequestParams parameter)
    {
        var (modules, metadata) = await _serviceManager.ModuleService.GetAllModulesAsync(courseId, parameter);
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
        return Ok(modules);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Teacher, Student")]
    [SwaggerOperation(Summary = "Get module by ID", Description = "Retrieves a specific module by its ID within a course.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Module retrieved successfully", typeof(ModuleDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Module not found")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<ActionResult<ModuleDto>> GetModule(int courseId, int id, bool includeActivities)
    {
        var module = await _serviceManager.ModuleService.GetModuleByIdAsync(courseId, id, includeActivities);

        return Ok(module);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Teacher")]
    [SwaggerOperation(Summary = "Update module", Description = "Updates an existing module within a course.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Module updated successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid module data")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Module not found")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<IActionResult> PutModule(int courseId, int id, ModuleUpdateDto moduleDto)
    {
        await _serviceManager.ModuleService.UpdateModuleAsync(courseId, id, moduleDto);
        return NoContent();
    }
}
