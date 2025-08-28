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
    public ModuleNotFoundException(int id, int courseId) : base($"The Module with id: {id}  is not found in course with id: {courseId}!")
    {
    }
    public ModuleNotFoundException(int id) : base($"The Module with id: {id}  is not found!")
    {
    }

    public ModuleNotFoundException(string name) : base($"The Module '{name}' is not found!")
    {
    }
}

public class CourseNotFoundException : NotFoundException
{
    public CourseNotFoundException(int courseId) : base($"No Course with id: {courseId}  found!")
    {
    }
}

public class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(string userId) : base($"No user with id: {userId}  found!")
    {
    }
}
