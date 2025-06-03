using System;
using System.Globalization;

namespace Core.Application.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when an attempt is made to create an entity that already exists.
    /// </summary>
    public sealed class AlreadyExistsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlreadyExistsException"/> class.
        /// </summary>
        public AlreadyExistsException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlreadyExistsException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public AlreadyExistsException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlreadyExistsException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public AlreadyExistsException(string message, Exception? innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlreadyExistsException"/> class with a specified error message and arguments to format the message.
        /// </summary>
        /// <param name="message">The composite format string that describes the error.</param>
        /// <param name="args">An array of objects to format the message.</param>
        public AlreadyExistsException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
