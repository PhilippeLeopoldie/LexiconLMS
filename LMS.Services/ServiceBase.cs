using Domain.Models.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace LMS.Services;

public class ServiceBase
{
    public virtual void EnsureNotNull<T>(T obj, string message)
    {
        if (obj == null)
            throw new BadRequestException(message);
    }

    public virtual bool ValidateEntity<T>(T entity, out string? errors)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var validationContext = new ValidationContext(entity);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(entity, validationContext, validationResults, true);
        errors = string.Join("; ", validationResults.Select(x => x.ErrorMessage ?? string.Empty));
        return isValid;
    }
}
