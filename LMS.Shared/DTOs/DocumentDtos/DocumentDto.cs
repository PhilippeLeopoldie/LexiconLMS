namespace LMS.Shared.DTOs.DocumentDtos;
public record DocumentDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public DateTime UploadedAt { get; init; }
    public DateTime CreatedAt { get; init; }
    public required string StoragePath { get; init; }
    public int Size { get; init; }
    public string? FileType { get; init; }
    public required string UploadedByUserId { get; init; }
    public int? CourseId { get; init; }
    public int? ModuleId { get; init; }
    public int? ActivityId { get; init; }
}
