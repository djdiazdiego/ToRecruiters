using Core.Exceptions;
using System;

namespace Core.Helpers
{
    public static class ExceptionHelpers
    {
        public static Exception CreateException(int code, string message, Exception? exception = null) => code switch
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
