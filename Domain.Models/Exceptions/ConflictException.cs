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
    public ModuleOverlappingException(string moduleRange) : base($"Tidsintervallet: {moduleRange} överlappar med en annan module i denna kurs.")
    {
    }

    public ModuleOverlappingException(DateTime courseStart, DateTime courseEnd) 
        : base($"Modulen måste ligga inom kursens datum: {courseStart.Date} - {courseEnd.Date}.")
    {
    }

}

public class ActivityOverlapException(string range)
    : ConflictException($"Tidsintervallet: {range} överlappar med en annan aktivitet i denna module.")
{ }

public class DuplicateStudentInCourseException : ConflictException
{
    public DuplicateStudentInCourseException(string studentId, int courseId) : base($"Eleven {studentId} är redan registrerad på kursen {courseId}.")
    {
    }
}

public class DuplicateTeacherInCourseException : ConflictException
{
    public DuplicateTeacherInCourseException(string teacherId, int courseId) : base($"Läraren {teacherId} är redan registrerad på kursen {courseId}.")
    {
    }
}
