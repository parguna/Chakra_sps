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
using CConstant = DI.Chakra.Connector.BizCMS.Constant;
using CMessage = DI.Chakra.Connector.BizCMS.Message;

namespace DI.Chakra.Connector
{
    public class BizCMSConnector : ConnectorBase
    {
        #region Members
        private DomainModel.EndPoint _endpoint;
        private EndpointConfiguration _bizCMSConfiguration = null;
        #endregion
        public BizCMSConnector(DomainModel.EndPoint endpoint, AccessRequest currentRequest)
        {
            this._endpoint = endpoint;
            this.CurrentRequest = currentRequest;
            var bizCMSConfiguration = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);         
            bizCMSConfiguration.ConfigurationDetails.ForEach(lq =>
            {
                if (lq.EndpointConfigurationKey.KeyName == EndpointKey.Password.ToString())
                {
                    lq.Value = RSAHelper.DecryptData(lq.Value);
                }
            });
            this.BizCMSConfiguration = bizCMSConfiguration;
            this.EndpointConfiguration = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);
            this.NetworkUserName = GetConfiguration(EndpointKey.BizServiceUserName);
            this.NetworkPassword = GetConfiguration(EndpointKey.BizServicePassword);
        }
        public EndpointConfiguration BizCMSConfiguration
        {
            get { return _bizCMSConfiguration; }
            set { this._bizCMSConfiguration = value; }
        }
        private string GetConfiguration(EndpointKey Key)
        {
            var config = BizCMSConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.EndpointConfigurationKey.KeyName == Key.ToString());
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
                BizCMS.IMProcessor processor = new BizCMS.IMProcessor();
                processor._endpoint = this.EndPoint;
                processor._bizCMSConfiguration = this._bizCMSConfiguration;
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
            //        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.BizCMS_101, GetConfiguration(EndpointKey.IPAddress)));
            //        return true;
            //    }
            //    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.BizCMS_102, GetConfiguration(EndpointKey.IPAddress)));
            //    return false;
            //}
            //catch (Exception ex)
            //{
            //    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.BizCMS_103, GetConfiguration(EndpointKey.IPAddress)));
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
                bool IsUsersSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_BIZCMS_User) != null;
                bool IsRolesSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_BIZCMS_ROLE) != null;
                bool IsFunctionSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_BIZCMS_FUNCTION) != null;
                bool IsPrivillegeAcctTransSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_BizCMS_PrivillegeAcctTrans) != null;

                if (!MarkDiscoveryFail && IsRolesSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.BizCMS_110, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverRoles();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.BizCMS_111, GetConfiguration(EndpointKey.IPAddress)));
                }
                if (!MarkDiscoveryFail && IsFunctionSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.BizCMS_107, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverFunctions();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.BizCMS_108, GetConfiguration(EndpointKey.IPAddress)));
                }
                if (!MarkDiscoveryFail && IsUsersSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.BizCMS_105, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverUser();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.BizCMS_106, GetConfiguration(EndpointKey.IPAddress)));
                }
                if (!MarkDiscoveryFail && IsPrivillegeAcctTransSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.BizCMS_113, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverPrivillegeAcctTrans();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.BizCMS_114, GetConfiguration(EndpointKey.IPAddress)));
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

                    BizCMSUserInfo.MapWithMasterUserForReconTable(_endpoint, BizCMSConfiguration.MapAll != null ? BizCMSConfiguration.MapAll : false);
                    BizCMSUserInfo.MapWithMasterUserForEntitlementReconTable(_endpoint, BizCMSConfiguration.MapAll != null ? BizCMSConfiguration.MapAll : false, discoveryProcessId);

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
            BizCMSUserRole userRole = null;
            IList<BizCMSUserRole> bizCMSUserRole = new List<BizCMSUserRole>();
            try
            {
                long userId = 0;
                string mandatoryCols = CMessage.BizCMS_USERS_MANDATORY_COLS;
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
                                     select new BizCMSUserInfo
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
                                    select new BizCMSRoleInfo
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
                    BizCMSUserInfo.SaveUsers(usersList, _endpoint.Id, MasterConfigHelper.ServiceUser);

                    //Save Roles
                    BizCMSRoleInfo.SaveRoles(roleList, _endpoint.Id, MasterConfigHelper.ServiceUser);

                    List<BizCMSUserInfo> existingUsers = BizCMSUserInfo.GetByEndpointSystemIdIncludeEndPointSystem(_endpoint.Id).ToList();
                    List<BizCMSRoleInfo> existingRoles = BizCMSRoleInfo.GetByEndpointSystemIdIncludeEndPointSystem(_endpoint.Id).ToList();
                    BizCMSRoleInfo roleInfo = null;
                    BizCMSUserInfo userInfo = null;
                    List<BizCMSUserRole> userRoleList = new List<BizCMSUserRole>();
                    BizCMSUserRole userRoleItem = null;

                    int counter = 0;
                    foreach (DataRow currentWorkingRow in dataTable.Rows)
                    {
                        try
                        {
                            var bizCMSUserADID = Convert.ToString(GetColumnValueByIndex(currentWorkingRow, 0)).Trim();
                            var bizCMSRoleCode = Convert.ToString(GetColumnValueByIndex(currentWorkingRow, 1)).Trim();
                            var lastLogin = GetColumnValueByIndex(currentWorkingRow, 2);

                            if (string.IsNullOrEmpty(bizCMSUserADID))
                            {
                                continue;
                            }


                            if (!string.IsNullOrEmpty(bizCMSUserADID))
                            {
                                string[] adidWithoutDomain = bizCMSUserADID.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                                if (adidWithoutDomain.Count() > 1)
                                {
                                    bizCMSUserADID = adidWithoutDomain[1];
                                }
                                else
                                {
                                    bizCMSUserADID = adidWithoutDomain[0];
                                }
                            }

                            roleInfo = existingRoles.Where(r => r.RoleCode == bizCMSRoleCode).FirstOrDefault();
                            userInfo = existingUsers.Where(u => u.ADID == bizCMSUserADID).FirstOrDefault();

                            if (roleInfo != null && userInfo != null && !userRoleList.Any(u => u.UserId == userInfo.Id && u.RoleId == roleInfo.Id))
                            {
                                userRoleItem = new BizCMSUserRole();
                                userRoleItem.RoleId = roleInfo.Id;
                                userRoleItem.UserId = userInfo.Id;
                                userRoleList.Add(userRoleItem);
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, CMessage.BizCMS_120 + ex.Message);
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
                                executeQuery = string.Format(CMessage.BizCMS_118, item.UserId);
                                BizCMSRoleInfo.ExecuteQuery(executeQuery);
                            }

                            queryString.AppendLine(string.Format(CMessage.BizCMS_119, item.UserId, item.RoleId, this._endpoint.Id, Convert.ToInt16(VersionNo.Available)));
                            if (counter == 500)
                            {
                                BizCMSRoleInfo.ExecuteQuery(queryString.ToString());
                                queryString = new StringBuilder();
                                counter = 0;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(queryString.ToString()))
                    {
                        BizCMSRoleInfo.ExecuteQuery(queryString.ToString());
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
                WriteDiscoveryLog(LogLevelType.Error, CMessage.BizCMS_120 + ex.Message, null, true);
            }
        }

        private void AccountRecon()
        {
            try
            {
                List<BizCMSUserInfo> userList = BizCMSUserInfo.GetDeletedUsers(this._endpoint.Id, this.Process, this.FileTimeStamp).ToList();
                if (userList != null && userList.Count > 0)
                {
                    BizCMSUserInfo.RemoveCMSUser(userList.ToList());
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
                string mandatoryCols = CMessage.BizCMS_FUN_MANDATORY_COLS;
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
                                    select new BizCMSRoleInfo
                                    {
                                        RoleCode = g.Key.RoleName,
                                        RoleDesc = g.Key.RoleName
                                    }).ToList();

                    //Get Functions
                    var functionList = (from table in dataTable.AsEnumerable()
                                        group table by new
                                        {
                                            RoleName = table.Field<string>(0),
                                            bizCMSFNCode = table.Field<string>(1),
                                            bizCMSFunctionDesc = table.Field<string>(2),
                                            bizCMSFNStatus = table.Field<string>(3),
                                            bizCMSView = table.Field<string>(4),
                                            bizCMSCreate = table.Field<string>(5),
                                            bizCMSModify = table.Field<string>(6),
                                            bizCMSDelete = table.Field<string>(7),
                                            bizCMSPrint = table.Field<string>(8),
                                            bizCMSExport = table.Field<string>(9)
                                        } into g
                                        select new BizCMSFunctionInfo
                                        {
                                            RoleCode = !string.IsNullOrEmpty(g.Key.RoleName) ? Convert.ToString(g.Key.RoleName).Trim() : null,
                                            FNCode = !string.IsNullOrEmpty(g.Key.bizCMSFNCode) ? Convert.ToString(g.Key.bizCMSFNCode).Trim() : null,
                                            FunctionDesc = !string.IsNullOrEmpty(g.Key.bizCMSFunctionDesc) ? Convert.ToString(g.Key.bizCMSFunctionDesc).Trim() : null,
                                            FNStatus = !string.IsNullOrEmpty(g.Key.bizCMSFNStatus) ? Convert.ToString(g.Key.bizCMSFNStatus).Trim() : null,
                                            View = string.Compare(Convert.ToString(g.Key.bizCMSView), "Y", true) == 0 ? true : false,
                                            Create = string.Compare(Convert.ToString(g.Key.bizCMSCreate), "Y", true) == 0 ? true : false,
                                            Modify = string.Compare(Convert.ToString(g.Key.bizCMSModify), "Y", true) == 0 ? true : false,
                                            Delete = string.Compare(Convert.ToString(g.Key.bizCMSDelete), "Y", true) == 0 ? true : false,
                                            Print = string.Compare(Convert.ToString(g.Key.bizCMSPrint), "Y", true) == 0 ? true : false,
                                            Export = string.Compare(Convert.ToString(g.Key.bizCMSExport), "Y", true) == 0 ? true : false,

                                        }).ToList();

                    //Save Roles
                    BizCMSRoleInfo.SaveRoles(roleList, _endpoint.Id, MasterConfigHelper.ServiceUser);

                    //SaveFunctions
                    BizCMSFunctionInfo.SaveFunctions(functionList, _endpoint.Id, MasterConfigHelper.ServiceUser);

                    List<BizCMSRoleInfo> existingRoles = BizCMSRoleInfo.GetByEndpointSystem(_endpoint.Id).ToList();
                    List<BizCMSFunctionInfo> existingFuncs = BizCMSFunctionInfo.GetByEndpointSystemIdIncludeEndPointSystem(_endpoint.Id).ToList();

                    BizCMSRoleInfo roleItem = null;
                    BizCMSFunctionInfo funcItem = null;

                    long batchCount = 0;
                    StringBuilder queryBuilder = new StringBuilder();
                    if (dataTable.Rows.Count > 0)
                    {
                        string query = string.Format(CMessage.BizCMS_DELETE_FUNCROLE_QUERY, _endpoint.Id);
                        BizCMSFunctionInfo.ExecuteQuery(query);
                    }

                    List<BizCMSFunctionRole> funRoleList = new List<BizCMSFunctionRole>();
                    BizCMSFunctionRole funcRoleItem = null;
                    foreach (DataRow currentWorkingRow in dataTable.Rows)
                    {
                        try
                        {

                            var bizCMSRoleCode = Convert.ToString(GetColumnValueByIndex(currentWorkingRow, 0)).Trim();
                            var bizCMSFNCode = Convert.ToString(GetColumnValueByIndex(currentWorkingRow, 1)).Trim();


                            if (string.IsNullOrEmpty(bizCMSFNCode) || string.IsNullOrEmpty(bizCMSRoleCode))
                            {
                                continue;
                            }

                            roleItem = existingRoles.Where(r => r.RoleCode == bizCMSRoleCode).FirstOrDefault();
                            funcItem = existingFuncs.Where(r => r.FNCode == bizCMSFNCode && r.RoleCode == bizCMSRoleCode).FirstOrDefault();

                            if (roleItem != null && funcItem != null && !funRoleList.Any(a => a.FunctionId == funcItem.Id && a.RoleId == roleItem.Id))
                            {
                                funcRoleItem = new BizCMSFunctionRole();
                                funcRoleItem.RoleId = roleItem.Id;
                                funcRoleItem.FunctionId = funcItem.Id;

                                funRoleList.Add(funcRoleItem);
                            }


                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, CMessage.BizCMS_124 + ex.Message);
                        }
                    }

                    if (funRoleList != null && funRoleList.Count() > 0)
                    {
                        funRoleList = funRoleList.Distinct().ToList();

                        foreach (var item in funRoleList)
                        {
                            batchCount++;
                            queryBuilder.AppendLine(string.Format(CMessage.BizCMS_INSERT_FUNCROLE_QUERY, item.RoleId, item.FunctionId, _endpoint.Id, Convert.ToInt16(VersionNo.Available)));

                            if (batchCount == 500)
                            {
                                BizCMSFunctionInfo.ExecuteQuery(queryBuilder.ToString());
                                queryBuilder = new StringBuilder();
                                batchCount = 0;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(queryBuilder.ToString()))
                    {
                        BizCMSFunctionInfo.ExecuteQuery(queryBuilder.ToString());
                    }

                    //Delete files
                    base.AuthenticateNetworkAndDeleteAllFiles(filePath);

                    IList<BizCMSFunctionInfo> deleteFuncs = BizCMSFunctionInfo.GetDeletedByEndPointSystemId(this._endpoint.Id);
                    BizCMSFunctionInfo.DeleteList(deleteFuncs.ToList());
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, CMessage.BizCMS_124 + ex.Message, null, true);
            }
        }
        private void GetRoles()
        {
            try
            {
                string mandatoryCols = CMessage.BizCMS_ROLES_MANDATORY_COLS;
                string filePath = GetConfiguration(EndpointKey.RoleFilePath);
                var dataTable = base.AuthenticateNetworkAndGetFileData(filePath, mandatoryCols, 3); //new TextDataConnect().ReadCSV(filePath, 3);

                if (dataTable != null)
                {
                    List<BizCMSRoleInfo> existingRoles = BizCMSRoleInfo.GetByEndpointSystem(_endpoint.Id);
                    BizCMSRoleInfo bizCMSRoles = null;
                    List<BizCMSRoleInfo> newEntils = new List<BizCMSRoleInfo>();
                    EndPoint endpoint = EndPoint.GetById(this.EndPoint.Id, true);

                    var roleList = (from table in dataTable.AsEnumerable()
                                    group table by new
                                    {
                                        FNGROUP = table.Field<string>(0),
                                        RoleName = table.Field<string>(1),
                                        RoleDESC = table.Field<string>(2)
                                    } into g
                                    select new BizCMSRoleInfo
                                    {
                                        FunctionalGroup = g.Key.FNGROUP,
                                        RoleCode = g.Key.RoleName,
                                        RoleDesc = g.Key.RoleDESC
                                    }).ToList();

                    foreach (BizCMSRoleInfo currentWorkingRow in roleList)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(currentWorkingRow.RoleCode))
                            {
                                continue;
                            }
                            bizCMSRoles = existingRoles.Where(r => r.RoleCode == currentWorkingRow.RoleCode).FirstOrDefault();
                            if (bizCMSRoles == null)
                            {
                                bizCMSRoles = new BizCMSRoleInfo();
                                bizCMSRoles.RoleCode = currentWorkingRow.RoleCode.Trim();
                                bizCMSRoles.RoleDesc = currentWorkingRow.RoleDesc.Trim();
                                bizCMSRoles.FunctionalGroup = currentWorkingRow.FunctionalGroup.Trim();
                                bizCMSRoles.UserDefinedDisplayName = string.Format(CMessage.BizCMS_156, bizCMSRoles.FunctionalGroup, bizCMSRoles.RoleDesc);
                                bizCMSRoles.CreatedBy = CMessage.BizCMS_122;
                                bizCMSRoles.CreatedDate = DateTime.Now;
                                bizCMSRoles.CanRequest = true;
                                existingRoles.Add(bizCMSRoles);
                                newEntils.Add(bizCMSRoles);

                            }
                            else
                            {
                                bizCMSRoles.RoleDesc = currentWorkingRow.RoleDesc.Trim();
                                bizCMSRoles.FunctionalGroup = currentWorkingRow.FunctionalGroup.Trim();
                                bizCMSRoles.UserDefinedDisplayName = string.Format(CMessage.BizCMS_156, bizCMSRoles.FunctionalGroup, bizCMSRoles.RoleDesc);

                                bizCMSRoles.LastModifiedBy = CMessage.BizCMS_122;
                                bizCMSRoles.LastModifiedDate = DateTime.Now;
                            }

                            bizCMSRoles.EndPointSystemId = _endpoint.Id;
                            bizCMSRoles.Save(MasterConfigHelper.ServiceUser);
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, CMessage.BizCMS_130 + ex.Message);
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
                WriteDiscoveryLog(LogLevelType.Error, CMessage.BizCMS_130 + ex.Message, null, true);
            }
        }
        private void GetPrivillegeAcctTrans()
        {
            try
            {
                string mandatoryCols = CMessage.BizCMS_PRIV_MANDATORY_COLS;
                string readFileName = string.Empty;
                string filePath = GetConfiguration(EndpointKey.PrivillegeAcctTransFilePath);
                var dataTable = base.AuthenticateNetworkAndGetFileData(filePath, mandatoryCols, 6, false, true);

                if (dataTable != null)
                {
                    BizCMSPrivillegeAcctTransInfo privilTran = null;
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
                            var bizCMSIDASTnxId = GetColumnValueByIndex(currentWorkingRow, 5);

                            privilTran = new BizCMSPrivillegeAcctTransInfo();

                            privilTran.CreatedBy = CMessage.BizCMS_122;
                            privilTran.CreatedDate = DateTime.Now;

                            privilTran.IDASTnxID = !string.IsNullOrEmpty(bizCMSIDASTnxId) ? Convert.ToString(bizCMSIDASTnxId).Trim() : null;
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
                            WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, CMessage.BizCMS_137 + ex.Message);
                        }
                    }

                    //Delete files
                    base.AuthenticateNetworkAndDeleteAllFiles(filePath);
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, CMessage.BizCMS_137 + ex.Message, null, true);
            }
        }
        private string GetColumnValue(DataRow currentRow, string columnName)
        {
            try
            {
                if (currentRow.Table.Columns.Contains(columnName))
                {
                    var value = currentRow[columnName].ToString().Trim();

                    if (value.ToUpper() == CMessage.BizCMS_138) return "";

                    return value;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, string.Format(CMessage.BizCMS_139, columnName), ex);
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

                    if (value.ToUpper() == CMessage.BizCMS_138) return "";

                    return value;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, string.Format(CMessage.BizCMS_139, index), ex);
                return string.Empty;
            }
        }

        #endregion
        private void UpdateEntitlementReconcilation()
        {
            try
            {
                var dicUsers = BizCMSUserInfo.GetAllByEndPointSystemId(this._endpoint.Id);

                foreach (KeyValuePair<long, string> item in dicUsers)
                {
                    var bizCMSUserInfo = BizCMSUserInfo.GetByIdIncludeBizCMSRoles(item.Key);
                    if (bizCMSUserInfo.BizCMSRoles != null)
                    {
                        foreach (BizCMSRoleInfo entitlement in bizCMSUserInfo.BizCMSRoles)
                        {
                            var acctRecon = AccountReconcilation.GetByUser(bizCMSUserInfo.EndPointSystemId, bizCMSUserInfo.Id);

                            if (acctRecon == null) continue;

                            var entRecon = EntitlementReconcilation.GetByUser(bizCMSUserInfo.Id, bizCMSUserInfo.EndPointSystemId, entitlement.RoleCode.Trim(), EntitlementType.Role);
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
                                var entitlements = (bizCMSUserInfo.Entitlements.Select(x => x.EntitlementName.ToString()));
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
                IList<BizCMSRoleInfo> deleteRoles = BizCMSRoleInfo.GetDeletedByEndPointSystemId(this._endpoint.Id, this.FileTimeStamp);
                // BizCMSRoleInfo.DeleteList(deleteRoles.ToList());

                if (deleteRoles != null && deleteRoles.Count > 0)
                {
                    BizCMSRoleInfo.RemoveCMSRole(deleteRoles.ToList());
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
