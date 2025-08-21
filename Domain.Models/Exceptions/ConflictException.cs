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
    public ModuleOverlappingException(string range) : base($"The time range: {range} overlaps with an other module in this course.")
    {
    }
}

public class ActivityOverlapException(string range)
    : ConflictException($"The time range: {range} overlaps with an other activity in this module.")
{ }

