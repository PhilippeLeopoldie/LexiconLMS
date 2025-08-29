using Domain.Models.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace LMS.API.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static void ConfigureExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(builder =>
        {
            builder.Run(async context =>
            {
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    var problemDetailsFactory = app.Services.GetRequiredService<ProblemDetailsFactory>();

                    ProblemDetails problemDetails;
                    int statusCode;

                    switch (contextFeature.Error)
                    {
                        case TokenValidationException tokenValidationException:
                            statusCode = tokenValidationException.StatusCode;
                            problemDetails = problemDetailsFactory.CreateProblemDetails(
                                    context,
                                    statusCode,
                                    detail: tokenValidationException.Message,
                                    instance: context.Request.Path);
                            break;
                        case NotFoundException notFoundException:
                            statusCode = StatusCodes.Status404NotFound;
                            problemDetails = problemDetailsFactory.CreateProblemDetails(
                                context,
                                statusCode,
                                title: notFoundException.Title,
                                detail: notFoundException.Message,
                                instance: context.Request.Path);
                            break;
                        case BadRequestException badRequestException:
                            statusCode = StatusCodes.Status400BadRequest;
                            problemDetails = problemDetailsFactory.CreateProblemDetails(
                                context,
                                statusCode,
                                title: badRequestException.Title,
                                detail: badRequestException.Message,
                                instance: context.Request.Path);
                            break;
                        case ConflictException conflictException:
                            statusCode = StatusCodes.Status409Conflict;
                            problemDetails = problemDetailsFactory.CreateProblemDetails(
                                context,
                                statusCode,
                                title: conflictException.Title,
                                detail: conflictException.Message,
                                instance: context.Request.Path);
                            break;
                        case UserRoleException conflictException:
                            statusCode = StatusCodes.Status403Forbidden;
                            problemDetails = problemDetailsFactory.CreateProblemDetails(
                                context,
                                statusCode,
                                title: conflictException.Title,
                                detail: conflictException.Message,
                                instance: context.Request.Path);
                            break;
                        default:
                            statusCode = StatusCodes.Status500InternalServerError;
                            problemDetails = problemDetailsFactory.CreateProblemDetails(
                                    context,
                                    statusCode,
                                    title: "Internal Server Error",
                                    detail: contextFeature.Error.Message,
                                    instance: context.Request.Path);
                            break;
                    }

                    context.Response.StatusCode = statusCode;
                    await context.Response.WriteAsJsonAsync(problemDetails);
                }
            });
        });
    }
}
