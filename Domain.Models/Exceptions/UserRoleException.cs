using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Exceptions;

public class UserRoleException : Exception
{
    public string Title { get; set; }
    public UserRoleException(string message, string title = "Conflict") : base(message)
    {
        Title = title;
    }
}

public class UserIsNotTeacherException : UserRoleException
{
    public UserIsNotTeacherException(string userId) : base($"Användaren med ID '{userId}' har inte den nödvändiga rollen 'Lärare'.")
    {
    }
}

public class UserIsNotStudentException : UserRoleException
{
    public UserIsNotStudentException(string userId) : base($"Användaren med ID '{userId}' har inte rollen 'Elev'.")
    {
    }
}
