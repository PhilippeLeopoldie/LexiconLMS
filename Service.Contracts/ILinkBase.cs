namespace Service.Contracts;

public interface ILinkBase
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Starts { get; set; }
    public DateTime Ends { get; set; }  
}
