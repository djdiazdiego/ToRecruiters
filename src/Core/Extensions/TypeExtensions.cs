using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Core.Extensions
{
    /// <summary>
    /// Provides extension methods for working with <see cref="Type"/>.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Retrieves all concrete types that are assignable to the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The base <see cref="Type"/> to find assignable concrete types for.</param>
        /// <param name="filter">An optional filter expression to further refine the types.</param>
        /// <param name="assemblies">
        /// An optional array of <see cref="Assembly"/> objects to search. 
        /// If not provided, the assembly of the specified <paramref name="type"/> is used.
        /// </param>
        /// <returns>An array of <see cref="Type"/> objects representing the concrete types found.</returns>
        /// <remarks>
        /// A concrete type is defined as a non-abstract, non-interface class that is assignable to the specified <paramref name="type"/>.
        /// </remarks>
        public static Type[] GetConcreteTypes(
            this Type type,
            Expression<Func<Type, bool>>? filter = null,
            Assembly[]? assemblies = null)
        {
            bool predicate(Type t) => t.IsClass && !t.IsAbstract && type.IsAssignableFrom(t);

            var query = assemblies != null ?
                assemblies.SelectMany(x => x.GetTypes().Where(predicate)).AsQueryable() :
                type.Assembly.GetTypes().Where(predicate).AsQueryable();

            if (filter != null)
                query = query.Where(filter);

            return query.ToArray();
        }
    }
}
