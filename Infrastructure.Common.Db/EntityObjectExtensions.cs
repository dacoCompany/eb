using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Infrastructure.Common.DB
{
    public static class EntityObjectExtensions
    {
        /// <summary>Finds if true.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="condition">if set to <c>true</c> [condition].</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>IQueryable list of entities.</returns>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate) where T : IEntity
        {
            return condition ? query.Where(predicate) : query;
        }

        /// <summary>Wheres if.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="condition">if set to <c>true</c> [condition].</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>IEnumerable list of entities.</returns>
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool condition, Func<T, bool> predicate)
        {
            return condition ? source.Where(predicate) : source;
        }

        /// <summary>
        /// Return the first active item from the list.
        /// </summary>
        /// <typeparam name="T">Type of items</typeparam>
        /// <param name="set">List of items.</param>
        /// <returns>First item or null.</returns>
        public static T FirstActive<T>(this ICollection<T> set) where T : IEntity
        {
            return set.FirstOrDefault(s => s.IsActive);
        }

        /// <summary>
        /// Return the first active item from the list.
        /// </summary>
        /// <typeparam name="T">Type of items</typeparam>
        /// <param name="set">List of items.</param>
        /// <param name="predicate">Conditions to satisfy.</param>
        /// <returns>First item or null.</returns>
        public static T FirstActive<T>(this ICollection<T> set, Func<T, bool> predicate) where T : IEntity
        {
            return set.Where(predicate).FirstOrDefault(s => s.IsActive);
        }

        public static int CountActive<T>(this ICollection<T> set) where T : IEntity
        {
            return set.Count(s => s.IsActive);
        }

        /// <summary>
        /// Return only active items from the list.
        /// </summary>
        /// <typeparam name="T">Type of items</typeparam>
        /// <param name="set">List of items.</param>
        /// <param name="predicate">Conditions to satisfy.</param>
        /// <returns>Active items</returns>
        public static IEnumerable<T> WhereActive<T>(this ICollection<T> set, Func<T, bool> predicate) where T : IEntity
        {
            return set.Where(predicate).Where(s => s.IsActive);
        }

        /// <summary>
        /// Return only active items from the list.
        /// </summary>
        /// <typeparam name="T">Type of items</typeparam>
        /// <param name="set">List of items.</param>
        /// <returns>Active items</returns>
        public static IEnumerable<T> WhereActive<T>(this ICollection<T> set) where T : IEntity
        {
            return set.Where(s => s.IsActive);
        }

        /// <summary>
        /// Returns whether an active item exists in the list.
        /// </summary>
        /// <typeparam name="T">Type of items</typeparam>
        /// <param name="set">List of items.</param>
        /// <returns>True if exists, otherwise false.</returns>
        public static bool AnyActive<T>(this ICollection<T> set) where T : IEntity
        {
            return set.Any(s => s.IsActive);
        }
    }
}
