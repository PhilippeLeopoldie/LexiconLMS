using Service.Contracts;

namespace Domain.Models.Entities;

public class Course : BaseModel
{
    public DateTime Starts { get; set; }
    public DateTime Ends { get; set; }
}
