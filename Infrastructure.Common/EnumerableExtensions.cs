using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Adds the range of items to the provided collection.
        /// </summary>
        /// <typeparam name="T">Type of the collection.</typeparam>
        /// <param name="collection">The collection to add the items to.</param>
        /// <param name="items">The items to add.</param>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        /// <summary>
        /// Removes the range of items from the provided collection.
        /// </summary>
        /// <typeparam name="T">Type of the collection.</typeparam>
        /// <param name="collection">The collection to remove the items from.</param>
        /// <param name="items">The items to remove.</param>
        public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                collection.Remove(item);
            }
        }
    }
}
