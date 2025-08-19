namespace Domain.Models.Exceptions;

public abstract class BadRequestException : Exception
{
    public string Title { get; set; }
    protected BadRequestException(string message, string title = "Bad request") : base(message)
    {
        Title = title;
    }
}

public class InvalidEntryBadRequestException : BadRequestException
{
    public InvalidEntryBadRequestException(int id) : base($"Invalid Id: '{id}'")
    {
    }
    public InvalidEntryBadRequestException() : base($"No patchDocument")
    {
    }
}
