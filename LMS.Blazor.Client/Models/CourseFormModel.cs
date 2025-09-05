using System.ComponentModel.DataAnnotations;

namespace LMS.Blazor.Client.Models;

public class CourseFormModel
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    [Required]
    public string StartsAtText { get; set; } = string.Empty;
    [Required]
    public string EndsAtText { get; set; } = string.Empty;

}
