using System.Globalization;

namespace Core.Head.Exceptions
{
    public sealed class InternalServerErrorException : Exception
    {
        public InternalServerErrorException() : base() { }

        public InternalServerErrorException(string message) : base(message) { }

        public InternalServerErrorException(string message, Exception? innerException) : base(message, innerException) { }

        public InternalServerErrorException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
