

namespace Domain.Models.Entities;
public class Activity : BaseModel
{
    public required DateTime StartsAt { get; set; }
    public required DateTime EndsAt { get; set; }
    public required ActivityType Type { get; set; }

    //// Foreign Key
    //public int ModuleId { get; set; }
    //// Navigation property
    //public Module Module { get; set; }

    //// Collection for documents
    //public ICollection<Document> Documents { get; set; }
}
