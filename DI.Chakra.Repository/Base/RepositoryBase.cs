using DI.Chakra.DBClient;
using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic;
using System.Data.Entity;
using DI.Chakra.DBClient;
using DI.Chakra.Transaction;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Metadata.Edm;
using System.Reflection;
using EntityFramework.BulkInsert.Extensions;
using DI.Chakra.Logger;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Repository
{
    public class RepositoryBase<TEntity> where TEntity : EntityBase
    {
        private IACMDBContext context;
        private DbSet<TEntity> dbSet;
        //private Dictionary<string, List<long>> uniqueEntity = new Dictionary<string, List<long>>();
        private DataAccessTransaction currentTransaction;


        public RepositoryBase(DataAccessTransaction transaction)
        {
            currentTransaction = transaction;
            this.context = transaction.Context;
            this.dbSet = this.context.Set<TEntity>();
        }
        /// <summary>
        /// Do not use this. which yields memory lead and connection pool issue if we  are not disposing
        /// </summary>
        public RepositoryBase()
        {
            this.context = new IACMDBContext();
            this.dbSet = this.context.Set<TEntity>();
        }

        protected internal IACMDBContext CurrentContext
        {
            get
            {
                return context;
            }
        }

        protected internal DbSet<TEntity> CurrentEntity
        {
            get
            {
                return dbSet;
            }
        }

        #region Get

        #region GetById

        public virtual TEntity GetById(long id)
        {
            return GetById(id, false);
        }

        public virtual TEntity GetById(long id, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = dbSet;

            query = query.Where(entity => entity.Id == id);

            if (includeProperties != null)
            {
                query = includeProperties.Aggregate(query,
                          (current, include) => current.Include(include));
            }

            return query.FirstOrDefault();
        }

        public virtual List<TEntity> GetByIds(List<long> listOfIds, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = dbSet;

            query = query.Where(entity => listOfIds.Contains(entity.Id));
            
            if (includeProperties != null)
            {
                query = includeProperties.Aggregate(query,
                          (current, include) => current.Include(include));
            }

            return query.ToList();
        }

        public virtual TEntity GetById(long id, bool eagerLoadAllProperties)
        {
            IQueryable<TEntity> query = dbSet;

            query = query.Where(entity => entity.Id == id);

            if (eagerLoadAllProperties)
            {
                var workspace = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;
                var itemCollection = (ObjectItemCollection)(workspace.GetItemCollection(DataSpace.OSpace));
                var entityType = itemCollection.OfType<EntityType>().FirstOrDefault(e => itemCollection.GetClrType(e) == typeof(TEntity));

                if (entityType != null)
                {
                    foreach (var navigationProperty in entityType.NavigationProperties)
                    {
                        query = query.Include(navigationProperty.Name);
                    }
                }
            }

            return query.FirstOrDefault();
        }

        #endregion

        #region GetAll

        public virtual IEnumerable<TEntity> GetAll()
        {
            return GetEntity(filter: null, orderBy: null, pageSize: null, pageNumber: null, eagerLoadAllProperties: false);
        }

        public virtual IEnumerable<TEntity> GetAll(int pageSize, int pageNumber)
        {
            return GetEntity(filter: null, orderBy: null, pageSize: pageSize, pageNumber: pageNumber, eagerLoadAllProperties: false);
        }

        public virtual IEnumerable<TEntity> GetAll(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return GetEntity(filter: null, orderBy: null, pageSize: null, pageNumber: null, eagerLoadAllProperties: false, includeProperties: includeProperties);
        }

        public virtual IEnumerable<TEntity> GetAll(int pageSize, int pageNumber, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return GetEntity(filter: null, orderBy: null, pageSize: pageSize, pageNumber: pageNumber, eagerLoadAllProperties: false, includeProperties: includeProperties);
        }

        public virtual IEnumerable<TEntity> GetAll(bool eagerLoadAllProperties)
        {
            return GetEntity(filter: null, orderBy: null, pageSize: null, pageNumber: null, eagerLoadAllProperties: eagerLoadAllProperties);
        }

        public virtual IEnumerable<TEntity> GetAll(bool eagerLoadAllProperties, int pageSize, int pageNumber)
        {
            return GetEntity(filter: null, orderBy: null, pageSize: pageSize, pageNumber: pageNumber, eagerLoadAllProperties: eagerLoadAllProperties);
        }

        public virtual IEnumerable<TEntity> GetAll(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
        {
            return GetEntity(filter: null, orderBy: orderBy, pageSize: null, pageNumber: null, eagerLoadAllProperties: false);
        }

        public virtual IEnumerable<TEntity> GetAll(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, int pageSize, int pageNumber)
        {
            return GetEntity(filter: null, orderBy: orderBy, pageSize: pageSize, pageNumber: pageNumber, eagerLoadAllProperties: false);
        }

        public virtual IEnumerable<TEntity> GetAll(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return GetEntity(filter: null, orderBy: orderBy, pageSize: null, pageNumber: null, eagerLoadAllProperties: false, includeProperties: includeProperties);
        }

        public virtual IEnumerable<TEntity> GetAll(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, int pageSize, int pageNumber, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return GetEntity(filter: null, orderBy: orderBy, pageSize: pageSize, pageNumber: pageNumber, eagerLoadAllProperties: false, includeProperties: includeProperties);
        }

        public virtual IEnumerable<TEntity> GetAll(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, bool eagerLoadAllProperties)
        {
            return GetEntity(filter: null, orderBy: orderBy, pageSize: null, pageNumber: null, eagerLoadAllProperties: eagerLoadAllProperties);
        }

        public virtual IEnumerable<TEntity> GetAll(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, bool eagerLoadAllProperties, int pageSize, int pageNumber)
        {
            return GetEntity(filter: null, orderBy: orderBy, pageSize: pageSize, pageNumber: pageNumber, eagerLoadAllProperties: eagerLoadAllProperties);
        }

        #endregion

        #region Get with filter

        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter)
        {
            return GetEntity(filter: filter, orderBy: null, pageSize: null, pageNumber: null, eagerLoadAllProperties: false);
        }

        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter, int pageSize, int pageNumber)
        {
            return GetEntity(filter: filter, orderBy: null, pageSize: pageSize, pageNumber: pageNumber, eagerLoadAllProperties: false);
        }

        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return GetEntity(filter: filter, orderBy: null, pageSize: null, pageNumber: null, eagerLoadAllProperties: false, includeProperties: includeProperties);
        }

        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter, int pageSize, int pageNumber, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return GetEntity(filter: filter, orderBy: null, pageSize: pageSize, pageNumber: pageNumber, eagerLoadAllProperties: false, includeProperties: includeProperties);
        }

        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter, bool eagerLoadAllProperties)
        {
            return GetEntity(filter: filter, orderBy: null, pageSize: null, pageNumber: null, eagerLoadAllProperties: eagerLoadAllProperties);
        }

        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter, bool eagerLoadAllProperties, int pageSize, int pageNumber)
        {
            return GetEntity(filter: filter, orderBy: null, pageSize: pageSize, pageNumber: pageNumber, eagerLoadAllProperties: eagerLoadAllProperties);
        }

        public virtual IEnumerable<TEntity> Get(int pageSize, int pageNumber, Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
        {
            return GetEntity(filter: filter, orderBy: orderBy, pageSize: null, pageNumber: null, eagerLoadAllProperties: false);
        }

        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, int pageSize, int pageNumber)
        {
            return GetEntity(filter: filter, orderBy: orderBy, pageSize: pageSize, pageNumber: pageNumber, eagerLoadAllProperties: false);
        }

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return GetEntity(filter: filter, orderBy: orderBy, pageSize: null, pageNumber: null, eagerLoadAllProperties: false, includeProperties: includeProperties);
        }

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            int pageSize, int pageNumber,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return GetEntity(filter: filter, orderBy: orderBy, pageSize: pageSize, pageNumber: pageNumber, eagerLoadAllProperties: false, includeProperties: includeProperties);
        }

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            bool eagerLoadAllProperties)
        {
            return GetEntity(filter: filter, orderBy: orderBy, pageSize: null, pageNumber: null, eagerLoadAllProperties: eagerLoadAllProperties);
        }

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            int pageSize, int pageNumber,
            bool eagerLoadAllProperties)
        {
            return GetEntity(filter: filter, orderBy: orderBy, pageSize: pageSize, pageNumber: pageNumber, eagerLoadAllProperties: eagerLoadAllProperties);
        }



        #endregion

        #region private method

        private IEnumerable<TEntity> GetEntity(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            int? pageSize,
            int? pageNumber,
            bool eagerLoadAllProperties,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (eagerLoadAllProperties)
            {
                var workspace = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;
                var itemCollection = (ObjectItemCollection)(workspace.GetItemCollection(DataSpace.OSpace));
                var entityType = itemCollection.OfType<EntityType>().FirstOrDefault(e => itemCollection.GetClrType(e) == typeof(TEntity));

                if (entityType != null)
                {
                    foreach (var navigationProperty in entityType.NavigationProperties)
                    {
                        query = query.Include(navigationProperty.Name);
                    }
                }
            }
            else if (includeProperties != null)
            {
                query = includeProperties.Aggregate(query,
                          (current, include) => current.Include(include));
            }

            if (pageNumber.HasValue && pageSize.HasValue && pageNumber.Value>0 && pageSize.Value > 0)
            {
                if (orderBy != null)
                {
                    var qry = orderBy(query);
                    return qry.Skip(pageNumber.Value * pageSize.Value).Take(pageSize.Value);
                }
                else
                {
                    return query.OrderByPropertyOrField("Id").Skip(pageNumber.Value * pageSize.Value).Take(pageSize.Value);
                }
            }
            if (orderBy != null)
            {
                return orderBy(query);
            }
            else
            {
                return query;
            }
        }

        #endregion

        #endregion

        #region  Miscellaneous

        public virtual long Count()
        {
            return Count(null);
        }

        public virtual long Count(Expression<Func<TEntity, bool>> filter)
        {
            if (filter != null)
                return dbSet.Count(filter);
            else
                return dbSet.Count();
        }

        public virtual bool Any()
        {
            return Any(null);
        }

        public virtual bool Any(Expression<Func<TEntity, bool>> filter)
        {
            if (filter != null)
                return dbSet.Any(filter);
            else
                return dbSet.Any();
        }

        #endregion

        #region Insert Update Delete

        public virtual long Insert(TEntity entityToInsert)
        {
            #region old commented logic
            //uniqueEntity.Clear();
            //ProcessNavigationalProperties(entityToInsert);

            //if (currentTransaction.transactionDateTime == null)
            //    throw new ApplicationException("Transaction DateTime is null.");
            //else if (currentTransaction.User == null)
            //    throw new ApplicationException("Transaction User is null.");
            //else
            //{
            //    if (entityToInsert is ICreatable)
            //    {
            //        ((ICreatable)entityToInsert).CreatedDate = currentTransaction.transactionDateTime;
            //        ((ICreatable)entityToInsert).CreatedBy = currentTransaction.User;
            //    }

            //    if (entityToInsert is IUpdatable)
            //    {
            //        ((IUpdatable)entityToInsert).LastModifiedDate = null;
            //        ((IUpdatable)entityToInsert).LastModifiedBy = null;
            //    }
            //}
            #endregion

            // Detech All old entities is needed when working with transaction, when same entity could be there in muliple insert in same transaction
            DetachAllEntities();

            dbSet.Add(entityToInsert);
            AttachExistingEntity();
            context.SaveChanges();
            return entityToInsert.Id;
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            TEntity dbEntity = dbSet.Find(entityToUpdate.Id);

            if (dbEntity == null)
            {
                throw new ApplicationException("Entity does not exist for update");
            }

            if (entityToUpdate is IUpdatable)
            {
                if (currentTransaction.transactionDateTime == null)
                    throw new ApplicationException("Transaction DateTime is null.");
                else if (currentTransaction.User == null)
                    throw new ApplicationException("Transaction User is null.");
                else
                {

                    //To resolve Icreatable exception during update, following lines are added
                    ((ICreatable)entityToUpdate).CreatedDate = ((ICreatable)dbEntity).CreatedDate;
                    ((ICreatable)entityToUpdate).CreatedBy = ((ICreatable)dbEntity).CreatedBy;

                    ((IUpdatable)entityToUpdate).LastModifiedDate = currentTransaction.transactionDateTime;
                    ((IUpdatable)entityToUpdate).LastModifiedBy = currentTransaction.User;
                }
            }

            context.Entry(dbEntity).CurrentValues.SetValues(entityToUpdate);
            context.SaveChanges();
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);

            if (entityToDelete == null)
            {
                throw new ApplicationException("Entity does not exist for delete");
            }

            dbSet.Remove(entityToDelete);

            context.SaveChanges();
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            Delete(entityToDelete.Id);
        }

        public virtual void DeleteAll()
        {
            dbSet.RemoveRange(dbSet);

            context.SaveChanges();
        }



        /*
        private void ProcessNavigationalProperties(EntityBase entityBase)
        {
            Type type = entityBase.GetType();
            List<EntityBase> navigationPropertyList = new List<EntityBase>();

            //Get all navigational properties
            foreach (PropertyInfo propertyInfo in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                object value = propertyInfo.GetValue(entityBase);

                if (value != null && typeof(EntityBase).IsAssignableFrom(value.GetType()))
                    navigationPropertyList.Add((EntityBase)value);
                else if (value != null && typeof(IEnumerable<EntityBase>).IsAssignableFrom(value.GetType()))
                    navigationPropertyList.AddRange((IEnumerable<EntityBase>)value);
            }


            foreach (EntityBase navigationProperty in navigationPropertyList)
            {
                string navigationPropertyType = navigationProperty.GetType().ToString();

                //Attach existing
                if (navigationProperty.Id > 0)
                {
                    // Suresh : need to fix this 6/24/2016
                    try
                    {
                        //if (!
                        //    (uniqueEntity.ContainsKey(navigationPropertyType) && uniqueEntity[navigationPropertyType].Contains(navigationProperty.Id))
                        //   )
                        //{
                            this.CurrentContext.Entry(navigationProperty).State = EntityState.Unchanged;
                        //}
                        //this.CurrentContext.Set(navigationProperty.GetType()).Find(navigationProperty.Id);
                        //this.CurrentContext.Set(navigationProperty.GetType()).Attach(navigationProperty);
                    }
                    catch(Exception ex)
                    {
                        this.CurrentContext.Entry(navigationProperty).State = EntityState.Unchanged;
                    }
                }
                else
                {
                    if (navigationProperty is ICreatable)
                    {
                        ((ICreatable)navigationProperty).CreatedDate = currentTransaction.transactionDateTime;
                        ((ICreatable)navigationProperty).CreatedBy = currentTransaction.User;
                    }
                    if (navigationProperty is IUpdatable)
                    {
                        ((IUpdatable)navigationProperty).LastModifiedDate = null;
                        ((IUpdatable)navigationProperty).LastModifiedBy = null;
                    }
                }

                //Below logic is to avoid Referrence Looping
                bool recursive = false;
                if (uniqueEntity.ContainsKey(navigationPropertyType))
                {
                    if (!uniqueEntity[navigationPropertyType].Contains(navigationProperty.Id))
                    {
                        uniqueEntity[navigationPropertyType].Add(navigationProperty.Id);
                        recursive = true;
                    }
                }
                else
                {
                    uniqueEntity.Add(navigationPropertyType, new List<long>() { navigationProperty.Id });
                    recursive = true;
                }

                if (recursive)
                    ProcessNavigationalProperties(navigationProperty);
            }
        }
        */

        private void AttachExistingEntity()
        {
            foreach (var entry in this.CurrentContext.ChangeTracker.Entries<EntityBase>())
            {
                if (entry.State == EntityState.Added && entry.Entity.Id > 0)
                {
                    entry.State = EntityState.Unchanged;
                }
                else if (entry.State == EntityState.Added && entry.Entity.Id <= 0)
                {
                    if (entry.Entity is ICreatable)
                    {
                        ((ICreatable)entry.Entity).CreatedDate = currentTransaction.transactionDateTime;
                        ((ICreatable)entry.Entity).CreatedBy = currentTransaction.User;
                    }
                    if (entry.Entity is IUpdatable)
                    {
                        ((IUpdatable)entry.Entity).LastModifiedDate = null;
                        ((IUpdatable)entry.Entity).LastModifiedBy = null;
                    }
                }
            }
        }

        public void DetachAllEntities()
        {
            foreach (var entry in this.CurrentContext.ChangeTracker.Entries<EntityBase>())
            {
                entry.State = EntityState.Detached;
            }
        }
        #endregion
        public virtual void ExecuteQuery(string Query)
        {
            CurrentContext.Database.ExecuteSqlCommand(Query);
        }
        public virtual void ExecuteQuery(string Query,params object[] parameter)
        {
            CurrentContext.Database.ExecuteSqlCommand(Query,parameter);
        }
        public virtual int ExecuteQueryWithReturnValue(string Query)
        {
            return CurrentContext.Database.ExecuteSqlCommand(Query);
        }
        public System.Data.DataTable GetDataTableByQuery(string sqlQuery)
        {
            return GetDataTableByQuery(sqlQuery, null);
        }

        public System.Data.DataTable GetDataTableByQuery(string sqlQuery, params System.Data.SqlClient.SqlParameter[] parameters)
        {

            try
            {
                System.Data.DataTable ds = new System.Data.DataTable();
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(CurrentContext.Database.Connection.ConnectionString))
                {
                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sqlQuery, connection))
                    {
                        if (parameters != null && parameters.Length > 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                        adapter.SelectCommand.CommandType = System.Data.CommandType.Text;
                        adapter.Fill(ds);
                    }
                }
                return ds;
            }
            catch (Exception ex)
            {

                //DILog.WriteServiceLogToDB(new ServiceLog()
                //{
                //    ExceptionMessage = ex.Message,
                //    StackTrace = ex.StackTrace,
                //    InnerExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : ""
                //});
            }
            return new System.Data.DataTable();
        }

        public List<string> GetAllColumnsFromQuery(string sqlQuery)
        {
            return GetAllColumnsFromQuery(sqlQuery, null);
        }

        public List<string> GetAllColumnsFromQuery(string sqlQuery, params System.Data.SqlClient.SqlParameter[] parameters)
        {
            var lstColumns = new List<string>();

            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(CurrentContext.Database.Connection.ConnectionString))
                {
                    connection.Open();
                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sqlQuery, connection))
                    {
                        if (parameters != null && parameters.Length > 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader(System.Data.CommandBehavior.SchemaOnly);
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            lstColumns.Add(reader.GetName(i));
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return lstColumns;
        }
        public virtual void ExecuteQueryWithCommandTimeOut(string Query, int commandTimeout)
        {
            CurrentContext.Database.CommandTimeout = commandTimeout;
            CurrentContext.Database.ExecuteSqlCommand(Query);
        }
        // {0}/{1}/... are EF's own positional-parameter tokens (bound as real SqlParameters
        // by Database.ExecuteSqlCommand), not string.Format substitutions - callers building a
        // query from external/discovered data (e.g. an account name) must use this overload
        // instead of concatenating that data into Query directly.
        public virtual void ExecuteQueryWithCommandTimeOut(string Query, int commandTimeout, params object[] parameter)
        {
            CurrentContext.Database.CommandTimeout = commandTimeout;
            CurrentContext.Database.ExecuteSqlCommand(Query, parameter);
        }
        public virtual PagedResults<TEntity> GetAllPagedResults()
        {
            return GetEntityPagedResults(filter: null, orderBy: null, pageSize: null, pageNumber: null, eagerLoadAllProperties: false, filterExpression: null, sortExpression: null);
        }
       
        public virtual PagedResults<TEntity> GetAllPagedResults(int pageSize, int pageNumber)
        {
            return GetEntityPagedResults(filter: null, orderBy: null, pageSize: pageSize, pageNumber: pageNumber, eagerLoadAllProperties: false, filterExpression: null, sortExpression: null);
        }

        public virtual PagedResults<TEntity> GetAllPagedResults(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return GetEntityPagedResults(filter: null, orderBy: null, pageSize: null, pageNumber: null, eagerLoadAllProperties: false, filterExpression: null, sortExpression: null, includeProperties: includeProperties);
        }

        public virtual PagedResults<TEntity> GetAllPagedResults(int pageSize, int pageNumber, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return GetEntityPagedResults(filter: null, orderBy: null, pageSize: pageSize, pageNumber: pageNumber, eagerLoadAllProperties: false, filterExpression: null, sortExpression: null, includeProperties: includeProperties);
        }

        public virtual PagedResults<TEntity> GetAllPagedResults(bool eagerLoadAllProperties)
        {
            return GetEntityPagedResults(filter: null, orderBy: null, pageSize: null, pageNumber: null, eagerLoadAllProperties: eagerLoadAllProperties, sortExpression: null, filterExpression: null);
        }

        public virtual PagedResults<TEntity> GetAllPagedResults(bool eagerLoadAllProperties, int pageSize, int pageNumber)
        {
            return GetEntityPagedResults(filter: null, orderBy: null, pageSize: pageSize, pageNumber: pageNumber, eagerLoadAllProperties: eagerLoadAllProperties, filterExpression: null, sortExpression: null);
        }

        public virtual PagedResults<TEntity> GetAllPagedResults(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
        {
            return GetEntityPagedResults(filter: null, orderBy: orderBy, pageSize: null, pageNumber: null, eagerLoadAllProperties: false, filterExpression: null, sortExpression: null);
        }

        public virtual PagedResults<TEntity> GetAllPagedResults(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, int pageSize, int pageNumber)
        {
            return GetEntityPagedResults(filter: null, orderBy: orderBy, pageSize: pageSize, pageNumber: pageNumber, eagerLoadAllProperties: false, filterExpression: null, sortExpression: null);
        }

        public virtual PagedResults<TEntity> GetAllPagedResults(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return GetEntityPagedResults(filter: null, orderBy: orderBy, pageSize: null, pageNumber: null, eagerLoadAllProperties: false, filterExpression: null, sortExpression: null, includeProperties: includeProperties);
        }

        public virtual PagedResults<TEntity> GetAllPagedResults(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, int pageSize, int pageNumber, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return GetEntityPagedResults(filter: null, orderBy: orderBy, pageSize: pageSize, pageNumber: pageNumber, eagerLoadAllProperties: false, filterExpression: null, sortExpression: null, includeProperties: includeProperties);
        }

        public virtual PagedResults<TEntity> GetAllPagedResults(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, bool eagerLoadAllProperties)
        {
            return GetEntityPagedResults(filter: null, orderBy: orderBy, pageSize: null, pageNumber: null, eagerLoadAllProperties: eagerLoadAllProperties, filterExpression: null, sortExpression: null);
        }

        public virtual PagedResults<TEntity> GetAllPagedResults(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, bool eagerLoadAllProperties, int pageSize, int pageNumber)
        {
            return GetEntityPagedResults(filter: null, orderBy: orderBy, pageSize: pageSize, pageNumber: pageNumber, eagerLoadAllProperties: eagerLoadAllProperties, filterExpression: null, sortExpression: null);
        }
        private PagedResults<TEntity> GetEntityPagedResults(Expression<Func<TEntity, bool>> filter,
              Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
              int? pageSize,
              int? pageNumber,
              bool eagerLoadAllProperties, string filterExpression, string sortExpression,
              params Expression<Func<TEntity,
                object>>[] includeProperties)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (filterExpression != null)
                query = query.Where(filterExpression);

            if (eagerLoadAllProperties)
            {
                var workspace = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;
                var itemCollection = (ObjectItemCollection)(workspace.GetItemCollection(DataSpace.OSpace));
                var entityType = itemCollection.OfType<EntityType>().FirstOrDefault(e => itemCollection.GetClrType(e) == typeof(TEntity));

                if (entityType != null)
                {
                    foreach (var navigationProperty in entityType.NavigationProperties)
                    {
                        query = query.Include(navigationProperty.Name);
                    }
                }
            }
            else if (includeProperties != null)
            {
                query = includeProperties.Aggregate(query,
                          (current, include) => current.Include(include));
            }

            if (pageNumber.HasValue && pageSize.HasValue)
            {
                var skipAmount = pageNumber.Value * pageSize.Value;
                var totalNumberOfRecords = query.Count();
                var mod = totalNumberOfRecords % pageSize.Value;
                var totalPageCount = (totalNumberOfRecords / pageSize.Value) + (mod == 0 ? 0 : 1);

                if (orderBy != null)
                {
                    var qry = orderBy(query);


                    var projection = qry
            .Skip(skipAmount)
            .Take(pageSize.Value);

                    return new PagedResults<TEntity>
                    {
                        Results = projection.ToList(),
                        PageNumber = pageNumber.Value,
                        PageSize = projection.Count(),
                        TotalNumberOfPages = totalPageCount,
                        TotalNumberOfRecords = totalNumberOfRecords
                    };
                }

                else
                {
                    //var skipAmount = pageSize.Value * (pageNumber.Value - 1);
                    //var totalNumberOfRecords = query.Count();
                    //var mod = totalNumberOfRecords % pageSize.Value;
                    //var totalPageCount = (totalNumberOfRecords / pageSize.Value) + (mod == 0 ? 0 : 1);


                    var projection = query
                        .OrderByPropertyOrField("Id")
           .Skip(skipAmount)
           .Take(pageSize.Value);

                    return new PagedResults<TEntity>
                    {
                        Results = projection.ToList(),
                        PageNumber = pageNumber.Value,
                        PageSize = projection.Count(),
                        TotalNumberOfPages = totalPageCount,
                        TotalNumberOfRecords = totalNumberOfRecords
                    };

                }

            }
            return new PagedResults<TEntity>
            {
                Results = new List<TEntity>(),
                PageNumber = 0,
                PageSize = 0,
                TotalNumberOfPages = 0,
                TotalNumberOfRecords = 0
            };
        }
    }
}