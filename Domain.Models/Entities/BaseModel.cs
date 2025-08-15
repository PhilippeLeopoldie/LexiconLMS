namespace Domain.Models.Entities;
abstract public class BaseModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}
