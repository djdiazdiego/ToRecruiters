using Core.Application.Exceptions;

namespace Core.Application.Helpers
{
    /// <summary>
    /// Provides helper methods for creating exceptions based on HTTP status codes.
    /// </summary>
    public static class ExceptionHelpers
    {
        /// <summary>
        /// Creates an exception instance based on the provided HTTP status code, message, and optional inner exception.
        /// </summary>
        /// <param name="code">The HTTP status code.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="exception">The optional inner exception.</param>
        /// <returns>An exception corresponding to the provided HTTP status code.</returns>
        /// <exception cref="BadRequestException">Thrown when the status code is 400.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the status code is 401.</exception>
        /// <exception cref="ForbiddenException">Thrown when the status code is 403.</exception>
        /// <exception cref="NotFoundException">Thrown when the status code is 404.</exception>
        /// <exception cref="AlreadyExistsException">Thrown when the status code is 409.</exception>
        /// <exception cref="InternalServerErrorException">Thrown for all other status codes.</exception>
        public static Exception FromHttpStatusCode(int code, string message, Exception? exception = null) => code switch
        {
            400 => new BadRequestException(message, exception),
            401 => new UnauthorizedAccessException(message, exception),
            403 => new ForbiddenException(message, exception),
            404 => new NotFoundException(message, exception),
            409 => new AlreadyExistsException(message, exception),
            _ => new InternalServerErrorException(message, exception),
        };
    }
}
