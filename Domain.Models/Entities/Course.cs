namespace Domain.Models.Entities;

public class Course : BaseModel
{
    public DateTime Starts { get; set; }
    public DateTime Ends { get; set; }

    public ICollection<ApplicationUser> Students { get; set; } = [];
    public ICollection<ApplicationUser> Teachers { get; set; } = [];

    public ICollection<Module> Modules { get; set; } = [];
    public ICollection<Document> Documents { get; set; } = [];
}
