using System;
using System.Globalization;

namespace Core.Application.Exceptions
{
    /// <summary>
    /// Represents an exception that occurs when an internal server error is encountered.
    /// </summary>
    public sealed class InternalServerErrorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InternalServerErrorException"/> class.
        /// </summary>
        public InternalServerErrorException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalServerErrorException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InternalServerErrorException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalServerErrorException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public InternalServerErrorException(string message, Exception? innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalServerErrorException"/> class with a specified error message and arguments to format the message.
        /// </summary>
        /// <param name="message">The composite format string that describes the error.</param>
        /// <param name="args">An array of objects to format the message.</param>
        public InternalServerErrorException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
