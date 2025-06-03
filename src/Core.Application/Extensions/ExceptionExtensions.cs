namespace Core.Application.Extensions
{
    /// <summary>
    /// Provides extension methods for working with exceptions.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Retrieves all messages from an exception and its inner exceptions.
        /// </summary>
        /// <param name="exception">The exception to extract messages from.</param>
        /// <returns>A single string containing all exception messages, separated by a period.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="exception"/> is null.</exception>
        public static string GetAllMessages(this Exception exception)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));

            var messages = new List<string>();
            var current = exception;

            while (current != null)
            {
                var message = current.Message?.Trim(' ', '.');
                if (!string.IsNullOrWhiteSpace(message) && !messages.Contains(message))
                {
                    messages.Add(message);
                }
                current = current.InnerException;
            }

            return string.Join(". ", messages);
        }
    }
}
