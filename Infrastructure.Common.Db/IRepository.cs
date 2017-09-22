using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Common.DB
{
    /// <summary>
    /// Interface of repository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Finds the where.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        IQueryable<T> FindWhere(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Finds all.
        /// </summary>
        /// <returns></returns>
        IQueryable<T> FindAll();

        /// <summary>
        /// Any active result.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        bool AnyActive(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Firsts the or default.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        T FirstOrDefault(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Finds the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        T FindById(int id);

        /// <summary>
        /// Saves the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>Saved instance</returns>
        T Save(T instance);

        /// <summary>
        /// Saves all instances.
        /// </summary>
        /// <param name="instances">The instances.</param>
        /// <returns>Saved instances</returns>
        IEnumerable<T> SaveAll(IEnumerable<T> instances);

        /// <summary>
        /// Adds the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        T Add(T obj);

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="objCollection">The object collection.</param>
        /// <returns></returns>
        IEnumerable<T> AddRange(IEnumerable<T> objCollection);

        /// <summary>
        /// Deletes the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        void Delete(T obj);

    }
}
