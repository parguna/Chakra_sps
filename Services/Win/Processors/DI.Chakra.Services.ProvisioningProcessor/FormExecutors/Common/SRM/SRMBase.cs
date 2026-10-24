using DI.Chakra.Connector;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.ProvisioningUtility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.Common
{
    public class SRMBase : FormExecutorBase
    {
        AccountType accounttype = AccountType.User;
        public SRMBase(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(idmsLogger)
        {
            Endpoint = endpoint;
            CurrentRequest = currentRequest;
            this.Accounttype = this.CurrentRequest.AccountType;
        }

        protected AccountType Accounttype
        {
            get
            {
                return accounttype;
            }
            set
            {
                accounttype = value;
            }
        }
        public override bool Execute(ProvisioningParamCollection fields, DomainModel.User user)
        {

            AdHocSAPSRMRequest sapDetail = null;
            SRMRoleData srmDetail = null;
            AdHocAccessRequest adhocRequest = null;
            string roleListStr = string.Empty;
            bool isExecutedSuccessfully = false;
            if (user == null)
            {
                WriteProvisioningLog(LogLevelType.Warning, "SRM Provisioning failed. DIUser object is null");
                return false;
            }

            //if adhoc request, get prov details from adhoc request object
            string leveldetail = string.Empty;
            if (CurrentRequest.AdHocAccessRequestId.HasValue)
            {
                adhocRequest = CurrentRequest.AdHocAccessRequest;
                sapDetail = (AdHocSAPSRMRequest)CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();
                roleListStr = this.GetRoleStringFromRequest(sapDetail, adhocRequest.AdhocActionType);
                WriteProvisioningLog(LogLevelType.DebugStep, $"roleListStr:{roleListStr}");
                WriteProvisioningLog(LogLevelType.DebugStep, $"sapDetail != null:{sapDetail != null}");
                if (sapDetail != null)
                {
                    WriteProvisioningLog(LogLevelType.DebugStep, $"Accounttype:{Accounttype}");
                    if (this.Accounttype == AccountType.User || this.Accounttype == AccountType.Secondary)
                    {
                        srmDetail = this.PopulateSRMRoleData(sapDetail, adhocRequest.AdhocActionType);
                        if (adhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                        {
                            //Added below code for SRM to use Flins account Name.
                            var adEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.ADSingaporePowerLocal);
                            if (this.Accounttype == AccountType.Secondary)
                            {
                                var adUserValue = ADSUserInfo.GetPrimaryByDIUserIdAccountType(user.Id, sapDetail.AccountName, adEndpoint.Id, this.Accounttype);
                                if (adUserValue != null)
                                {
                                    sapDetail.AccountName = srmDetail.AccountName = adUserValue.AccountName;
                                    WriteProvisioningLog(LogLevelType.DebugStep, $"AD Account:{adUserValue.AccountName} AND SRM AccountName:{srmDetail.AccountName}");
                                }
                                else
                                {
                                    WriteProvisioningLog(LogLevelType.Info, $"AD Secondary Account:{sapDetail.AccountName} not found or locked");
                                }
                            }
                            else
                            {
                                var adUserValue = ADSUserInfo.GetBysAMAccountNameEndPointSystemId(sapDetail.AccountName, adEndpoint.Id);
                                if (adUserValue != null)
                                {
                                    sapDetail.AccountName = srmDetail.AccountName = adUserValue.AccountName;
                                    WriteProvisioningLog(LogLevelType.DebugStep, $"AD Account:{adUserValue.AccountName} AND SRM AccountName:{srmDetail.AccountName}");
                                }
                                else
                                {
                                    WriteProvisioningLog(LogLevelType.Info, $"AD Account:{sapDetail.AccountName} not found or locked");
                                }
                            }
                        }
                    }
                    else
                    {
                        srmDetail = this.PopulateSpecialAccountsSRMRoleData(sapDetail, adhocRequest.AdhocActionType);
                    }
                }
            }
            WriteProvisioningLog(LogLevelType.DebugStep, $"Request AccountName:{sapDetail.AccountName} srm AccountName:{srmDetail.AccountName}");
            if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount || CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
            {
                isExecutedSuccessfully = CreateAccount(fields, user, sapDetail, srmDetail, roleListStr);
            }
            else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount)
            {
                isExecutedSuccessfully = DeleteAccount(user, srmDetail, sapDetail);
            }
            else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                isExecutedSuccessfully = AmendentRemove(user, sapDetail, srmDetail, roleListStr);
            }

            return isExecutedSuccessfully;
        }
        //public override bool OnBoard(ProvisioningParamCollection fields, DomainModel.User user)
        //{
        //    return true;
        //}
        public override bool OffBoard(DomainModel.User user, ProvisioningParamCollection fields)
        {
            if (this.Accounttype == AccountType.Service || this.Accounttype == AccountType.Shared || this.Accounttype == AccountType.Secondary)
                return true;
            try
            {
                string accountName = string.Empty;

                var srpEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.SAP_SRP);
                if (srpEndpoint.Id == this.Endpoint.Id) return true;

                var susEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.SAP_SUS);
                if (susEndpoint.Id == this.Endpoint.Id) return true;

                var accts = AccountReconcilation.GetAllActiveAccountsByUserIdAndEndpointSystemId(user.Id, srpEndpoint.Id, AccountType.User);
                foreach (var acctRecon in accts)
                {
                    WriteProvisioningLog(LogLevelType.Info, $"Endpoint:{srpEndpoint.DisplayName} LockUser for User:{acctRecon.AccountName}");
                    var sapUsr = SAPUserinfo.GetByUserLogonName(acctRecon.AccountName, srpEndpoint.Id);
                    if (sapUsr != null)
                    {
                        TakeOUSnapshotForSRPSUP(user,sapUsr, EmployeeEvents.Resignation);
                        WriteProvisioningLog(LogLevelType.Info, $"Endpoint:{srpEndpoint.DisplayName} LockUser for User:{sapUsr.UserLogonName}");
                        this.LockUser(sapUsr, LockScenario.OffBoard, srpEndpoint);
                        WriteProvisioningLog(LogLevelType.Info, $"SRP User InActive for User:{accountName}");
                        sapUsr.IsActive = false;
                        sapUsr.Save(MasterConfigHelper.ServiceUser);

                        WriteProvisioningLog(LogLevelType.Info, $"RemoveEntitlementsByUser for User:{accountName}");
                        EntitlementReconcilation.RemoveEntitlementsByUser(srpEndpoint.Id, sapUsr.Id, "System");
                    }
                }

               
                accts = AccountReconcilation.GetAllActiveAccountsByUserIdAndEndpointSystemId(user.Id, susEndpoint.Id, AccountType.User);
                foreach (var acctRecon in accts)
                {
                    WriteProvisioningLog(LogLevelType.Info, $"Endpoint:{susEndpoint.DisplayName} LockUser for User:{acctRecon.AccountName} ");
                    var sapUsr = SAPUserinfo.GetByUserLogonName(acctRecon.AccountName, susEndpoint.Id);
                    if (sapUsr != null)
                    {
                        TakeOUSnapshotForSRPSUP(user, sapUsr, EmployeeEvents.Resignation);
                        WriteProvisioningLog(LogLevelType.Info, $"LockUser for User:{acctRecon.AccountName}");
                        this.LockUser(sapUsr, LockScenario.OffBoard, susEndpoint);
                        WriteProvisioningLog(LogLevelType.Info, $"SUP User InActive for User:{accountName}");
                        sapUsr.IsActive = false;
                        sapUsr.Save(MasterConfigHelper.ServiceUser);

                        WriteProvisioningLog(LogLevelType.Info, $"RemoveEntitlementsByUser for User:{accountName}");
                        EntitlementReconcilation.RemoveEntitlementsByUser(susEndpoint.Id, sapUsr.Id, "System");
                    }
                }
                var srmEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.SRM);
                var srmaccts = AccountReconcilation.GetAllActiveAccountsByUserIdAndEndpointSystemId(user.Id, srmEndpoint.Id, AccountType.User).Select(i=>i.AccountName).Distinct();
                foreach (var AccountName in srmaccts)
                {
                    var name = AccountName;
                    WriteProvisioningLog(LogLevelType.Info, $"Endpoint:{srmEndpoint.DisplayName} LockUser for User:{name} ");
                    var sapUsr = SAPUserinfo.GetByUserLogonName(name, this.Endpoint.Id);
                    if (sapUsr != null)
                    {
                        accountName = sapUsr.UserLogonName;
                        WriteProvisioningLog(LogLevelType.Info, $"LockUser for User:{accountName}");
                        var mappings = SAPSRMBusinessRoleMapping.GetMappingByUserName(accountName);
                        List<SRMUserRoleMappingData> mappingInfos = new List<SRMUserRoleMappingData>();

                        foreach (SAPSRMBusinessRoleMapping mapping in mappings)
                        {
                            SRMUserRoleMappingData mappingInfo = new SRMUserRoleMappingData();

                            mappingInfo.BusinessRole = mapping.BusinessRole;
                            mappingInfo.PGrp = mapping.PGrp;
                            mappingInfo.POrg = mapping.POrg;
                            mappingInfo.UserId = mapping.UserId;
                            mappingInfo.UserName = mapping.UserName;

                            mappingInfos.Add(mappingInfo);
                        }
                        WriteProvisioningLog(LogLevelType.Info, $"TakeOUSnapshot for User:{accountName}");
                        this.TakeOUSnapshot(user, sapUsr, EmployeeEvents.Resignation, mappingInfos, ApplicationType.SRM);
                        WriteProvisioningLog(LogLevelType.Info, $"LockUser for User:{accountName}");
                        this.LockUser(sapUsr, LockScenario.OffBoard, srmEndpoint);
                        WriteProvisioningLog(LogLevelType.Info, $"SRM User InActive for User:{accountName}");
                        sapUsr.IsActive = false;
                        sapUsr.Save(MasterConfigHelper.ServiceUser);

                        WriteProvisioningLog(LogLevelType.Info, $"RemoveEntitlementsByUser for User:{accountName}");
                        EntitlementReconcilation.RemoveEntitlementsByUser(srmEndpoint.Id, sapUsr.Id, "System");
                    }
                }

                ProvisioningEmailHelper.NotifySRMSupportTeam(this.CurrentRequest, string.Empty, accountName, "Off-board");
                SAPSRMBusinessRoleMapping.DeleteMappingByUserId(user.Id, this.Accounttype);
                return true;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"Error while executing SRM offboarding request. {ex.Message}. Stack Trace:{ex.StackTrace}", ex);
                return false;
            }
        }
        public override bool Propagate(DomainModel.User user, ProvisioningParamCollection fields)
        {
            if (CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared)
                return true;
            try
            {
                bool isUpdated = true;

                var accountList = AccountReconcilation.GetActiveAccountsByAccountType(user.Id, this.accounttype).ToList();

                var srpEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.SAP_SRP);
                ConnectorBase connector = ConnectorFactory.GetConnector(srpEndpoint, this.CurrentRequest);
                var accountNames = accountList.Where(i => i.EndpointSystemId == srpEndpoint.Id).Select(i => i.AccountName).Distinct().ToList();
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, srpEndpoint.Id, LogLevelType.Info,$"AccountNames to update:{string.Join("", accountNames)}", null);
                foreach (string name in accountNames)
                {
                    var sapUsr = SAPUserinfo.GetByUserLogonName(name, srpEndpoint.Id);

                    if (sapUsr != null)
                    {
                        sapUsr.Lastname = user.KnownAs;
                        sapUsr.E_Mail = user.EmailId;
                        sapUsr.Tel1_Numbr = user.TelephoneNumber;
                        sapUsr.Department = user.OrgDepartment;
                        sapUsr.Function0 = user.IdentificationNo;

                        isUpdated = connector.UpdateAccount(sapUsr);

                        if (isUpdated)
                        {
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, srpEndpoint.Id, LogLevelType.Info, $"User:{sapUsr.UserLogonName}(EmployeeId:{user.EmployeeId})  modified  in SAP - Lastname:{sapUsr.Lastname} E_Mail:{sapUsr.E_Mail} Department:{sapUsr.Department} Function0:{sapUsr.Function0}", null);
                            sapUsr.Save("System");
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, srpEndpoint.Id, LogLevelType.Info, string.Format("User {0} modified  in SAP.", sapUsr.UserLogonName), null);
                        }
                    }
                }

                var susEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.SAP_SUS);
                connector = ConnectorFactory.GetConnector(susEndpoint, this.CurrentRequest);
                accountNames = accountList.Where(i => i.EndpointSystemId == susEndpoint.Id).Select(i => i.AccountName).Distinct().ToList();
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, susEndpoint.Id, LogLevelType.Info, $"AccountNames to update:{string.Join("", accountNames)}", null);
                foreach (string name in accountNames)
                {
                    var sapUsr = SAPUserinfo.GetByUserLogonName(name, susEndpoint.Id);

                    if (sapUsr != null)
                    {
                        sapUsr.Lastname = user.KnownAs;
                        sapUsr.E_Mail = user.EmailId;
                        sapUsr.Tel1_Numbr = user.TelephoneNumber;
                        sapUsr.Department = user.OrgDepartment;
                        sapUsr.Function0 = user.IdentificationNo;

                        isUpdated = connector.UpdateAccount(sapUsr);

                        if (isUpdated)
                        {
                            sapUsr.Save("System");
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, susEndpoint.Id, LogLevelType.Info, string.Format("User {0} modified  in SAP.", sapUsr.UserLogonName), null);
                        }
                    }
                }

                return isUpdated;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing SRM Propagate request. {0}", ex.Message), ex);
                return false;
            }
        }
        public override bool ReOrg(DomainModel.User user, ProvisioningParamCollection fields)
        {
            if (this.Accounttype == AccountType.Service || this.Accounttype == AccountType.Shared)
                return true;
            try
            {
                //SRP
                var srpEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.SAP_SRP);
                ConnectorBase connector = ConnectorFactory.GetConnector(srpEndpoint, this.CurrentRequest);

                var accountNames = SAPSRMBusinessRoleMapping.GetAccountNamesByUserIdAndAccountType(user.Id, this.Accounttype);

                foreach (string name in accountNames)
                {
                    var sapUsr = SAPUserinfo.GetByUserLogonName(name, srpEndpoint.Id);

                    if (sapUsr != null)
                    {
                        sapUsr.Function0 = user.IdentificationNo;
                        sapUsr.Department = user.OrgDepartment;

                        connector.ChangeContainer(sapUsr);

                        sapUsr.Save("System");
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, srpEndpoint.Id, LogLevelType.Info, string.Format("User {0} modified  in SAP.", sapUsr.UserLogonName), null);
                    }
                }

                //SUS
                var susEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.SAP_SUS);
                ConnectorBase susconnector = ConnectorFactory.GetConnector(susEndpoint, this.CurrentRequest);

                foreach (string name in accountNames)
                {
                    var sapUsr = SAPUserinfo.GetByUserLogonName(name, susEndpoint.Id);

                    if (sapUsr != null)
                    {
                        sapUsr.Function0 = user.IdentificationNo;
                        sapUsr.Department = user.OrgDepartment;

                        susconnector.ChangeContainer(sapUsr);

                        sapUsr.Save("System");
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, susEndpoint.Id, LogLevelType.Info, string.Format("User {0} modified  in SAP.", sapUsr.UserLogonName), null);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing SRM ReOrg request. {0}", ex.Message), ex);
                return false;
            }
        }
        public override bool Transfer(DomainModel.User user, ProvisioningParamCollection fields)
        {
            if (this.Accounttype == AccountType.Service || this.Accounttype == AccountType.Shared)
                return true;
            try
            {

                string accountName = string.Empty;

                var accountNames = AccountReconcilation.GetAllActiveAccountsByUserIdAndEndpointSystemId(user.Id,this.Endpoint.Id, this.Accounttype).Select(i=>i.AccountName).Distinct().ToList();
                var srpEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.SAP_SRP);
                if (srpEndpoint.Id == this.Endpoint.Id) return true;

                var susEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.SAP_SUS);
                if (susEndpoint.Id == this.Endpoint.Id) return true;

                var accts = AccountReconcilation.GetAllActiveAccountsByUserIdAndEndpointSystemId(user.Id, srpEndpoint.Id, AccountType.User);

                foreach (var item in accts)
                {
                    if (!string.IsNullOrWhiteSpace(item.AccountName) && !accountNames.Contains(item.AccountName))
                        accountNames.Add(item.AccountName);
                }

                var srmaccts = AccountReconcilation.GetAllActiveAccountsByUserIdAndEndpointSystemId(user.Id, this.Endpoint.Id, AccountType.User);
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, this.Endpoint.Id, LogLevelType.Info, $"SRM Snapshot started Account count:{srmaccts?.Count}");
                foreach (var item in srmaccts)
                {
                    if (!string.IsNullOrWhiteSpace(item.AccountName) && accountNames.Contains(item.AccountName,StringComparer.InvariantCultureIgnoreCase))
                    {
                        //accountNames.Add(item.AccountName);

                        var sapUsr = SAPUserinfo.GetByUserLogonName(item.AccountName, this.Endpoint.Id);
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, this.Endpoint.Id, LogLevelType.Info, $"SRM SAP User UserLogonName:{item.AccountName} ");
                        if (sapUsr != null)
                        {
                            var mappings = SAPSRMBusinessRoleMapping.GetMappingByUserName(sapUsr.UserLogonName);
                            List<SRMUserRoleMappingData> mappingInfos = new List<SRMUserRoleMappingData>();
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, this.Endpoint.Id, LogLevelType.DebugStep, $"SRM SAP User UserLogonName:{sapUsr.UserLogonName} Role Count:{mappings.Count}");
                            foreach (SAPSRMBusinessRoleMapping mapping in mappings)
                            {
                                SRMUserRoleMappingData mappingInfo = new SRMUserRoleMappingData();

                                mappingInfo.BusinessRole = mapping.BusinessRole;
                                mappingInfo.PGrp = mapping.PGrp;
                                mappingInfo.POrg = mapping.POrg;
                                mappingInfo.UserId = mapping.UserId;
                                mappingInfo.UserName = mapping.UserName;
                                mappingInfos.Add(mappingInfo);
                            }
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, this.Endpoint.Id, LogLevelType.Info, $"Create Snapshot for User:{sapUsr.UserLogonName}");
                            this.TakeOUSnapshot(user, sapUsr, EmployeeEvents.OUChange, mappingInfos, ApplicationType.SRM);
                        }
                    }
                    else
                    {
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, this.Endpoint.Id, LogLevelType.Info, $"finding accountName {item} for SRM not matching");
                    }
                }
                #region SRP
                ConnectorBase srmconnector = ConnectorFactory.GetConnector(srpEndpoint, this.CurrentRequest);
                foreach (string name in accountNames)
                {
                    var sapUsr = SAPUserinfo.GetByUserLogonName(name, srpEndpoint.Id);

                    if (sapUsr != null)
                    {
                        accountName = sapUsr.UserLogonName;

                        sapUsr.Function0 = user.IdentificationNo;
                        sapUsr.Department = user.OrgDepartment;

                        var existingRoles = SAPUserinfo.GetByIdIncludeSAPRoleInfo(sapUsr.Id).SAPRoleInfo.Select(item => item.ROLE_CODE).ToList();

                        if (!string.IsNullOrWhiteSpace(fields["Roles"].Value))
                        {
                            foreach (string role in existingRoles)
                            {
                                if (!fields["Roles"].Value.Contains(role))
                                {
                                    sapUsr.Roles.Add(role);
                                }
                            }
                        }
                        else
                        {
                            sapUsr.Roles = existingRoles;
                        }
                        TakeOUSnapshotForSRPSUP(user, sapUsr, EmployeeEvents.OUChange);
                        bool isSucces = srmconnector.RemoveAllEntitlements(sapUsr);
                        string roleListStr = string.Join(",", sapUsr.Roles.Select(item => item));

                        if (isSucces)
                        {
                            SAPUserinfo.RemoveEntitlement(sapUsr.Id, EntitlementType.Role, roleListStr);
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, srpEndpoint.Id, LogLevelType.Info, string.Format("Roles - {0} removed successfully for the user {1}.", roleListStr, sapUsr.UserLogonName), null);
                        }
                        else
                        {
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, srpEndpoint.Id, LogLevelType.Error, string.Format("Error while removing roles - {0} for the user {1}.", roleListStr, sapUsr.UserLogonName), null);
                        }

                        srmconnector.ChangeContainer(sapUsr);

                        sapUsr.Roles = new List<string>();

                        sapUsr.Save("System");

                        this.LockUser(sapUsr, LockScenario.OUChange, srpEndpoint);

                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, srpEndpoint.Id, LogLevelType.Info, string.Format("User {0} modified  in SAP.", sapUsr.UserLogonName), null);
                    }
                }
                #endregion
                #region SUS
                
                var susconnector = ConnectorFactory.GetConnector(susEndpoint, this.CurrentRequest);
                foreach (string name in accountNames)
                {
                    var sapUsr = SAPUserinfo.GetByUserLogonName(name, susEndpoint.Id);
                    if (sapUsr != null)
                    {
                        sapUsr.Function0 = user.IdentificationNo;
                        sapUsr.Department = user.OrgDepartment;

                        var existingRoles = SAPUserinfo.GetByIdIncludeSAPRoleInfo(sapUsr.Id).SAPRoleInfo.Select(item => item.ROLE_CODE).ToList();


                        if (!string.IsNullOrWhiteSpace(fields["Roles"].Value))
                        {
                            foreach (string role in existingRoles)
                            {
                                if (!fields["Roles"].Value.Contains(role))
                                {
                                    sapUsr.Roles.Add(role);
                                }
                            }
                        }
                        else
                        {
                            sapUsr.Roles = existingRoles;
                        }
                        TakeOUSnapshotForSRPSUP(user, sapUsr, EmployeeEvents.OUChange);
                        bool isSucces = susconnector.RemoveAllEntitlements(sapUsr);
                        string roleListStr = string.Join(",", sapUsr.Roles.Select(item => item));

                        if (isSucces)
                        {
                            SAPUserinfo.RemoveEntitlement(sapUsr.Id, EntitlementType.Role, roleListStr);
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, susEndpoint.Id, LogLevelType.Info, string.Format("Roles - {0} removed successfully for the user {1}.", roleListStr, sapUsr.UserLogonName), null);
                        }
                        else
                        {
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, susEndpoint.Id, LogLevelType.Error, string.Format("Error while removing roles - {0} for the user {1}.", roleListStr, sapUsr.UserLogonName), null);
                        }

                        susconnector.ChangeContainer(sapUsr);

                        sapUsr.Roles = new List<string>();

                        sapUsr.Save("System");

                        this.LockUser(sapUsr, LockScenario.OUChange, susEndpoint);

                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, susEndpoint.Id, LogLevelType.Info, string.Format("User {0} modified  in SAP.", sapUsr.UserLogonName), null);
                    }
                }
                #endregion

                //foreach (var item in srmaccts)
                //{
                //    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, this.Endpoint.Id, LogLevelType.DebugStep, $"SRM User:{item.AccountName}");
                //    if (!string.IsNullOrWhiteSpace(item.AccountName) && !accountNames.Contains(item.AccountName))
                //    {
                //        var sapUsr = SAPUserinfo.GetByUserLogonName(item.AccountName, this.Endpoint.Id);

                //        if (sapUsr != null)
                //        {
                //            var mappings = SAPSRMBusinessRoleMapping.GetMappingByUserName(sapUsr.UserLogonName);
                //            List<SRMUserRoleMappingData> mappingInfos = new List<SRMUserRoleMappingData>();
                //            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, this.Endpoint.Id, LogLevelType.DebugStep, $"SRM SAP User UserLogonName:{sapUsr.UserLogonName} Role Count:{mappings.Count}");
                //            foreach (SAPSRMBusinessRoleMapping mapping in mappings)
                //            {
                //                SRMUserRoleMappingData mappingInfo = new SRMUserRoleMappingData();

                //                mappingInfo.BusinessRole = mapping.BusinessRole;
                //                mappingInfo.PGrp = mapping.PGrp;
                //                mappingInfo.POrg = mapping.POrg;
                //                mappingInfo.UserId = mapping.UserId;
                //                mappingInfo.UserName = mapping.UserName;

                //                mappingInfos.Add(mappingInfo);
                //            }
                //            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, this.Endpoint.Id, LogLevelType.Info, $"Create Snapshot for User:{sapUsr.UserLogonName}");
                //            this.TakeOUSnapshot(user, sapUsr, EmployeeEvents.OUChange, mappingInfos, ApplicationType.SRM);
                //        }
                //    }
                //}
                ProvisioningEmailHelper.NotifySRMSupportTeam(this.CurrentRequest, string.Empty, accountName, "Transfer");
                SAPSRMBusinessRoleMapping.DeleteMappingByUserId(user.Id, this.Accounttype);

                foreach (var name in accountNames)
                {
                    var srmsapUsr = SAPUserinfo.GetByUserLogonName(name, this.Endpoint.Id);
                    if (srmsapUsr != null)
                    {
                        EntitlementReconcilation.RemoveEntitlementsByUser(this.Endpoint.Id, srmsapUsr.Id, "System");

                        var accorecon = AccountReconcilation.GetByAccountName(name, this.Endpoint.Id);
                        if (accorecon != null)
                        {
                            accorecon.IsDeleted = true;
                            accorecon.IsLocked = true;
                            accorecon.DeletedDate = DateTime.Now;
                            accorecon.Save("System");
                        }
                        srmsapUsr.IsActive = false;
                        srmsapUsr.Save("System");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing transfer request. {0}", ex.Message), ex);
                return false;
            }
        }
        public override bool Retain(DomainModel.User user, ProvisioningParamCollection fields)
        {
            throw new NotImplementedException();
        }
        public override bool RemediateAttestation(DomainModel.User user, ProvisioningParamCollection fields)
        {
            bool isExecutedSuccessfully = false;

            try
            {
                AdHocAccessRequest adhocRequest = null;
                AdHocSAPSRMRequest sapDetail = null;
                SRMRoleData srmDetail = null;

                string roleListStr = string.Empty;

                //if adhoc request, get prov details from adhoc request object
                if (this.CurrentRequest.AdHocAccessRequest != null)
                {

                    adhocRequest = AdHocAccessRequest.GetById(this.CurrentRequest.AdHocAccessRequestId.Value, false);
                    sapDetail = (AdHocSAPSRMRequest)adhocRequest.GetEntitlementDetail();

                    roleListStr = this.GetRoleStringFromRequest(sapDetail, adhocRequest.AdhocActionType);

                    if (sapDetail != null)
                    {
                        if (this.Accounttype == AccountType.User || this.Accounttype == AccountType.Secondary)
                        {
                            srmDetail = this.PopulateSRMRoleData(sapDetail, adhocRequest.AdhocActionType);
                        }
                        else
                        {
                            srmDetail = this.PopulateSpecialAccountsSRMRoleData(sapDetail, adhocRequest.AdhocActionType);
                        }
                    }
                }

                if (this.CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    isExecutedSuccessfully = this.LockSRMUser(srmDetail, LockScenario.AttestationRemediation, this.CurrentRequest.UserId);
                }
                else if (this.CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    isExecutedSuccessfully = this.RemoveSRMRoles(sapDetail, srmDetail, this.CurrentRequest.UserId, roleListStr);
                }

                if (isExecutedSuccessfully)
                {
                    this.UpdateSRMUserAndRoleMapping(user, sapDetail, adhocRequest.AdhocActionType);
                    ProvisioningEmailHelper.SendEmailAfterAttestationRemediation(user, this.CurrentRequest, roleListStr, srmDetail.AccountName);
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing access request - {0}.", CurrentRequest.Id), ex);
                return false;
            }

            return isExecutedSuccessfully;
        }

        public override bool UndoEvent(DomainModel.User user, ProvisioningParamCollection fields)
        {
            if (this.Accounttype == AccountType.Service || this.Accounttype == AccountType.Shared || this.Accounttype == AccountType.Secondary)
                return true;
            bool isUpdated = true;
            try
            {
                if (this.Endpoint.ApplicationType == ApplicationType.SRM)
                {
                    if (this.CurrentRequest.EventUndoRequestId.HasValue == false)
                    {
                        WriteProvisioningLog(LogLevelType.Info, $"SRMBase UndoEvent: RequestId{this.CurrentRequest.Id} EventUndoRequestId not found");
                        return false;
                    }

                    if (!string.IsNullOrWhiteSpace(this.CurrentRequest.EventUndoRequest.TechnicalRoles))
                    {
                        WriteProvisioningLog(LogLevelType.Info, $"SRMBase RequestId{this.CurrentRequest.Id} restoring roles:{this.CurrentRequest.EventUndoRequest.TechnicalRoles}");
                        var srmSNapShot = JsonConvert.DeserializeObject<SRMSnapShot>(this.CurrentRequest.EventUndoRequest.TechnicalRoles);
                        List<SRMUserRoleMappingData> mappingData = new List<SRMUserRoleMappingData>();
                        if (srmSNapShot == null)
                            mappingData = new List<SRMUserRoleMappingData>();
                        else
                        {
                            mappingData = srmSNapShot.SRMUserRoles ?? new List<SRMUserRoleMappingData>();
                        }
                        AdHocSAPSRMRequest sapDetail = new AdHocSAPSRMRequest();
                        sapDetail.AccountName = user.AccountName;
                        sapDetail.AdHocSAPSRMRequestDetails = new List<AdHocSAPSRMRequestDetail>();
                        foreach (var data in mappingData)
                        {
                            sapDetail.AdHocSAPSRMRequestDetails.Add(new AdHocSAPSRMRequestDetail()
                            {
                                EntitlementType = EntitlementType.Role,
                                PurchaseGroup = data.PGrp,
                                PurchaseOrg = data.POrg,
                                Entitlement = data.BusinessRole
                            });
                        }
                        WriteProvisioningLog(LogLevelType.Info, $"SRMBase RequestId{this.CurrentRequest.Id} SRMUserRoleMappingData count:{mappingData.Count}");

                        var AdhocActionType = AdHocActionType.NewAccount;
                        var srmDetail = this.PopulateSRMRoleData(sapDetail, AdhocActionType);
                        var endpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.ADSingaporePowerLocal);
                        var adUserValue = ADSUserInfo.GetActivePrimaryByDIUserId(user.Id, endpoint.Id);
                        srmDetail.AccountName = adUserValue.AccountName;
                        WriteProvisioningLog(LogLevelType.Info, $"SRMBase RequestId{this.CurrentRequest.Id} AccountName:{srmDetail.AccountName}");
                        var roleListStr = this.GetRoleStringFromRequest(sapDetail, AdhocActionType);
                        WriteProvisioningLog(LogLevelType.Info, $"SRMBase RequestId{this.CurrentRequest.Id} RoleList:{roleListStr}");
                        isUpdated = this.CreateAccount(fields, user, sapDetail, srmDetail, roleListStr, false, true);
                        if (isUpdated)
                        {
                            var acc = AccountReconcilation.GetByAccountName(srmDetail.AccountName, this.Endpoint.Id);
                            acc.ValidToDate = GetValidTo(acc.UserId);
                            acc.Save("System");
                        }
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Info, $"SRMBase UndoEvent RequestId:{this.CurrentRequest.Id} TechnicalRoles empty. Cannot Undo this request.");
                        return false;
                    }
                }
                
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, this.Endpoint.Id, LogLevelType.Exception, ex.Message, ex);
            }
            return isUpdated;
        }
        bool CreateAccount(ProvisioningParamCollection fields, DomainModel.User user, AdHocSAPSRMRequest srmReqDetail, SRMRoleData srmDetail, string srmRolesString, bool sendemail = true, bool fromUndo = false)
        {
            bool isExecutedSuccessfully = false;
            SAPUserinfo sapUserValue = null;
            SAPUserinfo srmsapUsr = new SAPUserinfo();
            try
            {
                var srpEPSSO = EndPoint.GetEndPointByApplicationType(ApplicationType.SAP_SRP);
                var isSRPSSOEnabled = EndpointConfiguration.IsSSOEnableForEndpoint(srpEPSSO.Id) && this.Accounttype == AccountType.User;

                //Add AD Groups
                if (!string.IsNullOrWhiteSpace(srmDetail.ADGroups))
                {
                    var endpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.ADSingaporePowerLocal);
                    var adUserValue = ADSUserInfo.GetBysAMAccountNameEndPointSystemId(srmDetail.AccountName, endpoint.Id);

                    if (adUserValue != null)
                    {
                        if (!adUserValue.sAMAccountName.Equals(srmDetail.AccountName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Error, string.Format("AD account name \"{0}\" and HRMS FLINS account name \"{1}\" are not same. Please correct the AD account name manually and re-submit the request.", adUserValue.sAMAccountName, srmDetail.AccountName), null);
                            return false;
                        }

                        bool isSuccess = this.AssignADGroups(adUserValue, endpoint, srmDetail.ADGroups);
                        if (!isSuccess)
                        {
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, string.Format("Error while adding SRM Roles \"{1}\" for User \"{0}\".", srmDetail.AccountName, srmRolesString), null);
                            return false;
                        }
                    }
                    else
                    {
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, $"AD Account does not exists for the user:{srmDetail.AccountName}");
                    }
                }
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.DebugStep, $"FLINSBIPGrpRoles[{srmDetail.FLINSBIPGrpRoles}]; FLINSBIRoles:[{srmDetail.FLINSBIRoles}]; FLINSBIPOrgRoles:[{srmDetail.FLINSBIPOrgRoles}]");

                //Add FLINSBI Roles
                if (!string.IsNullOrWhiteSpace(srmDetail.FLINSBIRoles) || !string.IsNullOrWhiteSpace(srmDetail.FLINSBIPOrgRoles) ||
                        !string.IsNullOrWhiteSpace(srmDetail.FLINSBIPGrpRoles))
                {
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, "FLINSBIRoles");
                    var endpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.FLINS_BI);
                    if (endpoint != null && endpoint.IsActive)
                    {
                        sapUserValue = SAPUserinfo.GetActiveByUserLogonName(srmDetail.AccountName, endpoint.Id);

                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, $"sapUserValue:{sapUserValue != null} FLINS_BI:{endpoint == null}");
                        if (sapUserValue != null)
                        {
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, $"Active FLINS BI account found for SRM User {srmDetail.AccountName}", null);

                            string biRoles = string.Format("{0},{1},{2}", srmDetail.FLINSBIRoles, srmDetail.FLINSBIPGrpRoles, srmDetail.FLINSBIPOrgRoles);

                            bool isSuccess = this.AssignSAPRoles(sapUserValue, endpoint, biRoles);

                            if (!isSuccess)
                            {
                                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, string.Format("Error while adding SRM Roles \"{1}\" for User \"{0}\".", srmDetail.AccountName, srmRolesString), null);
                                return false;
                            }
                        }
                        else
                        {
                            bool isSuccess = false;
                            if (this.Accounttype == AccountType.User || this.Accounttype == AccountType.Secondary)
                            {
                                isSuccess = this.CreateFLINSBIUser(user, srmDetail);
                            }
                            else
                            {
                                isSuccess = this.CreateFLINSBIUser(user, srmDetail, srmReqDetail);
                            }
                            if (!isSuccess)
                            {
                                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, "FLINSBIRoles CreateFLINSBIUser failed");
                                return false;
                            }
                            else
                            {
                                sapUserValue = SAPUserinfo.GetByUserLogonName(srmDetail.AccountName, endpoint.Id);
                                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.DebugStep, $"sapUserValue:{sapUserValue == null}");
                                string biRoles = string.Format("{0},{1},{2}", srmDetail.FLINSBIRoles, srmDetail.FLINSBIPGrpRoles, srmDetail.FLINSBIPOrgRoles);
                                isSuccess = this.AssignSAPRoles(sapUserValue, endpoint, biRoles);
                                if (!isSuccess)
                                {
                                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, string.Format("Error while adding SRM Roles \"{1}\" for User \"{0}\".", srmDetail.AccountName, srmRolesString), null);
                                    return false;
                                }
                            }
                        }
                    }
                }



                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.DebugStep, $"SRPEnablerRoles {srmDetail.SRPEnablerRoles}");

                if (!string.IsNullOrWhiteSpace(srmDetail.SRPRoles) || !string.IsNullOrWhiteSpace(srmDetail.SRPEnablerRoles))
                {
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, "SRPRoles started");
                    var endpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.SAP_SRP);
                    sapUserValue = SAPUserinfo.GetActiveByUserLogonName(srmDetail.AccountName, endpoint.Id);
                    if (!string.IsNullOrWhiteSpace(srmDetail.SUSRoles))
                    {
                        sendemail = false;
                    }

                    if (sapUserValue != null)
                    {
                        #region Exisiting Account
                        var isSUSExists = IsSUSRoleAlreadyExists(srmDetail.AccountName);
                        string biRoles = string.Format("{0},{1}", srmDetail.SRPRoles, srmDetail.SRPEnablerRoles);
                        bool isSuccess = this.AssignSAPRoles(sapUserValue, endpoint, biRoles);
                        if (!isSuccess)
                        {
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, string.Format("Error while adding SRM Roles \"{1}\" for User \"{0}\".", srmDetail.AccountName, srmRolesString), null);
                            return false;
                        }
                        else
                        {
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, $"SRPRoles AssignSAPRoles:{isSuccess}");
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, $"SRPRoles Existing SUS Role found:{isSUSExists}");
                            if(isSUSExists == false)
                            {
                                sapUserValue.Password = this.constructUserPassword(fields, endpoint.Id);
                                var sapConnector = ConnectorFactory.GetConnector(endpoint, this.CurrentRequest, _idmsLogger);
                                if (!string.IsNullOrWhiteSpace(srmDetail.SUSRoles))
                                {
                                    sapConnector.ResetPassword(sapUserValue.UserLogonName, sapUserValue.Password, string.Empty);
                                    ProvisioningEmailHelper.SendEmailForAccountProvisioning(this.CurrentRequest, sapUserValue, user, ApplicationType.SAP_SRP,isForSAPSSO: isSRPSSOEnabled);
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        bool isSuccess = false;
                        if (this.Accounttype == AccountType.User || this.Accounttype == AccountType.Secondary)
                        {
                            isSuccess = this.CreateSRPUser(user, srmDetail);
                        }
                        else
                        {
                            isSuccess = this.CreateSRPUser(user, srmDetail, srmReqDetail);
                        }
                        if (!isSuccess)
                        {
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, string.Format("Error while creating SRP Account for User \"{0}\".", srmDetail.AccountName), null);
                            return false;
                        }
                        else
                        {
                            sapUserValue = SAPUserinfo.GetByUserLogonName(srmDetail.AccountName, endpoint.Id);
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.DebugStep, $"SRPRoles : sapUserValue:{sapUserValue == null}");
                            string biRoles = string.Format("{0},{1}", srmDetail.SRPRoles, srmDetail.SRPEnablerRoles);
                            isSuccess = this.AssignSAPRoles(sapUserValue, endpoint, biRoles);
                            if (!isSuccess)
                            {
                                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, string.Format("Error while adding SRM Roles \"{1}\" for User \"{0}\".", srmDetail.AccountName, srmRolesString), null);
                                return false;
                            }
                        }

                        //wait for the sap job to create user in portal
                        //Thread.Sleep(5000);
                    }
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, "SRPRoles Completed");
                }



                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, $"SUSRoles is null:{srmDetail?.SUSRoles == null}");

                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, $"SUSRoles is empty:{ string.IsNullOrWhiteSpace(srmDetail?.SUSRoles)}");

                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.DebugStep, $"SUSRoles {srmDetail.SUSRoles}");

                if (!string.IsNullOrWhiteSpace(srmDetail.SUSRoles))
                {
                    sendemail = false;
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, $"SUSRoles is/are:{ srmDetail?.SUSRoles}");

                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, "SUSRoles started");
                    var endpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.SAP_SUS);
                    if (endpoint != null && endpoint.IsActive)
                    {
                        sapUserValue = SAPUserinfo.GetActiveByUserLogonName(srmDetail.AccountName, endpoint.Id);

                        if (sapUserValue != null)
                        {


                            if (CurrentRequest.ActivationDate != null && CurrentRequest.ActivationDate != DateTime.MinValue)
                            {
                                sapUserValue.UserValidFrom = this.getSAPDateFormat((DateTime)CurrentRequest.ActivationDate);
                            }
                            else
                            {
                                sapUserValue.UserValidFrom = this.getSAPDateFormat(DateTime.Now);
                            }

                            sapUserValue.UserValidTo = this.getSAPDateFormat(GetValidTo(CurrentRequest.UserId).Value);


                            if (!string.IsNullOrWhiteSpace(sapUserValue.UserValidFrom) && !string.IsNullOrWhiteSpace(sapUserValue.UserValidTo))
                            {
                                var connector = ConnectorFactory.GetConnector(endpoint, this.CurrentRequest, _idmsLogger);
                                connector.UpdateAccount(sapUserValue);
                            }

                            UnLockUserIfLocked(sapUserValue, endpoint);
                            string biRoles = srmDetail.SUSRoles;
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, $"Before Assign SUSRoles");

                            bool isSuccess = this.AssignSAPRoles(sapUserValue, endpoint, biRoles);
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, $"Assign SUSRoles is done");

                            if (!isSuccess)
                            {
                                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, string.Format("Error while adding SRM Roles \"{1}\" for User \"{0}\".", srmDetail.AccountName, srmRolesString), null);
                                return false;
                            }
                            else
                            {
                                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, $"SUSRoles AssignSAPRoles:{isSuccess}");
                            }
                        }
                        else
                        {
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, "SUS new account creation started");
                            bool isSuccess = false;
                            if (this.Accounttype == AccountType.User || this.Accounttype == AccountType.Secondary)
                            {
                                isSuccess = this.CreateSUSUser(user, srmDetail);
                            }
                            else
                            {
                                isSuccess = this.CreateSUSUser(user, srmDetail, srmReqDetail);
                            }
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, $"SUS new account creation status:{isSuccess}");
                            if (isSuccess)
                            {
                                sapUserValue = SAPUserinfo.GetByUserLogonName(srmDetail.AccountName, endpoint.Id);
                                string biRoles = srmDetail.SUSRoles;
                                isSuccess = this.AssignSAPRoles(sapUserValue, endpoint, biRoles);

                                if (!isSuccess)
                                {
                                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, string.Format("Error while adding SRM Roles \"{1}\" for User \"{0}\".", srmDetail.AccountName, srmRolesString), null);
                                    return false;
                                }
                            }

                            if (!isSuccess)
                            {
                                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, string.Format("Error while creating SRM Account for User \"{0}\".", srmDetail.AccountName), null);
                                return false;
                            }
                        }
                    }
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, "SUSRoles Completed");
                }



                var AdhocActionType = fromUndo == false ? CurrentRequest.AdHocAccessRequest.AdhocActionType : AdHocActionType.NewAccount;
                if (CurrentRequest.AdHocAccessRequest != null && CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                {
                    ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentAdd(user, CurrentRequest, srmRolesString, srmDetail.AccountName);
                    ProvisioningEmailHelper.NotifySRMSupportTeam(this.CurrentRequest, srmRolesString, srmDetail.AccountName, UtilityMethods.GetEnumAttribute<Description>(AdhocActionType));

                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, string.Format("SRM Roles \"{1}\" added successfully to User \"{0}\".", srmDetail.AccountName, srmRolesString), null);
                }
                else
                {
                    srmsapUsr = SAPUserinfo.GetByUserLogonName(srmDetail.AccountName, this.Endpoint.Id);
                    if (srmsapUsr == null)
                    {
                        srmsapUsr = new SAPUserinfo();
                    }
                    srmsapUsr.UserLogonName = srmDetail.AccountName;
                    
                    
                    if (this.CurrentRequest.AccountType == AccountType.User)
                    {
                        srmsapUsr.Firstname = user.FirstName;
                        srmsapUsr.Lastname = user.LastName;
                        srmsapUsr.SapUserType = "A";
                        srmsapUsr.UserType = SAPUserType.Dialog;
                    }
                    else if (srmReqDetail != null && srmReqDetail.AdHocSAPSRMSharedAccount != null)
                    {

                        srmsapUsr.Firstname = srmReqDetail.AdHocSAPSRMSharedAccount.Description;
                        srmsapUsr.Lastname = srmReqDetail.AdHocSAPSRMSharedAccount.AccountName;
                        srmsapUsr.UserType = srmReqDetail.AdHocSAPSRMSharedAccount.SAPUserType;
                        srmsapUsr.SapUserType = GetSAPUserType(srmsapUsr.UserType);
                    }
                    srmsapUsr.Password = "FLINS Password";
                    srmsapUsr.EndPointSystemId = this.Endpoint.Id;
                    srmsapUsr.IsActive = true;

                    if (CurrentRequest.ActivationDate != null && CurrentRequest.ActivationDate != DateTime.MinValue)
                    {
                        srmsapUsr.UserValidFrom = this.getSAPDateFormat((DateTime)CurrentRequest.ActivationDate);
                    }
                    else
                    {
                        srmsapUsr.UserValidFrom = this.getSAPDateFormat(DateTime.Now);
                    }

                    srmsapUsr.UserValidTo = this.getSAPDateFormat(GetValidTo(CurrentRequest.UserId).Value);


                    srmsapUsr.Save("System");
                    CreateUserInReconTable(srmsapUsr, "System");
                    //Mail will be send only if NO SUS(SUQ) roles
                    if (sendemail)
                    {
                        ProvisioningEmailHelper.SendEmailForAccountProvisioning(this.CurrentRequest, srmsapUsr, user, ApplicationType.SRM, isForSAPSSO: isSRPSSOEnabled);
                    }

                    ProvisioningEmailHelper.NotifySRMSupportTeam(this.CurrentRequest, srmRolesString, srmDetail.AccountName, UtilityMethods.GetEnumAttribute<Description>(AdhocActionType));

                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, string.Format("User \"{0}\" created in SRM successfully to User.", srmDetail.AccountName), null);
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, string.Format("SRM Roles \"{1}\" added successfully to User \"{0}\".", srmDetail.AccountName, srmRolesString), null);
                }



                _idmsLogger.WriteLog(LogLevelType.Info, "SRM: Update SRM User Role Mapping started");
                this.UpdateSRMUserAndRoleMapping(user, srmReqDetail, AdhocActionType);
                _idmsLogger.WriteLog(LogLevelType.Info, "SRM: Update SRM User Role Mapping Completed");
                srmsapUsr = SAPUserinfo.GetByUserLogonName(srmDetail.AccountName, this.Endpoint.Id);
                if (srmsapUsr != null)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, "SRM: AddBusinessRoleRecon started");
                    AddBusinessRoleRecon(srmsapUsr, srmReqDetail);
                    _idmsLogger.WriteLog(LogLevelType.Info, "SRM: AddBusinessRoleRecon completed");
                }
                isExecutedSuccessfully = true;

            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"{ex.Message} StackTrace:{ex.StackTrace}", ex);
                return isExecutedSuccessfully;
            }

            return isExecutedSuccessfully;
        }

        private void UnLockUserIfLocked(SAPUserinfo sapUserValue, EndPoint endpoint)
        {
            try
            {
                var connector = ConnectorFactory.GetConnector(endpoint, this.CurrentRequest, _idmsLogger);
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, $"UnLockUserIfLocked {endpoint.DisplayName} trying to unlock the account...");
                if (connector.IsUserLocked(sapUserValue.UserLogonName))
                {
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, $"UnLockUserIfLocked {endpoint.DisplayName} UnLockAccount");
                    connector.UnLockAccount(sapUserValue);
                    sapUserValue.IsActive = true;
                    sapUserValue.IsUserLocked = false;
                    sapUserValue.Save(MasterConfigHelper.ServiceUser);
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, $"{endpoint.DisplayName} trying to unlock the account...Ex:{ex.Message}", ex);
            }
        }



        void AddBusinessRoleRecon(SAPUserinfo srmSAPUser, AdHocSAPSRMRequest srmReq)
        {
            try
            {
                AccountReconcilation accrecon = AccountReconcilation.GetByEndpointUserId(srmSAPUser.Id, srmSAPUser.EndPointSystemId);
                if (accrecon == null)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, "SRM: AddBusinessRoleRecon AccountReconcilation null");
                    return;
                }

                var srmMatrices = SAPSRMMainRoleMatrix.GetAllSAPSRMEMainRoleMatrix(true, true);
                foreach (AdHocSAPSRMRequestDetail item in srmReq.AdHocSAPSRMRequestDetails)
                {
                    var businessRole = srmMatrices.Where(m => m.BusinessRole == item.Entitlement).FirstOrDefault();
                    if (businessRole != null)
                    {
                        var entitlementReconcilation = EntitlementReconcilation.GetByAccountReconcilationIdAndEntitlementName(accrecon.Id, businessRole.BusinessRole);
                        if (entitlementReconcilation == null)
                        {
                            entitlementReconcilation = CreateEntitlementReconcilationShell(accrecon, businessRole.BusinessRole, businessRole.Id);
                        }
                        else
                        {
                            entitlementReconcilation.EntitlementId = businessRole.Id;
                            entitlementReconcilation.Entitlement = businessRole.BusinessRole;
                            if (entitlementReconcilation.IsDeleted)
                            {
                                entitlementReconcilation.EntitlementDescription = "";
                            }
                            entitlementReconcilation.IsDeleted = false;
                            entitlementReconcilation.DeletedDate = null;
                        }
                        var userBRoles = SAPSRMBusinessRoleMapping.GetMappingByAccountTypeUserNameAndRole(srmReq.AccountName, item.Entitlement, this.CurrentRequest.AccountType);
                        _idmsLogger.WriteLog(LogLevelType.Info, $"SAPSRMBusinessRoleMapping count:{userBRoles.Count}");
                        var description = userBRoles.GroupBy(j => new { j.BusinessRole, j.POrg }).Select(k => k.Key.POrg + " - " + string.Join(" , ", k.Select(l => l.PGrp))).ToList();
                        entitlementReconcilation.EntitlementDescription = string.Join("<br/>", description);
                        entitlementReconcilation.ValidFromDate = CurrentRequest.ActivationDate.HasValue ? CurrentRequest.ActivationDate.Value : DateTime.Now;
                        entitlementReconcilation.ValidToDate = GetValidTo(CurrentRequest.UserId);
                        entitlementReconcilation.Save("System");
                    }
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"AddBusinessRoleRecon failed", ex);
            }
        }

        void RemoveBusinessRoleRecon(SAPUserinfo srmSAPUser, AdHocSAPSRMRequest srmReq)
        {
            try
            {
                AccountReconcilation accrecon = AccountReconcilation.GetByEndpointUserId(srmSAPUser.Id, srmSAPUser.EndPointSystemId);
                if (accrecon == null)
                    return;
                var entitlementRecon = EntitlementReconcilation.GetByAccountReconcilationId(accrecon.Id, accrecon.EndpointSystemId);
                var userBusinessRoleMapping = SAPSRMBusinessRoleMapping.GetMappingByActiveUserName(srmSAPUser.AccountName, accrecon.AccountType);
                if (userBusinessRoleMapping.Count > 0 && userBusinessRoleMapping.All(i=>!string.IsNullOrWhiteSpace(i.BusinessRole)))
                {
                    var userBRoles = userBusinessRoleMapping.Select(i => i.BusinessRole).Distinct().ToList();
                    userBRoles.ForEach(userBRole =>
                    {
                        var description = userBusinessRoleMapping.Where(h => h.BusinessRole.Equals(userBRole, StringComparison.InvariantCultureIgnoreCase))
                                        .GroupBy(j => new { j.BusinessRole, j.POrg })
                                        .Select(k => k.Key.POrg + "-" + string.Join(",", k.Select(l => l.PGrp))).ToList();
                        var entRecon = entitlementRecon.FirstOrDefault(k => k.Entitlement.ToUpper() == userBRole.ToUpper());
                        if (entRecon != null)
                        {
                            entRecon.EntitlementDescription = string.Join("|", description);
                            entRecon.Save("System");
                        }
                    });
                    var deletedRecords = entitlementRecon.Where(j => !userBRoles.Exists(k => k.Equals(j.Entitlement, StringComparison.InvariantCultureIgnoreCase))).ToList();
                    deletedRecords.ForEach(i =>
                    {
                        i.IsDeleted = true;
                        i.DeletedDate = DateTime.Now;
                        i.AccessRequestId = CurrentRequest.Id;
                        i.Save("System");
                    });
                }
                else
                {
                    entitlementRecon.ForEach(i =>
                    {
                        i.IsDeleted = true;
                        i.DeletedDate = DateTime.Now;
                        i.AccessRequestId = this.CurrentRequest.Id;
                        i.Save("System");
                    });
                }
                //var srmMatrices = SAPSRMMainRoleMatrix.GetAllSAPSRMEMainRoleMatrix(true, true);
                //foreach (AdHocSAPSRMRequestDetail item in srmReq.AdHocSAPSRMRequestDetails)
                //{
                //    var businessRole = srmMatrices.Where(m => m.BusinessRole == item.Entitlement).FirstOrDefault();
                //    if (businessRole != null)
                //    {
                //        var entitlementReconcilation = EntitlementReconcilation.GetByAccountReconcilationIdAndEntitlementId(accrecon.Id, businessRole.Id);
                //        if (entitlementReconcilation != null)
                //        {
                //            entitlementReconcilation.EntitlementId = businessRole.Id;
                //            entitlementReconcilation.Entitlement = businessRole.BusinessRole;
                //            entitlementReconcilation.IsDeleted = false;
                //            entitlementReconcilation.Save("System");
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, "SRM: RemoveBusinessRoleRecon", ex);
            }
        }

        EntitlementReconcilation CreateEntitlementReconcilationShell(AccountReconcilation acctRecon, string entitlement, long entitlementId)
        {
            return new EntitlementReconcilation()
            {
                AccountReconcilationId = acctRecon.Id,
                EndpointSystemId = acctRecon.EndpointSystemId,
                EndpointUserId = acctRecon.EndpointUserId,
                AccountName = acctRecon.AccountName,
                UserId = acctRecon.UserId,
                EnteredDate = DateTime.Now,
                IsDeleted = false,
                DeletedDate = null,
                EntitlementType = Convert.ToInt32(EntitlementType.Role),
                EndpointType = (EndpointType)this.Endpoint.EndpointSystemTypeId,
                Entitlement = entitlement,
                EntitlementId = entitlementId
            };
        }
        bool DeleteAccount(DomainModel.User user, SRMRoleData srmDetail, AdHocSAPSRMRequest srmReqDetail)
        {
            bool isRemovedSuccessfully = false;
            try
            {
                isRemovedSuccessfully = this.LockSRMUser(srmDetail, LockScenario.AdhocRequest, user.Id);
                if (!isRemovedSuccessfully)
                {
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, string.Format("Error while locking SRM User \"{0}\".", srmDetail.AccountName), null);
                    return false;
                }
                ProvisioningEmailHelper.NotifySRMSupportTeam(this.CurrentRequest, string.Empty, srmDetail.AccountName, UtilityMethods.GetEnumAttribute<Description>(CurrentRequest.AdHocAccessRequest.AdhocActionType));

                this.UpdateSRMUserAndRoleMapping(user, srmReqDetail, CurrentRequest.AdHocAccessRequest.AdhocActionType);

                var srmsapUsr = SAPUserinfo.GetByUserLogonName(srmDetail.AccountName, this.Endpoint.Id);
                if (srmsapUsr != null)
                {
                    var accorecon = AccountReconcilation.GetByAccountName(srmDetail.AccountName, this.Endpoint.Id);
                    if (accorecon != null)
                    {
                        var entitlements = EntitlementReconcilation.GetAllActiveByAccountReconcilationId(accorecon.Id);
                        DILog.ProvisioningLogger.WriteLog(LogLevelType.Info, $"Deleting EntitlementReconcilation:{entitlements.Count} started");
                        foreach (var entitlement in entitlements)
                        {
                            if (entitlement.IsDeleted == false)
                            {
                                entitlement.IsDeleted = true;
                                entitlement.DeletedDate = DateTime.Now;
                                entitlement.AccessRequestId = this.CurrentRequest?.Id;
                                entitlement.LastModifiedDate = DateTime.Now;
                                entitlement.Save("System");
                            }
                        }
                        DILog.ProvisioningLogger.WriteLog(LogLevelType.Info, "Deleting EntitlementReconcilation completed");
                        accorecon.IsDeleted = true;
                        accorecon.IsLocked = true;
                        accorecon.DeletedDate = DateTime.Now;
                        accorecon.Save("System");

                    }

                    srmsapUsr.IsActive = false;
                    srmsapUsr.Save("System");
                    isRemovedSuccessfully = true;
                    ProvisioningEmailHelper.SendEmailAfterAdhocUserRemoval(user, Endpoint, CurrentRequest, srmDetail.AccountName);
                }

            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while deleting the SRM account. {0}", ex.Message), ex);
                return false;
            }

            return isRemovedSuccessfully;
        }
        bool AmendentRemove(DomainModel.User user, AdHocSAPSRMRequest srmReqDetail, SRMRoleData srmDetail, string srmRolesString)
        {
            try
            {

                bool isSuccess = this.RemoveSRMRoles(srmReqDetail, srmDetail, user.Id, srmRolesString);
                if (!isSuccess)
                {
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, string.Format("Error while removing SRM Roles \"{1}\" for User \"{0}\".", srmDetail.AccountName, srmRolesString), null);
                    return false;
                }

                //var srmsapUsr = SAPUserinfo.GetByUserLogonName(srmDetail.AccountName, this.Endpoint.Id);
                //if (srmsapUsr != null)
                //{
                //    srmsapUsr = new SAPUserinfo();
                //    srmsapUsr.UserLogonName = srmDetail.AccountName;
                //    srmsapUsr.Password = "FLINS Password";
                //    srmsapUsr.EndPointSystemId = this.Endpoint.Id;
                //    srmsapUsr.Save("System");
                //    CreateUserInReconTable(srmsapUsr, "System");
                //}
                ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentRemove(user, CurrentRequest, srmRolesString, srmDetail.AccountName);
                ProvisioningEmailHelper.NotifySRMSupportTeam(this.CurrentRequest, srmRolesString, srmDetail.AccountName, UtilityMethods.GetEnumAttribute<Description>(CurrentRequest.AdHocAccessRequest.AdhocActionType));

                this.UpdateSRMUserAndRoleMapping(user, srmReqDetail, CurrentRequest.AdHocAccessRequest.AdhocActionType);

                var srmsapUsr = SAPUserinfo.GetByIsActive(srmDetail.AccountName, this.Endpoint.Id);
                if (srmsapUsr != null)
                {
                    RemoveBusinessRoleRecon(srmsapUsr, srmReqDetail);
                }
                return true;

            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while removing roles from the SRM account. {0}", ex.Message), ex);
                return false;
            }
        }
        private string ValidateUserNameDuringReActivation(DomainModel.User user)
        {
            string userName;
            StringBuilder userNameSB = new StringBuilder();
            userName = removeSpecialCharacters(user.DisplayName, false, false, user.Ethinicity);

            if (user.EmployeeType == EmployeeType.Permanent)
                userNameSB.Append("P");
            else
                userNameSB.Append("T");

            if (!string.IsNullOrWhiteSpace(userName)) // && userName.Length >= 3)
            {
                string[] nameAry = userName.Split(' ');
                if (nameAry.Length == 1)
                {
                    if (nameAry[0].Length >= 3)
                        userNameSB.Append(nameAry[0].Substring(0, 3));
                    else
                        userNameSB.Append(nameAry[0]);
                }
                else if (nameAry.Length == 2)
                {
                    if (nameAry[0].Length >= 2)
                    {
                        userNameSB.Append(nameAry[0].Substring(0, 2));
                        userNameSB.Append(nameAry[1].Substring(0, 1));
                    }
                    else if (nameAry[1].Length >= 2)
                    {
                        userNameSB.Append(nameAry[0].Substring(0, 1));
                        userNameSB.Append(nameAry[1].Substring(0, 2));
                    }
                    else
                    {
                        userNameSB.Append(nameAry[0]);
                        userNameSB.Append(nameAry[1]);
                    }
                }
                else if (nameAry.Length >= 3)
                {
                    userNameSB.Append(nameAry[0].Substring(0, 1));
                    userNameSB.Append(nameAry[1].Substring(0, 1));
                    userNameSB.Append(nameAry[2].Substring(0, 1));
                }
            }
            if (!string.IsNullOrWhiteSpace(user.IdentificationNo) && user.IdentificationNo.Length >= 5)
            {
                userNameSB.Append(user.IdentificationNo.Remove(0, user.IdentificationNo.Length - 4));
            }

            userName = userNameSB.ToString().ToUpper();
            userName = this.CheckUserNameExistsForReactivation(userName, user.Id);
            return userName;
        }
        private string CheckUserNameExistsForReactivation(string userName, long DIUserId)
        {
            int no = 1;
            var npacUser = NPACUserInfo.GetByUserIdentity(userName);

            if (npacUser == null)
            {
                return userName;
            }

            var accountRecon = AccountReconcilation.GetAllByEndpointIdAndEndpointUserId(npacUser.EndPointSystemId, npacUser.Id);
            if (accountRecon != null && accountRecon.Count > 0)
            {
                if (accountRecon.Any(a => a.UserId == DIUserId && a.EndpointUserId == npacUser.Id
                                     && a.AccountName == npacUser.UserIdentity))
                    return userName;
            }
            else
            {
                bool isExist = true;
                while (isExist == true)
                {
                    userName = userName.Substring(0, userName.Length - 1) + no;
                    var isUsrExists = NPACUserInfo.GetByUserIdentityAndEndpointId(userName, this.Endpoint.Id);

                    if (isUsrExists != null)
                    {
                        isExist = true;
                    }
                    no++;
                }
                return userName;
            }

            return "";
        }
        private string constructUserPassword(ProvisioningParamCollection fields, long endPointId)
        {
            //if (!string.IsNullOrWhiteSpace(fields["PasswordExpression"].Value) && fields["PasswordExpression"].Value.Length > 0)
            //    return fields["PasswordExpression"].Value;

            string passwd = ConstructPassword(endPointId);
            return passwd;
        }
        private string GetRoleStringFromRequest(AdHocSAPSRMRequest sapDetail, AdHocActionType actionType)
        {
            if (actionType == AdHocActionType.DeleteAccount)
            {
                return string.Empty;
            }
            else
            {
                if (sapDetail.AdHocSAPSRMRequestDetails != null)
                {
                    List<string> roleInfo = new List<string>();

                    foreach (AdHocSAPSRMRequestDetail item in sapDetail.AdHocSAPSRMRequestDetails)
                    {
                        roleInfo.Add(string.Format("{0} - {1} ({2})", item.Entitlement, item.PurchaseOrg, item.PurchaseGroup));
                    }

                    return string.Join(",", roleInfo);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        private SRMRoleData PopulateSRMRoleData(AdHocSAPSRMRequest sapDetail, AdHocActionType actionType)
        {
            SRMRoleData srmData = new SRMRoleData();

            List<string> srpRoles = new List<string>();
            List<string> srpEnablerRoles = new List<string>();
            List<string> susRoles = new List<string>();
            List<string> biRoles = new List<string>();
            List<string> biPOrgRoles = new List<string>();
            List<string> biPGrpRoles = new List<string>();
            List<string> adGroups = new List<string>();
            List<string> srpEnableRolesToBeMaintained = new List<string>();
            //List<string> intPortalRoles = new List<string>();
            //List<string> intPortalGroups = new List<string>();
            //List<string> extPortalRoles = new List<string>();
            //List<string> extPortalGroups = new List<string>();

            // for delete account request account name population is enough
            srmData.AccountName = sapDetail.AccountName;

            if (actionType == AdHocActionType.NewAccount || actionType == AdHocActionType.AmendmentAdd)
            {
                foreach (AdHocSAPSRMRequestDetail detail in sapDetail.AdHocSAPSRMRequestDetails)
                {
                    var srmMatrix = SAPSRMMainRoleMatrix.GetSRMMatrixByRequest(detail);

                    if (srmMatrix != null)
                    {
                        WriteProvisioningLog(LogLevelType.Info, $"SUSRole:{srmMatrix.SUSRoles} - AD Groups:{srmMatrix.ADGroups} - SUP Roles:{srmMatrix.SRMRoles} - Flins BI Roles:{srmMatrix.BIRoles} - PurchaseOrg :{detail.PurchaseOrg} - PurchaseGroup - {detail.PurchaseGroup}");
                        if (!string.IsNullOrWhiteSpace(srmMatrix.ADGroups))
                        {
                            var groups = srmMatrix.ADGroups.Trim().Split(',').ToList();
                            foreach (string group in groups)
                            {
                                if (!adGroups.Contains(group))
                                {
                                    adGroups.Add(group);
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(srmMatrix.SRMRoles))
                        {
                            var groups = srmMatrix.SRMRoles.Trim().Split(',').ToList();
                            foreach (string group in groups)
                            {
                                if (!srpRoles.Contains(group))
                                {
                                    srpRoles.Add(group);
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(srmMatrix.SUSRoles))
                        {
                            var groups = srmMatrix.SUSRoles.Trim().Split(',').ToList();
                            foreach (string group in groups)
                            {
                                if (!susRoles.Contains(group))
                                {
                                    susRoles.Add(group);
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(srmMatrix.BIRoles))
                        {
                            var groups = srmMatrix.BIRoles.Trim().Split(',').ToList();
                            foreach (string group in groups)
                            {
                                if (!biRoles.Contains(group))
                                {
                                    biRoles.Add(group);
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(detail.PurchaseOrg))
                        {
                            var pGrps = detail.PurchaseGroup.Trim().Split(',').ToList();
                            foreach (string pGrp in pGrps)
                            {
                                var enablerRules = srmMatrix.SRMEnablerRule.Split(',').ToList();
                                foreach (string enablerRule in enablerRules)
                                {
                                    var srmEnablerMatrix = SAPSRMEnablerRoleMatrix.GetSRMEnablerMatrixByRequest(enablerRule, detail.PurchaseOrg, pGrp);

                                    if (srmEnablerMatrix != null)
                                    {
                                        if (!string.IsNullOrWhiteSpace(srmEnablerMatrix.SRMTechRole))
                                        {
                                            var groups = srmEnablerMatrix.SRMTechRole.Trim().Split(',').ToList();
                                            foreach (string group in groups)
                                            {
                                                if (!srpEnablerRoles.Contains(group))
                                                {
                                                    srpEnablerRoles.Add(group);
                                                }
                                            }
                                        }

                                        if (!string.IsNullOrWhiteSpace(srmEnablerMatrix.BIPOrgRole))
                                        {
                                            var groups = srmEnablerMatrix.BIPOrgRole.Trim().Split(',').ToList();
                                            foreach (string group in groups)
                                            {
                                                if (!biPOrgRoles.Contains(group))
                                                {
                                                    biPOrgRoles.Add(group);
                                                }
                                            }
                                        }

                                        if (!string.IsNullOrWhiteSpace(srmEnablerMatrix.BIPGrpRole))
                                        {
                                            var groups = srmEnablerMatrix.BIPGrpRole.Trim().Split(',').ToList();
                                            foreach (string group in groups)
                                            {
                                                if (!biPGrpRoles.Contains(group))
                                                {
                                                    biPGrpRoles.Add(group);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, this.Endpoint.Id, LogLevelType.Info, $"AccountName: {srmData?.AccountName} Requested BusinessRole:{detail?.Entitlement} not found in the matrix", null);
                    }
                }
            }
            else if (actionType == AdHocActionType.AmendmentRemove)
            {
                srpEnableRolesToBeMaintained = GetSrpEnablerRolesToBeMaintained(sapDetail);
                var existingRoles = SAPSRMBusinessRoleMapping.GetMappingByUserName(sapDetail.AccountName);
                var userRoles = new List<SAPSRMMainRoleMatrix>();
                foreach (var existingRole in existingRoles)
                {
                    var srmMatrix = SAPSRMMainRoleMatrix.GetSRMMatrixByRequest(existingRole.BusinessRole);
                    if(srmMatrix != null)
                    {
                        userRoles.Add(srmMatrix);
                    }
                }
                List<string> srpRolesToRetain = new List<string>();
                List<string> supRolesToRetain = new List<string>();
                foreach (AdHocSAPSRMRequestDetail detail in sapDetail.AdHocSAPSRMRequestDetails)
                {
                    var rolestoRemove = userRoles.FirstOrDefault(m => m.BusinessRole == detail.Entitlement);
                    if(rolestoRemove != null)
                    {
                        userRoles.Remove(rolestoRemove);

                        foreach (var userRole in userRoles)
                        {
                            userRole.SRMRoles = string.Join(",", userRole.SRMRoles.Split(new char[] {',' },StringSplitOptions.RemoveEmptyEntries).ToList().Intersect(rolestoRemove.SRMRoles.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList()).ToList());
                            userRole.SUSRoles = string.Join(",", userRole.SUSRoles.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList().Intersect(rolestoRemove.SUSRoles.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList()).ToList());
                        }
                    }
                    var srmMatrix = SAPSRMMainRoleMatrix.GetSRMMatrixByRequest(detail);
                  
                    if (srmMatrix != null)
                    {
                        //if retain main role is true then don't remove main roles of all 6 systems, Else remove main roles of all systems
                        if (!detail.RetainMainRole)
                        {
                            if (!string.IsNullOrWhiteSpace(srmMatrix.ADGroups))
                            {
                                var groups = srmMatrix.ADGroups.Trim().Split(',').ToList();
                                foreach (string group in groups)
                                {
                                    if (!adGroups.Contains(group))
                                    {
                                        adGroups.Add(group);
                                    }
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(srmMatrix.SRMRoles))
                            {
                                var groups = srmMatrix.SRMRoles.Trim().Split(',').ToList();
                                foreach (string group in groups)
                                {
                                    if (!srpRoles.Contains(group))
                                    {
                                        srpRoles.Add(group);
                                    }
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(srmMatrix.SUSRoles))
                            {
                                var groups = srmMatrix.SUSRoles.Trim().Split(',').ToList();
                                foreach (string group in groups)
                                {
                                    if (!susRoles.Contains(group))
                                    {
                                        susRoles.Add(group);
                                    }
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(srmMatrix.BIRoles))
                            {
                                var groups = srmMatrix.BIRoles.Trim().Split(',').ToList();
                                foreach (string group in groups)
                                {
                                    if (!biRoles.Contains(group))
                                    {
                                        biRoles.Add(group);
                                    }
                                }
                            }

                            if (string.IsNullOrWhiteSpace(detail.PurchaseOrg))
                            {
                                List<SAPSRMBusinessRoleMapping> existingPorgs = SAPSRMBusinessRoleMapping.GetMappingByUserNameAndRole(sapDetail.AccountName, detail.Entitlement);

                                foreach (SAPSRMBusinessRoleMapping mapping in existingPorgs)
                                {
                                    if (string.IsNullOrWhiteSpace(mapping.PGrp)) continue;
                                    var pGrps = mapping.PGrp.Trim().Split(',').ToList();

                                    foreach (string pGrp in pGrps)
                                    {
                                        var enablerRules = srmMatrix.SRMEnablerRule.Split(',').ToList();

                                        foreach (string enablerRule in enablerRules)
                                        {
                                            var srmEnablerMatrix = SAPSRMEnablerRoleMatrix.GetSRMEnablerMatrixByRequest(enablerRule, mapping.POrg, pGrp);

                                            if (srmEnablerMatrix != null)
                                            {
                                                if (!string.IsNullOrWhiteSpace(srmEnablerMatrix.SRMTechRole))
                                                {
                                                    var groups = srmEnablerMatrix.SRMTechRole.Trim().Split(',').ToList();
                                                    foreach (string group in groups)
                                                    {
                                                        if (!srpEnablerRoles.Contains(group))
                                                        {
                                                            srpEnablerRoles.Add(group);
                                                        }
                                                    }
                                                }

                                                if (!string.IsNullOrWhiteSpace(srmEnablerMatrix.BIPOrgRole))
                                                {
                                                    var groups = srmEnablerMatrix.BIPOrgRole.Trim().Split(',').ToList();
                                                    foreach (string group in groups)
                                                    {
                                                        if (!biPOrgRoles.Contains(group))
                                                        {
                                                            biPOrgRoles.Add(group);
                                                        }
                                                    }
                                                }

                                                if (!string.IsNullOrWhiteSpace(srmEnablerMatrix.BIPGrpRole))
                                                {
                                                    var groups = srmEnablerMatrix.BIPGrpRole.Trim().Split(',').ToList();
                                                    foreach (string group in groups)
                                                    {
                                                        if (!biPGrpRoles.Contains(group))
                                                        {
                                                            biPGrpRoles.Add(group);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(detail.PurchaseOrg))
                        {
                            var pGrps = detail.PurchaseGroup.Trim().Split(',').ToList();
                            foreach (string pGrp in pGrps)
                            {
                                var enablerRule = srmMatrix.SRMEnablerRule.Split(',').ToList();
                                enablerRule.ForEach(item =>
                                {
                                    var srmEnablerMatrix = SAPSRMEnablerRoleMatrix.GetSRMEnablerMatrixByRequest(item, detail.PurchaseOrg, pGrp);

                                    if (srmEnablerMatrix != null)
                                    {
                                        if (!string.IsNullOrWhiteSpace(srmEnablerMatrix.SRMTechRole))
                                        {
                                            var groups = srmEnablerMatrix.SRMTechRole.Trim().Split(',').ToList();
                                            foreach (string group in groups)
                                            {
                                                if (!srpEnablerRoles.Contains(group))
                                                {
                                                    srpEnablerRoles.Add(group);
                                                }
                                            }
                                        }

                                        if (!string.IsNullOrWhiteSpace(srmEnablerMatrix.BIPOrgRole))
                                        {
                                            var groups = srmEnablerMatrix.BIPOrgRole.Trim().Split(',').ToList();
                                            foreach (string group in groups)
                                            {
                                                if (!biPOrgRoles.Contains(group))
                                                {
                                                    biPOrgRoles.Add(group);
                                                }
                                            }
                                        }

                                        if (!string.IsNullOrWhiteSpace(srmEnablerMatrix.BIPGrpRole))
                                        {
                                            var groups = srmEnablerMatrix.BIPGrpRole.Trim().Split(',').ToList();
                                            foreach (string group in groups)
                                            {
                                                if (!biPGrpRoles.Contains(group))
                                                {
                                                    biPGrpRoles.Add(group);
                                                }
                                            }
                                        }
                                    }
                                });

                            }
                        }
                    }
                    else
                    {
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, this.Endpoint.Id, LogLevelType.Info, $"AccountName: {srmData?.AccountName} Requested BusinessRole:{detail?.Entitlement} not found in the matrix", null);
                    }
                }
                if(userRoles != null && userRoles.Count > 0)
                {
                    foreach (var usrRole in userRoles)
                    {
                        srpRolesToRetain.AddRange(usrRole.SRMRoles.SplitBySeparator(",",StringSplitOptions.RemoveEmptyEntries).ToList());
                        supRolesToRetain.AddRange(usrRole.SUSRoles.SplitBySeparator(",", StringSplitOptions.RemoveEmptyEntries).ToList());
                    }
                    if(srpRolesToRetain.Count > 0)
                    {
                        srpRoles = srpRoles.Except(srpRolesToRetain).ToList();
                    }

                    if (supRolesToRetain.Count > 0)
                    {
                        susRoles = susRoles.Except(supRolesToRetain).ToList();
                    }
                }
            }

            srmData.ADGroups = string.Join(",", adGroups);

            srmData.SRPRoles = string.Join(",", srpRoles);
            
            srmData.SRPEnablerRoles = string.Join(",", (actionType == AdHocActionType.AmendmentRemove)?srpEnablerRoles.Except(srpEnableRolesToBeMaintained):srpEnablerRoles);
            srmData.SUSRoles = string.Join(",", susRoles);

            srmData.FLINSBIRoles = string.Join(",", biRoles);
            srmData.FLINSBIPOrgRoles = string.Join(",", biPOrgRoles);
            srmData.FLINSBIPGrpRoles = string.Join(",", biPGrpRoles);

            return srmData;
        }

        private List<string> GetSrpEnablerRolesToBeMaintained(AdHocSAPSRMRequest sapDetail)
        {
            //get the mappings for the requested user
            var existingMappings = SAPSRMBusinessRoleMapping.GetMappingByUserName(sapDetail.AccountName);
            
            //iterate over request details and make amendment over the mappings
            foreach (AdHocSAPSRMRequestDetail item in sapDetail.AdHocSAPSRMRequestDetails)
            {
                var existingMapping=existingMappings.FirstOrDefault(m =>
                         (m.BusinessRole ?? string.Empty) == (item.Entitlement ?? string.Empty) &&
                          (m.POrg ?? string.Empty) == (item.PurchaseOrg ?? string.Empty));

                if (existingMapping != null)
                {
                    if (item.RetainMainRole)
                    {
                        if (!string.IsNullOrWhiteSpace(item.PurchaseOrg))
                        {
                            var existingPGrps = existingMapping.PGrp.Split(',').ToList();
                            var requestedPGrps = item.PurchaseGroup.Split(',').ToList();
                            List<string> pGrps = new List<string>();

                            foreach (string pgrp in existingPGrps)
                            {
                                if (!requestedPGrps.Contains(pgrp))
                                {
                                    pGrps.Add(pgrp);
                                }
                            }
                            existingMapping.PGrp = string.Join(",", pGrps);
                            if (string.IsNullOrWhiteSpace(existingMapping.PGrp))
                            {
                                existingMappings.RemoveAll(m=>m.Id==existingMapping.Id);
                            }
                        }
                    }
                    else
                    {
                        //SAPSRMBusinessRoleMapping.DeleteMappingByUserName(sapDetail.AccountName, item.Entitlement);
                        existingMappings.RemoveAll(m => m.UserName.ToLower() == sapDetail.AccountName.ToLower() && m.BusinessRole.ToLower()==item.Entitlement.ToLower());
                    }
                }
                else
                {
                    if (!item.RetainMainRole)
                    {
                        //SAPSRMBusinessRoleMapping.DeleteMappingByUserName(sapDetail.AccountName, item.Entitlement);
                        existingMappings.RemoveAll(m => m.UserName.ToLower() == sapDetail.AccountName.ToLower() && m.BusinessRole.ToLower() == item.Entitlement.ToLower());
                    }
                }
            }

            List<string> srpEnableRolesToBeMaintained = new List<string>();

            //After amendment removal, iterate over each mapping, list out all SRMTechRole(EnablerRole) 
            foreach (SAPSRMBusinessRoleMapping existingMapping in existingMappings)
            {
                var srmMatrix = SAPSRMMainRoleMatrix.GetSRMMatrixByRequest(existingMapping.BusinessRole);
                if (srmMatrix != null)
                {
                    if (!string.IsNullOrWhiteSpace(existingMapping.POrg))
                    {
                        var pGrps = existingMapping.PGrp.Trim().Split(',').ToList();
                        foreach (string pGrp in pGrps)
                        {
                            var enablerRules = srmMatrix.SRMEnablerRule.Split(',').ToList();
                            foreach (string enablerRule in enablerRules)
                            {
                                var srmEnablerMatrix = SAPSRMEnablerRoleMatrix.GetSRMEnablerMatrixByRequest(enablerRule, existingMapping.POrg, pGrp);

                                if (srmEnablerMatrix != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(srmEnablerMatrix.SRMTechRole))
                                    {
                                        var groups = srmEnablerMatrix.SRMTechRole.Trim().Split(',').ToList();
                                        foreach (string group in groups)
                                        {
                                            if (!srpEnableRolesToBeMaintained.Contains(group))
                                            {
                                                srpEnableRolesToBeMaintained.Add(group);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return srpEnableRolesToBeMaintained;
        }

        private SRMRoleData PopulateSpecialAccountsSRMRoleData(AdHocSAPSRMRequest sapDetail, AdHocActionType actionType)
        {
            SRMRoleData srmData = new SRMRoleData();

            List<string> srpRoles = new List<string>();
            List<string> srpEnablerRoles = new List<string>();
            List<string> susRoles = new List<string>();
            List<string> biRoles = new List<string>();
            List<string> biPOrgRoles = new List<string>();
            List<string> biPGrpRoles = new List<string>();
            List<string> adGroups = new List<string>();
            List<string> srpEnableRolesToBeMaintained = new List<string>();
            //List<string> intPortalRoles = new List<string>();
            //List<string> intPortalGroups = new List<string>();
            //List<string> extPortalRoles = new List<string>();
            //List<string> extPortalGroups = new List<string>();

            // for delete account request account name population is enough
            srmData.AccountName = sapDetail.AccountName;

            if (actionType == AdHocActionType.NewAccount || actionType == AdHocActionType.AmendmentAdd)
            {
                foreach (AdHocSAPSRMRequestDetail detail in sapDetail.AdHocSAPSRMRequestDetails)
                {
                    var srmMatrix = SAPSRMMainRoleMatrix.GetSRMMatrixByRequest(detail);

                    if (srmMatrix != null)
                    {
                        if (!string.IsNullOrWhiteSpace(srmMatrix.ADGroups))
                        {
                            var groups = srmMatrix.ADGroups.Trim().Split(',').ToList();
                            foreach (string group in groups)
                            {
                                if (!adGroups.Contains(group))
                                {
                                    adGroups.Add(group);
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(srmMatrix.SRMRoles))
                        {
                            var groups = srmMatrix.SRMRoles.Trim().Split(',').ToList();
                            foreach (string group in groups)
                            {
                                if (!srpRoles.Contains(group))
                                {
                                    srpRoles.Add(group);
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(srmMatrix.SUSRoles))
                        {
                            var groups = srmMatrix.SUSRoles.Trim().Split(',').ToList();
                            foreach (string group in groups)
                            {
                                if (!susRoles.Contains(group))
                                {
                                    susRoles.Add(group);
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(srmMatrix.BIRoles))
                        {
                            var groups = srmMatrix.BIRoles.Trim().Split(',').ToList();
                            foreach (string group in groups)
                            {
                                if (!biRoles.Contains(group))
                                {
                                    biRoles.Add(group);
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(detail.PurchaseOrg))
                        {
                            var pGrps = detail.PurchaseGroup.Trim().Split(',').ToList();
                            foreach (string pGrp in pGrps)
                            {
                                var enablerRules = srmMatrix.SRMEnablerRule.Split(',').ToList();
                                foreach (string enablerRule in enablerRules)
                                {
                                    var srmEnablerMatrix = SAPSRMEnablerRoleMatrix.GetSRMEnablerMatrixByRequest(enablerRule, detail.PurchaseOrg, pGrp);

                                    if (!string.IsNullOrWhiteSpace(srmEnablerMatrix.SRMTechRole))
                                    {
                                        var groups = srmEnablerMatrix.SRMTechRole.Trim().Split(',').ToList();
                                        foreach (string group in groups)
                                        {
                                            if (!srpEnablerRoles.Contains(group))
                                            {
                                                srpEnablerRoles.Add(group);
                                            }
                                        }
                                    }

                                    if (!string.IsNullOrWhiteSpace(srmEnablerMatrix.BIPOrgRole))
                                    {
                                        var groups = srmEnablerMatrix.BIPOrgRole.Trim().Split(',').ToList();
                                        foreach (string group in groups)
                                        {
                                            if (!biPOrgRoles.Contains(group))
                                            {
                                                biPOrgRoles.Add(group);
                                            }
                                        }
                                    }

                                    if (!string.IsNullOrWhiteSpace(srmEnablerMatrix.BIPGrpRole))
                                    {
                                        var groups = srmEnablerMatrix.BIPGrpRole.Trim().Split(',').ToList();
                                        foreach (string group in groups)
                                        {
                                            if (!biPGrpRoles.Contains(group))
                                            {
                                                biPGrpRoles.Add(group);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, this.Endpoint.Id, LogLevelType.Info,$"AccountName: {srmData.AccountName} Requested BusinessRole:{detail.Entitlement} not found in the matrix", null);
                    }
                }
            }
            else if (actionType == AdHocActionType.AmendmentRemove)
            {
                srpEnableRolesToBeMaintained = GetSrpEnablerRolesToBeMaintained(sapDetail);
                foreach (AdHocSAPSRMRequestDetail detail in sapDetail.AdHocSAPSRMRequestDetails)
                {
                    var srmMatrix = SAPSRMMainRoleMatrix.GetSRMMatrixByRequest(detail);

                    if (srmMatrix != null)
                    {
                        //if retail main role is false then don't remove main roles of all 6 systems, Else remove main roles of all systems
                        if (!detail.RetainMainRole)
                        {
                            if (!string.IsNullOrWhiteSpace(srmMatrix.ADGroups))
                            {
                                var groups = srmMatrix.ADGroups.Trim().Split(',').ToList();
                                foreach (string group in groups)
                                {
                                    if (!adGroups.Contains(group))
                                    {
                                        adGroups.Add(group);
                                    }
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(srmMatrix.SRMRoles))
                            {
                                var groups = srmMatrix.SRMRoles.Trim().Split(',').ToList();
                                foreach (string group in groups)
                                {
                                    if (!srpRoles.Contains(group))
                                    {
                                        srpRoles.Add(group);
                                    }
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(srmMatrix.SUSRoles))
                            {
                                var groups = srmMatrix.SUSRoles.Trim().Split(',').ToList();
                                foreach (string group in groups)
                                {
                                    if (!susRoles.Contains(group))
                                    {
                                        susRoles.Add(group);
                                    }
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(srmMatrix.BIRoles))
                            {
                                var groups = srmMatrix.BIRoles.Trim().Split(',').ToList();
                                foreach (string group in groups)
                                {
                                    if (!biRoles.Contains(group))
                                    {
                                        biRoles.Add(group);
                                    }
                                }
                            }

                            //if purchase is org is empty and 'retain main role' is false, retreive all the org currently assigned to the user , remove one by one
                            if (string.IsNullOrWhiteSpace(detail.PurchaseOrg))
                            {
                                List<SAPSRMBusinessRoleMapping> existingPorgs = SAPSRMBusinessRoleMapping.GetMappingByUserNameAndRole(sapDetail.AccountName, detail.Entitlement); //TO DO : is it necessary to check the account type as well? Since it is not checked for 'user account', assumed the same account name can not be created mulitple times , as different type. Verified against the other source code and retreiving with user name is enough.

                                foreach (SAPSRMBusinessRoleMapping mapping in existingPorgs)
                                {
                                    if (string.IsNullOrWhiteSpace(mapping.PGrp)) continue;
                                    var pGrps = mapping.PGrp.Trim().Split(',').ToList();

                                    foreach (string pGrp in pGrps)
                                    {
                                        var enablerRules = srmMatrix.SRMEnablerRule.Split(',').ToList();

                                        foreach (string enablerRule in enablerRules)
                                        {
                                            var srmEnablerMatrix = SAPSRMEnablerRoleMatrix.GetSRMEnablerMatrixByRequest(enablerRule, mapping.POrg, pGrp);

                                            if (srmEnablerMatrix != null)
                                            {
                                                if (!string.IsNullOrWhiteSpace(srmEnablerMatrix.SRMTechRole))
                                                {
                                                    var groups = srmEnablerMatrix.SRMTechRole.Trim().Split(',').ToList();
                                                    foreach (string group in groups)
                                                    {
                                                        if (!srpEnablerRoles.Contains(group))
                                                        {
                                                            srpEnablerRoles.Add(group);
                                                        }
                                                    }
                                                }

                                                if (!string.IsNullOrWhiteSpace(srmEnablerMatrix.BIPOrgRole))
                                                {
                                                    var groups = srmEnablerMatrix.BIPOrgRole.Trim().Split(',').ToList();
                                                    foreach (string group in groups)
                                                    {
                                                        if (!biPOrgRoles.Contains(group))
                                                        {
                                                            biPOrgRoles.Add(group);
                                                        }
                                                    }
                                                }

                                                if (!string.IsNullOrWhiteSpace(srmEnablerMatrix.BIPGrpRole))
                                                {
                                                    var groups = srmEnablerMatrix.BIPGrpRole.Trim().Split(',').ToList();
                                                    foreach (string group in groups)
                                                    {
                                                        if (!biPGrpRoles.Contains(group))
                                                        {
                                                            biPGrpRoles.Add(group);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(detail.PurchaseOrg))
                        {
                            var pGrps = detail.PurchaseGroup.Trim().Split(',').ToList();
                            foreach (string pGrp in pGrps)
                            {
                                var srmEnablerMatrix = SAPSRMEnablerRoleMatrix.GetSRMEnablerMatrixByRequest(srmMatrix.SRMEnablerRule, detail.PurchaseOrg, pGrp);

                                if (srmEnablerMatrix != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(srmEnablerMatrix.SRMTechRole))
                                    {
                                        var groups = srmEnablerMatrix.SRMTechRole.Trim().Split(',').ToList();
                                        foreach (string group in groups)
                                        {
                                            if (!srpEnablerRoles.Contains(group))
                                            {
                                                srpEnablerRoles.Add(group);
                                            }
                                        }
                                    }

                                    if (!string.IsNullOrWhiteSpace(srmEnablerMatrix.BIPOrgRole))
                                    {
                                        var groups = srmEnablerMatrix.BIPOrgRole.Trim().Split(',').ToList();
                                        foreach (string group in groups)
                                        {
                                            if (!biPOrgRoles.Contains(group))
                                            {
                                                biPOrgRoles.Add(group);
                                            }
                                        }
                                    }

                                    if (!string.IsNullOrWhiteSpace(srmEnablerMatrix.BIPGrpRole))
                                    {
                                        var groups = srmEnablerMatrix.BIPGrpRole.Trim().Split(',').ToList();
                                        foreach (string group in groups)
                                        {
                                            if (!biPGrpRoles.Contains(group))
                                            {
                                                biPGrpRoles.Add(group);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, this.Endpoint.Id, LogLevelType.Info, $"AccountName: {srmData.AccountName} BusinessRole:{detail.Entitlement} not found in the matrix", null);
                    }
                }
            }

            srmData.ADGroups = string.Join(",", adGroups);

            srmData.SRPRoles = string.Join(",", srpRoles);
            //srmData.SRPEnablerRoles = string.Join(",", srpEnablerRoles);
            srmData.SRPEnablerRoles = string.Join(",", (actionType == AdHocActionType.AmendmentRemove) ? srpEnablerRoles.Except(srpEnableRolesToBeMaintained) : srpEnablerRoles);
            srmData.SUSRoles = string.Join(",", susRoles);

            srmData.FLINSBIRoles = string.Join(",", biRoles);
            srmData.FLINSBIPOrgRoles = string.Join(",", biPOrgRoles);
            srmData.FLINSBIPGrpRoles = string.Join(",", biPGrpRoles);

            return srmData;
        }

        private bool AssignADGroups(ADSUserInfo adUserValue, EndPoint endpoint, string adGroups)
        {
            try
            {
                adUserValue.Groups = adGroups.Split(',').ToList();

                ConnectorBase ADConnector = ConnectorFactory.GetConnector(endpoint, this.CurrentRequest);

                bool isExecutedSuccessfully = ADConnector.AddEntitlement(adUserValue, EntitlementType.Group);
                string grpListStr = this.getGroupListAsString(adUserValue.Groups);

                if (isExecutedSuccessfully)
                {
                    ADSUserInfo.AddEntitlement(adUserValue.Id, EntitlementType.Group, adGroups);

                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningADUserAddedInToGroup, adUserValue.sAMAccountName, grpListStr, endpoint.Id), null);
                }
                else
                {

                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningADUserAddedInToGroupFailed, adUserValue.sAMAccountName, grpListStr, endpoint.Id), null);
                }

                return isExecutedSuccessfully;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Exception, ex.Message, ex);
                return false;
            }
        }
        private bool RemoveADGroups(ADSUserInfo adUserValue, EndPoint endpoint, string adGroups)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(adGroups.Trim())) return true;

                adUserValue.Groups = adGroups.Trim().Split(',').ToList();

                ConnectorBase ADConnector = ConnectorFactory.GetConnector(endpoint, this.CurrentRequest);

                bool isExecutedSuccessfully = ADConnector.RemoveEntitlement(adUserValue, EntitlementType.Group);
                string grpListStr = this.getGroupListAsString(adUserValue.Groups);

                if (isExecutedSuccessfully)
                {
                    ADSUserInfo.RemoveEntitlement(adUserValue.Id, EntitlementType.Group, adGroups);

                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningADUserRemovedFromGroup, adUserValue.sAMAccountName, grpListStr, endpoint.Id), null);
                }
                else
                {
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningADUserRemoveFromGroupFailed, adUserValue.sAMAccountName, grpListStr, endpoint.Id), null);
                }
                return isExecutedSuccessfully;

            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Exception, ex.Message, ex);
                return false;
            }
        }
        private bool AssignSAPRoles(SAPUserinfo sapUserValue, EndPoint endpoint, string biRoles)
        {
            try
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.DebugStep, "AssignSAPRoles started");
                ConnectorBase sapConnector = ConnectorFactory.GetConnector(endpoint, this.CurrentRequest);
                //SAP Roles
                List<string> sapRoles = new List<string>();
                string[] rolesAry = !string.IsNullOrWhiteSpace(biRoles) ? biRoles.Split(',') : null;
                if (rolesAry != null)
                {
                    foreach (string tRole in rolesAry)
                    {
                        if (!string.IsNullOrWhiteSpace(tRole))
                        {
                            sapRoles.Add(tRole.Trim());
                        }
                    }
                }
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.DebugStep, $"SAP Roles:{string.Join(",", sapRoles)}");
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.DebugStep, $"sapUserValue:{sapUserValue == null}");
                sapUserValue.Roles = sapRoles;

                //Valid From and Valid To
                if (this.CurrentRequest.ActivationDate != null && this.CurrentRequest.ActivationDate != DateTime.MinValue)
                {
                    sapUserValue.UserValidFrom = this.getSAPDateFormat((DateTime)this.CurrentRequest.ActivationDate);
                }
                else
                {
                    sapUserValue.UserValidFrom = this.getSAPDateFormat(DateTime.Now);
                }

                if (this.CurrentRequest.DeactivationDate != null && this.CurrentRequest.DeactivationDate != DateTime.MinValue)
                {
                    sapUserValue.UserValidTo = this.getSAPDateFormat((DateTime)this.CurrentRequest.DeactivationDate);
                }
                else
                {
                    sapUserValue.UserValidTo = sapUserDefaultValidTo;
                }

                if (sapUserValue.Roles != null && sapUserValue.Roles.Count > 0)
                {
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.DebugStep, $"SAP AddEntitlement started");
                    bool isExecutedSuccessfully = sapConnector.AddEntitlement(sapUserValue, sapUserValue.UserValidFrom, sapUserValue.UserValidTo, EntitlementType.Role);

                    string roleListStr = this.getGroupListAsString(sapUserValue.Roles);
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.DebugStep, $"SAP AddEntitlement IsSuccess:{isExecutedSuccessfully},{roleListStr}");
                    if (isExecutedSuccessfully)
                    {
                        SAPUserinfo.AddSAPUserIntoRoles(sapUserValue);
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserRoleAssigned, sapUserValue.UserLogonName, roleListStr, endpoint.Id), null);
                    }
                    else
                    {
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserRoleAssignmentFailed, sapUserValue.UserLogonName, roleListStr, endpoint.Id), null);
                    }

                    return isExecutedSuccessfully;
                }

                return true;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Exception, ex.Message, ex);
                return true;
            }
        }
        private bool RemoveSAPRoles(SAPUserinfo sapUserValue, EndPoint endpoint, string biRoles)
        {
            try
            {

                ConnectorBase sapConnector = ConnectorFactory.GetConnector(endpoint, this.CurrentRequest);

                //SAP Roles
                List<string> sapRoles = new List<string>();
                string[] rolesAry = !string.IsNullOrWhiteSpace(biRoles) ? biRoles.Split(',') : null;
                if (rolesAry != null)
                {
                    foreach (string tRole in rolesAry)
                    {
                        if (!string.IsNullOrWhiteSpace(tRole))
                        {
                            sapRoles.Add(tRole.Trim());
                        }
                    }
                }
                sapUserValue.Roles = sapRoles;

                //Valid From and Valid To
                if (this.CurrentRequest.ActivationDate != null && this.CurrentRequest.ActivationDate != DateTime.MinValue)
                {
                    sapUserValue.UserValidFrom = this.getSAPDateFormat((DateTime)this.CurrentRequest.ActivationDate);
                }
                else
                {
                    sapUserValue.UserValidFrom = this.getSAPDateFormat(DateTime.Now);
                }

                if (this.CurrentRequest.DeactivationDate != null && this.CurrentRequest.DeactivationDate != DateTime.MinValue)
                {
                    sapUserValue.UserValidTo = this.getSAPDateFormat((DateTime)this.CurrentRequest.DeactivationDate);
                }
                else
                {
                    sapUserValue.UserValidTo = sapUserDefaultValidTo;
                }

                bool isExecutedSuccessfully = sapConnector.RemoveEntitlement(sapUserValue, EntitlementType.Role);

                string roleListStr = this.getGroupListAsString(sapUserValue.Roles);
                if (isExecutedSuccessfully)
                {
                    SAPUserinfo.RemoveEntitlement(sapUserValue.Id, EntitlementType.Role, biRoles);

                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserRemovedFromRoles, sapUserValue.UserLogonName, roleListStr, endpoint.Id), null);
                }
                else
                {
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserRemoveFromRoleFailed, sapUserValue.UserLogonName, roleListStr, endpoint.Id), null);
                }
                return isExecutedSuccessfully;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Exception, ex.Message, ex);
                return true;
            }
        }
        /// <summary>
        /// User for User and Secondary account only
        /// </summary>
        /// <param name="usr"></param>
        /// <param name="srmDetail"></param>
        /// <returns></returns>
        private bool CreateFLINSBIUser(DomainModel.User usr, SRMRoleData srmDetail)
        {
            bool isExecutedSuccessfully = false;
            SAPUserinfo sapUserValue = null;

            var endpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.FLINS_BI);

            var fields = NProvisioningField.GetProvisioningDefaults(endpoint.Id, this.Accounttype);

            ConnectorBase sapConnector = ConnectorFactory.GetConnector(endpoint, this.CurrentRequest);

            fields["Roles"].Value = string.Format("{0},{1},{2},{3}", fields["Roles"].Value, srmDetail.FLINSBIRoles, srmDetail.FLINSBIPGrpRoles, srmDetail.FLINSBIPOrgRoles);

            try
            {
                string adAccountName = null;
                sapUserValue = SAPUserinfo.GetLockedAccountByName(srmDetail.AccountName, usr.Id, endpoint.Id, this.Accounttype);
                
                if (sapUserValue != null)
                {
                    sapUserValue.UserType = SAPUserType.Dialog;
                    sapUserValue.Lastname = usr.KnownAs;
                    sapUserValue.SapUserType = GetSAPUserType(sapUserValue.UserType);
                    sapUserValue.IsActive = true;
                    #region AD AccountName
                    if (this.Accounttype == AccountType.User)
                    {
                        sapUserValue.UserSamAccountName = GetUserADPrimaryAccountName(usr);
                    }
                    #endregion
                    isExecutedSuccessfully = this.UnlockUser(sapUserValue, usr, this.CurrentRequest, endpoint);
                    if (isExecutedSuccessfully)
                    {
                        sapConnector.UpdateSSOAttributes(sapUserValue, sapUserValue.UserSamAccountName, this.Accounttype);
                        this.ReOrg(usr, fields);

                        sapUserValue.Password = this.constructUserPassword(fields, endpoint.Id);
                        sapConnector.ChangePassword(sapUserValue.UserLogonName, sapUserValue.Password, usr.Id, true);
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, $"DeactivatePassword for account:{sapUserValue.UserLogonName}");
                        bool isSuccess = sapConnector.DeactivatePassword(sapUserValue.UserLogonName);
                        if (!isSuccess)
                        {
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Error, string.Format("Error while deactivating password for the account - {0}", sapUserValue.UserLogonName), null);
                        }

                        //SAP Roles
                        List<string> sapRoles = new List<string>();
                        string[] rolesAry = !string.IsNullOrWhiteSpace(fields["Roles"].Value) ? fields["Roles"].Value.Split(',') : null;
                        if (rolesAry != null)
                        {
                            foreach (string tRole in rolesAry)
                            {
                                if (!string.IsNullOrWhiteSpace(tRole))
                                {
                                    sapRoles.Add(tRole.Trim());
                                }
                            }
                        }
                        sapUserValue.Roles = sapRoles;

                        //Valid From and Valid To
                        if (this.CurrentRequest.ActivationDate != null && this.CurrentRequest.ActivationDate != DateTime.MinValue)
                        {
                            sapUserValue.UserValidFrom = this.getSAPDateFormat((DateTime)this.CurrentRequest.ActivationDate);
                        }
                        else
                        {
                            sapUserValue.UserValidFrom = this.getSAPDateFormat(DateTime.Now);
                        }


                        sapUserValue.UserValidTo = this.getSAPDateFormat((DateTime)GetValidTo(this.CurrentRequest.UserId));

                    }
                }
                else
                {
                    sapUserValue = new SAPUserinfo();

                    #region Set SAP User object Values
                    foreach (var gData in fields)
                    {
                        sapUserValue = setUserInfoByAtrbValue(sapUserValue, usr, gData) as SAPUserinfo;
                    }

                    sapUserValue.IsActive = true;
                    sapUserValue.EndPointSystemId = endpoint.Id;
                    sapUserValue.UserLogonName = srmDetail.AccountName;
                    if (string.IsNullOrWhiteSpace(sapUserValue.UserLogonName))
                    {
                        return false;
                    }
                    sapUserValue.Password = this.constructUserPassword(fields, endpoint.Id);
                    sapUserValue.UserType = SAPUserType.Dialog;
                    sapUserValue.SapUserType = GetSAPUserType(sapUserValue.UserType);
                    //SAP Roles
                    List<string> sapRoles = new List<string>();
                    string[] rolesAry = !string.IsNullOrWhiteSpace(fields["Roles"].Value) ? fields["Roles"].Value.Split(',') : null;
                    if (rolesAry != null)
                    {
                        foreach (string tRole in rolesAry)
                        {
                            if (tRole.Trim() != string.Empty)
                                sapRoles.Add(tRole.Trim());
                        }
                    }
                    sapUserValue.Roles = sapRoles.Distinct().ToList();

                    //SAP Params
                    Dictionary<string, string> paramsInfoDictionary = new Dictionary<string, string>();
                    if (!string.IsNullOrWhiteSpace(fields["Parameters"].Value))
                    {
                        string[] sapParamList = Regex.Split(fields["Parameters"].Value, "##PP##");
                        string[] tmpParamInfo;
                        if (sapParamList != null)
                        {
                            foreach (string param in sapParamList)
                            {
                                tmpParamInfo = param.Split(',');
                                if (tmpParamInfo.Length == 2)
                                {
                                    paramsInfoDictionary.Add(tmpParamInfo[0], tmpParamInfo[1]);
                                }
                            }

                            //paramsInfoDictionary.Add("CYMP_PERSON", "");
                        }
                    }
                    sapUserValue.Parameters = paramsInfoDictionary;

                    //OutPut Device - Default Printer
                    if (!string.IsNullOrWhiteSpace(fields["OutputDevice"].Value))
                        sapUserValue.OuputDevice = fields["OutputDevice"].Value;

                    //Valid From and Valid To

                    if (this.CurrentRequest.ActivationDate != null && this.CurrentRequest.ActivationDate != DateTime.MinValue)
                    {
                        sapUserValue.UserValidFrom = this.getSAPDateFormat((DateTime)this.CurrentRequest.ActivationDate);
                    }
                    else
                    {
                        sapUserValue.UserValidFrom = this.getSAPDateFormat(DateTime.Now);
                    }

                    sapUserValue.UserValidTo = this.getSAPDateFormat((DateTime)GetValidTo(this.CurrentRequest.UserId));

                    #endregion

                    #region AD AccountName
                    if(this.Accounttype == AccountType.User)
                    {
                        sapUserValue.UserSamAccountName = GetUserADPrimaryAccountName(usr);
                    }
                    #endregion
                    isExecutedSuccessfully = sapConnector.CreateAccount(sapUserValue, fields);
                    if (isExecutedSuccessfully)
                    {
                        this.addNewUserintoSAPUserInfo(sapUserValue);
                        CreateUserInReconTable(sapUserValue, "System");
                        //SAPUserinfo.AddEntitlement(sapUserValue.Id, EntitlementType.Role, fields["Roles"].Value);
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, $"DeactivatePassword for account:{sapUserValue.UserLogonName}");
                        bool isSuccess = sapConnector.DeactivatePassword(sapUserValue.UserLogonName);
                        if (!isSuccess)
                        {
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Error, string.Format("Error while deactivating password for the account - {0}", sapUserValue.UserLogonName), null);
                        }

                        if (this.CurrentRequest.AdHocAccessRequestId.HasValue)
                        {
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreated, sapUserValue.UserLogonName, endpoint.DisplayName), null);
                        }
                        else
                        {
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreated, sapUserValue.UserLogonName, endpoint.DisplayName), null);
                        }

                    }
                    else
                    {
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreationFailed, sapUserValue.UserLogonName, endpoint.DisplayName), null);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Exception, ex.Message, ex);
            }

            return isExecutedSuccessfully;
        }
        /// <summary>
        /// Used for User and Secondary account
        /// </summary>
        /// <param name="usr"></param>
        /// <param name="srmDetail"></param>
        /// <returns></returns>
        private bool CreateSRPUser(DomainModel.User usr, SRMRoleData srmDetail)
        {
            bool isExecutedSuccessfully = false;
            SAPUserinfo sapUserValue = null;

            var endpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.SAP_SRP);
            var isSSoEnabled = EndpointConfiguration.IsSSOEnableForEndpoint(endpoint.Id) && this.Accounttype == AccountType.User;

            var fields = NProvisioningField.GetProvisioningDefaults(endpoint.Id, this.Accounttype);
            ConnectorBase sapConnector = ConnectorFactory.GetConnector(endpoint, this.CurrentRequest);

            fields["Roles"].Value = string.Format("{0},{1},{2}", fields["Roles"].Value, srmDetail.SRPRoles, srmDetail.SRPEnablerRoles);

            try
            {
                sapUserValue = SAPUserinfo.GetLockedAccountByName(srmDetail.AccountName, usr.Id, endpoint.Id, this.Accounttype);

                if (sapUserValue != null && sapConnector.IsUserExists(sapUserValue.AccountName))
                {
                    #region AD AccountName
                    if (this.Accounttype == AccountType.User)
                    {
                        sapUserValue.UserSamAccountName = GetUserADPrimaryAccountName(usr);
                    }
                    #endregion
                    sapUserValue.IsActive = true;
                    sapUserValue.UserType = SAPUserType.Dialog;
                    sapUserValue.SapUserType = GetSAPUserType(sapUserValue.UserType);
                    //Valid From and Valid To
                    if (this.CurrentRequest.ActivationDate != null && this.CurrentRequest.ActivationDate != DateTime.MinValue)
                    {
                        sapUserValue.UserValidFrom = this.getSAPDateFormat((DateTime)this.CurrentRequest.ActivationDate);
                    }
                    else
                    {
                        sapUserValue.UserValidFrom = this.getSAPDateFormat(DateTime.Now);
                    }

                    if (this.CurrentRequest.DeactivationDate != null && this.CurrentRequest.DeactivationDate != DateTime.MinValue)
                    {
                        sapUserValue.UserValidTo = this.getSAPDateFormat((DateTime)this.CurrentRequest.DeactivationDate);
                    }
                    else
                    {
                        sapUserValue.UserValidTo = sapUserDefaultValidTo;
                    }
                    if (fields["Groups"] != null && !string.IsNullOrWhiteSpace(fields["Groups"].Value))
                    {
                        foreach (var item in fields["Groups"].Value.Split(','))
                        {
                            if (!string.IsNullOrWhiteSpace(item) && !sapUserValue.Groups.Contains(item))
                                sapUserValue.Groups.Add(item);
                        }
                    }
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, string.Format("User {0} Unlocked in SAP started.", sapUserValue.UserLogonName), null);
                    isExecutedSuccessfully = this.UnlockUser(sapUserValue, usr, this.CurrentRequest, endpoint);
                    if (isExecutedSuccessfully)
                    {
                        sapConnector.UpdateSSOAttributes(sapUserValue, sapUserValue.UserSamAccountName, this.Accounttype);
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, string.Format("User {0} Unlocked in SAP successful.", sapUserValue.UserLogonName), null);
                        CreateUserInReconTable(sapUserValue, "System");

                        //this.UnLockDependentSystemsInIDMS(sapUserValue, true);
                        sapUserValue.Password = this.constructUserPassword(fields, endpoint.Id);
                        //Below condition is required to deactivate the password. Please do not confuse with SRP and SUS role condition check here.
                        if (!string.IsNullOrWhiteSpace(srmDetail.SUSRoles))
                        {
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, $"SUSRoles Exists,ResetPassword for account:{sapUserValue.UserLogonName}");
                            sapConnector.ResetPassword(sapUserValue.UserLogonName, sapUserValue.Password,string.Empty);
                            ProvisioningEmailHelper.SendEmailForAccountProvisioning(this.CurrentRequest, sapUserValue, usr, ApplicationType.SAP_SRP,isForSAPSSO: isSSoEnabled);
                        }
                        else
                        {
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, $"SUSRoles does not exists,ChangePassword for account:{sapUserValue.UserLogonName}");
                            sapConnector.ChangePassword(sapUserValue.UserLogonName, sapUserValue.Password, usr.Id, true);
                            System.Threading.Thread.Sleep(30000);
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, $"DeactivatePassword for account:{sapUserValue.UserLogonName}");
                            bool isSuccess = sapConnector.DeactivatePassword(sapUserValue.UserLogonName);
                            if (!isSuccess)
                            {
                                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Error, string.Format("Error while deactivating password for the account - {0}", sapUserValue.UserLogonName), null);
                            }
                            else
                            {
                                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, $"Deactivating password for the account - {sapUserValue.UserLogonName} successful.");
                            }
                        }

                        //SAP Roles
                        List<string> sapRoles = new List<string>();
                        string[] rolesAry = !string.IsNullOrWhiteSpace(fields["Roles"].Value) ? fields["Roles"].Value.Split(',') : null;
                        if (rolesAry != null)
                        {
                            foreach (string tRole in rolesAry)
                            {
                                if (!string.IsNullOrWhiteSpace(tRole))
                                {
                                    sapRoles.Add(tRole.Trim());
                                }
                            }
                        }
                        sapUserValue.Roles = sapRoles.Distinct().ToList();

                   
                    }
                }
                else
                {
                    sapUserValue = new SAPUserinfo();

                    #region Set SAP User object Values
                    foreach (var gData in fields)
                    {
                        sapUserValue = setUserInfoByAtrbValue(sapUserValue, usr, gData) as SAPUserinfo;
                    }

                    sapUserValue.IsActive = true;
                    sapUserValue.EndPointSystemId = endpoint.Id;
                    sapUserValue.UserLogonName = srmDetail.AccountName;
                    if (string.IsNullOrWhiteSpace(sapUserValue.UserLogonName))
                    {
                        return false;
                    }
                    sapUserValue.Password = this.constructUserPassword(fields, endpoint.Id);
                    sapUserValue.UserType = SAPUserType.Dialog;
                    sapUserValue.SapUserType = GetSAPUserType(sapUserValue.UserType);
                    //SAP Roles
                    List<string> sapRoles = new List<string>();
                    string[] rolesAry = !string.IsNullOrWhiteSpace(fields["Roles"].Value) ? fields["Roles"].Value.Split(',') : null;
                    if (rolesAry != null)
                    {
                        foreach (string tRole in rolesAry)
                        {
                            if (tRole.Trim() != string.Empty)
                                sapRoles.Add(tRole.Trim());
                        }
                    }
                    sapUserValue.Roles = sapRoles.Distinct().ToList();

                    //SAP Params
                    Dictionary<string, string> paramsInfoDictionary = new Dictionary<string, string>();
                    if (!string.IsNullOrWhiteSpace(fields["Parameters"].Value))
                    {
                        string[] sapParamList = Regex.Split(fields["Parameters"].Value, "##PP##");
                        string[] tmpParamInfo;
                        if (sapParamList != null)
                        {
                            foreach (string param in sapParamList)
                            {
                                tmpParamInfo = param.Split(',');
                                if (tmpParamInfo.Length == 2)
                                {
                                    paramsInfoDictionary.Add(tmpParamInfo[0], tmpParamInfo[1]);
                                }
                            }

                            //paramsInfoDictionary.Add("CYMP_PERSON", "");
                        }
                    }
                    sapUserValue.Parameters = paramsInfoDictionary;

                    if (fields["Groups"] != null && !string.IsNullOrWhiteSpace(fields["Groups"].Value))
                    {
                        foreach (var item in fields["Groups"].Value.Split(','))
                        {
                            if (!string.IsNullOrWhiteSpace(item) && !sapUserValue.Groups.Contains(item))
                                sapUserValue.Groups.Add(item);
                        }
                    }

                    //OutPut Device - Default Printer
                    if (!string.IsNullOrWhiteSpace(fields["OutputDevice"].Value))
                        sapUserValue.OuputDevice = fields["OutputDevice"]?.Value;

                    //Valid From and Valid To

                    if (this.CurrentRequest.ActivationDate != null && this.CurrentRequest.ActivationDate != DateTime.MinValue)
                    {
                        sapUserValue.UserValidFrom = this.getSAPDateFormat((DateTime)this.CurrentRequest.ActivationDate);
                    }
                    else
                    {
                        sapUserValue.UserValidFrom = this.getSAPDateFormat(DateTime.Now);
                    }

                    if (this.CurrentRequest.DeactivationDate != null && this.CurrentRequest.DeactivationDate != DateTime.MinValue)
                    {
                        sapUserValue.UserValidTo = this.getSAPDateFormat((DateTime)this.CurrentRequest.DeactivationDate);
                    }
                    else
                    {
                        sapUserValue.UserValidTo = sapUserDefaultValidTo;
                    }
                    #endregion
                    #region AD AccountName
                    if (this.Accounttype == AccountType.User)
                    {
                        sapUserValue.UserSamAccountName = GetUserADPrimaryAccountName(usr);
                    }
                    #endregion
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, $"CreateAccount CreateSRPUser started.");
                    isExecutedSuccessfully = sapConnector.CreateAccount(sapUserValue, fields);
                    if (isExecutedSuccessfully)
                    {
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, $"CreateAccount CreateSRPUser completed.");
                        this.addNewUserintoSAPUserInfo(sapUserValue);
                        CreateUserInReconTable(sapUserValue, "System");

                        //SAPUserinfo.AddEntitlement(sapUserValue.Id, EntitlementType.Role, string.Join("", sapUserValue.Roles));
                        //Below condition is required to deactivate the password. Please do not confuse with SRP and SUS role condition check here.
                        if (!string.IsNullOrWhiteSpace(srmDetail.SUSRoles))
                        {
                            ProvisioningEmailHelper.SendEmailForAccountProvisioning(this.CurrentRequest, sapUserValue, usr, ApplicationType.SAP_SRP,isForSAPSSO: isSSoEnabled);
                        }
                        else
                        {
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, $"DeactivatePassword for account:{sapUserValue.UserLogonName}");
                            bool isSuccess = sapConnector.DeactivatePassword(sapUserValue.UserLogonName);
                            if (!isSuccess)
                            {
                                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Error, string.Format("Error while deactivating password for the account - {0}", sapUserValue.UserLogonName), null);
                            }
                        }

                        if (this.CurrentRequest.AdHocAccessRequestId.HasValue)
                        {
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreated, sapUserValue.UserLogonName, endpoint.DisplayName), null);
                        }
                        else
                        {
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreated, sapUserValue.UserLogonName, endpoint.DisplayName), null);
                        }

                    }
                    else
                    {
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreationFailed, sapUserValue.UserLogonName, endpoint.DisplayName), null);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Exception, $"Message:{ex.Message} StackTrace:{ex.StackTrace}", ex);
            }

            return isExecutedSuccessfully;
        }
        /// <summary>
        /// Used for User and Secondary account creation
        /// </summary>
        /// <param name="usr"></param>
        /// <param name="srmDetail"></param>
        /// <returns></returns>
        private bool CreateSUSUser(DomainModel.User usr, SRMRoleData srmDetail)
        {
            bool isExecutedSuccessfully = false;
            SAPUserinfo sapUserValue = null;

            var endpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.SAP_SUS);
            var isSSoEnabled = EndpointConfiguration.IsSSOEnableForEndpoint(endpoint.Id) && this.Accounttype == AccountType.User;

            var fields = NProvisioningField.GetProvisioningDefaults(endpoint.Id, this.Accounttype);

            ConnectorBase sapConnector = ConnectorFactory.GetConnector(endpoint, this.CurrentRequest);
            if (sapConnector.Connect())
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, $"Sap connection:successful");
            }

            fields["Roles"].Value = string.Format("{0},{1}", fields["Roles"].Value, srmDetail.SUSRoles);

            try
            {
                sapUserValue = SAPUserinfo.GetLockedAccountByName(srmDetail.AccountName, usr.Id, endpoint.Id, this.Accounttype);

                if (sapUserValue != null && sapConnector.IsUserExists(sapUserValue.AccountName))
                {
                    #region AD AccountName
                    if (this.Accounttype == AccountType.User)
                    {
                        sapUserValue.UserSamAccountName = GetUserADPrimaryAccountName(usr);
                    }
                    #endregion
                    sapUserValue.IsActive = true;
                    foreach (var gData in fields)
                    {
                        sapUserValue = setUserInfoByAtrbValue(sapUserValue, usr, gData) as SAPUserinfo;
                    }
                    sapUserValue.UserType = SAPUserType.Dialog;
                    sapUserValue.SapUserType = GetSAPUserType(sapUserValue.UserType);
                    //Valid From and Valid To
                    if (this.CurrentRequest.ActivationDate != null && this.CurrentRequest.ActivationDate != DateTime.MinValue)
                    {
                        sapUserValue.UserValidFrom = this.getSAPDateFormat((DateTime)this.CurrentRequest.ActivationDate);
                    }
                    else
                    {
                        sapUserValue.UserValidFrom = this.getSAPDateFormat(DateTime.Now);
                    }


                    sapUserValue.UserValidTo = this.getSAPDateFormat((DateTime)GetValidTo(this.CurrentRequest.UserId)); ;


                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, "CreateSUSUser UnlockUser started");
                    isExecutedSuccessfully = this.UnlockUser(sapUserValue, usr, this.CurrentRequest, endpoint);
                    if (isExecutedSuccessfully)
                    {
                        sapConnector.UpdateSSOAttributes(sapUserValue, sapUserValue.UserSamAccountName, this.Accounttype);

                        sapUserValue.Password = this.constructUserPassword(fields, endpoint.Id);
                        if (this.CurrentRequest.AccountType == AccountType.Service)
                        {
                            sapConnector.ChangePassword(sapUserValue.UserLogonName, sapUserValue.Password, usr.Id, true);
                        }
                        else
                        {
                            sapConnector.ResetPassword(sapUserValue.UserLogonName, sapUserValue.Password, string.Empty);
                        }
                        //ProvisioningEmailHelper.SendEmailForAccountProvisioning(this.CurrentRequest, sapUserValue, usr, ApplicationType.SAP_SUS);
                    }
                }
                else
                {
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, "CreateSUSUser New User started");
                    sapUserValue = new SAPUserinfo();

                    #region Set SAP User object Values
                    foreach (var gData in fields)
                    {
                        sapUserValue = setUserInfoByAtrbValue(sapUserValue, usr, gData) as SAPUserinfo;
                    }

                    sapUserValue.IsActive = true;
                    sapUserValue.EndPointSystemId = endpoint.Id;
                    sapUserValue.UserLogonName = srmDetail.AccountName;
                    if (string.IsNullOrWhiteSpace(sapUserValue.UserLogonName))
                    {
                        return false;
                    }
                    sapUserValue.Password = this.constructUserPassword(fields, endpoint.Id);

                    sapUserValue.UserType = SAPUserType.Dialog;
                    sapUserValue.SapUserType = GetSAPUserType(sapUserValue.UserType);

                    //SAP Params
                    Dictionary<string, string> paramsInfoDictionary = new Dictionary<string, string>();
                    if (!string.IsNullOrWhiteSpace(fields["Parameters"].Value))
                    {
                        string[] sapParamList = Regex.Split(fields["Parameters"].Value, "##PP##");
                        string[] tmpParamInfo;
                        if (sapParamList != null)
                        {
                            foreach (string param in sapParamList)
                            {
                                tmpParamInfo = param.Split(',');
                                if (tmpParamInfo.Length == 2)
                                {
                                    paramsInfoDictionary.Add(tmpParamInfo[0], tmpParamInfo[1]);
                                }
                            }

                            //paramsInfoDictionary.Add("CYMP_PERSON", "");
                        }
                    }
                    sapUserValue.Parameters = paramsInfoDictionary;

                    //OutPut Device - Default Printer
                    if (!string.IsNullOrWhiteSpace(fields["OutputDevice"].Value))
                    {
                        sapUserValue.OuputDevice = fields["OutputDevice"].Value;
                    }

                    //Valid From and Valid To

                    if (this.CurrentRequest.ActivationDate != null && this.CurrentRequest.ActivationDate != DateTime.MinValue)
                    {
                        sapUserValue.UserValidFrom = this.getSAPDateFormat((DateTime)this.CurrentRequest.ActivationDate);
                    }
                    else
                    {
                        sapUserValue.UserValidFrom = this.getSAPDateFormat(DateTime.Now);
                    }


                    sapUserValue.UserValidTo = this.getSAPDateFormat((DateTime)GetValidTo(this.CurrentRequest.UserId)); ;
                    if (fields["Groups"] != null && !string.IsNullOrWhiteSpace(fields["Groups"].Value))
                    {
                        foreach (var item in fields["Groups"].Value.Split(','))
                        {
                            if (!string.IsNullOrWhiteSpace(item) && !sapUserValue.Groups.Contains(item))
                                sapUserValue.Groups.Add(item);
                        }
                    }
                    #region AD AccountName
                    if (this.Accounttype == AccountType.User)
                    {
                        sapUserValue.UserSamAccountName = GetUserADPrimaryAccountName(usr);
                    }
                    #endregion
                    #endregion
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, $"CreateAccount CreateSUSUser started.");
                    isExecutedSuccessfully = sapConnector.CreateAccount(sapUserValue, fields);
                }
                if (isExecutedSuccessfully)
                {
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, $"CreateAccount CreateSUSUser completed.");
                    ProvisioningEmailHelper.SendEmailForAccountProvisioning(this.CurrentRequest, sapUserValue, usr, ApplicationType.SAP_SUS,isForSAPSSO: isSSoEnabled);
                    this.addNewUserintoSAPUserInfo(sapUserValue);
                    CreateUserInReconTable(sapUserValue, "System");
                    // SAPUserinfo.AddEntitlement(sapUserValue.Id, EntitlementType.Role, fields["Roles"].Value);

                    //this.CreateDependentSystemEntriesInIDMS(sapUserValue, false);

                    if (this.CurrentRequest.AdHocAccessRequestId.HasValue)
                    {
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreated, sapUserValue.UserLogonName, endpoint.DisplayName), null);
                    }
                    else
                    {
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreated, sapUserValue.UserLogonName, endpoint.DisplayName), null);
                    }

                }
                else
                {
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreationFailed, sapUserValue.UserLogonName, endpoint.DisplayName), null);
                }

            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Exception, ex.Message, ex);
            }

            return isExecutedSuccessfully;
        }
        private bool CreateFLINSBIUser(DomainModel.User usr, SRMRoleData srmDetail, AdHocSAPSRMRequest sapDetail)
        {
            bool isExecutedSuccessfully = false;
            SAPUserinfo sapUserValue = null;

            var endpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.FLINS_BI);
            var isSSoEnabled = EndpointConfiguration.IsSSOEnableForEndpoint(endpoint.Id) && this.Accounttype == AccountType.User;

            var fields = NProvisioningField.GetProvisioningDefaults(endpoint.Id, this.Accounttype);

            ConnectorBase sapConnector = ConnectorFactory.GetConnector(endpoint, this.CurrentRequest);

            fields["Roles"].Value = string.Format("{0},{1},{2},{3}", fields["Roles"].Value, srmDetail.FLINSBIRoles, srmDetail.FLINSBIPGrpRoles, srmDetail.FLINSBIPOrgRoles);

            try
            {


                sapUserValue = SAPUserinfo.GetLockedAccountByName(srmDetail.AccountName, usr.Id, endpoint.Id, this.Accounttype);

                if (sapUserValue != null)
                {
                    sapUserValue.IsUserLocked = false;
                    sapUserValue.IsActive = true;
                    if (sapDetail != null && sapDetail.AdHocSAPSRMSharedAccount != null)
                    {
                        sapUserValue.UserType = sapDetail.AdHocSAPSRMSharedAccount.SAPUserType;
                        sapUserValue.SapUserType = GetSAPUserType(sapUserValue.UserType);
                        sapUserValue.Firstname = sapDetail.AdHocSAPSRMSharedAccount.Description;
                    }
                    else
                    {
                        var endpoint_srq = EndPoint.GetEndPointByApplicationType(ApplicationType.SAP_SRP);
                        var srmsap = SAPUserinfo.GetActiveByUserLogonName(srmDetail.AccountName, endpoint_srq.Id);
                        if (srmsap != null)
                        {
                            sapUserValue.UserType = srmsap.UserType;
                            sapUserValue.SapUserType = GetSAPUserType(sapUserValue.UserType);
                            sapUserValue.Lastname = srmsap.Lastname;
                            sapUserValue.Firstname = srmsap.Firstname;
                        }
                    }

                    isExecutedSuccessfully = this.UnlockUser(sapUserValue, usr, this.CurrentRequest, endpoint);
                    if (isExecutedSuccessfully)
                    {
                        sapUserValue.Password = this.constructUserPassword(fields, endpoint.Id);
                        sapConnector.ChangePassword(sapUserValue.UserLogonName, sapUserValue.Password, usr.Id, true);
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, $"DeactivatePassword for account:{sapUserValue.UserLogonName}");
                        bool isSuccess = sapConnector.DeactivatePassword(sapUserValue.UserLogonName);
                        if (!isSuccess)
                        {
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Error, string.Format("Error while deactivating password for the account - {0}", sapUserValue.UserLogonName), null);
                        }

                        //SAP Roles
                        List<string> sapRoles = new List<string>();
                        string[] rolesAry = !string.IsNullOrWhiteSpace(fields["Roles"].Value) ? fields["Roles"].Value.Split(',') : null;
                        if (rolesAry != null)
                        {
                            foreach (string tRole in rolesAry)
                            {
                                if (!string.IsNullOrWhiteSpace(tRole))
                                {
                                    sapRoles.Add(tRole.Trim());
                                }
                            }
                        }
                        sapUserValue.Roles = sapRoles;

                        //Valid From and Valid To
                        if (this.CurrentRequest.ActivationDate != null && this.CurrentRequest.ActivationDate != DateTime.MinValue)
                        {
                            sapUserValue.UserValidFrom = this.getSAPDateFormat((DateTime)this.CurrentRequest.ActivationDate);
                        }
                        else
                        {
                            sapUserValue.UserValidFrom = this.getSAPDateFormat(DateTime.Now);
                        }


                        sapUserValue.UserValidTo = this.getSAPDateFormat((DateTime)GetValidTo(this.CurrentRequest.UserId));

                    }
                }
                else
                {
                    sapUserValue = new SAPUserinfo();

                    #region Set SAP User object Values
                    sapUserValue.IsActive = true;
                    sapUserValue.EndPointSystemId = endpoint.Id;
                    sapUserValue.UserLogonName = sapDetail.AccountName;
                    sapUserValue.Lastname = sapDetail.AccountName;
                    var entitlement = ((ISAPUserType)sapDetail.AdHocSAPSRMSharedAccount);

                    if (entitlement != null)
                    {
                        sapUserValue.Firstname = entitlement.Description;
                        sapUserValue.UserType = entitlement.SAPUserType;
                        sapUserValue.SapUserType = GetSAPUserType(sapUserValue.UserType);
                    }
                    else
                    {
                        var endpoint_srq = EndPoint.GetEndPointByApplicationType(ApplicationType.SAP_SRP);
                        var srmsap = SAPUserinfo.GetActiveByUserLogonName(srmDetail.AccountName, endpoint_srq.Id);
                        if (srmsap != null)
                        {
                            sapUserValue.UserType = srmsap.UserType;
                            sapUserValue.SapUserType = GetSAPUserType(sapUserValue.UserType);
                            sapUserValue.Lastname = srmsap.Lastname;
                            sapUserValue.Firstname = srmsap.Firstname;

                        }
                    }


                    sapUserValue.Lastname = sapUserValue.UserLogonName;
                    if (string.IsNullOrWhiteSpace(sapUserValue.UserLogonName))
                    {
                        return false;
                    }
                    sapUserValue.Password = this.constructUserPassword(fields, endpoint.Id);



                    //SAP Roles
                    List<string> sapRoles = new List<string>();
                    string[] rolesAry = !string.IsNullOrWhiteSpace(fields["Roles"].Value) ? fields["Roles"].Value.Split(',') : null;
                    if (rolesAry != null)
                    {
                        foreach (string tRole in rolesAry)
                        {
                            if (tRole.Trim() != string.Empty)
                                sapRoles.Add(tRole.Trim());
                        }
                    }
                    sapUserValue.Roles = sapRoles.Distinct().ToList();

                    //SAP Params
                    Dictionary<string, string> paramsInfoDictionary = new Dictionary<string, string>();
                    if (!string.IsNullOrWhiteSpace(fields["Parameters"].Value))
                    {
                        string[] sapParamList = Regex.Split(fields["Parameters"].Value, "##PP##");
                        string[] tmpParamInfo;
                        if (sapParamList != null)
                        {
                            foreach (string param in sapParamList)
                            {
                                tmpParamInfo = param.Split(',');
                                if (tmpParamInfo.Length == 2)
                                {
                                    paramsInfoDictionary.Add(tmpParamInfo[0], tmpParamInfo[1]);
                                }
                            }

                            //paramsInfoDictionary.Add("CYMP_PERSON", "");
                        }
                    }
                    sapUserValue.Parameters = paramsInfoDictionary;

                    //Valid From and Valid To

                    if (this.CurrentRequest.ActivationDate != null && this.CurrentRequest.ActivationDate != DateTime.MinValue)
                    {
                        sapUserValue.UserValidFrom = this.getSAPDateFormat((DateTime)this.CurrentRequest.ActivationDate);
                    }
                    else
                    {
                        sapUserValue.UserValidFrom = this.getSAPDateFormat(DateTime.Now);
                    }

                    if (this.CurrentRequest.DeactivationDate != null && this.CurrentRequest.DeactivationDate != DateTime.MinValue)
                    {
                        sapUserValue.UserValidTo = this.getSAPDateFormat((DateTime)this.CurrentRequest.DeactivationDate);
                    }
                    else
                    {
                        sapUserValue.UserValidTo = sapUserDefaultValidTo;
                    }

                    #endregion
                    isExecutedSuccessfully = sapConnector.CreateAccount(sapUserValue, fields);
                }
                if (isExecutedSuccessfully)
                {
                    this.addNewUserintoSAPUserInfo(sapUserValue);
                    CreateUserInReconTable(sapUserValue, "System");
                    //SAPUserinfo.AddEntitlement(sapUserValue.Id, EntitlementType.Role, fields["Roles"].Value);

                    ProvisioningEmailHelper.SendEmailForAccountProvisioning(this.CurrentRequest, sapUserValue, usr, ApplicationType.FLINS_BI, isForSAPSSO: isSSoEnabled);

                    if (this.CurrentRequest.AdHocAccessRequestId.HasValue)
                    {
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreated, sapUserValue.UserLogonName, endpoint.DisplayName), null);
                    }
                    else
                    {
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreated, sapUserValue.UserLogonName, endpoint.DisplayName), null);
                    }

                }
                else
                {
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreationFailed, sapUserValue.UserLogonName, endpoint.DisplayName), null);
                }

            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Exception, ex.Message, ex);
            }

            return isExecutedSuccessfully;
        }
        private bool CreateSRPUser(DomainModel.User usr, SRMRoleData srmDetail, AdHocSAPSRMRequest sapDetail)
        {
            bool isExecutedSuccessfully = false;
            SAPUserinfo sapUserValue = null;

            var endpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.SAP_SRP);
            var isSSoEnabled = EndpointConfiguration.IsSSOEnableForEndpoint(endpoint.Id) && this.Accounttype == AccountType.User;

            var fields = NProvisioningField.GetProvisioningDefaults(endpoint.Id, this.Accounttype);
            ConnectorBase sapConnector = ConnectorFactory.GetConnector(endpoint, this.CurrentRequest,_idmsLogger);

            if (!string.IsNullOrWhiteSpace(fields["Roles"]?.Value))
                fields["Roles"].Value = string.Format("{0},{1},{2}", fields["Roles"].Value, srmDetail.SRPRoles, srmDetail.SRPEnablerRoles);

            try
            {

                sapUserValue = SAPUserinfo.GetLockedAccountByName(srmDetail.AccountName, usr.Id, endpoint.Id, this.Accounttype);

                if (sapUserValue != null)
                {
                    sapUserValue.IsActive = true;
                    if (fields["Groups"] != null && !string.IsNullOrWhiteSpace(fields["Groups"].Value))
                    {
                        foreach (var item in fields["Groups"].Value.Split(','))
                        {
                            if (!string.IsNullOrWhiteSpace(item) && !sapUserValue.Groups.Contains(item))
                                sapUserValue.Groups.Add(item);
                        }
                    }
                    if (sapDetail != null && sapDetail.AdHocSAPSRMSharedAccount != null)
                    {
                        sapUserValue.UserType = sapDetail.AdHocSAPSRMSharedAccount.SAPUserType;
                        sapUserValue.SapUserType = GetSAPUserType(sapUserValue.UserType);
                        sapUserValue.Firstname = sapDetail.AdHocSAPSRMSharedAccount.Description;
                    }

                    isExecutedSuccessfully = this.UnlockUser(sapUserValue, usr, this.CurrentRequest, endpoint);
                    if (isExecutedSuccessfully)
                    {
                        sapUserValue.Password = this.constructUserPassword(fields, endpoint.Id);

                        if (this.CurrentRequest.AccountType == AccountType.Service)
                        {
                            sapConnector.ChangePassword(sapUserValue.UserLogonName, sapUserValue.Password, usr.Id, true);
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(srmDetail.SUSRoles))
                            {
                                sapConnector.ResetPassword(sapUserValue.UserLogonName, sapUserValue.Password, string.Empty);
                            }
                            else
                            {
                                sapConnector.ChangePassword(sapUserValue.UserLogonName, sapUserValue.Password, usr.Id, true);
                            }
                        }
                        //Below condition is required to deactivate the password. Please do not confuse with SRP and SUS role condition check here.
                        if (!string.IsNullOrWhiteSpace(srmDetail.SUSRoles))
                        {
                            ProvisioningEmailHelper.SendEmailForAccountProvisioning(this.CurrentRequest, sapUserValue, usr, ApplicationType.SAP_SRP,isForSAPSSO: isSSoEnabled);
                        }
                        else
                        {
                           
                            if (this.CurrentRequest.AccountType != AccountType.Service)
                            {
                                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, $"DeactivatePassword for account:{sapUserValue.UserLogonName}");
                                bool isSuccess = sapConnector.DeactivatePassword(sapUserValue.UserLogonName);
                                if (!isSuccess)
                                {
                                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Error, string.Format("Error while deactivating password for the account - {0}", sapUserValue.UserLogonName), null);
                                }
                            }
                        }

                        //SAP Roles
                        List<string> sapRoles = new List<string>();
                        string[] rolesAry = !string.IsNullOrWhiteSpace(fields["Roles"].Value) ? fields["Roles"].Value.Split(',') : null;
                        if (rolesAry != null)
                        {
                            foreach (string tRole in rolesAry)
                            {
                                if (!string.IsNullOrWhiteSpace(tRole))
                                {
                                    sapRoles.Add(tRole.Trim());
                                }
                            }
                        }
                        sapUserValue.Roles = sapRoles;

                        //Valid From and Valid To
                        if (this.CurrentRequest.ActivationDate != null && this.CurrentRequest.ActivationDate != DateTime.MinValue)
                        {
                            sapUserValue.UserValidFrom = this.getSAPDateFormat((DateTime)this.CurrentRequest.ActivationDate);
                        }
                        else
                        {
                            sapUserValue.UserValidFrom = this.getSAPDateFormat(DateTime.Now);
                        }


                        sapUserValue.UserValidTo = this.getSAPDateFormat((DateTime)GetValidTo(this.CurrentRequest.UserId));

                    }
                }
                else
                {  
                    sapUserValue = new SAPUserinfo();

                    #region Set SAP User object Values
                    sapUserValue.IsActive = true;
                    sapUserValue.EndPointSystemId = endpoint.Id;
                    sapUserValue.UserLogonName = sapDetail.AccountName;
                    sapUserValue.Firstname = sapDetail.AccountName;
                    var entitlement = ((ISAPUserType)sapDetail.AdHocSAPSRMSharedAccount);
                    if (entitlement != null)
                    {
                        sapUserValue.Firstname = entitlement.Description;
                    }
                    sapUserValue.Lastname = sapUserValue.UserLogonName;
                    if (string.IsNullOrWhiteSpace(sapUserValue.UserLogonName))
                    {
                        return false;
                    }
                    sapUserValue.Password = this.constructUserPassword(fields, endpoint.Id);
                    sapUserValue.UserType = sapDetail.AdHocSAPSRMSharedAccount.SAPUserType;
                    sapUserValue.SapUserType = GetSAPUserType(sapUserValue.UserType);

                    //SAP Roles
                    List<string> sapRoles = new List<string>();
                    string[] rolesAry = (fields["Roles"] != null && !string.IsNullOrWhiteSpace(fields["Roles"].Value)) ? fields["Roles"].Value.Split(',') : null;
                    if (rolesAry != null)
                    {
                        foreach (string tRole in rolesAry)
                        {
                            if (tRole.Trim() != string.Empty)
                                sapRoles.Add(tRole.Trim());
                        }
                    }
                    sapUserValue.Roles = sapRoles.Distinct().ToList();

                    //SAP Params
                    Dictionary<string, string> paramsInfoDictionary = new Dictionary<string, string>();
                    if (fields["Parameters"] != null && !string.IsNullOrWhiteSpace(fields["Parameters"].Value))
                    {
                        string[] sapParamList = Regex.Split(fields["Parameters"].Value, "##PP##");
                        string[] tmpParamInfo;
                        if (sapParamList != null)
                        {
                            foreach (string param in sapParamList)
                            {
                                tmpParamInfo = param.Split(',');
                                if (tmpParamInfo.Length == 2)
                                {
                                    paramsInfoDictionary.Add(tmpParamInfo[0], tmpParamInfo[1]);
                                }
                            }

                            //paramsInfoDictionary.Add("CYMP_PERSON", "");
                        }
                    }
                    sapUserValue.Parameters = paramsInfoDictionary;

                    //Valid From and Valid To

                    if (this.CurrentRequest.ActivationDate != null && this.CurrentRequest.ActivationDate != DateTime.MinValue)
                    {
                        sapUserValue.UserValidFrom = this.getSAPDateFormat((DateTime)this.CurrentRequest.ActivationDate);
                    }
                    else
                    {
                        sapUserValue.UserValidFrom = this.getSAPDateFormat(DateTime.Now);
                    }

                    if (this.CurrentRequest.DeactivationDate != null && this.CurrentRequest.DeactivationDate != DateTime.MinValue)
                    {
                        sapUserValue.UserValidTo = this.getSAPDateFormat((DateTime)this.CurrentRequest.DeactivationDate);
                    }
                    else
                    {
                        sapUserValue.UserValidTo = sapUserDefaultValidTo;
                    }
                    if (fields["Groups"] != null && !string.IsNullOrWhiteSpace(fields["Groups"].Value))
                    {
                        foreach (var item in fields["Groups"].Value.Split(','))
                        {
                            if (!string.IsNullOrWhiteSpace(item) && !sapUserValue.Groups.Contains(item))
                                sapUserValue.Groups.Add(item);
                        }
                    }
                    #endregion

                    isExecutedSuccessfully = sapConnector.CreateAccount(sapUserValue, fields);
                }
                if (isExecutedSuccessfully)
                {
                    this.addNewUserintoSAPUserInfo(sapUserValue);
                    CreateUserInReconTable(sapUserValue, "System");
                    // SAPUserinfo.AddEntitlement(sapUserValue.Id, EntitlementType.Role, fields["Roles"].Value);
                    if (!string.IsNullOrWhiteSpace(srmDetail.SUSRoles))
                    {
                        ProvisioningEmailHelper.SendEmailForAccountProvisioning(this.CurrentRequest, sapUserValue, usr, ApplicationType.SAP_SRP,isForSAPSSO: isSSoEnabled);
                    }
                    else
                    {
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, $"DeactivatePassword for account:{sapUserValue.UserLogonName}");
                        bool isSuccess = sapConnector.DeactivatePassword(sapUserValue.UserLogonName);
                        if (!isSuccess)
                        {
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Error, string.Format("Error while deactivating password for the account - {0}", sapUserValue.UserLogonName), null);
                        }
                    }

                    if (this.CurrentRequest.AdHocAccessRequestId.HasValue)
                    {
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreated, sapUserValue.UserLogonName, endpoint.DisplayName), null);
                    }
                    else
                    {
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreated, sapUserValue.UserLogonName, endpoint.DisplayName), null);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Exception, ex.Message, ex);
            }

            return isExecutedSuccessfully;
        }
        private bool CreateSUSUser(DomainModel.User usr, SRMRoleData srmDetail, AdHocSAPSRMRequest sapDetail)
        {
            bool isExecutedSuccessfully = false;
            SAPUserinfo sapUserValue = null;

            var endpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.SAP_SUS);
            var isSSoEnabled = EndpointConfiguration.IsSSOEnableForEndpoint(endpoint.Id) && this.Accounttype == AccountType.User;

            var fields = NProvisioningField.GetProvisioningDefaults(endpoint.Id, this.Accounttype);

            ConnectorBase sapConnector = ConnectorFactory.GetConnector(endpoint, this.CurrentRequest);

            if (!string.IsNullOrWhiteSpace(fields["Roles"].Value))
                fields["Roles"].Value = string.Format("{0},{1}", fields["Roles"].Value, srmDetail.SUSRoles);

            try
            {
                sapUserValue = SAPUserinfo.GetLockedAccountByName(srmDetail.AccountName, usr.Id, endpoint.Id, this.Accounttype);

                if (sapUserValue != null && sapConnector.IsUserExists(sapUserValue.AccountName))
                {

                    sapUserValue.IsUserLocked = false;
                    sapUserValue.IsActive = true;
                    if (sapDetail != null && sapDetail.AdHocSAPSRMSharedAccount != null)
                    {
                        sapUserValue.UserType = sapDetail.AdHocSAPSRMSharedAccount.SAPUserType;
                        sapUserValue.SapUserType = GetSAPUserType(sapUserValue.UserType);
                        sapUserValue.Firstname = sapDetail.AdHocSAPSRMSharedAccount.Description;
                    }
                    else
                    {
                        var srp = DomainModel.EndPoint.GetByApplicationType(ApplicationType.SAP_SRP);
                        var srpUsr = SAPUserinfo.GetActiveByUserLogonName(sapUserValue.AccountName, srp.Id);
                        sapUserValue.UserType = GetUserType(srpUsr.SapUserType);
                        sapUserValue.SapUserType = srpUsr.SapUserType;
                    }
                    if (fields["Groups"] != null && !string.IsNullOrWhiteSpace(fields["Groups"].Value))
                    {
                        foreach (var item in fields["Groups"].Value.Split(','))
                        {
                            if (!string.IsNullOrWhiteSpace(item) && !sapUserValue.Groups.Contains(item))
                                sapUserValue.Groups.Add(item);
                        }
                    }
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, "CreateSUSUser UnlockUser started");
                    isExecutedSuccessfully = this.UnlockUser(sapUserValue, usr, this.CurrentRequest, endpoint);
                    if (isExecutedSuccessfully)
                    {
                        //this.UnLockDependentSystemsInIDMS(sapUserValue, false);

                        sapUserValue.Password = this.constructUserPassword(fields, endpoint.Id);

                        if (this.CurrentRequest.AccountType == AccountType.Service)
                        {
                            sapConnector.ChangePassword(sapUserValue.UserLogonName, sapUserValue.Password, usr.Id, true);
                        }
                        else
                        {
                            sapConnector.ResetPassword(sapUserValue.UserLogonName, sapUserValue.Password, string.Empty);
                        }

                    }
                }
                else
                {


                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, "CreateSUSUser started");
                    sapUserValue = new SAPUserinfo();

                    #region Set SAP User object Values
                    sapUserValue.IsActive = true;
                    sapUserValue.EndPointSystemId = endpoint.Id;
                    sapUserValue.UserLogonName = sapDetail.AccountName;
                    sapUserValue.Firstname = sapDetail.AccountName;

                    var entitlement = ((ISAPUserType)sapDetail.AdHocSAPSRMSharedAccount);
                    if (entitlement != null)
                    {
                        sapUserValue.Firstname = entitlement.Description;
                    }

                    sapUserValue.Lastname = sapUserValue.UserLogonName;
                    if (string.IsNullOrWhiteSpace(sapUserValue.UserLogonName))
                    {
                        return false;
                    }
                    sapUserValue.Password = this.constructUserPassword(fields, endpoint.Id);
                    if (sapDetail != null && sapDetail.AdHocSAPSRMSharedAccount != null)
                    {
                        sapUserValue.UserType = sapDetail.AdHocSAPSRMSharedAccount.SAPUserType;
                        sapUserValue.SapUserType = GetSAPUserType(sapUserValue.UserType);
                        sapUserValue.Firstname = sapDetail.AdHocSAPSRMSharedAccount.Description;
                    }
                    else
                    {
                        var srp = DomainModel.EndPoint.GetByApplicationType(ApplicationType.SAP_SRP);
                        var srpUsr = SAPUserinfo.GetActiveByUserLogonName(sapUserValue.AccountName, srp.Id);
                        sapUserValue.UserType = GetUserType(srpUsr.SapUserType);
                        sapUserValue.SapUserType = srpUsr.SapUserType;
                    }

                    //SAP Params
                    Dictionary<string, string> paramsInfoDictionary = new Dictionary<string, string>();
                    if (!string.IsNullOrWhiteSpace(fields["Parameters"].Value))
                    {
                        string[] sapParamList = Regex.Split(fields["Parameters"].Value, "##PP##");
                        string[] tmpParamInfo;
                        if (sapParamList != null)
                        {
                            foreach (string param in sapParamList)
                            {
                                tmpParamInfo = param.Split(',');
                                if (tmpParamInfo.Length == 2)
                                {
                                    paramsInfoDictionary.Add(tmpParamInfo[0], tmpParamInfo[1]);
                                }
                            }

                            //paramsInfoDictionary.Add("CYMP_PERSON", "");
                        }
                    }
                    sapUserValue.Parameters = paramsInfoDictionary;

                    //Valid From and Valid To

                    if (this.CurrentRequest.ActivationDate != null && this.CurrentRequest.ActivationDate != DateTime.MinValue)
                    {
                        sapUserValue.UserValidFrom = this.getSAPDateFormat((DateTime)this.CurrentRequest.ActivationDate);
                    }
                    else
                    {
                        sapUserValue.UserValidFrom = this.getSAPDateFormat(DateTime.Now);
                    }

                    if (this.CurrentRequest.DeactivationDate != null && this.CurrentRequest.DeactivationDate != DateTime.MinValue)
                    {
                        sapUserValue.UserValidTo = this.getSAPDateFormat((DateTime)this.CurrentRequest.DeactivationDate);
                    }
                    else
                    {
                        sapUserValue.UserValidTo = sapUserDefaultValidTo;
                    }
                    if (fields["Groups"] != null && !string.IsNullOrWhiteSpace(fields["Groups"].Value))
                    {
                        foreach (var item in fields["Groups"].Value.Split(','))
                        {
                            if (!string.IsNullOrWhiteSpace(item) && !sapUserValue.Groups.Contains(item))
                                sapUserValue.Groups.Add(item);
                        }
                    }
                    #endregion

                    isExecutedSuccessfully = sapConnector.CreateAccount(sapUserValue, fields);
                }
                if (isExecutedSuccessfully)
                {
                    ProvisioningEmailHelper.SendEmailForAccountProvisioning(this.CurrentRequest, sapUserValue, usr, ApplicationType.SAP_SUS,isForSAPSSO: isSSoEnabled);
                    this.addNewUserintoSAPUserInfo(sapUserValue);
                    CreateUserInReconTable(sapUserValue, "System");
                    //  SAPUserinfo.AddEntitlement(sapUserValue.Id, EntitlementType.Role, fields["Roles"].Value);

                    //this.CreateDependentSystemEntriesInIDMS(sapUserValue, false);

                    if (this.CurrentRequest.AdHocAccessRequestId.HasValue)
                    {
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreated, sapUserValue.UserLogonName, endpoint.DisplayName), null);
                    }
                    else
                    {
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreated, sapUserValue.UserLogonName, endpoint.DisplayName), null);
                    }

                }
                else
                {
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreationFailed, sapUserValue.UserLogonName, endpoint.DisplayName), null);
                }

            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Exception, ex.Message, ex);
            }

            return isExecutedSuccessfully;
        }
        private bool LockUser(SAPUserinfo sapUsr, LockScenario lockScenario, EndPoint endpoint)
        {
            bool isusrDisabled = true;

            try
            {
                if (sapUsr != null)
                {
                    isusrDisabled = false;

                    ConnectorBase sapconnector = ConnectorFactory.GetConnector(endpoint, this.CurrentRequest);

                    sapUsr.UserValidTo = this.getSAPDateFormat(DateTime.Today);// lockScenario != LockScenario.AdhocRequest ? this.getSAPDateFormat(DateTime.Today.AddDays(-1)) : this.getSAPDateFormat(DateTime.Today);
                    sapconnector.UpdateAccount(sapUsr);
                    sapconnector.UpdateSSOAttributes(sapUsr, null, this.Accounttype);
                    isusrDisabled = sapconnector.LockAccount(sapUsr);
                    if (isusrDisabled)
                    {
                        sapUsr.IsActive = false;
                        sapUsr.IsUserLocked = true;
                        sapUsr.LastModifiedBy = "System";
                        sapUsr.LastModifiedDate = DateTime.Now;
                        sapUsr.Save("System");
                        if (isusrDisabled)
                        {
                            sapUsr.Roles = SAPUserinfo.GetByIdIncludeSAPRoleInfo(sapUsr.Id).SAPRoleInfo.Select(item => item.ROLE_CODE).ToList();
                            sapconnector.RemoveAllEntitlements(sapUsr);
                            var rolstring = getGroupListAsString(sapUsr.Roles);
                            SAPUserinfo.RemoveEntitlement(sapUsr.Id, EntitlementType.Role, rolstring);
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, string.Format("User {0} Disabled in SAP.", sapUsr.UserLogonName), null);

                        }
                    }
                }

                return isusrDisabled;
            }
            catch
            {
                throw;
            }
        }
        private bool UnlockUser(SAPUserinfo sapUsr, DomainModel.User user, AccessRequest accessRequest, EndPoint endpoint)
        {
            bool isusrDisabled = true;

            try
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, string.Format("User {0} Unlocked in SAP started.", sapUsr.UserLogonName), null);
                if (sapUsr != null)
                {
                    isusrDisabled = false;

                    ConnectorBase sapconnector = ConnectorFactory.GetConnector(endpoint, this.CurrentRequest);

                    //Valid From and Valid To

                    if (accessRequest.ActivationDate != null && accessRequest.ActivationDate != DateTime.MinValue)
                    {
                        sapUsr.UserValidFrom = this.getSAPDateFormat((DateTime)accessRequest.ActivationDate);
                    }
                    else
                    {
                        sapUsr.UserValidFrom = this.getSAPDateFormat(DateTime.Now);
                    }

                    sapUsr.UserValidTo = this.getSAPDateFormat((DateTime)GetValidTo(this.CurrentRequest.UserId));

                    if (!string.IsNullOrWhiteSpace(sapUsr.UserValidFrom) && !string.IsNullOrWhiteSpace(sapUsr.UserValidTo))
                    {
                        sapconnector.UpdateAccount(sapUsr);
                    }

                    isusrDisabled = sapconnector.UnLockAccount(sapUsr);
                    if (isusrDisabled)
                    {
                        sapUsr.IsUserLocked = false;
                        sapUsr.LastModifiedBy = "System";
                        sapUsr.LastModifiedDate = DateTime.Now;

                        sapUsr.Save("System");
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, endpoint.Id, LogLevelType.Info, string.Format("User {0} Unlocked in SAP.", sapUsr.UserLogonName), null);
                    }
                }

                return isusrDisabled;
            }
            catch
            {
                throw;
            }
        }
        private void addNewUserintoSAPUserInfo(SAPUserinfo sapUser)
        {
            sapUser.CreatedBy = ProvisioningConstants.ProcessName;
            sapUser.LastModifiedBy = ProvisioningConstants.ProcessName;
            sapUser.CreatedDate = DateTime.Now;
            sapUser.LastModifiedDate = DateTime.Now;
            string emptyStr = string.Empty;

            foreach (PropertyInfo pInfo in sapUser.GetType().GetProperties())
            {
                if (pInfo.GetValue(sapUser, null) == null && pInfo.PropertyType == typeof(string))
                {
                    pInfo.SetValue(sapUser, emptyStr, null);
                }
            }

            sapUser.validToDate = DateTime.MaxValue;
            sapUser.validFromDate = DateTime.Parse(sapUser.UserValidFrom);
            sapUser.PASSWORDEXPTIME = sapUser.PASSWORDEXPTIME == DateTime.MinValue ? null : sapUser.PASSWORDEXPTIME; 
            sapUser.SapUserType = GetSAPUserType(sapUser.UserType);

            sapUser.Save("System");
        }
        private bool RemoveSRMRoles(AdHocSAPSRMRequest sapDetail, SRMRoleData srmDetail, long userId, string roleListStr)
        {
            try
            {
                SAPUserinfo sapUserValue = null;


                List<SAPSRMBusinessRoleMapping> existingBussRoles = new List<SAPSRMBusinessRoleMapping>();
                existingBussRoles = SAPSRMBusinessRoleMapping.GetMappingByUserName(srmDetail.AccountName);
                List<string> userBussRoles = existingBussRoles.Select(item => item.BusinessRole).Distinct().ToList();

                List<string> bussRolesToRetain = new List<string>();

                //populate the list with business role, porg, pgrp to retain
                List<SRMRolePOrgData> existingbussRolesAndPGrpPOrg = new List<SRMRolePOrgData>();
                List<SRMRolePOrgData> bussRolesAndPGrpPOrgToRemove = new List<SRMRolePOrgData>();
                List<SRMRolePOrgData> bussRolesAndPGrpPOrgToRetain = new List<SRMRolePOrgData>();
                SRMRolePOrgData data = new SRMRolePOrgData();

                foreach (var detail in sapDetail.AdHocSAPSRMRequestDetails)
                {
                    string enablerRule = string.Empty;
                    string businessRole = detail.Entitlement;
                    var srmEnablerRule = SAPSRMMainRoleMatrix.GetSRMMatrixByRequest(detail);

                    if (srmEnablerRule != null)
                        enablerRule = srmEnablerRule.SRMEnablerRule;

                    if (detail.RetainMainRole)
                    {
                        data = new SRMRolePOrgData();
                        data.BusinessRole = businessRole;
                        data.EnablerRule = enablerRule;
                        data.POrg = detail.PurchaseOrg;
                        data.PGrp = detail.PurchaseGroup;
                        bussRolesAndPGrpPOrgToRemove.Add(data);
                    }
                    else
                    {
                        List<SAPSRMBusinessRoleMapping> existingPorgs = SAPSRMBusinessRoleMapping.GetMappingByUserNameAndRole(sapDetail.AccountName, detail.Entitlement);

                        foreach (SAPSRMBusinessRoleMapping mapping in existingPorgs)
                        {
                            data = new SRMRolePOrgData();
                            data.BusinessRole = businessRole;
                            data.EnablerRule = enablerRule;
                            data.POrg = mapping.POrg;
                            data.PGrp = mapping.PGrp;
                            bussRolesAndPGrpPOrgToRemove.Add(data);
                        }
                    }
                }


                foreach (var detail in existingBussRoles)
                {
                    data = new SRMRolePOrgData();
                    data.BusinessRole = detail.BusinessRole;
                    AdHocSAPSRMRequestDetail req = new AdHocSAPSRMRequestDetail();
                    req.Entitlement = data.BusinessRole;
                    var SRMEnablerRule = SAPSRMMainRoleMatrix.GetSRMMatrixByRequest(req);
                    if (SRMEnablerRule != null)
                        data.EnablerRule = SRMEnablerRule.SRMEnablerRule;
                    data.POrg = detail.POrg;
                    data.PGrp = detail.PGrp;
                    existingbussRolesAndPGrpPOrg.Add(data);
                }

                foreach (var detail in existingbussRolesAndPGrpPOrg)
                {
                    if (!(bussRolesAndPGrpPOrgToRemove.Any(item => item.BusinessRole == detail.BusinessRole)))
                    {
                        data = new SRMRolePOrgData();
                        data.BusinessRole = detail.BusinessRole;
                        data.EnablerRule = detail.EnablerRule;
                        data.POrg = detail.POrg;
                        data.PGrp = detail.PGrp;
                        bussRolesAndPGrpPOrgToRetain.Add(data);
                    }
                    else
                    {
                        data = new SRMRolePOrgData();
                        var temp = bussRolesAndPGrpPOrgToRemove.FirstOrDefault(item => item.BusinessRole == detail.BusinessRole && item.EnablerRule == detail.EnablerRule && item.POrg == detail.POrg);
                        if (temp != null)
                        {
                            List<string> existingPgrps = new List<string>();
                            if (!string.IsNullOrWhiteSpace(detail.PGrp))
                            {
                                existingPgrps = detail.PGrp.Split(',').ToList();
                            }
                            List<string> pgrpsToremove = new List<string>();
                            if (!string.IsNullOrWhiteSpace(temp.PGrp))
                            {
                                pgrpsToremove = temp.PGrp.Split(',').ToList();
                            }
                            pgrpsToremove.ForEach(x =>
                            {
                                if (existingPgrps.Contains(x))
                                {
                                    existingPgrps.Remove(x);
                                }
                            });
                            data.BusinessRole = detail.BusinessRole;
                            data.EnablerRule = detail.EnablerRule;
                            data.PGrp = string.Join(",", existingPgrps);

                            if (!string.IsNullOrWhiteSpace(data.PGrp))
                            {
                                data.POrg = detail.POrg;
                            }
                            else
                            {
                                data.POrg = string.Empty;
                            }

                            bussRolesAndPGrpPOrgToRetain.Add(data);
                        }
                    }
                }


                foreach (string userBussRole in userBussRoles)
                {
                    if (sapDetail.AdHocSAPSRMRequestDetails.FirstOrDefault(item => item.Entitlement == userBussRole) == null)
                    {
                        if (!bussRolesToRetain.Contains(userBussRole))
                        {
                            bussRolesToRetain.Add(userBussRole);
                        }
                    }
                    else
                    {
                        if (this.Accounttype == AccountType.User || this.Accounttype == AccountType.Secondary)
                        {
                            if (sapDetail.AdHocSAPSRMRequestDetails.Any(item => item.Entitlement == userBussRole && item.RetainMainRole))
                            {
                                if (!bussRolesToRetain.Contains(userBussRole))
                                {
                                    bussRolesToRetain.Add(userBussRole);
                                }
                            }
                        }
                        else
                        {
                            if (sapDetail.AdHocSAPSRMRequestDetails.FirstOrDefault(item => item.Entitlement == userBussRole && !item.RetainMainRole) == null)
                            {
                                if (!bussRolesToRetain.Contains(userBussRole))
                                {
                                    bussRolesToRetain.Add(userBussRole);
                                }
                            }
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(srmDetail.ADGroups))
                {
                    var endpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.ADSingaporePowerLocal);
                    ADSUserInfo adUserValue = null;
                    if (this.Accounttype == AccountType.User || this.Accounttype == AccountType.Secondary)
                    {
                        adUserValue = ADSUserInfo.GetPrimaryByDIUserIdAccountType(userId, srmDetail.AccountName, endpoint.Id, this.Accounttype);
                    }
                    else
                    {
                        adUserValue = ADSUserInfo.GetBysAMAccountNameEndPointSystemId(srmDetail.AccountName, endpoint.Id);
                    }

                    if (adUserValue != null)
                    {

                        if (!adUserValue.sAMAccountName.Equals(srmDetail.AccountName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Error, string.Format("AD account name \"{0}\" and HRMS FLINS account name \"{1}\" are not same. Please correct the AD account name manually and re-submit the request.", adUserValue.sAMAccountName, srmDetail.AccountName), null);
                            return false;
                        }

                        List<string> groupsToRemove = srmDetail.ADGroups.Split(',').ToList();
                        List<string> adGroupsToRetain = new List<string>();

                        foreach (string bussRoleToRetain in bussRolesToRetain)
                        {
                            var srmMatrix = SAPSRMMainRoleMatrix.GetSRMMatrixByRequest(bussRoleToRetain);

                            if (srmMatrix != null)
                            {
                                adGroupsToRetain.AddRange(srmMatrix.ADGroups.Split(',').ToList());
                            }
                        }

                        List<string> adGrpsToRemove = new List<string>();

                        foreach (string groupToRemove in groupsToRemove)
                        {
                            if (!adGroupsToRetain.Contains(groupToRemove))
                            {
                                if (!adGrpsToRemove.Contains(groupToRemove))
                                {
                                    adGrpsToRemove.Add(groupToRemove);
                                }
                            }
                        }

                        bool isSuccess = this.RemoveADGroups(adUserValue, endpoint, string.Join(",", adGrpsToRemove));
                        if (!isSuccess)
                        {
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, string.Format("Error while removing SRM AD Groups \"{1}\" for User \"{0}\".", srmDetail.AccountName, string.Join(",", adGrpsToRemove)), null);
                            //return false;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(srmDetail.FLINSBIRoles) || !string.IsNullOrWhiteSpace(srmDetail.FLINSBIPOrgRoles) ||
                    !string.IsNullOrWhiteSpace(srmDetail.FLINSBIPGrpRoles))
                {
                    var endpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.FLINS_BI);
                    sapUserValue = SAPUserinfo.GetByUserLogonName(srmDetail.AccountName, endpoint.Id);
                    if (sapUserValue != null && endpoint.IsActive)
                    {

                        List<string> groupsToRemove = srmDetail.FLINSBIRoles.Split(',').ToList();
                        List<string> adGroupsToRetain = new List<string>();

                        foreach (string bussRoleToRetain in bussRolesToRetain)
                        {
                            var srmMatrix = SAPSRMMainRoleMatrix.GetSRMMatrixByRequest(bussRoleToRetain);

                            if (srmMatrix != null)
                            {
                                adGroupsToRetain.AddRange(srmMatrix.BIRoles.Split(',').ToList());
                            }
                        }

                        List<string> adGrpsToRemove = new List<string>();

                        foreach (string groupToRemove in groupsToRemove)
                        {
                            if (!adGroupsToRetain.Contains(groupToRemove))
                            {
                                if (!adGrpsToRemove.Contains(groupToRemove))
                                {
                                    adGrpsToRemove.Add(groupToRemove);
                                }
                            }
                        }

                        //use the list bussRolesAndPGrpPOrgToRetain to retain shared bi porg and pgrp roles before removing.

                        List<string> porgRolesToRemove = srmDetail.FLINSBIPOrgRoles.Split(',').ToList();
                        List<string> pgrpRolesToRemove = srmDetail.FLINSBIPGrpRoles.Split(',').ToList();
                        List<string> porgRolesToRetain = new List<string>();
                        List<string> pgrpRolesToRetain = new List<string>();

                        foreach (SRMRolePOrgData bussRoleToRetain in bussRolesAndPGrpPOrgToRetain)
                        {
                            if (!string.IsNullOrWhiteSpace(bussRoleToRetain.POrg))
                            {
                                var pGrps = bussRoleToRetain.PGrp.Trim().Split(',').ToList();
                                foreach (string pGrp in pGrps)
                                {
                                    var enablerRules = bussRoleToRetain.EnablerRule.Split(',').ToList();
                                    foreach (string enablerRule in enablerRules)
                                    {
                                        var srmEnablerMatrix = SAPSRMEnablerRoleMatrix.GetSRMEnablerMatrixByRequest(enablerRule, bussRoleToRetain.POrg, pGrp);

                                        if (srmEnablerMatrix != null)
                                        {
                                            if (!string.IsNullOrWhiteSpace(srmEnablerMatrix.BIPOrgRole))
                                            {
                                                var groups = srmEnablerMatrix.BIPOrgRole.Trim().Split(',').ToList();
                                                foreach (string group in groups)
                                                {
                                                    if (!porgRolesToRetain.Contains(group))
                                                    {
                                                        porgRolesToRetain.Add(group);
                                                    }
                                                }
                                            }

                                            if (!string.IsNullOrWhiteSpace(srmEnablerMatrix.BIPGrpRole))
                                            {
                                                var groups = srmEnablerMatrix.BIPGrpRole.Trim().Split(',').ToList();
                                                foreach (string group in groups)
                                                {
                                                    if (!pgrpRolesToRetain.Contains(group))
                                                    {
                                                        pgrpRolesToRetain.Add(group);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        List<string> finalPorgRolesToRemove = new List<string>();
                        List<string> finalPgrpRolesToRemove = new List<string>();

                        foreach (string roleToRemove in porgRolesToRemove)
                        {
                            if (!porgRolesToRetain.Contains(roleToRemove))
                            {
                                if (!finalPorgRolesToRemove.Contains(roleToRemove))
                                {
                                    finalPorgRolesToRemove.Add(roleToRemove);
                                }
                            }
                        }

                        foreach (string roleToRemove in pgrpRolesToRemove)
                        {
                            if (!pgrpRolesToRetain.Contains(roleToRemove))
                            {
                                if (!finalPgrpRolesToRemove.Contains(roleToRemove))
                                {
                                    finalPgrpRolesToRemove.Add(roleToRemove);
                                }
                            }
                        }

                        string biRoles = string.Format("{0},{1},{2}", string.Join(",", adGrpsToRemove), string.Join(",", finalPgrpRolesToRemove), string.Join(",", finalPorgRolesToRemove));
                        bool isSuccess = this.RemoveSAPRoles(sapUserValue, endpoint, biRoles);
                        if (!isSuccess)
                        {
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, string.Format("Error while removing SRM AD Groups \"{1}\" for User \"{0}\".", srmDetail.AccountName, string.Join(",", adGrpsToRemove)), null);
                            return false;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(srmDetail.SRPRoles) || !string.IsNullOrWhiteSpace(srmDetail.SRPEnablerRoles))
                {
                    var endpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.SAP_SRP);

                    sapUserValue = SAPUserinfo.GetByUserLogonName(srmDetail.AccountName, endpoint.Id);

                    if (sapUserValue != null && endpoint.IsActive)
                    {
                        string biRoles = string.Format("{0},{1}", srmDetail.SRPRoles, srmDetail.SRPEnablerRoles);
                        bool isSuccess = this.RemoveSAPRoles(sapUserValue, endpoint, biRoles);
                        if (!isSuccess)
                        {
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, string.Format("Error while removing SRM Roles \"{1}\" for User \"{0}\".", srmDetail.AccountName, roleListStr), null);
                            return false;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(srmDetail.SUSRoles))
                {
                    var endpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.SAP_SUS);

                    sapUserValue = SAPUserinfo.GetByUserLogonName(srmDetail.AccountName, endpoint.Id);

                    if (sapUserValue != null && endpoint.IsActive)
                    {
                        string biRoles = srmDetail.SUSRoles;
                        bool isSuccess = this.RemoveSAPRoles(sapUserValue, endpoint, biRoles);

                        if (!isSuccess)
                        {
                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, string.Format("Error while removing SRM Roles \"{1}\" for User \"{0}\".", srmDetail.AccountName, roleListStr), null);
                            return false;
                        }
                    }
                }

                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, string.Format("User \"{0}\" is Removed from SRM Roles \"{1}\" successfully.", srmDetail.AccountName, roleListStr), null);

                return true;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Exception, "Error while removing SRM roles.", ex);
                return false;
            }
        }
        private bool LockSRMUser(SRMRoleData srmDetail, LockScenario lockScenario, long userId)
        {
            try
            {
                var srpEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.SAP_SRP);
                var srpUser = SAPUserinfo.GetByUserLogonName(srmDetail.AccountName, srpEndpoint.Id);

                if (srpUser != null)
                {
                    bool isSuccess = this.LockUser(srpUser, lockScenario, srpEndpoint);
                    if (!isSuccess)
                    {
                        WriteProvisioningLog(LogLevelType.Info, $"SRM SAP SRP Lock account failed for the User {srmDetail.AccountName}.");
                        return false;
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Info, $"SRM SAP SRP account is locked for the User {srmDetail.AccountName}.");
                    }
                }

                var susEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.SAP_SUS);
                var susUser = SAPUserinfo.GetByUserLogonName(srmDetail.AccountName, susEndpoint.Id);

                if (susUser != null && susEndpoint.IsActive)
                {
                    bool isSuccess = this.LockUser(susUser, lockScenario, susEndpoint);
                    if (!isSuccess)
                    {
                        WriteProvisioningLog(LogLevelType.Info, $"SRM SAP SUS Lock account failed for the User {srmDetail.AccountName}.");
                        return false;
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Info, $"SRM SAP SUS account is locked for the User {srmDetail.AccountName}.");
                    }
                }

                var adEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.ADSingaporePowerLocal);
                ADSUserInfo adUserValue = null;
                if (this.Accounttype == AccountType.User || this.Accounttype == AccountType.Secondary)
                {
                    adUserValue = ADSUserInfo.GetPrimaryByDIUserIdAccountType(userId, srmDetail.AccountName, adEndpoint.Id, this.Accounttype);
                }
                else
                {
                    adUserValue = ADSUserInfo.GetBysAMAccountNameEndPointSystemId(srmDetail.AccountName, adEndpoint.Id);
                }

                if (adUserValue != null)
                {
                    List<string> groupsToRemove = new List<string>();
                    var aduser = ADSUserInfo.GetByIdIncludeADSGroups(adUserValue.Id);
                    var userADGroups = aduser != null ? aduser.ADSGroups.Select(item => item.name).ToList() : null;

                    var srmADGroups = SAPSRMMainRoleMatrix.GetAllSRMADGroups();

                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, this.Endpoint.Id, LogLevelType.Info, $"Started removing AD Groups {srmADGroups} for the user {srmDetail.AccountName}");
                    foreach (string srmADGroup in srmADGroups)
                    {
                        var list = srmADGroup.Split(',').ToList();

                        foreach (string grp in list)
                        {
                            if (userADGroups.Contains(grp))
                            {
                                if (!groupsToRemove.Contains(grp))
                                {
                                    groupsToRemove.Add(grp);
                                }
                            }
                        }
                    }

                    if (groupsToRemove.Count > 0)
                    {
                        string grpsToRemove = string.Join(",", groupsToRemove);
                        bool isSuccess = this.RemoveADGroups(adUserValue, adEndpoint, grpsToRemove);
                        if (!isSuccess)
                        {
                            return false;
                        }
                    }
                }


                var biEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.FLINS_BI);
                var biUser = SAPUserinfo.GetByUserLogonName(srmDetail.AccountName, biEndpoint.Id);

                if (biUser != null && biEndpoint.IsActive)
                {

                    List<string> rolesToRemove = new List<string>();
                    var userBIRoles = SAPUserinfo.GetByIdIncludeSAPRoleInfo(biUser.Id).SAPRoleInfo.Select(item => item.ROLE_CODE).ToList();
                    var srmBIRoles = SAPSRMMainRoleMatrix.GetAllSRMFLINSBIRolesIncludePOrgPGrp();

                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, this.Endpoint.Id, LogLevelType.Info, $"Started removing BI roles {srmBIRoles} for the user {srmDetail.AccountName}");
                    foreach (string srmBIRole in srmBIRoles)
                    {
                        var list = srmBIRole.Split(',').ToList();

                        foreach (string role in list)
                        {
                            if (userBIRoles.Contains(role))
                            {
                                if (!rolesToRemove.Contains(role))
                                {
                                    rolesToRemove.Add(role);
                                }
                            }
                        }
                    }

                    if (rolesToRemove.Count > 0)
                    {
                        string rlsToRemove = string.Join(",", rolesToRemove);
                        bool isSuccess = this.RemoveSAPRoles(biUser, biEndpoint, rlsToRemove);
                        if (isSuccess)
                        {
                            WriteProvisioningLog(LogLevelType.Info, $"Remove FLINS BI roles:{rlsToRemove} removed successfully for the User {srmDetail.AccountName}.");
                        }
                        else
                        {
                            WriteProvisioningLog(LogLevelType.Exception, $"Failed to remove FLINS BI roles for the User {srmDetail.AccountName}.");
                            return false;
                        }
                    }
                    userBIRoles = SAPUserinfo.GetByIdIncludeSAPRoleInfo(biUser.Id).SAPRoleInfo.Select(item => item.ROLE_CODE).ToList();
                    if (userBIRoles == null || userBIRoles.Count() == 0)
                    {
                        WriteProvisioningLog(LogLevelType.Info, $"Lock the FLINS BI account if no more roles exist for the User {srmDetail.AccountName}.");
                        // Lock the FLINS BI account if no more roles exist.
                        var isSuccess = this.LockUser(biUser, lockScenario, biEndpoint);
                        if (isSuccess)
                        {
                            WriteProvisioningLog(LogLevelType.Info, $"SRM FLINS BI account is locked for the User {srmDetail.AccountName}.");
                            return true;
                        }
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Info, $"Since SRM FLINS BI account has more roles, it is not locked for the User {srmDetail.AccountName}.");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, this.Endpoint.Id, LogLevelType.Exception, "Error while locking user in SRM.", ex);
                return false;
            }
        }
        private void UpdateSRMUserAndRoleMapping(List<SRMUserRoleMappingData> mappings)
        {
            try
            {
                foreach (SRMUserRoleMappingData item in mappings)
                {
                    var existingMapping = SAPSRMBusinessRoleMapping.GetMappingByUserNameAndRolepOrg(item.UserName, item.BusinessRole, item.POrg);

                    if (existingMapping == null)
                    {
                        var mapping = this.CreateUserRoleMappingObjectShell(item);
                        mapping.Save("System");
                    }
                    else
                    {
                        existingMapping.LastModifiedBy = "System";
                        existingMapping.LastModifiedDate = DateTime.Now;

                        existingMapping.Save("System");
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Exception, "Error while updating SRM user and business role mapping table.", ex);
            }
        }
        private SAPSRMBusinessRoleMapping CreateUserRoleMappingObjectShell(SRMUserRoleMappingData mappingData)
        {
            SAPSRMBusinessRoleMapping mapping = new SAPSRMBusinessRoleMapping();
            mapping.AccountType = this.Accounttype;
            mapping.BusinessRole = mappingData.BusinessRole;
            mapping.PGrp = mappingData.PGrp;
            mapping.POrg = mappingData.POrg;
            mapping.UserId = mappingData.UserId;
            mapping.UserName = mappingData.UserName;
            mapping.IsActive = true;
            mapping.CreatedBy = "System";
            mapping.CreatedDate = DateTime.Now;
            mapping.LastModifiedBy = "System";
            mapping.LastModifiedDate = DateTime.Now;

            return mapping;
        }
        private void UpdateSRMUserAndRoleMapping(DomainModel.User user, AdHocSAPSRMRequest sapDetail, AdHocActionType actionType)
        {
            try
            {
                if (actionType == AdHocActionType.DeleteAccount)
                {
                    SAPSRMBusinessRoleMapping.DeleteMappingByUserName(sapDetail.AccountName, this.Accounttype);
                }
                else if (actionType == AdHocActionType.AmendmentAdd || actionType == AdHocActionType.NewAccount)
                {
                    var accountRecon = AccountReconcilation.GetByAccountNameByAccountType(sapDetail.AccountName, Endpoint.Id, this.Accounttype);
                    var accountOwner = DomainModel.User.GetById(accountRecon.UserId.Value);
                    foreach (AdHocSAPSRMRequestDetail item in sapDetail.AdHocSAPSRMRequestDetails)
                    {
                        _idmsLogger.WriteLog(LogLevelType.DebugStep, $"UpdateSRMUserAndRoleMapping: Create User RoleMapping:{sapDetail.AccountName} Entitlement:{item?.Entitlement} POrg:{item?.PurchaseOrg}");
                        var existingMapping = SAPSRMBusinessRoleMapping.GetMappingByUserNameAndRolepOrg(sapDetail.AccountName, item.Entitlement, item.PurchaseOrg);

                        if (existingMapping == null)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"UpdateSRMUserAndRoleMapping: Create User RoleMapping:{sapDetail.AccountName}");
                            var mapping = this.CreateUserRoleMappingObjectShell(accountOwner, sapDetail, item);
                            mapping.Save("System");
                        }
                        else
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"UpdateSRMUserAndRoleMapping: {item.PurchaseOrg}");
                            if (!string.IsNullOrWhiteSpace(item.PurchaseOrg))
                            {
                                var existingPGrps = existingMapping.PGrp.Split(',').ToList();
                                var requestedPGrps = item.PurchaseGroup.Split(',').ToList();
                                List<string> pGrps = existingPGrps.Union(requestedPGrps).ToList();

                                existingMapping.PGrp = string.Join(",", pGrps.Distinct());
                                if (string.IsNullOrWhiteSpace(existingMapping.PGrp))
                                {
                                    _idmsLogger.WriteLog(LogLevelType.Info, $"No Purchasing group. Hence deleting BusinessRole({existingMapping.BusinessRole})-(Purchasing Org:{existingMapping.POrg})");
                                    existingMapping.Delete();
                                }
                                else
                                {
                                    existingMapping.LastModifiedBy = "System";
                                    existingMapping.LastModifiedDate = DateTime.Now;
                                    existingMapping.Save("System");
                                }
                            }
                        }
                    }
                }
                else if (actionType == AdHocActionType.AmendmentRemove)
                {
                    foreach (AdHocSAPSRMRequestDetail item in sapDetail.AdHocSAPSRMRequestDetails)
                    {
                        var existingMapping = SAPSRMBusinessRoleMapping.GetMappingByUserNameAndRolepOrg(sapDetail.AccountName, item.Entitlement, item.PurchaseOrg);

                        if (existingMapping != null)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"AmendmentRemove Retain Role:{item.RetainMainRole}");
                            if (item.RetainMainRole)
                            {
                                if (!string.IsNullOrWhiteSpace(item.PurchaseOrg))
                                {
                                    var existingPGrps = existingMapping.PGrp.Split(',').ToList();
                                    var requestedPGrps = item.PurchaseGroup.Split(',').ToList();
                                    List<string> pGrps = new List<string>();

                                    foreach (string pgrp in existingPGrps)
                                    {
                                        if (!requestedPGrps.Contains(pgrp))
                                        {
                                            pGrps.Add(pgrp);
                                        }
                                    }
                                    existingMapping.PGrp = string.Join(",", pGrps);
                                    if (string.IsNullOrWhiteSpace(existingMapping.PGrp))
                                    {
                                        _idmsLogger.WriteLog(LogLevelType.Info, $"No Purchasing group. Hence deleting BusinessRole({existingMapping.BusinessRole})-(Purchasing Org:{existingMapping.POrg})");
                                        existingMapping.Delete();
                                    }
                                    else
                                    {
                                        existingMapping.LastModifiedBy = "System";
                                        existingMapping.LastModifiedDate = DateTime.Now;
                                        existingMapping.Save("System");
                                    }

                                }
                            }
                            else
                            {
                                SAPSRMBusinessRoleMapping.DeleteMappingByUserName(sapDetail.AccountName, item.Entitlement);
                            }
                        }
                        else
                        {
                            if (!item.RetainMainRole)
                            {
                                SAPSRMBusinessRoleMapping.DeleteMappingByUserName(sapDetail.AccountName, item.Entitlement);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Exception, $"Error while updating SRM user and business role mapping table. {ex.StackTrace}", ex);
            }
        }

        private SAPSRMBusinessRoleMapping CreateUserRoleMappingObjectShell(DomainModel.User user, AdHocSAPSRMRequest sapDetail, AdHocSAPSRMRequestDetail roleDetail)
        {
            SAPSRMBusinessRoleMapping mapping = new SAPSRMBusinessRoleMapping();
            mapping.AccountType = this.Accounttype;
            mapping.BusinessRole = roleDetail.Entitlement;
            mapping.PGrp = roleDetail.PurchaseGroup;
            mapping.POrg = roleDetail.PurchaseOrg;
            mapping.UserId = user.Id;
            mapping.UserName = sapDetail.AccountName;
            mapping.IsActive = true;
            mapping.CreatedBy = "System";
            mapping.CreatedDate = DateTime.Now;
            mapping.LastModifiedBy = "System";
            mapping.LastModifiedDate = DateTime.Now;

            return mapping;
        }
        public override bool Extension(DomainModel.User user, ProvisioningParamCollection fields, AccountType accountType, long accountReconId = 0)
        {
            LogDebugStep($"Extension Endpoint: {Endpoint.DisplayName}");

            if (CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared)
            {
                WriteProvisioningLog(LogLevelType.DebugStep, $"Skipping the request, Account Type:{CurrentRequest.AccountType}");
                return true;
            }

            if (!CurrentRequest.EmployeeEventTriggerId.HasValue)
            {
                WriteProvisioningLog(LogLevelType.DebugStep, $"Failing the extension request, EmployeeEventTriggerId:{CurrentRequest.EmployeeEventTriggerId}");
                return true;
            }

            bool isUpdated = true;
            try
            {
                var oldTerminationDate = UtilityMethods.Deserialize<DomainModel.User>(EmployeeEventTrigger.GetById(CurrentRequest.EmployeeEventTriggerId.Value).OldUserDetails)?.TerminationDate;

                if (!oldTerminationDate.HasValue) return false;

                var accountRecons = AccountReconcilation.GetActiveAccountRecons(user.Id, Endpoint.Id, CurrentRequest.AccountType);

                WriteProvisioningLog(LogLevelType.DebugStep, $"Accounts found for extension count :{accountRecons?.Count}");

                foreach (AccountReconcilation accountRecon in accountRecons)
                {
                    try
                    {
                        if (accountRecon.ValidToDate.HasValue &&
                            (accountRecon.ValidToDate.Value.ToShortDateString() == oldTerminationDate.Value.ToShortDateString() || accountRecon.ValidToDate.Value > user.TerminationDate.Value))
                        {
                            var sapUserInfo = SAPUserinfo.GetById(accountRecon.EndpointUserId);

                            if (sapUserInfo != null)
                            {
                                sapUserInfo.UserValidTo = UtilityMethods.GetSAPDateString(user.TerminationDate.Value);

                                sapUserInfo.LastModifiedBy = "System";
                                sapUserInfo.LastModifiedDate = DateTime.Now;

                                isUpdated = EndpointConnector.ExtendValidity(sapUserInfo);

                                if (isUpdated)
                                {
                                    sapUserInfo.Save(MasterConfigHelper.ServiceUser);
                                    base.Extension(user, fields, CurrentRequest.AccountType, accountRecon.Id);
                                    WriteProvisioningLog(LogLevelType.Info, string.Format("Validity of User {0} is extended from {1} to {2} in Active Directory.", sapUserInfo.AccountName, oldTerminationDate, user.TerminationDate), null);
                                }
                            }
                        }
                        else
                        {
                            WriteProvisioningLog(LogLevelType.DebugStep, $"Skipping the account :{accountRecon.AccountName}");
                        }
                    }
                    catch (Exception)
                    {
                        WriteProvisioningLog(LogLevelType.Exception, $"Failed while extending the account {accountRecon.AccountName}, for user {user.UserName}");
                        isUpdated = false;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing the extension request. {0}", ex.Message), ex);
                return false;
            }

            return isUpdated;
        }

        private bool IsSUSRoleAlreadyExists(string accountName)
        {
            var srmRole = SAPSRMBusinessRoleMapping.GetMappingByUserName(accountName);
            if(srmRole.Count > 0)
            {
                var srm = SAPSRMMainRoleMatrix.GetByBusinessRoles(srmRole.Select(i=>i.BusinessRole).ToList());
                if(srm != null && srm.Count > 0)
                {
                    return srm.Any(i => !string.IsNullOrWhiteSpace(i.SUSRoles));
                }
            }

            return true;
        }

        private string GetUserADPrimaryAccountName(DomainModel.User user)
        {
            string accName = null;
            var adEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.ADSingaporePowerLocal);
            if(adEndpoint != null && user != null)
            {
                var adUserValue = ADSUserInfo.GetActivePrimaryByDIUserId(user.Id, adEndpoint.Id);
                accName = adUserValue?.sAMAccountName;
            }
            return accName;
        }
    }
}
