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
using CConstant = DI.Chakra.Connector.WMS.Constant;
using CMessage = DI.Chakra.Connector.WMS.Message;

namespace DI.Chakra.Connector
{
    public class WMSConnector : ConnectorBase
    {
        #region Members
        private DomainModel.EndPoint _endpoint;
        private EndpointConfiguration _wmsConfiguration = null;
        #endregion
        public WMSConnector(DomainModel.EndPoint endpoint, AccessRequest currentRequest)
        {
            this._endpoint = endpoint;
            this.CurrentRequest = currentRequest;
            var wmsConfiguration = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);         
            wmsConfiguration.ConfigurationDetails.ForEach(lq =>
            {
                if (lq.EndpointConfigurationKey.KeyName == EndpointKey.Password.ToString())
                {
                    lq.Value = RSAHelper.DecryptData(lq.Value);
                }
            });
            this.WMSConfiguration = wmsConfiguration;
            this.EndpointConfiguration = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);
            this.NetworkUserName = GetConfiguration(EndpointKey.BizServiceUserName);
            this.NetworkPassword = GetConfiguration(EndpointKey.BizServicePassword);
        }
        public EndpointConfiguration WMSConfiguration
        {
            get { return _wmsConfiguration; }
            set { this._wmsConfiguration = value; }
        }
        private string GetConfiguration(EndpointKey Key)
        {
            var config = WMSConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.EndpointConfigurationKey.KeyName == Key.ToString());
            return config != null ? config.Value : "";
        }

        private string GetFileNameByDateTime(string sharedPath, string moduleName)
        {
            if (!string.IsNullOrEmpty(sharedPath) && !string.IsNullOrEmpty(moduleName))
            {
                string todaydateTime = DateTime.Now.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

                return string.Concat(sharedPath, "\\" + "CMS_" + moduleName + "_" + todaydateTime + ".csv");
            }

            return string.Empty;
        }

        public override IMProcessor IMProcessor
        {
            get
            {
                WMS.IMProcessor processor = new WMS.IMProcessor();
                processor._endpoint = this.EndPoint;
                processor._wmsConfiguration = this._wmsConfiguration;
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
            //        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.WMS_101, GetConfiguration(EndpointKey.IPAddress)));
            //        return true;
            //    }
            //    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.WMS_102, GetConfiguration(EndpointKey.IPAddress)));
            //    return false;
            //}
            //catch (Exception ex)
            //{
            //    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.WMS_103, GetConfiguration(EndpointKey.IPAddress)));
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
                bool IsUsersSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_WMS_User) != null;
                bool IsRolesSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_WMS_ROLE) != null;
                bool IsFunctionSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_WMS_FUNCTION) != null;
                bool IsPrivillegeAcctTransSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_WMS_PRIVILLEGEACCTTRANS) != null;

                if (!MarkDiscoveryFail && IsRolesSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.WMS_110, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverRoles();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.WMS_111, GetConfiguration(EndpointKey.IPAddress)));
                }
                if (!MarkDiscoveryFail && IsFunctionSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.WMS_107, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverFunctions();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.WMS_108, GetConfiguration(EndpointKey.IPAddress)));
                }
                if (!MarkDiscoveryFail && IsUsersSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.WMS_105, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverUser();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.WMS_106, GetConfiguration(EndpointKey.IPAddress)));
                }
                if (!MarkDiscoveryFail && IsPrivillegeAcctTransSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.WMS_113, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverPrivillegeAcctTrans();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.WMS_114, GetConfiguration(EndpointKey.IPAddress)));
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

                    WMSUserInfo.MapWithMasterUserForReconTable(_endpoint, WMSConfiguration.MapAll != null ? WMSConfiguration.MapAll : false);
                    WMSUserInfo.MapWithMasterUserForEntitlementReconTable(_endpoint, WMSConfiguration.MapAll != null ? WMSConfiguration.MapAll : false, discoveryProcessId);

                    base.DeleteLocalCopyFiles();
                }

                if (this.IsExceptionRaised || MarkDiscoveryFail)
                {
                    throw new Exception(string.Format("CMS discovery failed {0}", GetConfiguration(EndpointKey.IPAddress)));    
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
            WMSUserRole userRole = null;
            IList<WMSUserRole> wmsUserRole = new List<WMSUserRole>();
            try
            {
                long userId = 0;
                string mandatoryCols = CMessage.WMS_USERS_MANDATORY_COLS;
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
                                     select new WMSUserInfo
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
                                    select new WMSRoleInfo
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
                    WMSUserInfo.SaveUsers(usersList, _endpoint.Id, MasterConfigHelper.ServiceUser);

                    //Save Roles
                    WMSRoleInfo.SaveRoles(roleList, _endpoint.Id, MasterConfigHelper.ServiceUser);

                    List<WMSUserInfo> existingUsers = WMSUserInfo.GetByEndpointSystemIdIncludeEndPointSystem(_endpoint.Id).ToList();
                    List<WMSRoleInfo> existingRoles = WMSRoleInfo.GetByEndpointSystemIdIncludeEndPointSystem(_endpoint.Id).ToList();
                    WMSRoleInfo roleInfo = null;
                    WMSUserInfo userInfo = null;
                    List<WMSUserRole> userRoleList = new List<WMSUserRole>();
                    WMSUserRole userRoleItem = null;

                    int counter = 0;
                    foreach (DataRow currentWorkingRow in dataTable.Rows)
                    {
                        try
                        {
                            var wmsUserADID = Convert.ToString(GetColumnValueByIndex(currentWorkingRow, 0)).Trim();
                            var wmsRoleCode = Convert.ToString(GetColumnValueByIndex(currentWorkingRow, 1)).Trim();
                            var lastLogin = GetColumnValueByIndex(currentWorkingRow, 2);

                            if (string.IsNullOrEmpty(wmsUserADID))
                            {
                                continue;
                            }


                            if (!string.IsNullOrEmpty(wmsUserADID))
                            {
                                string[] adidWithoutDomain = wmsUserADID.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                                if (adidWithoutDomain.Count() > 1)
                                {
                                    wmsUserADID = adidWithoutDomain[1];
                                }
                                else
                                {
                                    wmsUserADID = adidWithoutDomain[0];
                                }
                            }

                            roleInfo = existingRoles.Where(r => r.RoleCode == wmsRoleCode).FirstOrDefault();
                            userInfo = existingUsers.Where(u => u.ADID == wmsUserADID).FirstOrDefault();

                            if (roleInfo != null && userInfo != null && !userRoleList.Any(u => u.UserId == userInfo.Id && u.RoleId == roleInfo.Id))
                            {
                                userRoleItem = new WMSUserRole();
                                userRoleItem.RoleId = roleInfo.Id;
                                userRoleItem.UserId = userInfo.Id;
                                userRoleList.Add(userRoleItem);
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, CMessage.WMS_120 + ex.Message);
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
                                executeQuery = string.Format(CMessage.WMS_118, item.UserId);
                                WMSRoleInfo.ExecuteQuery(executeQuery);
                            }

                            queryString.AppendLine(string.Format(CMessage.WMS_119, item.UserId, item.RoleId, this._endpoint.Id, Convert.ToInt16(VersionNo.Available)));
                            if (counter == 500)
                            {
                                WMSRoleInfo.ExecuteQuery(queryString.ToString());
                                queryString = new StringBuilder();
                                counter = 0;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(queryString.ToString()))
                    {
                        WMSRoleInfo.ExecuteQuery(queryString.ToString());
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
                WriteDiscoveryLog(LogLevelType.Error, CMessage.WMS_120 + ex.Message, null, true);
            }
        }

        private void AccountRecon()
        {
            try
            {
                List<WMSUserInfo> userList = WMSUserInfo.GetDeletedUsers(this._endpoint.Id, this.Process, this.FileTimeStamp).ToList();
                if (userList != null && userList.Count > 0)
                {
                    WMSUserInfo.RemoveCMSUser(userList.ToList());
                }

                AccountReconcilation.ProcessDeletedAccounts(this._endpoint.Id, this.Process, MasterConfigHelper.ServiceUser, true, this.FileTimeStamp);

            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.BIZCMS_ERROR_CMS_USERS, GetConfiguration(EndpointKey.IPAddress)), ex, true);

            }
        }

        private void GetFunctions()
        {
            try
            {
                string mandatoryCols = CMessage.WMS_FUN_MANDATORY_COLS;
                string filePath = GetConfiguration(EndpointKey.FunctionFilePath);
                var dataTable = base.AuthenticateNetworkAndGetFileData(filePath, mandatoryCols);

                if (dataTable != null)
                {
                    //Get Roles
                    var roleList = (from table in dataTable.AsEnumerable()
                                    group table by new
                                    {
                                        RoleName = table.Field<string>(0)
                                    } into g
                                    select new WMSRoleInfo
                                    {
                                        RoleCode = g.Key.RoleName,
                                        RoleDesc = g.Key.RoleName
                                    }).ToList();

                    //Get Functions
                    var functionList = (from table in dataTable.AsEnumerable()
                                        group table by new
                                        {
                                            RoleName = table.Field<string>(0),
                                            wmsFNCode = table.Field<string>(1),
                                            wmsFunctionDesc = table.Field<string>(2),
                                            wmsFNStatus = table.Field<string>(3),
                                            wmsView = table.Field<string>(4),
                                            wmsCreate = table.Field<string>(5),
                                            wmsModify = table.Field<string>(6),
                                            wmsDelete = table.Field<string>(7),
                                            wmsPrint = table.Field<string>(8),
                                            wmsExport = table.Field<string>(9)
                                        } into g
                                        select new WMSFunctionInfo
                                        {
                                            RoleCode = !string.IsNullOrEmpty(g.Key.RoleName) ? Convert.ToString(g.Key.RoleName).Trim() : null,
                                            FNCode = !string.IsNullOrEmpty(g.Key.wmsFNCode) ? Convert.ToString(g.Key.wmsFNCode).Trim() : null,
                                            FunctionDesc = !string.IsNullOrEmpty(g.Key.wmsFunctionDesc) ? Convert.ToString(g.Key.wmsFunctionDesc).Trim() : null,
                                            FNStatus = !string.IsNullOrEmpty(g.Key.wmsFNStatus) ? Convert.ToString(g.Key.wmsFNStatus).Trim() : null,
                                            View = string.Compare(Convert.ToString(g.Key.wmsView), "Y", true) == 0 ? true : false,
                                            Create = string.Compare(Convert.ToString(g.Key.wmsCreate), "Y", true) == 0 ? true : false,
                                            Modify = string.Compare(Convert.ToString(g.Key.wmsModify), "Y", true) == 0 ? true : false,
                                            Delete = string.Compare(Convert.ToString(g.Key.wmsDelete), "Y", true) == 0 ? true : false,
                                            Print = string.Compare(Convert.ToString(g.Key.wmsPrint), "Y", true) == 0 ? true : false,
                                            Export = string.Compare(Convert.ToString(g.Key.wmsExport), "Y", true) == 0 ? true : false,

                                        }).ToList();

                    //Save Roles
                    WMSRoleInfo.SaveRoles(roleList, _endpoint.Id, MasterConfigHelper.ServiceUser);

                    //SaveFunctions
                    WMSFunctionInfo.SaveFunctions(functionList, _endpoint.Id, MasterConfigHelper.ServiceUser);

                    List<WMSRoleInfo> existingRoles = WMSRoleInfo.GetByEndpointSystem(_endpoint.Id).ToList();
                    List<WMSFunctionInfo> existingFuncs = WMSFunctionInfo.GetByEndpointSystemIdIncludeEndPointSystem(_endpoint.Id).ToList();

                    WMSRoleInfo roleItem = null;
                    WMSFunctionInfo funcItem = null;

                    long batchCount = 0;
                    StringBuilder queryBuilder = new StringBuilder();
                    if (dataTable.Rows.Count > 0)
                    {
                        string query = string.Format(CMessage.WMS_DELETE_FUNCROLE_QUERY, _endpoint.Id);
                        WMSFunctionInfo.ExecuteQuery(query);
                    }

                    List<WMSFunctionRole> funRoleList = new List<WMSFunctionRole>();
                    WMSFunctionRole funcRoleItem = null;
                    foreach (DataRow currentWorkingRow in dataTable.Rows)
                    {
                        try
                        {

                            var wmsRoleCode = Convert.ToString(GetColumnValueByIndex(currentWorkingRow, 0)).Trim();
                            var wmsFNCode = Convert.ToString(GetColumnValueByIndex(currentWorkingRow, 1)).Trim();


                            if (string.IsNullOrEmpty(wmsFNCode) || string.IsNullOrEmpty(wmsRoleCode))
                            {
                                continue;
                            }

                            roleItem = existingRoles.Where(r => r.RoleCode == wmsRoleCode).FirstOrDefault();
                            funcItem = existingFuncs.Where(r => r.FNCode == wmsFNCode && r.RoleCode == wmsRoleCode).FirstOrDefault();

                            if (roleItem != null && funcItem != null && !funRoleList.Any(a => a.FunctionId == funcItem.Id && a.RoleId == roleItem.Id))
                            {
                                funcRoleItem = new WMSFunctionRole();
                                funcRoleItem.RoleId = roleItem.Id;
                                funcRoleItem.FunctionId = funcItem.Id;

                                funRoleList.Add(funcRoleItem);
                            }


                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, CMessage.WMS_124 + ex.Message);
                        }
                    }

                    if (funRoleList != null && funRoleList.Count() > 0)
                    {
                        funRoleList = funRoleList.Distinct().ToList();

                        foreach (var item in funRoleList)
                        {
                            batchCount++;
                            queryBuilder.AppendLine(string.Format(CMessage.WMS_INSERT_FUNCROLE_QUERY, item.RoleId, item.FunctionId, _endpoint.Id, Convert.ToInt16(VersionNo.Available)));

                            if (batchCount == 500)
                            {
                                WMSFunctionInfo.ExecuteQuery(queryBuilder.ToString());
                                queryBuilder = new StringBuilder();
                                batchCount = 0;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(queryBuilder.ToString()))
                    {
                        WMSFunctionInfo.ExecuteQuery(queryBuilder.ToString());
                    }

                    //Delete files
                    base.AuthenticateNetworkAndDeleteAllFiles(filePath);

                    IList<WMSFunctionInfo> deleteFuncs = WMSFunctionInfo.GetDeletedByEndPointSystemId(this._endpoint.Id);
                    WMSFunctionInfo.DeleteList(deleteFuncs.ToList());
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, CMessage.WMS_124 + ex.Message, null, true);
            }
        }
        private void GetRoles()
        {
            try
            {
                string mandatoryCols = CMessage.WMS_ROLES_MANDATORY_COLS;
                string filePath = GetConfiguration(EndpointKey.RoleFilePath);
                var dataTable = base.AuthenticateNetworkAndGetFileData(filePath, mandatoryCols); //new TextDataConnect().ReadCSV(filePath, 3);

                if (dataTable != null)
                {
                    List<WMSRoleInfo> existingRoles = WMSRoleInfo.GetByEndpointSystem(_endpoint.Id);
                    WMSRoleInfo wmsRoles = null;
                    List<WMSRoleInfo> newEntils = new List<WMSRoleInfo>();
                    EndPoint endpoint = EndPoint.GetById(this.EndPoint.Id, true);

                    var roleList = (from table in dataTable.AsEnumerable()
                                    group table by new
                                    {
                                        FNGROUP = table.Field<string>(0),
                                        RoleName = table.Field<string>(1),
                                        RoleDESC = table.Field<string>(2)
                                    } into g
                                    select new WMSRoleInfo
                                    {
                                        FunctionalGroup = g.Key.FNGROUP,
                                        RoleCode = g.Key.RoleName,
                                        RoleDesc = g.Key.RoleDESC
                                    }).ToList();

                    foreach (WMSRoleInfo currentWorkingRow in roleList)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(currentWorkingRow.RoleCode))
                            {
                                continue;
                            }
                            wmsRoles = existingRoles.Where(r => r.RoleCode == currentWorkingRow.RoleCode).FirstOrDefault();
                            if (wmsRoles == null)
                            {
                                wmsRoles = new WMSRoleInfo();
                                wmsRoles.RoleCode = currentWorkingRow.RoleCode.Trim();
                                wmsRoles.RoleDesc = currentWorkingRow.RoleDesc.Trim();
                                wmsRoles.FunctionalGroup = currentWorkingRow.FunctionalGroup.Trim();
                                wmsRoles.UserDefinedDisplayName = string.Format(CMessage.WMS_156, wmsRoles.FunctionalGroup, wmsRoles.RoleDesc);
                                wmsRoles.CreatedBy = CMessage.WMS_122;
                                wmsRoles.CreatedDate = DateTime.Now;
                                wmsRoles.CanRequest = true;
                                existingRoles.Add(wmsRoles);
                                newEntils.Add(wmsRoles);

                            }
                            else
                            {
                                wmsRoles.RoleDesc = currentWorkingRow.RoleDesc.Trim();
                                wmsRoles.FunctionalGroup = currentWorkingRow.FunctionalGroup.Trim();
                                wmsRoles.UserDefinedDisplayName = string.Format(CMessage.WMS_156, wmsRoles.FunctionalGroup, wmsRoles.RoleDesc);

                                wmsRoles.LastModifiedBy = CMessage.WMS_122;
                                wmsRoles.LastModifiedDate = DateTime.Now;
                            }

                            wmsRoles.EndPointSystemId = _endpoint.Id;
                            wmsRoles.Save(MasterConfigHelper.ServiceUser);
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, CMessage.WMS_130 + ex.Message);
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
                WriteDiscoveryLog(LogLevelType.Error, CMessage.WMS_130 + ex.Message, null, true);
            }
        }
        private void GetPrivillegeAcctTrans()
        {
            try
            {
                string mandatoryCols = CMessage.WMS_PRIV_MANDATORY_COLS;
                string readFileName = string.Empty;
                string filePath = GetConfiguration(EndpointKey.PrivillegeAcctTransFilePath);
                var dataTable = base.AuthenticateNetworkAndGetFileData(filePath, mandatoryCols, 0, false, true);

                if (dataTable != null)
                {
                    WMSPrivillegeAcctTransInfo privilTran = null;
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
                            var wmsIDASTnxId = GetColumnValueByIndex(currentWorkingRow, 5);

                            privilTran = new WMSPrivillegeAcctTransInfo();

                            privilTran.CreatedBy = CMessage.WMS_122;
                            privilTran.CreatedDate = DateTime.Now;

                            privilTran.IDASTnxID = !string.IsNullOrEmpty(wmsIDASTnxId) ? Convert.ToString(wmsIDASTnxId).Trim() : null;
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
                            WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, CMessage.WMS_137 + ex.Message);
                        }
                    }

                    //Delete files
                    base.AuthenticateNetworkAndDeleteAllFiles(filePath);
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, CMessage.WMS_137 + ex.Message, null, true);
            }
        }
        private string GetColumnValue(DataRow currentRow, string columnName)
        {
            try
            {
                if (currentRow.Table.Columns.Contains(columnName))
                {
                    var value = currentRow[columnName].ToString().Trim();

                    if (value.ToUpper() == CMessage.WMS_138) return "";

                    return value;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, string.Format(CMessage.WMS_139, columnName), ex);
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

                    if (value.ToUpper() == CMessage.WMS_138) return "";

                    return value;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, string.Format(CMessage.WMS_139, index), ex);
                return string.Empty;
            }
        }

        #endregion
        private void UpdateEntitlementReconcilation()
        {
            try
            {
                var dicUsers = WMSUserInfo.GetAllByEndPointSystemId(this._endpoint.Id);

                foreach (KeyValuePair<long, string> item in dicUsers)
                {
                    var wmsUserInfo = WMSUserInfo.GetByIdIncludeWMSRoles(item.Key);
                    if (wmsUserInfo.WMSRoles != null)
                    {
                        foreach (WMSRoleInfo entitlement in wmsUserInfo.WMSRoles)
                        {
                            var acctRecon = AccountReconcilation.GetByUser(wmsUserInfo.EndPointSystemId, wmsUserInfo.Id);

                            if (acctRecon == null) continue;

                            var entRecon = EntitlementReconcilation.GetByUser(wmsUserInfo.Id, wmsUserInfo.EndPointSystemId, entitlement.RoleCode.Trim(), EntitlementType.Role);
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
                                var entitlements = (wmsUserInfo.Entitlements.Select(x => x.EntitlementName.ToString()));
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
                IList<WMSRoleInfo> deleteRoles = WMSRoleInfo.GetDeletedByEndPointSystemId(this._endpoint.Id, this.FileTimeStamp);
                // WMSRoleInfo.DeleteList(deleteRoles.ToList());

                if (deleteRoles != null && deleteRoles.Count > 0)
                {
                    WMSRoleInfo.RemoveCMSRole(deleteRoles.ToList());
                }

                UpdateEntitlementReconcilation();
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.BIZCMS_ERROR_CMS_USERS, GetConfiguration(EndpointKey.IPAddress)), ex, true);

            }


        }
    }
}
