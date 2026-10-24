using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class QueryExecutor
    {
        private string m_ConStr = string.Empty;

        public IACMDbOperator IACMDbOperator { get; set; }

        public QueryExecutor(IACMDbOperator _operator)
        {
            IACMDbOperator = _operator;


        }


        public object ExecuteQueryObj()
        {
            object obj = null;

            if (null == IACMDbOperator)
                throw new Exception("IACMDbOperator cannot be null.");

            try
            {
                obj = IACMDbOperator.Command.ExecuteScalar();

                return obj;
            }
            catch (Exception)
            {
                throw ;
            }

            finally
            {
                if(IACMDbOperator.Connection != null)
                {
                    if (IACMDbOperator.Connection.State == ConnectionState.Open)
                        IACMDbOperator.Connection.Close();

                    IACMDbOperator.Connection.Dispose();
                }
            }

        }

        public IDataReader ExecuteQueryDR()
        {
            if (null == IACMDbOperator)
                throw new Exception("IACMDbOperator cannot be null.");

            try
            {
                IDataReader reader = IACMDbOperator.Command.ExecuteReader();

                return reader;
            }
            catch (Exception)
            {
                if (null != IACMDbOperator.Connection)
                {
                    IACMDbOperator.Connection.Close();
                    IACMDbOperator.Connection.Dispose();
                }

                throw;
            }

        }

        public DataSet ExecuteQueryDS()
        {
            if (null == IACMDbOperator)
                throw new Exception("IACMDbOperator cannot be null.");

            try
            {

                DataSet ds = new DataSet();

                SqlDataAdapter da = new SqlDataAdapter(IACMDbOperator.Command);

                da.Fill(ds);

                return ds;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null != IACMDbOperator.Connection)
                {
                    IACMDbOperator.Connection.Close();
                    IACMDbOperator.Connection.Dispose();
                }
            }
        }
        public int ExecuteQuery()
        {
            if (null == IACMDbOperator)
                throw new Exception("IACMDbOperator cannot be null.");

            try
            {


                return IACMDbOperator.Command.ExecuteNonQuery();


            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null != IACMDbOperator.Connection)
                {
                    IACMDbOperator.Connection.Close();
                    IACMDbOperator.Connection.Dispose();
                }
            }
        }
    }
}
