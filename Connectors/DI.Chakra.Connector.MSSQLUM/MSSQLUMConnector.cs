using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using DI.Chakra.DBConfigHelper;

namespace DI.Chakra.Connector
{
    public class MSSQLUMConnector : DI.Chakra.Connector.ConnectorBase
    {
        #region Members

        private Dictionary<long, string> dicUsers = new Dictionary<long, string>();
        private Dictionary<long, string> dicRoles = new Dictionary<long, string>();
        private Dictionary<long, string> dicLogin = new Dictionary<long, string>();
        private Dictionary<string, ArrayList> dicRoleUsers = new Dictionary<string, ArrayList>();
        private DomainModel.EndpointConfiguration m_msqlConfig = null;
        public EndpointConfiguration MSSQLUMConfiguraiton
        {
            get { return m_msqlConfig; }
            set { this.m_msqlConfig = value; }
        }

        SqlConnection sqlConnect = null;
        string connectionString = "";
        public EndPoint endpoint;
        string databaseName = "";
        string _connectionString = "";

        #endregion

        #region Core
        public MSSQLUMConnector(EndPoint endpoint, AccessRequest currentRequest)
        {
            this.EndPoint = endpoint;
            this.CurrentRequest = currentRequest;
            var mssqlConfig = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);
            mssqlConfig.ConfigurationDetails.ForEach(lq =>
            {
                if (lq.KeyId == Convert.ToInt64(EndpointKey.Password))
                {
                    lq.Value = RSAHelper.DecryptData(lq.Value);
                }
            });
            this.MSSQLUMConfiguraiton = mssqlConfig;
            string databaseName = Configuration(EndpointKey.InstanceName);

        }
        public override IMProcessor IMProcessor
        {
            get
            {
                MSSQL.IMProcessor processor = new MSSQL.IMProcessor();

                processor.EndPoint = this.EndPoint;
                processor.MSSQLUMConfiguraiton = this.MSSQLUMConfiguraiton;


                return processor;
            }


        }

        #endregion

        #region Discovery
        public override bool Connect()
        {
            connectionString = "Data Source=" + Configuration(EndpointKey.IPAddress) + "; Initial Catalog=" + Configuration(EndpointKey.InstanceName) + ";Persist Security Info= True; User ID=" + Configuration(EndpointKey.UserName) + ";Password=" + Configuration(EndpointKey.Password) + ";Connect Timeout=100";
            try
            {
                sqlConnect = new SqlConnection(connectionString);
                sqlConnect.Open();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        // ALTER LOGIN accepts neither a variable for the login name nor for the password, so the
        // statement is built server-side from bound parameters and run via sp_executesql:
        // the login is bracket-quoted (']' doubled, as QUOTENAME does) and the password is a
        // single-quoted literal (''' doubled). The command text sent from here is constant.
        private const string ChangeUserPasswordQuery =
            "DECLARE @sql nvarchar(max) = N'ALTER LOGIN [' + REPLACE(@LoginName, N']', N']]') + N'] WITH PASSWORD = N''' + REPLACE(@Password, N'''', N'''''') + N''';'; " +
            "EXEC sp_executesql @sql;";
        private string Configuration(EndpointKey Key)
        {
            var config = MSSQLUMConfiguraiton.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
            return config != null ? config.Value : "";
        }
        public override void Disconnect()
        {
            throw new NotImplementedException();
        }
        public override bool ChangePassword(string LoginName, string Password, long DIUserId, bool isReactivation)
        {
            try
            {
                if (string.IsNullOrEmpty(LoginName))
                {
                    throw new ArgumentException("Login name must not be null or empty.", nameof(LoginName));
                }
                SqlCommand cmd = null;
                //     con.Open();
                cmd = new SqlCommand();
                cmd.Connection = sqlConnect;
                cmd.CommandType = CommandType.Text;


                cmd.CommandText = ChangeUserPasswordQuery;
                cmd.Parameters.Add("@LoginName", SqlDbType.NVarChar, 128).Value = LoginName;
                cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 128).Value = (object)Password ?? DBNull.Value;
                int Result = cmd.ExecuteNonQuery();
                return true;
            }


            catch (Exception ex)
            {

                return false;
            }
        }

        public override void GetData()
        {
            try
            {

                long DiscoveryProcessId = base.DiscoveryProcess.Id;

                List<EndpointObjects> EndpointObjects = DI.Chakra.DomainModel.DiscoveryProcess.GetByDiscoveryProcessId(DiscoveryProcessId);

                bool IsUserSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_MSSQL_User) != null;
                bool IsRoleSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_MSSQL_Role) != null;
                bool IsLoginSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_MSSQL_Login) != null;
                bool IsDatabaseSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_MSSQL_Database) != null;
                bool IsSchemaSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_MSSQL_Schemainfo) != null;
                bool IsDatabaseRolePermissionSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_MSSQL_DatabaseRolePermission) != null;
                bool IsServerRolePermissionSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_MSSQL_ServerRolePermission) != null;
                bool IsUserRoleSchemaPermissionSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_MSSQL_UserRoleSchemaPermission) != null;
                bool IsServerPermissionSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_MSSQL_ServerPermission) != null;
                bool IsDatabasePermissionSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_MSSQL_DatabasePermission) != null;
                bool IsLoginDatabaseMapingSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_MSSQL_LoginDatabaseMaping) != null;

                if (IsUserSelected || IsRoleSelected)
                {
                    DiscoverUsers();
                    DiscoverRoles();



                }


                if (IsLoginSelected)
                {
                    DiscoverLogin();

                }



                if (IsSchemaSelected)
                {
                    DiscoverSchema();
                }

                if (IsDatabaseSelected)
                {
                    DiscoverDatabase();

                }
                if (IsDatabasePermissionSelected)
                {
                    DiscoverDatabasePermission();
                }

                if (IsDatabaseRolePermissionSelected)
                {
                    DiscoverDatabaseRolePermission();
                }
                if (IsLoginDatabaseMapingSelected)
                {
                    DiscoverLoginDatabaseMappingPermission();
                }
                if (IsServerPermissionSelected)
                {
                    DiscoverServerPermission();
                }
                if (IsServerRolePermissionSelected)
                {
                    DiscoverServerRolePermission();
                }

                if (IsUserRoleSchemaPermissionSelected || IsUserSelected || IsRoleSelected)
                {
                    DiscoverUserRoleSchemaDBMap();
                    SaveUserGroups();
                    this.UpdateEntitlementRecon();

                }




                //MSSQLUMUserInfo.MapWithMasterUser(EndPoint, MSSQLUMConfiguraiton.MapAll != null ? MSSQLUMConfiguraiton.MapAll : false);
                MSSQLUMUserInfo.MapWithMasterUserForReconTable(EndPoint, MSSQLUMConfiguraiton.MapAll != null ? MSSQLUMConfiguraiton.MapAll : false);
                MSSQLUMUserInfo.PopulateMasterUserFrommssql(EndPoint);

            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, ex.InnerException.ToString(), ex);

                throw;
            }

        }

        #region Discover Objects
        private void DiscoverUsers()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering Users"));
            IList<MSSQLUMUserInfo> userlist = this.GetUsers();
            MSSQLUMUserInfo.SaveUsers(userlist, EndPoint.Id, MasterConfigHelper.ServiceUser);
            userlist = null;
            AccountReconcilation.ProcessDeletedAccounts(this.EndPoint.Id, this.Process, MasterConfigHelper.ServiceUser);
            userlist = MSSQLUMUserInfo.GetDeletedUsers(this.EndPoint.Id, this.Process);

            MSSQLUMUserInfo.RemovemssqlsUser(userlist.ToList());
            dicUsers = MSSQLUMUserInfo.GetAllByEndPointSystemId(this.EndPoint.Id);
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering Users"));
        }
        public IList<MSSQLUMUserInfo> GetUsers()
        {
            try
            {

                // string query = "select uid, status, name, sid, roles, createdate, updatedate, altuid, password, gid, environ, hasdbaccess, islogin, isntname, isntgroup, isntuser, issqluser, isaliased,issqlrole, isapprole from master.dbo.sysusers  where name not like '#%#'";
                string query = string.Format("select uid, status, name, sid, roles, createdate, updatedate, altuid, password, gid, environ, hasdbaccess, islogin, isntname, isntgroup, isntuser, issqluser, isaliased,issqlrole, isapprole from {0}.dbo.sysusers  where name not like '#%#'", Configuration(EndpointKey.InstanceName));
                MSSQLUMUserInfo _MSSQLUMUser = null;
                IDataReader reader = null;

                IList<MSSQLUMUserInfo> _list = new List<MSSQLUMUserInfo>();
                try
                {

                    SqlCommand cmd = null;
                    //     con.Open();
                    cmd = new SqlCommand();
                    cmd.Connection = sqlConnect;


                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.Text;

                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        _MSSQLUMUser = new MSSQLUMUserInfo();

                        _MSSQLUMUser.uid = CheckNull.AgainstInt(reader["uid"]);
                        _MSSQLUMUser.EndPointSystemId = EndPoint.Id;
                        _MSSQLUMUser.status = CheckNull.AgainstInt(reader["status"]);
                        _MSSQLUMUser.Name = CheckNull.AgainstString(reader["Name"]);
                        //double sid = CheckNull.AgainstDouble(Convert.ToDouble(reader["sid"]));
                        byte[] binaryString = (byte[])CheckNull.AgainstByte(reader["sid"]);
                        if (binaryString != null)
                        {
                            // if the original encoding was ASCII
                            string sid = Convert.ToBase64String(binaryString); // BitConverter.ToString(binaryString);

                            StringBuilder hex = new StringBuilder(binaryString.Length * 2);
                            foreach (byte b in binaryString)
                                hex.AppendFormat("{0:X2}", b);

                            _MSSQLUMUser.sid = hex.ToString();
                        }
                        //_MSSQLUMUser.Roles = CheckNull.AgainstString(reader["Roles"]);
                        _MSSQLUMUser.CreateDate = CheckNull.AgainstDatetime(reader["CreateDate"]);
                        _MSSQLUMUser.UpdateDate = CheckNull.AgainstDatetime(reader["UpdateDate"]);
                        _MSSQLUMUser.altuid = CheckNull.AgainstInt(reader["altuid"]);
                        _MSSQLUMUser.environ = CheckNull.AgainstString(reader["environ"]);
                        _MSSQLUMUser.hasdbaccess = CheckNull.AgainstBit(reader["hasdbaccess"]);
                        _MSSQLUMUser.islogin = CheckNull.AgainstBit(reader["islogin"]);
                        _MSSQLUMUser.isntname = CheckNull.AgainstBit(reader["isntname"]);
                        _MSSQLUMUser.isntgroup = CheckNull.AgainstBit(reader["isntgroup"]);
                        _MSSQLUMUser.isntuser = CheckNull.AgainstBit(reader["isntuser"]);
                        _MSSQLUMUser.issqluser = CheckNull.AgainstBit(reader["issqluser"]);
                        _MSSQLUMUser.isaliased = CheckNull.AgainstBit(reader["isaliased"]);
                        _MSSQLUMUser.issqlrole = CheckNull.AgainstBit(reader["issqlrole"]);
                        _MSSQLUMUser.isapprole = CheckNull.AgainstBit(reader["isapprole"]);
                        //_MSSQLUMUser.CreatedDate = DateTime.Now;
                        //_MSSQLUMUser.LastModifiedDate = DateTime.Now;

                        _list.Add(_MSSQLUMUser);

                    }
                    return _list;

                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    reader.Close();
                    reader.Dispose();
                    // con.Close();
                    //con.Dispose();
                }
                return _list;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void DiscoverRoles()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering Roles"));
            //this.GetRoles();
            IList<MSSQLUMRoleInfo> list = this.GetRoles();
            //MSSQLUMRoleInfo.SaveRoles(list);
            //list = MSSQLUMRoleInfo.GetDeletedByEndPointSystemId(this.EndPoint.Id);
            MSSQLUMRoleInfo.SaveRoles(list.ToList(), MasterConfigHelper.ServiceUser);
            dicRoles = MSSQLUMRoleInfo.GetByEndpointId(this.EndPoint.Id);
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering Roles"));
        }
        public IList<MSSQLUMRoleInfo> GetRoles()


        {



            try
            {

                string query = "exec sp_helprole";

                MSSQLUMRoleInfo _MSSQLUMRoleInfo = null;

                IList<MSSQLUMRoleInfo> _list = new List<MSSQLUMRoleInfo>();
                IDataReader reader = null;
                try
                {

                    SqlCommand cmd = null;
                    // con.Open();
                    cmd = new SqlCommand();

                    cmd.Connection = sqlConnect;


                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.Text;

                    reader = cmd.ExecuteReader();



                    while (reader.Read())
                    {
                        _MSSQLUMRoleInfo = new MSSQLUMRoleInfo();

                        //_MSSQLUMRoleInfo.Id = CheckNull.AgainstInt(reader["RoleId"]);
                        _MSSQLUMRoleInfo.RoleName = CheckNull.AgainstString(reader["RoleName"]);
                        _MSSQLUMRoleInfo.IsAppRole = CheckNull.AgainstBit(Convert.ToBoolean(reader["IsAppRole"]));
                        _MSSQLUMRoleInfo.DatabaseName = Configuration(EndpointKey.InstanceName);


                        _MSSQLUMRoleInfo.EndPointSystemId = EndPoint.Id;
                        //_MSSQLUMRoleInfo.CreatedDate = DateTime.Now;
                        //_MSSQLUMRoleInfo.LastModifiedDate = DateTime.Now;
                        _list.Add(_MSSQLUMRoleInfo);

                    }


                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    reader.Close();
                    //  con.Close();
                }
                return _list;
            }
            catch (Exception)
            {
                throw;
            }


        }

        private void DiscoverLogin()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering login"));
            IList<MSSQLUMLoginInfo> listPrinters = this.GetLoginInfo();
            if (listPrinters.Count > 0)
                MSSQLUMLoginInfo.AddmssqlPrinterInfoBulk(listPrinters.ToList());
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering login"));
        }
        public IList<MSSQLUMLoginInfo> GetLoginInfo()
        {
            try
            {

                string query = "Select LoginName,Createdate,updatedate,accdate,dbname,password,denylogin,hasaccess,isntname,isntgroup,isntuser,sysadmin,securityadmin,serveradmin,setupadmin,processadmin,diskadmin,dbcreator,bulkadmin,sid from master.dbo.syslogins where LoginName not like '#%#'";

                MSSQLUMLoginInfo _MSSQLUMLogin = null;
                IDataReader reader = null;

                IList<MSSQLUMLoginInfo> _list = new List<MSSQLUMLoginInfo>();
                try
                {

                    SqlCommand cmd = null;
                    //     con.Open();
                    cmd = new SqlCommand();
                    cmd.Connection = sqlConnect;


                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.Text;

                    reader = cmd.ExecuteReader();



                    while (reader.Read())
                    {
                        _MSSQLUMLogin = new MSSQLUMLoginInfo();

                        _MSSQLUMLogin.accdate = CheckNull.AgainstDatetime(reader["accdate"]);
                        _MSSQLUMLogin.bulkadmin = CheckNull.AgainstBit(Convert.ToBoolean(reader["bulkadmin"]));
                        _MSSQLUMLogin.Createdate = CheckNull.AgainstDatetime(reader["Createdate"]);
                        _MSSQLUMLogin.dbcreator = CheckNull.AgainstBit(Convert.ToBoolean(reader["dbcreator"]));
                        _MSSQLUMLogin.dbname = CheckNull.AgainstString(reader["dbname"]);
                        _MSSQLUMLogin.denylogin = CheckNull.AgainstBit(Convert.ToBoolean(reader["denylogin"]));
                        _MSSQLUMLogin.diskadmin = CheckNull.AgainstBit(Convert.ToBoolean(reader["diskadmin"]));
                        _MSSQLUMLogin.hasaccess = CheckNull.AgainstBit(Convert.ToBoolean(reader["hasaccess"]));
                        _MSSQLUMLogin.isntgroup = CheckNull.AgainstBit(Convert.ToBoolean(reader["isntgroup"]));
                        _MSSQLUMLogin.isntname = CheckNull.AgainstBit(Convert.ToBoolean(reader["isntname"]));
                        _MSSQLUMLogin.isntuser = CheckNull.AgainstBit(Convert.ToBoolean(reader["isntuser"]));
                        _MSSQLUMLogin.LoginName = CheckNull.AgainstString(reader["LoginName"]);
                        //_MSSQLUMLogin.password = CheckNull.AgainstString(reader["password"]);
                        _MSSQLUMLogin.processadmin = CheckNull.AgainstBit(Convert.ToBoolean(reader["processadmin"]));
                        _MSSQLUMLogin.securityadmin = CheckNull.AgainstBit(Convert.ToBoolean(reader["securityadmin"]));
                        _MSSQLUMLogin.serveradmin = CheckNull.AgainstBit(Convert.ToBoolean(reader["serveradmin"]));
                        _MSSQLUMLogin.setupadmin = CheckNull.AgainstBit(Convert.ToBoolean(reader["setupadmin"]));
                        _MSSQLUMLogin.sysadmin = CheckNull.AgainstBit(Convert.ToBoolean(reader["sysadmin"]));
                        _MSSQLUMLogin.updatedate = CheckNull.AgainstDatetime(reader["updatedate"]);
                        byte[] binaryString = (byte[])CheckNull.AgainstByte(reader["sid"]);
                        if (binaryString != null)
                        {
                            // if the original encoding was ASCII
                            string sid = Convert.ToBase64String(binaryString); // BitConverter.ToString(binaryString);

                            StringBuilder hex = new StringBuilder(binaryString.Length * 2);
                            foreach (byte b in binaryString)
                                hex.AppendFormat("{0:X2}", b);

                            _MSSQLUMLogin.sid = hex.ToString();
                        }


                        //_MSSQLUMLogin.EndPointSystemId = EndPoint.Id;
                        //_MSSQLUMLogin.CreatedDate = DateTime.Now;
                        //_MSSQLUMLogin.LastModifiedDate = DateTime.Now;
                        _list.Add(_MSSQLUMLogin);

                    }

                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    reader.Close();
                    reader.Dispose();
                    // con.Close();
                    //con.Dispose();
                }
                return _list;


            }
            catch (Exception)
            {
                throw;
            }
        }


        private void DiscoverDatabaseRolePermission()

        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering DatabaseRolePermission"));
            IList<MSSQLUMDatabaseRolePermissions> listPrinters = this.GetDatabaseRolePermissionsInfo();
            if (listPrinters.Count > 0)
                MSSQLUMDatabaseRolePermissions.AddDatabaseRolePermissionsInfoBulk(listPrinters.ToList());
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering DatabaseRolePermission"));
        }
        public IList<MSSQLUMDatabaseRolePermissions> GetDatabaseRolePermissionsInfo()
        {
            try
            {

                string query = "sp_dbfixedrolepermission";

                MSSQLUMDatabaseRolePermissions _MSSQLUMRolePermissionInfo = null;
                IDataReader reader = null;

                IList<MSSQLUMDatabaseRolePermissions> _list = new List<MSSQLUMDatabaseRolePermissions>();
                try
                {

                    SqlCommand cmd = null;
                    //     con.Open();
                    cmd = new SqlCommand();
                    cmd.Connection = sqlConnect;


                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.Text;

                    reader = cmd.ExecuteReader();



                    while (reader.Read())
                    {
                        _MSSQLUMRolePermissionInfo = new MSSQLUMDatabaseRolePermissions();

                        _MSSQLUMRolePermissionInfo.DBFixedRoleName = CheckNull.AgainstString(reader["DBFixedRole"]);
                        _MSSQLUMRolePermissionInfo.Permission = CheckNull.AgainstString(reader["Permission"]);
                        _MSSQLUMRolePermissionInfo.EndPointSystemId = EndPoint.Id;
                        //_MSSQLUMRolePermissionInfo.CreatedDate = DateTime.Now;
                        //_MSSQLUMRolePermissionInfo.LastModifiedDate = DateTime.Now;
                        _list.Add(_MSSQLUMRolePermissionInfo);

                    }


                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    reader.Close();
                    reader.Dispose();
                    //con.Close();
                    //con.Dispose();
                }

                return _list;

            }
            catch (Exception)
            {
                throw;
            }
        }

        private void DiscoverServerRolePermission()

        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering ServerRolePermission"));
            IList<MSSQLUMServerRolePermissions> listPrinters = this.GetServerRolePermissionsInfo();
            if (listPrinters.Count > 0)
                MSSQLUMServerRolePermissions.AddServerRolePermissionsInfoBulk(listPrinters.ToList());
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering ServerRolePermission"));
        }
        public IList<MSSQLUMServerRolePermissions> GetServerRolePermissionsInfo()
        {
            try
            {

                string query = "sp_srvrolepermission";

                MSSQLUMServerRolePermissions _MSSQLUMRolePermissionInfo = null;
                IDataReader reader = null;

                IList<MSSQLUMServerRolePermissions> _list = new List<MSSQLUMServerRolePermissions>();
                try
                {

                    SqlCommand cmd = null;
                    //     con.Open();
                    cmd = new SqlCommand();
                    cmd.Connection = sqlConnect;


                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.Text;

                    reader = cmd.ExecuteReader();



                    while (reader.Read())
                    {
                        _MSSQLUMRolePermissionInfo = new MSSQLUMServerRolePermissions();

                        _MSSQLUMRolePermissionInfo.ServerRole = CheckNull.AgainstString(reader["ServerRole"]);
                        _MSSQLUMRolePermissionInfo.Permission = CheckNull.AgainstString(reader["Permission"]);
                        _MSSQLUMRolePermissionInfo.EndPointSystemId = EndPoint.Id;
                        //_MSSQLUMRolePermissionInfo.CreatedDate = DateTime.Now;
                        //_MSSQLUMRolePermissionInfo.LastModifiedDate = DateTime.Now;
                        _list.Add(_MSSQLUMRolePermissionInfo);

                    }


                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    reader.Close();
                    reader.Dispose();
                    //con.Close();
                    //con.Dispose();
                }

                return _list;

            }
            catch (Exception)
            {
                throw;
            }
        }


        private void DiscoverUserRoleSchemaDBMap()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering UserRoleSchemaDBMap"));
            IList<MSSQLUMUserRoleSchemaDBMapInfo> listPrinters = this.GetUserRoleSchemaDBMapInfo();

            FilldicUserRoles(listPrinters);

            if (listPrinters.Count > 0)
                MSSQLUMUserRoleSchemaDBMapInfo.AddUserRoleSchemaDBMapInfoInfoBulk(listPrinters.ToList());
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering UserRoleSchemaDBMap"));
        }
        public IList<MSSQLUMUserRoleSchemaDBMapInfo> GetUserRoleSchemaDBMapInfo()
        {

            try
            {
                string query = @"DECLARE @DB_USers TABLE
                    (DBName sysname, UserName sysname, LoginType sysname, AssociatedRole varchar(max),create_date datetime,modify_date datetime)

                    INSERT @DB_USers
                    EXEC sp_MSforeachdb

                    '
                    use [?]
                    SELECT ''?'' AS DB_Name,
                    case prin.name when ''dbo'' then  (select SUSER_SNAME(owner_sid) from master.sys.databases where name =''?'')  else prin.name end AS UserName,
                    prin.type_desc AS LoginType,
                    isnull(USER_NAME(mem.role_principal_id),'''') AS AssociatedRole ,create_date,modify_date
                    FROM sys.database_principals prin
                    LEFT OUTER JOIN sys.database_role_members mem ON prin.principal_id=mem.member_principal_id
                    WHERE prin.sid IS NOT NULL and prin.sid NOT IN (0x00) and
                    prin.is_fixed_role <> 1 AND prin.name NOT LIKE ''##%'''

                    SELECT

                    user1.dbname,user1.username ,user1.logintype ,user1.create_date ,user1.modify_date , user2.associatedrole

                    FROM @DB_USers user1

                    inner join

                     @DB_USers user2

                    on

                    user1.DBName=user2.DBName AND user1.UserName=user2.UserName



                    GROUP BY

                    user1.dbname,user1.username ,user1.logintype ,user1.create_date ,user1.modify_date, user2.associatedrole


                    ORDER BY DBName,user1.username
                    ";

                MSSQLUMUserRoleSchemaDBMapInfo _MSSQLUMDatabaseInfo = null;

                IList<MSSQLUMUserRoleSchemaDBMapInfo> _list = new List<MSSQLUMUserRoleSchemaDBMapInfo>();
                IDataReader reader = null;
                try
                {

                    SqlCommand cmd = null;
                    // con.Open();
                    cmd = new SqlCommand();

                    cmd.Connection = sqlConnect;


                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.Text;

                    reader = cmd.ExecuteReader();



                    while (reader.Read())
                    {
                        _MSSQLUMDatabaseInfo = new MSSQLUMUserRoleSchemaDBMapInfo();

                        _MSSQLUMDatabaseInfo.DBName = CheckNull.AgainstString(reader["dbname"]);
                        _MSSQLUMDatabaseInfo.UserName = CheckNull.AgainstString(reader["username"]);
                        _MSSQLUMDatabaseInfo.LoginType = CheckNull.AgainstString(reader["logintype"]);

                        _MSSQLUMDatabaseInfo.RoleName = CheckNull.AgainstString(reader["associatedrole"]);
                        _MSSQLUMDatabaseInfo.EndPointSystemId = EndPoint.Id;
                        //_MSSQLUMDatabaseInfo.CreatedDate = DateTime.Now;
                        //_MSSQLUMDatabaseInfo.LastModifiedDate = DateTime.Now;

                        _list.Add(_MSSQLUMDatabaseInfo);

                    }


                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    reader.Close();
                    //  con.Close();
                }
                return _list;


            }
            catch (Exception)
            {
                throw;
            }
        }

        private void FilldicUserRoles(IList<MSSQLUMUserRoleSchemaDBMapInfo> listPrinters)
        {
            dicRoleUsers = new Dictionary<string, ArrayList>();
            foreach (var item in listPrinters)
            {
                if (!dicRoleUsers.ContainsKey(item.UserName))
                {
                    ArrayList roles = new ArrayList();
                    foreach (var roleitems in listPrinters)
                    {
                        if (roleitems.UserName == item.UserName && !string.IsNullOrEmpty(roleitems.RoleName))
                        {
                            if (!roles.Contains(roleitems.RoleName))
                            {
                                roles.Add(roleitems.RoleName);
                            }
                        }
                    }
                    dicRoleUsers.Add(item.UserName, roles);
                }
            }
        }


        //private void FilldicUserRoles(IList<MSSQLUMUserRole> listUserRoles)
        //{
        //    dicRoleUsers = new Dictionary<string, ArrayList>(); string userName = string.Empty; string roleName = string.Empty;

        //    foreach (var item in dicUsers)
        //    {
        //        long userId = item.Key;
        //        userName = item.Value;// OracleEBSUserInfo.GetByUserId(userId).USER_NAME;
        //        if (!dicRoleUsers.ContainsKey(userName))
        //        {
        //            ArrayList roles = new ArrayList();
        //            var listcurrentUser = listUserRoles.Where(c => c.MSSQLUMUserInfo_Id == userId);
        //            foreach (var roleitems in listcurrentUser)
        //            {
        //                long roleId = roleitems.MSSQLUMRoleInfo_Id;

        //                var currentRole = dicRoles.Where(c => c.Key == roleId);
        //                if (currentRole != null)
        //                {
        //                    roleName = currentRole.First().Value;
        //                    if (!string.IsNullOrEmpty(roleName) && !roles.Contains(roleName))
        //                    {
        //                        roles.Add(roleName);
        //                    }
        //                }
        //            }
        //            dicRoleUsers.Add(userName, roles);
        //        }
        //    }
        //}

        private void DiscoverDatabasePermission()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering DatabasePermission"));
            IList<MSSQLUMDatabasePermissionInfo> listPrinters = this.GetDatabasePermissions();
            if (listPrinters.Count > 0)
                MSSQLUMDatabasePermissionInfo.AddDatabasepermissionInfoBulk(listPrinters.ToList());
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering DatabasePermission"));
        }
        public IList<MSSQLUMDatabasePermissionInfo> GetDatabasePermissions()
        {
            try
            {

                string query = string.Format("select * from {0}.sys.database_permissions", Configuration(EndpointKey.InstanceName));

                MSSQLUMDatabasePermissionInfo _MSSQLUMPermission = null;
                IDataReader reader = null;

                IList<MSSQLUMDatabasePermissionInfo> _list = new List<MSSQLUMDatabasePermissionInfo>();
                try
                {

                    SqlCommand cmd = null;
                    //     con.Open();
                    cmd = new SqlCommand();
                    cmd.Connection = sqlConnect;


                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.Text;

                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        _MSSQLUMPermission = new MSSQLUMDatabasePermissionInfo();

                        _MSSQLUMPermission.GranteeId = CheckNull.AgainstInt(reader["grantee_principal_id"]);
                        _MSSQLUMPermission.GrantorId = CheckNull.AgainstInt(reader["grantor_principal_id"]);
                        _MSSQLUMPermission.PermissionsName = CheckNull.AgainstString(reader["permission_name"]);
                        _MSSQLUMPermission.PermissionsType = CheckNull.AgainstString(reader["type"]);
                        _MSSQLUMPermission.State = CheckNull.AgainstString(reader["State"]);
                        _MSSQLUMPermission.State_Desc = CheckNull.AgainstString(reader["State_desc"]);
                        //_MSSQLUMPermission.CreatedDate = DateTime.Now;
                        //_MSSQLUMPermission.LastModifiedDate = DateTime.Now;

                        _MSSQLUMPermission.EndPointSystemId = EndPoint.Id;

                        // _MSSQLUMUser.DIUserId = CheckNull.AgainstInt(reader["DIUserId"]);
                        _list.Add(_MSSQLUMPermission);

                    }

                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    reader.Close();
                    reader.Dispose();
                    // con.Close();
                    //con.Dispose();
                }
                return _list;
            }
            catch (Exception)
            {
                throw;
            }

        }


        private void DiscoverServerPermission()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering ServerPermission"));
            IList<MSSQLUMServerPermissionInfo> listPrinters = this.GetServerPermissions();
            if (listPrinters.Count > 0)
                MSSQLUMServerPermissionInfo.AddServerPermissionInfoBulk(listPrinters.ToList());
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering ServerPermission"));
        }
        public IList<MSSQLUMServerPermissionInfo> GetServerPermissions()
        {
            try
            {

                string query = string.Format("select * from {0}.sys.server_permissions", Configuration(EndpointKey.InstanceName));

                MSSQLUMServerPermissionInfo _MSSQLUMPermission = null;
                IDataReader reader = null;

                IList<MSSQLUMServerPermissionInfo> _list = new List<MSSQLUMServerPermissionInfo>();
                try
                {

                    SqlCommand cmd = null;
                    //     con.Open();
                    cmd = new SqlCommand();
                    cmd.Connection = sqlConnect;


                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.Text;

                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        _MSSQLUMPermission = new MSSQLUMServerPermissionInfo();

                        _MSSQLUMPermission.GranteeId = CheckNull.AgainstInt(reader["grantee_principal_id"]);
                        _MSSQLUMPermission.GrantorId = CheckNull.AgainstInt(reader["grantor_principal_id"]);
                        _MSSQLUMPermission.PermissionsName = CheckNull.AgainstString(reader["permission_name"]);
                        _MSSQLUMPermission.PermissionsType = CheckNull.AgainstString(reader["type"]);
                        _MSSQLUMPermission.State = CheckNull.AgainstString(reader["State"]);
                        _MSSQLUMPermission.State_Desc = CheckNull.AgainstString(reader["State_desc"]);

                        //_MSSQLUMPermission.CreatedDate = DateTime.Now;
                        //_MSSQLUMPermission.LastModifiedDate = DateTime.Now;
                        _MSSQLUMPermission.EndPointSystemId = EndPoint.Id;

                        // _MSSQLUMUser.DIUserId = CheckNull.AgainstInt(reader["DIUserId"]);
                        _list.Add(_MSSQLUMPermission);

                    }


                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    reader.Close();
                    reader.Dispose();
                    // con.Close();
                    //con.Dispose();
                }
                return _list;
            }
            catch (Exception)
            {
                throw;
            }
        }


        private void DiscoverLoginDatabaseMappingPermission()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering LoginDatabaseMappingPermission"));
            IList<MSSQLUMLoginDatabaseMapInfo> listPrinters = this.GetLoginDatabaseMapInfoInfo();
            if (listPrinters.Count > 0)
                MSSQLUMLoginDatabaseMapInfo.AddLoginDatabaseMappingInfoBulk(listPrinters.ToList());
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering LoginDatabaseMappingPermission"));
        }
        public IList<MSSQLUMLoginDatabaseMapInfo> GetLoginDatabaseMapInfoInfo()
        {
            try
            {

                string query = "";

                MSSQLUMLoginDatabaseMapInfo _MSSQLUMLoginDataMap = null;
                IDataReader reader = null;

                IList<MSSQLUMLoginDatabaseMapInfo> _list = new List<MSSQLUMLoginDatabaseMapInfo>();
                try
                {

                    SqlCommand cmd = null;
                    //     con.Open();


                    List<MSSQLUMDatabaseinfo> listDatabases = (List<MSSQLUMDatabaseinfo>)GetDatabaseInfo();

                    foreach (MSSQLUMDatabaseinfo databaseinfo in listDatabases)
                    {

                        //     con.Open();
                        try
                        {
                            cmd = new SqlCommand();
                            cmd.Connection = sqlConnect;

                            query = "select name as loginname from [" + databaseinfo.name + "].sys.syslogins";
                            cmd.CommandText = query;
                            cmd.CommandType = CommandType.Text;

                            reader = cmd.ExecuteReader();



                            while (reader.Read())
                            {
                                _MSSQLUMLoginDataMap = new MSSQLUMLoginDatabaseMapInfo();

                                _MSSQLUMLoginDataMap.DatabaseName = databaseinfo.name; // CheckNull.AgainstString(reader["DatabaseName"]);
                                _MSSQLUMLoginDataMap.LoginName = CheckNull.AgainstString(reader["LoginName"]);

                                //_MSSQLUMLoginDataMap.CreatedDate = DateTime.Now;
                                //_MSSQLUMLoginDataMap.LastModifiedDate = DateTime.Now;
                                _MSSQLUMLoginDataMap.EndPointSystemId = EndPoint.Id;
                                _list.Add(_MSSQLUMLoginDataMap);

                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        finally
                        {
                            reader.Close();
                            reader.Dispose();
                            //con.Close();
                            //con.Dispose();
                        }

                    }

                }
                catch (Exception)
                {
                    throw;
                }


                return _list;
            }
            catch (Exception)
            {
                throw;
            }
        }



        private void DiscoverSchema()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering Schema"));
            IList<MSSQLUMSchemainfo> listComputers = this.GetSchemainfo();
            if (listComputers.Count > 0)
                MSSQLUMSchemainfo.AddSchemaInfoBulk(listComputers.ToList());
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering Schema"));
        }
        public IList<MSSQLUMSchemainfo> GetSchemainfo()
        {
            try
            {

                string query = "select * from sys.schemas";

                MSSQLUMSchemainfo _MSSQLUMSchemaInfo = null;
                IDataReader reader = null;

                IList<MSSQLUMSchemainfo> _list = new List<MSSQLUMSchemainfo>();
                try
                {

                    SqlCommand cmd = null;
                    //     con.Open();
                    cmd = new SqlCommand();
                    cmd.Connection = sqlConnect;


                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.Text;

                    reader = cmd.ExecuteReader();



                    while (reader.Read())
                    {
                        _MSSQLUMSchemaInfo = new MSSQLUMSchemainfo();

                        _MSSQLUMSchemaInfo.Id = CheckNull.AgainstInt(reader["schema_id"]);
                        _MSSQLUMSchemaInfo.name = CheckNull.AgainstString(reader["name"]);
                        _MSSQLUMSchemaInfo.principal_id = CheckNull.AgainstInt(reader["principal_id"]);
                        _MSSQLUMSchemaInfo.EndPointSystemId = EndPoint.Id;
                        //_MSSQLUMSchemaInfo.CreatedDate = DateTime.Now;
                        //_MSSQLUMSchemaInfo.LastModifiedDate = DateTime.Now;
                        _list.Add(_MSSQLUMSchemaInfo);

                    }


                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    reader.Close();
                    reader.Dispose();
                    //con.Close();
                    //con.Dispose();
                }

                return _list;

            }
            catch (Exception)
            {
                throw;
            }
        }


        private void DiscoverDatabase()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering Database"));
            IList<MSSQLUMDatabaseinfo> listComputers = this.GetDatabaseInfo();
            if (listComputers.Count > 0)
                MSSQLUMDatabaseinfo.ADDdatabse(listComputers.ToList());
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering Database"));
        }
        public IList<MSSQLUMDatabaseinfo> GetDatabaseInfo()
        {
            try
            {

                string query = "select * from sys.databases";

                MSSQLUMDatabaseinfo _MSSQLUMDatabaseInfo = null;

                IList<MSSQLUMDatabaseinfo> _list = new List<MSSQLUMDatabaseinfo>();
                IDataReader reader = null;
                try
                {

                    SqlCommand cmd = null;
                    // con.Open();
                    cmd = new SqlCommand();

                    cmd.Connection = sqlConnect;


                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.Text;

                    reader = cmd.ExecuteReader();



                    while (reader.Read())
                    {
                        _MSSQLUMDatabaseInfo = new MSSQLUMDatabaseinfo();

                        _MSSQLUMDatabaseInfo.database_id = CheckNull.AgainstInt(reader["database_id"]);
                        _MSSQLUMDatabaseInfo.name = CheckNull.AgainstString(reader["name"]);
                        _MSSQLUMDatabaseInfo.Collation_Name = CheckNull.AgainstString(reader["collation_Name"]);
                        _MSSQLUMDatabaseInfo.Create_date = CheckNull.AgainstDatetime(reader["create_date"]);
                        _MSSQLUMDatabaseInfo.is_readonly = CheckNull.AgainstBit(Convert.ToBoolean(reader["is_read_only"]));
                        _MSSQLUMDatabaseInfo.owner_sid = CheckNull.AgainstString(reader["owner_sid"]);
                        _MSSQLUMDatabaseInfo.EndPointSystemId = EndPoint.Id;
                        //_MSSQLUMDatabaseInfo.CreatedDate = DateTime.Now;
                        //_MSSQLUMDatabaseInfo.LastModifiedDate = DateTime.Now;
                        _MSSQLUMDatabaseInfo.EndPointSystemId = EndPoint.Id;
                        _list.Add(_MSSQLUMDatabaseInfo);

                    }


                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    reader.Close();
                    //  con.Close();
                }
                return _list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        private void SaveUserGroups()
        {
            try
            {
                MSSQLUMUserInfo.SaveUserRoleMap(dicUsers, dicRoles, dicRoleUsers, this.EndPoint.Id);
            }
            catch (Exception ex)
            {
                return;
            }
        }
        private void UpdateEntitlementRecon()
        {
            try
            {
                foreach (KeyValuePair<long, string> item in dicUsers)
                {
                    var mssqlUserInfo = MSSQLUMUserInfo.GetByIdIncludeMSSQLRoles(item.Key);

                    if (mssqlUserInfo.MSSQLRoles != null)
                    {
                        foreach (MSSQLUMRoleInfo entitlement in mssqlUserInfo.MSSQLRoles)
                        {

                            var acctRecon = AccountReconcilation.GetByUser(mssqlUserInfo.EndPointSystemId, mssqlUserInfo.Id);
                            var entRecon = EntitlementReconcilation.GetByUser(mssqlUserInfo.Id, mssqlUserInfo.EndPointSystemId, entitlement.RoleName.Trim(), EntitlementType.Role);

                            if (entRecon != null)
                            {
                                //entRecon.LastModifiedDate = DateTime.Now;
                                //entRecon.LastModifiedBy = "System";
                                entRecon.Save(MasterConfigHelper.ServiceUser);
                            }
                            else
                            {
                                var entitlements = (mssqlUserInfo.Entitlements.Select(x => x.EntitlementName.ToString()));
                                entRecon = this.CreateEntitlementReconcilationShell(acctRecon, EntitlementType.Group, string.Join(",", entitlements != null ? entitlements.ToArray() : new string[] { }));
                                entRecon.Entitlement = entitlement.RoleName;
                                entRecon.ExceptionType = ExceptionTypes.Added;
                                entRecon.Save(MasterConfigHelper.ServiceUser);
                            }
                        }
                    }
                }

                EntitlementReconcilation.ProcessDeletedEntitlements(this.EndPoint.Id, this.Process, MasterConfigHelper.ServiceUser);
            }
            catch (Exception ex)
            {
                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while updating Entitlement reconciliation.", ex);
            }
        }
        #endregion
    }

}