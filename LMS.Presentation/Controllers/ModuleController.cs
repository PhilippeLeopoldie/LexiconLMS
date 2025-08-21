using LMS.Shared.Common;
using LMS.Shared.DTOs.ActivityDtos;
using LMS.Shared.DTOs.ModuleDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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

}
