using LMS.Shared.DTOs.ActivityDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Swashbuckle.AspNetCore.Annotations;

namespace LMS.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ActivityTypesController(IServiceManager serviceManager) : ControllerBase
    {

        [HttpGet]
        [SwaggerOperation(Summary = "Get all activity types", Description = "Retrieves a list of all activity types.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Activity types retrieved successfully", typeof(IEnumerable<ActivityTypeDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request parameters")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
        public async Task<ActionResult<IEnumerable<ActivityTypeDto>>> GetActivityTypes()
        {
            var activityTypes = await serviceManager.ActivityTypeService.GetAllAsync();
            return Ok(activityTypes);
        }
    }
}
