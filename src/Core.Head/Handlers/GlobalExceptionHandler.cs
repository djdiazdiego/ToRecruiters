using Core.Exceptions;
using Core.Exceptions.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Core.Head.Handlers;
/// <summary>
/// Handles global exceptions and formats them into a standardized response.
/// </summary>
/// <param name="logger">The logger instance used for logging errors.</param>
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    /// <summary>
    /// Attempts to handle the given exception and write a standardized response to the HTTP context.
    /// </summary>
    /// <param name="context">The HTTP context of the current request.</param>
    /// <param name="exception">The exception to handle.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> containing a boolean indicating whether the exception was handled.
    /// </returns>
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Error occurred at {Time}", DateTime.UtcNow);

        // Extracts detailed error message and exception type name.
        string detail = exception.GetAllMessages();
        string title = exception.GetType().Name;

        // Maps exception types to corresponding HTTP status codes.
        int statusCode = exception switch
        {
            InternalServerErrorException => StatusCodes.Status500InternalServerError,
            ValidationException => StatusCodes.Status400BadRequest,
            BadRequestException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            AlreadyExistsException => StatusCodes.Status409Conflict,
            ForbiddenException => StatusCodes.Status403Forbidden,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        context.Response.StatusCode = statusCode;

        // Constructs a ProblemDetails object to standardize the error response.
        var problemDetails = new ProblemDetails
        {
            Title = title,
            Detail = detail,
            Status = statusCode,
            Instance = context.Request.Path
        };

        // Adds additional metadata to the response.
        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        if (exception is ValidationException validationException)
        {
            problemDetails.Extensions["ValidationErrors"] = validationException.Errors;
        }

        // Writes the ProblemDetails object as a JSON response.
        await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);
        return true;
    }
}
