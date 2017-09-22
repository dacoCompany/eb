using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Common.DB
{
    /// <summary>
    /// Implementation of repository pattern
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Infrastructure.Common.DB.IRepository{T}" />
    public class Repository<T> : IRepository<T> where T : class, IEntity
    {
        #region Private members

        private DbSet<T> dataEntity;
        private DbContext dataContext;

        #endregion

        #region Properties

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public Repository(DbContext context)
        {
            this.dataContext = context;
            this.dataEntity = context.Set<T>();

        }

        #endregion

        #region Interface methods

        /// <summary>
        /// Finds all (active and inactive entities)
        /// </summary>
        /// <returns>Collection of entites</returns>
        public IQueryable<T> FindAll()
        {
            return dataEntity;
        }

        /// <summary>
        /// Finds the active entity by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Active entity</returns>
        public T FindById(int id)
        {
            return dataEntity.Where(entity => entity.Id == id).FirstOrDefault(entity => entity.IsActive == true);
        }
        
        /// <summary>
        /// Saves the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>Saved instance</returns>
        public T Save(T instance)
        {
            long identity = instance.Id;
            if (identity == 0)
            {
                dataEntity.Add(instance);
                return instance;
            }

            dataContext.Entry(instance).State = EntityState.Modified;

            return instance;
        }

        /// <summary>
        /// Saves all instances.
        /// </summary>
        /// <param name="instances">The instances.</param>
        /// <returns>Saved instances</returns>
        public IEnumerable<T> SaveAll(IEnumerable<T> instances)
        {
            foreach (T instance in instances)
            {
                Save(instance);
            }

            return instances;
        }

        /// <summary>
        /// Finds the active entites by predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>Collection of active entities</returns>
        public virtual IQueryable<T> FindWhere(Expression<Func<T, bool>> predicate)
        {
            return dataEntity.Where(predicate).Where(entity => entity.IsActive == true);
        }

        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <param name="obj">The entity object.</param>
        /// <returns>Added entity</returns>
        public T Add(T obj)
        {
            return dataEntity.Add(obj);
        }

        /// <summary>
        /// Adds the specified entities.
        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return dataEntity.FirstOrDefault(predicate);
        }

        /// </summary>
        /// <param name="objCollection">The object collection.</param>
        /// <returns>Collection of added entites</returns>
        public IEnumerable<T> AddRange(IEnumerable<T> objCollection)
        {
            return dataEntity.AddRange(objCollection);
        }

        /// <summary>
        /// Sets specified entity to inactive
        /// </summary>
        /// <param name="obj">The entity object.</param>
        public void Delete(T obj)
        {
            obj.IsActive = false;
        }

        /// <summary>
        /// Any active result.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public bool AnyActive(Expression<Func<T, bool>> predicate)
        {
            return dataEntity.Where(predicate).Any(de => de.IsActive == true);
        }

        #endregion
    }
}
