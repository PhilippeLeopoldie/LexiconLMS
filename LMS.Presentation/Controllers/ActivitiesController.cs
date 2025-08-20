using LMS.Shared.Common;
using LMS.Shared.DTOs.ActivityDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;

namespace LMS.Presentation.Controllers
{
    [Route("api/modules/{moduleId:int}/[controller]")]
    [ApiController]
    [Authorize]
    public class ActivitiesController(IServiceManager serviceManager) : ControllerBase
    {

        [HttpGet]
        [Authorize(Roles = "Teacher, Student")]
        [SwaggerOperation(Summary = "Get all activities", Description = "Retrieves a list of all activities for a specific module.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Activities retrieved successfully", typeof(IEnumerable<ActivityDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request parameters")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
        public async Task<ActionResult<IEnumerable<ActivityDto>>> GetActivities(int moduleId, [FromQuery] RequestParams parameter)
        {
            var (activities, metaData) = await serviceManager.ActivityService.GetAllAsync(moduleId, parameter);
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metaData));
            return Ok(activities);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Teacher, Student")]
        [SwaggerOperation(Summary = "Get activity by ID", Description = "Retrieves a specific activity by its ID within a module.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Activity retrieved successfully", typeof(ActivityDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Activity not found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
        public async Task<ActionResult<ActivityDto>> GetActivity(int moduleId, int id)
        {
            var activity = await serviceManager.ActivityService.GetByIdAsync(moduleId, id);

            return activity;
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Teacher")]
        [SwaggerOperation(Summary = "Update activity", Description = "Updates an existing activity within a module.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Activity updated successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid activity data")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Activity not found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
        public async Task<IActionResult> PutActivity(int moduleId, int id, ActivityEditDto activity)
        {
            await serviceManager.ActivityService.UpdateAsync(moduleId, id, activity);
            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        [SwaggerOperation(Summary = "Create activity", Description = "Creates a new activity within a module.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Activity created successfully", typeof(ActivityDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid activity data")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Module not found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
        public async Task<ActionResult<ActivityDto>> PostActivity(int moduleId, ActivityCreateDto activity)
        {

            var result = await serviceManager.ActivityService.CreateAsync(moduleId, activity);
            return CreatedAtAction(actionName: nameof(GetActivity), routeValues: new { moduleId, id = result.createdActivityId }, value: result.activityDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Teacher")]
        [SwaggerOperation(Summary = "Delete activity", Description = "Deletes an existing activity within a module.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Activity deleted successfully")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Activity not found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
        public async Task<IActionResult> DeleteActivity(int moduleId, int id)
        {
            await serviceManager.ActivityService.DeleteAsync(moduleId, id);
            return NoContent();
        }
    }
}
