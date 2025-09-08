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
    public ModuleNotFoundException(int id, int courseId) : base($"Modulen med id: {id} hittades inte i kursen med id: {courseId}!")
    {
    }
    public ModuleNotFoundException(int id) : base($"Modulen med id: {id} hittades inte!")
    {
    }

    public ModuleNotFoundException(string name) : base($"Modulen '{name}' hittades inte!")
    {
    }
}

public class CourseNotFoundException : NotFoundException
{
    public CourseNotFoundException(int courseId) : base($"Ingen kurs med id: {courseId} hittades!")
    {
    }
}

public class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(string userId) : base($"Ingen användare med id: {userId} hittades!")
    {
    }
}
