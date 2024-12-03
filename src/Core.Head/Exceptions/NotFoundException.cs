using System.Globalization;

namespace Core.Head.Exceptions
{
    public sealed class NotFoundException : Exception
    {
        public NotFoundException() : base() { }

        public NotFoundException(string message) : base(message) { }

        public NotFoundException(string message, Exception? innerException) : base(message, innerException) { }

        public NotFoundException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
