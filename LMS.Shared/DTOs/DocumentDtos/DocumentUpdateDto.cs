namespace LMS.Shared.DTOs.DocumentDtos;
public record DocumentUpdateDto
{
    public string? Name { get; init; }
    public string? Description { get; init; }
}
