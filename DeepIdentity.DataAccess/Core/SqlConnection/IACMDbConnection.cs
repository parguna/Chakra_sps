using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using DeepIdentity.Infrastructure;

namespace DeepIdentity.DataAccess
{
    public class IACMDbOperator
    {
         private string m_ConStr = string.Empty;

         private ConnectionType m_connectionType = ConnectionType.Production;


        

        public SqlConnection Connection { get; set; }

        public SqlCommand  Command { get; set; }

        public IACMDbOperator()
        {
            try
            {
                string connection = ConfigurationManager.AppSettings["Connection"];

                m_connectionType = (ConnectionType)Enum.Parse(typeof(ConnectionType), connection);

                Connection = GetConnection();
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.FATAL, "Not able to get Connection from Web.config", ex);

                throw;
            }
           

            
        }

        public IACMDbOperator(string _query, CommandType _cmdType)
            : this(_query, _cmdType, null)
        {
            
        }
        public IACMDbOperator(string _query, CommandType _cmdType, List<SqlParameter> sqlParams)
        {
            try
            {
                string connection = ConfigurationManager.AppSettings["Connection"];

                m_connectionType = (ConnectionType)Enum.Parse(typeof(ConnectionType), connection);

                Connection = GetConnection();

                Command = GetCommand(Connection, _query, _cmdType, sqlParams);
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.FATAL, "Not able to get Connection from Web.config", ex);

                throw;
            }
        }
       
         public SqlConnection GetConnection()
         {
             SqlConnection con = null;

             
             try
             {
                 if (m_connectionType == ConnectionType.Development)
                 {
                     m_ConStr = ConfigurationManager.ConnectionStrings["Development"].ToString();
                 }
                 else if (m_connectionType == ConnectionType.DIServer)
                 {
                     m_ConStr = ConfigurationManager.ConnectionStrings["DIServer"].ToString();
                 }
                 else if (m_connectionType == ConnectionType.Integration)
                 {
                     m_ConStr = ConfigurationManager.ConnectionStrings["Integration"].ToString();
                 }
                 else if (m_connectionType == ConnectionType.Local)
                 {
                     m_ConStr = ConfigurationManager.ConnectionStrings["Local"].ToString();
                 }
                 else if (m_connectionType == ConnectionType.Production)
                 {
                     m_ConStr = ConfigurationManager.ConnectionStrings["Production"].ToString();
                 }
                 else if (m_connectionType == ConnectionType.UAT)
                 {
                     m_ConStr = ConfigurationManager.ConnectionStrings["UAT"].ToString();
                 }
                 con = new SqlConnection(m_ConStr);
                 
                 con.Open();

                 return con;
             }
             catch (Exception ex)
             {
                 Log.General.WriteMessage(LogLevel.FATAL, string.Format("Connection {0} can not be initialized", m_ConStr));

                 if (null != con)
                 {
                     con.Close();
                     con.Dispose();
                 }
                 throw;
             }
             finally
             {
                 
             }
         }

         public SqlCommand GetCommand(string _query, CommandType _cmdType, List<SqlParameter> sqlParams)
         {
             if (null == Connection)
                 return null;

             if (null == _query)
             {
                 throw new ArgumentNullException("SQLCommand object can not be created without Query");
             }


             SqlCommand cmd = null;

             try
             {
                 cmd = new SqlCommand();
                 cmd.Connection = Connection;

                 
                 cmd.CommandText = _query;
                 cmd.CommandType = _cmdType;
                 cmd.CommandTimeout = 1800;
                 if (null != sqlParams)
                     cmd.Parameters.AddRange(sqlParams.ToArray<SqlParameter>());

                 Command = cmd;

                 return cmd;
             }
             catch (Exception ex)
             {

                 throw;
             }
         }
         public SqlCommand GetCommand(SqlConnection connection, string _query, CommandType _cmdType, List<SqlParameter> sqlParams)
         {
             if (null == connection)
                 return null;
             
             if (null == _query)
             {
                 throw new ArgumentNullException("SQLCommand object can not be created without Query");
             }


             SqlCommand cmd = null;

             try
             {
                 cmd = new SqlCommand();
                 cmd.Connection = connection;
                 cmd.CommandText = _query;
                 cmd.CommandType = _cmdType;
                 cmd.CommandTimeout = 1800;
                 if (null != sqlParams)
                     cmd.Parameters.AddRange(sqlParams.ToArray<SqlParameter>());

                 return cmd;
             }
             catch (Exception ex)
             {
                 
                 throw;
             }
         }
         public void Close()
         {
             if(null != Command)
                 Command.Dispose();

             if (null != Connection)
             {
                 Connection.Close();
                 Connection.Dispose();
             }
         }  

    }
}
