using AutoMapper;
using DI.Chakra.Infrastructure;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public abstract class DomainModelBase<TModel, TEntity> : IdBase
        where TModel : DomainModelBase<TModel, TEntity>
        where TEntity : EntityModel.EntityBase
    {
        public bool IsNew
        {
            get { return (Id <= 0); }
        }

        #region GetById

        public static TModel GetById(long id)
        {
            return GetById(id, false);
        }
       
        public static TModel GetById(long id, bool eagerLoadAllProperties)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<TEntity>(dataAccessTransaction).GetById(id, eagerLoadAllProperties));
            }
        }

        #endregion

        #region GetAll

        protected PagedResults<TModel> CreatePagedResults(
            IQueryable<TModel> queryable,
            int page,
            int pageSize,
            string orderBy,
            bool ascending)
        {
            var skipAmount = pageSize * (page - 1);

            var projection = queryable
                .OrderByPropertyOrField(orderBy, ascending)
                .Skip(skipAmount)
                .Take(pageSize);

            var totalNumberOfRecords = queryable.Count<TModel>();
            var results = projection.ToList();

            var mod = totalNumberOfRecords % pageSize;
            var totalPageCount = (totalNumberOfRecords / pageSize) + (mod == 0 ? 0 : 1);



            return new PagedResults<TModel>
            {
                Results = results,
                PageNumber = page,
                PageSize = results.Count,
                TotalNumberOfPages = totalPageCount,
                TotalNumberOfRecords = totalNumberOfRecords
            };
        }

        public static List<TModel> GetAll()
        {
            return GetAll(false);
        }

        public static PagedResults<TModel> GetAllPaging(int? pageSize, int? pageNumber)
        {
            var lst = GetAll(false);

            var aa = lst.Skip(pageNumber.Value * pageSize.Value).Take(pageSize.Value).ToList();

            PagedResults<TModel> paged = new PagedResults<TModel>()
            {
                Results = aa,
                TotalNumberOfRecords = lst.Count,
                PageNumber = pageNumber.Value,
                PageSize = pageSize.Value
            };

            return paged;
        }

        public static List<TModel> GetAll(int pageSize, int pageNumber)
        {
            return GetAll(false, pageSize, pageNumber);
        }

        public static List<TModel> GetAll(bool eagerLoadAllProperties)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<TEntity>(dataAccessTransaction).GetAll(eagerLoadAllProperties).ToList());
            }
        }

        public static List<TModel> GetAll(bool eagerLoadAllProperties, int pageSize, int pageNumber)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<TEntity>(dataAccessTransaction).GetAll(eagerLoadAllProperties, pageSize, pageNumber).ToList());
            }
        }

        #endregion

        #region Get

        public static List<TModel> Get(Expression<Func<TModel, bool>> filter, bool eagerLoadAllProperties, int pageSize, int pageNumber)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<TEntity>(dataAccessTransaction).Get(ModelToEntityFilterVisitor.Convert(filter), eagerLoadAllProperties, pageSize, pageNumber).ToList());
            }
        }

        /// <summary>
        /// Rewrites a filter on TModel into the equivalent filter on TEntity by matching property names.
        /// AutoMapper 4.x did this via Mapper.Map; the upgraded AutoMapper no longer maps expressions.
        /// </summary>
        private class ModelToEntityFilterVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression modelParameter;
            private readonly ParameterExpression entityParameter;

            private ModelToEntityFilterVisitor(ParameterExpression modelParameter)
            {
                this.modelParameter = modelParameter;
                entityParameter = Expression.Parameter(typeof(TEntity), modelParameter.Name);
            }

            public static Expression<Func<TEntity, bool>> Convert(Expression<Func<TModel, bool>> filter)
            {
                var visitor = new ModelToEntityFilterVisitor(filter.Parameters[0]);
                return Expression.Lambda<Func<TEntity, bool>>(visitor.Visit(filter.Body), visitor.entityParameter);
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == modelParameter ? entityParameter : base.VisitParameter(node);
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Expression == modelParameter)
                {
                    var entityProperty = typeof(TEntity).GetProperty(node.Member.Name);
                    if (entityProperty == null)
                        throw new NotSupportedException(string.Format("Property '{0}' on {1} has no matching property on {2}.", node.Member.Name, typeof(TModel).Name, typeof(TEntity).Name));
                    return Expression.Property(entityParameter, entityProperty);
                }
                return base.VisitMember(node);
            }
        }
        #endregion

        public static bool ExistsById(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<TEntity>(dataAccessTransaction).Count(entity => entity.Id == id) > 0;
            }
        }

        #region Save, Add, Update, Delete

        //public virtual void Save()
        //{
        //    using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        Save(dataAccessTransaction);
        //        dataAccessTransaction.Commit();
        //    }
        //}

        public virtual long Save(string currentUser)
        {

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                var id = Save(dataAccessTransaction);
                dataAccessTransaction.Commit();
                return id;
            }
        }

        public virtual long Save(DataAccessTransaction dataAccessTransaction)
        {
            if (IsNew)
                this.Add(dataAccessTransaction);
            else
                this.Update(dataAccessTransaction);
            return this.Id;
        }

        private void Add()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                this.Add(dataAccessTransaction);
                dataAccessTransaction.Commit();
            }
        }

        private void Add(DataAccessTransaction dataAccessTransaction)
        {
            this.Id = new RepositoryBase<TEntity>(dataAccessTransaction).Insert(ORUnMap());
        }

        private void Update()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                this.Update(dataAccessTransaction);
                dataAccessTransaction.Commit();
            }
        }

        private void Update(DataAccessTransaction dataAccessTransaction)
        {
            new RepositoryBase<TEntity>(dataAccessTransaction).Update(ORUnMap());
        }

        public virtual void Delete()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                this.Delete(dataAccessTransaction);
                dataAccessTransaction.Commit();
            }
        }

        public virtual void Delete(DataAccessTransaction dataAccessTransaction)
        {
            new RepositoryBase<TEntity>(dataAccessTransaction).Delete(ORUnMap());
        }

        #endregion

        #region List Save and delete

        public static void DeleteAll()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                DeleteAll(dataAccessTransaction);
                dataAccessTransaction.Commit();
            }
        }

        public static void DeleteAll(DataAccessTransaction dataAccessTransaction)
        {
            new RepositoryBase<TEntity>(dataAccessTransaction).DeleteAll();
        }

        public static void SaveList(DataAccessTransaction dataAccessTransaction, List<TModel> domainModelList)
        {
            foreach (TModel domainModel in domainModelList)
            {
                domainModel.Save(dataAccessTransaction);
            }
        }

        //public static void SaveList(List<TModel> domainModelList)
        //{
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        SaveList(dataAccessTransaction, domainModelList);

        //        dataAccessTransaction.Commit();
        //    }
        //}

        public static void SaveList(List<TModel> domainModelList, string currentUser)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                SaveList(dataAccessTransaction, domainModelList);

                dataAccessTransaction.Commit();
            }
        }

        public static void DeleteList(DataAccessTransaction dataAccessTransaction, List<TModel> domainModelList)
        {
            foreach (TModel domainModel in domainModelList)
            {
                domainModel.Delete(dataAccessTransaction);
            }
        }

        public static void DeleteList(List<TModel> domainModelList)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                DeleteList(dataAccessTransaction, domainModelList);

                dataAccessTransaction.Commit();
            }
        }

        public static void DeleteList(DataAccessTransaction dataAccessTransaction, List<long> idList)
        {
            foreach (long id in idList)
            {
                Delete(dataAccessTransaction, id);
            }
        }

        public static void DeleteList(List<long> idList)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                DeleteList(dataAccessTransaction, idList);

                dataAccessTransaction.Commit();
            }
        }

        public static void Delete(long id)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                Delete(dataAccessTransaction, id);
                dataAccessTransaction.Commit();
            }
        }

        public static void Delete(DataAccessTransaction dataAccessTransaction, long id)
        {
            new RepositoryBase<TEntity>(dataAccessTransaction).Delete(id);
        }

        #endregion 

        #region ORMap & ORUnMap

        public static TModel ORMap(TEntity tEntity)
        {
            if (tEntity == null)
                return default(TModel);

            return Mapper.Map<TEntity, TModel>(tEntity);
        }

        public static List<TModel> ORMapList(IList<TEntity> tEntities)
        {
            return tEntities.Select(ORMap).ToList();
        }

        public virtual TEntity ORUnMap()
        {
            return Mapper.Map<DomainModelBase<TModel, TEntity>, TEntity>(this);
        }

        public static PagedResults<TModel> ORMapListPager(PagedResults<TEntity> tEntities)
        {
            var results = tEntities.Results;
            var mappedResults = ORMapList(results);

            return new PagedResults<TModel>()
            {
                PageNumber = tEntities.PageNumber,
                PageSize = tEntities.PageSize,
                TotalNumberOfPages = tEntities.TotalNumberOfPages,
                TotalNumberOfRecords = tEntities.TotalNumberOfRecords,
                Results = mappedResults
            };
        }

        #endregion
        public static bool ExecuteQuery(string Query)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                try
                {
                    new RepositoryBase<TEntity>(dataAccessTransaction).ExecuteQuery(Query);

                }
                catch (Exception ex)
                {
                    return false;
                }

                dataAccessTransaction.Commit();
                return true;
            }
        }

        public static bool ExecuteQuery(string Query,params object[] parameter)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                try
                {
                    new RepositoryBase<TEntity>(dataAccessTransaction).ExecuteQuery(Query,parameter);

                }
                catch (Exception ex)
                {
                    return false;
                }

                dataAccessTransaction.Commit();
                return true;
            }
        }

        public static int ExecuteQueryReturnValue(string Query)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                try
                {
                    return new RepositoryBase<TEntity>(dataAccessTransaction).ExecuteQueryWithReturnValue(Query);
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }
        public static PagedResults<TModel> GetAllPaging(bool eagerLoadAllProperties, int pageSize, int pageNumber)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapListPager(new RepositoryBase<TEntity>(dataAccessTransaction).GetAllPagedResults(eagerLoadAllProperties, pageSize, pageNumber));
            }
        }


        public static bool ExecuteQuery(List<string> Querylist)
        {

            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                foreach (string Query in Querylist)
                {
                    try
                    {
                        new RepositoryBase<TEntity>(dataAccessTransaction).ExecuteQuery(Query);
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }

                }
                dataAccessTransaction.Commit();
                return true;
            }

        }

        // Parameterized batch overload: each entry's query uses EF "{0}", "{1}" ... tokens
        // which Database.ExecuteSqlCommand binds as real SqlParameters (not string-substituted),
        // so tainted string values (e.g. a user name) can be embedded safely without escaping.
        public static bool ExecuteQuery(List<Tuple<string, object[]>> QueryList)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                foreach (var query in QueryList)
                {
                    try
                    {
                        new RepositoryBase<TEntity>(dataAccessTransaction).ExecuteQuery(query.Item1, query.Item2);
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }

                }
                dataAccessTransaction.Commit();
                return true;
            }

        }

        public static DataTable GetDataTableByQuery(string Query)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return new RepositoryBase<TEntity>(tran).GetDataTableByQuery(Query);
            }
        }
        public static void ExecuteQueryWithCommandTimeout(string Query, int commandTimeout)
        {
            try
            {
                using(var tran = new DataAccessTransaction(false))
                {
                    new RepositoryBase<TEntity>(tran).ExecuteQueryWithCommandTimeOut(Query, commandTimeout);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
