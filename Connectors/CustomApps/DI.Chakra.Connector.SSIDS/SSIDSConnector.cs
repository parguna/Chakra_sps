
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
    public class SSIDSConnector : ConnectorBase
    {
        readonly string MasterConfigHelperServiceUser;
        #region Constants and Variable
        private const string MSG_Connected = "{0} Host {1} Connected";
        private const string MSG_ConnectionFailed = "{0} Host {1} Connection Failed";
        private const string PROCESSNAME = "DiscoveryProcess";
        const string ACTIVE_FLAG = "Y";
        const string INACTIVE_FLAG = "N";
        #endregion

        #region private variables
        private Dictionary<long, string> dicUsers = new Dictionary<long, string>();
        private Dictionary<long, string> dicRoles = new Dictionary<long, string>();
        private string IPAddress = string.Empty;
        private string NPACencryptPassword = string.Empty;
        #endregion

        #region Constructors
        public SSIDSConnector(DI.Chakra.DomainModel.EndPoint _endpoint, AccessRequest currentRequest, IdmsLogger logger) : base(logger)
        {
            this.Endpoint = _endpoint;
            this.CurrentRequest = currentRequest;
            this.EndpointConfiguration = EndpointConfiguration.GetByEndpointSystemId(_endpoint.Id);
            this.IPAddress = Convert.ToString(Configuration(EndpointKey.IPAddress));
            MasterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;
        }
        #endregion

        #region Discovery
        public override bool Connect()
        {
            bool isConnected = false;
            try
            {
                using(SqlConnection sqlconn = new SqlConnection())
                {
                    sqlconn.ConnectionString = this.getConnectionString;
                    sqlconn.Open();
                    isConnected = true;
                    sqlconn.Close();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("SSIDS System -  {0} Connected", this.IPAddress));
                }
                
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Exception in connecting SSIDS {0}", this.IPAddress), ex);
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
                     Convert.ToBoolean(Convert.ToString(Configuration(EndpointKey.IsWindowsAuthendication))) != true)
                {
                    Credentials = string.Format("User Id={0};Password={1};", Convert.ToString(Configuration(EndpointKey.UserName)), Convert.ToString(Configuration(EndpointKey.Password)));
                }
                else
                {
                    Credentials = "Integrated Security=True;";
                }

                string connStr = string.Format(SSIDSQueryHelper.ConnectionStringFormatter, Convert.ToString(Configuration(EndpointKey.ServerName)), Convert.ToString(Configuration(EndpointKey.DatabaseName)), Credentials);
                return connStr;
            }
        }
        public override void Disconnect()
        {
        }
        string GetAccountType()
        {
            User user = null;
            if (CurrentRequest != null)
                user = User.GetById(CurrentRequest.UserId);
            else if(this.UserId.HasValue)
                user = user = User.GetById(this.UserId.Value);
            return user != null && user.FullTimeEmployee ? "INTERNAL" : "EXTERNAL";
        }
        public override void GetData()
        {
            long DiscoveryProcessId = base.DiscoveryProcess.Id;

            List<EndpointObjects> EndpointObjects = DI.Chakra.DomainModel.DiscoveryProcess.GetByDiscoveryProcessId(DiscoveryProcessId);

            bool IsUserSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_SSIDS_User) != null;
            try
            {
                if (IsUserSelected)
                {
                    DiscoverUsers();
                }

                if (this.Endpoint.ApplicationType == ApplicationType.SSIDS_GUI)
                {
                    WriteDiscoveryLog(LogLevelType.Info, "Update Entitlement Recon Started for SSIDS system");
                    this.UpdateEntitlementRecon();
                    WriteDiscoveryLog(LogLevelType.Info, "Update Entitlement Recon Completed for SSIDS system");
                }

                SSIDSUserInfo.MapWithMasterUserForReconTable(Endpoint, EndpointConfiguration.MapAll != null ? EndpointConfiguration.MapAll : false, DiscoveryProcessId);
                SSIDSUserInfo.MapWithMasterUserForEntitlementReconTable(Endpoint, EndpointConfiguration.MapAll != null ? EndpointConfiguration.MapAll : false, DiscoveryProcessId);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "Discovery failed for SSIDS system", ex);
                throw;
            }
            finally
            {
                dicUsers?.Clear();
                dicUsers = null;

                dicRoles?.Clear();
                dicRoles = null;

                EndpointObjects?.Clear();
                EndpointObjects = null;
            }
        }
        private void DiscoverUsers()
        {
            IList<SSIDSUserInfo> delusers = null;
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started SSIDS Discovering Users"), DiscoveryObjectType.User);
                IList<SSIDSUserInfo> list = new List<SSIDSUserInfo>();
                list = this.GetUsers();
                SSIDSUserInfo.SaveUsers(list, this.Endpoint.Id, MasterConfigHelperServiceUser, this.Endpoint.ApplicationType, (long)this.Endpoint.EndpointSystemTypeId);

                AccountReconcilation.ProcessDeletedAccounts(this.Endpoint.Id, this.Process, MasterConfigHelperServiceUser);
                delusers = SSIDSUserInfo.GetDeletedUsers(this.Endpoint.Id, this.Process);
                SSIDSUserInfo.RemoveSSIDSUser(delusers.ToList());

                if (this.Endpoint.ApplicationType == ApplicationType.SSIDS_SMS)
                {
                    try
                    {
                        EntitlementReconcilation.ProcessDeletedEntitlements(this.Endpoint.Id, this.Process, MasterConfigHelperServiceUser);
                    }
                    catch (Exception ex)
                    {
                        WriteDiscoveryLog(LogLevelType.Exception, "Error while ProcessDelete Entitlements Recon.", DiscoveryObjectType.User, ex);
                    }
                }

                dicUsers = SSIDSUserInfo.GetAllByEndPointSystemId(this.Endpoint.Id);
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed SSIDS Discovering Users", DiscoveryObjectType.User));
            }
            catch (Exception ex)
            {
                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while Discovery SSIDS Users.", DiscoveryObjectType.User, ex);
                throw ex;
            }
            finally
            {
                delusers = null;
            }
        }
        private IList<SSIDSUserInfo> GetUsers()
        {
            //SqlConnection conn = null;
            //SqlCommand cmd = null;
            try
            {

                IList<SSIDSUserInfo> list = new List<SSIDSUserInfo>();

                SSIDSUserInfo ssidsUserInfo = null;

                string connStr = this.getConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = conn;
                        if (this.Endpoint.ApplicationType == ApplicationType.SSIDS_GUI)
                        {
                            cmd.CommandText = SSIDSQueryHelper.GUI_SelectUserQuery;

                            cmd.Connection.Open();

                            IDataReader reader = cmd.ExecuteReader();

                            while (reader.Read())
                            {
                                ssidsUserInfo = new SSIDSUserInfo();

                                ssidsUserInfo.EndPointSystemId = this.Endpoint.Id;
                                ssidsUserInfo.User_Id = CheckNull.AgainstString(reader["emp_id"]);
                                ssidsUserInfo.UserName = CheckNull.AgainstString(reader["username"]);
                                ssidsUserInfo.Designation = CheckNull.AgainstString(reader["designation"]);
                                ssidsUserInfo.Section = CheckNull.AgainstString(reader["section"]);

                                //ssidsUserInfo.IsActive = CheckNull.AgainstString(reader["Active"]) == ACTIVE_FLAG;
                                ssidsUserInfo.LastLogon = CheckNull.AgainstNullableDatetime(reader["last_successful_login"]);

                                var status = CheckNull.AgainstString(reader["status"]);
                                if (!string.IsNullOrWhiteSpace(status) && (status.Trim().Equals("Active", StringComparison.InvariantCultureIgnoreCase)
                                    || status.Trim().Equals("Locked", StringComparison.InvariantCultureIgnoreCase))
                                )
                                {
                                    ssidsUserInfo.IsActive = true;
                                    ssidsUserInfo.AccountLocked = status.Trim().Equals("Locked", StringComparison.InvariantCultureIgnoreCase);
                                }
                                else
                                {
                                    ssidsUserInfo.IsActive = false;
                                }

                                ssidsUserInfo.Roles = new List<string>();
                                ssidsUserInfo.Roles.Add(CheckNull.AgainstString(reader["role"]));
                                list.Add(ssidsUserInfo);
                            }
                        }
                        else if (this.Endpoint.ApplicationType == ApplicationType.SSIDS_SMS)
                        {
                            cmd.CommandText = SSIDSQueryHelper.SMS_SelectUserQuery;
                            cmd.Parameters.Add(new SqlParameter() {ParameterName= "@account_type", SqlDbType = SqlDbType.VarChar,Size=10,Value="SMS" });
                            cmd.Connection.Open();

                            IDataReader reader = cmd.ExecuteReader();

                            while (reader.Read())
                            {
                                ssidsUserInfo = new SSIDSUserInfo();

                                ssidsUserInfo.EndPointSystemId = this.Endpoint.Id;
                                ssidsUserInfo.Emp_Id = CheckNull.AgainstString(reader["emp_id"]);
                                ssidsUserInfo.User_Id = CheckNull.AgainstString(reader["username"]);
                                ssidsUserInfo.UserName = CheckNull.AgainstString(reader["emp_name"]);
                                ssidsUserInfo.HandPhone = CheckNull.AgainstString(reader["handphone"]);
                                ssidsUserInfo.Designation = CheckNull.AgainstString(reader["designation"]);
                                
                                //ssidsUserInfo.IsActive = CheckNull.AgainstString(reader["Active"]) == ACTIVE_FLAG;
                                ssidsUserInfo.LastLogon = CheckNull.AgainstNullableDatetime(reader["last_sms_sent"]);
                                var status = CheckNull.AgainstString(reader["status"]);
                                if (!string.IsNullOrWhiteSpace(status) && (status.Trim().Equals("Active", StringComparison.InvariantCultureIgnoreCase)
                                    || status.Trim().Equals("Locked", StringComparison.InvariantCultureIgnoreCase))
                                )
                                {
                                    ssidsUserInfo.IsActive = true;
                                    ssidsUserInfo.AccountLocked = status.Trim().Equals("Locked", StringComparison.InvariantCultureIgnoreCase);
                                }
                                else
                                {
                                    ssidsUserInfo.IsActive = false;
                                }
                                //ssidsUserInfo.Roles = new List<string>();
                                //ssidsUserInfo.Roles.Add(CheckNull.AgainstString(reader["role"]));
                                list.Add(ssidsUserInfo);
                            }
                        }
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("Found {0} {1} User(s)", list.Count.ToString(), this.Endpoint.ApplicationType.ToString()));
                        return list;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, "Error occured at SSIDS Discovery of GetUsers", ex);
                throw;
            }
        }
        private void UpdateEntitlementRecon()
        {
            List<AccountReconcilation> exsitngAccountrecon = null;
            try
            {
                exsitngAccountrecon = AccountReconcilation.GetAllByEndpointId(this.Endpoint.Id).ToList();
                AccountReconcilation acctRecon = null;
                EntitlementReconcilation entRecon = null;
                EntitlementType entitlementType = EntitlementType.None;

                if (Endpoint.ApplicationType == ApplicationType.SSIDS_SMS)
                    entitlementType = EntitlementType.HandPhone;
                else if (Endpoint.ApplicationType == ApplicationType.SSIDS_GUI)
                    entitlementType = EntitlementType.Role;

                dicUsers = SSIDSUserInfo.GetAllByEndPointSystemId(this.Endpoint.Id);
                SSIDSUserInfo ssidsUserInfo = null;
                foreach (KeyValuePair<long, string> item in dicUsers)
                {
                    ssidsUserInfo = null;
                    try
                    {
                        ssidsUserInfo = SSIDSUserInfo.GetByIdIncludeRoles(item.Key);

                        if (ssidsUserInfo != null && ssidsUserInfo.SSIDSRolelnfos != null)
                        {
                            foreach (SSIDSRolelnfo entitlement in ssidsUserInfo.SSIDSRolelnfos)
                            {
                                // acctRecon = AccountReconcilation.GetByUser(adsUserInfo.EndPointSystemId, adsUserInfo.Id);
                                acctRecon = exsitngAccountrecon.Where(a => a.EndpointUserId == ssidsUserInfo.Id).FirstOrDefault();
                                if (acctRecon == null) continue;

                                entRecon = EntitlementReconcilation.GetEndpointUserObjectByParameter(this.Endpoint.Id, ssidsUserInfo.Id, Convert.ToString(entitlement.RoleName).Trim(),
                                                                             (int)entitlementType);
                                if (entRecon == null)
                                {
                                    entRecon = new EntitlementReconcilation();
                                    var entitlements = (ssidsUserInfo.Entitlements.Select(x => x.EntitlementName.ToString()));
                                    entRecon = this.CreateEntitlementReconcilationShell(acctRecon, entitlementType, string.Join(",", entitlements != null ? entitlements.ToArray() : new string[] { }));
                                    entRecon.Entitlement = Convert.ToString(entitlement.RoleName);
                                    entRecon.EntitlementId = entitlement.Id;
                                    entRecon.EntitlementDescription = entitlement.Description;
                                    EntitlementReconcilation.UpdateException(entRecon, true);
                                    entRecon.CreatedDate = DateTime.Now;
                                    entRecon.CreatedBy = "System";
                                    entRecon.EndpointSystemId = this.Endpoint.Id;
                                    entRecon.ValidToDate = acctRecon.ValidToDate;
                                    entRecon.Save(MasterConfigHelperServiceUser);
                                }
                                else
                                {
                                    entRecon.LastModifiedDate = DateTime.Now;
                                    entRecon.LastModifiedBy = "System";
                                    entRecon.Entitlement = Convert.ToString(entitlement.RoleName);
                                    entRecon.EntitlementId = entitlement.Id;
                                    entRecon.EntitlementDescription = entitlement.Description;
                                    entRecon.ValidToDate = acctRecon.ValidToDate;
                                    if (entRecon.IsDeleted)
                                    {
                                        EntitlementReconcilation.UpdateException(entRecon, true);
                                    }
                                    entRecon.Save(MasterConfigHelperServiceUser);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error while updating Entitlement reconciliation. SSIDS User -{0}", ssidsUserInfo.UserName), ex);
                    }
                }

                try
                {
                    EntitlementReconcilation.ProcessDeletedEntitlements(this.Endpoint.Id, this.Process, MasterConfigHelperServiceUser);
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
                exsitngAccountrecon?.Clear();
                exsitngAccountrecon = null;
            }

        }
        #endregion

        #region Provisioning
        public override bool ChangePassword(string userName, string newPassword, long DIUserId, bool isReactivation)
        {
            UnLockChangePasswordAPIMessage = $"ChangePassword does not supported.";
            _idmsLogger.WriteLog(LogLevelType.Info, $"ChangePassword does not supported.");
            return true;
        }

        void SetDefaultPassword(SqlCommand cmd, SqlConnection conn, SSIDSUserInfo ssidsUserInfo)
        {
            try
            {
                cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;

                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.NVarChar, Size = 9, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.User_Id) });

                WriteDiscoveryLog(LogLevelType.DebugStep, $"EmployeeId: {SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Emp_Id)}, UserId: {SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.User_Id)}, connectionState: {cmd.Connection.State}");
                cmd.CommandText = SSIDSQueryHelper.GUI_GET_DefaultPassword;
                var password = Convert.ToString(cmd.ExecuteScalar());
                ssidsUserInfo.Encrypt_Pwd = password;
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog( LogLevelType.Exception, ex.Message, ex);
            }
        }

        public override bool CreateAccount(IEndpointUserInfo _userInfo, ProvisioningParamCollection fields)
        {
            WriteDiscoveryLog(LogLevelType.DebugStep, "CreateAccount");
            string UserExist = string.Empty;

            SqlConnection conn = null;
            SqlCommand cmd = null;
            if (this.Connect())
            {
                try
                {

                    SSIDSUserInfo ssidsUserInfo = (SSIDSUserInfo)_userInfo;
                    this._idmsLogger.WriteLog(LogLevelType.Info, $"ADID:{ssidsUserInfo?.User_Id} Emp_Id:{ssidsUserInfo?.Emp_Id}");
                    string connStr = this.getConnectionString;
                    using (conn = new SqlConnection(connStr))
                    {
                        this._idmsLogger.WriteLog( LogLevelType.Info, "Started HR Record to Insert");
                        //HR Record 
                        cmd = new SqlCommand();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = conn;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@account_type", SqlDbType = SqlDbType.NVarChar, Size = 9, Value = GetAccountType() });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@emp_id", SqlDbType = SqlDbType.NVarChar, Size = 9, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Emp_Id) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@ad_id", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.User_Id) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@emp_name", SqlDbType = SqlDbType.NVarChar, Size = 30, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.UserName) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@designation", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Designation) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@office_tel", SqlDbType = SqlDbType.NVarChar, Size = 12, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.OfficeTelePhone) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@handphone", SqlDbType = SqlDbType.NVarChar, Size = 12, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.HandPhone) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@company", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Company) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@department", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Department) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@branch", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Branch) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@section", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Section) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@unit", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.OrgUnit) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@email", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Email_Id) });

                        cmd.CommandText = SSIDSQueryHelper.SMS_HR_InsertUserQuery;
                        cmd.Connection.Open();

                        var hrRecordSaved = ExecuteCommandWithReader(cmd);

                        if (hrRecordSaved <= 0)
                            return false;

                        if (this.Endpoint.ApplicationType == ApplicationType.SSIDS_GUI)
                        {
                            cmd = new SqlCommand();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Connection = conn;

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@emp_id", SqlDbType = SqlDbType.NVarChar, Size = 9, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Emp_Id) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.NVarChar, Size = 9, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.User_Id) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@role", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(fields["Role"]?.Value) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Updated_by", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = GetLoginIdAliasCreatedBy() });

                            WriteDiscoveryLog(LogLevelType.DebugStep, $"EmployeeId: {SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Emp_Id)}, UserId: {SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.User_Id)}, Role: {SQLCheckNULL.CheckStringAgainstNull(fields["Role"]?.Value)}, connectionState: {cmd.Connection.State}");
                            cmd.CommandText = SSIDSQueryHelper.GUI_InsertUserQuery;
                            //if (cmd.Connection.State != ConnectionState.Open)
                            //    cmd.Connection.Open();

                            var success = ExecuteCommandWithReader(cmd);
                            var accountCreated = success >= 1;
                            if (accountCreated)
                            {
                                SetDefaultPassword(cmd, conn, ssidsUserInfo);
                            }

                            return accountCreated;
                        }
                        else if (this.Endpoint.ApplicationType == ApplicationType.SSIDS_SMS)
                        {
                            cmd = new SqlCommand();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Connection = conn;

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@emp", SqlDbType = SqlDbType.NVarChar, Size = 9, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Emp_Id) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@hp", SqlDbType = SqlDbType.NVarChar, Size = 9, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.HandPhone) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@login_id", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = GetLoginIdAliasCreatedBy() });

                            WriteDiscoveryLog(LogLevelType.DebugStep, $"emp: {SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Emp_Id)}, hp: {SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.HandPhone)}, login_id: {SQLCheckNULL.CheckStringAgainstNull(CurrentRequest.Id.ToString())}, connectionState: {cmd.Connection.State}");
                            cmd.CommandText = SSIDSQueryHelper.SMS_InsertUserQuery;
                            //if (cmd.Connection.State != ConnectionState.Open)
                            //    cmd.Connection.Open();

                            var success = ExecuteCommandWithReader(cmd);

                            return success >= 1 ? true : false;
                        }


                    }
                }
                catch (Exception ex)
                {
                    this._idmsLogger.WriteLog( LogLevelType.Error, ex.Message, ex);
                    if (ex.Message.Contains("The object already exists."))
                        throw ex;
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
        public override bool UpdateAccount(IEndpointUserInfo _userInfo)
        {
            string UserExist = string.Empty;
            string UserStatus = string.Empty;
            SqlConnection conn = null;
            SqlCommand cmd = null;
            LogDebugStep("UpdateAccount 1");
            if (this.Connect())
            {
                LogDebugStep("UpdateAccount 2");
                try
                {
                    SSIDSUserInfo ssidsUserInfo = (SSIDSUserInfo)_userInfo;
                    if (ssidsUserInfo == null)
                    {
                        this._idmsLogger.WriteLog(LogLevelType.Error, "SSIDS User info is null");
                        return false;
                    }
                    this._idmsLogger.WriteLog(LogLevelType.Info, $"ADID:{ssidsUserInfo?.User_Id} Emp_Id:{ssidsUserInfo?.Emp_Id}");
                    string connStr = this.getConnectionString;
                    using (conn = new SqlConnection(connStr))
                    {
                        LogDebugStep("UpdateAccount 3");
                        cmd = new SqlCommand();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = conn;

                        LogDebugStep("UpdateAccount 4");
                        if (this.Endpoint.ApplicationType == ApplicationType.SSIDS_GUI)
                        {
                            if (ssidsUserInfo.Roles != null && ssidsUserInfo.Roles.Count > 0)
                            {
                                cmd.Parameters.Clear();
                                var role = ssidsUserInfo.Roles[0];
                                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@emp_id", SqlDbType = SqlDbType.NVarChar, Size = 9, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Emp_Id) });
                                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.NVarChar, Size = 9, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.User_Id) });
                                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@role", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(role) });
                                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Updated_by", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = GetLoginIdAliasCreatedBy() });
                                LogDebugStep("UpdateAccount 5");
                                cmd.CommandText = SSIDSQueryHelper.GUI_UpdateUserQuery;
                                if (cmd.Connection.State == ConnectionState.Closed)
                                    cmd.Connection.Open();
                                LogDebugStep("UpdateAccount 6");
                                var success = ExecuteCommandWithReader(cmd);
                                LogDebugStep("UpdateAccount 7");
                            }
                        }
                        else if (this.Endpoint.ApplicationType == ApplicationType.SSIDS_SMS)
                        {
                            if (!string.IsNullOrWhiteSpace(ssidsUserInfo.HandPhone))
                            {
                                cmd = new SqlCommand();
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Connection = conn;
                                LogDebugStep("UpdateAccount 8");
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@emp", SqlDbType = SqlDbType.NVarChar, Size = 9, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Emp_Id) });
                                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@hp", SqlDbType = SqlDbType.NVarChar, Size = 9, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.HandPhone) });
                                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@login_id", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = GetLoginIdAliasCreatedBy() });

                                LogDebugStep("UpdateAccount 9");
                                cmd.CommandText = SSIDSQueryHelper.SMS_UpdateUserQuery;
                                if (cmd.Connection.State == ConnectionState.Closed)
                                    cmd.Connection.Open();

                                LogDebugStep("UpdateAccount 10");
                                var success = ExecuteCommandWithReader(cmd);
                                LogDebugStep("UpdateAccount 11");
                            }
                        }

                        this._idmsLogger.WriteLog(LogLevelType.Info, "Started HR Record to Update");
                        //HR Record 
                        {
                            cmd = new SqlCommand();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Connection = conn;
                            LogDebugStep("UpdateAccount 12");
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@account_type", SqlDbType = SqlDbType.NVarChar, Size = 9, Value = GetAccountType() });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@emp_id", SqlDbType = SqlDbType.NVarChar, Size = 9, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Emp_Id) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@ad_id", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.UserName) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@emp_name", SqlDbType = SqlDbType.NVarChar, Size = 30, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.UserName) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@designation", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Designation) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@office_tel", SqlDbType = SqlDbType.NVarChar, Size = 12, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.OfficeTelePhone) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@handphone", SqlDbType = SqlDbType.NVarChar, Size = 12, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.HandPhone) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@company", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Company) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@department", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Department) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@branch", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Branch) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@section", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Section) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@unit", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.OrgUnit) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@email", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Email_Id) });
                            LogDebugStep("UpdateAccount 13");
                            cmd.CommandText = SSIDSQueryHelper.SMS_HR_InsertUserQuery;

                            if (cmd.Connection.State == ConnectionState.Closed)
                                cmd.Connection.Open();

                            LogDebugStep("UpdateAccount 14");
                            var success = ExecuteCommandWithReader(cmd);
                            LogDebugStep("UpdateAccount 15");
                            return success >= 1 ? true : false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    this._idmsLogger.WriteLog( LogLevelType.Error, ex.Message, ex);
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
                    SSIDSUserInfo ssidsUserInfo = (SSIDSUserInfo)_userInfo;

                    string connStr = this.getConnectionString;
                    using (conn = new SqlConnection(connStr))
                    {
                        cmd = new SqlCommand();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = conn;

                        if (this.Endpoint.ApplicationType == ApplicationType.SSIDS_GUI)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = ssidsUserInfo.User_Id });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Updated_by", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = GetLoginIdAliasCreatedBy() });
                            cmd.CommandText = SSIDSQueryHelper.GUI_LockUserQuery;
                            if (conn.State == ConnectionState.Closed)
                                cmd.Connection.Open();
                            int locked = cmd.ExecuteNonQuery();

                            if (locked > 0)
                            {
                                base.LockAccount(_userInfo);
                                base.RemoveAllEntitlements(_userInfo);
                                return true;
                            }
                            else
                            {
                                this._idmsLogger.WriteLog( LogLevelType.Error, String.Format("User {0} Not exists.", ssidsUserInfo.User_Id), null);
                                return false;
                            }
                        }
                        else if (this.Endpoint.ApplicationType == ApplicationType.SSIDS_SMS)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@as_empid", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = ssidsUserInfo.Emp_Id });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@as_vflag", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = INACTIVE_FLAG });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@login_id", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = GetLoginIdAliasCreatedBy() });

                            cmd.CommandText = SSIDSQueryHelper.SMS_LockUserQuery;
                            if (cmd.Connection.State == ConnectionState.Closed)
                                cmd.Connection.Open();

                            int locked = cmd.ExecuteNonQuery();

                            if (locked > 0)
                            {
                                base.LockAccount(_userInfo);
                                base.RemoveAllEntitlements(_userInfo);
                                return true;
                            }
                            else
                            {
                                this._idmsLogger.WriteLog( LogLevelType.Error, String.Format("User {0} Not exists.", ssidsUserInfo.User_Id), null);
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    this._idmsLogger.WriteLog( LogLevelType.Error, ex.Message, ex);
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
                    _idmsLogger.WriteLog(LogLevelType.Info, $"UnlockAccount for User:{_userInfo.AccountName} started.");
                    SSIDSUserInfo ssidsUserInfo = (SSIDSUserInfo)_userInfo;

                    string connStr = this.getConnectionString;
                    using (conn = new SqlConnection(connStr))
                    {
                        cmd = new SqlCommand();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = conn;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@account_type", SqlDbType = SqlDbType.NVarChar, Size = 9, Value = GetAccountType() });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@emp_id", SqlDbType = SqlDbType.NVarChar, Size = 9, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Emp_Id) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@ad_id", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.User_Id) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@emp_name", SqlDbType = SqlDbType.NVarChar, Size = 30, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.UserName) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@designation", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Designation) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@office_tel", SqlDbType = SqlDbType.NVarChar, Size = 12, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.OfficeTelePhone) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@handphone", SqlDbType = SqlDbType.NVarChar, Size = 12, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.HandPhone) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@company", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Company) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@department", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Department) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@branch", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Branch) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@section", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Section) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@unit", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.OrgUnit) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@email", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Email_Id) });

                        cmd.CommandText = SSIDSQueryHelper.SMS_HR_InsertUserQuery;
                        cmd.Connection.Open();

                        var hrRecordSaved = ExecuteCommandWithReader(cmd);

                        if (hrRecordSaved <= 0)
                            return false;

                        cmd = new SqlCommand();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = conn;

                        if (this.Endpoint.ApplicationType == ApplicationType.SSIDS_GUI)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = ssidsUserInfo.User_Id });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Updated_by", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = GetLoginIdAliasCreatedBy(_userInfo.AccountName) });
                            cmd.CommandText = SSIDSQueryHelper.GUI_UnLockUserQuery;
                            if (cmd.Connection.State == ConnectionState.Closed)
                                    cmd.Connection.Open();
                            int locked = cmd.ExecuteNonQuery();
                            if (locked > 0)
                            {
                                SetDefaultPassword(cmd, conn, ssidsUserInfo);
                                base.UnLockAccount(_userInfo);
                                _idmsLogger.WriteLog(LogLevelType.Exception, $"UnlockAccount for User:{_userInfo.AccountName} successfully completed.");
                                return true;
                            }
                            else
                            {
                                UnLockChangePasswordAPIMessage = String.Format("User {0} Not exists. Please contact IAM Team.", ssidsUserInfo.AccountName);
                                _idmsLogger.WriteLog(LogLevelType.Error, String.Format("User {0} Not exists.", ssidsUserInfo.User_Id), null);
                                return false;
                            }
                        }
                        else if (this.Endpoint.ApplicationType == ApplicationType.SSIDS_SMS)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@as_empid", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = ssidsUserInfo.Emp_Id });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@as_vflag", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = ACTIVE_FLAG });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@login_id", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = GetLoginIdAliasCreatedBy() });
                            cmd.CommandText = SSIDSQueryHelper.SMS_UnLockUserQuery;
                            if (cmd.Connection.State == ConnectionState.Closed)
                                cmd.Connection.Open();
                            int locked = cmd.ExecuteNonQuery();

                            if (locked > 0)
                            {
                                base.UnLockAccount(_userInfo);
                                _idmsLogger.WriteLog(LogLevelType.Info, $"UnlockAccount for User:{_userInfo.AccountName} successfully completed.");
                                return true;
                            }
                            else
                            {
                                UnLockChangePasswordAPIMessage = String.Format("User {0} does not exist. Please contact IAM Team.", ssidsUserInfo.AccountName);
                                _idmsLogger.WriteLog(LogLevelType.Error, String.Format("User {0} Not exists.", ssidsUserInfo.User_Id), null);
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, ex.Message, ex);
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
            //SqlConnection conn = null;
            //SqlCommand cmd = null;
            //string UserExist = string.Empty;

            //NPACUserInfo npacUserInfo = (NPACUserInfo)_userInfo;

            //try
            //{
            //    string connStr = this.getConnectionString;
            //    conn = new SqlConnection(connStr);
            //    cmd = new SqlCommand();
            //    cmd.CommandType = CommandType.Text;
            //    cmd.Connection = conn;

            //    cmd.Parameters.Clear();
            //    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@username", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = npacUserInfo.UserIdentity });

            //    cmd.CommandText = SSIDSQueryHelper.CheckUserLoginExistQuery;
            //    cmd.Connection.Open();

            //    var reader = cmd.ExecuteScalar();
            //    if (reader != null)
            //    {
            //        cmd = new SqlCommand();
            //        cmd.CommandType = CommandType.Text;
            //        cmd.Connection = conn;

            //        cmd.Parameters.Clear();
            //        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@section", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(npacUserInfo.Section) });
            //        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@designation", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(npacUserInfo.Designation) });
            //        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(npacUserInfo.UserIdentity) });

            //        cmd.CommandText = SSIDSQueryHelper.ReOrgUserQuery;

            //        int updated = cmd.ExecuteNonQuery();
            //    }
            //    else
            //    {
            //        this._idmsLogger.WriteLog( LogLevelType.Error, String.Format("User {0} Not exists.", npacUserInfo.UserIdentity));
            //        return false;
            //    }
            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    this._idmsLogger.WriteLog( LogLevelType.Error, ex.Message, ex);
            //    return false;
            //}
            //finally
            //{
            //    if (conn != null)
            //    {
            //        conn.Close();
            //    }
            //}

            return true;
        }
        public override bool ExtendValidity(IEndpointUserInfo _userInfo)
        {
            //string UserExist = string.Empty;
            //string UserStatus = string.Empty;
            //SqlConnection conn = null;
            //SqlCommand cmd = null;

            //if (this.Connect())
            //{
            //    try
            //    {
            //        NPACUserInfo npacUserInfo = (NPACUserInfo)_userInfo;

            //        string connStr = this.getConnectionString;
            //        conn = new SqlConnection(connStr);
            //        cmd = new SqlCommand();
            //        cmd.CommandType = CommandType.Text;
            //        cmd.Connection = conn;

            //        cmd.Parameters.Clear();
            //        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@username", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = npacUserInfo.UserIdentity });

            //        cmd.CommandText = SSIDSQueryHelper.SelectUserByUserLoginQuery;
            //        cmd.Connection.Open();

            //        var reader = cmd.ExecuteScalar();
            //        if (reader != null)
            //        {
            //            cmd = new SqlCommand();
            //            cmd.CommandType = CommandType.Text;
            //            cmd.Connection = conn;

            //            cmd.Parameters.Clear();
            //            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@invalid_date", SqlDbType = SqlDbType.DateTime, Value = npacUserInfo.Invalid_Date });
            //            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@username", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = npacUserInfo.UserIdentity });

            //            cmd.CommandText = SSIDSQueryHelper.ExtendUserQuery;

            //            int locked = cmd.ExecuteNonQuery();
            //            if (locked > 0)
            //            {
            //                base.UnLockAccount(_userInfo);
            //                return true;
            //            }
            //            else
            //            {
            //                this._idmsLogger.WriteLog( LogLevelType.Error, String.Format("User {0} Not exists.", npacUserInfo.UserIdentity), null);
            //                return false;
            //            }
            //        }
            //        else
            //        {
            //            this._idmsLogger.WriteLog( LogLevelType.Error, String.Format("User {0} Not exists.", npacUserInfo.UserIdentity), null);
            //            return false;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        this._idmsLogger.WriteLog( LogLevelType.Error, ex.Message, ex);
            //        return false;
            //    }
            //    finally
            //    {
            //        if (conn != null)
            //        {
            //            conn.Close();
            //        }
            //    }
            //}
            return true;
        }
        public override bool DisableAccount(IEndpointUserInfo _userInfo)
        {

            return true;
        }
        public override bool EnableAccount(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }
        public override bool DeleteAccount(IEndpointUserInfo _userInfo)
        {
            return true;
        }
        int ExecuteCommandWithReader(SqlCommand cmd)
        {
            string resultFromSystem = string.Empty;
            int rowsAffected = 0;
            using (var reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
            {
                rowsAffected = reader.RecordsAffected;
                if (reader.Read())
                {
                    resultFromSystem = Convert.ToString(reader[0]);
                }
            }
            try
            {
                this._idmsLogger.WriteLog(LogLevelType.Info, $"Endpoint Returning ResultFromSystem: {resultFromSystem}, RowsAffected:{rowsAffected}");
                if (!string.IsNullOrWhiteSpace(resultFromSystem) && resultFromSystem.Contains("No change in HP num",StringComparison.InvariantCultureIgnoreCase) && rowsAffected == -1)
                {
                    rowsAffected = 1;
                    this._idmsLogger.WriteLog(LogLevelType.Info, $"Setting rowsAffected = 1  because No change in HP num.");
                }
                
            }
            catch 
            {
            }
            //WriteDiscoveryLog(LogLevelType.Info, $"ResultFromSystem: {resultFromSystem}, RowsAffected:{rowsAffected}");
            return rowsAffected;
        }
        public override bool AddEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            string UserExist = string.Empty;
            string UserStatus = string.Empty;
            SqlConnection conn = null;
            SqlCommand cmd = null;

            if (this.Connect())
            {
                try
                {
                    SSIDSUserInfo ssidsUserInfo = (SSIDSUserInfo)_userInfo;
                    this._idmsLogger.WriteLog(LogLevelType.Info, $"ADID:{ssidsUserInfo?.User_Id} Emp_Id:{ssidsUserInfo?.Emp_Id}");
                    string connStr = this.getConnectionString;
                    using (conn = new SqlConnection(connStr))
                    {
                        //HR Record 
                        cmd = new SqlCommand();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = conn;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@account_type", SqlDbType = SqlDbType.NVarChar, Size = 9, Value = GetAccountType() });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@emp_id", SqlDbType = SqlDbType.NVarChar, Size = 9, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Emp_Id) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@ad_id", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.User_Id) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@emp_name", SqlDbType = SqlDbType.NVarChar, Size = 30, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.UserName) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@designation", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Designation) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@office_tel", SqlDbType = SqlDbType.NVarChar, Size = 12, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.OfficeTelePhone) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@handphone", SqlDbType = SqlDbType.NVarChar, Size = 12, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.HandPhone) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@company", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Company) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@department", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Department) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@branch", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Branch) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@section", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Section) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@unit", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.OrgUnit) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@email", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Email_Id) });

                        cmd.CommandText = SSIDSQueryHelper.SMS_HR_InsertUserQuery;
                        if (cmd.Connection.State == ConnectionState.Closed)
                            cmd.Connection.Open();

                        var hrRecordSaved = ExecuteCommandWithReader(cmd);

                        if (this.Endpoint.ApplicationType == ApplicationType.SSIDS_GUI)
                        {
                            cmd.Parameters.Clear();
                            var role = ssidsUserInfo.Roles[0];
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@emp_id", SqlDbType = SqlDbType.NVarChar, Size = 9, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Emp_Id) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.NVarChar, Size = 9, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.User_Id) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@role", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(role) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Updated_by", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = GetLoginIdAliasCreatedBy() });
                            this._idmsLogger.WriteLog(LogLevelType.Info, $"EmployeeId: {SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Emp_Id)}, UserId: {SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.User_Id)}, Role: {SQLCheckNULL.CheckStringAgainstNull(role)}");

                            cmd.CommandText = SSIDSQueryHelper.GUI_UpdateUserQuery;

                            var success = ExecuteCommandWithReader(cmd);
                            base.RemoveAllEntitlements(ssidsUserInfo);
                            base.AddEntitlement(ssidsUserInfo, _entitlementType);
                            return success >= 1 ? true : false;
                        }
                        else if (this.Endpoint.ApplicationType == ApplicationType.SSIDS_SMS)
                        {
                            cmd = new SqlCommand();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Connection = conn;

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@emp", SqlDbType = SqlDbType.NVarChar, Size = 9, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Emp_Id) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@hp", SqlDbType = SqlDbType.NVarChar, Size = 9, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.HandPhone) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@login_id", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = GetLoginIdAliasCreatedBy() });
                            this._idmsLogger.WriteLog(LogLevelType.Info, $"emp: {SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Emp_Id)}, hp: {SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.HandPhone)}, login_id: {GetLoginIdAliasCreatedBy()}");

                            cmd.CommandText = SSIDSQueryHelper.SMS_UpdateUserQuery;

                            var success = ExecuteCommandWithReader(cmd);
                            base.RemoveAllEntitlements(ssidsUserInfo);
                            base.AddEntitlement(ssidsUserInfo, _entitlementType);
                            return success >= 1 ? true : false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    this._idmsLogger.WriteLog( LogLevelType.Error, ex.Message, ex);
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
        public override bool RemoveEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            string UserExist = string.Empty;
            string UserStatus = string.Empty;
            SqlConnection conn = null;
            SqlCommand cmd = null;

            if (this.Connect())
            {
                try
                {
                    SSIDSUserInfo ssidsUserInfo = (SSIDSUserInfo)_userInfo;

                    string connStr = this.getConnectionString;
                    using (conn = new SqlConnection(connStr))
                    {

                        cmd = new SqlCommand();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = conn;

                        if (this.Endpoint.ApplicationType == ApplicationType.SSIDS_GUI)
                        {
                            cmd.Parameters.Clear();
                            var role = ssidsUserInfo.Roles[0];
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@emp_id", SqlDbType = SqlDbType.NVarChar, Size = 9, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.User_Id) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.NVarChar, Size = 9, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.User_Id) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@role", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(role) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Updated_by", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = GetLoginIdAliasCreatedBy() });

                            cmd.CommandText = SSIDSQueryHelper.GUI_UpdateUserQuery;
                            if (cmd.Connection.State == ConnectionState.Closed)
                                cmd.Connection.Open();

                            var success = ExecuteCommandWithReader(cmd);
                            base.RemoveEntitlement(ssidsUserInfo, _entitlementType);
                            return success >= 1 ? true : false;
                        }
                        else if (this.Endpoint.ApplicationType == ApplicationType.SSIDS_SMS)
                        {
                            cmd = new SqlCommand();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Connection = conn;

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@emp", SqlDbType = SqlDbType.NVarChar, Size = 9, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.User_Id) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@hp", SqlDbType = SqlDbType.NVarChar, Size = 9, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.HandPhone) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@login_id", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = GetLoginIdAliasCreatedBy() });


                            cmd.CommandText = SSIDSQueryHelper.SMS_UpdateUserQuery;
                            if (cmd.Connection.State == ConnectionState.Closed)
                                cmd.Connection.Open();

                            var success = ExecuteCommandWithReader(cmd);

                            base.RemoveEntitlement(ssidsUserInfo, _entitlementType);
                            return success >= 1 ? true : false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    this._idmsLogger.WriteLog( LogLevelType.Error, ex.Message, ex);
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
        public override bool RemoveAllEntitlements(IEndpointUserInfo _userInfo)
        {
            return true;
        }
        public override bool UpdateUserIdentity(string oldUserId, string newUserId)
        {
            return true;
        }

        private string GetLoginIdAliasCreatedBy(string userName = null)
        {
            if(this.CurrentRequest != null)
            {
                return $"IDMS:{this.CurrentRequest.Id.ToString()}";
            }
            else
            {
                return $"IDMS:SelfService by{userName}";
            }
            
        }
        #endregion
        /// <summary>
        /// Used for Selfservice account to unlock
        /// </summary>
        /// <param name="_userInfo"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public override bool UnLockAccount(IEndpointUserInfo _userInfo, long userId)
        {
            string UserExist = string.Empty;
            string UserStatus = string.Empty;
            SqlConnection conn = null;
            SqlCommand cmd = null;

            if (this.Connect())
            {
                try
                {
                    var user = User.GetById(userId);
                    var accountType = user != null && user.FullTimeEmployee ? "INTERNAL" : "EXTERNAL";

                    _idmsLogger.WriteLog(LogLevelType.Info, $"UnlockAccount for User:{_userInfo.AccountName} started.");
                    SSIDSUserInfo ssidsUserInfo = (SSIDSUserInfo)_userInfo;

                    string connStr = this.getConnectionString;
                    using (conn = new SqlConnection(connStr))
                    {
                        cmd = new SqlCommand();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = conn;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@account_type", SqlDbType = SqlDbType.NVarChar, Size = 9, Value = accountType });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@emp_id", SqlDbType = SqlDbType.NVarChar, Size = 9, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Emp_Id) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@ad_id", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.User_Id) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@emp_name", SqlDbType = SqlDbType.NVarChar, Size = 30, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.UserName) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@designation", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Designation) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@office_tel", SqlDbType = SqlDbType.NVarChar, Size = 12, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.OfficeTelePhone) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@handphone", SqlDbType = SqlDbType.NVarChar, Size = 12, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.HandPhone) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@company", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Company) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@department", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Department) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@branch", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Branch) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@section", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Section) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@unit", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.OrgUnit) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@email", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(ssidsUserInfo.Email_Id) });

                        cmd.CommandText = SSIDSQueryHelper.SMS_HR_InsertUserQuery;
                        cmd.Connection.Open();

                        var hrRecordSaved = ExecuteCommandWithReader(cmd);

                        if (hrRecordSaved <= 0)
                            return false;

                        cmd = new SqlCommand();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = conn;

                        if (this.Endpoint.ApplicationType == ApplicationType.SSIDS_GUI)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = ssidsUserInfo.User_Id });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Updated_by", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = GetLoginIdAliasCreatedBy(_userInfo.AccountName) });
                            cmd.CommandText = SSIDSQueryHelper.GUI_UnLockUserQuery;
                            if (cmd.Connection.State == ConnectionState.Closed)
                                cmd.Connection.Open();
                            int locked = cmd.ExecuteNonQuery();
                            if (locked > 0)
                            {
                                SetDefaultPassword(cmd, conn, ssidsUserInfo);
                                base.UnLockAccount(_userInfo);
                                _idmsLogger.WriteLog(LogLevelType.Exception, $"UnlockAccount for User:{_userInfo.AccountName} successfully completed.");
                                return true;
                            }
                            else
                            {
                                UnLockChangePasswordAPIMessage = String.Format("User {0} Not exists. Please contact IAM Team.", ssidsUserInfo.AccountName);
                                _idmsLogger.WriteLog(LogLevelType.Error, String.Format("User {0} Not exists.", ssidsUserInfo.User_Id), null);
                                return false;
                            }
                        }
                        else if (this.Endpoint.ApplicationType == ApplicationType.SSIDS_SMS)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@as_empid", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = ssidsUserInfo.Emp_Id });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@as_vflag", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = ACTIVE_FLAG });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@login_id", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = GetLoginIdAliasCreatedBy() });
                            cmd.CommandText = SSIDSQueryHelper.SMS_UnLockUserQuery;
                            if (cmd.Connection.State == ConnectionState.Closed)
                                cmd.Connection.Open();
                            int locked = cmd.ExecuteNonQuery();

                            if (locked > 0)
                            {
                                base.UnLockAccount(_userInfo);
                                _idmsLogger.WriteLog(LogLevelType.Info, $"UnlockAccount for User:{_userInfo.AccountName} successfully completed.");
                                return true;
                            }
                            else
                            {
                                UnLockChangePasswordAPIMessage = String.Format("User {0} does not exist. Please contact IAM Team.", ssidsUserInfo.AccountName);
                                _idmsLogger.WriteLog(LogLevelType.Error, String.Format("User {0} Not exists.", ssidsUserInfo.User_Id), null);
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, ex.Message, ex);
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
    }
}
