using System.Globalization;

namespace Core.Head.Exceptions
{
    public sealed class BadRequestException : Exception
    {
        public BadRequestException() : base() { }

        public BadRequestException(string message) : base(message) { }

        public BadRequestException(string message, Exception? innerException) : base(message, innerException) { }

        public BadRequestException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
