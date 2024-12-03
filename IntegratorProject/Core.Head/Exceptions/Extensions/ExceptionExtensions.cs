namespace Core.Head.Exceptions.Extensions
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Get all messages in exception
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string GetAllMessages(this Exception exception)
        {
            var messages = exception.FromHierarchy(ex => ex.InnerException)
                .Select(ex => ex.Message.Trim([' ', '.']));

            return string.Join(". ", messages);
        }

        private static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Func<TSource, TSource> nextItem,
            Func<TSource, bool> canContinue)
        {
            for (var current = source; canContinue(current); current = nextItem(current))
            {
                yield return current;
            }
        }

        private static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Func<TSource, TSource> nextItem)
            where TSource : class
            => source.FromHierarchy(nextItem, s => s != null);
    }
}
