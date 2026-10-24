using DI.Chakra.DBClient;
using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Logger;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class TablePropertiesRepository
    {
        public TablePropertiesRepository(DataAccessTransaction transaction) { }
        QueryExecutor executor;
        IACMDbOperator iacmOperator;

        public IList<TableProperties> GetByTableName(string TableName)
        {
            IACMDBContext dbcontext = new IACMDBContext();
            ObjectContext ctx = ((IObjectContextAdapter)dbcontext).ObjectContext;

            //var test = from meta in ctx.MetadataWorkspace.GetItems(DataSpace.CSpace)
            //           .Where(m => m.BuiltInTypeKind == BuiltInTypeKind.EntityType)
            //           from p in (meta as EntityType).Properties select meta;

            var cols = from meta in ctx.MetadataWorkspace.GetItems(DataSpace.CSpace)
                       .Where(m => m.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                       from p in (meta as EntityType).Properties
                          .Where(p => p.DeclaringType.Name == TableName)
                       select new TableProperties
                       {
                           ColumnName = p.Name,
                           DataType = p.TypeUsage.EdmType.Name,
                       };
            return cols.ToList();
        }
        public IList<string> GetTableList()
        {
            IList<string> tableNameList = new List<string>();
            IACMDBContext dbcontext = new IACMDBContext();
            ObjectContext ctx = ((IObjectContextAdapter)dbcontext).ObjectContext;
            var metadata = ((IObjectContextAdapter)dbcontext).ObjectContext.MetadataWorkspace;

            EntityContainer container = metadata.GetEntityContainer(ctx.DefaultContainerName, DataSpace.CSpace);
            List<string> result = (from meta in container.BaseEntitySets
                                   where meta.BuiltInTypeKind == BuiltInTypeKind.EntitySet
                                   select meta.ElementType.Name.ToString()).ToList<string>();
            return result;
        }
        public int GetMaxId(string tableName, string columnName)
        {

            SqlParameter sqlParam = null;

            try
            {
                string query = "DI_spGetMaxId";

                List<SqlParameter> parameters = new List<SqlParameter>();



                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@TableName";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 100;
                sqlParam.Value = tableName;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@ColumnName";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 100;
                sqlParam.Value = columnName;
                parameters.Add(sqlParam);



                iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);
                executor = new QueryExecutor(iacmOperator);
                object maxId = executor.ExecuteQueryObj();

                if (maxId != null)
                {
                    return CheckNull.AgainstInt(maxId);
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while GetMaxId", ex);
                throw ex;
            }
            finally
            {
                executor.IACMDbOperator.Close();
            }
            return -1;
        }
    }
}
