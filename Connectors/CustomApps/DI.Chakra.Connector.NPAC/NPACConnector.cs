
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
    public class NPACConnector : ConnectorBase
    {
        #region Constants and Variable
        private const string MSG_Connected = "{0} Host {1} Connected";
        private const string MSG_ConnectionFailed = "{0} Host {1} Connection Failed";
        private const string PROCESSNAME = "DiscoveryProcess";
        #endregion

        #region private variables
        private Dictionary<long, string> dicUsers = new Dictionary<long, string>();
        private Dictionary<long, string> dicRoles = new Dictionary<long, string>();
        private string IPAddress = string.Empty;
        private string NPACencryptPassword = string.Empty;
        #endregion

        #region Constructors

        readonly string MasterConfigHelperServiceUser;
        public NPACConnector(DI.Chakra.DomainModel.EndPoint _endpoint, AccessRequest currentRequest, IdmsLogger logger) : base(logger)
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
                using (SqlConnection sqlconn = new SqlConnection())
                {
                    sqlconn.ConnectionString = this.getConnectionString;
                    sqlconn.Open();
                    isConnected = true;
                    sqlconn.Close();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("NPAC System -  {0} Connected", this.IPAddress));
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Exception in connecting NPAC {0}", this.IPAddress));
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

                string connStr = string.Format(NPACQueryHelper.ConnectionStringFormatter, Convert.ToString(Configuration(EndpointKey.ServerName)), Convert.ToString(Configuration(EndpointKey.DatabaseName)), Credentials);
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

            bool IsUserSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_NPAC_User) != null;
            try
            {
                if (IsUserSelected)
                {
                    DiscoverSections();
                    DiscoverUsers();

                    this.UpdateEntitlementRecon();
                }



                NPACUserInfo.MapWithMasterUserForReconTable(Endpoint, EndpointConfiguration.MapAll != null ? EndpointConfiguration.MapAll : false, DiscoveryProcessId);
                NPACUserInfo.MapWithMasterUserForEntitlementReconTable(Endpoint, EndpointConfiguration.MapAll != null ? EndpointConfiguration.MapAll : false, DiscoveryProcessId);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "Discovery failed for NPAC system", ex);
                throw;
            }
            finally
            {
                GC.Collect();
            }
        }
        private void DiscoverUsers()
        {
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started NPAC Discovering Users"), DiscoveryObjectType.User);
                IList<NPACUserInfo> list = new List<NPACUserInfo>();
                IList<NPACUserInfo> delNPACUsrs = null;
                list = this.GetUsers();
                NPACUserInfo.SaveUsers(list, this.Endpoint.Id, MasterConfigHelperServiceUser);

                AccountReconcilation.ProcessDeletedAccounts(this.Endpoint.Id, this.Process, MasterConfigHelperServiceUser);
                delNPACUsrs = NPACUserInfo.GetDeletedUsers(this.Endpoint.Id, this.Process);
                NPACUserInfo.RemoveNPACUser(delNPACUsrs.ToList());

                dicUsers = NPACUserInfo.GetAllByEndPointSystemId(this.Endpoint.Id);
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed NPAC Discovering Users"), DiscoveryObjectType.User);
            }
            catch (Exception ex)
            {
                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while Discovery NPAC Users.", DiscoveryObjectType.User, ex);
                throw ex;
            }
        }

        private void DiscoverSections()
        {
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started NPAC Discovering Sections"), DiscoveryObjectType.Section);
                IList<NPACSectionInfo> list = new List<NPACSectionInfo>();
                list = this.GetSections();
                NPACSectionInfo.saveSections(list, this.Endpoint.Id, MasterConfigHelperServiceUser);
                var npacSections = NPACSectionInfo.GetDeletedUsers(this.Endpoint.Id, this.Process);
                NPACSectionInfo.DeleteList(npacSections);
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed NPAC Discovering Sections"), DiscoveryObjectType.Section);
            }
            catch (Exception ex)
            {
                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while Discovery NPAC Sections.", DiscoveryObjectType.Section, ex);
                throw ex;
            }
        }

        private IList<NPACUserInfo> GetUsers()
        {
            //SqlConnection conn = null;
            //SqlCommand cmd = null;
            try
            {

                IList<NPACUserInfo> list = new List<NPACUserInfo>();

                NPACUserInfo npacUserInfo = null;

                string connStr = this.getConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;
                        cmd.CommandText = NPACQueryHelper.SelectUserQuery;
                        cmd.Connection.Open();

                        IDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            npacUserInfo = new NPACUserInfo();

                            npacUserInfo.EndPointSystemId = this.Endpoint.Id;
                            npacUserInfo.Emp_Id = CheckNull.AgainstString(reader["emp_id"]).Trim();
                            npacUserInfo.UserIdentity = CheckNull.AgainstString(reader["userid"]).Trim();
                            npacUserInfo.UserName = CheckNull.AgainstString(reader["username"]).Trim();
                            npacUserInfo.Passwds = CheckNull.AgainstString(reader["passwds"]).Trim();
                            npacUserInfo.Designation = CheckNull.AgainstString(reader["designation"]).Trim();
                            npacUserInfo.Phone = CheckNull.AgainstString(reader["phone"]).Trim();
                            npacUserInfo.Pager = CheckNull.AgainstString(reader["pager"]).Trim();
                            npacUserInfo.Fax = CheckNull.AgainstString(reader["fax"]).Trim();
                            npacUserInfo.Groups = CheckNull.AgainstString(reader["groups"]).Trim();
                            npacUserInfo.Levels = CheckNull.AgainstNullableInt(reader["levels"]);
                            npacUserInfo.Section = CheckNull.AgainstString(reader["section"]).Trim();
                            npacUserInfo.Grade = CheckNull.AgainstString(reader["grade"]).Trim();
                            npacUserInfo.Zone = CheckNull.AgainstString(reader["zone"]).Trim();
                            npacUserInfo.Create_Date = (reader["create_date"] == DBNull.Value || (reader["create_date"] != DBNull.Value && (Convert.ToString(reader["create_date"].ToString()) == "NULL" || Convert.ToString(reader["create_date"].ToString()) == "'NULL'"))) ? npacUserInfo.Create_Date : CheckNull.AgainstDatetime(reader["create_date"]);
                            npacUserInfo.Invalid_Date = (reader["invalid_date"] == DBNull.Value || (reader["invalid_date"] != DBNull.Value && (Convert.ToString(reader["invalid_date"].ToString()) == "NULL" || Convert.ToString(reader["invalid_date"].ToString()) == "'NULL'"))) ? npacUserInfo.Invalid_Date : CheckNull.AgainstDatetime(reader["invalid_date"]);
                            npacUserInfo.Encrypt_Pwd = CheckNull.AgainstString(reader["encrypt_pwd"]);
                            npacUserInfo.Valid_Date = (reader["valid_date"] == DBNull.Value || (reader["valid_date"] != DBNull.Value && (Convert.ToString(reader["valid_date"].ToString()) == "NULL" || Convert.ToString(reader["valid_date"].ToString()) == "'NULL'"))) ? npacUserInfo.Valid_Date : CheckNull.AgainstDatetime(reader["valid_date"]);
                            npacUserInfo.UserADID = CheckNull.AgainstString(reader["user_adid"]).Trim();
                            npacUserInfo.EmailId = CheckNull.AgainstString(reader["email"]).Trim();
                            npacUserInfo.CompanyName = CheckNull.AgainstString(reader["company"]).Trim();
                            npacUserInfo.SectionName = CheckNull.AgainstString(reader["section_name"]).Trim();

                            list.Add(npacUserInfo);
                        }

                        WriteDiscoveryLog(LogLevelType.Info, string.Format("Found {0} NPAC User(s)", list.Count.ToString()), DiscoveryObjectType.User);
                        return list;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, "Error occured at NPAC Discovery of NPAC GetUsers", DiscoveryObjectType.User, ex);
                throw;
            }
        }

        private IList<NPACSectionInfo> GetSections()
        {
            //SqlConnection conn = null;
            //SqlCommand cmd = null;
            try
            {

                IList<NPACSectionInfo> list = new List<NPACSectionInfo>();

                NPACSectionInfo npacsecinfo = null;

                string connStr = this.getConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {

                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;
                        cmd.CommandText = NPACQueryHelper.SelectSectionsQuery;
                        cmd.Connection.Open();

                        IDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            npacsecinfo = new NPACSectionInfo();

                            npacsecinfo.EndPointSystemId = this.Endpoint.Id;
                            npacsecinfo.NPACSection = CheckNull.AgainstString(reader["mcode"]);
                            npacsecinfo.HRSectionName = CheckNull.AgainstString(reader["mdesc"]);

                            if (list.Any(p => p.EndPointSystemId == npacsecinfo.EndPointSystemId && p.NPACSection == npacsecinfo.NPACSection && p.HRSectionName == npacsecinfo.HRSectionName))
                                continue;

                            list.Add(npacsecinfo);
                        }
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("Found {0} NPAC Section(s)", list.Count.ToString()), DiscoveryObjectType.Section);
                        return list;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, "Error occured at NPAC Discovery of NPAC GetSections", DiscoveryObjectType.Section, ex);
                throw ex;
            }
        }
        private void UpdateEntitlementRecon()
        {
            List<AccountReconcilation> exsitngAccountrecon = null;
            List<EntitlementReconcilation> existingEntilements = null;
            try
            {
                exsitngAccountrecon = AccountReconcilation.GetAllByEndpointId(this.Endpoint.Id).Where(i=> i.IsDeleted == false).ToList();
                existingEntilements = EntitlementReconcilation.GetAllByEndpointSystemId(this.Endpoint.Id);
                AccountReconcilation acctRecon = null;
                EntitlementReconcilation entRecon = null;

                dicUsers = NPACUserInfo.GetAllByEndPointSystemId(this.Endpoint.Id);
                NPACUserInfo npacUserInfo = null;
                foreach (KeyValuePair<long, string> item in dicUsers)
                {
                    npacUserInfo = null;
                    try
                    {

                        npacUserInfo = NPACUserInfo.GetByIdIncludeLevels(item.Key);

                        if (npacUserInfo != null && npacUserInfo.NPACLevelInfos != null)
                        {
                            foreach (NPACLevelInfo entitlement in npacUserInfo.NPACLevelInfos)
                            {
                                acctRecon = null;
                                var accounts = exsitngAccountrecon.Where(a => a.AccountName == npacUserInfo.AccountName);
                                if(accounts != null && accounts.Count() == 1)
                                {
                                    acctRecon = accounts.FirstOrDefault();
                                }
                                else if(accounts != null && accounts.Count() > 1)
                                {
                                    if(accounts.Any(i=> i.IsDeleted == false))
                                    {
                                        acctRecon = accounts.FirstOrDefault(i => i.IsDeleted == false);
                                    }
                                    else
                                    {
                                        acctRecon = accounts.FirstOrDefault();
                                    }
                                }
                                if (acctRecon == null) continue;

                                entRecon = existingEntilements.Where(e => e.AccountName == npacUserInfo.AccountName &&
                                                                            e.Entitlement == Convert.ToString(entitlement.Level).Trim() &&
                                                                            e.EntitlementType == (int)EntitlementType.Level).FirstOrDefault();

                                if (entRecon == null)
                                {
                                    entRecon = new EntitlementReconcilation();
                                    var entitlements = (npacUserInfo.Entitlements.Select(x => x.EntitlementName.ToString()));
                                    entRecon = this.CreateEntitlementReconcilationShell(acctRecon, EntitlementType.Level, string.Join(",", entitlements != null ? entitlements.ToArray() : new string[] { }));
                                    entRecon.Entitlement = Convert.ToString(entitlement.Level);
                                    entRecon.EntitlementId = entitlement.Id;
                                    entRecon.EntitlementDescription = entitlement.Description;
                                    EntitlementReconcilation.UpdateException(entRecon, true);
                                    entRecon.CreatedDate = DateTime.Now;
                                    entRecon.CreatedBy = "System";
                                    entRecon.EndpointSystemId = this.Endpoint.Id;
                                    entRecon.EndpointUserId = npacUserInfo.Id;
                                    entRecon.ValidToDate = acctRecon.ValidToDate;
                                    entRecon.Save(MasterConfigHelperServiceUser);
                                    existingEntilements.Add(entRecon);
                                }
                                else
                                {
                                    entRecon.ValidToDate = acctRecon.ValidToDate;
                                    entRecon.LastModifiedDate = DateTime.Now;
                                    entRecon.LastModifiedBy = "System";
                                    entRecon.Entitlement = Convert.ToString(entitlement.Level);
                                    entRecon.EntitlementId = entitlement.Id;
                                    entRecon.EndpointUserId = npacUserInfo.Id;
                                    entRecon.EntitlementDescription = entitlement.Description;
                                    entRecon.AccountReconcilationId = acctRecon.Id;
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
                        this.WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error while updating Entitlement reconciliation. NPAC User -{0}", npacUserInfo.UserName), ex);
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
                existingEntilements?.Clear();
                existingEntilements = null;
            }
        }
        #endregion

        #region Provisioning
        public override bool ChangePassword(string userName, string newPassword, long DIUserId, bool isReactivation)
        {
            var endpointUser = NPACUserInfo.GetByUserIdentity(userName);
            if (endpointUser == null)
            {
                return false;
            }
            if (this.Connect())
            {
                //bool passwordChanged = false;
                //SqlConnection conn = null;
                SqlCommand cmd = null;

                try
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"ChangePassword for User:{userName} started.");
                    string connStr = this.getConnectionString;
                    using (SqlConnection conn = new SqlConnection(connStr))
                    {

                        cmd = new SqlCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@username", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = userName }); // _npacUserInfo.UserIdentity

                        cmd.CommandText = NPACQueryHelper.SelectUserByUserLoginQuery;
                        cmd.Connection.Open();

                        var reader = cmd.ExecuteScalar();
                        if (reader != null)
                        {
                            cmd = new SqlCommand();
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = conn;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@encrypt_pwd", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(newPassword) });
                            cmd.CommandText = NPACQueryHelper.GetEncryptPasswordQuery;

                            var encPw = cmd.ExecuteScalar(); // ExecuteNonQuery();
                            NPACencryptPassword = string.Empty; // string 
                            if (encPw != null)
                                NPACencryptPassword = encPw.ToString();

                            cmd = new SqlCommand();
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = conn;

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@encrypt_pwd", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = NPACencryptPassword }); // Password
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@username", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = userName });
                            if (IsThroughPasswordReset)
                            {
                                endpointUser.Invalid_Date = null;
                                endpointUser.Valid_Date = DateTime.Now;
                                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@valid_date", SqlDbType = SqlDbType.DateTime, Value = endpointUser.Valid_Date });
                                cmd.CommandText = NPACQueryHelper.ChangePasswordQueryForPasswordReset;

                            }
                            else
                            {
                                cmd.CommandText = NPACQueryHelper.ChangePasswordQuery;
                            }

                            int changed = cmd.ExecuteNonQuery();

                            if (changed > 0)
                            {
                                endpointUser.Encrypt_Pwd = NPACencryptPassword;
                                endpointUser.LastModifiedBy = "System";
                                endpointUser.LastModifiedDate = DateTime.Now;
                                endpointUser.Save("System");
                            }
                            _idmsLogger.WriteLog(LogLevelType.Info, $"ChangePassword for User:{userName} successfully completed.");
                            return true;
                        }
                        else
                        {
                            UnLockChangePasswordAPIMessage = $"User {userName} does not exist.";
                            this._idmsLogger.WriteLog(LogLevelType.Error, String.Format("User {0} Not exist.", userName), null);
                            return false;
                        }
                    }
                    //return passwordChanged;
                }
                catch (Exception ex)
                {
                    UnLockChangePasswordAPIMessage = $"Connnector Error. Please contact IAM Team.";
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
        public override bool CreateAccount(IEndpointUserInfo _userInfo, ProvisioningParamCollection fields)
        {
            string UserExist = string.Empty;

            //SqlConnection conn = null;
            SqlCommand cmd = null;
            if (this.Connect())
            {
                try
                {

                    NPACUserInfo _npacUserInfo = (NPACUserInfo)_userInfo;

                    string connStr = this.getConnectionString;
                    using (SqlConnection conn = new SqlConnection(connStr))
                    {


                        cmd = new SqlCommand();

                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@encrypt_pwd", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(_npacUserInfo.Encrypt_Pwd) });
                        cmd.CommandText = NPACQueryHelper.GetEncryptPasswordQuery;
                        cmd.Connection.Open();

                        var encPw = cmd.ExecuteScalar(); // ExecuteNonQuery();
                        NPACencryptPassword = string.Empty; // string 
                        if (encPw != null)
                        {
                            NPACencryptPassword = encPw.ToString();
                            _npacUserInfo.Encrypt_Pwd = encPw.ToString();
                        }

                        cmd.Parameters.Clear();
                        cmd = new SqlCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;
                        cmd.CommandText = NPACQueryHelper.InsertUserQuery;

                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@emp_id", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(_npacUserInfo.Emp_Id) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(_npacUserInfo.UserIdentity) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@username", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(_npacUserInfo.UserName) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@passwds", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = DBNull.Value });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@designation", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(_npacUserInfo.Designation) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@phone", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(_npacUserInfo.Phone) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@pager", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(_npacUserInfo.Pager) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@fax", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(_npacUserInfo.Fax) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@groups", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(_npacUserInfo.Groups) });
                        //cmd.Parameters.Add(new SqlParameter() { ParameterName = "@levels", SqlDbType = SqlDbType.Int, Size = 255, Value = CheckAgainstNull(_npacUserInfo.Levels) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@section", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(_npacUserInfo.Section) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@grade", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(_npacUserInfo.Grade) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@zone", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(_npacUserInfo.Zone) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@create_date", SqlDbType = SqlDbType.DateTime, Value = SQLCheckNULL.CheckAgainstNull(_npacUserInfo.Create_Date) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@encrypt_pwd", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(NPACencryptPassword) }); // _npacUserInfo.Encrypt_Pwd
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@invalid_date", SqlDbType = SqlDbType.DateTime, Value = SQLCheckNULL.CheckAgainstNull(_npacUserInfo.Invalid_Date) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@users_adid", SqlDbType = SqlDbType.NVarChar, Value = SQLCheckNULL.CheckAgainstNull(_npacUserInfo.UserADID) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@email", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(_npacUserInfo.EmailId) }); // _npacUserInfo.Encrypt_Pwd
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@company", SqlDbType = SqlDbType.NVarChar, Value = SQLCheckNULL.CheckAgainstNull(_npacUserInfo.CompanyName) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@section_name", SqlDbType = SqlDbType.NVarChar, Value = SQLCheckNULL.CheckAgainstNull(_npacUserInfo.SectionName) });

                        int success = cmd.ExecuteNonQuery();

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("The object already exists."))
                        throw;
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
        public override bool UpdateAccount(IEndpointUserInfo _userInfo)
        {
            string UserExist = string.Empty;
            string UserStatus = string.Empty;
            //SqlConnection conn = null;
           SqlCommand cmd = null;

            if (this.Connect())
            {
                try
                {
                    NPACUserInfo npacUserInfo = (NPACUserInfo)_userInfo;

                    string connStr = this.getConnectionString;
                    using (SqlConnection conn = new SqlConnection(connStr))
                    {

                        cmd = new SqlCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@username", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = npacUserInfo.UserIdentity });

                        cmd.CommandText = NPACQueryHelper.SelectUserByUserLoginQuery;
                        cmd.Connection.Open();

                        var reader = cmd.ExecuteScalar();
                        if (reader != null)
                        {
                            cmd = new SqlCommand();
                            cmd.Parameters.Clear();

                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = conn;
                            cmd.CommandText = NPACQueryHelper.updateUserQuery;

                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@pager", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(npacUserInfo.Pager) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@section", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(npacUserInfo.Section) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@zone", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(npacUserInfo.Zone) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@phone", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(npacUserInfo.Phone) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@username", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(npacUserInfo.UserName) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(npacUserInfo.UserIdentity) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@email", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(npacUserInfo.EmailId) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@users_adid", SqlDbType = SqlDbType.NVarChar, Value = SQLCheckNULL.CheckAgainstNull(npacUserInfo.UserADID) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@company", SqlDbType = SqlDbType.NVarChar, Value = SQLCheckNULL.CheckAgainstNull(npacUserInfo.CompanyName) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@section_name", SqlDbType = SqlDbType.NVarChar, Value = SQLCheckNULL.CheckAgainstNull(npacUserInfo.SectionName) });

                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@designation", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(npacUserInfo.Designation) });

                            int updated = cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            this._idmsLogger.WriteLog(LogLevelType.Error, String.Format("User {0} Not exists.", npacUserInfo.UserIdentity), null);
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
            //SqlConnection conn = null;
            SqlCommand cmd = null;

            if (this.Connect())
            {
                try
                {
                    NPACUserInfo npacUserInfo = (NPACUserInfo)_userInfo;

                    string connStr = this.getConnectionString;
                    using (SqlConnection conn = new SqlConnection(connStr))
                    {
                        cmd = new SqlCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@username", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = npacUserInfo.UserIdentity });

                        cmd.CommandText = NPACQueryHelper.SelectUserByUserLoginQuery;
                        cmd.Connection.Open();

                        var reader = cmd.ExecuteScalar();
                        if (reader != null)
                        {
                            cmd = new SqlCommand();
                            cmd.Parameters.Clear();
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = conn;
                            cmd.CommandText = NPACQueryHelper.LockUserQuery;

                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@invalid_date", SqlDbType = SqlDbType.DateTime, Value = npacUserInfo.Invalid_Date });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@levels", SqlDbType = SqlDbType.Int, Size = 255, Value = DBNull.Value }); // _npacUserInfo.Levels
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@section", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = DBNull.Value }); // _npacUserInfo.Section
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@sectionname", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = DBNull.Value }); // _npacUserInfo.SectionName
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@zone", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = DBNull.Value });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@username", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = npacUserInfo.UserIdentity });

                            int locked = cmd.ExecuteNonQuery();

                            if (locked > 0)
                            {
                                base.LockAccount(_userInfo);
                                return true;
                            }
                            else
                            {
                                this._idmsLogger.WriteLog(LogLevelType.Error, String.Format("User {0} Not exists.", npacUserInfo.UserIdentity), null);
                                return false;
                            }
                        }
                        else
                        {
                            this._idmsLogger.WriteLog(LogLevelType.Error, String.Format("User {0} Not exists.", npacUserInfo.UserIdentity));
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
            //SqlConnection conn = null;
            SqlCommand cmd = null;

            if (this.Connect())
            {
                try
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"UnlockAccount for User:{_userInfo.AccountName} started.");
                    NPACUserInfo npacUserInfo = (NPACUserInfo)_userInfo;

                    string connStr = this.getConnectionString;
                    using (SqlConnection conn = new SqlConnection(connStr))
                    {
                        cmd = new SqlCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@username", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = npacUserInfo.UserIdentity });

                        cmd.CommandText = NPACQueryHelper.SelectUserByUserLoginQuery;
                        cmd.Connection.Open();

                        var reader = cmd.ExecuteScalar();
                        if (reader != null)
                        {
                            cmd = new SqlCommand();
                            cmd.Parameters.Clear();
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = conn;

                            if (IsThroughPasswordReset)
                            {
                                npacUserInfo.Invalid_Date = null;
                                npacUserInfo.Valid_Date = DateTime.Now;
                                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@valid_date", SqlDbType = SqlDbType.DateTime, Value = npacUserInfo.Valid_Date });
                                cmd.CommandText = NPACQueryHelper.UnLockUserForPasswordResetQuery;
                            }
                            else
                            {
                                cmd.CommandText = NPACQueryHelper.UnLockUserQuery;
                                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@invalid_date", SqlDbType = SqlDbType.DateTime, Value = npacUserInfo.Invalid_Date });
                            }

                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@username", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = npacUserInfo.UserIdentity });

                            int locked = cmd.ExecuteNonQuery();

                            if (locked > 0)
                            {
                                base.UnLockAccount(_userInfo);
                                if (IsThroughPasswordReset)
                                {
                                    npacUserInfo.Save("System");
                                }
                                _idmsLogger.WriteLog(LogLevelType.Info, $"UnlockAccount for User:{_userInfo.AccountName} successfully completed.");
                                return true;
                            }
                            else
                            {
                                UnLockChangePasswordAPIMessage = String.Format("User {0} does not exist.  Please contact IAM Team.", npacUserInfo.UserIdentity);
                                this._idmsLogger.WriteLog(LogLevelType.Error, String.Format("User {0} does not exist.", npacUserInfo.UserIdentity), null);
                                return false;
                            }
                        }
                        else
                        {
                            UnLockChangePasswordAPIMessage = String.Format("User {0} does not exist.", npacUserInfo.UserIdentity);
                            this._idmsLogger.WriteLog(LogLevelType.Error, String.Format("User {0} does not exist", npacUserInfo.UserIdentity));
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
            //SqlConnection conn = null;
            SqlCommand cmd = null;
            string UserExist = string.Empty;

            NPACUserInfo npacUserInfo = (NPACUserInfo)_userInfo;

            try
            {
                string connStr = this.getConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {

                    cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conn;

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@username", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = npacUserInfo.UserIdentity });

                    cmd.CommandText = NPACQueryHelper.CheckUserLoginExistQuery;
                    cmd.Connection.Open();

                    var reader = cmd.ExecuteScalar();
                    if (reader != null)
                    {
                        cmd = new SqlCommand();
                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;
                            
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@section", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(npacUserInfo.Section) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@designation", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(npacUserInfo.Designation) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@company", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(npacUserInfo.CompanyName) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@section_name", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(npacUserInfo.SectionName) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(npacUserInfo.UserIdentity) });

                        cmd.CommandText = NPACQueryHelper.ReOrgUserQuery;

                        int updated = cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        this._idmsLogger.WriteLog(LogLevelType.Error, String.Format("User {0} Not exists.", npacUserInfo.UserIdentity));
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
        public override bool ExtendValidity(IEndpointUserInfo _userInfo)
        {
            string UserExist = string.Empty;
            string UserStatus = string.Empty;
            //SqlConnection conn = null;
            SqlCommand cmd = null;

            if (this.Connect())
            {
                try
                {
                    NPACUserInfo npacUserInfo = (NPACUserInfo)_userInfo;

                    string connStr = this.getConnectionString;
                    using (SqlConnection conn = new SqlConnection(connStr))
                    {
                        cmd = new SqlCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@username", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = npacUserInfo.UserIdentity });

                        cmd.CommandText = NPACQueryHelper.SelectUserByUserLoginQuery;
                        cmd.Connection.Open();

                        var reader = cmd.ExecuteScalar();
                        if (reader != null)
                        {
                            cmd = new SqlCommand();
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = conn;

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@invalid_date", SqlDbType = SqlDbType.DateTime, Value = npacUserInfo.Invalid_Date });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@section", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(npacUserInfo.Section) });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@username", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = npacUserInfo.UserIdentity });

                            cmd.CommandText = NPACQueryHelper.ExtendUserQuery;

                            int locked = cmd.ExecuteNonQuery();
                            if (locked > 0)
                            {
                                base.UnLockAccount(_userInfo);
                                return true;
                            }
                            else
                            {
                                this._idmsLogger.WriteLog(LogLevelType.Error, String.Format("User {0} Not exists.", npacUserInfo.UserIdentity), null);
                                return false;
                            }
                        }
                        else
                        {
                            this._idmsLogger.WriteLog(LogLevelType.Error, String.Format("User {0} Not exists.", npacUserInfo.UserIdentity), null);
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
        public override bool DisableAccount(IEndpointUserInfo _userInfo)
        {
            string UserExist = string.Empty;
            string UserStatus = string.Empty;
            //SqlConnection conn = null;
            SqlCommand cmd = null;

            if (this.Connect())
            {
                try
                {
                    RemoveAllEntitlements(_userInfo);
                    NPACUserInfo _npacUserInfo = (NPACUserInfo)_userInfo;

                    string connStr = this.getConnectionString;
                    using (SqlConnection conn = new SqlConnection(connStr))
                    {

                        cmd = new SqlCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@username", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = _npacUserInfo.UserIdentity });

                        cmd.CommandText = NPACQueryHelper.SelectUserByUserLoginQuery;
                        cmd.Connection.Open();

                        var reader = cmd.ExecuteScalar();
                        if (reader != null)
                        {
                            cmd = new SqlCommand();
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = conn;
                            cmd.Parameters.Clear();

                            //cmd.Parameters.Add(new SqlParameter() { ParameterName = "@invalid_date", SqlDbType = SqlDbType.DateTime, Value = _npacUserInfo.Invalid_Date });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@levels", SqlDbType = SqlDbType.Int, Size = 255, Value = DBNull.Value }); // _npacUserInfo.Levels
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@section", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = DBNull.Value }); // _npacUserInfo.Section
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@username", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = _npacUserInfo.UserIdentity });

                            cmd.CommandText = NPACQueryHelper.DisableUserQuery;

                            int locked = cmd.ExecuteNonQuery();
                            if (locked > 0)
                            {
                                base.DisableAccount(_userInfo);
                                return true;
                            }
                            else
                            {
                                this._idmsLogger.WriteLog(LogLevelType.Error, String.Format("User {0} Not exists.", _npacUserInfo.UserIdentity), null);
                                return false;
                            }
                        }
                        else
                        {
                            this._idmsLogger.WriteLog(LogLevelType.Error, String.Format("User {0} Not exists.", _npacUserInfo.UserIdentity), null);
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
        public override bool EnableAccount(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }
        public override bool DeleteAccount(IEndpointUserInfo _userInfo)
        {
            string UserExist = string.Empty;
            string UserStatus = string.Empty;
            //SqlConnection conn = null;
            //SqlCommand cmd = null;

            if (this.Connect())
            {
                try
                {
                    NPACUserInfo npacUserInfo = (NPACUserInfo)_userInfo;

                    string connStr = this.getConnectionString;
                    using (SqlConnection conn = new SqlConnection(connStr))
                    {

                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = conn;

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@username", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = npacUserInfo.UserIdentity });

                            cmd.CommandText = NPACQueryHelper.DeleteUserQuery;
                            cmd.Connection.Open();

                            int deleted = cmd.ExecuteNonQuery();
                            if (deleted > 0)
                            {
                                base.DeleteAccount(_userInfo);
                                return true;
                            }
                            else
                            {
                                this._idmsLogger.WriteLog(LogLevelType.Error, String.Format("User {0} Not exists.", npacUserInfo.UserIdentity), null);
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    this._idmsLogger.WriteLog(LogLevelType.Error, ex.Message, ex);
                    return false;
                }
            }
            return false;
        }
        public override bool AddEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            //SqlConnection conn = null;
            SqlCommand cmd = null;
            string UserExist = string.Empty;

            try
            {
                NPACUserInfo npacUserInfo = (NPACUserInfo)_userInfo;
                //AccountReconcilation accrecon = AccountReconcilation.GetAllByEndpointIdAndEndpointUserId(this.Endpoint.Id, npacUserInfo.Id).FirstOrDefault();
                if (IsAccountDisabled(npacUserInfo.UserIdentity))
                {
                    if (this.CurrentRequest.AdHocAccessRequest != null && (this.CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd || this.CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove))
                    {
                        this._idmsLogger.WriteLog(LogLevelType.Error, string.Format("Account - {0} is locked.", npacUserInfo.AccountName), null);
                        return false;
                    }
                }

                base.RemoveAllEntitlements(_userInfo);

                string connStr = this.getConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {

                    cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conn;

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@username", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = npacUserInfo.UserIdentity });

                    cmd.CommandText = NPACQueryHelper.SelectUserByUserLoginQuery;
                    cmd.Connection.Open();

                    var reader = cmd.ExecuteScalar();
                    if (reader != null)
                    {
                        int levelDetail = 0;
                        List<string> lstRoles = npacUserInfo.Roles;
                        if (lstRoles != null)
                        {
                            foreach (string Role in lstRoles)
                            {
                                if (!string.IsNullOrEmpty(Role))
                                {
                                    levelDetail = Convert.ToInt32(Role);
                                    break;
                                }
                            }
                        }

                        cmd = new SqlCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@levels", SqlDbType = SqlDbType.Int, Size = 255, Value = levelDetail }); // _npacUserInfo.Levels
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@username", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = npacUserInfo.UserIdentity });

                        cmd.CommandText = NPACQueryHelper.InsertUserLevelQuery;

                        int result = cmd.ExecuteNonQuery();
                        this._idmsLogger.WriteLog(LogLevelType.Info, string.Format("User {0} Level {1} Added Successfully in NPAC.", npacUserInfo.UserIdentity, npacUserInfo.Levels), null);
                        DeleteExistingEntitlements(npacUserInfo);
                        base.AddEntitlement(_userInfo, _entitlementType);
                        return true;
                    }
                    else
                    {
                        this._idmsLogger.WriteLog(LogLevelType.Error, String.Format("User {0} Not exists.", npacUserInfo.UserIdentity), null);
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
        public override bool RemoveEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            string UserExist = string.Empty;


            try
            {
                NPACUserInfo npacUserInfo = (NPACUserInfo)_userInfo;
                AccountReconcilation accrecon = AccountReconcilation.GetAllByEndpointIdAndEndpointUserId(this.Endpoint.Id, npacUserInfo.Id).FirstOrDefault();
                if (accrecon != null && accrecon.IsLocked)
                {
                    if (this.CurrentRequest.AdHocAccessRequest != null && (this.CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd || this.CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove))
                    {
                        this._idmsLogger.WriteLog(LogLevelType.Error, string.Format("Account - {0} is locked.", npacUserInfo.AccountName), null);
                        return false;
                    }
                }

                string connStr = this.getConnectionString;
                conn = new SqlConnection(connStr);
                cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;

                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@username", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = npacUserInfo.UserIdentity });

                cmd.CommandText = NPACQueryHelper.SelectUserByUserLoginQuery;
                cmd.Connection.Open();

                var reader = cmd.ExecuteScalar();
                if (reader != null)
                {
                    int levelDetail = 0;
                    List<string> lstRoles = npacUserInfo.Roles;
                    if (lstRoles != null)
                    {
                        foreach (string Role in lstRoles)
                        {
                            if (!string.IsNullOrEmpty(Role))
                            {
                                levelDetail = Convert.ToInt32(Role);
                                break;
                            }
                        }
                    }

                    cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conn;

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@levels", SqlDbType = SqlDbType.Int, Size = 255, Value = DBNull.Value }); // _npacUserInfo.Levels
                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@username", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = npacUserInfo.UserIdentity });
                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@currentlevel", SqlDbType = SqlDbType.Int, Size = 255, Value = levelDetail });

                    cmd.CommandText = NPACQueryHelper.DeleteUserLevelQuery;

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        this._idmsLogger.WriteLog(LogLevelType.Info, string.Format("User {0} Level Removed Successfully in NPAC.", npacUserInfo.UserIdentity), null);
                        base.RemoveEntitlement(_userInfo, _entitlementType);
                        return true;
                    }
                    return false;
                }
                else
                {
                    this._idmsLogger.WriteLog(LogLevelType.Error, String.Format("User {0} Not exists.", npacUserInfo.UserIdentity), null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Error, ex.Message, ex);
                return false;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }
        public override bool RemoveAllEntitlements(IEndpointUserInfo _userInfo)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            string UserExist = string.Empty;

            try
            {
                NPACUserInfo npacUserInfo = (NPACUserInfo)_userInfo;
                AccountReconcilation accrecon = AccountReconcilation.GetAllByEndpointIdAndEndpointUserId(this.Endpoint.Id, npacUserInfo.Id).FirstOrDefault();
                if (accrecon != null && accrecon.IsLocked)
                {
                    if (this.CurrentRequest.AdHocAccessRequest != null && (this.CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd || this.CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove))
                    {
                        this._idmsLogger.WriteLog(LogLevelType.Error, string.Format("Account - {0} is locked.", npacUserInfo.AccountName), null);
                        return false;
                    }
                }

                string connStr = this.getConnectionString;
                conn = new SqlConnection(connStr);
                cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;

                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@username", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = npacUserInfo.UserIdentity });

                cmd.CommandText = NPACQueryHelper.SelectUserByUserLoginQuery;
                cmd.Connection.Open();

                var reader = cmd.ExecuteScalar();
                if (reader != null)
                {
                    cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conn;

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@levels", SqlDbType = SqlDbType.Int, Size = 255, Value = DBNull.Value }); // _npacUserInfo.Levels
                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@username", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = npacUserInfo.UserIdentity });

                    cmd.CommandText = NPACQueryHelper.RemoveUserLevelQuery;

                    int result = cmd.ExecuteNonQuery();
                    this._idmsLogger.WriteLog(LogLevelType.Info, string.Format("User {0} Level Removed Successfully in NPAC.", npacUserInfo.UserIdentity), null);
                    base.RemoveAllEntitlements(_userInfo);
                    return true;
                }
                else
                {
                    this._idmsLogger.WriteLog(LogLevelType.Error, String.Format("User {0} Not exists.", npacUserInfo.UserIdentity), null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Error, ex.Message, ex);
                return false;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }
        public override bool UpdateUserIdentity(string oldUserId, string newUserId)
        {
            string UserExist = string.Empty;
            string UserStatus = string.Empty;
            SqlConnection conn = null;
            SqlCommand cmd = null;

            if (this.Connect())
            {
                try
                {
                    string connStr = this.getConnectionString;
                    conn = new SqlConnection(connStr);
                    cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conn;

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@username", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = oldUserId });

                    cmd.CommandText = NPACQueryHelper.SelectUserByUserLoginQuery;
                    cmd.Connection.Open();

                    var reader = cmd.ExecuteScalar();
                    if (reader != null)
                    {
                        cmd = new SqlCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(newUserId) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@olduserid", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = SQLCheckNULL.CheckStringAgainstNull(oldUserId) });

                        cmd.CommandText = NPACQueryHelper.updateUserIdentityQuery;

                        int updated = cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        this._idmsLogger.WriteLog(LogLevelType.Error, String.Format("User {0} Not exists.", oldUserId), null);
                        return false;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    this._idmsLogger.WriteLog(LogLevelType.Error, ex.Message, ex);
                    return false;
                }
                finally
                {
                    if (conn != null)
                    {
                        conn.Close();
                    }
                }
            }
            return false;
        }
        #endregion

        private void DeleteExistingEntitlements(NPACUserInfo npacUser)
        {
            try
            {
                var npaUsrWithExistLevels = NPACUserInfo.GetByIdIncludeLevels(npacUser.Id);
                npaUsrWithExistLevels.Roles = new List<string>();
                var exisintLevls = string.Join(",", npaUsrWithExistLevels.NPACLevelInfos.Select(l => l.Level).ToList());

                if (!string.IsNullOrWhiteSpace(exisintLevls))
                {
                    NPACUserInfo.DeleteNPACUserLEVELMap(npaUsrWithExistLevels, exisintLevls);
                }

                var entitlementRecon = EntitlementReconcilation.GetByEndpointUserId(npaUsrWithExistLevels.Id, this.Endpoint.Id);
                foreach (var ent in entitlementRecon)
                {
                    ent.IsDeleted = true;
                    ent.DeletedDate = DateTime.Now;
                    ent.LastModifiedBy = "System";
                    ent.LastModifiedDate = DateTime.Now;
                    ent.IsSODConflict = false; // Clear SOD Conflict Flag if its set.
                    ent.IsSensitiveAccess = false; // Clear SA Flag if its set.
                    ent.Save("System");
                }

            }
            catch (Exception ex)
            {

            }
        }

        public override bool IsAdminLock(string userLogonName)
        {
            bool isLocked = false;
            try
            {
                return !IsUserExistsInEndpoint(userLogonName);
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Error, $"IsAdminLock:{userLogonName} failed.{ex.Message}", ex);
                isLocked = true;
            }
            return isLocked;
        }
        public bool IsUserExistsInEndpoint(string userIdentity)
        {
            bool IsUserExists = false;
            SqlConnection conn = null;
            SqlCommand cmd = null;
            try
            {
                string connStr = this.getConnectionString;
                conn = new SqlConnection(connStr);
                cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@username", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = userIdentity });
                cmd.CommandText = NPACQueryHelper.SelectUserQuery + " WHERE userid = @username";
                cmd.Connection.Open();

                IDataReader reader = cmd.ExecuteReader();
                if(reader != null)
                {
                    while (reader.Read())
                    {
                        var levels = CheckNull.AgainstNullableInt(reader["levels"]);
                        var section = CheckNull.AgainstString(reader["section"]).Trim();
                        if (levels == null && string.IsNullOrWhiteSpace(section))
                        {
                            UnLockChangePasswordAPIMessage = $"User{userIdentity} is not active(Section and Level Empty). Please contact IAM Team.";
                            IsUserExists = false;
                        }
                        IsUserExists = true;
                    }
                }
                else
                {
                    IsUserExists = false;
                    UnLockChangePasswordAPIMessage = $"User:{userIdentity} does not exist.";
                }
            }
            catch (Exception ex)
            {
                IsUserExists = false;
                UnLockChangePasswordAPIMessage = $"Error occured while finding the user:{userIdentity}";
                _idmsLogger.WriteLog(LogLevelType.Error, $"Error occured at IsUserExistsInEndpoint {ex.ToString()}", ex);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
            return IsUserExists;
        }

        private bool IsAccountDisabled(string userIdentity)
        {
            bool IsUserNotExists = false;
            SqlConnection conn = null;
            SqlCommand cmd = null;
            try
            {
                string connStr = this.getConnectionString;
                conn = new SqlConnection(connStr);
                cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@username", SqlDbType = SqlDbType.NVarChar, Size = 255, Value = userIdentity });
                cmd.CommandText = NPACQueryHelper.SelectUserQuery + " WHERE userid = @username";
                cmd.Connection.Open();

                IDataReader reader = cmd.ExecuteReader();
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var levels = CheckNull.AgainstNullableInt(reader["levels"]);
                        var section = CheckNull.AgainstString(reader["section"]).Trim();
                        _idmsLogger.WriteLog(LogLevelType.Info, $"Checking User:{userIdentity} Details in NPAC: levels:{levels} section:{section}");
                        return levels == null && string.IsNullOrWhiteSpace(section);
                    }
                }
                else
                {
                    IsUserNotExists = true;
                }
            }
            catch (Exception ex)
            {
                IsUserNotExists = true;
                _idmsLogger.WriteLog(LogLevelType.Error, $"Error occured at IsUserExistsInEndpoint {ex.ToString()}", ex);
                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
            return IsUserNotExists;
        }
    }
}
