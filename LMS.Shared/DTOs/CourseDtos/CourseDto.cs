using LMS.Shared.DTOs.ModuleDtos;
using LMS.Shared.DTOs.UserDtos;

namespace LMS.Shared.DTOs.CourseDtos;

public record CourseDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTime Starts { get; init; }
    public DateTime Ends { get; set; }
    public IEnumerable<UserDto>? Teachers { get; init; } = null;
    //public UserDto? Teacher { get; init; } = null;
    public IEnumerable<UserDto>? Students { get; init; } = null;
    public IEnumerable<ModuleDto>? Modules { get; init; } = null;
}