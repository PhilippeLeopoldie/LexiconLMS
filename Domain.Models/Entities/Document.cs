namespace Domain.Models.Entities;
public class Document : BaseModel
{
    public DateTime UploadedAt { get; set; }
    public required string StoragePath { get; set; }
    public int Size { get; set; }
    public string? FileType { get; set; }

    public required string UploadedByUserId { get; set; }
    public required ApplicationUser User { get; set; }
    public int? CourseId { get; set; }
    public Course? Course { get; set; }
    public int? ModuleId { get; set; }
    public Module? Module { get; set; }
    public int? ActivityId { get; set; }
    public Activity? Activity { get; set; }
}
