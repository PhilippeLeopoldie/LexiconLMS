namespace Domain.Models.Exceptions;

public class ConflictException : Exception
{
    public string Title { get; set; }
    public ConflictException(string message, string title = "Conflict") : base(message)
    {
        Title = title;
    }
}

public class ModuleOverlappingException : ConflictException
{
    public ModuleOverlappingException(string moduleRange) : base($"The time range: {moduleRange} overlaps with an other module in this course.")
    {
    }

    public ModuleOverlappingException(DateTime courseStart, DateTime courseEnd) 
        : base($"Module must be within course dates: {courseStart} - {courseEnd}.")
    {
    }

}

public class ActivityOverlapException(string range)
    : ConflictException($"The time range: {range} overlaps with an other activity in this module.")
{ }

