using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Exceptions.Extensions
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

            var messages = exception.FromHierarchy(ex => ex.InnerException)
                .Select(ex => ex.Message?.Trim(new char[] { ' ', '.' }) ?? string.Empty);

            return string.Join(". ", messages.Where(msg => !string.IsNullOrWhiteSpace(msg)));
        }

        /// <summary>
        /// Traverses a hierarchy of objects starting from a source object.
        /// </summary>
        /// <typeparam name="TSource">The type of the objects in the hierarchy.</typeparam>
        /// <param name="source">The starting object in the hierarchy.</param>
        /// <param name="nextItem">A function to retrieve the next object in the hierarchy.</param>
        /// <param name="canContinue">A function to determine whether traversal should continue.</param>
        /// <returns>An enumerable of objects in the hierarchy.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="source"/>, <paramref name="nextItem"/>, or <paramref name="canContinue"/> is null.
        /// </exception>
        private static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Func<TSource, TSource> nextItem,
            Func<TSource, bool> canContinue)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (nextItem == null) throw new ArgumentNullException(nameof(nextItem));
            if (canContinue == null) throw new ArgumentNullException(nameof(canContinue));

            for (var current = source; canContinue(current); current = nextItem(current))
            {
                yield return current;
            }
        }

        /// <summary>
        /// Traverses a hierarchy of objects starting from a source object.
        /// </summary>
        /// <typeparam name="TSource">The type of the objects in the hierarchy.</typeparam>
        /// <param name="source">The starting object in the hierarchy.</param>
        /// <param name="nextItem">A function to retrieve the next object in the hierarchy.</param>
        /// <returns>An enumerable of objects in the hierarchy.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="source"/> or <paramref name="nextItem"/> is null.
        /// </exception>
        private static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Func<TSource, TSource> nextItem)
            where TSource : class
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (nextItem == null) throw new ArgumentNullException(nameof(nextItem));

            return source.FromHierarchy(nextItem, s => s != null);
        }
    }
}
