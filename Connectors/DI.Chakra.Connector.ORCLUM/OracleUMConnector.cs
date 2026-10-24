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
using DI.Chakra.DomainModel;
using DI.Chakra.Logger;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using CheckNulls = DI.Chakra.DomainModel.CheckNull;
using DI.Chakra.DBConfigHelper;
#endregion
namespace DI.Chakra.Connector
{
    public class ORCLUMConnector : ConnectorBase
    {
        #region Members

        
        public EndPoint endpoint;
        private Dictionary<long, string> dicUsers = new Dictionary<long, string>();
        private Dictionary<long, string> dicRoles = new Dictionary<long, string>();
        private Dictionary<string, ArrayList> dicRoleUsers = new Dictionary<string, ArrayList>();

        private EndpointConfiguration oraUMConfig = null;
        public EndpointConfiguration oracleUMConfiguration
        {
            get { return oraUMConfig; }
            set { this.oraUMConfig = value; }
        }
        #endregion

        #region Core

        public ORCLUMConnector(EndPoint endpoint, AccessRequest currentRequest)
        {
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started OracleUMConfiguration"));
                this.EndPoint = endpoint;
                this.CurrentRequest = currentRequest;
                var oracleUMConfig = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);
                oracleUMConfig.ConfigurationDetails.ForEach(lq =>
                {
                    if (lq.KeyId == Convert.ToInt64(EndpointKey.Password))
                    {
                        lq.Value = RSAHelper.DecryptData(lq.Value);
                    }
                });
                this.oracleUMConfiguration = oracleUMConfig;
                WriteDiscoveryLog(LogLevelType.Info, string.Format("OracleUMConfiguration Established SuccessFully."));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Error occured while Establishing the Configuration (ORCLUMConnector Method)."));
            }
        }
        public override IMProcessor IMProcessor
        {
            get
            {
                ORCLUM.IMProcessor processor = new ORCLUM.IMProcessor();

                processor.endpoint = this.EndPoint;
                processor.oracleUMConfiguration = this.oracleUMConfiguration;


                return processor;
            }


        }

        #endregion

        #region Discovery

        public override bool Connect()
        {
            string oracleConnectionString = string.Empty;
            OracleConnection OrCon = null;
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Connection Establish"));
                string portNo = Configuration(EndpointKey.PortNo);
                string InstanceName = Configuration(EndpointKey.InstanceName);
                string Password = Configuration(EndpointKey.Password);
                oracleConnectionString = string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));" +
                    "User Id={3};Password={4};", Configuration(EndpointKey.IPAddress), portNo, InstanceName, Configuration(EndpointKey.UserName), Password);
                OrCon = new OracleConnection(oracleConnectionString);
                OrCon.Open();
                WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - Connection Established SuccessFully ", Configuration(EndpointKey.IPAddress)));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error occured while Connecting the Connection {0} (Connect Method) ", Configuration(EndpointKey.IPAddress)));
                return false;
            }
            finally
            {
                OrCon.Close();
                OrCon.Dispose();
            }
            return true;
        }
        private string Configuration(EndpointKey Key)
        {
            var config = oracleUMConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
            return config != null ? config.Value : "";
        }
        public override void Disconnect()
        {
            throw new NotImplementedException();
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
        public override void GetData()
        {
            try
            {
                long DiscoveryProcessId = base.DiscoveryProcess.Id;
                List<EndpointObjects> Objects = DI.Chakra.DomainModel.DiscoveryProcess.GetByDiscoveryProcessId(DiscoveryProcessId);
                bool IsRolesSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_ORCLUM_Roles) != null;
                bool IsUsersSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_ORCLUM_Users) != null;
                if (IsRolesSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - Roles discovery started ", Configuration(EndpointKey.HostName)));
                    DiscoverRoles();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - Roles discovery completed ", Configuration(EndpointKey.HostName)));

                }
                if (IsUsersSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - users discovery started ", Configuration(EndpointKey.HostName)));
                    DiscoverUsers();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - users discovery completed ", Configuration(EndpointKey.HostName)));
                }
                if (IsRolesSelected || IsUsersSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Started DiscoveredUsersRoles Method"));
                    DiscoveredUsersRoles();
                    bool isSuccess = SaveUserRoles();
                    if (isSuccess)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - Save UserRoles discovery completed ", Configuration(EndpointKey.HostName)));
                    }
                    else
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("Error occured while processing the Save UserRoles discovery Method"));
                    }
                    //Update recon table
                    this.UpdateEntitlementRecon();
                }
                //OracleUMUserInfo.MapWithMasterUser(EndPoint, oracleUMConfiguration.MapAll != null ? oracleUMConfiguration.MapAll : false);
                OracleUMUserInfo.MapWithMasterUserForReconTable(EndPoint, oracleUMConfiguration.MapAll != null ? oracleUMConfiguration.MapAll : false);
                OracleUMUserInfo.PopulateMasterUserFromOracleUM(EndPoint);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private void FilldicRoleUsers(IList<OracleUMUserRole> listUserRoles)
        {
            dicRoleUsers = new Dictionary<string, ArrayList>(); string userName = string.Empty; string roleName = string.Empty; string userNames = string.Empty;
            foreach (var item in listUserRoles)
            {
                long userId = item.UserId;
                userName = OracleUMUserInfo.GetByUserId(userId).UserName;
                if (!dicRoleUsers.ContainsKey(userName))
                {
                    ArrayList roles = new ArrayList();
                    foreach (var roleitems in listUserRoles)
                    {
                        long roleId = roleitems.RoleId; long userIds = roleitems.UserId;
                        roleName = OracleUMRoleInfo.GetByRoleId(roleId).RoleName;
                        userNames = OracleUMUserInfo.GetByUserId(userIds).UserName;
                        if (userNames == userName && !string.IsNullOrEmpty(roleName))
                        {

                            roles.Add(roleName);
                        }
                    }
                    dicRoleUsers.Add(userName, roles);
                }
            }
        }
        private bool SaveUserRoles()
        {
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started SaveUserRoles Method"));
                OracleUMUserInfo.SaveUserRoleMap(dicUsers, dicRoles, dicRoleUsers, this.EndPoint.Id);
                WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - SaveUserRoles discovery completed ", Configuration(EndpointKey.HostName)));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private void DiscoverUsers()
        {
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering Users"));
                IList<OracleUMUserInfo> discoveredUsersList = this.GetUsers();
                if (discoveredUsersList != null && discoveredUsersList.Count > 0)
                {
                    OracleUMUserInfo.SaveUsers(discoveredUsersList, EndPoint.Id, MasterConfigHelper.ServiceUser);
                    discoveredUsersList = null;
                    AccountReconcilation.ProcessDeletedAccounts(this.EndPoint.Id, this.Process, MasterConfigHelper.ServiceUser);
                    discoveredUsersList = OracleUMUserInfo.GetDeletedUsers(this.EndPoint.Id, this.Process);
                    if (discoveredUsersList != null && discoveredUsersList.Count > 0)
                    {
                        OracleUMUserInfo.RemoveOracleUMUser(discoveredUsersList.ToList());
                    }
                    dicUsers = OracleUMUserInfo.GetAllByEndPointSysId(this.EndPoint.Id);

                }
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering Users"));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error occured while processing the DiscoverUsers Method."));
                throw;
            }
        }
        private void DiscoverRoles()
        {
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering Roles"));
                IList<OracleUMRoleInfo> discoveredRolesList = new List<OracleUMRoleInfo>();
                discoveredRolesList = this.GetRoles();
                if (discoveredRolesList != null && discoveredRolesList.Count > 0)
                {
                    OracleUMRoleInfo.SaveRoles(discoveredRolesList, EndPoint.Id, MasterConfigHelper.ServiceUser);
                }
                discoveredRolesList = null;
                discoveredRolesList = OracleUMRoleInfo.GetDeletedByEndPointSystemId(this.EndPoint.Id);
                OracleUMRoleInfo.DeleteList(discoveredRolesList.ToList());
                dicRoles = OracleUMRoleInfo.GetRoleByEndpointId(this.EndPoint.Id);
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering Roles from {0}", EndPoint.DisplayName));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error occured while processing the DiscoverRoles Method."));
                throw;
            }
        }
        private void DiscoveredUsersRoles()
        {
            try
            {

                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering UsersRoles"));
                IList<OracleUMUserRole> discoverdUserRoleList = this.GetUsersRoles();
                if (discoverdUserRoleList != null && discoverdUserRoleList.Count > 0)
                {
                    OracleUMUserRole.SaveUserRoles(discoverdUserRoleList, EndPoint.Id, MasterConfigHelper.ServiceUser);
                }
                discoverdUserRoleList = null;
                discoverdUserRoleList = OracleUMUserRole.GetDeletedByEndPointSystemId(this.EndPoint.Id);
                OracleUMUserRole.DeleteList(discoverdUserRoleList.ToList());
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering UsersRoles"));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error occured while processing the UsersRoles Method."));
                throw;
            }
        }
        private IList<OracleUMUserInfo> GetUsers()
        {
            OracleUMUserInfo user = null;
            IList<OracleUMUserInfo> listUsers = new List<OracleUMUserInfo>();
            try
            {
                DataSet dsResults = getDSFromOracleQuery("SELECT * FROM DBA_USERS");
                if (dsResults.Tables.Count <= 0)
                {
                    return null;
                }
                foreach (DataRow item in dsResults.Tables[0].Rows)
                {
                    user = new OracleUMUserInfo();
                    user.EndPointSystemId = EndPoint.Id;
                    user.OraUserId = Convert.ToInt32(item["USER_ID"]);
                    user.UserName = CheckNull.AgainstString(item["USERNAME"]);
                    user.AccountStatus = CheckNull.AgainstString(item["ACCOUNT_STATUS"]);
                    user.LockAt = CheckNull.AgainstNullableDatetime(item["LOCK_DATE"]);
                    user.ExpiryAt = CheckNull.AgainstNullableDatetime(item["EXPIRY_DATE"]);
                    user.DefaultTableSpace = CheckNull.AgainstString(item["DEFAULT_TABLESPACE"]);
                    user.CreatedAt = CheckNull.AgainstNullableDatetime(item["CREATED"]);
                    user.Profile = CheckNull.AgainstString(item["PROFILE"]);
                    user.InitialRSRCConsumerGroup = CheckNull.AgainstString(item["INITIAL_RSRC_CONSUMER_GROUP"]);
                    listUsers.Add(user);
                }
                WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} Users Found.", listUsers.Count));
            }
            catch (Exception)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Failed Users Discovery in Oracle UM"));
                throw;
            }
            return listUsers;
        }
        private List<OracleUMRoleInfo> GetRoles()
        {
            OracleUMRoleInfo role = null;
            List<OracleUMRoleInfo> listRoles = new List<OracleUMRoleInfo>();
            try
            {
                DataSet dsResults = getDSFromOracleQuery("SELECT * FROM DBA_ROLES");
                if (dsResults.Tables.Count <= 0)
                {
                    return null;
                }
                foreach (DataRow item in dsResults.Tables[0].Rows)
                {
                    role = new OracleUMRoleInfo();
                    role.RoleName = CheckNull.AgainstString(item["ROLE"]);
                    role.EndPointSystemId = EndPoint.Id;
                    role.PasswordRequired = CheckNull.AgainstString(item["PASSWORD_REQUIRED"]);
                    listRoles.Add(role);

                }
                WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} Roles Found.", listRoles.Count));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Failed Roles Discovery in Oracle UM."));
                throw;
            }
            return listRoles;
        }
        private IList<OracleUMUserRole> GetUsersRoles()
        {
            OracleUMUserRole userRole = null;
            IList<OracleUMUserRole> listUserRoles = new List<OracleUMUserRole>();
            try
            {
                DataSet dsUserRoleResults = getDSFromOracleQuery("SELECT GRANTEE,GRANTED_ROLE, ADMIN_OPTION, DEFAULT_ROLE FROM DBA_ROLE_PRIVS inner JOIN DBA_Users on GRANTEE = Username ");
                if (dsUserRoleResults.Tables.Count <= 0)
                {
                    return null;
                }
                foreach (DataRow item in dsUserRoleResults.Tables[0].Rows)
                {
                    userRole = new OracleUMUserRole();
                    string roleName = CheckNull.AgainstString(item["GRANTED_ROLE"]);
                    string userName = CheckNull.AgainstString(item["GRANTEE"]);
                    userRole.UserId = OracleUMUserInfo.GetByUserName(userName).Id;
                    userRole.RoleId = OracleUMRoleInfo.GetByRoleName(roleName).Id;
                    listUserRoles.Add(userRole);
                }

                FilldicRoleUsers(listUserRoles);
                WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} Users Roles mapping found.", listUserRoles.Count));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Failed Users Roles Mapping Discovery in Oracle UM"));
                throw;
            }
            return listUserRoles;
        }
        private DataSet getDSFromOracleQuery(string strQry)
        {
            OracleConnection OrCon = null;
            OracleCommand OrCom = null;
            OracleDataAdapter OrAdapter;
            DataSet dsResults = new DataSet();
            try
            {
                string oracleConnectionString = string.Empty;
                string portNo = Configuration(EndpointKey.PortNo);
                string InstanceName = Configuration(EndpointKey.InstanceName);
                string Password = Configuration(EndpointKey.Password);
                oracleConnectionString = string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));" +
                    "User Id={3};Password={4};", Configuration(EndpointKey.IPAddress), portNo, InstanceName, Configuration(EndpointKey.UserName), Password);
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
                string oracleConnectionString = string.Empty;
                string portNo = Configuration(EndpointKey.PortNo);
                string InstanceName = Configuration(EndpointKey.InstanceName);
                string Password = Configuration(EndpointKey.Password);
                oracleConnectionString = string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));" +
                    "User Id={3};Password={4};", Configuration(EndpointKey.IPAddress), portNo, InstanceName, Configuration(EndpointKey.UserName), Password);
                OrCon = new OracleConnection(oracleConnectionString);
                OrCon.Open();
                OrCom = new OracleCommand();
                OrCom.Connection = OrCon;
                OrCom.CommandType = CommandType.Text;
                OrCom.CommandText = strQry;
                int Result = OrCom.ExecuteNonQuery();
                ReturnValue = true;
            }
            catch (Exception ex)
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
        private void UpdateEntitlementRecon()
        {
            try
            {
                foreach (KeyValuePair<long, string> item in dicUsers)
                {
                    var oracleUMUserInfo = OracleUMUserInfo.GetByIdIncludeOracleRoles(item.Key);

                    if (oracleUMUserInfo.OracleRoles != null)
                    {
                        foreach (OracleUMRoleInfo entitlement in oracleUMUserInfo.OracleRoles)
                        {
                            var acctRecon = AccountReconcilation.GetByUser(oracleUMUserInfo.EndPointSystemId, oracleUMUserInfo.Id);
                            var entRecon = EntitlementReconcilation.GetByUser(oracleUMUserInfo.Id, oracleUMUserInfo.EndPointSystemId, entitlement.RoleName.Trim(), EntitlementType.Group);

                            if (entRecon != null)
                            {
                                entRecon.Save(MasterConfigHelper.ServiceUser);
                            }
                            else
                            {
                                var entitlements = (oracleUMUserInfo.Entitlements.Select(x => x.EntitlementName.ToString()));
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