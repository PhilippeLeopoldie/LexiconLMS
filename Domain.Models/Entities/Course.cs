namespace Domain.Models.Entities;

public class Course : BaseModel
{
    public class Module : BaseModel { }  // TODO: Remove when actual Module class is defined
    public DateTime Starts { get; set; }
    public DateTime Ends { get; set; }

    public string StudentId { get; set; } = null!;
    public ApplicationUser Student { get; set; } = null!;

    public ICollection<Module> Modules { get; set; } = [];
}
