using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using DeepIdentity.Model;

using System.Data;
using System.ComponentModel;
using DeepIdentity.Infrastructure;
using DeepIdentity.Model.SAP;
using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Logger;
using System.Data.Entity;

namespace DeepIdentity.DataAccess
{
    public class EndpointObjectDA
    {

        IACMDBContext _context = null;

        private QueryExecutor m_executor;

        private IACMDbOperator m_iacmOperator;

        public string SelectCommand { get; set; }

        public EndPoint Endpoint { get; set; }

        public string InsertCommand { get; set; }

        public string UpdateCommand { get; set; }

        public string DeleteCommand { get; set; }

        public string UniqueColumn { get; set; }

        private DataTable getUserInfoTable<T>()
        {
            DataTable table = new DataTable();

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));

            foreach (PropertyDescriptor prop1 in properties)
            {
                if (prop1.PropertyType.Namespace == "System")
                {
                    table.Columns.Add(prop1.Name, Nullable.GetUnderlyingType(prop1.PropertyType) ?? prop1.PropertyType);
                }
                else
                {

                    Type entityType1 = prop1.PropertyType;
                    PropertyDescriptorCollection properties1 = TypeDescriptor.GetProperties(entityType1);

                    foreach (PropertyDescriptor prop11 in properties1)
                    {
                        table.Columns.Add(string.Format("{0}.{1}", prop1.Name, prop11.Name), Nullable.GetUnderlyingType(prop11.PropertyType) ?? prop11.PropertyType);
                    }

                }
            }

            DataColumn dcol = table.Columns["UserId"];
            dcol.AutoIncrement = true;
            dcol.AutoIncrementSeed = 1;
            dcol.AutoIncrementStep = 1;

            //dcol = table.Columns["DIUserId"];
            //dcol.AutoIncrement = true;
            //dcol.AutoIncrementSeed = 1;
            //dcol.AutoIncrementStep = 1;

            return table;

        }


        private List<SqlParameter> GetUserParams<T>()
        {
            SqlParameter sqlParam = null;
            List<SqlParameter> parameters = new List<SqlParameter>();


            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));

            foreach (PropertyDescriptor prop1 in properties)
            {
                if (prop1.PropertyType.Namespace == "System")
                {


                    if ((prop1.PropertyType == typeof(int)) || (prop1.PropertyType == typeof(int?)))
                    {
                        sqlParam = new SqlParameter();
                        sqlParam.ParameterName = string.Format("@{0}", prop1.Name);
                        sqlParam.SqlDbType = SqlDbType.Int;
                        sqlParam.SourceColumn = prop1.Name;
                        parameters.Add(sqlParam);
                    }
                    if ((prop1.PropertyType == typeof(long)) || (prop1.PropertyType == typeof(long?)))
                    {
                        sqlParam = new SqlParameter();
                        sqlParam.ParameterName = string.Format("@{0}", prop1.Name);
                        sqlParam.SqlDbType = SqlDbType.BigInt;
                        sqlParam.SourceColumn = prop1.Name;
                        parameters.Add(sqlParam);
                    }
                    else if ((prop1.PropertyType == typeof(double)) || (prop1.PropertyType == typeof(double?)))
                    {
                        sqlParam = new SqlParameter();
                        sqlParam.ParameterName = string.Format("@{0}", prop1.Name);
                        sqlParam.SqlDbType = SqlDbType.Float;
                        sqlParam.SourceColumn = prop1.Name;
                        parameters.Add(sqlParam);
                    }
                    else if (prop1.PropertyType == typeof(string))
                    {
                        sqlParam = new SqlParameter();
                        sqlParam.ParameterName = string.Format("@{0}", prop1.Name);
                        sqlParam.SqlDbType = SqlDbType.NVarChar;
                        sqlParam.Size = -1;
                        sqlParam.SourceColumn = prop1.Name;
                        parameters.Add(sqlParam);
                    }
                    else if ((prop1.PropertyType == typeof(DateTime)) || (prop1.PropertyType == typeof(DateTime?)))
                    {
                        sqlParam = new SqlParameter();
                        sqlParam.ParameterName = string.Format("@{0}", prop1.Name);
                        sqlParam.SqlDbType = SqlDbType.DateTime;
                        sqlParam.SourceColumn = prop1.Name;
                        parameters.Add(sqlParam);
                    }
                    else if ((prop1.PropertyType == typeof(bool)) || (prop1.PropertyType == typeof(bool?)))
                    {
                        sqlParam = new SqlParameter();
                        sqlParam.ParameterName = string.Format("@{0}", prop1.Name);
                        sqlParam.SqlDbType = SqlDbType.Bit;
                        sqlParam.SourceColumn = prop1.Name;
                        parameters.Add(sqlParam);
                    }


                }
            }


            return parameters;
        }

        private DateTime? getLastDiscoveredDate()
        {
            try
            {

                //SchedulerDA scheduler = new SchedulerDA();
                ////DiscoveryProcessHistory discovery = scheduler.GetLastDiscovery(this.Endpoint.EndpointSystemId);
                //List<DiscoveryProcessHistory> discovery = scheduler.GetLastDiscovery(this.Endpoint.EndpointSystemId).ToList();

                //if (discovery == null)
                //    return null;

                //return Convert.ToDateTime(discovery.ProcessEndAt);


                return null;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public void ExecuteQry(string cmd)
        {
            int noAffected = -1;
            try
            {

                Log.General.WriteMessage(LogLevel.INFO, string.Format("Query for Clrear mapping information {0}", cmd));


                m_iacmOperator = new IACMDbOperator(cmd, CommandType.Text, null);

                m_executor = new QueryExecutor(m_iacmOperator);

                noAffected = m_executor.ExecuteQuery();

                Log.General.WriteMessage(LogLevel.DEBUG, string.Format("Deleted {0} records", noAffected));
            }
            catch (Exception)
            {

                Log.General.WriteMessage(LogLevel.ERROR, "Error Occured While deleting the records from Unmapped users");
            }
            finally
            {
                m_iacmOperator.Close();
            }
        }

        public IList<UsersByEndPointGridData> GetUserByEndPoint(int EndPointSystemId, string EndPointSysType) // DataSet
        {
            //string query = "DI_spGetUsersByEndPoint";
            //DataSet user = null;
            try
            {
                /* DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                DataRow dr;
                ds.Tables.Add(dt);
                dt.Columns.Add("name"); dt.Columns.Add("UserId"); dt.Columns.Add("SpecialAccount");
                dt.Columns.Add("lastLogon"); dt.Columns.Add("mailId"); dt.Columns.Add("CreatedDate");
                dt.Columns.Add("LastModifiedDate"); dt.Columns.Add("DIUserId"); dt.Columns.Add("currentuser");
                dt.Columns.Add("FullName"); */

                IList<UsersByEndPointGridData> usersList = new List<UsersByEndPointGridData>();
                UsersByEndPointGridData _user;

                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (EndPointSysType == "Active Directory")
                    {
                        var list = oIACMDBContext.ADSUserInfos.Where(endP => endP.EndPointSystemId == EndPointSystemId).ToList().Join(oIACMDBContext.Users.ToList(),
                                   EndPnt => EndPnt.DIUserId, usr => usr.UserId,
                                    (EndPnt, usr) => new // UsersByEndPointGridData
                                    {
                                        name = EndPnt.sAMAccountName, EndPnt.UserId, EndPnt.lastLogon,
                                        mailId = EndPnt.mail, EndPnt.CreatedDate, EndPnt.LastModifiedDate, EndPnt.DIUserId,
                                        currentuser = usr.UserName,
                                        FullName = usr.DisplayName
                                    });

                        foreach (var listItem in list)
                        {
                            _user = new UsersByEndPointGridData(); // dr = dt.NewRow();
                            _user.name = listItem.name; // dr["name"]
                            _user.UserId = listItem.UserId;

                            _user.lastLogon = listItem.lastLogon; _user.mailId = listItem.mailId; _user.CreatedDate = listItem.CreatedDate;
                            _user.LastModifiedDate = listItem.LastModifiedDate; _user.DIUserId = listItem.DIUserId; _user.currentuser = listItem.currentuser;
                            _user.FullName = listItem.FullName;
                            usersList.Add(_user); // dt.Rows.Add(dr);
                        }
                    }
                    else if (EndPointSysType == "SAP")
                    {
                        var list = oIACMDBContext.SAPUserinfos.Where(endP => endP.EndPointSystemId == EndPointSystemId).ToList().Join(oIACMDBContext.Users.ToList(),
                                   EndPnt => EndPnt.DIUserId, usr => usr.UserId,
                                    (EndPnt, usr) => new
                                    {
                                        name = EndPnt.UserLogonName, EndPnt.UserId,
                                        lastLogon = EndPnt.LastLoginTime,
                                        mailId = EndPnt.E_Mail, EndPnt.CreatedDate, EndPnt.LastModifiedDate, EndPnt.DIUserId,
                                        currentuser = usr.UserName
                                    });

                        foreach (var listItem in list)
                        {
                            _user = new UsersByEndPointGridData();
                            _user.name = listItem.name;
                            _user.UserId = listItem.UserId;
                            _user.lastLogon = listItem.lastLogon; _user.mailId = listItem.mailId; _user.CreatedDate = listItem.CreatedDate;
                            _user.LastModifiedDate = listItem.LastModifiedDate; _user.DIUserId = listItem.DIUserId; _user.currentuser = listItem.currentuser;
                            //_user.FullName = listItem.FullName;
                            usersList.Add(_user);
                        }
                    }
                    else if (EndPointSysType == "Lotus Notes")
                    {
                        var list = oIACMDBContext.LotusUserInfos.Where(endP => endP.EndPointSystemId == EndPointSystemId).ToList().Join(oIACMDBContext.Users.ToList(),
                                   EndPnt => EndPnt.DIUserId, usr => usr.UserId,
                                    (EndPnt, usr) => new
                                    {
                                        name = EndPnt.Name, EndPnt.UserId,
                                        lastLogon = EndPnt.LastModifiedDate,
                                        mailId = EndPnt.InternetAddress, EndPnt.CreatedDate, EndPnt.LastModifiedDate, EndPnt.DIUserId,
                                        currentuser = usr.UserName
                                    });

                        foreach (var listItem in list)
                        {
                            _user = new UsersByEndPointGridData();
                            _user.name = listItem.name;
                            _user.UserId = listItem.UserId;

                            _user.lastLogon = listItem.lastLogon; _user.mailId = listItem.mailId; _user.CreatedDate = listItem.CreatedDate;
                            _user.LastModifiedDate = listItem.LastModifiedDate; _user.DIUserId = listItem.DIUserId; _user.currentuser = listItem.currentuser;
                            //_user.FullName = listItem.FullName;
                            usersList.Add(_user);
                        }
                    }
                    else if (EndPointSysType == "SAP EP")
                    {
                        var list = oIACMDBContext.SAPEPUserInfos.Where(endP => endP.EndPointSystemId == EndPointSystemId).ToList().Join(oIACMDBContext.Users.ToList(),
                                   EndPnt => EndPnt.DIUserId, usr => usr.UserId,
                                    (EndPnt, usr) => new
                                    {
                                        name = EndPnt.UserName, EndPnt.UserId,
                                        lastLogon = EndPnt.LastModifiedDate,
                                        mailId = "", EndPnt.CreatedDate, EndPnt.LastModifiedDate, EndPnt.DIUserId,
                                        currentuser = usr.UserName
                                    });

                        foreach (var listItem in list)
                        {
                            _user = new UsersByEndPointGridData();
                            _user.name = listItem.name;
                            _user.UserId = listItem.UserId;
                            _user.lastLogon = listItem.lastLogon; _user.mailId = listItem.mailId; _user.CreatedDate = listItem.CreatedDate;
                            _user.LastModifiedDate = listItem.LastModifiedDate; _user.DIUserId = listItem.DIUserId; _user.currentuser = listItem.currentuser;
                            //_user.FullName = listItem.FullName;
                            usersList.Add(_user);
                        }
                    }

                    else if (EndPointSysType == "Other Systems")
                    {
                        var list = oIACMDBContext.InhouseUserInfos.Where(endP => endP.EndPointSystemId == EndPointSystemId).ToList().Join(oIACMDBContext.Users.ToList(),
                                   EndPnt => EndPnt.DIUserId, usr => usr.UserId,
                                    (EndPnt, usr) => new
                                    {
                                        name = EndPnt.UserName, EndPnt.UserId,
                                        //SpecialAccount = EndPnt.SpecialAccount,
                                        lastLogon = EndPnt.LastModifiedDate,
                                        mailId = "", EndPnt.CreatedDate, EndPnt.LastModifiedDate, EndPnt.DIUserId,
                                        currentuser = usr.UserName
                                    });

                        foreach (var listItem in list)
                        {
                            _user = new UsersByEndPointGridData();
                            _user.name = listItem.name;
                            _user.UserId = listItem.UserId;
                            //_user.SpecialAccount = listItem.SpecialAccount;
                            //if (listItem.SpecialAccount == null)
                            //    _user.SpecialAccount = false;
                            //else
                            //    _user.SpecialAccount = listItem.SpecialAccount;

                            _user.lastLogon = listItem.lastLogon; _user.mailId = listItem.mailId; _user.CreatedDate = listItem.CreatedDate;
                            _user.LastModifiedDate = listItem.LastModifiedDate; _user.DIUserId = listItem.DIUserId; _user.currentuser = listItem.currentuser;
                            //_user.FullName = listItem.FullName;
                            usersList.Add(_user);
                        }
                    }

                    /* if (0 == ds.Tables[0].Rows.Count)
                        return null;

                    user = ds;
                    return user; */
                    return usersList;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataSet GetInActiveUserByEndPoint(string Name, string EndpointSystemTypeName)
        {

            DataSet user = null;
            try
            {
                string query = "DI_spGetInActiveUsers";

                List<SqlParameter> parameters = new List<SqlParameter>();

                SqlParameter sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@Name";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Value = Name;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@EndpointSystemTypeName";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = EndpointSystemTypeName;
                parameters.Add(sqlParam);

                m_iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);

                m_executor = new QueryExecutor(m_iacmOperator);

                DataSet ds = m_executor.ExecuteQueryDS();

                if (null == ds)
                    return null;

                if (0 == ds.Tables.Count)
                    return null;

                user = ds;
                return user;

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
            }
        }

        public DataSet GetResourceNotAccessedSince(int NoOfDays)
        {
            string strQry = string.Format("select ResouceID,ParentID from DI_Win_ResourceInfo WHERE (DATEDIFF(dd, Resource_LastAccessedDate ,GETDATE()) > {0}) and Resource_LastAccessedDate is NOT NULL order by ParentID desc", NoOfDays);

            m_iacmOperator = new IACMDbOperator(strQry, CommandType.Text, null);

            m_executor = new QueryExecutor(m_iacmOperator);

            DataSet ds = m_executor.ExecuteQueryDS();

            if (null == ds)
                return null;

            if (0 == ds.Tables.Count)
                return null;

            return ds;
        }
        public DataSet GetResourceNotModifiedSince(int NoOfDays)
        {
            string strQry = string.Format("select ResouceID,ParentID from DI_Win_ResourceInfo WHERE (DATEDIFF(dd, Resource_LasteWriteDate ,GETDATE()) > {0}) and Resource_LasteWriteDate is NOT NULL order by ParentID desc", NoOfDays);

            m_iacmOperator = new IACMDbOperator(strQry, CommandType.Text, null);

            m_executor = new QueryExecutor(m_iacmOperator);

            DataSet ds = m_executor.ExecuteQueryDS();

            if (null == ds)
                return null;

            if (0 == ds.Tables.Count)
                return null;

            return ds;
        }

        public DataSet GetResourcesById(long ResourceId, int EndPointSysId)
        {

            DataSet user = null;
            try
            {
                string query = "DI_spGetResourcesById";

                List<SqlParameter> parameters = new List<SqlParameter>();

                SqlParameter sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@ResourceId";
                sqlParam.SqlDbType = SqlDbType.BigInt;
                sqlParam.Value = ResourceId;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@EndPointSysId";
                sqlParam.SqlDbType = SqlDbType.Int;
                sqlParam.Value = EndPointSysId;
                parameters.Add(sqlParam);

                m_iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);

                m_executor = new QueryExecutor(m_iacmOperator);

                DataSet ds = m_executor.ExecuteQueryDS();

                if (null == ds)
                    return null;

                if (0 == ds.Tables.Count)
                    return null;

                user = ds;
                return user;

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                m_executor.IACMDbOperator.Close();
            }

        }

        public DataSet GetResourcesHistoryById(long ResourceId, int EndPointSysId, int HistoryId)
        {

            DataSet user = null;
            try
            {
                string query = "DI_spGetResourcesHistoryById";

                List<SqlParameter> parameters = new List<SqlParameter>();

                SqlParameter sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@ResourceId";
                sqlParam.SqlDbType = SqlDbType.BigInt;
                sqlParam.Value = ResourceId;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@EndPointSysId";
                sqlParam.SqlDbType = SqlDbType.Int;
                sqlParam.Value = EndPointSysId;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@HistoryId";
                sqlParam.SqlDbType = SqlDbType.Int;
                sqlParam.Value = HistoryId;
                parameters.Add(sqlParam);

                m_iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);

                m_executor = new QueryExecutor(m_iacmOperator);

                DataSet ds = m_executor.ExecuteQueryDS();

                if (null == ds)
                    return null;

                if (0 == ds.Tables.Count)
                    return null;

                user = ds;
                return user;

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                m_executor.IACMDbOperator.Close();
            }

        }
        public DataSet GetAccessById(long ResourceId, int EndPointSysId)
        {

            DataSet user = null;
            try
            {
                string query = "DI_spGetAccessId";

                List<SqlParameter> parameters = new List<SqlParameter>();

                SqlParameter sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@ResourceId";
                sqlParam.SqlDbType = SqlDbType.BigInt;
                sqlParam.Value = ResourceId;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@EndPointSysId";
                sqlParam.SqlDbType = SqlDbType.Int;
                sqlParam.Value = EndPointSysId;
                parameters.Add(sqlParam);

                m_iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);

                m_executor = new QueryExecutor(m_iacmOperator);

                DataSet ds = m_executor.ExecuteQueryDS();

                if (null == ds)
                    return null;

                if (0 == ds.Tables.Count)
                    return null;

                user = ds;
                return user;

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                m_executor.IACMDbOperator.Close();
            }

        }



        public DataSet GetUXAccessById(long ResourceId, int EndPointSysId)
        {

            DataSet user = null;
            try
            {
                string query = "DI_spGetUXAccessId";

                List<SqlParameter> parameters = new List<SqlParameter>();

                SqlParameter sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@ResourceId";
                sqlParam.SqlDbType = SqlDbType.BigInt;
                sqlParam.Value = ResourceId;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@EndPointSysId";
                sqlParam.SqlDbType = SqlDbType.Int;
                sqlParam.Value = EndPointSysId;
                parameters.Add(sqlParam);

                m_iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);

                m_executor = new QueryExecutor(m_iacmOperator);

                DataSet ds = m_executor.ExecuteQueryDS();

                if (null == ds)
                    return null;

                if (0 == ds.Tables.Count)
                    return null;

                user = ds;
                return user;

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                m_executor.IACMDbOperator.Close();
            }

        }
        public DataSet GetResourcesByMount(string strMount, int EndPointSysId, string MountDrives)
        {

            DataSet user = null;
            try
            {
                string query = "DI_spGetResourcesByMount";

                List<SqlParameter> parameters = new List<SqlParameter>();

                SqlParameter sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@strMount";
                sqlParam.SqlDbType = SqlDbType.Text;
                sqlParam.Value = strMount;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@EndPointSysId";
                sqlParam.SqlDbType = SqlDbType.Int;
                sqlParam.Value = EndPointSysId;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@MountDrives";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Value = MountDrives;
                parameters.Add(sqlParam);

                m_iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);

                m_executor = new QueryExecutor(m_iacmOperator);

                DataSet ds = m_executor.ExecuteQueryDS();

                if (null == ds)
                    return null;

                if (0 == ds.Tables.Count)
                    return null;

                user = ds;
                return user;

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                m_executor.IACMDbOperator.Close();
            }

        }
        public void DeleteAll(int? _endpointSystemId)
        {
            int noAffected = -1;
            string _tablename = "DI_M_UnMappedUsers";
            try
            {

                string strQuery = string.Format("DELETE {0}", _tablename);

                if (_endpointSystemId != null)
                {
                    strQuery = string.Format("{0} WHERE EndpointSystemId={1}", strQuery, _endpointSystemId);
                }

                Log.General.WriteMessage(LogLevel.INFO, string.Format("Query {0}", strQuery));

                using (_context = new IACMDBContext())
                {
                    noAffected = _context.Database.ExecuteSqlCommand(strQuery);
                }
                Log.General.WriteMessage(LogLevel.DEBUG, string.Format("Deleted {0} records from table {1}", noAffected.ToString(), _tablename));
            }
            catch (Exception)
            {

                Log.General.WriteMessage(LogLevel.ERROR, "Error Occured While deleting the records from Unmapped users");
            }


        }

        public void AddAllUnmappedUsers(string _endpointtype, int _endpointSystemId, int _processId)
        {
            int i = 0;
            try
            {
                DeleteAll(_endpointSystemId);

                string query = "";
                string table = string.Empty;

                switch (_endpointtype)
                {
                    case Constants.ACTIVEDIRECTORY:
                        AddAdUnmappedUsers(_endpointtype, _endpointSystemId, _processId);
                        break;
                    case Constants.LOTUSNOTES:
                        AddAdUnmappedUsers(_endpointtype, _endpointSystemId, _processId);
                        break;
                    case Constants.SAP:
                        AddAdUnmappedUsers(_endpointtype, _endpointSystemId, _processId);
                        break;
                    case Constants.WINDOWS:
                        AddAdUnmappedUsers(_endpointtype, _endpointSystemId, _processId);
                        break;
                    case Constants.SAPEP:
                        AddAdUnmappedUsers(_endpointtype, _endpointSystemId, _processId);
                        break;
                    case Constants.OracleUM:
                        AddAdUnmappedUsers(_endpointtype, _endpointSystemId, _processId);
                        break;
                    case Constants.GEMS:
                        AddAdUnmappedUsers(_endpointtype, _endpointSystemId, _processId);
                        break;
                    case Constants.NPAC:
                        AddAdUnmappedUsers(_endpointtype, _endpointSystemId, _processId);
                        break;
                    case Constants.LINUX:
                        AddAdUnmappedUsers(_endpointtype, _endpointSystemId, _processId);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void TruncateTable<T>()
        {
            Type entityType = typeof(T);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            try
            {
                TableInfoAttribute tableInfo = null;
                object[] attrs = entityType.GetCustomAttributes(typeof(TableInfoAttribute), false); // Get collection of custom attributes from the method
                foreach (Attribute attr in attrs)
                {
                    if (attr is TableInfoAttribute)
                    {
                        tableInfo = (TableInfoAttribute)attr;

                        if (tableInfo == null)
                            break;

                        string strQuery = string.Format("TRUNCATE TABLE {0}", tableInfo.Name);




                        m_iacmOperator = new IACMDbOperator(strQuery, CommandType.Text, null);

                        m_executor = new QueryExecutor(m_iacmOperator);

                        int noAffected = m_executor.ExecuteQuery();

                        Log.General.WriteMessage(LogLevel.DEBUG, string.Format("Truncated {0} records from table {1}",
                                                                      noAffected,
                                                                      tableInfo.Name));


                    }
                }
            }


            catch (Exception)
            {
                throw;
            }
            finally
            {
                m_iacmOperator.Close();
            }

        }


        #region NEW LOGIC
        private void AddAdUnmappedUsers(string _endpointtype, int _endpointSystemId, int _processId)
        {
            using (_context = new IACMDBContext())
            {
                UnMappedUsers objunmappeduser = null;
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<UnMappedUsers> listUnmappeduser = new List<UnMappedUsers>();

                if (_endpointtype == Constants.ACTIVEDIRECTORY)
                {
                    IList<ADSUserInfo> listadsUnmappedusers = new List<ADSUserInfo>();
                    objDiscoveryProcess = _context.DiscoveryProcess.Where(e => e.Endpointsystemid == _endpointSystemId && e.DiscoveryProcessId == _processId).FirstOrDefault();
                    listadsUnmappedusers = _context.ADSUserInfos.Where(e => e.EndPointSystemId == _endpointSystemId && e.DIUserId == null || e.DIUserId == -1).ToList();
                    foreach (ADSUserInfo adsuser in listadsUnmappedusers)
                    {
                        objunmappeduser = new UnMappedUsers();
                        objunmappeduser.UserId = adsuser.UserId;
                        objunmappeduser.EndPointSystemId = adsuser.EndPointSystemId;
                        objunmappeduser.ProcessId = _processId;
                        objunmappeduser.CreatedBy = Constants.CreatedBy;
                        objunmappeduser.CreatedDate = DateTime.Now;
                        objunmappeduser.LastModifiedBy = "DiscoveryEngine";
                        objunmappeduser.LastModifiedDate = DateTime.Now;
                        listUnmappeduser.Add(objunmappeduser);
                    }
                }

                if (_endpointtype == Constants.LOTUSNOTES)
                {
                    IList<LotusUserInfo> listlnsUnmappedusers = new List<LotusUserInfo>();
                    objDiscoveryProcess = _context.DiscoveryProcess.Where(e => e.Endpointsystemid == _endpointSystemId && e.DiscoveryProcessId == _processId).FirstOrDefault();
                    listlnsUnmappedusers = _context.LotusUserInfos.Where(e => e.EndPointSystemId == _endpointSystemId && e.DIUserId == null || e.DIUserId == -1).ToList();
                    foreach (LotusUserInfo lnsuser in listlnsUnmappedusers)
                    {
                        objunmappeduser = new UnMappedUsers();
                        objunmappeduser.UserId = lnsuser.UserId;
                        objunmappeduser.EndPointSystemId = lnsuser.EndPointSystemId;
                        objunmappeduser.ProcessId = _processId;
                        objunmappeduser.CreatedBy = Constants.CreatedBy;
                        objunmappeduser.CreatedDate = DateTime.Now;
                        objunmappeduser.LastModifiedBy = "DiscoveryEngine";
                        objunmappeduser.LastModifiedDate = DateTime.Now;
                        listUnmappeduser.Add(objunmappeduser);
                    }
                }

                if (_endpointtype == Constants.SAPEP)
                {
                    IList<SAPEPUserInfo> listsapepUnmappedusers = new List<SAPEPUserInfo>();
                    objDiscoveryProcess = _context.DiscoveryProcess.Where(e => e.Endpointsystemid == _endpointSystemId && e.DiscoveryProcessId == _processId).FirstOrDefault();
                    listsapepUnmappedusers = _context.SAPEPUserInfos.Where(e => e.EndPointSystemId == _endpointSystemId && e.DIUserId == null || e.DIUserId == -1).ToList();
                    foreach (SAPEPUserInfo sapepuser in listsapepUnmappedusers)
                    {
                        objunmappeduser = new UnMappedUsers();
                        objunmappeduser.UserId = sapepuser.UserId;
                        objunmappeduser.EndPointSystemId = sapepuser.EndPointSystemId;
                        objunmappeduser.ProcessId = _processId;
                        objunmappeduser.CreatedBy = Constants.CreatedBy;
                        objunmappeduser.CreatedDate = DateTime.Now;
                        objunmappeduser.LastModifiedBy = "DiscoveryEngine";
                        objunmappeduser.LastModifiedDate = DateTime.Now;
                        listUnmappeduser.Add(objunmappeduser);
                    }
                }

                if (_endpointtype == Constants.SAP)
                {
                    IList<SAPUserinfo> listsapUnmappedusers = new List<SAPUserinfo>();
                    objDiscoveryProcess = _context.DiscoveryProcess.Where(e => e.Endpointsystemid == _endpointSystemId && e.DiscoveryProcessId == _processId).FirstOrDefault();
                    listsapUnmappedusers = _context.SAPUserinfos.Where(e => e.EndPointSystemId == _endpointSystemId && e.DIUserId == null || e.DIUserId == -1).ToList();
                    foreach (SAPUserinfo sapuser in listsapUnmappedusers)
                    {
                        objunmappeduser = new UnMappedUsers();
                        objunmappeduser.UserId = sapuser.UserId;
                        objunmappeduser.EndPointSystemId = sapuser.EndPointSystemId;
                        objunmappeduser.ProcessId = _processId;
                        objunmappeduser.CreatedBy = Constants.CreatedBy;
                        objunmappeduser.CreatedDate = DateTime.Now;
                        objunmappeduser.LastModifiedBy = "DiscoveryEngine";
                        objunmappeduser.LastModifiedDate = DateTime.Now;
                        listUnmappeduser.Add(objunmappeduser);
                    }
                }

               
                
                if (_endpointtype == Constants.LINUX)
                {
                    IList<LinuxUserInfo> listlnsUnmappedusers = new List<LinuxUserInfo>();
                    objDiscoveryProcess = _context.DiscoveryProcess.Where(e => e.Endpointsystemid == _endpointSystemId && e.DiscoveryProcessId == _processId).FirstOrDefault();
                    listlnsUnmappedusers = _context.LinuxUserInfos.Where(e => e.EndPointSystemId == _endpointSystemId && e.DIUserId == null || e.DIUserId == -1 && !e.IsLocked).ToList();
                    foreach (LinuxUserInfo lnsuser in listlnsUnmappedusers)
                    {
                        objunmappeduser = new UnMappedUsers();
                        objunmappeduser.UserId = lnsuser.UserId;
                        objunmappeduser.EndPointSystemId = lnsuser.EndPointSystemId;
                        objunmappeduser.ProcessId = _processId;
                        objunmappeduser.CreatedBy = Constants.CreatedBy;
                        objunmappeduser.CreatedDate = DateTime.Now;
                        objunmappeduser.LastModifiedBy = "DiscoveryEngine";
                        objunmappeduser.LastModifiedDate = DateTime.Now;
                        listUnmappeduser.Add(objunmappeduser);
                    }
                }
                if (_endpointtype == Constants.OracleUM)
                {
                    IList<OracleUMUserInfo> listlnsUnmappedusers = new List<OracleUMUserInfo>();
                    objDiscoveryProcess = _context.DiscoveryProcess.Where(e => e.Endpointsystemid == _endpointSystemId && e.DiscoveryProcessId == _processId).FirstOrDefault();
                    listlnsUnmappedusers = _context.OracleUMUserInfo.Where(e => e.EndPointSystemId == _endpointSystemId && e.DIUserId == null || e.DIUserId == -1).ToList();
                    foreach (OracleUMUserInfo lnsuser in listlnsUnmappedusers)
                    {
                        objunmappeduser = new UnMappedUsers();
                        objunmappeduser.UserId = lnsuser.UserId;
                        objunmappeduser.EndPointSystemId = lnsuser.EndPointSystemId;
                        objunmappeduser.ProcessId = _processId;
                        objunmappeduser.CreatedBy = Constants.CreatedBy;
                        objunmappeduser.CreatedDate = DateTime.Now;
                        objunmappeduser.LastModifiedBy = "DiscoveryEngine";
                        objunmappeduser.LastModifiedDate = DateTime.Now;
                        listUnmappeduser.Add(objunmappeduser);
                    }
                }

                if (_endpointtype == Constants.OracleEBS)
                {
                    IList<OracleEBSUserInfo> listlnsUnmappedusers = new List<OracleEBSUserInfo>();
                    objDiscoveryProcess = _context.DiscoveryProcess.Where(e => e.Endpointsystemid == _endpointSystemId && e.DiscoveryProcessId == _processId).FirstOrDefault();
                    listlnsUnmappedusers = _context.OracleEBSUserInfo.Where(e => (e.EndPointSystemId == _endpointSystemId && e.DIUserId == null || e.DIUserId == -1) && !e.IsLocked).ToList();
                    foreach (OracleEBSUserInfo lnsuser in listlnsUnmappedusers)
                    {
                        objunmappeduser = new UnMappedUsers();
                        objunmappeduser.UserId = lnsuser.UserId;
                        objunmappeduser.EndPointSystemId = lnsuser.EndPointSystemId;
                        objunmappeduser.ProcessId = _processId;
                        objunmappeduser.CreatedBy = Constants.CreatedBy;
                        objunmappeduser.CreatedDate = DateTime.Now;
                        objunmappeduser.LastModifiedBy = "DiscoveryEngine";
                        objunmappeduser.LastModifiedDate = DateTime.Now;
                        listUnmappeduser.Add(objunmappeduser);
                    }
                }

                SQLBulkUploader<UnMappedUsers> sqlUnMappedUsers = null;
                try
                {
                    string tableName = "DI_M_UnMappedUsers";

                    sqlUnMappedUsers = new SQLBulkUploader<UnMappedUsers>(tableName);

                    sqlUnMappedUsers.Flush(listUnmappeduser);

                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    sqlUnMappedUsers.Close();
                }
            }
        }
        #endregion



        public void DeleteDiscoveryJobSchedule(int _discoveryProcessId)
        {
            //TODO: CHECK HARI
            //try
            //{
            //    using (IACMDBContext _context = new IACMDBContext())
            //    {


            //        Scheduler DeletedSchedulerItems = (from ep in _context.Scheduler
            //                                           where ep.DiscoveryProcessId == _discoveryProcessId
            //                                           select ep).FirstOrDefault();
            //        _context.Scheduler.Remove(DeletedSchedulerItems);


            //        DiscoveryProcess DeletedItems = (from ep in _context.DiscoveryProcess
            //                                         where ep.DiscoveryProcessId == _discoveryProcessId
            //                                         select ep).FirstOrDefault();
            //        if (!DeletedItems.Default)
            //        {
            //            _context.DiscoveryProcess.Remove(DeletedItems);
            //            _context.SaveChanges();
            //            DiscoveryLog DeletionLog=new DiscoveryLog();
            //            DeletionLog.LogLevel=LogLevelType.Info.ToString();
            //            DeletionLog.Message= string.Format("Discovery Process {0}-{1} has been deleted.",DeletedItems.ProcessName,DeletedItems.DiscoveryProcessId);
            //            DeletionLog.LogDate = DateTime.Now;
            //            DILog.WriteLogToDB(DeletionLog);
            //        }


            //    }
            //}
            //catch (Exception ex)
            //{
            //    Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Deleting Discovery Jobs ", ex);
            //    throw ex;
            //}
        }







        public static void AddEndpointVPN(EndPoint _Endpoint)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (_Endpoint.EndpointSystemId == 0)
                    {
                        _context.EndpointSystems.Add(_Endpoint);
                    }
                    else
                    {
                        EndPoint Item = _context.EndpointSystems.Find(_Endpoint.EndpointSystemId);
                        Item.CreatedBy = _Endpoint.CreatedBy;
                        Item.CreatedDate = _Endpoint.CreatedDate;
                        Item.Description = _Endpoint.Description;
                        Item.EndPointSystemType = _Endpoint.EndPointSystemType;
                        Item.EndpointSystemTypeId = _Endpoint.EndpointSystemTypeId;
                        Item.DisplayName = _Endpoint.DisplayName;
                        Item.IncludeInAttestation = _Endpoint.IncludeInAttestation;
                        Item.IsActive = _Endpoint.IsActive;
                        Item.IsPasswordResetAllowed = _Endpoint.IsPasswordResetAllowed;
                        Item.IsTrustedSource = _Endpoint.IsTrustedSource;
                        Item.LastModifiedBy = _Endpoint.LastModifiedBy;
                        Item.LastModifiedDate = _Endpoint.LastModifiedDate;
                        Item.TenantId = _Endpoint.TenantId;

                    }
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void AddEndPoint(EndPoint endpoint)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.EndpointSystems.Add(endpoint);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdateEndPoint(EndPoint endpoint)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var entry = _context.Entry(endpoint);
                    if (entry.State == EntityState.Detached)
                    {
                        var set = _context.Set<CostCenter>();
                        CostCenter attached = set.Find(endpoint.EndpointSystemId);

                        if (attached != null)
                        {
                            var attachedEntry = _context.Entry(attached);
                            attachedEntry.CurrentValues.SetValues(endpoint);
                        }
                    }
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void KillDiscoveryProcess(int DiscoveryProcessId, string LastModifiedBy)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var set = _context.Set<DiscoveryProcess>();
                    DiscoveryProcess discovery = set.Find(DiscoveryProcessId);

                    if (discovery != null)
                    {
                        // discovery.Status = "Completed";
                        //  discovery.ProcessEndAt = DateTime.Now;
                        discovery.LastModifiedBy = LastModifiedBy;
                        discovery.LastModifiedDate = DateTime.Now;
                        var attachedEntry = _context.Entry(discovery);
                        attachedEntry.CurrentValues.SetValues(discovery);
                    }
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public IList<UsersByEndPointGridData> GetUsersByEndPoint(int EndPointSystemId, EndpointType EndPointSysType) // DataSet
        {
            try
            {
                IList<UsersByEndPointGridData> usersList = new List<UsersByEndPointGridData>();
                UsersByEndPointGridData _user;

                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (EndPointSysType == EndpointType.ActiveDirectory)
                    {
                        var list = oIACMDBContext.ADSUserInfos.Where(endP => endP.EndPointSystemId == EndPointSystemId).ToList().Join(oIACMDBContext.Users.ToList(),
                                   EndPnt => EndPnt.DIUserId, usr => usr.UserId,
                                    (EndPnt, usr) => new // UsersByEndPointGridData
                                    {
                                        name = EndPnt.sAMAccountName, EndPnt.UserId, EndPnt.lastLogon,
                                        mailId = EndPnt.mail, EndPnt.CreatedDate, EndPnt.LastModifiedDate, EndPnt.DIUserId,
                                        currentuser = usr.UserName,
                                        FullName = usr.DisplayName
                                    });

                        foreach (var listItem in list)
                        {
                            _user = new UsersByEndPointGridData(); // dr = dt.NewRow();
                            _user.name = listItem.name; // dr["name"]
                            _user.UserId = listItem.UserId;

                            _user.lastLogon = listItem.lastLogon; _user.mailId = listItem.mailId; _user.CreatedDate = listItem.CreatedDate;
                            _user.LastModifiedDate = listItem.LastModifiedDate; _user.DIUserId = listItem.DIUserId; _user.currentuser = listItem.currentuser;
                            _user.FullName = listItem.FullName;
                            usersList.Add(_user); // dt.Rows.Add(dr);
                        }
                    }
                    else if (EndPointSysType == EndpointType.SAP)
                    {
                        var list = oIACMDBContext.SAPUserinfos.Where(endP => endP.EndPointSystemId == EndPointSystemId).ToList().Join(oIACMDBContext.Users.ToList(),
                                   EndPnt => EndPnt.DIUserId, usr => usr.UserId,
                                    (EndPnt, usr) => new
                                    {
                                        name = EndPnt.UserLogonName, EndPnt.UserId,
                                        lastLogon = EndPnt.LastLoginTime,
                                        mailId = EndPnt.E_Mail, EndPnt.CreatedDate, EndPnt.LastModifiedDate, EndPnt.DIUserId,
                                        currentuser = usr.UserName,
                                        FullName = usr.DisplayName
                                    });

                        foreach (var listItem in list)
                        {
                            _user = new UsersByEndPointGridData();
                            _user.name = listItem.name;
                            _user.UserId = listItem.UserId;
                            _user.lastLogon = listItem.lastLogon; _user.mailId = listItem.mailId; _user.CreatedDate = listItem.CreatedDate;
                            _user.LastModifiedDate = listItem.LastModifiedDate; _user.DIUserId = listItem.DIUserId; _user.currentuser = listItem.currentuser;
                            _user.FullName = listItem.FullName;
                            usersList.Add(_user);
                        }
                    }
                    else if (EndPointSysType == EndpointType.LotusNotes)
                    {
                        var list = oIACMDBContext.LotusUserInfos.Where(endP => endP.EndPointSystemId == EndPointSystemId).ToList().Join(oIACMDBContext.Users.ToList(),
                                   EndPnt => EndPnt.DIUserId, usr => usr.UserId,
                                    (EndPnt, usr) => new
                                    {
                                        name = EndPnt.AccountName, EndPnt.UserId,
                                        lastLogon = EndPnt.LastModifiedDate,
                                        mailId = EndPnt.InternetAddress, EndPnt.CreatedDate, EndPnt.LastModifiedDate, EndPnt.DIUserId,
                                        currentuser = usr.UserName,
                                        FullName = usr.DisplayName
                                    });

                        foreach (var listItem in list)
                        {
                            _user = new UsersByEndPointGridData();
                            _user.name = listItem.name;
                            _user.UserId = listItem.UserId;

                            _user.lastLogon = listItem.lastLogon; _user.mailId = listItem.mailId; _user.CreatedDate = listItem.CreatedDate;
                            _user.LastModifiedDate = listItem.LastModifiedDate; _user.DIUserId = listItem.DIUserId; _user.currentuser = listItem.currentuser;
                            _user.FullName = listItem.FullName;
                            usersList.Add(_user);
                        }
                    }
                    else if (EndPointSysType == EndpointType.SAPEnterprisePortal)
                    {
                        var list = oIACMDBContext.SAPEPUserInfos.Where(endP => endP.EndPointSystemId == EndPointSystemId).ToList().Join(oIACMDBContext.Users.ToList(),
                                   EndPnt => EndPnt.DIUserId, usr => usr.UserId,
                                    (EndPnt, usr) => new
                                    {
                                        name = EndPnt.UserName, EndPnt.UserId,
                                        lastLogon = EndPnt.LastModifiedDate,
                                        mailId = "", EndPnt.CreatedDate, EndPnt.LastModifiedDate, EndPnt.DIUserId,
                                        currentuser = usr.UserName,
                                        FullName = usr.DisplayName
                                    });

                        foreach (var listItem in list)
                        {
                            _user = new UsersByEndPointGridData();
                            _user.name = listItem.name;
                            _user.UserId = listItem.UserId;
                            _user.lastLogon = listItem.lastLogon; _user.mailId = listItem.mailId; _user.CreatedDate = listItem.CreatedDate;
                            _user.LastModifiedDate = listItem.LastModifiedDate; _user.DIUserId = listItem.DIUserId; _user.currentuser = listItem.currentuser;
                            _user.FullName = listItem.FullName;
                            usersList.Add(_user);
                        }
                    }

                    else if (EndPointSysType == EndpointType.InhouseSystems)
                    {
                        var list = oIACMDBContext.InhouseUserInfos.Where(endP => endP.EndPointSystemId == EndPointSystemId).ToList().Join(oIACMDBContext.Users.ToList(),
                                   EndPnt => EndPnt.DIUserId, usr => usr.UserId,
                                    (EndPnt, usr) => new
                                    {
                                        name = EndPnt.UserName, EndPnt.UserId,
                                        lastLogon = EndPnt.LastModifiedDate,
                                        mailId = "", EndPnt.CreatedDate, EndPnt.LastModifiedDate, EndPnt.DIUserId,
                                        currentuser = usr.UserName,
                                        FullName = usr.DisplayName
                                    });

                        foreach (var listItem in list)
                        {
                            _user = new UsersByEndPointGridData();
                            _user.name = listItem.name;
                            _user.UserId = listItem.UserId;
                            _user.lastLogon = listItem.lastLogon; _user.mailId = listItem.mailId; _user.CreatedDate = listItem.CreatedDate;
                            _user.LastModifiedDate = listItem.LastModifiedDate; _user.DIUserId = listItem.DIUserId; _user.currentuser = listItem.currentuser;
                            _user.FullName = listItem.FullName;
                            usersList.Add(_user);
                        }
                    }

                    /* if (0 == ds.Tables[0].Rows.Count)
                        return null;

                    user = ds;
                    return user; */
                    return usersList;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static string GetExceptionComment(int endpointSystemId, int endpointUserId, int userId, int version)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                return (from table in _context.AccountReconcilations
                        where table.UserId == userId && table.EndpointUserId == endpointUserId && table.EndpointSystemId == endpointSystemId && table.Version == version
                        select table.Comments
                                ).FirstOrDefault();
            }
        }
    }
}
