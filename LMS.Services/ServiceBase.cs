using Domain.Models.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace LMS.Services;

public abstract class ServiceBase
{
    protected static void EnsureNotNull<T>(T obj, string message)
    {
        if (obj == null)
            throw new BadRequestException(message);
    }

    protected static bool ValidateEntity<T>(T entity, out string? errors)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var validationContext = new ValidationContext(entity);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(entity, validationContext, validationResults, true);
        errors = string.Join("; ", validationResults.Select(x => x.ErrorMessage ?? string.Empty));
        return isValid;
    }

    protected static void ValidateDateRange(DateTime? startDate, DateTime? endDate)
    {
        if (startDate.HasValue && endDate.HasValue && startDate.Value >= endDate.Value)
            throw new BadRequestException("Start date must be before end date.");
    }
}
