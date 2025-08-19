
namespace Domain.Models.Entities;

public class Module: BaseModel
{
    public DateTime Starts { get; set; }
    public DateTime Ends { get; set; }

    public required Course Course { get; set; }
    public int CourseId { get; set; }

    public ICollection<Activity> Activities { get; set; }

}