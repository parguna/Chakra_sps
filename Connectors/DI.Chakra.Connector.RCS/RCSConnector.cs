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
using DI.Chakra.Logger;
using System.Data;
using System.Collections;
using System.IO;
using CConstant = DI.Chakra.Connector.RCS.Constant;
using CMessage = DI.Chakra.Connector.RCS.Message;

namespace DI.Chakra.Connector
{
    public class RCSConnector : ConnectorBase
    {
        #region Members
        private DomainModel.EndPoint _endpoint;
        private EndpointConfiguration _rcsConfiguration = null;
        #endregion
        public RCSConnector(DomainModel.EndPoint endpoint, AccessRequest currentRequest)
        {
            this._endpoint = endpoint;
            this.CurrentRequest = currentRequest;
            var rcsConfiguration = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);
            rcsConfiguration.ConfigurationDetails.ForEach(lq =>
            {
                if (lq.EndpointConfigurationKey.KeyName == EndpointKey.Password.ToString())
                {
                    lq.Value = RSAHelper.DecryptData(lq.Value);
                }
            });
            this.RCSConfiguration = rcsConfiguration;
            this.EndpointConfiguration = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);
            this.NetworkUserName = GetConfiguration(EndpointKey.BizServiceUserName);
            this.NetworkPassword = GetConfiguration(EndpointKey.BizServicePassword);
        }
        public EndpointConfiguration RCSConfiguration
        {
            get { return _rcsConfiguration; }
            set { this._rcsConfiguration = value; }
        }
        private string GetConfiguration(EndpointKey Key)
        {
            var config = RCSConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.EndpointConfigurationKey.KeyName == Key.ToString());
            return config != null ? config.Value : "";
        }

        private string GetFileNameByDateTime(string sharedPath, string moduleName)
        {
            if (!string.IsNullOrEmpty(sharedPath) && !string.IsNullOrEmpty(moduleName))
            {
                string todaydateTime = DateTime.Now.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

                return string.Concat(sharedPath, "\\" + "RCS_" + moduleName + "_" + todaydateTime + ".csv");
            }

            return string.Empty;
        }

        public override IMProcessor IMProcessor
        {
            get
            {
                RCS.IMProcessor processor = new RCS.IMProcessor();
                processor._endpoint = this.EndPoint;
                processor._rcsConfiguration = this._rcsConfiguration;
                return processor;
            }
        }

        public override bool Connect()
        {

            return true;
        }
        public override void Disconnect()
        {

        }
        public override bool ChangePassword(string UserName, string Password, long DIUserId, bool isReactivation)
        {
            throw new NotImplementedException();
        }
        public override void GetData()
        {
            try
            {
                long discoveryProcessId = base.DiscoveryProcess.Id;
                List<EndpointObjects> Objects = DI.Chakra.DomainModel.DiscoveryProcess.GetByDiscoveryProcessId(discoveryProcessId);
                bool IsUsersSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_RCS_User) != null;
                bool IsRolesSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_RCS_ROLE) != null;
                bool IsFunctionSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_RCS_FUNCTION) != null;
                bool IsPrivillegeAcctTransSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_RCS_PRIVILLEGEACCTTRANS) != null;


                if (!MarkDiscoveryFail && IsFunctionSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.RCS_107, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverFunctions();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.RCS_108, GetConfiguration(EndpointKey.IPAddress)));
                }
                if (!MarkDiscoveryFail && IsRolesSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.RCS_110, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverRoles();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.RCS_111, GetConfiguration(EndpointKey.IPAddress)));
                }
                if (!MarkDiscoveryFail && IsUsersSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.RCS_105, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverUser();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.RCS_106, GetConfiguration(EndpointKey.IPAddress)));
                }
                if (!MarkDiscoveryFail && IsPrivillegeAcctTransSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.RCS_113, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverPrivillegeAcctTrans();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.RCS_114, GetConfiguration(EndpointKey.IPAddress)));
                }
                if (!MarkDiscoveryFail)
                {
                    if (this.DoRecon)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("Account Recon Started For CMS", GetConfiguration(EndpointKey.IPAddress)));
                        this.AccountRecon();
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("Account Recon Ended For CMS", GetConfiguration(EndpointKey.IPAddress)));

                        WriteDiscoveryLog(LogLevelType.Info, string.Format("Entitlement Recon Started For CMS", GetConfiguration(EndpointKey.IPAddress)));
                        this.EntitlementRecon();
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("Entitlement Recon Started For CMS", GetConfiguration(EndpointKey.IPAddress)));
                    }

                    RCSUserInfo.MapWithMasterUserForReconTable(_endpoint, RCSConfiguration.MapAll != null ? RCSConfiguration.MapAll : false);
                    RCSUserInfo.MapWithMasterUserForEntitlementReconTable(_endpoint, RCSConfiguration.MapAll != null ? RCSConfiguration.MapAll : false, discoveryProcessId);

                    base.DeleteLocalCopyFiles();
                }

                if (this.IsExceptionRaised || MarkDiscoveryFail)
                {
                    throw new Exception(string.Format("RCS discovery failed {0}", GetConfiguration(EndpointKey.IPAddress)));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Discover Objects
        private void DiscoverUser()
        {
            try
            {
                this.GetUsers();
            }
            catch (Exception ex)
            {

            }
        }
        private void DiscoverFunctions()
        {
            try
            {
                this.GetFunctions();

            }
            catch (Exception ex)
            {

            }
        }
        private void DiscoverRoles()
        {
            try
            {
                this.GetRoles();
            }
            catch (Exception ex)
            {

            }
        }
        private void DiscoverPrivillegeAcctTrans()
        {
            try
            {
                this.GetPrivillegeAcctTrans();
            }
            catch (Exception ex)
            {

            }
        }
        private void GetUsers()
        {

            List<long> selectedUsersList = new List<long>();
            StringBuilder queryString = new StringBuilder();
            RCSUserRole userRole = null;
            IList<RCSUserRole> rcsUserRole = new List<RCSUserRole>();
            try
            {
                long userId = 0;
                string mandatoryCols = CMessage.RCS_USERS_MANDATORY_COLS;
                string filePath = GetConfiguration(EndpointKey.UserFilePath);
                var dataTable = base.AuthenticateNetworkAndGetFileData(filePath, mandatoryCols, 0, true, false);

                if (dataTable != null)
                {
                    this.DoRecon = true;
                    //Get User List
                    var usersList = (from table in dataTable.AsEnumerable()
                                     group table by new
                                     {
                                         ADID = table.Field<string>(0),
                                         RoleName = table.Field<string>(1),
                                         LastLogin = table.Field<string>(2)
                                     } into g
                                     select new RCSUserInfo
                                     {
                                         ADID = g.Key.ADID,
                                         dummyLastLoginDate = g.Key.LastLogin
                                     }).ToList();
                    //Get Roles
                    var roleList = (from table in dataTable.AsEnumerable()
                                    group table by new
                                    {
                                        ADID = table.Field<string>(0),
                                        RoleName = table.Field<string>(1),
                                        LastLogin = table.Field<string>(2)
                                    } into g
                                    select new RCSRoleInfo
                                    {
                                        RoleCode = g.Key.RoleName,
                                        RoleDesc = g.Key.RoleName
                                    }).ToList();

                    string[] separators = { "\\" };
                    usersList.ForEach(u =>
                    {
                        if (!string.IsNullOrEmpty(u.ADID))
                        {
                            string[] adidWithoutDomain = u.ADID.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                            if (adidWithoutDomain.Count() > 1)
                            {
                                u.ADID = adidWithoutDomain[1];
                            }
                            else
                            {
                                u.ADID = adidWithoutDomain[0];
                            }
                        }
                    });

                    //Save Users
                    RCSUserInfo.SaveUsers(usersList, _endpoint.Id, MasterConfigHelper.ServiceUser);

                    //Save Roles
                    RCSRoleInfo.SaveRoles(roleList, _endpoint.Id, MasterConfigHelper.ServiceUser);

                    List<RCSUserInfo> existingUsers = RCSUserInfo.GetByEndpointSystemIdIncludeEndPointSystem(_endpoint.Id).ToList();
                    List<RCSRoleInfo> existingRoles = RCSRoleInfo.GetByEndpointSystemIdIncludeEndPointSystem(_endpoint.Id).ToList();
                    RCSRoleInfo roleInfo = null;
                    RCSUserInfo userInfo = null;
                    List<RCSUserRole> userRoleList = new List<RCSUserRole>();
                    RCSUserRole userRoleItem = null;

                    int counter = 0;
                    foreach (DataRow currentWorkingRow in dataTable.Rows)
                    {
                        try
                        {
                            var rcsUserADID = Convert.ToString(GetColumnValueByIndex(currentWorkingRow, 0)).Trim();
                            var rcsRoleCode = Convert.ToString(GetColumnValueByIndex(currentWorkingRow, 1)).Trim();
                            var lastLogin = GetColumnValueByIndex(currentWorkingRow, 2);

                            if (string.IsNullOrEmpty(rcsUserADID))
                            {
                                continue;
                            }


                            if (!string.IsNullOrEmpty(rcsUserADID))
                            {
                                string[] adidWithoutDomain = rcsUserADID.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                                if (adidWithoutDomain.Count() > 1)
                                {
                                    rcsUserADID = adidWithoutDomain[1];
                                }
                                else
                                {
                                    rcsUserADID = adidWithoutDomain[0];
                                }
                            }

                            roleInfo = existingRoles.Where(r => r.RoleCode == rcsRoleCode).FirstOrDefault();
                            userInfo = existingUsers.Where(u => u.ADID == rcsUserADID).FirstOrDefault();

                            if (roleInfo != null && userInfo != null && !userRoleList.Any(u => u.UserId == userInfo.Id && u.RoleId == roleInfo.Id))
                            {
                                userRoleItem = new RCSUserRole();
                                userRoleItem.RoleId = roleInfo.Id;
                                userRoleItem.UserId = userInfo.Id;
                                userRoleList.Add(userRoleItem);
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, CMessage.RCS_120 + ex.Message);
                        }
                    }

                    if (userRoleList != null && userRoleList.Count() > 0)
                    {
                        userRoleList = userRoleList.Distinct().ToList();
                        string executeQuery = string.Empty;
                        foreach (var item in userRoleList)
                        {
                            if (!selectedUsersList.Any(e => e == item.UserId))
                            {
                                executeQuery = string.Empty;
                                selectedUsersList.Add(item.UserId);
                                executeQuery = string.Format(CMessage.RCS_118, item.UserId);
                                RCSRoleInfo.ExecuteQuery(executeQuery);
                            }

                            queryString.AppendLine(string.Format(CMessage.RCS_119, item.UserId, item.RoleId, this._endpoint.Id, Convert.ToInt16(VersionNo.Available)));
                            if (counter == 500)
                            {
                                RCSRoleInfo.ExecuteQuery(queryString.ToString());
                                queryString = new StringBuilder();
                                counter = 0;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(queryString.ToString()))
                    {
                        RCSRoleInfo.ExecuteQuery(queryString.ToString());
                    }

                    //Delete files
                    base.AuthenticateNetworkAndDeleteAllFiles(filePath);
                }
                else
                {
                    this.DoRecon = false;
                }
            }
            catch (Exception ex)
            {
                this.DoRecon = false;
                WriteDiscoveryLog(LogLevelType.Error, CMessage.RCS_120 + ex.Message, null, true);
            }
        }

        private void AccountRecon()
        {
            try
            {
                List<RCSUserInfo> userList = RCSUserInfo.GetDeletedUsers(this._endpoint.Id, this.Process, this.FileTimeStamp).ToList();
                if (userList != null && userList.Count > 0)
                {
                    RCSUserInfo.RemoveCMSUser(userList.ToList());
                }

                AccountReconcilation.ProcessDeletedAccounts(this._endpoint.Id, this.Process, MasterConfigHelper.ServiceUser, true, this.FileTimeStamp);

            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.RCS_ERROR_USERS, GetConfiguration(EndpointKey.IPAddress)), ex, true);

            }
        }

        private void GetFunctions()
        {
            try
            {
                string mandatoryCols = CMessage.RCS_FUN_MANDATORY_COLS;
                string filePath = GetConfiguration(EndpointKey.FunctionFilePath);
                var dataTable = base.AuthenticateNetworkAndGetFileData(filePath, mandatoryCols, 0);

                if (dataTable != null)
                {
                    //Get Roles
                    var roleList = (from table in dataTable.AsEnumerable()
                                    group table by new
                                    {
                                        RoleName = table.Field<string>(0)
                                    } into g
                                    select new RCSRoleInfo
                                    {
                                        RoleCode = g.Key.RoleName,
                                        RoleDesc = g.Key.RoleName
                                    }).ToList();

                    //Get Functions
                    var functionList = (from table in dataTable.AsEnumerable()
                                        group table by new
                                        {
                                            RoleName = table.Field<string>(0),
                                            rcsFNCode = table.Field<string>(1),
                                            rcsFunctionDesc = table.Field<string>(2),
                                            rcsFNStatus = table.Field<string>(3),
                                            rcsView = table.Field<string>(4),
                                            rcsCreate = table.Field<string>(5),
                                            rcsModify = table.Field<string>(6),
                                            rcsDelete = table.Field<string>(7),
                                            rcsPrint = table.Field<string>(8),
                                            rcsExport = table.Field<string>(9)
                                        } into g
                                        select new RCSFunctionInfo
                                        {
                                            RoleCode = !string.IsNullOrEmpty(g.Key.RoleName) ? Convert.ToString(g.Key.RoleName).Trim() : null,
                                            FNCode = !string.IsNullOrEmpty(g.Key.rcsFNCode) ? Convert.ToString(g.Key.rcsFNCode).Trim() : null,
                                            FunctionDesc = !string.IsNullOrEmpty(g.Key.rcsFunctionDesc) ? Convert.ToString(g.Key.rcsFunctionDesc).Trim() : null,
                                            FNStatus = !string.IsNullOrEmpty(g.Key.rcsFNStatus) ? Convert.ToString(g.Key.rcsFNStatus).Trim() : null,
                                            View = string.Compare(Convert.ToString(g.Key.rcsView), "Y", true) == 0 ? true : false,
                                            Create = string.Compare(Convert.ToString(g.Key.rcsCreate), "Y", true) == 0 ? true : false,
                                            Modify = string.Compare(Convert.ToString(g.Key.rcsModify), "Y", true) == 0 ? true : false,
                                            Delete = string.Compare(Convert.ToString(g.Key.rcsDelete), "Y", true) == 0 ? true : false,
                                            Print = string.Compare(Convert.ToString(g.Key.rcsPrint), "Y", true) == 0 ? true : false,
                                            Export = string.Compare(Convert.ToString(g.Key.rcsExport), "Y", true) == 0 ? true : false,

                                        }).ToList();

                    //Save Roles
                    RCSRoleInfo.SaveRoles(roleList, _endpoint.Id, MasterConfigHelper.ServiceUser);

                    //SaveFunctions
                    RCSFunctionInfo.SaveFunctions(functionList, _endpoint.Id, MasterConfigHelper.ServiceUser);

                    List<RCSRoleInfo> existingRoles = RCSRoleInfo.GetByEndpointSystem(_endpoint.Id).ToList();
                    List<RCSFunctionInfo> existingFuncs = RCSFunctionInfo.GetByEndpointSystemIdIncludeEndPointSystem(_endpoint.Id).ToList();

                    RCSRoleInfo roleItem = null;
                    RCSFunctionInfo funcItem = null;

                    long batchCount = 0;
                    StringBuilder queryBuilder = new StringBuilder();
                    if (dataTable.Rows.Count > 0)
                    {
                        string query = string.Format(CMessage.RCS_DELETE_FUNCROLE_QUERY, _endpoint.Id);
                        RCSFunctionInfo.ExecuteQuery(query);
                    }

                    List<RCSFunctionRole> funRoleList = new List<RCSFunctionRole>();
                    RCSFunctionRole funcRoleItem = null;
                    foreach (DataRow currentWorkingRow in dataTable.Rows)
                    {
                        try
                        {

                            var rcsRoleCode = Convert.ToString(GetColumnValueByIndex(currentWorkingRow, 0)).Trim();
                            var rcsFNCode = Convert.ToString(GetColumnValueByIndex(currentWorkingRow, 1)).Trim();


                            if (string.IsNullOrEmpty(rcsFNCode) || string.IsNullOrEmpty(rcsRoleCode))
                            {
                                continue;
                            }

                            roleItem = existingRoles.Where(r => r.RoleCode == rcsRoleCode).FirstOrDefault();
                            funcItem = existingFuncs.Where(r => r.FNCode == rcsFNCode && r.RoleCode == rcsRoleCode).FirstOrDefault();

                            if (roleItem != null && funcItem != null && !funRoleList.Any(a => a.FunctionId == funcItem.Id && a.RoleId == roleItem.Id))
                            {
                                funcRoleItem = new RCSFunctionRole();
                                funcRoleItem.RoleId = roleItem.Id;
                                funcRoleItem.FunctionId = funcItem.Id;

                                funRoleList.Add(funcRoleItem);
                            }


                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, CMessage.RCS_124 + ex.Message);
                        }
                    }

                    if (funRoleList != null && funRoleList.Count() > 0)
                    {
                        funRoleList = funRoleList.Distinct().ToList();

                        foreach (var item in funRoleList)
                        {
                            batchCount++;
                            queryBuilder.AppendLine(string.Format(CMessage.RCS_INSERT_FUNCROLE_QUERY, item.RoleId, item.FunctionId, _endpoint.Id, Convert.ToInt16(VersionNo.Available)));

                            if (batchCount == 500)
                            {
                                RCSFunctionInfo.ExecuteQuery(queryBuilder.ToString());
                                queryBuilder = new StringBuilder();
                                batchCount = 0;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(queryBuilder.ToString()))
                    {
                        RCSFunctionInfo.ExecuteQuery(queryBuilder.ToString());
                    }

                    //Delete files
                    base.AuthenticateNetworkAndDeleteAllFiles(filePath);

                    IList<RCSFunctionInfo> deleteFuncs = RCSFunctionInfo.GetDeletedByEndPointSystemId(this._endpoint.Id);
                    RCSFunctionInfo.DeleteList(deleteFuncs.ToList());
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, CMessage.RCS_124 + ex.Message, null, true);
            }
        }
        private void GetRoles()
        {
            try
            {
                string mandatoryCols = CMessage.RCS_ROLES_MANDATORY_COLS;
                string filePath = GetConfiguration(EndpointKey.RoleFilePath);
                var dataTable = base.AuthenticateNetworkAndGetFileData(filePath, mandatoryCols, 0);

                if (dataTable != null)
                {
                    List<RCSRoleInfo> existingRoles = RCSRoleInfo.GetByEndpointSystem(_endpoint.Id);
                    RCSRoleInfo rcsRoles = null;
                    List<RCSRoleInfo> newEntils = new List<RCSRoleInfo>();
                    EndPoint endpoint = EndPoint.GetById(this.EndPoint.Id, true);

                    var roleList = (from table in dataTable.AsEnumerable()
                                    group table by new
                                    {
                                        FNGROUP = table.Field<string>(0),
                                        RoleName = table.Field<string>(1),
                                        RoleDESC = table.Field<string>(2)
                                    } into g
                                    select new RCSRoleInfo
                                    {
                                        FunctionalGroup = g.Key.FNGROUP,
                                        RoleCode = g.Key.RoleName,
                                        RoleDesc = g.Key.RoleDESC
                                    }).ToList();

                    foreach (RCSRoleInfo currentWorkingRow in roleList)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(currentWorkingRow.RoleCode))
                            {
                                continue;
                            }
                            rcsRoles = existingRoles.Where(r => r.RoleCode == currentWorkingRow.RoleCode).FirstOrDefault();
                            if (rcsRoles == null)
                            {
                                rcsRoles = new RCSRoleInfo();
                                rcsRoles.RoleCode = currentWorkingRow.RoleCode.Trim();
                                rcsRoles.RoleDesc = currentWorkingRow.RoleDesc.Trim();
                                rcsRoles.FunctionalGroup = currentWorkingRow.FunctionalGroup.Trim();
                                rcsRoles.UserDefinedDisplayName = string.Format(CMessage.RCS_156, rcsRoles.FunctionalGroup, rcsRoles.RoleDesc);
                                rcsRoles.CreatedBy = CMessage.RCS_122;
                                rcsRoles.CreatedDate = DateTime.Now;
                                rcsRoles.CanRequest = true;
                                existingRoles.Add(rcsRoles);
                                newEntils.Add(rcsRoles);

                            }
                            else
                            {
                                rcsRoles.RoleDesc = currentWorkingRow.RoleDesc.Trim();
                                rcsRoles.FunctionalGroup = currentWorkingRow.FunctionalGroup.Trim();
                                rcsRoles.UserDefinedDisplayName = string.Format(CMessage.RCS_156, rcsRoles.FunctionalGroup, rcsRoles.RoleDesc);

                                rcsRoles.LastModifiedBy = CMessage.RCS_122;
                                rcsRoles.LastModifiedDate = DateTime.Now;
                            }

                            rcsRoles.EndPointSystemId = _endpoint.Id;
                            rcsRoles.Save(MasterConfigHelper.ServiceUser);
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, CMessage.RCS_130 + ex.Message);
                        }
                    }

                    if (newEntils != null && newEntils.Count > 0)
                    {
                        StringBuilder remarks = new StringBuilder();
                        remarks.AppendLine(string.Format("New Role(s) has been discovered for the Endpoint System - {0}", (endpoint != null ? endpoint.EndPointSystemType.DisplayName : "CMS")));
                        foreach (var ent in newEntils)
                        {
                            remarks.AppendLine(string.Format("{0} - {1}", ent.RoleCode, ent.RoleDesc));
                        }
                        DILog.AlertNotificationLogger.WriteLog((long)endpoint.Id, null, remarks.ToString(), AlertCenterNotificationType.Discovery);
                    }

                    //Delete files
                    base.AuthenticateNetworkAndDeleteAllFiles(filePath);
                }
                else
                {
                    this.DoRecon = false;
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, CMessage.RCS_130 + ex.Message, null, true);
            }
        }
        private void GetPrivillegeAcctTrans()
        {
            try
            {
                string mandatoryCols = CMessage.RCS_PRIV_MANDATORY_COLS;
                string readFileName = string.Empty;
                string filePath = GetConfiguration(EndpointKey.PrivillegeAcctTransFilePath);
                var dataTable = base.AuthenticateNetworkAndGetFileData(filePath, mandatoryCols, 0, false, true);

                if (dataTable != null)
                {
                    RCSPrivillegeAcctTransInfo privilTran = null;
                    //loop through the file data and create user if not already exists.    
                    foreach (DataRow currentWorkingRow in dataTable.Rows)
                    {
                        try
                        {
                            var activitydate = GetColumnValueByIndex(currentWorkingRow, 0);
                            var performedby = GetColumnValueByIndex(currentWorkingRow, 1);
                            var appliedon = GetColumnValueByIndex(currentWorkingRow, 2);
                            var actionTaken = GetColumnValueByIndex(currentWorkingRow, 3);
                            var activity = GetColumnValueByIndex(currentWorkingRow, 4);
                            var rcsIDASTnxId = GetColumnValueByIndex(currentWorkingRow, 5);

                            privilTran = new RCSPrivillegeAcctTransInfo();

                            privilTran.CreatedBy = CMessage.RCS_122;
                            privilTran.CreatedDate = DateTime.Now;

                            privilTran.IDASTnxID = !string.IsNullOrEmpty(rcsIDASTnxId) ? Convert.ToString(rcsIDASTnxId).Trim() : null;
                            privilTran.Activitydate = Convert.ToDateTime(activitydate);
                            privilTran.Performedby = !string.IsNullOrEmpty(performedby) ? Convert.ToString(performedby).Trim() : null;
                            privilTran.Appliedon = !string.IsNullOrEmpty(appliedon) ? Convert.ToString(appliedon).Trim() : null;
                            privilTran.Actiontaken = !string.IsNullOrEmpty(actionTaken) ? actionTaken.Trim() : null;
                            privilTran.Activity = !string.IsNullOrEmpty(activity) ? activity.Trim() : null;
                            privilTran.EndPointSystemId = _endpoint.Id;

                            privilTran.Save(MasterConfigHelper.ServiceUser);
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, CMessage.RCS_137 + ex.Message);
                        }
                    }

                    //Delete files
                    base.AuthenticateNetworkAndDeleteAllFiles(filePath);
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, CMessage.RCS_137 + ex.Message, null, true);
            }
        }
        private string GetColumnValue(DataRow currentRow, string columnName)
        {
            try
            {
                if (currentRow.Table.Columns.Contains(columnName))
                {
                    var value = currentRow[columnName].ToString().Trim();

                    if (value.ToUpper() == CMessage.RCS_138) return "";

                    return value;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, string.Format(CMessage.RCS_139, columnName), ex);
                return string.Empty;
            }
        }
        private string GetColumnValueByIndex(DataRow currentRow, int index)
        {
            try
            {
                if (currentRow.Table.Columns[index] != null)
                {
                    var value = currentRow[index].ToString().Trim();

                    if (value.ToUpper() == CMessage.RCS_138) return "";

                    return value;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, string.Format(CMessage.RCS_139, index), ex);
                return string.Empty;
            }
        }

        #endregion
        private void UpdateEntitlementReconcilation()
        {
            try
            {
                var dicUsers = RCSUserInfo.GetAllByEndPointSystemId(this._endpoint.Id);

                foreach (KeyValuePair<long, string> item in dicUsers)
                {
                    var rcsUserInfo = RCSUserInfo.GetByIdIncludeRCSRoles(item.Key);
                    if (rcsUserInfo.RCSRoles != null)
                    {
                        foreach (RCSRoleInfo entitlement in rcsUserInfo.RCSRoles)
                        {
                            var acctRecon = AccountReconcilation.GetByUser(rcsUserInfo.EndPointSystemId, rcsUserInfo.Id);

                            if (acctRecon == null) continue;

                            var entRecon = EntitlementReconcilation.GetByUser(rcsUserInfo.Id, rcsUserInfo.EndPointSystemId, entitlement.RoleCode.Trim(), EntitlementType.Role);
                            if (entRecon != null)
                            {
                                entRecon.LastModifiedBy = "System";
                                entRecon.LastModifiedDate = DateTime.Now;
                                entRecon.EntitlementId = entitlement.Id;
                                entRecon.Entitlement = entitlement.RoleCode;
                                entRecon.EntitlementDescription = entitlement.RoleDesc;
                                if (entRecon.IsDeleted)
                                {
                                    EntitlementReconcilation.UpdateException(entRecon, true);
                                }

                                entRecon.Save(MasterConfigHelper.ServiceUser);
                            }
                            else
                            {
                                entRecon = new EntitlementReconcilation();
                                var entitlements = (rcsUserInfo.Entitlements.Select(x => x.EntitlementName.ToString()));
                                entRecon = this.CreateEntitlementReconcilationShell(acctRecon, EntitlementType.Role, string.Join(",", entitlements != null ? entitlements.ToArray() : new string[] { }));
                                entRecon.Entitlement = entitlement.RoleCode;
                                entRecon.EntitlementDescription = entitlement.RoleDesc;
                                entRecon.EntitlementId = entitlement.Id;
                                EntitlementReconcilation.UpdateException(entRecon, true);
                                entRecon.CreatedBy = "System";
                                entRecon.CreatedDate = DateTime.Now;
                                entRecon.Save(MasterConfigHelper.ServiceUser);
                            }
                        }
                    }
                }
                EntitlementReconcilation.ProcessDeletedEntitlements(_endpoint.Id, this.Process, MasterConfigHelper.ServiceUser, true, this.FileTimeStamp);
            }
            catch (Exception ex)
            {
                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while updating Entitlement reconciliation.", ex);
            }
        }

        private void EntitlementRecon()
        {
            try
            {
                IList<RCSRoleInfo> deleteRoles = RCSRoleInfo.GetDeletedByEndPointSystemId(this._endpoint.Id, this.FileTimeStamp);
                // RCSRoleInfo.DeleteList(deleteRoles.ToList());

                if (deleteRoles != null && deleteRoles.Count > 0)
                {
                    RCSRoleInfo.RemoveRCSRole(deleteRoles.ToList());
                }

                UpdateEntitlementReconcilation();
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.RCS_ERROR_USERS, GetConfiguration(EndpointKey.IPAddress)), ex, true);

            }


        }
    }
}
