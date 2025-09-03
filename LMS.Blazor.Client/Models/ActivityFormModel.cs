using System.ComponentModel.DataAnnotations;

namespace LMS.Blazor.Client.Pages;
public class ActivityFormModel
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    [Required]
    public string StartsAtText { get; set; } = string.Empty;
    [Required]
    public string EndsAtText { get; set; } = string.Empty;
    [Range(1, int.MaxValue, ErrorMessage = "Välj en aktivitetstyp")]
    public int ActivityTypeId { get; set; }
}