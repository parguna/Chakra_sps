using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using DeepIdentity.Infrastructure;

namespace DeepIdentity.DataAccess
{
    public class GeneralDA
    {

        QueryExecutor executor;

        IACMDbOperator iacmOperator;


        public int GetMaxId(string _tableName, string _columnName)
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
                sqlParam.Value = _tableName;
                parameters.Add(sqlParam);


                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@ColumnName";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 100;
                sqlParam.Value = _columnName;
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
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while GetMaxId", ex);

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
