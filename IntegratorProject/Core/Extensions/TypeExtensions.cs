using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Core.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Get concrete types
        /// </summary>
        /// <param name="type"></param>
        /// <param name="filter"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
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
