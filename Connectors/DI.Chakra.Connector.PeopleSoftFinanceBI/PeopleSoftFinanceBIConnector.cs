#region NameSpace
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Configuration;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using CheckNulls = DI.Chakra.DomainModel.CheckNull;
using CMessage = DI.Chakra.Connector.PeopleSoftFinanceBI.Message;
using CQuery = DI.Chakra.Connector.PeopleSoftFinanceBI.Query;
using DI.Chakra.Logger;
using DI.Chakra.DBConfigHelper;
#endregion

namespace DI.Chakra.Connector
{
    public class PeopleSoftFinanceBIConnector : ConnectorBase
    {
        #region Members
        public EndpointConfiguration _peopleSoftFinanceBIConfiguration { get; set; }
        public EndPoint _endpoint;
        private List<PeopleSoftFinanceBIUserInfo> discoveredUsers = null;
        private Dictionary<long, string> _dicRoles = new Dictionary<long, string>();
        private List<long> selectedUserList = new List<long>();
        #endregion
        #region Core
        public PeopleSoftFinanceBIConnector(EndPoint endpoint, AccessRequest currentRequest = null)
        {
            _endpoint = endpoint;
            this._peopleSoftFinanceBIConfiguration = EndpointConfiguration.GetByEndpointSystemId(_endpoint.Id);
            this.EndpointConfiguration = _peopleSoftFinanceBIConfiguration;
            this.CurrentRequest = currentRequest;
            this.discoveredUsers = new List<PeopleSoftFinanceBIUserInfo>();

            this.NetworkUserName = Configuration(EndpointKey.BizServiceUserName);
            this.NetworkPassword = Configuration(EndpointKey.BizServicePassword);
        }
        public override IMProcessor IMProcessor
        {
            get
            {
                PeopleSoftFinanceBI.IMProcessor processor = new PeopleSoftFinanceBI.IMProcessor();
                processor.EndPoint = this.EndPoint;
                processor._peopleSoftFinanceBIConfiguration = this._peopleSoftFinanceBIConfiguration;
                return processor;
            }
        }
        #endregion
        #region Discovery
        public override bool Connect()
        {
            return true;
        }
        private new string Configuration(EndpointKey Key)
        {
            var config = _peopleSoftFinanceBIConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.EndpointConfigurationKey.KeyName == Key.ToString());
            return config != null ? config.Value : "";
        }

        public override void Disconnect()
        {
        }
        public override bool ChangePassword(string UserName, string Password, long DIUserId, bool isReactivation)
        {
            bool ReturnValue = false;
            return ReturnValue;
        }
        public override void GetData()
        {
            try
            {
                if (!this._endpoint.IsTrustedSource)
                {
                    long DiscoveryProcessId = base.DiscoveryProcess.Id;

                    this.MarkDiscoveryFail = false;

                    List<EndpointObjects> Objects = DI.Chakra.DomainModel.DiscoveryProcess.GetByDiscoveryProcessId(DiscoveryProcessId);
                    bool IsRolesSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EPOBJ_PSFBI_RoleDesc) != null;
                    bool IsCostcenterSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EPOBJ_PSFBI_COSTCENTER) != null;
                    bool IsUsers2RoleSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EPOBJ_PSFBI_User2Role) != null;
                    bool IsUsers2CCSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EPOBJ_PSFBI_User2CC) != null;

                    if (!this.MarkDiscoveryFail && IsCostcenterSelected)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.BI_202, Configuration(EndpointKey.HostName)));
                        DiscoverCostCenterGroups();
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.BI_203, Configuration(EndpointKey.HostName)));
                    }

                    if (!this.MarkDiscoveryFail && IsRolesSelected)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.BI_103, Configuration(EndpointKey.HostName)));
                        DiscoverRoles();
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.BI_104, Configuration(EndpointKey.HostName)));
                    }

                    if (!this.MarkDiscoveryFail && IsUsers2RoleSelected)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.BI_208, Configuration(EndpointKey.HostName)));
                        GetRoleUser();
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.BI_209, Configuration(EndpointKey.HostName)));
                    }
                    if (!this.MarkDiscoveryFail && IsUsers2CCSelected)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.BI_208, Configuration(EndpointKey.HostName)));
                        GetUser2CC();
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.BI_209, Configuration(EndpointKey.HostName)));
                    }


                    if (!this.MarkDiscoveryFail)
                    {
                        if (this.DoRecon)
                        {
                            WriteDiscoveryLog(LogLevelType.Info, string.Format("FBI Account Recon Started {0}", Configuration(EndpointKey.HostName)));
                            UserAccountReconcilation();
                            WriteDiscoveryLog(LogLevelType.Info, string.Format("FBI Account Recon Ended {0}", Configuration(EndpointKey.HostName)));

                            WriteDiscoveryLog(LogLevelType.Info, string.Format("FBI Entitlement Recon Started {0}", Configuration(EndpointKey.HostName)));
                            this.UpdateEntitlementReconcilation();
                            WriteDiscoveryLog(LogLevelType.Info, string.Format("FBI Entitlement Recon Ended {0}", Configuration(EndpointKey.HostName)));

                        }

                        base.DeleteLocalCopyFiles();
                    }

                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.BI_117, Configuration(EndpointKey.HostName)));

                    PeopleSoftFinanceBIUserInfo.MapWithMasterUserForReconTable(_endpoint, _peopleSoftFinanceBIConfiguration.MapAll != null ? _peopleSoftFinanceBIConfiguration.MapAll : false);
                    PeopleSoftFinanceBIUserInfo.MapWithMasterUserForEntitlementReconTable(_endpoint, _peopleSoftFinanceBIConfiguration.MapAll != null ? _peopleSoftFinanceBIConfiguration.MapAll : false, DiscoveryProcessId);

                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.BI_118, Configuration(EndpointKey.HostName)));

                    if (this.IsExceptionRaised || this.MarkDiscoveryFail)
                    {
                        throw new Exception(string.Format("FBI discovery failed {0}", Configuration(EndpointKey.IPAddress)));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void DiscoverRoles()
        {
            try
            {
                this.GetRoles();
                IList<PeopleSoftFinanceBIRoleInfo> discoveredRoles = PeopleSoftFinanceBIRoleInfo.GetDeletedByEndPointSystemId(this._endpoint.Id);
                PeopleSoftFinanceBIRoleInfo.DeleteRoles(discoveredRoles.ToList(), this._endpoint.Id);
                _dicRoles = PeopleSoftFinanceBIRoleInfo.GetRoleByEndpointId(this._endpoint.Id);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.BI_207, Configuration(EndpointKey.IPAddress)), ex, true);
            }
        }
        private string GetColumnValue(DataRow currentRow, string columnName)
        {
            try
            {
                if (currentRow.Table.Columns.Contains(columnName))
                {
                    var value = currentRow[columnName].ToString().Trim();

                    if (value.ToUpper() == CMessage.BI_201) return "";

                    return value;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, string.Format(CMessage.BI_201, columnName), ex);
                return string.Empty;
            }
        }
        private void DiscoverCostCenterGroups()
        {
            try
            {
                this.GetCostCenterGroups();
                IList<PeopleSoftFinanceBICostCenterGroup> discoveredCC = PeopleSoftFinanceBICostCenterGroup.GetDeletedByEndPointSystemId(this._endpoint.Id);
                PeopleSoftFinanceBICostCenterGroup.DeleteCotCenters(discoveredCC.ToList(), this._endpoint.Id);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.BI_204, Configuration(EndpointKey.HostName)), ex, true);
            }
        }
        private void GetCostCenterGroups()
        {
            try
            {
                string mandatoryCols = CMessage.BI_CC_MANDATORY;
                string filePath = Configuration(EndpointKey.CostCenterGroupFilePath);
                var dataTable = base.AuthenticateNetworkAndGetFileData(filePath, mandatoryCols);

                if (dataTable != null)
                {
                    foreach (DataRow currentWorkingRow in dataTable.Rows)
                    {
                        try
                        {
                            var costCenterCode = Convert.ToString(GetColumnValue(currentWorkingRow, CMessage.BI_198)).Trim();
                            if (string.IsNullOrEmpty(costCenterCode))
                            {
                                continue;
                            }
                            PeopleSoftFinanceBICostCenterGroup costcenter = PeopleSoftFinanceBICostCenterGroup.GetByCodeEndPointsystemId(costCenterCode.Trim(), _endpoint.Id);
                            if (costcenter == null)
                            {
                                costcenter = new PeopleSoftFinanceBICostCenterGroup();
                                costcenter.CreatedBy = CMessage.BI_200;
                                costcenter.CreatedDate = DateTime.Now;
                            }
                            else
                            {
                                costcenter.LastModifiedBy = CMessage.BI_200;
                                costcenter.LastModifiedDate = DateTime.Now;
                            }
                            costcenter.CanRequest = true;
                            costcenter.CostCenterCode = costCenterCode.Trim();
                            costcenter.CosCenterTree = Convert.ToString(GetColumnValue(currentWorkingRow, CMessage.BI_197)).Trim();
                            costcenter.CostCenterDescription = Convert.ToString(GetColumnValue(currentWorkingRow, CMessage.BI_199)).Trim();
                            costcenter.UserDefinedDisplayName = costcenter.CosCenterTree + " - " + costcenter.CostCenterCode;
                            costcenter.EndPointSystemId = _endpoint.Id;
                            costcenter.Save(MasterConfigHelper.ServiceUser);
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, CMessage.BI_204 + ex.Message);
                        }
                    }

                    this.AuthenticateNetworkAndDeleteAllFiles(filePath);
                }

            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, CMessage.BI_204, ex, true);
            }
        }
        private void GetRoles()
        {
            try
            {
                string mandatory = CMessage.BI_ROLES_MANDATORY;
                string readFileName = string.Empty;
                string filePath = Configuration(EndpointKey.RoleFilePath);
                var dataTable = base.AuthenticateNetworkAndGetFileData(filePath, mandatory);

                if (dataTable != null)
                {
                    foreach (DataRow currentWorkingRow in dataTable.Rows)
                    {
                        var roleId = Convert.ToString(GetColumnValue(currentWorkingRow, CMessage.BI_195));
                        var roleDescription = Convert.ToString(GetColumnValue(currentWorkingRow, CMessage.BI_196));

                        if (string.IsNullOrEmpty(roleId.Trim()))
                        {
                            continue;
                        }

                        PeopleSoftFinanceBIRoleInfo financeBIRole = PeopleSoftFinanceBIRoleInfo.GetByRoleIdEndpointSystemId(roleId.Trim(), _endpoint.Id);
                        if (financeBIRole == null)
                        {
                            financeBIRole = new PeopleSoftFinanceBIRoleInfo();
                            financeBIRole.CreatedBy = CMessage.BI_200;
                            financeBIRole.CreatedDate = DateTime.Now;
                            financeBIRole.LastModifiedBy = CMessage.BI_200;
                            financeBIRole.LastModifiedDate = DateTime.Now;
                        }
                        else
                        {
                            financeBIRole.LastModifiedBy = CMessage.BI_200;
                            financeBIRole.LastModifiedDate = DateTime.Now;
                        }
                        financeBIRole.RoleId = roleId.Trim();
                        financeBIRole.CanRequest = true;
                        financeBIRole.RoleDescription = !string.IsNullOrEmpty(roleDescription.Trim()) ? roleDescription.Trim() : roleId.Trim();
                        financeBIRole.EndPointSystemId = _endpoint.Id;
                        financeBIRole.Save(MasterConfigHelper.ServiceUser);
                    }

                    this.AuthenticateNetworkAndDeleteAllFiles(filePath);
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, CMessage.BI_207, ex, true);
            }
        }
        private void GetRoleUser()
        {
            try
            {
                string mandatory = CMessage.BI_U2R_MANDATORY;
                string filePath = Configuration(EndpointKey.UserRoleFilePath);
                var dataTable = base.AuthenticateNetworkAndGetFileData(filePath, mandatory, 0, true, false);

                if (dataTable != null)
                {
                    foreach (DataRow currentWorkingRow in dataTable.Rows)
                    {
                        try
                        {
                            var peopleSoftFinanceBIADId = Convert.ToString(GetColumnValue(currentWorkingRow, CMessage.BI_194));
                            var peopleSoftFinanceBIRoleId = Convert.ToString(GetColumnValue(currentWorkingRow, CMessage.BI_195));
                            if (string.IsNullOrEmpty(peopleSoftFinanceBIADId) && string.IsNullOrEmpty(peopleSoftFinanceBIRoleId))
                            {
                                continue;
                            }
                            var peoplesoftFinanceBIUser = PeopleSoftFinanceBIUserInfo.GetByADIdEndPointsystemId(peopleSoftFinanceBIADId.Trim(), _endpoint.Id);
                            var peoplesoftFinanceBIRole = PeopleSoftFinanceBIRoleInfo.GetByRoleIdEndpointSystemId(peopleSoftFinanceBIRoleId.Trim(), _endpoint.Id);

                            if (peoplesoftFinanceBIUser == null)
                            {
                                peoplesoftFinanceBIUser = new PeopleSoftFinanceBIUserInfo();
                                peoplesoftFinanceBIUser.ADID = peopleSoftFinanceBIADId;
                                peoplesoftFinanceBIUser.EndPointSystemId = _endpoint.Id;
                                peoplesoftFinanceBIUser.CreatedBy = "System";
                                peoplesoftFinanceBIUser.CreatedDate = DateTime.Now;
                            }
                            else
                            {
                                peoplesoftFinanceBIUser.LastModifiedBy = "System";
                                peoplesoftFinanceBIUser.LastModifiedDate = DateTime.Now;
                            }

                            peoplesoftFinanceBIUser.Save("System");
                            discoveredUsers.Add(peoplesoftFinanceBIUser);

                            if (peoplesoftFinanceBIRole == null)
                            {
                                peoplesoftFinanceBIRole = new PeopleSoftFinanceBIRoleInfo();
                                peoplesoftFinanceBIRole.CreatedBy = CMessage.BI_200;
                                peoplesoftFinanceBIRole.CreatedDate = DateTime.Now;
                                peoplesoftFinanceBIRole.LastModifiedBy = CMessage.BI_200;
                                peoplesoftFinanceBIRole.LastModifiedDate = DateTime.Now;

                                peoplesoftFinanceBIRole.RoleId = peopleSoftFinanceBIRoleId.Trim();
                                peoplesoftFinanceBIRole.RoleDescription = peopleSoftFinanceBIRoleId.Trim();
                                peoplesoftFinanceBIRole.EndPointSystemId = _endpoint.Id;
                                peoplesoftFinanceBIRole.Save(MasterConfigHelper.ServiceUser);
                            }


                            if (peoplesoftFinanceBIUser != null && peoplesoftFinanceBIRole != null)
                            {
                                string executeQuery = string.Empty;

                                if (!selectedUserList.Any(u => u == peoplesoftFinanceBIUser.Id))
                                {
                                    selectedUserList.Add(peoplesoftFinanceBIUser.Id);
                                    executeQuery = string.Format(CMessage.BI_205, peoplesoftFinanceBIUser.Id, _endpoint.Id);
                                    PeopleSoftFinanceBIUserInfo.ExecuteQuery(executeQuery);
                                    executeQuery = null;
                                }

                                executeQuery = string.Format(CMessage.BI_206, peoplesoftFinanceBIUser.Id, peoplesoftFinanceBIRole.Id, this._endpoint.Id, Convert.ToInt16(VersionNo.Available));
                                BizCMSRoleInfo.ExecuteQuery(executeQuery);
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, CMessage.BI_204 + ex.Message);
                        }
                    }

                    this.AuthenticateNetworkAndDeleteAllFiles(filePath);
                }
                else
                {
                    this.DoRecon = false;
                }
            }
            catch (Exception ex)
            {
                this.DoRecon = false;
                WriteDiscoveryLog(LogLevelType.Error, CMessage.BI_204, ex, true);
            }
        }

        private void GetUser2CC()
        {
            try
            {
                string mandatory = CMessage.BI_U2CC_MANDATORY;
                string filePath = Configuration(EndpointKey.User2CostCenterFilePath);
                var dataTable = base.AuthenticateNetworkAndGetFileData(filePath, mandatory);

                if (dataTable != null)
                {
                    string executeQuery = string.Empty;
                    executeQuery = string.Format(CMessage.BI_DELETE_USER2CC);
                    PeopleSoftFinanceBIUserInfo.ExecuteQuery(executeQuery);
                    executeQuery = null;

                    foreach (DataRow currentWorkingRow in dataTable.Rows)
                    {
                        try
                        {
                            var ADID = Convert.ToString(GetColumnValue(currentWorkingRow, CMessage.BI_194));
                            var CCCODE = Convert.ToString(GetColumnValue(currentWorkingRow, CMessage.BI_198));
                            var CCTREE = Convert.ToString(GetColumnValue(currentWorkingRow, CMessage.BI_197));
                            var SEQUNO = Convert.ToString(GetColumnValue(currentWorkingRow, CMessage.BI_SQU));

                            if (string.IsNullOrEmpty(ADID) && string.IsNullOrEmpty(CCCODE))
                            {
                                continue;
                            }
                            var peoplesoftFinanceBIUser = PeopleSoftFinanceBIUserInfo.GetByADIdEndPointsystemId(ADID.Trim(), _endpoint.Id);
                            var peoplesoftFinanceBICC = PeopleSoftFinanceBICostCenterGroup.GetByCCCodeAndTree(CCCODE.Trim(), CCTREE.Trim());

                            if (peoplesoftFinanceBIUser == null)
                            {
                                peoplesoftFinanceBIUser = new PeopleSoftFinanceBIUserInfo();
                                peoplesoftFinanceBIUser.ADID = ADID;
                                peoplesoftFinanceBIUser.EndPointSystemId = _endpoint.Id;
                                peoplesoftFinanceBIUser.CreatedBy = MasterConfigHelper.ServiceUser;
                                peoplesoftFinanceBIUser.CreatedDate = DateTime.Now;
                            }
                            else
                            {
                                peoplesoftFinanceBIUser.LastModifiedBy = MasterConfigHelper.ServiceUser;
                                peoplesoftFinanceBIUser.LastModifiedDate = DateTime.Now;
                            }

                            peoplesoftFinanceBIUser.Save(MasterConfigHelper.ServiceUser);
                            discoveredUsers.Add(peoplesoftFinanceBIUser);

                            if (peoplesoftFinanceBICC == null)
                            {
                                peoplesoftFinanceBICC = new PeopleSoftFinanceBICostCenterGroup();
                                peoplesoftFinanceBICC.CreatedBy = CMessage.BI_200;
                                peoplesoftFinanceBICC.CreatedDate = DateTime.Now;

                                peoplesoftFinanceBICC.CostCenterCode = CCCODE.Trim();
                                peoplesoftFinanceBICC.CosCenterTree = CCTREE.Trim();
                                peoplesoftFinanceBICC.CostCenterDescription = CCCODE.Trim();
                                peoplesoftFinanceBICC.EndPointSystemId = _endpoint.Id;
                                peoplesoftFinanceBICC.Save(MasterConfigHelper.ServiceUser);
                            }


                            if (peoplesoftFinanceBIUser != null && peoplesoftFinanceBICC != null)
                            {
                                executeQuery = string.Format(CMessage.BI_INSERT_USER2CC, peoplesoftFinanceBIUser.Id, peoplesoftFinanceBICC.Id, SEQUNO);
                                PeopleSoftFinanceBIUserInfo.ExecuteQuery(executeQuery);
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, CMessage.BI_204 + ex.Message);
                        }
                    }

                    this.AuthenticateNetworkAndDeleteAllFiles(filePath);
                }
                else
                {
                    this.DoRecon = false;
                }
            }
            catch (Exception ex)
            {
                this.DoRecon = false;
                WriteDiscoveryLog(LogLevelType.Error, CMessage.BI_204, ex, true);
            }
        }
        private void UpdateEntitlementReconcilation()
        {
            try
            {
                IList<PeopleSoftFinanceBIUserInfo> dicUsers = PeopleSoftFinanceBIUserInfo.GetByEndpointId(this._endpoint.Id);
                foreach (PeopleSoftFinanceBIUserInfo psUserInfo in dicUsers)
                {
                    var acctRecon = AccountReconcilation.GetByUser(this._endpoint.Id, psUserInfo.Id, true);
                    if (acctRecon == null)
                        continue;

                    if (psUserInfo.peopleSoftFinanceBIRole != null && psUserInfo.peopleSoftFinanceBIRole.Count() > 0)
                    {
                        foreach (PeopleSoftFinanceBIRoleInfo entitlement in psUserInfo.peopleSoftFinanceBIRole)
                        {

                            var entRecon = EntitlementReconcilation.GetByUser(psUserInfo.Id, psUserInfo.EndPointSystemId, entitlement.RoleId.Trim(), EntitlementType.Role);
                            if (entRecon != null && entRecon.Entitlement == entitlement.RoleId.Trim())
                            {
                                entRecon.LastModifiedDate = DateTime.Now;
                                entRecon.LastModifiedBy = "System";
                                entRecon.AccountName = psUserInfo.ADID;
                                entRecon.AccountReconcilationId = acctRecon.Id;
                                entRecon.Entitlement = entitlement.RoleId;
                                entRecon.EntitlementDescription = entitlement.RoleDescription;
                                entRecon.EntitlementId = entitlement.Id;
                                if (entRecon.IsDeleted)
                                {
                                    EntitlementReconcilation.UpdateException(entRecon, true);
                                }
                            }
                            else
                            {
                                var entitlements = (psUserInfo.Entitlements.Select(x => x.EntitlementName.ToString()));
                                entRecon = this.CreateEntitlementReconcilationShell(acctRecon, EntitlementType.Role, string.Join(",", entitlements != null ? entitlements.ToArray() : new string[] { }));
                                entRecon.Entitlement = entitlement.RoleId.Trim();
                                entRecon.EntitlementId = entitlement.Id;
                                entRecon.EntitlementDescription = entitlement.RoleDescription;
                                entRecon.AccountName = psUserInfo.ADID;
                                entRecon.AccountReconcilationId = acctRecon.Id;
                                entRecon.Entitlement = entitlement.RoleId;
                                entRecon.EntitlementDescription = entitlement.RoleDescription;

                                EntitlementReconcilation.UpdateException(entRecon, true);
                            }
                            entRecon.Save(MasterConfigHelper.ServiceUser);
                        }
                    }

                    if (psUserInfo.peopleSoftFinanceBICostCenter != null && psUserInfo.peopleSoftFinanceBICostCenter.Count() > 0)
                    {
                        foreach (PeopleSoftFinanceBICostCenterGroup entitlement in psUserInfo.peopleSoftFinanceBICostCenter)
                        {
                            var entRecon = EntitlementReconcilation.GetByUser(psUserInfo.Id, psUserInfo.EndPointSystemId, entitlement.CostCenterCode.Trim(), EntitlementType.CostCenter);
                            if (entRecon != null && entRecon.Entitlement == entitlement.CostCenterCode.Trim())
                            {
                                entRecon.LastModifiedDate = DateTime.Now;
                                entRecon.LastModifiedBy = "System";
                                entRecon.AccountName = psUserInfo.ADID;
                                entRecon.AccountReconcilationId = acctRecon.Id;
                                entRecon.Entitlement = entitlement.CostCenterCode;
                                entRecon.EntitlementDescription = entitlement.CostCenterDescription;
                                entRecon.EntitlementId = entitlement.Id;
                                if (entRecon.IsDeleted)
                                {
                                    EntitlementReconcilation.UpdateException(entRecon, true);
                                }
                            }
                            else
                            {
                                var entitlements = (psUserInfo.Entitlements.Select(x => x.EntitlementName.ToString()));
                                entRecon = this.CreateEntitlementReconcilationShell(acctRecon, EntitlementType.CostCenter, string.Join(",", entitlements != null ? entitlements.ToArray() : new string[] { }));
                                entRecon.Entitlement = entitlement.CostCenterCode.Trim();
                                entRecon.EntitlementId = entitlement.Id;
                                entRecon.EntitlementDescription = entitlement.CostCenterDescription;
                                entRecon.EntitlementId = entitlement.Id;
                                EntitlementReconcilation.UpdateException(entRecon, true);
                            }
                            entRecon.Save(MasterConfigHelper.ServiceUser);
                        }
                    }
                }
                EntitlementReconcilation.ProcessDeletedEntitlements(this._endpoint.Id, this.Process, MasterConfigHelper.ServiceUser);
            }
            catch (Exception ex)
            {
                this.WriteDiscoveryLog(LogLevelType.Exception, CMessage.BI_119, ex);
            }
        }
        private void UserAccountReconcilation()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("FBI User Account Reconcilation Started", Configuration(EndpointKey.HostName)));
            if (discoveredUsers != null && discoveredUsers.Count > 0)
            {
                discoveredUsers = discoveredUsers.Distinct().ToList();
                PeopleSoftFinanceBIUserInfo.SaveUsers(discoveredUsers, this._endpoint.Id, MasterConfigHelper.ServiceUser);
                discoveredUsers = null;

                AccountReconcilation.ProcessDeletedAccounts(this._endpoint.Id, this.Process, MasterConfigHelper.ServiceUser);
                discoveredUsers = PeopleSoftFinanceBIUserInfo.GetDeletedUsers(this._endpoint.Id, this.Process);
                if (discoveredUsers != null && discoveredUsers.Count > 0)
                {
                    PeopleSoftFinanceBIUserInfo.RemoveFBIUser(discoveredUsers.ToList(), _endpoint.Id);
                }

            }
            WriteDiscoveryLog(LogLevelType.Info, string.Format("FBI User Account Reconcilation Ended.", Configuration(EndpointKey.HostName)));
        }
    }
}

#endregion

