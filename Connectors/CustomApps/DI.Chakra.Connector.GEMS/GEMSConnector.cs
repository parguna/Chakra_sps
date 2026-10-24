
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DI.Chakra.Connector
{
    public class GEMSConnector : ConnectorBase
    {
        #region Constants and Variable
        private const string MSG_Connected = "{0} Host {1} Connected";
        private const string MSG_ConnectionFailed = "{0} Host {1} Connection Failed";
        private const string PROCESSNAME = "DiscoveryProcess";

        private Dictionary<long, string> dicUsers = new Dictionary<long, string>();
        private Dictionary<long, string> dicRoles = new Dictionary<long, string>();
        private string IPAddress = string.Empty;
        readonly string _masterConfigHelperServiceUser;
        #endregion
        #region Constructors
        public GEMSConnector(DI.Chakra.DomainModel.EndPoint _endpoint, AccessRequest currentRequest, IdmsLogger logger) : base(logger)
        {
            this.Endpoint = _endpoint;
            this.CurrentRequest = currentRequest;
            this.EndpointConfiguration = EndpointConfiguration.GetByEndpointSystemId(_endpoint.Id);
            this.IPAddress = Convert.ToString(Configuration(EndpointKey.ServerName));
            _masterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;
        }
        #endregion
        #region Discovery Methods
        public override bool Connect()
        {
            bool isConnected = false;
            try
            {
                using (SqlConnection sqlconn = new SqlConnection())
                {
                    sqlconn.ConnectionString = this.getConnectionString;
                    sqlconn.Open();
                    isConnected = true;
                    sqlconn.Close();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("GEMS {0} Connected", this.IPAddress));
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Exception in connecting GEMS {0}", this.IPAddress));
                return false;
            }
            return isConnected;
        }
        private string getConnectionString
        {
            get
            {
                string Credentials = "";

                if (!string.IsNullOrEmpty(Convert.ToString(Configuration(EndpointKey.UserName))) &&
                    !string.IsNullOrEmpty(Convert.ToString(Configuration(EndpointKey.Password))) &&
                     Convert.ToBoolean(Convert.ToString(Configuration(EndpointKey.IsWindowsAuthendication))) == true)
                {
                    Credentials = string.Format("User Id={0};Password={1};", Convert.ToString(Configuration(EndpointKey.UserName)), Convert.ToString(Configuration(EndpointKey.Password)));
                }
                else
                {
                    Credentials = "Integrated Security=True;";
                }

                string connStr = string.Format(GEMSQueryHelper.ConnectionStringFormatter, Convert.ToString(Configuration(EndpointKey.ServerName)), Convert.ToString(Configuration(EndpointKey.DatabaseName)), Credentials);
                return connStr;
            }
        }
        public override void Disconnect()
        {
        }
        public override void GetData()
        {
            long DiscoveryProcessId = base.DiscoveryProcess.Id;

            List<EndpointObjects> EndpointObjects = DI.Chakra.DomainModel.DiscoveryProcess.GetByDiscoveryProcessId(DiscoveryProcessId);

            bool IsUserSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_GEMS_User) != null;
            bool IsRoleSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_GEMS_Role) != null;

            try
            {
                if (IsUserSelected)
                {
                    DiscoverUsers();
                }

                if (IsRoleSelected)
                {
                    DiscoverRoles();
                }

                if (IsUserSelected && IsRoleSelected)
                {
                    DiscoverUserRole();
                }

                this.UpdateEntitlementRecon();

                GEMSUserInfo.MapWithMasterUserForReconTable(Endpoint, EndpointConfiguration.MapAll != null ? EndpointConfiguration.MapAll : false, DiscoveryProcessId);
                GEMSUserInfo.MapWithMasterUserForEntitlementReconTable(Endpoint, EndpointConfiguration.MapAll != null ? EndpointConfiguration.MapAll : false, DiscoveryProcessId);
                ProcessUserBusinessRoleMapping();
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "Discovery failed for GEMS system", ex);
                throw;
            }
            finally
            {
                GC.Collect();
            }
        }
        public void ProcessUserBusinessRoleMapping()
        {
            List<UserBusinessRoleMapping> usrRoles = new List<UserBusinessRoleMapping>();
            List<AccountEntitlementRecon> allAccounts = new List<AccountEntitlementRecon>();
            List<EntitlementReconcilation> gemsEntitlements = new List<EntitlementReconcilation>();
            List<EntitlementReconcilation> adAccount = new List<EntitlementReconcilation>();
            try
            {
                GEMSRoleMatrix.ExecuteQuery("TRUNCATE TABLE dbo.DI_M_UserBusinessRoleMapping");
                GEMSRoleMatrix.ExecuteQuery("TRUNCATE TABLE dbo.DI_GEMS_HangingRoleInfo");
                usrRoles = UserBusinessRoleMapping.GetAll();
                var gemsRoles = GEMSRoleInfo.GetAllByEndpointSystemId(this.Endpoint.Id);
                var adEnd = DomainModel.EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                var activeAccounts = AccountReconcilation.GetAllActiveByEndpointId(this.Endpoint.Id).Where(i => i.UserId.HasValue && i.UserId > 0);
                var gemsMatrix = GEMSRoleMatrix.GetAllBusinessRole(this.Endpoint.Id);
                var endpointIds = new List<long>() { this.Endpoint.Id, adEnd.Id };
                GEMSRoleMatrix matrixEntitlement = null;
                List<string> adEntitlements = new List<string>();
                List<string> data = new List<string>();
                UserBusinessRoleMapping objUsrBus = null;
                
                foreach (var activeAccount in activeAccounts)
                {
                    try
                    {
                        allAccounts = null;
                        allAccounts = EntitlementReconcilation.GetAccountEntitlementRecons(activeAccount.Id, activeAccount.UserId.Value, endpointIds, activeAccount.AccountType);

                        gemsEntitlements = allAccounts.FirstOrDefault(t => t.AccountRecon.Id == activeAccount.Id).Entitlement;
                        if (gemsEntitlements != null && gemsEntitlements.Count > 0)
                        {
                            adAccount = null;
                            adAccount = allAccounts.FirstOrDefault(i => i.AccountRecon.EndpointSystemId == adEnd.Id && i.AccountRecon.AccountName.Equals(activeAccount.AccountName,StringComparison.InvariantCultureIgnoreCase))?.Entitlement;
                            foreach (var entitlement in gemsEntitlements.Distinct())
                            {
                                
                                if (entitlement.IsDeleted) continue;
                                objUsrBus = new UserBusinessRoleMapping();
                                objUsrBus.UserId = activeAccount.UserId.Value;
                                objUsrBus.EndpointSystemId = this.Endpoint.Id;
                                objUsrBus.EndpointUserId = activeAccount.EndpointUserId;
                                objUsrBus.Id = 0;
                                objUsrBus.BusinessRole = null;
                                var gemRole = gemsRoles.FirstOrDefault(i => i.Name.Equals(entitlement.Entitlement, StringComparison.InvariantCultureIgnoreCase));
                                if (gemRole != null)
                                {   matrixEntitlement = gemsMatrix.FirstOrDefault(i => i.TechnicalRole.Trim().Equals(gemRole.Display_Name.Trim(), StringComparison.InvariantCultureIgnoreCase));
                                    if (matrixEntitlement != null)
                                    {
                                        objUsrBus.BusinessRole = matrixEntitlement.BusinessRole;
                                        if (String.IsNullOrWhiteSpace(matrixEntitlement.ADGroups))
                                        {
                                            if (usrRoles.FirstOrDefault(i =>i.UserId == objUsrBus.UserId &&  i.EndpointUserId == objUsrBus.EndpointUserId && i.BusinessRole.Equals( objUsrBus.BusinessRole,StringComparison.InvariantCultureIgnoreCase)) == null)
                                            {
                                                objUsrBus.Save("System");
                                                usrRoles.Add(objUsrBus);
                                            }
                                            continue;
                                        }

                                        if (adAccount != null && adAccount.Count > 0)
                                        {
                                            adEntitlements?.Clear();
                                            adEntitlements = matrixEntitlement.ADGroups.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                            data?.Clear();
                                            data = adEntitlements.Except(adAccount.Where(j => j.IsDeleted == false).Select(i => i.Entitlement), StringComparer.InvariantCultureIgnoreCase).ToList();
                                            if (data != null && data.Count == 0)
                                            {
                                                if (usrRoles.FirstOrDefault(i => i.UserId == objUsrBus.UserId && i.EndpointUserId == objUsrBus.EndpointUserId && i.BusinessRole.Equals(objUsrBus.BusinessRole, StringComparison.InvariantCultureIgnoreCase)) == null)
                                                {
                                                    objUsrBus.Save("System");
                                                    usrRoles.Add(objUsrBus);
                                                }
                                                
                                                continue;
                                            }
                                            else
                                            {
                                                DumpIntoHangedRoleTable(this.Endpoint.Id, activeAccount?.AccountName, entitlement?.Entitlement, matrixEntitlement?.BusinessRole, matrixEntitlement?.TechnicalRole, matrixEntitlement?.ADGroups, $"Unable to derive business role for User:{activeAccount.AccountName} and entilement: {entitlement.Entitlement}, AD entitlement not found:{string.Join(",", data)}", "System");
                                                //WriteDiscoveryLog(LogLevelType.Info, $"Unable to derive business role for User:{activeAccount.AccountName} and entilement: {entitlement.Entitlement}, AD entitlement not found");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        DumpIntoHangedRoleTable(this.Endpoint.Id, activeAccount?.AccountName, entitlement?.Entitlement, string.Empty, gemRole?.Display_Name, string.Empty, $"Matrix Technicalrole does not match with Gems DisplayName:{gemRole.Display_Name} for AccountName:{activeAccount.AccountName} & Entitlement:{entitlement.Entitlement} GemsRole:{gemRole.Name}", "System");
                                        //WriteDiscoveryLog(LogLevelType.Info, $"Matrix Technicalrole does not match with Gems DisplayName:{gemRole.Display_Name} for AccountName:{activeAccount.AccountName} & Entitlement:{entitlement.Entitlement} GemsRole:{gemRole.Name}");
                                    }
                                }
                                else
                                {
                                   
                                    WriteDiscoveryLog(LogLevelType.Info, $"Gems Role not found for AccountName:{activeAccount.AccountName} & Entitlement:{entitlement}");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteDiscoveryLog(LogLevelType.Exception, $"ProcessUserBusinessRoleMapping - Discovery Exception occured for User:{activeAccount.AccountName}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "ProcessUserBusinessRoleMapping - Discovery failed for GEMS system", ex);
                throw ex;
            }
            finally
            {
                usrRoles = null;

                allAccounts?.Clear();
                allAccounts = null;

                gemsEntitlements?.Clear();
                gemsEntitlements = null;

                adAccount?.Clear();
                allAccounts = null;
            }
        }

        private void DiscoverUsers()
        {
            IList<GEMSUserInfo> list = new List<GEMSUserInfo>();

            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started GEMS Discovering Users"),DiscoveryObjectType.User);

                list = this.GetUsers();
                GEMSUserInfo.SaveUsers(list, this.Endpoint.Id, _masterConfigHelperServiceUser);

                list = null;
                AccountReconcilation.ProcessDeletedAccounts(this.Endpoint.Id, this.Process, _masterConfigHelperServiceUser);
                list = GEMSUserInfo.GetDeletedUsers(this.Endpoint.Id, this.Process);
                GEMSUserInfo.RemoveGEMSUser(list.ToList());

                dicUsers = GEMSUserInfo.GetAllByEndPointSystemId(this.Endpoint.Id);
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed GEMS Discovering Users"), DiscoveryObjectType.User);
            }
            catch (Exception ex)
            {
                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while Discovery GEMS Users.", DiscoveryObjectType.User, ex);
                throw ex;
            }
            finally
            {
                list?.Clear();
                list = null;
            }
        }
        private void DiscoverRoles()
        {
            IList<GEMSRoleInfo> list = new List<GEMSRoleInfo>();

            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started GEMS Discovering Roles"), DiscoveryObjectType.Role);
                list = this.GetRoles();
                GEMSRoleInfo.SaveRoles(list, _masterConfigHelperServiceUser, this.Endpoint.Id);


                var listRoles = GEMSRoleInfo.GetDeletedByEndPointSystemId(this.Endpoint.Id, this.DiscoveryProcess.Id);
                GEMSRoleInfo.RemoveGEMSRoles(listRoles.ToList());
                dicRoles = GEMSRoleInfo.GetByEndpointId(this.Endpoint.Id);

                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed GEMS Discovering Roles"), DiscoveryObjectType.Role);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "Exception occured in Discovery Roles for GEMS system", DiscoveryObjectType.Role, ex);
                throw;
            }
            finally
            {
                list?.Clear();
                list = null;
            }
        }
        private void DiscoverUserRole()
        {
            var list = new List<GEMSUserRole>();
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started GEMS User Roles Mapping"), DiscoveryObjectType.UserRole);

                list = this.GetUserRoles();
                GEMSUserInfo.SaveUserRolesMap(list, this.Endpoint.Id);

                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed GEMS User Roles Mapping"), DiscoveryObjectType.UserRole);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "Exception occured in User Roles Mapping for GEMS system", DiscoveryObjectType.UserRole, ex);
                throw;
            }
            finally
            {
                list?.Clear();
                list = null;
            }
        }
        private IList<GEMSUserInfo> GetUsers()
        {
            //SqlConnection conn = null;
            //SqlCommand cmd = null;
            try
            {

                IList<GEMSUserInfo> list = new List<GEMSUserInfo>();

                GEMSUserInfo gemsUserInfo = null;

                string connStr = this.getConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;
                        cmd.CommandText = GEMSQueryHelper.SelectUserQuery;
                        cmd.Connection.Open();
                        IDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            gemsUserInfo = new GEMSUserInfo();

                            gemsUserInfo.EndPointSystemId = this.Endpoint.Id;
                            gemsUserInfo.UserId = Convert.ToInt32(CheckNull.AgainstString(reader["USER_ID"]));
                            gemsUserInfo.USER_NAME = CheckNull.AgainstString(reader["USER_NAME"]);
                            gemsUserInfo.DISPLAY_NAME = CheckNull.AgainstString(reader["DISPLAY_NAME"]);
                            gemsUserInfo.DESCRIPTION = CheckNull.AgainstString(reader["DESCRIPTION"]);
                            gemsUserInfo.EMAIL = CheckNull.AgainstString(reader["EMAIL"]);
                            gemsUserInfo.DATEMODIFIED = (reader["DATEMODIFIED"] == DBNull.Value || (reader["DATEMODIFIED"] != DBNull.Value && (Convert.ToString(reader["DATEMODIFIED"].ToString()) == "NULL" || Convert.ToString(reader["DATEMODIFIED"].ToString()) == "'NULL'"))) ? gemsUserInfo.DATEMODIFIED : CheckNull.AgainstDatetime(reader["DATEMODIFIED"]);
                            gemsUserInfo.LASTUSER = Convert.ToString(CheckNull.AgainstString(reader["LASTUSER"]));

                            list.Add(gemsUserInfo);
                        }

                        WriteDiscoveryLog(LogLevelType.Info, string.Format("Found {0} GEMS User(s)", list.Count.ToString()), DiscoveryObjectType.User);
                        return list;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, "Error occured at GEMS Discovery of GEMS GetUsers",DiscoveryObjectType.User, ex);
                throw;
            }
        }
        private IList<GEMSRoleInfo> GetRoles()
        {
            //SqlCommand cmd = null;
            try
            {

                IList<GEMSRoleInfo> list = new List<GEMSRoleInfo>();
                GEMSRoleInfo gemsRoleInfo = null;

                string connStr = this.getConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {

                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;
                        cmd.CommandText = GEMSQueryHelper.SelectRoleQuery;
                        cmd.Connection.Open();

                        IDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            gemsRoleInfo = new GEMSRoleInfo();
                            gemsRoleInfo.EndPointSystemId = this.Endpoint.Id;
                            gemsRoleInfo.RoleId = Convert.ToInt32(CheckNull.AgainstString(reader["ROLE_ID"]));
                            gemsRoleInfo.Name = CheckNull.AgainstString(reader["NAME"]);
                            gemsRoleInfo.Display_Name = CheckNull.AgainstString(reader["DISPLAY_NAME"]);
                            gemsRoleInfo.Description = CheckNull.AgainstString(reader["DESCRIPTION"]);
                            gemsRoleInfo.Extension_ID = CheckNull.AgainstNullableInt(reader["EXTENSION_ID"]);

                            list.Add(gemsRoleInfo);
                        }

                        WriteDiscoveryLog(LogLevelType.Info, string.Format("Found {0} GEMS User Role(s)", list.Count.ToString()), DiscoveryObjectType.Role);
                        return list;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, "Error occured at GEMS Discovery of GEMS Roles, Error : {0}", DiscoveryObjectType.Role, ex);
                throw;
            }
        }
        private List<GEMSUserRole> GetUserRoles()
        {
            //SqlConnection conn = null;
            //SqlCommand cmd = null;
            try
            {
                IList<GEMSUserRole> list = new List<GEMSUserRole>();
                GEMSUserRole gemsUserRole = null;

                string connStr = this.getConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {

                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;
                        cmd.CommandText = GEMSQueryHelper.SelectUserRoleQuery;
                        cmd.Connection.Open();

                        IDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            gemsUserRole = new GEMSUserRole();

                            gemsUserRole.GEMSUserInfo_Id = Convert.ToInt32(CheckNull.AgainstString(reader["USER_ID"]));
                            gemsUserRole.GEMSRoleInfo_Id = Convert.ToInt32(CheckNull.AgainstString(reader["ROLE_ID"]));
                            gemsUserRole.EndpointSystemId = this.Endpoint.Id;

                            if (gemsUserRole.GEMSUserInfo_Id != 0 && gemsUserRole.GEMSRoleInfo_Id != 0)
                            {
                                if (list.Any(p => p.GEMSRoleInfo_Id == gemsUserRole.GEMSRoleInfo_Id && p.GEMSUserInfo_Id == gemsUserRole.GEMSUserInfo_Id && p.EndpointSystemId == gemsUserRole.EndpointSystemId))
                                    continue;

                                list.Add(gemsUserRole);
                            }
                        }

                        WriteDiscoveryLog(LogLevelType.Info, string.Format("Found {0} GEMS UserRole(s)", list.Count.ToString()), DiscoveryObjectType.UserRole);

                        return list.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, "Error occured at GEMS Discovery of GEMS UserRoles", DiscoveryObjectType.UserRole, ex);
                throw;
            }
        }
        private void UpdateEntitlementRecon()
        {
            List<AccountReconcilation> exsitngAccountrecon = null;
            List<EntitlementReconcilation> existingEntilements = null;
            List<GEMSRoleMatrix> matrix = new List<GEMSRoleMatrix>();

            try
            {
                exsitngAccountrecon = AccountReconcilation.GetAllByEndpointId(this.Endpoint.Id).ToList();
                existingEntilements = EntitlementReconcilation.GetAllByEndpointSystemId(this.Endpoint.Id);
                matrix = GEMSRoleMatrix.GetAll();
                AccountReconcilation acctRecon = null;
                EntitlementReconcilation entRecon = null;
                //List<GEMSUserInfo> listAdsUserInfo = GEMSUserInfo.GetAllEndPointSystemId(this.Endpoint.Id);
              
                GEMSUserInfo gemsUserInfo = null;
                foreach (KeyValuePair<long, string> item in dicUsers)
                {
                    gemsUserInfo = null;
                    try
                    {
                        gemsUserInfo = GEMSUserInfo.GetByIdIncludeRoles(item.Key);
                        //gemsUserInfo = listAdsUserInfo.Where(a => a.Id == item.Key).FirstOrDefault();
                        string description = string.Empty;
                        if (gemsUserInfo != null && gemsUserInfo.GEMSRoles != null)
                        {
                            foreach (GEMSRoleInfo entitlement in gemsUserInfo.GEMSRoles)
                            {
                                description = string.Empty;
                                description = entitlement.Display_Name;
                                var gemRole = (matrix.FirstOrDefault(i=> i.TechnicalRole == description)?.BusinessRoleName)?? "";
                                if (!string.IsNullOrWhiteSpace(gemRole))
                                {
                                    description = $"{description}({gemRole})";
                                }
                                // acctRecon = AccountReconcilation.GetByUser(adsUserInfo.EndPointSystemId, adsUserInfo.Id);
                                acctRecon = exsitngAccountrecon.Where(a => a.EndpointUserId == gemsUserInfo.Id).FirstOrDefault();
                                if (acctRecon == null) continue;

                                //entRecon = EntitlementReconcilation.GetByUser(adsUserInfo.Id, adsUserInfo.EndPointSystemId, entitlement.name.Trim(), EntitlementType.Group);

                                entRecon = existingEntilements.Where(e => e.EndpointUserId == gemsUserInfo.Id &&
                                                                            e.Entitlement == Convert.ToString(entitlement.Name).Trim() &&
                                                                            e.EntitlementType == (int)EntitlementType.Role).FirstOrDefault();

                                if (entRecon == null)
                                {
                                    entRecon = new EntitlementReconcilation();
                                    var entitlements = (gemsUserInfo.Entitlements.Select(x => x.EntitlementName.ToString()));
                                    entRecon = this.CreateEntitlementReconcilationShell(acctRecon, EntitlementType.Role, string.Join(",", entitlements != null ? entitlements.ToArray() : new string[] { }));
                                    entRecon.Entitlement = entitlement.Name.Trim();
                                    entRecon.EntitlementId = entitlement.Id;
                                    entRecon.EntitlementDescription = description;
                                    EntitlementReconcilation.UpdateException(entRecon, true);
                                    entRecon.CreatedDate = DateTime.Now;
                                    entRecon.CreatedBy = "System";
                                    entRecon.ValidToDate = acctRecon.ValidToDate;
                                    entRecon.Save(_masterConfigHelperServiceUser);
                                    existingEntilements.Add(entRecon);
                                }
                                else
                                {
                                    entRecon.LastModifiedDate = DateTime.Now;
                                    entRecon.LastModifiedBy = "System";
                                    entRecon.Entitlement = entitlement.Name.Trim();
                                    entRecon.EntitlementId = entitlement.Id;
                                    entRecon.EntitlementDescription = description;
                                    entRecon.ValidToDate = acctRecon.ValidToDate;
                                    if (entRecon.IsDeleted)
                                    {
                                        EntitlementReconcilation.UpdateException(entRecon, true);
                                    }
                                    entRecon.Save(_masterConfigHelperServiceUser);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error while updating Entitlement reconciliation. GEMS User -{0}", gemsUserInfo.USER_NAME), ex);

                    }
                }

                try
                {
                    EntitlementReconcilation.ProcessDeletedEntitlements(this.Endpoint.Id, this.Process, _masterConfigHelperServiceUser);
                }
                catch (Exception ex)
                {
                    this.WriteDiscoveryLog(LogLevelType.Exception, "Error while ProcessDelete Entitlements Recon.", ex);
                }

            }
            catch (Exception ex)
            {
                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while Process Entitlements Recon.", ex);
            }
            finally
            {
                matrix?.Clear();
                matrix = null;

                exsitngAccountrecon?.Clear();
                exsitngAccountrecon = null;

                existingEntilements?.Clear();
                existingEntilements = null;
            }


        }
        #endregion
        #region Provisiong Methods
        public override bool ChangePassword(string userName, string newPassword, long DIUserId, bool isReactivation)
        {
            return true;
        }
        bool AccountExists(string username)
        {
            this._idmsLogger.WriteLog(LogLevelType.DebugStep, $"AccountExists : {username}");
            bool accountExists = false;
            SqlConnection conn = null;
            SqlCommand cmd = null;
            try
            {
                string connStr = this.getConnectionString;
                if (this.Connect())
                {
                    using (conn = new SqlConnection(connStr))
                    {
                        using(cmd = new SqlCommand())
                        {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@USER_NAME", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = username });

                        cmd.CommandText = GEMSQueryHelper.CheckUserLoginExistQuery;
                        cmd.Connection.Open();
                        accountExists = cmd.ExecuteScalar() != null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
            }
            
            return accountExists;
        }
        public override bool CreateAccount(IEndpointUserInfo _userInfo, ProvisioningParamCollection fields)
        {
            string UserExist = string.Empty;

            SqlConnection conn = null;
            SqlCommand cmd = null;
            if (this.Connect())
            {
                try
                {
                    GEMSUserInfo _gemsUserInfo = (GEMSUserInfo)_userInfo;

                    if (AccountExists(_gemsUserInfo.USER_NAME))
                    {
                        this._idmsLogger.WriteLog(LogLevelType.Info, string.Format("User {0} already exists in GEMS. Hence updating the user in gems", _gemsUserInfo.USER_NAME), null);
                        return UpdateAccount(_userInfo);
                    }

                    string connStr = this.getConnectionString;
                    using (conn = new SqlConnection(connStr))
                    {
                        using (cmd = new SqlCommand())
                        {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;
                        cmd.Connection.Open();

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@USER_ID", SqlDbType = SqlDbType.Int, Value = _gemsUserInfo.UserId });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@USER_NAME", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(_gemsUserInfo.USER_NAME) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@DISPLAY_NAME", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(_gemsUserInfo.DISPLAY_NAME) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@DESCRIPTION", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(_gemsUserInfo.DESCRIPTION) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@EMAIL", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(_gemsUserInfo.EMAIL) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@DATEMODIFIED", SqlDbType = SqlDbType.DateTime, Value = DateTime.Now });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@LASTUSER", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(_gemsUserInfo.LASTUSER) });

                        cmd.CommandText = GEMSQueryHelper.InsertUserQuery;


                        int success = cmd.ExecuteNonQuery();

                        this._idmsLogger.WriteLog(LogLevelType.Info, string.Format("User {0} Created Successfully in GEMS.", _gemsUserInfo.USER_NAME), null);

                        return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("The object already exists."))
                        throw;
                    this._idmsLogger.WriteLog(LogLevelType.Error, ex.Message, ex);
                    return false;
                }
                
            }
            return false;
        }
        public override bool UpdateAccount(IEndpointUserInfo _userInfo)
        {
            string UserExist = string.Empty;
            string UserStatus = string.Empty;
            SqlConnection conn = null;
            SqlCommand cmd = null;

            if (this.Connect())
            {
                try
                {
                    GEMSUserInfo gemsUserInfo = (GEMSUserInfo)_userInfo;

                    string connStr = this.getConnectionString;
                    using (conn = new SqlConnection(connStr))
                    {
                        cmd = new SqlCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@USER_NAME", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = gemsUserInfo.USER_NAME });

                        cmd.CommandText = GEMSQueryHelper.SelectUserByUserLoginQuery;
                        cmd.Connection.Open();

                        var reader = cmd.ExecuteScalar();
                        if (reader != null)
                        {
                            cmd = new SqlCommand();
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = conn;

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@DISPLAY_NAME", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(gemsUserInfo.DISPLAY_NAME) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@DESCRIPTION", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(gemsUserInfo.DESCRIPTION) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@EMAIL", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(gemsUserInfo.EMAIL) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@LASTUSER", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(gemsUserInfo.LASTUSER) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@DATEMODIFIED", SqlDbType = SqlDbType.DateTime, Value = DateTime.Now });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@USER_NAME", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(gemsUserInfo.USER_NAME) });

                            cmd.CommandText = GEMSQueryHelper.updateUserQuery;

                            int updated = cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            this._idmsLogger.WriteLog(LogLevelType.Error, String.Format("User {0} Not exists.", gemsUserInfo.USER_NAME), null);

                            return false;
                        }
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    this._idmsLogger.WriteLog(LogLevelType.Error, ex.Message, ex);
                    return false;
                }
                finally
                {
                    cmd?.Dispose();
                    cmd = null;
                }
            }
            return false;
        }
        public override bool LockAccount(IEndpointUserInfo _userInfo)
        {
            string UserExist = string.Empty;
            string UserStatus = string.Empty;
            SqlConnection conn = null;
            SqlCommand cmd = null;

            if (this.Connect())
            {
                try
                {
                    GEMSUserInfo gemsUserInfo = (GEMSUserInfo)_userInfo;

                    string connStr = this.getConnectionString;
                    using (conn = new SqlConnection(connStr))
                    {
                        cmd = new SqlCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@USER_NAME", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = gemsUserInfo.USER_NAME });

                        cmd.CommandText = GEMSQueryHelper.SelectUserByUserLoginQuery;
                        cmd.Connection.Open();

                        var reader = cmd.ExecuteScalar();
                        if (reader != null)
                        {
                            cmd = new SqlCommand();
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = conn;

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@LASTUSER", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = gemsUserInfo.LASTUSER }); // _gemsUserInfo.USER_NAME
                                                                                                                                                                               //cmd.Parameters.Add(new SqlParameter() { ParameterName = "@DATEMODIFIED", SqlDbType = SqlDbType.DateTime, Value = _gemsUserInfo.DATEMODIFIED }); 
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@USER_NAME", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = gemsUserInfo.USER_NAME });

                            cmd.CommandText = GEMSQueryHelper.LockUserQuery;

                            int locked = cmd.ExecuteNonQuery();
                            base.LockAccount(gemsUserInfo);
                            return true;
                        }
                        else
                        {
                            this._idmsLogger.WriteLog(LogLevelType.Error, String.Format("User {0} Not exists.", gemsUserInfo.USER_NAME), null);
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    this._idmsLogger.WriteLog(LogLevelType.Error, ex.Message, ex);
                    return false;
                }
                finally
                {
                    cmd?.Dispose();
                    cmd = null;
                }
            }
            return false;
        }
        public override bool UnLockAccount(IEndpointUserInfo _userInfo)
        {
            string UserExist = string.Empty;
            string UserStatus = string.Empty;
            SqlConnection conn = null;
            SqlCommand cmd = null;

            if (this.Connect())
            {
                try
                {
                    GEMSUserInfo gemsUserInfo = (GEMSUserInfo)_userInfo;

                    string connStr = this.getConnectionString;
                    using (conn = new SqlConnection(connStr))
                    {

                        cmd = new SqlCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@USER_NAME", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = gemsUserInfo.USER_NAME });

                        cmd.CommandText = GEMSQueryHelper.SelectUserByUserLoginQuery;
                        cmd.Connection.Open();

                        var reader = cmd.ExecuteScalar();
                        if (reader != null)
                        {
                            cmd = new SqlCommand();
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = conn;

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@LASTUSER", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = gemsUserInfo.LASTUSER });
                            //cmd.Parameters.Add(new SqlParameter() { ParameterName = "@DATEMODIFIED", SqlDbType = SqlDbType.DateTime, Value = _gemsUserInfo.DATEMODIFIED }); 
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@USER_NAME", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = gemsUserInfo.USER_NAME });

                            cmd.CommandText = GEMSQueryHelper.UnLockUserQuery;

                            int locked = cmd.ExecuteNonQuery();

                            base.UnLockAccount(gemsUserInfo);
                            return true;
                        }
                        else
                        {
                            this._idmsLogger.WriteLog(LogLevelType.Error, String.Format("User {0} Not exists.", gemsUserInfo.USER_NAME), null);
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    this._idmsLogger.WriteLog(LogLevelType.Error, ex.Message, ex);
                    return false;
                }
                finally
                {
                    cmd?.Dispose();
                    cmd = null;
                }
            }
            return false;
        }
        public override bool ChangeContainer(IEndpointUserInfo _userInfo)
        {
            return true;
            //SqlConnection conn = null;
            //SqlCommand cmd = null;
            //string UserExist = string.Empty;

            //GEMSUserInfo gemsUserInfo = (GEMSUserInfo)_userInfo;

            //try
            //{
            //    string connStr = this.getConnectionString;
            //    conn = new SqlConnection(connStr);
            //    cmd = new SqlCommand();
            //    cmd.CommandType = CommandType.Text;
            //    cmd.Connection = conn;

            //    cmd.Parameters.Clear();
            //    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@USER_NAME", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = gemsUserInfo.USER_NAME });

            //    cmd.CommandText = GEMSQueryHelper.CheckUserLoginExistQuery;
            //    cmd.Connection.Open();

            //    var reader = cmd.ExecuteScalar();
            //    if (reader == null)
            //    {
            //        this._idmsLogger.WriteLog(LogLevelType.Error, String.Format("User {0} Not exists.", gemsUserInfo.USER_NAME), null);
            //        return false;
            //    }
            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    this._idmsLogger.WriteLog(LogLevelType.Error, ex.Message, ex);
            //    return false;
            //}
            //finally
            //{
            //    if (conn != null)
            //    {
            //        conn.Close();
            //    }
            //}
        }
        public override bool ExtendValidity(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }
        public override bool DisableAccount(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }
        public override bool EnableAccount(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }
        public override bool DeleteAccount(IEndpointUserInfo _userInfo)
        {
            string UserExist = string.Empty;
            string UserStatus = string.Empty;
            SqlConnection conn = null;
            SqlCommand cmd = null;

            if (this.Connect())
            {
                try
                {
                    GEMSUserInfo gemsUserInfo = (GEMSUserInfo)_userInfo;

                    string connStr = this.getConnectionString;
                    using (conn = new SqlConnection(connStr))
                    {

                        cmd = new SqlCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@USER_NAME", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = gemsUserInfo.USER_NAME });

                        cmd.CommandText = GEMSQueryHelper.DeleteUserQuery;
                        cmd.Connection.Open();

                        int deleted = cmd.ExecuteNonQuery();
                        if (deleted >= 0)
                        {
                            this._idmsLogger.WriteLog(LogLevelType.Info, String.Format("User account:{0} deleted successfully.", gemsUserInfo.USER_NAME), null);
                            base.DeleteAccount(_userInfo);
                            return true;
                        }
                        else
                        {
                            this._idmsLogger.WriteLog(LogLevelType.Error, String.Format("User {0} Not exists.", gemsUserInfo.USER_NAME), null);
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    this._idmsLogger.WriteLog(LogLevelType.Error, ex.Message, ex);
                    return false;
                }
                finally
                {
                    cmd?.Dispose();
                    cmd = null;
                }
            }
            return false;
        }
        public override bool AddEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            string UserExist = string.Empty;

            GEMSUserInfo gemsUserInfo = (GEMSUserInfo)_userInfo;

            try
            {
                string connStr = this.getConnectionString;
                using (conn = new SqlConnection(connStr))
                {

                    cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conn;

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@USER_NAME", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = gemsUserInfo.USER_NAME });

                    cmd.CommandText = GEMSQueryHelper.CheckUserLoginExistQuery;
                    cmd.Connection.Open();

                    bool returnFlag = true;
                    var reader = cmd.ExecuteScalar();
                    if (reader != null)
                    {
                        //Assigning Default Roles
                        List<string> lstRoles = gemsUserInfo.Roles;
                        //IList<GEMSRoleInfo> lstRoles = _gemsUserInfo.GEMSRoles;
                        //bool returnFlag = false;
                        if (lstRoles != null)
                        {
                            foreach (string role in lstRoles)
                            {
                                cmd = new SqlCommand();
                                cmd.CommandType = CommandType.Text;
                                cmd.Connection = conn;

                                GEMSRoleInfo gemsRole = GEMSRoleInfo.GetByRoleName(role);
                                if (gemsRole != null)
                                {
                                    cmd.Parameters.Clear();
                                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@USER_ID", SqlDbType = SqlDbType.Int, Value = gemsUserInfo.UserId });
                                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@ROLE_ID", SqlDbType = SqlDbType.Int, Value = gemsRole.RoleId });

                                    cmd.CommandText = GEMSQueryHelper.CheckUserRoleExistQuery;

                                    var reader1 = cmd.ExecuteScalar();
                                    if (reader1 == null)
                                    {
                                        cmd = new SqlCommand();
                                        cmd.CommandType = CommandType.Text;
                                        cmd.Connection = conn;

                                        cmd.Parameters.Clear();
                                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@USER_ID", SqlDbType = SqlDbType.Int, Value = gemsUserInfo.UserId });
                                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@ROLE_ID", SqlDbType = SqlDbType.Int, Value = gemsRole.RoleId });
                                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@DATEMODIFIED", SqlDbType = SqlDbType.DateTime, Value = DateTime.Now }); // DateTime.Now
                                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@LASTUSER", SqlDbType = SqlDbType.NVarChar, Value = gemsUserInfo.LASTUSER });

                                        cmd.CommandText = GEMSQueryHelper.InsertUserRoleQuery;

                                        int success = cmd.ExecuteNonQuery();

                                        this._idmsLogger.WriteLog(LogLevelType.Info, string.Format("User role:{1} created Successfully in GEMS for user {0}.", gemsUserInfo.USER_NAME, role), null);
                                        returnFlag = true;
                                    }
                                    else
                                    {
                                        this._idmsLogger.WriteLog(LogLevelType.Info, string.Format("User role:{1} already exists in GEMS for user {0}.", gemsUserInfo.USER_NAME, role), null);
                                    }
                                }
                            }
                            base.AddEntitlement(_userInfo, _entitlementType);
                            return returnFlag;
                        }
                    }
                    else
                    {
                        this._idmsLogger.WriteLog(LogLevelType.Error, String.Format("User {0} Not exists.", gemsUserInfo.USER_NAME), null);
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Error, ex.Message, ex);
                return false;
            }
            finally
            {
                cmd?.Dispose();
                cmd = null;
            }
        }
        public override bool RemoveEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            string UserExist = string.Empty;

            GEMSUserInfo gemsUserInfo = (GEMSUserInfo)_userInfo;

            try
            {
                string connStr = this.getConnectionString;
                using (conn = new SqlConnection(connStr))
                {
                    cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conn;

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@USER_NAME", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = gemsUserInfo.USER_NAME });

                    cmd.CommandText = GEMSQueryHelper.CheckUserLoginExistQuery;
                    cmd.Connection.Open();

                    bool returnFlag = true;
                    var reader = cmd.ExecuteScalar();
                    if (reader != null)
                    {
                        //Assigning Default Roles
                        List<string> lstRoles = gemsUserInfo.Roles;
                        //IList<GEMSRoleInfo> lstRoles = _gemsUserInfo.GEMSRoles;
                        //bool returnFlag = false;
                        if (lstRoles != null && lstRoles.Count() > 0)
                        {
                            foreach (string role in lstRoles)
                            {
                                this._idmsLogger.WriteLog(LogLevelType.Info, $"Removing role:{role} for user {_userInfo?.AccountName}");

                                cmd = new SqlCommand();
                                cmd.CommandType = CommandType.Text;
                                cmd.Connection = conn;

                                GEMSRoleInfo gemsRole = GEMSRoleInfo.GetByRoleName(role);
                                if (gemsRole != null)
                                {
                                    this._idmsLogger.WriteLog(LogLevelType.Info, $"Removing Gems roleId:{gemsRole?.RoleId} for user {_userInfo?.AccountName}");

                                    cmd.Parameters.Clear();
                                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@USER_ID", SqlDbType = SqlDbType.Int, Value = gemsUserInfo.UserId });
                                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@ROLE_ID", SqlDbType = SqlDbType.Int, Value = gemsRole.RoleId });

                                    cmd.CommandText = GEMSQueryHelper.CheckUserRoleExistQuery;

                                    var reader1 = cmd.ExecuteScalar();
                                    if (reader1 != null)
                                    {
                                        cmd = new SqlCommand();
                                        cmd.CommandType = CommandType.Text;
                                        cmd.Connection = conn;

                                        cmd.Parameters.Clear();
                                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@USER_ID", SqlDbType = SqlDbType.Int, Value = gemsUserInfo.UserId });
                                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@ROLE_ID", SqlDbType = SqlDbType.Int, Value = gemsRole.RoleId });

                                        cmd.CommandText = GEMSQueryHelper.DeleteUserRoleQuery;
                                        //cmd.Connection.Open();

                                        int deleted = cmd.ExecuteNonQuery();
                                        if (deleted > 0)
                                        {
                                            this._idmsLogger.WriteLog(LogLevelType.Info, string.Format("User {0} Updated Successfully in GEMS.", gemsUserInfo.USER_NAME), null);

                                            returnFlag = true;
                                        }
                                        else
                                        {
                                            this._idmsLogger.WriteLog(LogLevelType.Error, String.Format("Role {0} Not exists.", role), null);

                                            returnFlag = false;
                                        }
                                    }
                                }
                            }
                            base.RemoveEntitlement(_userInfo, _entitlementType);
                            return returnFlag;
                        }
                        else
                        {
                            this._idmsLogger.WriteLog(LogLevelType.Info, String.Format("For the user {0} No valid roles available to remove, either technical role is being used as part of other business role.", _userInfo.AccountName), null);
                            return true;
                        }
                    }
                    else
                    {
                        this._idmsLogger.WriteLog(LogLevelType.Error, String.Format("User {0} Not exists.", gemsUserInfo.USER_NAME), null); return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Error, ex.Message, ex);
                return false;
            }
            finally
            {
                cmd?.Dispose();
                cmd = null;
            }
        }
        public override bool CheckEndPointAccountExists(string accountName)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            bool isUserExists = false;
            try
            {
                var user_id = int.Parse(accountName);
                string connStr = this.getConnectionString;
                using (conn = new SqlConnection(connStr))
                {
                    using (cmd = new SqlCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@USER_ID", SqlDbType = SqlDbType.Int, Value = user_id });
                        cmd.CommandText = GEMSQueryHelper.CheckUserExistQuery;
                        cmd.Connection.Open();

                        var reader = cmd.ExecuteScalar();
                        if (reader != null)
                        {
                            isUserExists = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Warning, ex.Message, ex);
                isUserExists = false;
            }
           
            return isUserExists;
        }


        #endregion

        #region Private Methods

        //insert into database through query instead of inserting through entity because there's concern newly created model could stop workflow functioning due to searilization
        private bool DumpIntoHangedRoleTable(long endpointsytemId,string accountName,string entitlement,string bussinessrole,string technicalrole,string adgroup,string reason,string user)
        {
            string query = "INSERT INTO [dbo].[DI_GEMS_HangingRoleInfo]([EndPointSystemId],[AccountName],[Entitlement] ,[BusinessRole],[TechnicalRole] ,[ADGroup], " +
                "[Reason] ,[CreatedBy],[CreatedDate],[LastModifiedBy],[LastModifiedDate]) " +
                "VALUES({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10})";

            return GEMSUserInfo.ExecuteQuery(query, endpointsytemId, accountName, entitlement, bussinessrole,  technicalrole, adgroup, reason,  user, DateTime.Now, user,DateTime.Now); ;
        }

        private bool ClearHangedRoleData()
        {
           return GEMSUserInfo.ExecuteQuery("TRUNCATE TABLE DI_GEMS_HangingRoleInfo");
        }
        #endregion
    }
}
