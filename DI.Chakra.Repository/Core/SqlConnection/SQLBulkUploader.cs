using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Repository
{
    public class SQLBulkUploader<T>
    {
        private SqlConnection m_connection = null;

        private DataTable dataTable;

        //public List<T> DataSource { get; set; }

        public string TableName { get; set; }


        public SQLBulkUploader(string tableName)
        {
            //DataSource = new List<T>();

            TableName = tableName;


        }

        public void Flush(IList<T> list)
        {

            dataTable = CollectionHelper.ConvertToNormalTable(list);
            WriteToDatabase();


        }
        public void Flush(DataTable dt)
        {

            dataTable = dt;
            WriteToDatabase();


        }
        private void WriteToDatabase()
        {



            IACMDbOperator dbOperator = null;

            SqlBulkCopy bulkCopy = null;

            try
            {

                dbOperator = new IACMDbOperator();

                //m_connection = dbOperator.GetConnection();


                bulkCopy = new SqlBulkCopy
                    (
                    dbOperator.Connection,
                    SqlBulkCopyOptions.TableLock |
                    SqlBulkCopyOptions.FireTriggers |
                    SqlBulkCopyOptions.UseInternalTransaction,
                    null
                    );

                bulkCopy.BatchSize = 5000;

                bulkCopy.DestinationTableName = TableName;
                //column mapping should be done

                foreach (DataColumn dcName in dataTable.Columns)
                {
                    if (dcName.ColumnName.ToLower() != "isnew")
                    {
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(dcName.ColumnName, dcName.ColumnName));
                    }
                }
                bulkCopy.WriteToServer(dataTable);


            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                dbOperator.Close();
            }

            dataTable.Clear();

        }

        public void Close()
        {
            if (null != m_connection)
            {
                m_connection.Close();
                m_connection.Dispose();
            }
        }
    }
}
