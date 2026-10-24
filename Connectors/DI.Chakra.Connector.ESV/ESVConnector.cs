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
using CConstant = DI.Chakra.Connector.ESV.Constant;
using CMessage = DI.Chakra.Connector.ESV.Message;

namespace DI.Chakra.Connector
{
    public class ESVConnector : ConnectorBase
    {
        #region Members
        private DomainModel.EndPoint _endpoint;
        private EndpointConfiguration _esvConfiguration = null;
        #endregion
        public ESVConnector(DomainModel.EndPoint endpoint, AccessRequest currentRequest)
        {
            this._endpoint = endpoint;
            this.EndPoint = endpoint;
            this.CurrentRequest = currentRequest;
            var esvConfiguration = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);
            esvConfiguration.ConfigurationDetails.ForEach(lq =>
            {
                if (lq.EndpointConfigurationKey.KeyName == EndpointKey.Password.ToString())
                {
                    lq.Value = RSAHelper.DecryptData(lq.Value);
                }
            });
            this.ESVConfiguration = esvConfiguration;
            this.EndpointConfiguration = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);
            this.NetworkUserName = GetConfiguration(EndpointKey.BizServiceUserName);
            this.NetworkPassword = GetConfiguration(EndpointKey.BizServicePassword);
        }
        public EndpointConfiguration ESVConfiguration
        {
            get { return _esvConfiguration; }
            set { this._esvConfiguration = value; }
        }
        private string GetConfiguration(EndpointKey Key)
        {
            var config = ESVConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.EndpointConfigurationKey.KeyName == Key.ToString());
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
                ESV.IMProcessor processor = new ESV.IMProcessor();
                processor._endpoint = this.EndPoint;
                processor._esvConfiguration = this._esvConfiguration;
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
            //        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.ESV_101, GetConfiguration(EndpointKey.IPAddress)));
            //        return true;
            //    }
            //    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.ESV_102, GetConfiguration(EndpointKey.IPAddress)));
            //    return false;
            //}
            //catch (Exception ex)
            //{
            //    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.ESV_103, GetConfiguration(EndpointKey.IPAddress)));
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
                bool IsUsersSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_ESV_User) != null;
                bool IsRolesSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_ESV_ROLE) != null;
                bool IsFunctionSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_ESV_FUNCTION) != null;
                bool IsPrivillegeAcctTransSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_ESV_PRIVILLEGEACCTTRANS) != null;


                if (!MarkDiscoveryFail && IsFunctionSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.ESV_107, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverFunctions();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.ESV_108, GetConfiguration(EndpointKey.IPAddress)));
                }
                if (!MarkDiscoveryFail && IsRolesSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.ESV_110, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverRoles();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.ESV_111, GetConfiguration(EndpointKey.IPAddress)));
                }
                if (!MarkDiscoveryFail && IsUsersSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.ESV_105, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverUser();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.ESV_106, GetConfiguration(EndpointKey.IPAddress)));
                }
                if (!MarkDiscoveryFail && IsPrivillegeAcctTransSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.ESV_113, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverPrivillegeAcctTrans();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.ESV_114, GetConfiguration(EndpointKey.IPAddress)));
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

                    ESVUserInfo.MapWithMasterUserForReconTable(_endpoint, ESVConfiguration.MapAll != null ? ESVConfiguration.MapAll : false);
                    ESVUserInfo.MapWithMasterUserForEntitlementReconTable(_endpoint, ESVConfiguration.MapAll != null ? ESVConfiguration.MapAll : false, discoveryProcessId);

                    base.DeleteLocalCopyFiles();
                }

                if (this.IsExceptionRaised || MarkDiscoveryFail)
                {
                    throw new Exception(string.Format("ESV discovery failed {0}", GetConfiguration(EndpointKey.IPAddress)));
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
            ESVUserRole userRole = null;
            IList<ESVUserRole> esvUserRole = new List<ESVUserRole>();
            try
            {
                long userId = 0;
                string mandatoryCols = CMessage.ESV_USERS_MANDATORY_COLS;
                string filePath = GetConfiguration(EndpointKey.UserFilePath);
                var dataTable = base.AuthenticateNetworkAndGetFileData(filePath, mandatoryCols, 0, true, false);

                if (dataTable != null)
                {
                    this.DoRecon = true;
                    //Get User List
                    var usersList = (from table in dataTable.AsEnumerable()
                                     group table by new
                                     {
                                         ADID = table.Field<string>("ADID"),
                                         //RoleName = table.Field<string>("RoleCode"),
                                         LastLogin = table.Field<string>("LastLoginDate")
                                     } into g
                                     select new ESVUserInfo
                                     {
                                         ADID = g.Key.ADID,
                                         dummyLastLoginDate = g.Key.LastLogin
                                     }).ToList();
                    ////Get Roles
                    //var roleList = (from table in dataTable.AsEnumerable()
                    //                group table by new
                    //                {
                    //                    ADID = table.Field<string>(0),
                    //                    RoleName = table.Field<string>(1),
                    //                    LastLogin = table.Field<string>(2)
                    //                } into g
                    //                select new ESVRoleInfo
                    //                {
                    //                    RoleCode = g.Key.RoleName,
                    //                    RoleDesc = g.Key.RoleName
                    //                }).ToList();

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
                    ESVUserInfo.SaveUsers(usersList, _endpoint.Id, MasterConfigHelper.ServiceUser);

                    ////Save Roles
                    //ESVRoleInfo.SaveRoles(roleList, _endpoint.Id, MasterConfigHelper.ServiceUser);

                    List<ESVUserInfo> existingUsers = ESVUserInfo.GetByEndpointSystemIdIncludeEndPointSystem(_endpoint.Id).ToList();
                    List<ESVRoleInfo> existingRoles = ESVRoleInfo.GetByEndpointSystemIdIncludeEndPointSystem(_endpoint.Id).ToList();
                    ESVRoleInfo roleInfo = null;
                    ESVUserInfo userInfo = null;
                    List<ESVUserRole> userRoleList = new List<ESVUserRole>();
                    ESVUserRole userRoleItem = null;
                    StringBuilder errorLog = new StringBuilder();

                    int counter = 0;
                    foreach (DataRow currentWorkingRow in dataTable.Rows)
                    {
                        try
                        {
                            var esvUserADID = Convert.ToString(GetColumnValueByIndex(currentWorkingRow, 0)).Trim();
                            var esvRoleCode = Convert.ToString(GetColumnValueByIndex(currentWorkingRow, 1)).Trim();
                            var lastLogin = GetColumnValueByIndex(currentWorkingRow, 2);
                            var status = Convert.ToString(GetColumnValueByIndex(currentWorkingRow, 3)).Trim();

                            if (string.IsNullOrEmpty(esvUserADID) || status.ToLower() != "1")
                            {
                                continue;
                            }

                            if (!string.IsNullOrEmpty(esvUserADID))
                            {
                                string[] adidWithoutDomain = esvUserADID.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                                if (adidWithoutDomain.Count() > 1)
                                {
                                    esvUserADID = adidWithoutDomain[1];
                                }
                                else
                                {
                                    esvUserADID = adidWithoutDomain[0];
                                }
                            }

                            roleInfo = existingRoles.Where(r => r.RoleCode == esvRoleCode).FirstOrDefault();
                            userInfo = existingUsers.Where(u => u.ADID == esvUserADID).FirstOrDefault();

                            if (roleInfo != null && userInfo != null && !userRoleList.Any(u => u.UserId == userInfo.Id && u.RoleId == roleInfo.Id))
                            {
                                userRoleItem = new ESVUserRole();
                                userRoleItem.RoleId = roleInfo.Id;
                                userRoleItem.UserId = userInfo.Id;
                                userRoleList.Add(userRoleItem);
                            }
                            else
                            {
                                if (roleInfo == null)
                                {
                                    errorLog.AppendLine(string.Format("User2Role discrepency. {0} Role not available to User {1} Mapping.", esvRoleCode, esvUserADID));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, CMessage.ESV_120 + ex.Message);
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
                                executeQuery = string.Format(CMessage.ESV_118, item.UserId);
                                ESVRoleInfo.ExecuteQuery(executeQuery);
                            }

                            queryString.AppendLine(string.Format(CMessage.ESV_119, item.UserId, item.RoleId));
                            if (counter == 500)
                            {
                                ESVRoleInfo.ExecuteQuery(queryString.ToString());
                                queryString = new StringBuilder();
                                counter = 0;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(queryString.ToString()))
                    {
                        ESVRoleInfo.ExecuteQuery(queryString.ToString());
                    }

                    //Write Error Log
                    if (!string.IsNullOrEmpty(errorLog.ToString()))
                    {
                        base.AuthenticateNetworkAndWriteDatatoFile(filePath, errorLog.ToString(), true);
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
                WriteDiscoveryLog(LogLevelType.Error, CMessage.ESV_120 + ex.Message, null, true);
            }
        }

        private void AccountRecon()
        {
            try
            {
                List<ESVUserInfo> userList = ESVUserInfo.GetDeletedUsers(this._endpoint.Id, this.Process, this.FileTimeStamp).ToList();
                if (userList != null && userList.Count > 0)
                {
                    ESVUserInfo.RemoveCMSUser(userList.ToList());
                }

                AccountReconcilation.ProcessDeletedAccounts(this._endpoint.Id, this.Process, MasterConfigHelper.ServiceUser, true, this.FileTimeStamp);

            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.ESV_ERROR_USERS, GetConfiguration(EndpointKey.IPAddress)), ex, true);

            }
        }

        private void GetFunctions()
        {
            try
            {
                string mandatoryCols = CMessage.ESV_FUN_MANDATORY_COLS;
                string filePath = GetConfiguration(EndpointKey.FunctionFilePath);
                var dataTable = base.AuthenticateNetworkAndGetFileData(filePath, mandatoryCols, 0);

                if (dataTable != null)
                {
                    ////Get Roles
                    //var roleList = (from table in dataTable.AsEnumerable()
                    //                group table by new
                    //                {
                    //                    RoleName = table.Field<string>(0)
                    //                } into g
                    //                select new ESVRoleInfo
                    //                {
                    //                    RoleCode = g.Key.RoleName,
                    //                    RoleDesc = g.Key.RoleName
                    //                }).ToList();

                    //Get Functions
                    var functionList = (from table in dataTable.AsEnumerable()
                                        group table by new
                                        {
                                            //RoleName = table.Field<string>("FNCode"),
                                            esvFNCode = table.Field<string>("FNCode"),
                                            esvFunctionDesc = table.Field<string>("FunctionDesc"),
                                            esvFNStatus = table.Field<string>("FNStatus"),
                                            esvView = table.Field<string>("View"),
                                            esvCreate = table.Field<string>("Create"),
                                            esvModify = table.Field<string>("Modify"),
                                            esvDelete = table.Field<string>("Delete"),
                                            esvPrint = table.Field<string>("Print"),
                                            esvExport = table.Field<string>("Export")
                                        } into g
                                        select new ESVFunctionInfo
                                        {
                                            //RoleCode = !string.IsNullOrEmpty(g.Key.RoleName) ? Convert.ToString(g.Key.RoleName).Trim() : null,
                                            FNCode = !string.IsNullOrEmpty(g.Key.esvFNCode) ? Convert.ToString(g.Key.esvFNCode).Trim() : null,
                                            FunctionDesc = !string.IsNullOrEmpty(g.Key.esvFunctionDesc) ? Convert.ToString(g.Key.esvFunctionDesc).Trim() : null,
                                            FNStatus = !string.IsNullOrEmpty(g.Key.esvFNStatus) ? Convert.ToString(g.Key.esvFNStatus).Trim() : null,
                                            View = string.Compare(Convert.ToString(g.Key.esvView), "Y", true) == 0 ? true : false,
                                            Create = string.Compare(Convert.ToString(g.Key.esvCreate), "Y", true) == 0 ? true : false,
                                            Modify = string.Compare(Convert.ToString(g.Key.esvModify), "Y", true) == 0 ? true : false,
                                            Delete = string.Compare(Convert.ToString(g.Key.esvDelete), "Y", true) == 0 ? true : false,
                                            Print = string.Compare(Convert.ToString(g.Key.esvPrint), "Y", true) == 0 ? true : false,
                                            Export = string.Compare(Convert.ToString(g.Key.esvExport), "Y", true) == 0 ? true : false,

                                        }).ToList();

                    //SaveFunctions
                    ESVFunctionInfo.SaveFunctions(functionList, _endpoint.Id, MasterConfigHelper.ServiceUser);

                    //Save Roles
                    // ESVRoleInfo.SaveRoles(roleList, _endpoint.Id, MasterConfigHelper.ServiceUser);

                    //List<ESVRoleInfo> existingRoles = ESVRoleInfo.GetByEndpointSystem(_endpoint.Id).ToList();
                    //List<ESVFunctionInfo> existingFuncs = ESVFunctionInfo.GetByEndpointSystemIdIncludeEndPointSystem(_endpoint.Id).ToList();

                    //ESVRoleInfo roleItem = null;
                    //ESVFunctionInfo funcItem = null;

                    //long batchCount = 0;
                    //StringBuilder queryBuilder = new StringBuilder();
                    //if (dataTable.Rows.Count > 0)
                    //{
                    //    string query = string.Format(CMessage.ESV_DELETE_FUNCROLE_QUERY, _endpoint.Id);
                    //    ESVFunctionInfo.ExecuteQuery(query);
                    //}

                    //List<ESVFunctionRole> funRoleList = new List<ESVFunctionRole>();
                    //ESVFunctionRole funcRoleItem = null;
                    //foreach (DataRow currentWorkingRow in dataTable.Rows)
                    //{
                    //    try
                    //    {

                    //        var esvRoleCode = Convert.ToString(GetColumnValueByIndex(currentWorkingRow, 0)).Trim();
                    //        var esvFNCode = Convert.ToString(GetColumnValueByIndex(currentWorkingRow, 1)).Trim();


                    //        if (string.IsNullOrEmpty(esvFNCode) || string.IsNullOrEmpty(esvRoleCode))
                    //        {
                    //            continue;
                    //        }

                    //        roleItem = existingRoles.Where(r => r.RoleCode == esvRoleCode).FirstOrDefault();
                    //        funcItem = existingFuncs.Where(r => r.FNCode == esvFNCode && r.RoleCode == esvRoleCode).FirstOrDefault();

                    //        if (roleItem != null && funcItem != null && !funRoleList.Any(a => a.FunctionId == funcItem.Id && a.RoleId == roleItem.Id))
                    //        {
                    //            funcRoleItem = new ESVFunctionRole();
                    //            funcRoleItem.RoleId = roleItem.Id;
                    //            funcRoleItem.FunctionId = funcItem.Id;

                    //            funRoleList.Add(funcRoleItem);
                    //        }


                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, CMessage.ESV_124 + ex.Message);
                    //    }
                    //}

                    //if (funRoleList != null && funRoleList.Count() > 0)
                    //{
                    //    funRoleList = funRoleList.Distinct().ToList();

                    //    foreach (var item in funRoleList)
                    //    {
                    //        batchCount++;
                    //        queryBuilder.AppendLine(string.Format(CMessage.ESV_INSERT_FUNCROLE_QUERY, item.RoleId, item.FunctionId, _endpoint.Id, Convert.ToInt16(VersionNo.Available)));

                    //        if (batchCount == 500)
                    //        {
                    //            ESVFunctionInfo.ExecuteQuery(queryBuilder.ToString());
                    //            queryBuilder = new StringBuilder();
                    //            batchCount = 0;
                    //        }
                    //    }
                    //}

                    //if (!string.IsNullOrEmpty(queryBuilder.ToString()))
                    //{
                    //    ESVFunctionInfo.ExecuteQuery(queryBuilder.ToString());
                    //}

                    //Delete files
                    base.AuthenticateNetworkAndDeleteAllFiles(filePath);

                    IList<ESVFunctionInfo> deleteFuncs = ESVFunctionInfo.GetDeletedByEndPointSystemId(this._endpoint.Id);
                    ESVFunctionInfo.DeleteList(deleteFuncs.ToList());
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, CMessage.ESV_124 + ex.Message, null, true);
            }
        }
        private void GetRoles()
        {
            try
            {
                StringBuilder errorLog = new StringBuilder();
                string mandatoryCols = CMessage.ESV_ROLES_MANDATORY_COLS;
                string filePath = GetConfiguration(EndpointKey.RoleFilePath);
                var dataTable = base.AuthenticateNetworkAndGetFileData(filePath, mandatoryCols, 0); //new TextDataConnect().ReadCSV(filePath, 3);

                if (dataTable != null)
                {
                    List<ESVRoleInfo> existingRoles = ESVRoleInfo.GetByEndpointSystem(_endpoint.Id);
                    ESVRoleInfo esvRoles = null;
                    List<ESVRoleInfo> newEntils = new List<ESVRoleInfo>();
                    EndPoint endpoint = EndPoint.GetById(this.EndPoint.Id, true);

                    var roleList = (from table in dataTable.AsEnumerable()
                                    group table by new
                                    {
                                        FNGROUP = table.Field<string>("FunctionalGroup"),
                                        RoleName = table.Field<string>("RoleCode"),
                                        RoleDESC = table.Field<string>("RoleDesc"),
                                        FNCode = table.Field<string>("FNCode")
                                    } into g
                                    select new ESVRoleInfo
                                    {
                                        FunctionalGroup = g.Key.FNGROUP,
                                        RoleCode = g.Key.RoleName,
                                        RoleDesc = g.Key.RoleDESC,
                                        FNCode = g.Key.FNCode
                                    }).ToList();

                    long batchCount = 0;
                    StringBuilder queryBuilder = new StringBuilder();
                    List<ESVFunctionInfo> existingFuncs = new List<ESVFunctionInfo>();
                    List<ESVFunctionRole> funRoleList = new List<ESVFunctionRole>();
                    ESVFunctionRole funcRoleItem = null;

                    if (roleList.Count > 0)
                    {
                        try
                        {
                            string query = string.Format(CMessage.ESV_DELETE_FUNCROLE_QUERY);
                            ESVFunctionInfo.ExecuteQuery(query);
                        }
                        catch (Exception ex)
                        {

                        }

                        existingFuncs = ESVFunctionInfo.GetByEndpointSystemIdIncludeEndPointSystem(_endpoint.Id).ToList();
                    }

                    ESVFunctionInfo funcItem = null;
                    foreach (ESVRoleInfo currentWorkingRow in roleList)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(currentWorkingRow.RoleCode))
                            {
                                continue;
                            }
                            esvRoles = existingRoles.Where(r => r.RoleCode == currentWorkingRow.RoleCode).FirstOrDefault();
                            if (esvRoles == null)
                            {
                                esvRoles = new ESVRoleInfo();
                                esvRoles.RoleCode = currentWorkingRow.RoleCode.Trim();
                                esvRoles.RoleDesc = currentWorkingRow.RoleDesc.Trim();
                                esvRoles.FunctionalGroup = currentWorkingRow.FunctionalGroup.Trim();
                                esvRoles.UserDefinedDisplayName = string.Format(CMessage.ESV_156, esvRoles.FunctionalGroup, esvRoles.RoleDesc);
                                esvRoles.CreatedBy = CMessage.ESV_122;
                                esvRoles.CreatedDate = DateTime.Now;
                                esvRoles.CanRequest = true;
                                existingRoles.Add(esvRoles);
                                newEntils.Add(esvRoles);
                            }
                            else
                            {
                                esvRoles.RoleDesc = currentWorkingRow.RoleDesc.Trim();
                                esvRoles.FunctionalGroup = currentWorkingRow.FunctionalGroup.Trim();
                                esvRoles.UserDefinedDisplayName = string.Format(CMessage.ESV_156, esvRoles.FunctionalGroup, esvRoles.RoleDesc);
                                esvRoles.LastModifiedBy = CMessage.ESV_122;
                                esvRoles.LastModifiedDate = DateTime.Now;
                            }

                            esvRoles.EndPointSystemId = _endpoint.Id;
                            esvRoles.Save(MasterConfigHelper.ServiceUser);

                            if (string.IsNullOrEmpty(currentWorkingRow.FNCode))
                            {
                                continue;
                            }

                            funcItem = null;
                            funcItem = existingFuncs.Where(r => r.FNCode == currentWorkingRow.FNCode).FirstOrDefault();

                            if (esvRoles != null && esvRoles.Id > 0 && funcItem != null && !funRoleList.Any(a => a.FunctionId == funcItem.Id && a.RoleId == esvRoles.Id))
                            {
                                funcRoleItem = new ESVFunctionRole();
                                funcRoleItem.RoleId = esvRoles.Id;
                                funcRoleItem.FunctionId = funcItem.Id;

                                funRoleList.Add(funcRoleItem);
                            }
                            else
                            {
                                if (funcItem == null)
                                {
                                    errorLog.AppendLine(string.Format("Role2Function discrepency. {0} Function not available to RoleCode {1} Mapping.", currentWorkingRow.FNCode, currentWorkingRow.RoleCode));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, CMessage.ESV_130 + ex.Message);
                        }
                    }

                    if (newEntils != null && newEntils.Count > 0)
                    {
                        StringBuilder remarks = new StringBuilder();
                        remarks.AppendLine(string.Format("New Role(s) has been discovered for the Endpoint System - {0}", (endpoint != null ? endpoint.EndPointSystemType.DisplayName : "EServices")));
                        foreach (var ent in newEntils)
                        {
                            remarks.AppendLine(string.Format("{0} - {1}", ent.RoleCode, ent.RoleDesc));
                        }
                        DILog.AlertNotificationLogger.WriteLog((long)endpoint.Id, null, remarks.ToString(), AlertCenterNotificationType.Discovery);
                    }

                    //Executing FunctionRole Bridge - Start
                    if (funRoleList != null && funRoleList.Count() > 0)
                    {
                        funRoleList = funRoleList.Distinct().ToList();

                        foreach (var item in funRoleList)
                        {
                            batchCount++;
                            queryBuilder.AppendLine(string.Format(CMessage.ESV_INSERT_FUNCROLE_QUERY, item.RoleId, item.FunctionId));

                            if (batchCount == 500)
                            {
                                ESVFunctionInfo.ExecuteQuery(queryBuilder.ToString());
                                queryBuilder = new StringBuilder();
                                batchCount = 0;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(queryBuilder.ToString()))
                    {
                        ESVFunctionInfo.ExecuteQuery(queryBuilder.ToString());
                    }
                    //Executing FunctionRole Bridge - End

                    //Write Error Log
                    if (!string.IsNullOrEmpty(errorLog.ToString()))
                    {
                        base.AuthenticateNetworkAndWriteDatatoFile(filePath, errorLog.ToString(), true);
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
                WriteDiscoveryLog(LogLevelType.Error, CMessage.ESV_130 + ex.Message, null, true);
            }
        }
        private void GetPrivillegeAcctTrans()
        {
            try
            {
                string mandatoryCols = CMessage.ESV_PRIV_MANDATORY_COLS;
                string readFileName = string.Empty;
                string filePath = GetConfiguration(EndpointKey.PrivillegeAcctTransFilePath);
                var dataTable = base.AuthenticateNetworkAndGetFileData(filePath, mandatoryCols, 0, false, true);

                if (dataTable != null)
                {
                    ESVPrivillegeAcctTransInfo privilTran = null;
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
                            var esvIDASTnxId = GetColumnValueByIndex(currentWorkingRow, 5);

                            privilTran = new ESVPrivillegeAcctTransInfo();

                            privilTran.CreatedBy = CMessage.ESV_122;
                            privilTran.CreatedDate = DateTime.Now;

                            privilTran.IDASTnxID = !string.IsNullOrEmpty(esvIDASTnxId) ? Convert.ToString(esvIDASTnxId).Trim() : null;
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
                            WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, CMessage.ESV_137 + ex.Message);
                        }
                    }

                    //Delete files
                    base.AuthenticateNetworkAndDeleteAllFiles(filePath);
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, CMessage.ESV_137 + ex.Message, null, true);
            }
        }
        private string GetColumnValue(DataRow currentRow, string columnName)
        {
            try
            {
                if (currentRow.Table.Columns.Contains(columnName))
                {
                    var value = currentRow[columnName].ToString().Trim();

                    if (value.ToUpper() == CMessage.ESV_138) return "";

                    return value;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, string.Format(CMessage.ESV_139, columnName), ex);
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

                    if (value.ToUpper() == CMessage.ESV_138) return "";

                    return value;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, string.Format(CMessage.ESV_139, index), ex);
                return string.Empty;
            }
        }

        #endregion
        private void UpdateEntitlementReconcilation()
        {
            try
            {
                var dicUsers = ESVUserInfo.GetAllByEndPointSystemId(this._endpoint.Id);

                foreach (KeyValuePair<long, string> item in dicUsers)
                {
                    var esvUserInfo = ESVUserInfo.GetByIdIncludeESVRoles(item.Key);
                    if (esvUserInfo.ESVRoles != null)
                    {
                        foreach (ESVRoleInfo entitlement in esvUserInfo.ESVRoles)
                        {
                            var acctRecon = AccountReconcilation.GetByUser(esvUserInfo.EndPointSystemId, esvUserInfo.Id);

                            if (acctRecon == null) continue;

                            var entRecon = EntitlementReconcilation.GetByUser(esvUserInfo.Id, esvUserInfo.EndPointSystemId, entitlement.RoleCode.Trim(), EntitlementType.Role);
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
                                var entitlements = (esvUserInfo.Entitlements.Select(x => x.EntitlementName.ToString()));
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
                IList<ESVRoleInfo> deleteRoles = ESVRoleInfo.GetDeletedByEndPointSystemId(this._endpoint.Id, this.FileTimeStamp);
                // ESVRoleInfo.DeleteList(deleteRoles.ToList());

                if (deleteRoles != null && deleteRoles.Count > 0)
                {
                    ESVRoleInfo.RemoveCMSRole(deleteRoles.ToList());
                }

                UpdateEntitlementReconcilation();
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.ESV_ERROR_USERS, GetConfiguration(EndpointKey.IPAddress)), ex, true);

            }


        }
    }
}
