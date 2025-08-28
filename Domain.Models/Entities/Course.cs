namespace Domain.Models.Entities;

public class Course : BaseModel
{
    public DateTime Starts { get; set; }
    public DateTime Ends { get; set; }

    // Student side (many students per course)
    public ICollection<ApplicationUser> Students { get; set; } = [];
    public ICollection<ApplicationUser> Teachers { get; set; } = [];

    // Teacher side (one teacher teaches many courses)
    /*public string? TeacherId { get; set; }
    public ApplicationUser? Teacher { get; set; }*/

    public ICollection<Module> Modules { get; set; } = [];
    public ICollection<Document> Documents { get; set; } = [];
}
