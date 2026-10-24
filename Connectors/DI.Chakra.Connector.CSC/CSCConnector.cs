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
using CConstant = DI.Chakra.Connector.CSC.Constant;
using CMessage = DI.Chakra.Connector.CSC.Message;

namespace DI.Chakra.Connector
{
    public class CSCConnector : ConnectorBase
    {
        #region Members
        private DomainModel.EndPoint _endpoint;
        private EndpointConfiguration _CSCConfiguration = null;
        #endregion
        public CSCConnector(DomainModel.EndPoint endpoint, AccessRequest currentRequest)
        {
            this._endpoint = endpoint;
            this.CurrentRequest = currentRequest;
            var CSCConfiguration = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);         
            CSCConfiguration.ConfigurationDetails.ForEach(lq =>
            {
                if (lq.EndpointConfigurationKey.KeyName == EndpointKey.Password.ToString())
                {
                    lq.Value = RSAHelper.DecryptData(lq.Value);
                }
            });
            this.CSCConfiguration = CSCConfiguration;
            this.EndpointConfiguration = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);
            this.NetworkUserName = GetConfiguration(EndpointKey.BizServiceUserName);
            this.NetworkPassword = GetConfiguration(EndpointKey.BizServicePassword);
        }
        public EndpointConfiguration CSCConfiguration
        {
            get { return _CSCConfiguration; }
            set { this._CSCConfiguration = value; }
        }
        private string GetConfiguration(EndpointKey Key)
        {
            var config = CSCConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.EndpointConfigurationKey.KeyName == Key.ToString());
            return config != null ? config.Value : "";
        }

        private string GetFileNameByDateTime(string sharedPath, string moduleName)
        {
            if (!string.IsNullOrEmpty(sharedPath) && !string.IsNullOrEmpty(moduleName))
            {
                string todaydateTime = DateTime.Now.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

                return string.Concat(sharedPath, "\\" + "CSC_" + moduleName + "_" + todaydateTime + ".csv");
            }

            return string.Empty;
        }

        public override IMProcessor IMProcessor
        {
            get
            {
                CSC.IMProcessor processor = new CSC.IMProcessor();
                processor._endpoint = this.EndPoint;
                processor._CSCConfiguration = this._CSCConfiguration;
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
            //        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.CSC_101, GetConfiguration(EndpointKey.IPAddress)));
            //        return true;
            //    }
            //    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.CSC_102, GetConfiguration(EndpointKey.IPAddress)));
            //    return false;
            //}
            //catch (Exception ex)
            //{
            //    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.CSC_103, GetConfiguration(EndpointKey.IPAddress)));
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
                bool IsUsersSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_CSC_User) != null;
                bool IsRolesSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_CSC_ROLE) != null;
                bool IsFunctionSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_CSC_FUNCTION) != null;
                bool IsPrivillegeAcctTransSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_CSC_PrivillegeAcctTrans) != null;

                if (!MarkDiscoveryFail && IsRolesSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.CSC_110, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverRoles();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.CSC_111, GetConfiguration(EndpointKey.IPAddress)));
                }
                if (!MarkDiscoveryFail && IsFunctionSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.CSC_107, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverFunctions();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.CSC_108, GetConfiguration(EndpointKey.IPAddress)));
                }
                if (!MarkDiscoveryFail && IsUsersSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.CSC_105, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverUser();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.CSC_106, GetConfiguration(EndpointKey.IPAddress)));
                }
                if (!MarkDiscoveryFail && IsPrivillegeAcctTransSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.CSC_113, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverPrivillegeAcctTrans();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.CSC_114, GetConfiguration(EndpointKey.IPAddress)));
                }
                if (!MarkDiscoveryFail)
                {
                    if (this.DoRecon)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("Account Recon Started For CSC", GetConfiguration(EndpointKey.IPAddress)));
                        this.AccountRecon();
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("Account Recon Ended For CSC", GetConfiguration(EndpointKey.IPAddress)));

                        WriteDiscoveryLog(LogLevelType.Info, string.Format("Entitlement Recon Started For CSC", GetConfiguration(EndpointKey.IPAddress)));
                        this.EntitlementRecon();
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("Entitlement Recon Started For CSC", GetConfiguration(EndpointKey.IPAddress)));
                    }

                    CSCUserInfo.MapWithMasterUserForReconTable(_endpoint, CSCConfiguration.MapAll != null ? CSCConfiguration.MapAll : false);
                    CSCUserInfo.MapWithMasterUserForEntitlementReconTable(_endpoint, CSCConfiguration.MapAll != null ? CSCConfiguration.MapAll : false, discoveryProcessId);

                    base.DeleteLocalCopyFiles();
                }

                if (this.IsExceptionRaised || MarkDiscoveryFail)
                {
                    throw new Exception(string.Format("CSC discovery failed {0}", GetConfiguration(EndpointKey.IPAddress)));    
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
            CSCUserRole userRole = null;
            IList<CSCUserRole> CSCUserRole = new List<CSCUserRole>();
            try
            {
                long userId = 0;
                string mandatoryCols = CMessage.CSC_USERS_MANDATORY_COLS;
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
                                     select new CSCUserInfo
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
                                    select new CSCRoleInfo
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
                    CSCUserInfo.SaveUsers(usersList, _endpoint.Id, MasterConfigHelper.ServiceUser);

                    //Save Roles
                    CSCRoleInfo.SaveRoles(roleList, _endpoint.Id, MasterConfigHelper.ServiceUser);

                    List<CSCUserInfo> existingUsers = CSCUserInfo.GetByEndpointSystemIdIncludeEndPointSystem(_endpoint.Id).ToList();
                    List<CSCRoleInfo> existingRoles = CSCRoleInfo.GetByEndpointSystemIdIncludeEndPointSystem(_endpoint.Id).ToList();
                    CSCRoleInfo roleInfo = null;
                    CSCUserInfo userInfo = null;
                    List<CSCUserRole> userRoleList = new List<CSCUserRole>();
                    CSCUserRole userRoleItem = null;

                    int counter = 0;
                    foreach (DataRow currentWorkingRow in dataTable.Rows)
                    {
                        try
                        {
                            var CSCUserADID = Convert.ToString(GetColumnValueByIndex(currentWorkingRow, 0)).Trim();
                            var CSCRoleCode = Convert.ToString(GetColumnValueByIndex(currentWorkingRow, 1)).Trim();
                            var lastLogin = GetColumnValueByIndex(currentWorkingRow, 2);

                            if (string.IsNullOrEmpty(CSCUserADID))
                            {
                                continue;
                            }


                            if (!string.IsNullOrEmpty(CSCUserADID))
                            {
                                string[] adidWithoutDomain = CSCUserADID.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                                if (adidWithoutDomain.Count() > 1)
                                {
                                    CSCUserADID = adidWithoutDomain[1];
                                }
                                else
                                {
                                    CSCUserADID = adidWithoutDomain[0];
                                }
                            }

                            roleInfo = existingRoles.Where(r => r.RoleCode == CSCRoleCode).FirstOrDefault();
                            userInfo = existingUsers.Where(u => u.ADID == CSCUserADID).FirstOrDefault();

                            if (roleInfo != null && userInfo != null && !userRoleList.Any(u => u.UserId == userInfo.Id && u.RoleId == roleInfo.Id))
                            {
                                userRoleItem = new CSCUserRole();
                                userRoleItem.RoleId = roleInfo.Id;
                                userRoleItem.UserId = userInfo.Id;
                                userRoleList.Add(userRoleItem);
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, CMessage.CSC_120 + ex.Message);
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
                                executeQuery = string.Format(CMessage.CSC_118, item.UserId);
                                CSCRoleInfo.ExecuteQuery(executeQuery);
                            }

                            queryString.AppendLine(string.Format(CMessage.CSC_119, item.UserId, item.RoleId, this._endpoint.Id, Convert.ToInt16(VersionNo.Available)));
                            if (counter == 500)
                            {
                                CSCRoleInfo.ExecuteQuery(queryString.ToString());
                                queryString = new StringBuilder();
                                counter = 0;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(queryString.ToString()))
                    {
                        CSCRoleInfo.ExecuteQuery(queryString.ToString());
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
                WriteDiscoveryLog(LogLevelType.Error, CMessage.CSC_120 + ex.Message, null, true);
            }
        }

        private void AccountRecon()
        {
            try
            {
                List<CSCUserInfo> userList = CSCUserInfo.GetDeletedUsers(this._endpoint.Id, this.Process, this.FileTimeStamp).ToList();
                if (userList != null && userList.Count > 0)
                {
                    CSCUserInfo.RemoveCSCUser(userList.ToList());
                }

                AccountReconcilation.ProcessDeletedAccounts(this._endpoint.Id, this.Process, MasterConfigHelper.ServiceUser, true, this.FileTimeStamp);

            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.CSC_ERROR_CSC_USERS, GetConfiguration(EndpointKey.IPAddress)), ex, true);

            }
        }

        private void GetFunctions()
        {
            try
            {
                string mandatoryCols = CMessage.CSC_FUN_MANDATORY_COLS;
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
                                    select new CSCRoleInfo
                                    {
                                        RoleCode = g.Key.RoleName,
                                        RoleDesc = g.Key.RoleName
                                    }).ToList();

                    //Get Functions
                    var functionList = (from table in dataTable.AsEnumerable()
                                        group table by new
                                        {
                                            RoleName = table.Field<string>(0),
                                            CSCFNCode = table.Field<string>(1),
                                            CSCFunctionDesc = table.Field<string>(2),
                                            CSCFNStatus = table.Field<string>(3),
                                            CSCView = table.Field<string>(4),
                                            CSCCreate = table.Field<string>(5),
                                            CSCModify = table.Field<string>(6),
                                            CSCDelete = table.Field<string>(7),
                                            CSCPrint = table.Field<string>(8),
                                            CSCExport = table.Field<string>(9)
                                        } into g
                                        select new CSCFunctionInfo
                                        {
                                            RoleCode = !string.IsNullOrEmpty(g.Key.RoleName) ? Convert.ToString(g.Key.RoleName).Trim() : null,
                                            FNCode = !string.IsNullOrEmpty(g.Key.CSCFNCode) ? Convert.ToString(g.Key.CSCFNCode).Trim() : null,
                                            FunctionDesc = !string.IsNullOrEmpty(g.Key.CSCFunctionDesc) ? Convert.ToString(g.Key.CSCFunctionDesc).Trim() : null,
                                            FNStatus = !string.IsNullOrEmpty(g.Key.CSCFNStatus) ? Convert.ToString(g.Key.CSCFNStatus).Trim() : null,
                                            View = string.Compare(Convert.ToString(g.Key.CSCView), "Y", true) == 0 ? true : false,
                                            Create = string.Compare(Convert.ToString(g.Key.CSCCreate), "Y", true) == 0 ? true : false,
                                            Modify = string.Compare(Convert.ToString(g.Key.CSCModify), "Y", true) == 0 ? true : false,
                                            Delete = string.Compare(Convert.ToString(g.Key.CSCDelete), "Y", true) == 0 ? true : false,
                                            Print = string.Compare(Convert.ToString(g.Key.CSCPrint), "Y", true) == 0 ? true : false,
                                            Export = string.Compare(Convert.ToString(g.Key.CSCExport), "Y", true) == 0 ? true : false,

                                        }).ToList();

                    //Save Roles
                    CSCRoleInfo.SaveRoles(roleList, _endpoint.Id, MasterConfigHelper.ServiceUser);

                    //SaveFunctions
                    CSCFunctionInfo.SaveFunctions(functionList, _endpoint.Id, MasterConfigHelper.ServiceUser);

                    List<CSCRoleInfo> existingRoles = CSCRoleInfo.GetByEndpointSystem(_endpoint.Id).ToList();
                    List<CSCFunctionInfo> existingFuncs = CSCFunctionInfo.GetByEndpointSystemIdIncludeEndPointSystem(_endpoint.Id).ToList();

                    CSCRoleInfo roleItem = null;
                    CSCFunctionInfo funcItem = null;

                    long batchCount = 0;
                    StringBuilder queryBuilder = new StringBuilder();
                    if (dataTable.Rows.Count > 0)
                    {
                        string query = string.Format(CMessage.CSC_DELETE_FUNCROLE_QUERY, _endpoint.Id);
                        CSCFunctionInfo.ExecuteQuery(query);
                    }

                    List<CSCFunctionRole> funRoleList = new List<CSCFunctionRole>();
                    CSCFunctionRole funcRoleItem = null;
                    foreach (DataRow currentWorkingRow in dataTable.Rows)
                    {
                        try
                        {

                            var CSCRoleCode = Convert.ToString(GetColumnValueByIndex(currentWorkingRow, 0)).Trim();
                            var CSCFNCode = Convert.ToString(GetColumnValueByIndex(currentWorkingRow, 1)).Trim();


                            if (string.IsNullOrEmpty(CSCFNCode) || string.IsNullOrEmpty(CSCRoleCode))
                            {
                                continue;
                            }

                            roleItem = existingRoles.Where(r => r.RoleCode == CSCRoleCode).FirstOrDefault();
                            funcItem = existingFuncs.Where(r => r.FNCode == CSCFNCode && r.RoleCode == CSCRoleCode).FirstOrDefault();

                            if (roleItem != null && funcItem != null && !funRoleList.Any(a => a.FunctionId == funcItem.Id && a.RoleId == roleItem.Id))
                            {
                                funcRoleItem = new CSCFunctionRole();
                                funcRoleItem.RoleId = roleItem.Id;
                                funcRoleItem.FunctionId = funcItem.Id;

                                funRoleList.Add(funcRoleItem);
                            }


                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, CMessage.CSC_124 + ex.Message);
                        }
                    }

                    if (funRoleList != null && funRoleList.Count() > 0)
                    {
                        funRoleList = funRoleList.Distinct().ToList();

                        foreach (var item in funRoleList)
                        {
                            batchCount++;
                            queryBuilder.AppendLine(string.Format(CMessage.CSC_INSERT_FUNCROLE_QUERY, item.RoleId, item.FunctionId, _endpoint.Id, Convert.ToInt16(VersionNo.Available)));

                            if (batchCount == 500)
                            {
                                CSCFunctionInfo.ExecuteQuery(queryBuilder.ToString());
                                queryBuilder = new StringBuilder();
                                batchCount = 0;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(queryBuilder.ToString()))
                    {
                        CSCFunctionInfo.ExecuteQuery(queryBuilder.ToString());
                    }

                    //Delete files
                    base.AuthenticateNetworkAndDeleteAllFiles(filePath);

                    IList<CSCFunctionInfo> deleteFuncs = CSCFunctionInfo.GetDeletedByEndPointSystemId(this._endpoint.Id);
                    CSCFunctionInfo.DeleteList(deleteFuncs.ToList());
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, CMessage.CSC_124 + ex.Message, null, true);
            }
        }
        private void GetRoles()
        {
            try
            {
                string mandatoryCols = CMessage.CSC_ROLES_MANDATORY_COLS;
                string filePath = GetConfiguration(EndpointKey.RoleFilePath);
                var dataTable = base.AuthenticateNetworkAndGetFileData(filePath, mandatoryCols, 3); //new TextDataConnect().ReadCSV(filePath, 3);

                if (dataTable != null)
                {
                    List<CSCRoleInfo> existingRoles = CSCRoleInfo.GetByEndpointSystem(_endpoint.Id);
                    CSCRoleInfo CSCRoles = null;
                    List<CSCRoleInfo> newEntils = new List<CSCRoleInfo>();
                    EndPoint endpoint = EndPoint.GetById(this.EndPoint.Id, true);

                    var roleList = (from table in dataTable.AsEnumerable()
                                    group table by new
                                    {
                                        FNGROUP = table.Field<string>(0),
                                        RoleName = table.Field<string>(1),
                                        RoleDESC = table.Field<string>(2)
                                    } into g
                                    select new CSCRoleInfo
                                    {
                                        FunctionalGroup = g.Key.FNGROUP,
                                        RoleCode = g.Key.RoleName,
                                        RoleDesc = g.Key.RoleDESC
                                    }).ToList();

                    foreach (CSCRoleInfo currentWorkingRow in roleList)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(currentWorkingRow.RoleCode))
                            {
                                continue;
                            }
                            CSCRoles = existingRoles.Where(r => r.RoleCode == currentWorkingRow.RoleCode).FirstOrDefault();
                            if (CSCRoles == null)
                            {
                                CSCRoles = new CSCRoleInfo();
                                CSCRoles.RoleCode = currentWorkingRow.RoleCode.Trim();
                                CSCRoles.RoleDesc = currentWorkingRow.RoleDesc.Trim();
                                CSCRoles.FunctionalGroup = currentWorkingRow.FunctionalGroup.Trim();
                                CSCRoles.UserDefinedDisplayName = string.Format(CMessage.CSC_156, CSCRoles.FunctionalGroup, CSCRoles.RoleDesc);
                                CSCRoles.CreatedBy = CMessage.CSC_122;
                                CSCRoles.CreatedDate = DateTime.Now;
                                CSCRoles.CanRequest = true;
                                existingRoles.Add(CSCRoles);
                                newEntils.Add(CSCRoles);

                            }
                            else
                            {
                                CSCRoles.RoleDesc = currentWorkingRow.RoleDesc.Trim();
                                CSCRoles.FunctionalGroup = currentWorkingRow.FunctionalGroup.Trim();
                                CSCRoles.UserDefinedDisplayName = string.Format(CMessage.CSC_156, CSCRoles.FunctionalGroup, CSCRoles.RoleDesc);

                                CSCRoles.LastModifiedBy = CMessage.CSC_122;
                                CSCRoles.LastModifiedDate = DateTime.Now;
                            }

                            CSCRoles.EndPointSystemId = _endpoint.Id;
                            CSCRoles.Save(MasterConfigHelper.ServiceUser);
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, CMessage.CSC_130 + ex.Message);
                        }
                    }

                    if (newEntils != null && newEntils.Count > 0)
                    {
                        StringBuilder remarks = new StringBuilder();
                        remarks.AppendLine(string.Format("New Role(s) has been discovered for the Endpoint System - {0}", (endpoint != null ? endpoint.EndPointSystemType.DisplayName : "CSC")));
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
                WriteDiscoveryLog(LogLevelType.Error, CMessage.CSC_130 + ex.Message, null, true);
            }
        }
        private void GetPrivillegeAcctTrans()
        {
            try
            {
                string mandatoryCols = CMessage.CSC_PRIV_MANDATORY_COLS;
                string readFileName = string.Empty;
                string filePath = GetConfiguration(EndpointKey.PrivillegeAcctTransFilePath);
                var dataTable = base.AuthenticateNetworkAndGetFileData(filePath, mandatoryCols, 6, false, true);

                if (dataTable != null)
                {
                    CSCPrivillegeAcctTransInfo privilTran = null;
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
                            var CSCIDASTnxId = GetColumnValueByIndex(currentWorkingRow, 5);

                            privilTran = new CSCPrivillegeAcctTransInfo();

                            privilTran.CreatedBy = CMessage.CSC_122;
                            privilTran.CreatedDate = DateTime.Now;

                            privilTran.IDASTnxID = !string.IsNullOrEmpty(CSCIDASTnxId) ? Convert.ToString(CSCIDASTnxId).Trim() : null;
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
                            WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, CMessage.CSC_137 + ex.Message);
                        }
                    }

                    //Delete files
                    base.AuthenticateNetworkAndDeleteAllFiles(filePath);
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, CMessage.CSC_137 + ex.Message, null, true);
            }
        }
        private string GetColumnValue(DataRow currentRow, string columnName)
        {
            try
            {
                if (currentRow.Table.Columns.Contains(columnName))
                {
                    var value = currentRow[columnName].ToString().Trim();

                    if (value.ToUpper() == CMessage.CSC_138) return "";

                    return value;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, string.Format(CMessage.CSC_139, columnName), ex);
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

                    if (value.ToUpper() == CMessage.CSC_138) return "";

                    return value;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, string.Format(CMessage.CSC_139, index), ex);
                return string.Empty;
            }
        }

        #endregion
        private void UpdateEntitlementReconcilation()
        {
            try
            {
                var dicUsers = CSCUserInfo.GetAllByEndPointSystemId(this._endpoint.Id);

                foreach (KeyValuePair<long, string> item in dicUsers)
                {
                    var cscUserInfo = CSCUserInfo.GetByIdIncludeCSCRoles(item.Key);
                    if (cscUserInfo.CSCRoles != null)
                    {
                        foreach (CSCRoleInfo entitlement in cscUserInfo.CSCRoles)
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
                IList<CSCRoleInfo> deleteRoles = CSCRoleInfo.GetDeletedByEndPointSystemId(this._endpoint.Id, this.FileTimeStamp);
                // CSCRoleInfo.DeleteList(deleteRoles.ToList());

                if (deleteRoles != null && deleteRoles.Count > 0)
                {
                    CSCRoleInfo.RemoveCSCRole(deleteRoles.ToList());
                }

                UpdateEntitlementReconcilation();
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.CSC_ERROR_CSC_USERS, GetConfiguration(EndpointKey.IPAddress)), ex, true);

            }


        }
    }
}
