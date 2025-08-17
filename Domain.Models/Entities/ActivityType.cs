namespace Domain.Models.Entities;
public class ActivityType : BaseModel
{
    public ICollection<Activity> Activities { get; set; } = [];
}
