using Core.Head.Exceptions;
using Core.Head.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Core.Head.Handlers;
public sealed class GlobalExceptionHandler
    (ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(
            "Error Message: {exceptionMessage}, Time of occurrence {time}",
            exception.Message, DateTime.UtcNow);

        string detail = exception.GetAllMessages();
        string title = exception.GetType().Name;

        int statusCode = exception switch
        {
            InternalServerErrorException => context.Response.StatusCode = StatusCodes.Status500InternalServerError,
            ValidationException => context.Response.StatusCode = StatusCodes.Status400BadRequest,
            BadRequestException => context.Response.StatusCode = StatusCodes.Status400BadRequest,
            NotFoundException => context.Response.StatusCode = StatusCodes.Status404NotFound,
            AlreadyExistsException => context.Response.StatusCode = StatusCodes.Status409Conflict,
            ForbiddenException => context.Response.StatusCode = StatusCodes.Status403Forbidden,
            UnauthorizedAccessException => context.Response.StatusCode = StatusCodes.Status401Unauthorized,
            _ => context.Response.StatusCode = StatusCodes.Status500InternalServerError
        };

        var problemDetails = new ProblemDetails
        {
            Title = title,
            Detail = detail,
            Status = statusCode,
            Instance = context.Request.Path
        };

        problemDetails.Extensions.Add("traceId", context.TraceIdentifier);

        if (exception is ValidationException validationException)
        {
            problemDetails.Extensions.Add("ValidationErrors", validationException.Errors);
        }

        await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);
        return true;
    }
}
