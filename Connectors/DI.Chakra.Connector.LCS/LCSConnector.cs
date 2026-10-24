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
using CConstant = DI.Chakra.Connector.LCS.Constant;
using CMessage = DI.Chakra.Connector.LCS.Message;

namespace DI.Chakra.Connector
{
    public class LCSConnector : ConnectorBase
    {
        #region Members
        private DomainModel.EndPoint _endpoint;
        private EndpointConfiguration _LCSConfiguration = null;
        #endregion
        public LCSConnector(DomainModel.EndPoint endpoint, AccessRequest currentRequest)
        {
            this._endpoint = endpoint;
            this.CurrentRequest = currentRequest;
            var LCSConfiguration = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);         
            LCSConfiguration.ConfigurationDetails.ForEach(lq =>
            {
                if (lq.EndpointConfigurationKey.KeyName == EndpointKey.Password.ToString())
                {
                    lq.Value = RSAHelper.DecryptData(lq.Value);
                }
            });
            this.LCSConfiguration = LCSConfiguration;
            this.EndpointConfiguration = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);
            this.NetworkUserName = GetConfiguration(EndpointKey.BizServiceUserName);
            this.NetworkPassword = GetConfiguration(EndpointKey.BizServicePassword);
        }
        public EndpointConfiguration LCSConfiguration
        {
            get { return _LCSConfiguration; }
            set { this._LCSConfiguration = value; }
        }
        private string GetConfiguration(EndpointKey Key)
        {
            var config = LCSConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.EndpointConfigurationKey.KeyName == Key.ToString());
            return config != null ? config.Value : "";
        }

        private string GetFileNameByDateTime(string sharedPath, string moduleName)
        {
            if (!string.IsNullOrEmpty(sharedPath) && !string.IsNullOrEmpty(moduleName))
            {
                string todaydateTime = DateTime.Now.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

                return string.Concat(sharedPath, "\\" + "LCS_" + moduleName + "_" + todaydateTime + ".csv");
            }

            return string.Empty;
        }

        public override IMProcessor IMProcessor
        {
            get
            {
                LCS.IMProcessor processor = new LCS.IMProcessor();
                processor._endpoint = this.EndPoint;
                processor._LCSConfiguration = this._LCSConfiguration;
                return processor;
            }
        }

        public override bool Connect()
        {
            //try
            //{

            //    if (File.Exists(GetConfiguration(EndpointKey.UserFilePath))
            //    && File.Exists(GetConfiguration(EndpointKey.RoleFilePath))
            //    && File.Exists(GetConfiguration(EndpointKey.FunctionFilePath))
            //    && File.Exists(GetConfiguration(EndpointKey.PrivillegeAcctTransFilePath)))
            //    {
            //        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.LCS_101, GetConfiguration(EndpointKey.IPAddress)));
            //        return true;
            //    }
            //    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.LCS_102, GetConfiguration(EndpointKey.IPAddress)));
            //    return false;
            //}
            //catch (Exception ex)
            //{
            //    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.LCS_103, GetConfiguration(EndpointKey.IPAddress)));
            //    return false;
            //}

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
                bool IsUsersSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_LCS_User) != null;
                bool IsRolesSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_LCS_ROLE) != null;
                bool IsFunctionSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_LCS_FUNCTION) != null;
                bool IsPrivillegeAcctTransSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_LCS_PrivillegeAcctTrans) != null;

                if (!MarkDiscoveryFail && IsRolesSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.LCS_110, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverRoles();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.LCS_111, GetConfiguration(EndpointKey.IPAddress)));
                }
                if (!MarkDiscoveryFail && IsFunctionSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.LCS_107, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverFunctions();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.LCS_108, GetConfiguration(EndpointKey.IPAddress)));
                }
                if (!MarkDiscoveryFail && IsUsersSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.LCS_105, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverUser();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.LCS_106, GetConfiguration(EndpointKey.IPAddress)));
                }
                if (!MarkDiscoveryFail && IsPrivillegeAcctTransSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.LCS_113, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverPrivillegeAcctTrans();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.LCS_114, GetConfiguration(EndpointKey.IPAddress)));
                }
                if (!MarkDiscoveryFail)
                {
                    if (this.DoRecon)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("Account Recon Started For LCS", GetConfiguration(EndpointKey.IPAddress)));
                        this.AccountRecon();
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("Account Recon Ended For LCS", GetConfiguration(EndpointKey.IPAddress)));

                        WriteDiscoveryLog(LogLevelType.Info, string.Format("Entitlement Recon Started For LCS", GetConfiguration(EndpointKey.IPAddress)));
                        this.EntitlementRecon();
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("Entitlement Recon Started For LCS", GetConfiguration(EndpointKey.IPAddress)));
                    }

                    LCSUserInfo.MapWithMasterUserForReconTable(_endpoint, LCSConfiguration.MapAll != null ? LCSConfiguration.MapAll : false);
                    LCSUserInfo.MapWithMasterUserForEntitlementReconTable(_endpoint, LCSConfiguration.MapAll != null ? LCSConfiguration.MapAll : false, discoveryProcessId);

                    base.DeleteLocalCopyFiles();
                }

                if (this.IsExceptionRaised || MarkDiscoveryFail)
                {
                    throw new Exception(string.Format("LCS discovery failed {0}", GetConfiguration(EndpointKey.IPAddress)));    
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
            LCSUserRole userRole = null;
            IList<LCSUserRole> LCSUserRole = new List<LCSUserRole>();
            try
            {
                long userId = 0;
                string mandatoryCols = CMessage.LCS_USERS_MANDATORY_COLS;
                string filePath = GetConfiguration(EndpointKey.UserFilePath);
                var dataTable = base.AuthenticateNetworkAndGetFileData(filePath, mandatoryCols, 3, true, false);

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
                                     select new LCSUserInfo
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
                                    select new LCSRoleInfo
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
                    LCSUserInfo.SaveUsers(usersList, _endpoint.Id, MasterConfigHelper.ServiceUser);

                    //Save Roles
                    LCSRoleInfo.SaveRoles(roleList, _endpoint.Id, MasterConfigHelper.ServiceUser);

                    List<LCSUserInfo> existingUsers = LCSUserInfo.GetByEndpointSystemIdIncludeEndPointSystem(_endpoint.Id).ToList();
                    List<LCSRoleInfo> existingRoles = LCSRoleInfo.GetByEndpointSystemIdIncludeEndPointSystem(_endpoint.Id).ToList();
                    LCSRoleInfo roleInfo = null;
                    LCSUserInfo userInfo = null;
                    List<LCSUserRole> userRoleList = new List<LCSUserRole>();
                    LCSUserRole userRoleItem = null;

                    int counter = 0;
                    foreach (DataRow currentWorkingRow in dataTable.Rows)
                    {
                        try
                        {
                            var LCSUserADID = Convert.ToString(GetColumnValueByIndex(currentWorkingRow, 0)).Trim();
                            var LCSRoleCode = Convert.ToString(GetColumnValueByIndex(currentWorkingRow, 1)).Trim();
                            var lastLogin = GetColumnValueByIndex(currentWorkingRow, 2);

                            if (string.IsNullOrEmpty(LCSUserADID))
                            {
                                continue;
                            }


                            if (!string.IsNullOrEmpty(LCSUserADID))
                            {
                                string[] adidWithoutDomain = LCSUserADID.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                                if (adidWithoutDomain.Count() > 1)
                                {
                                    LCSUserADID = adidWithoutDomain[1];
                                }
                                else
                                {
                                    LCSUserADID = adidWithoutDomain[0];
                                }
                            }

                            roleInfo = existingRoles.Where(r => r.RoleCode == LCSRoleCode).FirstOrDefault();
                            userInfo = existingUsers.Where(u => u.ADID == LCSUserADID).FirstOrDefault();

                            if (roleInfo != null && userInfo != null && !userRoleList.Any(u => u.UserId == userInfo.Id && u.RoleId == roleInfo.Id))
                            {
                                userRoleItem = new LCSUserRole();
                                userRoleItem.RoleId = roleInfo.Id;
                                userRoleItem.UserId = userInfo.Id;
                                userRoleList.Add(userRoleItem);
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, CMessage.LCS_120 + ex.Message);
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
                                executeQuery = string.Format(CMessage.LCS_118, item.UserId);
                                LCSRoleInfo.ExecuteQuery(executeQuery);
                            }

                            queryString.AppendLine(string.Format(CMessage.LCS_119, item.UserId, item.RoleId, this._endpoint.Id, Convert.ToInt16(VersionNo.Available)));
                            if (counter == 500)
                            {
                                LCSRoleInfo.ExecuteQuery(queryString.ToString());
                                queryString = new StringBuilder();
                                counter = 0;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(queryString.ToString()))
                    {
                        LCSRoleInfo.ExecuteQuery(queryString.ToString());
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
                WriteDiscoveryLog(LogLevelType.Error, CMessage.LCS_120 + ex.Message, null, true);
            }
        }

        private void AccountRecon()
        {
            try
            {
                List<LCSUserInfo> userList = LCSUserInfo.GetDeletedUsers(this._endpoint.Id, this.Process, this.FileTimeStamp).ToList();
                if (userList != null && userList.Count > 0)
                {
                    LCSUserInfo.RemoveLCSUser(userList.ToList());
                }

                AccountReconcilation.ProcessDeletedAccounts(this._endpoint.Id, this.Process, MasterConfigHelper.ServiceUser, true, this.FileTimeStamp);

            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.LCS_ERROR_LCS_USERS, GetConfiguration(EndpointKey.IPAddress)), ex, true);

            }
        }

        private void GetFunctions()
        {
            try
            {
                string mandatoryCols = CMessage.LCS_FUN_MANDATORY_COLS;
                string filePath = GetConfiguration(EndpointKey.FunctionFilePath);
                var dataTable = base.AuthenticateNetworkAndGetFileData(filePath, mandatoryCols, 10);

                if (dataTable != null)
                {
                    //Get Roles
                    var roleList = (from table in dataTable.AsEnumerable()
                                    group table by new
                                    {
                                        RoleName = table.Field<string>(0)
                                    } into g
                                    select new LCSRoleInfo
                                    {
                                        RoleCode = g.Key.RoleName,
                                        RoleDesc = g.Key.RoleName
                                    }).ToList();

                    //Get Functions
                    var functionList = (from table in dataTable.AsEnumerable()
                                        group table by new
                                        {
                                            RoleName = table.Field<string>(0),
                                            LCSFNCode = table.Field<string>(1),
                                            LCSFunctionDesc = table.Field<string>(2),
                                            LCSFNStatus = table.Field<string>(3),
                                            LCSView = table.Field<string>(4),
                                            LCSCreate = table.Field<string>(5),
                                            LCSModify = table.Field<string>(6),
                                            LCSDelete = table.Field<string>(7),
                                            LCSPrint = table.Field<string>(8),
                                            LCSExport = table.Field<string>(9)
                                        } into g
                                        select new LCSFunctionInfo
                                        {
                                            RoleCode = !string.IsNullOrEmpty(g.Key.RoleName) ? Convert.ToString(g.Key.RoleName).Trim() : null,
                                            FNCode = !string.IsNullOrEmpty(g.Key.LCSFNCode) ? Convert.ToString(g.Key.LCSFNCode).Trim() : null,
                                            FunctionDesc = !string.IsNullOrEmpty(g.Key.LCSFunctionDesc) ? Convert.ToString(g.Key.LCSFunctionDesc).Trim() : null,
                                            FNStatus = !string.IsNullOrEmpty(g.Key.LCSFNStatus) ? Convert.ToString(g.Key.LCSFNStatus).Trim() : null,
                                            View = string.Compare(Convert.ToString(g.Key.LCSView), "Y", true) == 0 ? true : false,
                                            Create = string.Compare(Convert.ToString(g.Key.LCSCreate), "Y", true) == 0 ? true : false,
                                            Modify = string.Compare(Convert.ToString(g.Key.LCSModify), "Y", true) == 0 ? true : false,
                                            Delete = string.Compare(Convert.ToString(g.Key.LCSDelete), "Y", true) == 0 ? true : false,
                                            Print = string.Compare(Convert.ToString(g.Key.LCSPrint), "Y", true) == 0 ? true : false,
                                            Export = string.Compare(Convert.ToString(g.Key.LCSExport), "Y", true) == 0 ? true : false,

                                        }).ToList();

                    //Save Roles
                    LCSRoleInfo.SaveRoles(roleList, _endpoint.Id, MasterConfigHelper.ServiceUser);

                    //SaveFunctions
                    LCSFunctionInfo.SaveFunctions(functionList, _endpoint.Id, MasterConfigHelper.ServiceUser);

                    List<LCSRoleInfo> existingRoles = LCSRoleInfo.GetByEndpointSystem(_endpoint.Id).ToList();
                    List<LCSFunctionInfo> existingFuncs = LCSFunctionInfo.GetByEndpointSystemIdIncludeEndPointSystem(_endpoint.Id).ToList();

                    LCSRoleInfo roleItem = null;
                    LCSFunctionInfo funcItem = null;

                    long batchCount = 0;
                    StringBuilder queryBuilder = new StringBuilder();
                    if (dataTable.Rows.Count > 0)
                    {
                        string query = string.Format(CMessage.LCS_DELETE_FUNCROLE_QUERY, _endpoint.Id);
                        LCSFunctionInfo.ExecuteQuery(query);
                    }

                    List<LCSFunctionRole> funRoleList = new List<LCSFunctionRole>();
                    LCSFunctionRole funcRoleItem = null;
                    foreach (DataRow currentWorkingRow in dataTable.Rows)
                    {
                        try
                        {

                            var LCSRoleCode = Convert.ToString(GetColumnValueByIndex(currentWorkingRow, 0)).Trim();
                            var LCSFNCode = Convert.ToString(GetColumnValueByIndex(currentWorkingRow, 1)).Trim();


                            if (string.IsNullOrEmpty(LCSFNCode) || string.IsNullOrEmpty(LCSRoleCode))
                            {
                                continue;
                            }

                            roleItem = existingRoles.Where(r => r.RoleCode == LCSRoleCode).FirstOrDefault();
                            funcItem = existingFuncs.Where(r => r.FNCode == LCSFNCode && r.RoleCode == LCSRoleCode).FirstOrDefault();

                            if (roleItem != null && funcItem != null && !funRoleList.Any(a => a.FunctionId == funcItem.Id && a.RoleId == roleItem.Id))
                            {
                                funcRoleItem = new LCSFunctionRole();
                                funcRoleItem.RoleId = roleItem.Id;
                                funcRoleItem.FunctionId = funcItem.Id;

                                funRoleList.Add(funcRoleItem);
                            }


                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, CMessage.LCS_124 + ex.Message);
                        }
                    }

                    if (funRoleList != null && funRoleList.Count() > 0)
                    {
                        funRoleList = funRoleList.Distinct().ToList();

                        foreach (var item in funRoleList)
                        {
                            batchCount++;
                            queryBuilder.AppendLine(string.Format(CMessage.LCS_INSERT_FUNCROLE_QUERY, item.RoleId, item.FunctionId, _endpoint.Id, Convert.ToInt16(VersionNo.Available)));

                            if (batchCount == 500)
                            {
                                LCSFunctionInfo.ExecuteQuery(queryBuilder.ToString());
                                queryBuilder = new StringBuilder();
                                batchCount = 0;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(queryBuilder.ToString()))
                    {
                        LCSFunctionInfo.ExecuteQuery(queryBuilder.ToString());
                    }

                    //Delete files
                    base.AuthenticateNetworkAndDeleteAllFiles(filePath);

                    IList<LCSFunctionInfo> deleteFuncs = LCSFunctionInfo.GetDeletedByEndPointSystemId(this._endpoint.Id);
                    LCSFunctionInfo.DeleteList(deleteFuncs.ToList());
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, CMessage.LCS_124 + ex.Message, null, true);
            }
        }
        private void GetRoles()
        {
            try
            {
                string mandatoryCols = CMessage.LCS_ROLES_MANDATORY_COLS;
                string filePath = GetConfiguration(EndpointKey.RoleFilePath);
                var dataTable = base.AuthenticateNetworkAndGetFileData(filePath, mandatoryCols, 3); //new TextDataConnect().ReadCSV(filePath, 3);

                if (dataTable != null)
                {
                    List<LCSRoleInfo> existingRoles = LCSRoleInfo.GetByEndpointSystem(_endpoint.Id);
                    LCSRoleInfo LCSRoles = null;
                    List<LCSRoleInfo> newEntils = new List<LCSRoleInfo>();
                    EndPoint endpoint = EndPoint.GetById(this.EndPoint.Id, true);

                    var roleList = (from table in dataTable.AsEnumerable()
                                    group table by new
                                    {
                                        FNGROUP = table.Field<string>(0),
                                        RoleName = table.Field<string>(1),
                                        RoleDESC = table.Field<string>(2)
                                    } into g
                                    select new LCSRoleInfo
                                    {
                                        FunctionalGroup = g.Key.FNGROUP,
                                        RoleCode = g.Key.RoleName,
                                        RoleDesc = g.Key.RoleDESC
                                    }).ToList();

                    foreach (LCSRoleInfo currentWorkingRow in roleList)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(currentWorkingRow.RoleCode))
                            {
                                continue;
                            }
                            LCSRoles = existingRoles.Where(r => r.RoleCode == currentWorkingRow.RoleCode).FirstOrDefault();
                            if (LCSRoles == null)
                            {
                                LCSRoles = new LCSRoleInfo();
                                LCSRoles.RoleCode = currentWorkingRow.RoleCode.Trim();
                                LCSRoles.RoleDesc = currentWorkingRow.RoleDesc.Trim();
                                LCSRoles.FunctionalGroup = currentWorkingRow.FunctionalGroup.Trim();
                                LCSRoles.UserDefinedDisplayName = string.Format(CMessage.LCS_156, LCSRoles.FunctionalGroup, LCSRoles.RoleDesc);
                                LCSRoles.CreatedBy = CMessage.LCS_122;
                                LCSRoles.CreatedDate = DateTime.Now;
                                LCSRoles.CanRequest = true;
                                existingRoles.Add(LCSRoles);
                                newEntils.Add(LCSRoles);

                            }
                            else
                            {
                                LCSRoles.RoleDesc = currentWorkingRow.RoleDesc.Trim();
                                LCSRoles.FunctionalGroup = currentWorkingRow.FunctionalGroup.Trim();
                                LCSRoles.UserDefinedDisplayName = string.Format(CMessage.LCS_156, LCSRoles.FunctionalGroup, LCSRoles.RoleDesc);

                                LCSRoles.LastModifiedBy = CMessage.LCS_122;
                                LCSRoles.LastModifiedDate = DateTime.Now;
                            }

                            LCSRoles.EndPointSystemId = _endpoint.Id;
                            LCSRoles.Save(MasterConfigHelper.ServiceUser);
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, CMessage.LCS_130 + ex.Message);
                        }
                    }

                    if (newEntils != null && newEntils.Count > 0)
                    {
                        StringBuilder remarks = new StringBuilder();
                        remarks.AppendLine(string.Format("New Role(s) has been discovered for the Endpoint System - {0}", (endpoint != null ? endpoint.EndPointSystemType.DisplayName : "LCS")));
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
                WriteDiscoveryLog(LogLevelType.Error, CMessage.LCS_130 + ex.Message, null, true);
            }
        }
        private void GetPrivillegeAcctTrans()
        {
            try
            {
                string mandatoryCols = CMessage.LCS_PRIV_MANDATORY_COLS;
                string readFileName = string.Empty;
                string filePath = GetConfiguration(EndpointKey.PrivillegeAcctTransFilePath);
                var dataTable = base.AuthenticateNetworkAndGetFileData(filePath, mandatoryCols, 6, false, true);

                if (dataTable != null)
                {
                    LCSPrivillegeAcctTransInfo privilTran = null;
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
                            var LCSIDASTnxId = GetColumnValueByIndex(currentWorkingRow, 5);

                            privilTran = new LCSPrivillegeAcctTransInfo();

                            privilTran.CreatedBy = CMessage.LCS_122;
                            privilTran.CreatedDate = DateTime.Now;

                            privilTran.IDASTnxID = !string.IsNullOrEmpty(LCSIDASTnxId) ? Convert.ToString(LCSIDASTnxId).Trim() : null;
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
                            WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, CMessage.LCS_137 + ex.Message);
                        }
                    }

                    //Delete files
                    base.AuthenticateNetworkAndDeleteAllFiles(filePath);
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, CMessage.LCS_137 + ex.Message, null, true);
            }
        }
        private string GetColumnValue(DataRow currentRow, string columnName)
        {
            try
            {
                if (currentRow.Table.Columns.Contains(columnName))
                {
                    var value = currentRow[columnName].ToString().Trim();

                    if (value.ToUpper() == CMessage.LCS_138) return "";

                    return value;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, string.Format(CMessage.LCS_139, columnName), ex);
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

                    if (value.ToUpper() == CMessage.LCS_138) return "";

                    return value;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, string.Format(CMessage.LCS_139, index), ex);
                return string.Empty;
            }
        }

        #endregion
        private void UpdateEntitlementReconcilation()
        {
            try
            {
                var dicUsers = LCSUserInfo.GetAllByEndPointSystemId(this._endpoint.Id);

                foreach (KeyValuePair<long, string> item in dicUsers)
                {
                    var cscUserInfo = LCSUserInfo.GetByIdIncludeLCSRoles(item.Key);
                    if (cscUserInfo.LCSRoles != null)
                    {
                        foreach (LCSRoleInfo entitlement in cscUserInfo.LCSRoles)
                        {
                            var acctRecon = AccountReconcilation.GetByUser(cscUserInfo.EndPointSystemId, cscUserInfo.Id);

                            if (acctRecon == null) continue;

                            var entRecon = EntitlementReconcilation.GetByUser(cscUserInfo.Id, cscUserInfo.EndPointSystemId, entitlement.RoleCode.Trim(), EntitlementType.Role);
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
                                var entitlements = (cscUserInfo.Entitlements.Select(x => x.EntitlementName.ToString()));
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
                IList<LCSRoleInfo> deleteRoles = LCSRoleInfo.GetDeletedByEndPointSystemId(this._endpoint.Id, this.FileTimeStamp);
                // LCSRoleInfo.DeleteList(deleteRoles.ToList());

                if (deleteRoles != null && deleteRoles.Count > 0)
                {
                    LCSRoleInfo.RemoveLCSRole(deleteRoles.ToList());
                }

                UpdateEntitlementReconcilation();
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.LCS_ERROR_LCS_USERS, GetConfiguration(EndpointKey.IPAddress)), ex, true);

            }


        }
    }
}
