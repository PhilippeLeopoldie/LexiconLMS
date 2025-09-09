namespace Domain.Models.Exceptions;


public  class BadRequestException : Exception
{
    public string Title { get; set; }
    public BadRequestException(string message, string title = "Bad request") : base(message)
    {
        Title = title;
    }
}

public class InvalidEntryBadRequestException : BadRequestException
{
    public InvalidEntryBadRequestException(int id) : base($"Ogiltigt ID: '{id}'")
    {
    }
    public InvalidEntryBadRequestException() : base($"No patchDocument")
    {
    }
}

