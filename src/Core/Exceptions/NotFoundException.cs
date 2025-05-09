using System;
using System.Globalization;

namespace Core.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when a requested resource is not found.
    /// </summary>
    public sealed class NotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class.
        /// </summary>
        public NotFoundException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public NotFoundException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public NotFoundException(string message, Exception? innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class with a specified error message and arguments to format the message.
        /// </summary>
        /// <param name="message">The composite format string that describes the error.</param>
        /// <param name="args">An array of objects to format the message.</param>
        public NotFoundException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
