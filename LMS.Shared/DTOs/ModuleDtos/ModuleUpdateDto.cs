
namespace LMS.Shared.DTOs.ModuleDtos;

public record ModuleUpdateDto: ModuleForManipulationDto
{
    public int Id { get; init; }
}
