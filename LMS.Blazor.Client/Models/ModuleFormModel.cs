using System.ComponentModel.DataAnnotations;

namespace LMS.Blazor.Client.Models;

public class ModuleFormModel
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;
    [Required]
    public DateTime StartsAt { get; set; } = DateTime.Now;
    [Required]
    public DateTime EndsAt { get; set; } = DateTime.Now;
}
