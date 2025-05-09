using System;
using System.Globalization;

namespace Core.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when a forbidden action is attempted.
    /// </summary>
    public sealed class ForbiddenException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class.
        /// </summary>
        public ForbiddenException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ForbiddenException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ForbiddenException(string message, Exception? innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class with a specified error message and arguments to format the message.
        /// </summary>
        /// <param name="message">The composite format string that describes the error.</param>
        /// <param name="args">An array of objects to format the message.</param>
        public ForbiddenException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
