

namespace Domain.Models.Entities;
public class Activity : BaseModel
{
    public required DateTime StartsAt { get; set; }
    public required DateTime EndsAt { get; set; }
    public required ActivityType Type { get; set; }
    public int ActivityTypeId { get; set; }

    public required Module Module { get; set; }
    public int ModuleId { get; set; }

}
