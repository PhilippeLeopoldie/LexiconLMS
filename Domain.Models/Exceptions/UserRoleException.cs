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
    public UserIsNotTeacherException(string userId) : base($"User with ID '{userId}' does not have the required 'Teacher' role.")
    {
    }
}

public class UserIsNotStudentException : UserRoleException
{
    public UserIsNotStudentException(string userId) : base($"User with ID '{userId}' doesn't have a 'Student' role.")
    {
    }
}
