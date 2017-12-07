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
    }
}
