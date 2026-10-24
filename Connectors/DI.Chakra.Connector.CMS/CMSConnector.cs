using DI.Chakra.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.DomainModel;
using System.Net.Http;
using System.Net.Http.Headers;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.DBConfigHelper;
using System.Data.SqlClient;
using DI.Chakra.Logger;
using System.Data;
using System.Collections;

namespace DI.Chakra.Connector
{
    public class CMSConnector : ConnectorBase
    {

        #region Members

        SqlConnection sqlConnect = null;
        string connectionString = "";
        public EndPoint endpoint;
        private Dictionary<long, string> dicUsers = new Dictionary<long, string>();
        private Dictionary<long, string> dicRoles = new Dictionary<long, string>();
        private Dictionary<string, ArrayList> dicRoleUsers = new Dictionary<string, ArrayList>();

        #endregion

        #region Core
        public CMSConnector(EndPoint endpoint, AccessRequest currentRequest)
        {
            this.EndPoint = endpoint;
            this.CurrentRequest = currentRequest;
            this.EndpointConfiguration = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);
        }
        public override IMProcessor IMProcessor
        {
            get
            {
                CMS.IMProcessor processor = new CMS.IMProcessor();

                processor.EndPoint = this.EndPoint;
                processor.CMSConfiguration = this.EndpointConfiguration;


                return processor;
            }
        }
        #endregion

        #region Discovery
        public override bool Connect()
        {
            connectionString = "Data Source=" + Configuration(EndpointKey.ServerName) + "; Initial Catalog=" + Configuration(EndpointKey.DatabaseName) + ";Persist Security Info= True; User ID=" + Configuration(EndpointKey.UserName) + ";Password=" + Configuration(EndpointKey.Password) + ";Connect Timeout=100";

            try
            {
                sqlConnect = new SqlConnection(connectionString);
                sqlConnect.Open();
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Connected to CMS Server - {0}",
                                                          Configuration(EndpointKey.ServerName)));
                return true;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, string.Format("Could not connect to CMS Server - {0}",
                                                   Configuration(EndpointKey.ServerName)), ex);
                return false;
            }
        }
        public override void Disconnect()
        {
            try
            {
                if (sqlConnect != null)
                {
                    sqlConnect.Close();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Disconnected to CMS Server - {0}",
                                                              Configuration(EndpointKey.ServerName)));
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, string.Format("Could not Disconnected to CMS Server - {0}",
                                                   Configuration(EndpointKey.ServerName)), ex);
            }
        }
        public override bool ChangePassword(string UserName, string Password, long DIUserId, bool isReactivation)
        {
            try
            {
                //table name needs to be configured in the configuration screen.
                string ChangeUserPasswordQuery = "UPDATE DI_ERP_User SET Password = @Password WHERE UserName = @UserName";
                SqlCommand cmd = null;
                cmd = new SqlCommand();
                if (sqlConnect == null) Connect();
                cmd.Connection = sqlConnect;
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = ChangeUserPasswordQuery;
                cmd.Parameters.AddWithValue("@Password", Password);
                cmd.Parameters.AddWithValue("@UserName", UserName);
                int Result = cmd.ExecuteNonQuery();
                if (Result > 0)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("CMS Change password completed for username - {0}",
                                                              UserName));
                    return true;
                }
                else
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("CMS Change password failed for username - {0}",
                                                              UserName));
                    return false;
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, string.Format("CMS Change password failed for username - {0}",
                                                          UserName), ex);
            }
            return false;
        }
        public override void GetData()
        {
            try
            {
                long DiscoveryProcessId = base.DiscoveryProcess.Id;
                List<EndpointObjects> EndpointObjects = DI.Chakra.DomainModel.DiscoveryProcess.GetByDiscoveryProcessId(DiscoveryProcessId);

                bool IsUserSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_CMS_User) != null;
                bool IsRoleSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_CMS_Role) != null;

                if (IsUserSelected || IsRoleSelected)
                {
                    if (IsUserSelected)
                    {
                        dicUsers.Clear();
                        DiscoverUser();
                    }
                    if (IsRoleSelected)
                    {
                        dicRoles.Clear();
                        dicRoleUsers.Clear();
                        DiscoverRole();
                    }
                    SaveUserRole();

                    //Update recon table
                    this.UpdateEntitlementRecon();
                }
                //CMSUserInfo.MapWithMasterUser(EndPoint, m_CMSConfig.MapAll != null ? m_CMSConfig.MapAll.Value : false);
                CMSUserInfo.MapWithMasterUserForReconTable(EndPoint, this.EndpointConfiguration.MapAll);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "CMS GetData Failed.", ex);
                throw;
            }
        }
        void SaveUserRole()
        {
            CMSUserInfo.SaveUserRoleMap(dicUsers, dicRoles, dicRoleUsers, this.EndPoint.Id);
        }
       
        #region Discover Objects
        void DiscoverUser()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering CMS Users"));
            IList<CMSUserInfo> userlist = this.GetUsers();
            CMSUserInfo.SaveUsers(userlist, EndPoint.Id, MasterConfigHelper.ServiceUser);
            userlist = null;
            AccountReconcilation.ProcessDeletedAccounts(this.EndPoint.Id, this.Process, MasterConfigHelper.ServiceUser);
            userlist = CMSUserInfo.GetDeletedUsers(this.EndPoint.Id, this.Process);

            CMSUserInfo.RemoveUser(userlist.ToList());
            dicUsers = CMSUserInfo.GetAllByEndPointSystemId(this.EndPoint.Id);
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering CMS Users"));
        }
        void DiscoverRole()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering CMS Roles"));
            List<CMSRoleInfo> rolelist = this.GetRoles().ToList();
            CMSRoleInfo.SaveRoles(rolelist, MasterConfigHelper.ServiceUser);
            IList<CMSRoleInfo> listCmsRole = new List<CMSRoleInfo>();
            listCmsRole = CMSRoleInfo.GetDeletedByEndPointSystemId(this.EndPoint.Id);
            CMSRoleInfo.DeleteList(listCmsRole.ToList());
            dicRoles = CMSRoleInfo.GetByEndpointId(this.EndPoint.Id);
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering CMS Roles"));
        }
        IList<CMSUserInfo> GetUsers()
        {
            try
            {
                //table name needs to configured from cms configuration screen;
                string query = @"SELECT IDType,IdentificationNo,UserName,FirstName,MiddleName,LastName,DisplayName,KnownAs,EmployeeId,OrganisationId,BusinessRoleId,ManagerId,EmailId,TelephoneNumber,HomephoneNumber,MobileNumber,Fax,PostalCode,PostalAddress,Ethinicity,Country,IsActive,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate, LastLogonDate, IsLocked, Password FROM DI_ERP_User";
                CMSUserInfo cmsUserInfo = null;
                IDataReader reader = null;

                IList<CMSUserInfo> cmsUserList = new List<CMSUserInfo>();
                try
                {
                    SqlCommand cmd = null;
                    cmd = new SqlCommand();
                    cmd.Connection = sqlConnect;

                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.Text;

                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        cmsUserInfo = new CMSUserInfo();


                        cmsUserInfo.EndPointSystemId = EndPoint.Id;
                        cmsUserInfo.IDType = CheckNull.AgainstString(reader["IDType"]);
                        cmsUserInfo.IdentificationNo = CheckNull.AgainstString(reader["IdentificationNo"]);
                        cmsUserInfo.UserName = CheckNull.AgainstString(reader["UserName"]);
                        cmsUserInfo.FirstName = CheckNull.AgainstString(reader["FirstName"]);
                        cmsUserInfo.MiddleName = CheckNull.AgainstString(reader["MiddleName"]);
                        cmsUserInfo.LastName = CheckNull.AgainstString(reader["LastName"]);
                        cmsUserInfo.DisplayName = CheckNull.AgainstString(reader["DisplayName"]);
                        cmsUserInfo.KnownAs = CheckNull.AgainstString(reader["KnownAs"]);
                        cmsUserInfo.EmployeeId = CheckNull.AgainstString(reader["EmployeeId"]);
                        cmsUserInfo.OrganisationId = CheckNull.AgainstInt(reader["OrganisationId"]);
                        cmsUserInfo.BusinessRoleId = CheckNull.AgainstInt(reader["BusinessRoleId"]);
                        cmsUserInfo.ManagerId = CheckNull.AgainstInt(reader["ManagerId"]);
                        cmsUserInfo.EmailId = CheckNull.AgainstString(reader["EmailId"]);
                        cmsUserInfo.TelephoneNumber = CheckNull.AgainstString(reader["TelephoneNumber"]);
                        cmsUserInfo.HomephoneNumber = CheckNull.AgainstString(reader["HomephoneNumber"]);
                        cmsUserInfo.MobileNumber = CheckNull.AgainstString(reader["MobileNumber"]);
                        cmsUserInfo.Fax = CheckNull.AgainstString(reader["Fax"]);
                        cmsUserInfo.PostalCode = CheckNull.AgainstString(reader["PostalCode"]);
                        cmsUserInfo.PostalAddress = CheckNull.AgainstString(reader["PostalAddress"]);
                        cmsUserInfo.Ethinicity = CheckNull.AgainstString(reader["Ethinicity"]);
                        cmsUserInfo.Country = CheckNull.AgainstString(reader["Country"]);
                        cmsUserInfo.IsActive = CheckNull.AgainstBit(reader["IsActive"]);
                        cmsUserList.Add(cmsUserInfo);

                    }
                    return cmsUserList;

                }
                catch (Exception ex)
                {
                    WriteDiscoveryLog(LogLevelType.Error, string.Format("Could not Get User Details from CMS Server - {0}",
                                                   Configuration(EndpointKey.ServerName)), ex);
                    throw;
                }
                finally
                {
                    reader.Close();
                    reader.Dispose();
                }
                return cmsUserList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public IList<CMSRoleInfo> GetRoles()
        {
            try
            {
                //table name needs to configured from cms configuration screen;
                string query = @"SELECT RoleName, RoleDescription, CreatedBy, CreatedDate, LastModifiedBy, LastModifiedDate FROM DI_ERP_Role";

                CMSRoleInfo cmsRoleInfo = null;
                IList<CMSRoleInfo> cmsRoleList = new List<CMSRoleInfo>();
                IDataReader reader = null;
                try
                {
                    SqlCommand cmd = null;
                    cmd = new SqlCommand();
                    cmd.Connection = sqlConnect;
                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.Text;
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        cmsRoleInfo = new CMSRoleInfo();

                        cmsRoleInfo.EndPointSystemId = EndPoint.Id;
                        cmsRoleInfo.RoleName = CheckNull.AgainstString(reader["RoleName"]);
                        cmsRoleInfo.RoleDescription = CheckNull.AgainstString(reader["RoleDescription"]);
                        cmsRoleInfo.CanRequest = true;
                        cmsRoleList.Add(cmsRoleInfo);
                    }
                    reader.Close();

                    foreach (var item in cmsRoleList)
                    {
                        try
                        {
                            SqlCommand cmdRoleUser = new SqlCommand();
                            cmdRoleUser.Connection = sqlConnect;
                            cmdRoleUser.CommandText = string.Format("SELECT erpUser.UserName as erpUserName FROM ERPUserRole userRole INNER JOIN DI_ERP_User erpUser ON userRole.ERPUser_Id = erpUser.Id WHERE Role_Id IN (SELECT ID FROM DI_ERP_Role WHERE RoleName = '{0}')", item.RoleName);
                            cmdRoleUser.CommandType = CommandType.Text;
                            SqlDataReader roleUserReader = cmdRoleUser.ExecuteReader();
                            ArrayList member = new ArrayList();
                            while (roleUserReader.Read())
                            {
                                if (roleUserReader.HasRows && roleUserReader.FieldCount > 0)
                                    member.Add(CheckNull.AgainstString(roleUserReader[0]));
                            }
                            dicRoleUsers.Add(cmsRoleInfo.RoleName, member);
                            roleUserReader.Close();
                            roleUserReader.Dispose();
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                }
                catch (Exception ex)
                {
                    WriteDiscoveryLog(LogLevelType.Error, string.Format("Could not Get Role Details from CMS Server - {0}",
                                                   Configuration(EndpointKey.ServerName)), ex);

                    throw;
                }
                finally
                {
                    reader.Close();
                    reader.Dispose();
                }
                return cmsRoleList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void UpdateEntitlementRecon()
        {
            try
            {
                foreach (KeyValuePair<long, string> item in dicUsers)
                {
                    var cmsUserInfo = CMSUserInfo.GetByIdIncludeCMSRoles(item.Key);

                    if (cmsUserInfo.CMSRoles != null)
                    {
                        foreach (CMSRoleInfo entitlement in cmsUserInfo.CMSRoles)
                        {
                            var acctRecon = AccountReconcilation.GetByUser(cmsUserInfo.EndPointSystemId, cmsUserInfo.Id);
                            var entRecon = EntitlementReconcilation.GetByUser(cmsUserInfo.Id, cmsUserInfo.EndPointSystemId, entitlement.RoleName.Trim(), EntitlementType.Role);

                            if (entRecon != null)
                            {
                                entRecon.LastModifiedDate = DateTime.Now;
                                entRecon.LastModifiedBy = "System";
                                entRecon.Save(MasterConfigHelper.ServiceUser);
                            }
                            else
                            {
                                var entitlements = (cmsUserInfo.Entitlements.Select(x => x.EntitlementName.ToString()));
                                entRecon = this.CreateEntitlementReconcilationShell(acctRecon, EntitlementType.Role, string.Join(",", entitlements != null ? entitlements.ToArray() : new string[] { }));
                                entRecon.Entitlement = entitlement.RoleName.Trim();
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
                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while updating Entitlement reconciliation for CMS.", ex);
            }
        }
        #endregion

        #endregion
    }
}
