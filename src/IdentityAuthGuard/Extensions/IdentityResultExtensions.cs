using Core.Application.Exceptions;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace IdentityAuthGuard.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="IdentityResult"/> to throw custom exceptions based on error codes.
    /// </summary>
    internal static class IdentityResultExtensions
    {
        /// <summary>
        /// Throws a custom exception based on the error code in the <see cref="IdentityResult"/>.
        /// </summary>
        /// <param name="result">The <see cref="IdentityResult"/> containing error information.</param>
        /// <param name="unknownError">A fallback error message if no errors are present in the result.</param>
        /// <exception cref="BadRequestException">Thrown when the error code is 400.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the error code is 401.</exception>
        /// <exception cref="ForbiddenException">Thrown when the error code is 403.</exception>
        /// <exception cref="NotFoundException">Thrown when the error code is 404.</exception>
        /// <exception cref="AlreadyExistsException">Thrown when the error code is 409.</exception>
        /// <exception cref="InternalServerErrorException">Thrown for all other error codes.</exception>
        public static void ErrorResponse(IdentityResult result, string unknownError = "")
        {
            if (result.Succeeded)
            {
                return;
            }

            var errors = result.Errors
                    .Select(e => e.Description?.Trim(' ', '.') ?? string.Empty)
                    .Where(error => !string.IsNullOrWhiteSpace(error))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

            var message = errors.Count > 0
                ? string.Join(". ", errors) + "."
                : unknownError;

            if (!int.TryParse(result?.Errors.First().Code, out int code))
            {
                code = 500;
            }

            throw code switch
            {
                (int)HttpStatusCode.BadRequest => new BadRequestException(message),
                (int)HttpStatusCode.Unauthorized => new UnauthorizedAccessException(message),
                (int)HttpStatusCode.Forbidden => new ForbiddenException(message),
                (int)HttpStatusCode.NotFound => new NotFoundException(message),
                (int)HttpStatusCode.Conflict => new AlreadyExistsException(message),
                _ => new InternalServerErrorException(message),
            };
        }
    }
}
