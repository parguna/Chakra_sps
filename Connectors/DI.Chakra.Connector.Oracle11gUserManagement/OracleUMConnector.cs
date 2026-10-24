#region NameSpace
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Configuration;
using System.Data.OracleClient;

using DI.Chakra.Facade;
using DI.Chakra.Infrastructure;
using DI.Chakra.Model;

using DI.Chakra.DataAccess;
using DI.Chakra.Logger;
using DI.Chakra.Infrastructure.Enums;

#endregion

namespace DI.Chakra.Connector
{
    public class OracleUMConnector : ConnectorBase
    {
        public OracleUMConfiguration OracleUMConfiguration { get; set; }

        public ORCLUMFacade OracleUserManagementFacade;
        private ORCLUMDA oracleum_DA;

        #region Constructor
        /// <summary>
        /// constructor
        /// </summary>
        public OracleUMConnector(EndPoint _endpoint)
        {
            this.OracleUMConfiguration = (OracleUMConfiguration)new EndPointFacade().GetConfiguration(_endpoint);
            EndPoint = _endpoint;
            OracleUserManagementFacade = new ORCLUMFacade();
            oracleum_DA = new ORCLUMDA();
        }
        #endregion

        public OracleUMConfiguration OracleUMConfiguraiton
        {
            get
            {
                return oracleum_DA.GetConfiguration(EndPoint.Id);
            }
        }

        #region IConnector Members

        public override bool Connect()
        {

            string oracleConnectionString = "";

            OracleConnection OrCon = null;
            try
            {

                string portNo = "";
                string InstanceName = "";
                string Password = RSAHelper.DecryptData(OracleUMConfiguraiton.Password);

                if (OracleUMConfiguraiton.PortNo != 0)
                {
                    portNo = OracleUMConfiguraiton.PortNo.ToString();
                }
                else
                {
                    portNo = "1521";
                }
                if (OracleUMConfiguraiton.InstanceName != null && OracleUMConfiguraiton.InstanceName != "")
                {
                    InstanceName = OracleUMConfiguraiton.InstanceName;
                }
                else
                {
                    InstanceName = "Oracle";
                }

                oracleConnectionString = string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));" +
                    "User Id={3};Password={4};", OracleUMConfiguraiton.IPAddress, portNo, InstanceName, OracleUMConfiguraiton.UserName, Password);
                OrCon = new OracleConnection(oracleConnectionString);
                OrCon.Open();
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                OrCon.Close();
                OrCon.Dispose();
            }
            return true;
        }

        public override void AddUnmappedUsers(int _processId)
        {
            try
            {
                EndpointObjectDA da = new EndpointObjectDA();
                da.AddAllUnmappedUsers(Convert.ToString(EndPoint.EndPointSystemType.EndpointSystemTypeName), EndPoint.Id, _processId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public override void GetData()
        {
            try
            {

                IList<OracleUMUserInfo> listOracleUsers_Existing = OracleUserManagementFacade.GetAllUsers();
                OracleUMUserInfo _objUser = null;


                EndpointObjectDA eDA = new EndpointObjectDA();
                eDA.ExecuteQry("DELETE DI_ORA_USERROLE");
                eDA.ExecuteQry("DELETE DI_ORA_ROLEINFO");
                eDA.ExecuteQry("DELETE DI_ORA_UserInfo");
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Discovery Process Started Successfully for {0}.", OracleUMConfiguraiton.HostName));

                IList<OracleUMRoleInfo> roles = getRoles();
                if (roles != null)
                {

                    //EntityRepository<OracleUMRoleInfo> erRole = new EntityRepository<OracleUMRoleInfo>();

                    foreach (OracleUMRoleInfo item in roles)
                    {
                        //erRole.Create(item);
                        OracleUserManagementFacade.AddOracleRoleInfo(item);
                    }
                }
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Roles Discovery Process Completed Successfully for {0}.", OracleUMConfiguraiton.HostName));
                IList<OracleUMTableInfo> tables = getTables();
                if (tables != null)
                {
                    //EntityRepository<OracleUMTableInfo> erTable = new EntityRepository<OracleUMTableInfo>();

                    foreach (OracleUMTableInfo item in tables)
                    {
                        //erTable.Create(item);
                        OracleUserManagementFacade.AddOracleTableInfo(item);
                    }
                }

                IList<OracleUMUserInfo> users = getUsers(roles, null);

                //EntityRepository<OracleUMUserInfo> erUser = new EntityRepository<OracleUMUserInfo>();
                oracleum_DA.DelUserRole();

                IList<OracleUMUserInfo> ors = null;

                foreach (OracleUMUserInfo item in users)
                {
                    ors = null;

                    ors = listOracleUsers_Existing.Where(u => u.UserName.ToUpper() == item.UserName.ToUpper()).ToList();

                    if (ors != null && ors.Count > 0)
                    {
                        _objUser = ors.First();
                        item.DIUserId = _objUser.DIUserId;
                        item.CreatedBy = _objUser.CreatedBy;
                        item.CreatedDate = _objUser.CreatedDate;
                    }
                    OracleUserManagementFacade.AddOracleUserInfo(item, ors == null ? true : false);
                    OracleUserManagementFacade.SaveUserRole(item.OracleUserRoles, (int)item.EndPointSystemId);
                }
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Users Discovery Process Completed Successfully for {0}.", OracleUMConfiguraiton.HostName));
                WriteDiscoveryLog(LogLevelType.Info, string.Format("User Role Mapping Process Completed Successfully for {0}.", OracleUMConfiguraiton.HostName));

            }
            catch (Exception ex)
            {

                throw;
            }



        }
        public override void Disconnect()
        {
            throw new NotImplementedException();
        }
        #endregion

        private IList<OracleUMUserInfo> getUsers(IList<OracleUMRoleInfo> _roles, IList<OracleUMTableInfo> _tables)
        {
            OracleUMUserInfo user = null;
            IList<OracleUMUserInfo> listUsers = new List<OracleUMUserInfo>();

            try
            {

                DataSet dsResults = getDSFromOracleQuery("SELECT * FROM DBA_USERS");

                DataSet dsUserRoleResults = getDSFromOracleQuery("SELECT * FROM DBA_ROLE_PRIVS");

                if (dsResults.Tables.Count <= 0)
                    return null;

                foreach (DataRow item in dsResults.Tables[0].Rows)
                {
                    user = new OracleUMUserInfo();
                    user.EndPointSystemId = EndPoint.Id;
                    //user.UserId = Convert.ToInt32(item["User_Id"]);
                    user.TenantId = EndPoint.TenantId;
                    user.UserName = CheckNull.AgainstString(item["USERNAME"]);
                    user.AccountStatus = CheckNull.AgainstString(item["ACCOUNT_STATUS"]);
                    user.LockAt = CheckNull.AgainstNullableDatetime(item["LOCK_DATE"]);
                    user.ExpiryAt = CheckNull.AgainstNullableDatetime(item["EXPIRY_DATE"]);
                    user.DefaultTableSpace = CheckNull.AgainstString(item["DEFAULT_TABLESPACE"]);
                    user.CreatedAt = CheckNull.AgainstNullableDatetime(item["CREATED"]);
                    user.Profile = CheckNull.AgainstString(item["PROFILE"]);
                    user.InitialRSRCConsumerGroup = CheckNull.AgainstString(item["INITIAL_RSRC_CONSUMER_GROUP"]);
                    user.AuthenticationType = CheckNull.AgainstString(item["AUTHENTICATION_TYPE"]);
                    user.CreatedBy = Constants.CreatedBy;
                    user.CreatedDate = DateTime.Now;
                    user.LastModifiedBy = "System";
                    user.LastModifiedDate = DateTime.Now;
                    user.OracleUserRoles = new List<OracleUMUserRole>();
                    DataRow[] dRows = dsUserRoleResults.Tables[0].Select(string.Format("GRANTEE='{0}'", user.UserName));

                    string roleName = string.Empty;

                    if (dRows.Count() > 0)
                    {
                        foreach (DataRow dR in dRows)
                        {
                            roleName = CheckNull.AgainstString(dR["GRANTED_ROLE"]);


                            OracleUMRoleInfo listOraRole = _roles.Where(t => t.RoleName.ToUpper() == roleName).FirstOrDefault();

                            if (listOraRole == null)
                                continue;
                            OracleUMUserRole userrole = new OracleUMUserRole();
                            //foreach (OracleUMRoleInfo ur in listOraRole)
                            //{
                            userrole.UserId = user.Id;
                            userrole.RoleId = listOraRole.Id;
                            user.OracleUserRoles.Add(userrole);
                            // user.OracleRoles.Add(listOraRole);
                            //}

                            // user.RolesInfo.Add(listOraRole.First());
                        }
                    }

                    //IList<OracleUMTableInfo> userTables = _tables.Where(y => y.Owner.ToUpper() == user.UserName.ToUpper()).ToList();

                    //foreach (OracleUMTableInfo itemTable in userTables)
                    //{
                    //    user.Tables.Add(itemTable);
                    //}    

                    listUsers.Add(user);
                }
                WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} Users Found.", listUsers.Count));

            }
            catch (Exception)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Failed Users Discovery in Oracle UM"));
                throw;
            }
            finally
            {

            }


            return listUsers;
        }

        private IList<OracleUMRoleInfo> getRoles()
        {
            OracleUMRoleInfo role = null;
            IList<OracleUMRoleInfo> listRoles = new List<OracleUMRoleInfo>();

            try
            {

                DataSet dsResults = getDSFromOracleQuery("SELECT * FROM DBA_ROLES");

                if (dsResults.Tables.Count <= 0)
                    return null;

                foreach (DataRow item in dsResults.Tables[0].Rows)
                {
                    role = new OracleUMRoleInfo();
                    role.RoleName = CheckNull.AgainstString(item["ROLE"]);
                    role.EndPointSystemId = EndPoint.Id;
                    role.TenantId = EndPoint.TenantId;
                    role.PasswordRequired = CheckNull.AgainstString(item["PASSWORD_REQUIRED"]);
                    role.AuthenticationType = CheckNull.AgainstString(item["AUTHENTICATION_TYPE"]);

                    role.CreatedBy = Constants.CreatedBy;
                    role.CreatedDate = DateTime.Now;
                    role.LastModifiedBy = "System";
                    role.LastModifiedDate = DateTime.Now;

                    listRoles.Add(role);
                }
                WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} Roles Found.", listRoles.Count));
            }
            catch (Exception)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Failed Roles Discovery in Oracle UM."));
                throw;
            }
            finally
            {

            }


            return listRoles;
        }

        private IList<OracleUMTableInfo> getTables()
        {
            OracleUMTableInfo Table = null;
            IList<OracleUMTableInfo> listTables = new List<OracleUMTableInfo>();

            try
            {

                DataSet dsResults = getDSFromOracleQuery("SELECT * FROM ALL_ALL_TABLES");

                if (dsResults.Tables.Count <= 0)
                    return null;

                foreach (DataRow item in dsResults.Tables[0].Rows)
                {
                    Table = new OracleUMTableInfo();
                    Table.TableName = CheckNull.AgainstString(item["TABLE_NAME"]);
                    Table.TableSpaceName = CheckNull.AgainstString(item["TABLESPACE_NAME"]);
                    Table.Owner = CheckNull.AgainstString(item["OWNER"]);
                    Table.EndPointSystemId = (int)EndPoint.Id;
                    Table.TenantId = EndPoint.TenantId;

                    Table.CreatedBy = Constants.CreatedBy;
                    Table.CreatedDate = DateTime.Now;
                    Table.LastModifiedBy = "System";
                    Table.LastModifiedDate = DateTime.Now;


                    if (listTables.Count() >= 200)
                        break;
                    listTables.Add(Table);
                }

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

            }


            return listTables;
        }

        private DataSet getDSFromOracleQuery(string strQry)
        {
            OracleConnection OrCon = null;
            OracleCommand OrCom = null;
            OracleDataAdapter OrAdapter;
            DataSet dsResults = new DataSet();
            try
            {

                string oracleConnectionString = "";
                string portNo = "";
                string InstanceName = "";
                string Password = RSAHelper.DecryptData(OracleUMConfiguraiton.Password);
                if (OracleUMConfiguraiton.PortNo != 0)
                {
                    portNo = OracleUMConfiguraiton.PortNo.ToString();
                }
                else
                {
                    portNo = "1521";
                }
                if (OracleUMConfiguraiton.InstanceName != null && OracleUMConfiguraiton.InstanceName != "")
                {
                    InstanceName = OracleUMConfiguraiton.InstanceName;
                }
                else
                {
                    InstanceName = "Oracle";
                }

                oracleConnectionString = string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));" +
                    "User Id={3};Password={4};", OracleUMConfiguraiton.IPAddress, portNo, InstanceName, OracleUMConfiguraiton.UserName, Password);

                //oracleConnectionString = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1})))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME={2})));User Id={3};Password={4};", OracleUMConfiguration.HostName, OracleUMConfiguration.PortNo, OracleUMConfiguration.InstanceName, OracleUMConfiguration.UserName, OracleUMConfiguration.Password);
                OrCon = new OracleConnection(oracleConnectionString);

                OrCon.Open();
                OrCom = new OracleCommand();
                OrCom.Connection = OrCon;
                OrCom.CommandType = CommandType.Text;
                OrCom.CommandText = strQry;
                OrAdapter = new OracleDataAdapter(OrCom.CommandText, OrCom.Connection);
                OrAdapter.Fill(dsResults);
            }
            catch (Exception)
            {
                if (OrCon.State == ConnectionState.Open)
                {
                    OrCon.Close();
                    OrCon.Dispose();
                }
            }

            return dsResults;
        }

        // \A/\z (not ^/$) so a value ending in a trailing newline can't sneak past the
        // anchors - .NET's $ also matches just before a final "\n", which ^...+$ would
        // otherwise accept even though the trailing "\n" itself was never validated.
        private static readonly System.Text.RegularExpressions.Regex SafeOracleIdentifier = new System.Text.RegularExpressions.Regex(@"\A[A-Za-z0-9_$#]+\z", System.Text.RegularExpressions.RegexOptions.Compiled);

        private static string ValidateOracleDdlToken(string value, string paramName)
        {
            if (string.IsNullOrEmpty(value) || !SafeOracleIdentifier.IsMatch(value))
            {
                throw new ArgumentException($"Invalid value supplied for '{paramName}'. Only letters, digits, '_', '$' and '#' are allowed.");
            }
            return value;
        }

        private bool putOracleQueryToDB(string strQry)
        {
            bool ReturnValue = false;
            OracleConnection OrCon = null;
            OracleCommand OrCom = null;
            try
            {

                string oracleConnectionString = "";
                string portNo = "";
                string InstanceName = "";
                string Password = RSAHelper.DecryptData(OracleUMConfiguraiton.Password);
                if (OracleUMConfiguraiton.PortNo != 0)
                {
                    portNo = OracleUMConfiguraiton.PortNo.ToString();
                }
                else
                {
                    portNo = "1521";
                }
                if (OracleUMConfiguraiton.InstanceName != null && OracleUMConfiguraiton.InstanceName != "")
                {
                    InstanceName = OracleUMConfiguraiton.InstanceName;
                }
                else
                {
                    InstanceName = "Oracle";
                }

                oracleConnectionString = string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));" +
                    "User Id={3};Password={4};", OracleUMConfiguraiton.IPAddress, portNo, InstanceName, OracleUMConfiguraiton.UserName, Password);

                //oracleConnectionString = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1})))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME={2})));User Id={3};Password={4};", OracleUMConfiguration.HostName, OracleUMConfiguration.PortNo, OracleUMConfiguration.InstanceName, OracleUMConfiguration.UserName, OracleUMConfiguration.Password);
                OrCon = new OracleConnection(oracleConnectionString);

                OrCon.Open();
                OrCom = new OracleCommand();
                OrCom.Connection = OrCon;
                OrCom.CommandType = CommandType.Text;
                OrCom.CommandText = strQry;
                int Result = OrCom.ExecuteNonQuery();

                ReturnValue = true;

            }
            catch (Exception)
            {
                if (OrCon.State == ConnectionState.Open)
                {
                    OrCon.Close();
                    OrCon.Dispose();
                }
            }
            finally
            {
                if (OrCon.State == ConnectionState.Open)
                {
                    OrCon.Close();
                    OrCon.Dispose();
                }
            }

            return ReturnValue;
        }

        public override bool ChangePassword(string UserName, string Password, long DIUserId, bool isReactivation)
        {
            bool ReturnValue = false;
            try
            {
                string CreateLoginQuery = String.Format("ALTER USER {0} IDENTIFIED BY {1}",
                    ValidateOracleDdlToken(UserName, nameof(UserName)), ValidateOracleDdlToken(Password, nameof(Password)));
                putOracleQueryToDB(CreateLoginQuery);
                ReturnValue = true;
            }
            catch (Exception)
            {
                throw;
            }
            return ReturnValue;
        }

        public override bool CreateUser(IEndpointUserInfo _userInfo, IProvisioningForm _formInfo)
        {
            bool ReturnValue = false;
            try
            {
                OracleUMUserInfo oracleUMUserInfo = (OracleUMUserInfo)_userInfo;
                string CreateLoginQuery = String.Format("CREATE USER {0} IDENTIFIED BY {1}",
                    ValidateOracleDdlToken(oracleUMUserInfo.UserName, "UserName"), ValidateOracleDdlToken(oracleUMUserInfo.AccountPassword, "AccountPassword"));
                ReturnValue = putOracleQueryToDB(CreateLoginQuery);
                ORCLUMDA OraDa = new ORCLUMDA();
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(CurrentRequest, LogLevelType.Exception, string.Format("Users Creation Failed in Oracle UM."), ex);
                throw;
            }
            return ReturnValue;
        }

        public override bool AddEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            bool ReturnValue = false;
            try
            {
                string CreateQuery = "";
                OracleUMUserInfo oracleUMUserInfo = (OracleUMUserInfo)_userInfo;
                foreach (string Role in oracleUMUserInfo.Roles)
                {
                    CreateQuery = String.Format("GRANT {0} TO {1}", ValidateOracleDdlToken(Role, "Role"), ValidateOracleDdlToken(oracleUMUserInfo.UserName, "UserName"));
                    putOracleQueryToDB(CreateQuery);
                }
                ReturnValue = true;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(CurrentRequest, LogLevelType.Exception, string.Format("Add Entitlement Failed in Oracle UM."), ex);
                throw;
            }
            return ReturnValue;
        }

        public override bool RemoveEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            bool ReturnValue = false;
            try
            {
                string CreateQuery = "";
                OracleUMUserInfo oracleUMUserInfo = (OracleUMUserInfo)_userInfo;
                foreach (string Role in oracleUMUserInfo.Roles)
                {
                    CreateQuery = String.Format("REVOKE {0} FROM {1}", ValidateOracleDdlToken(Role, "Role"), ValidateOracleDdlToken(oracleUMUserInfo.UserName, "UserName"));
                    putOracleQueryToDB(CreateQuery);
                }
                ReturnValue = true;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(CurrentRequest,  LogLevelType.Exception, string.Format("Remove Entitlement Failed in Oracle UM."),ex);
                throw;
            }
            return ReturnValue;
        }

        public override bool DeleteUser(IEndpointUserInfo _userInfo)
        {
            bool ReturnValue = false;
            try
            {
                OracleUMUserInfo oracleUMUserInfo = (OracleUMUserInfo)_userInfo;
                // string CreateLoginQuery = String.Format("DROP USER {0} CASCADE", oracleUMUserInfo.UserName);
                string CreateLoginQuery = String.Format("ALTER USER {0} ACCOUNT LOCK", ValidateOracleDdlToken(oracleUMUserInfo.UserName, "UserName"));
                ReturnValue = putOracleQueryToDB(CreateLoginQuery);
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(CurrentRequest,  LogLevelType.Exception, string.Format("User Deletion Failed in Oracle UM."),ex);
                throw;
            }
            return ReturnValue;
        }

        public override bool LockUser(IEndpointUserInfo _userInfo)
        {
            bool ReturnValue = false;
            try
            {
                OracleUMUserInfo oracleUMUserInfo = (OracleUMUserInfo)_userInfo;
                string CreateLoginQuery = String.Format("ALTER USER {0} ACCOUNT LOCK", ValidateOracleDdlToken(oracleUMUserInfo.UserName, "UserName"));
                ReturnValue = putOracleQueryToDB(CreateLoginQuery);
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(CurrentRequest,  LogLevelType.Exception, string.Format("User Lock Failed in Oracle UM."),ex);
                throw;
            }
            return ReturnValue;
        }

        public override bool UnLockUser(IEndpointUserInfo _userInfo)
        {
            bool ReturnValue = false;
            try
            {
                OracleUMUserInfo oracleUMUserInfo = (OracleUMUserInfo)_userInfo;
                // string CreateLoginQuery = String.Format("DROP USER {0} CASCADE", oracleUMUserInfo.UserName);
                string CreateLoginQuery = String.Format("ALTER USER {0} ACCOUNT UNLOCK", ValidateOracleDdlToken(oracleUMUserInfo.UserName, "UserName"));
                ReturnValue = putOracleQueryToDB(CreateLoginQuery);
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(CurrentRequest,  LogLevelType.Exception, string.Format("User Unlock Failed in Oracle UM."),ex);
                throw;
            }
            return ReturnValue;
        }

        public override bool ChangeContainer(IEndpointUserInfo _userInfo)
        {
            return false;
        }

        public override bool UpdateUser(IEndpointUserInfo _userInfo)
        {
            return false;
        }

        public override bool ExtendValidity(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }

    }
}