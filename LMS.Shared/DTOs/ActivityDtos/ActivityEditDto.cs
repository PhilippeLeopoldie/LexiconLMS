namespace LMS.Shared.DTOs.ActivityDtos;
public record ActivityEditDto : ActivityManipulationDto
{
    public int Id { get; init; }
}
