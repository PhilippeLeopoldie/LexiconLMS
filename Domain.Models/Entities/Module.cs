
namespace Domain.Models.Entities;

public class Module: BaseModel
{
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }

    public required Course Course { get; set; }
    public int CourseId { get; set; }

    public IEnumerable<Activity> Activities { get; set; }

}