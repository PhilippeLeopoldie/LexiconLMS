namespace Domain.Models.Exceptions;
public  class NotFoundException : Exception
{
    public string Title { get; }
    public NotFoundException(string message, string title = "Not Found") : base(message)
    {
        Title = title;
    }
}

public class ModuleNotFoundException : NotFoundException
{
    public ModuleNotFoundException(int id) : base($"The Module with id: {id} is not found!")
    {
    }

    public ModuleNotFoundException(string name) : base($"The Module '{name}' is not found!")
    {
    }
}

public class CourseNotFoundException : NotFoundException
{
    public CourseNotFoundException(int id) : base($"No Course with id: {id}  found!")
    {
    }
}
