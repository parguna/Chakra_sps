
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.ProvisioningUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.Shared
{
    class MSSLCRM : Common.FLINSBase
    {
        bool hasHRInfo = false;
        public MSSLCRM(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(endpoint, currentRequest, idmsLogger)
        {

        }
        public override bool Execute(ProvisioningParamCollection fields, DomainModel.User user)
        {
            bool isExecutedSuccessfully = false;
            AdHocMSSLRequest sapDetail = null;
            string roles = string.Empty;
            long endpointUserId = 0;
            //if adhoc request, get prov details from adhoc request object
            if (CurrentRequest.AdHocAccessRequestId.HasValue)
            {
                sapDetail = (AdHocMSSLRequest)CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();

                if (sapDetail != null)
                {
                    endpointUserId = sapDetail.EndpointUserId;

                    //assign prefix to reserved10 field for user name generation
                    user.Reserved10 = sapDetail.PositionPrefix;
                }
            }

            roles = getRoles(fields, sapDetail);

            if (!string.IsNullOrEmpty(roles) && !AreEntitlementsValid(roles.Split(',')))
            {
                return false;
            }


            if (CurrentRequest.RequestSource == AccessRequestSource.AdhocRequest)
            {
                if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    isExecutedSuccessfully = CreateAccount(fields, user, sapDetail);
                }
                else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    isExecutedSuccessfully = DeleteAccount(user, endpointUserId);
                    if (isExecutedSuccessfully)
                    {
                        var sapUser = SAPUserinfo.GetById(endpointUserId);
                        if (sapUser != null)
                        {
                            WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, $"Removing Position started");
                            var existingPositions = SAPHRP1001.GetPositionsByUser(sapUser.UserLogonName, this.Endpoint.Id);
                            bool isExecuted = false;
                            foreach (string position in existingPositions)
                            {
                                try
                                {
                                    isExecuted = EndpointConnector.RemovePosition(sapUser, position, CurrentRequest.ActivationDate, null, hasHRInfo);

                                    if (isExecuted)
                                    {
                                        WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, string.Format("Position {0} removed successfully for the user {1}", position, sapUser.UserLogonName));
                                    }
                                    else
                                    {
                                        WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, string.Format("Error while removing position {0} removed for the user {1}", position, sapUser.UserLogonName));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    WriteProvisioningLog(LogLevelType.Exception, $"Error while removing position:{position} for the user {sapUser.UserLogonName}", ex);
                                }
                            }
                        }
                    }
                }
                else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                {
                    isExecutedSuccessfully = AmendentAdd(user, sapDetail, fields);
                }
                else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    isExecutedSuccessfully = AmendentRemove(user, sapDetail);
                }
            }
            else if (CurrentRequest.RequestSource == AccessRequestSource.OnBoarding)
            {
                isExecutedSuccessfully = CreateAccount(fields, user, sapDetail);
            }

            return isExecutedSuccessfully;
        }

        private bool AmendentRemove(DomainModel.User user, AdHocMSSLRequest sapDetail)
        {
            bool isExecutedSuccessfully = false;
            try
            {
                var sapUser = SAPUserinfo.GetById(sapDetail.EndpointUserId);

                if (!string.IsNullOrWhiteSpace(sapDetail.Position))
                {
                    var matrix = SAPXLMSSLPositionsMatrix.GetMSSLPositionMatrixByPosition(sapDetail.Position);
                    if (matrix != null && !string.IsNullOrWhiteSpace(matrix.Position_MCP))
                    {
                        isExecutedSuccessfully = EndpointConnector.RemovePosition(sapUser, matrix.Position_MCP, CurrentRequest.ActivationDate, CurrentRequest.DeactivationDate, false);
                    }
                    else
                    {
                        WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, $"Position not found in the Matrix :{sapDetail.Position}");
                        isExecutedSuccessfully = true;
                    }
                    if (isExecutedSuccessfully)
                    {
                        WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, string.Format("Position {0} removed successfully for the user {1}", sapDetail.Position, sapUser.UserLogonName));
                    }
                    else
                    {
                        WriteProvisioningLog(Endpoint.Id, LogLevelType.Error, string.Format("Error while removing position {0} removed for the user {1}", sapDetail.Position, sapUser.UserLogonName));
                    }
                }

                if (CurrentRequest.ParentRequestId != null)
                    return true;

                if (!string.IsNullOrWhiteSpace(sapDetail.Roles))
                {
                    sapDetail.Roles = this.GetRolesFromAdhocRequest(string.Empty, sapDetail);
                    //SAP Roles
                    var rolesToRemove = UtilityMethods.GetListFromString(sapDetail.Roles);

                    //retain supplementary and general which are used in other existing business roles
                    sapUser.Roles = rolesToRemove;

                    //Valid From and Valid To
                    if (CurrentRequest.ActivationDate != null && CurrentRequest.ActivationDate != DateTime.MinValue)
                    {
                        sapUser.UserValidFrom = this.getSAPDateFormat((DateTime)CurrentRequest.ActivationDate);
                    }
                    else
                    {
                        sapUser.UserValidFrom = this.getSAPDateFormat(DateTime.Now);
                    }

                    if (CurrentRequest.DeactivationDate != null && CurrentRequest.DeactivationDate != DateTime.MinValue)
                    {
                        sapUser.UserValidTo = this.getSAPDateFormat((DateTime)CurrentRequest.DeactivationDate);
                    }
                    else
                    {
                        sapUser.UserValidTo = sapUserDefaultValidTo;
                    }

                    isExecutedSuccessfully = EndpointConnector.RemoveEntitlement(sapUser, EntitlementType.Role);

                    string grpListStr = string.Join(",", sapUser.Roles);

                    if (isExecutedSuccessfully)
                    {
                        if (sapUser.Roles.Count > 0)
                        {
                            SAPUserinfo.RemoveEntitlement(sapUser.Id, EntitlementType.Role, grpListStr);

                            ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentRemove(user, CurrentRequest, grpListStr, sapUser.UserLogonName);

                            WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningADUserRemovedFromGroup, sapUser.UserLogonName, grpListStr, Endpoint.DisplayName), null);
                        }
                        else
                        {
                            WriteProvisioningLog(LogLevelType.Info, $"No roles to remove for user {sapUser.UserLogonName}");
                        }
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningADUserRemoveFromGroupFailed, sapUser.UserLogonName, grpListStr, Endpoint.DisplayName), null);
                    }
                }

                return isExecutedSuccessfully;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while removing roles from the sap account. {0}", ex.Message), ex);
                return false;
            }
        }

        protected bool AmendentAdd(DomainModel.User user, AdHocMSSLRequest sapDetail, ProvisioningParamCollection fields)
        {
            bool isExecutedSuccessfully = false;
            var sapUserValue = SAPUserinfo.GetById(sapDetail.EndpointUserId);
            if (!string.IsNullOrWhiteSpace(sapDetail.Position))
            {
                var matrix = SAPXLMSSLPositionsMatrix.GetMSSLPositionMatrixByPosition(sapDetail.Position);
                if (!sapDetail.IsReallocate)
                {

                    if (matrix != null && !string.IsNullOrWhiteSpace(matrix.Position_MCP))
                    {
                        isExecutedSuccessfully = EndpointConnector.AssignPosition(sapUserValue, matrix.Position_MCP, matrix.PositionText, CurrentRequest.ActivationDate, CurrentRequest.DeactivationDate, hasHRInfo);
                    }
                    else
                    {
                        WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, $"Position not found in the Matrix :{sapDetail.Position}");
                        isExecutedSuccessfully = true;
                    }
                    if (isExecutedSuccessfully)
                    {
                        WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, string.Format("Position {0} assigned successfully for the user {1}", sapDetail.Position, sapUserValue.UserLogonName));
                    }
                    else
                    {
                        WriteProvisioningLog(Endpoint.Id, LogLevelType.Error, string.Format("Error while assigning position {0} for the user {1}", sapDetail.Position, sapUserValue.UserLogonName));
                    }
                }
                else
                {
                    var existingPositions = SAPHRP1001.GetPositionsByUser(sapUserValue.UserLogonName, this.Endpoint.Id);

                    foreach (string position in existingPositions)
                    {
                        isExecutedSuccessfully = EndpointConnector.RemovePosition(sapUserValue, position, CurrentRequest.ActivationDate, CurrentRequest.ActivationDate, false);

                        if (isExecutedSuccessfully)
                        {
                            WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, string.Format("Position {0} removed successfully for the user {1}", position, sapUserValue.UserLogonName));
                        }
                        else
                        {
                            WriteProvisioningLog(Endpoint.Id, LogLevelType.Error, string.Format("Error while removing position {0} removed for the user {1}", position, sapUserValue.UserLogonName));
                        }
                    }
                    var existingRoles = SAPUserinfo.GetByIdIncludeSAPRoleInfo(sapUserValue.Id).SAPRoleInfo.Select(item => item.ROLE_CODE).ToList();
                    var defaultRoles = fields["Roles"].Value;
                    if (sapUserValue.Roles == null)
                    {
                        sapUserValue.Roles = new List<string>();
                    }
                    if (!string.IsNullOrWhiteSpace(defaultRoles))
                    {
                        foreach (string role in existingRoles)
                        {
                            if (!defaultRoles.Contains(role))
                            {
                                sapUserValue.Roles.Add(role);
                            }
                        }
                    }
                    else
                    {
                        sapUserValue.Roles = existingRoles;
                    }
                    if (sapUserValue.Roles != null && sapUserValue.Roles.Count > 0 && EndpointConnector.RemoveAllEntitlements(sapUserValue))
                    {
                        SAPUserinfo.RemoveAllEntitlements(sapUserValue.Id);
                    }
                    if (matrix != null && !string.IsNullOrWhiteSpace(matrix.Position_MCP))
                    {
                        isExecutedSuccessfully = EndpointConnector.AssignPosition(sapUserValue, matrix.Position_MCP, matrix.PositionText, CurrentRequest.ActivationDate, CurrentRequest.DeactivationDate, hasHRInfo);
                    }
                    else
                    {
                        WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, $"Position not found in the Matrix :{sapDetail.Position}");
                        isExecutedSuccessfully = true;
                    }
                    if (isExecutedSuccessfully)
                    {
                        bool Result = ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentAdd(user, CurrentRequest, matrix.PositionText, sapUserValue.UserLogonName);

                        if (!Result)
                        {
                            WriteProvisioningLog(LogLevelType.Exception, "Email cannot be sent", new Exception("Error While Sending EMail in HR System for  Active Directory User Creation"));
                        }

                        WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, string.Format("Position {0} assgined successfully for the user {1}", sapDetail.Position, sapUserValue.UserLogonName));
                    }
                }
            }

            if (CurrentRequest.ParentRequestId == null)
            {
                if (!string.IsNullOrWhiteSpace(sapDetail.Roles))
                {
                    sapDetail.Roles = this.GetRolesFromAdhocRequest(string.Empty, sapDetail);
                }
                isExecutedSuccessfully = base.AmendentAdd(user, sapDetail.EndpointUserId, sapDetail.Roles);
            }
            return isExecutedSuccessfully;
        }

        public override bool OffBoard(DomainModel.User user, ProvisioningParamCollection fields)
        {
            return true;
        }

        public override bool Propagate(DomainModel.User user, ProvisioningParamCollection fields)
        {
            return true;
        }

        public override bool RemediateAttestation(DomainModel.User user, ProvisioningParamCollection fields)
        {
            return true;
        }

        public override bool ReOrg(DomainModel.User user, ProvisioningParamCollection fields)
        {
            return true;
        }

        public override bool Retain(DomainModel.User user, ProvisioningParamCollection fields)
        {
            throw new NotImplementedException();
        }

        public override bool Transfer(DomainModel.User user, ProvisioningParamCollection fields)
        {
            return true;
        }

        #region private methods
        private bool CreateAccount(ProvisioningParamCollection fields, DomainModel.User user, AdHocMSSLRequest sapDetail)
        {
            bool isSAPAacntCreated = false;
            try
            {
                //if (!string.IsNullOrWhiteSpace(sapDetail.Roles))
                //{
                //     = this.GetRolesFromAdhocRequest(Convert.ToString(fields["Roles"].Value), sapDetail);
                //}

                sapDetail.Roles = getRoles(fields, sapDetail);

                SAPUserinfo sapUserValue = null;
                sapUserValue = SAPUserinfo.GetByUserLogonName(sapDetail.AdHocSAPMSSLSharedAccount.AccountName, this.Endpoint.Id);
                if (sapUserValue != null)
                {
                    var entitlementDetail = ((ISAPUserType)sapDetail.SharedAccountDetail);
                    if (entitlementDetail != null)
                    {
                        sapUserValue.UserType = entitlementDetail.SAPUserType;
                        sapUserValue.SapUserType = GetSAPUserType(sapUserValue.UserType);
                        if (!string.IsNullOrWhiteSpace(entitlementDetail.Description))
                            sapUserValue.Firstname = entitlementDetail.Description;
                    }
                    //user group
                    var matrix = SAPXLMSSLPositionsMatrix.GetMSSLPositionMatrixByPosition(sapDetail.Position);
                    if (matrix != null)
                    {
                        if (!string.IsNullOrWhiteSpace(matrix.User_Group))
                        {
                            bool groupExists = this.IsSAPUserGroupExists(matrix.User_Group);
                            if (groupExists)
                            {

                                sapUserValue.Groups.Add(matrix.User_Group);
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(matrix.Department_Field))
                        {
                            sapUserValue.Department = matrix.Department_Field;
                        }
                    }
                    else
                    {
                        WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, $"Position not found in the Matrix :{sapDetail.Position}");
                    }
                    LogDebugStep("UnlockUser");
                    LogDebugStep($"SAPUserinfo.GetLockedAccount: { sapUserValue?.Id} UserLogonName:{sapUserValue?.UserLogonName}");
                    if (sapUserValue.IsUserLocked)
                    {
                        isSAPAacntCreated = UnlockUser(sapUserValue, false);
                        LogDebugStep($"UnlockUser Successful:{isSAPAacntCreated}");
                    }
                    else
                    {
                        LogDebugStep($"Account:{sapUserValue.UserLogonName} already in active state:");
                        return true;
                    }
                    if (isSAPAacntCreated)
                    {
                        if (matrix != null && !string.IsNullOrWhiteSpace(matrix.Position_MCP))
                        {
                            bool isPositionAssigned = EndpointConnector.AssignPosition(sapUserValue, matrix.Position_MCP, matrix.PositionText, CurrentRequest.ActivationDate, CurrentRequest.DeactivationDate, hasHRInfo);
                            LogDebugStep("EndpointConnector.AssignPosition: result " + isPositionAssigned);
                            if (isPositionAssigned)
                            {
                                sapUserValue.Position = matrix.Position_MCP;
                            }
                        }
                        if (CurrentRequest.ParentRequestId == null)
                        {
                            List<string> sapRoles = new List<string>();
                            string[] rolesAry = !string.IsNullOrWhiteSpace(sapDetail.Roles) ? sapDetail.Roles.Split(',') : null;
                            if (rolesAry != null)
                            {
                                foreach (string tRole in rolesAry)
                                {
                                    if (tRole.Trim() != string.Empty)
                                        sapRoles.Add(tRole.Trim());
                                }
                            }
                            sapUserValue.Roles = sapRoles.Distinct().ToList();

                            EndpointConnector.AddEntitlement(sapUserValue, sapUserValue.UserValidFrom, sapUserValue.UserValidTo, EntitlementType.Role);
                            LogDebugStep("EndpointConnector.AddEntitlement");
                            SAPUserinfo.AddEntitlement(sapUserValue.Id, EntitlementType.Role, string.Join(",", sapUserValue.Roles));
                            LogDebugStep("SAPUserinfo.AddEntitlement");
                        }
                        sapUserValue.Save(MasterConfigHelper.ServiceUser);
                        sapUserValue.Password = ConstructPassword();

                        LogDebugStep($"{Endpoint?.DisplayName} - account reactivation password: {sapUserValue.Password}");
                        var passwordResetStatus = EndpointConnector.ResetPassword(sapUserValue.UserLogonName, sapUserValue.Password, string.Empty);
                        LogDebugStep($"EndpointConnector.ResetPassword - Status: {passwordResetStatus}");
                    }
                    else
                    {
                        LogDebugStep($"{sapUserValue?.UserLogonName} - account unlock failed");
                    }
                }
                else
                {
                    sapUserValue = new SAPUserinfo();
                    #region Set SAP User object Values
                    sapUserValue.IsActive = true;
                    sapUserValue.EndPointSystemId = Endpoint.Id;
                    sapUserValue.UserLogonName = CheckUserNameExists(sapDetail.AdHocSAPMSSLSharedAccount.AccountName);
                    sapUserValue.Firstname = sapDetail.AdHocSAPMSSLSharedAccount.Description;
                    sapUserValue.Lastname = sapUserValue.UserLogonName;
                    if (string.IsNullOrWhiteSpace(sapUserValue.UserLogonName))
                    {
                        return false;
                    }
                    sapUserValue.Password = ConstructPassword();
                    //sapUserValue.DIUserId = user.Id;
                    //sapUserValue.IfUnmappedMapTo = "HOD";
                    //sapUserValue.MappingType = "Manual";
                    //sapUserValue.IsSharedAccount = true;
                    sapUserValue.UserType = sapDetail.AdHocSAPMSSLSharedAccount.SAPUserType;
                    sapUserValue.SapUserType = GetSAPUserType(sapUserValue.UserType);
                    //SAP Roles
                    List<string> sapRoles = new List<string>();
                    string[] rolesAry = !string.IsNullOrWhiteSpace(sapDetail.Roles) ? sapDetail.Roles.Split(',') : null;
                    if (rolesAry != null)
                    {
                        foreach (string tRole in rolesAry)
                        {
                            if (tRole.Trim() != string.Empty)
                                sapRoles.Add(tRole.Trim());
                        }
                    }
                    sapUserValue.Roles = sapRoles.Distinct().ToList();
                    UpdateProvisioningField(fields, sapUserValue, user);
                    //SAP Params
                    Dictionary<string, string> paramsInfoDictionary = new Dictionary<string, string>();
                    if (fields["Parameters"] != null && fields["Parameters"].Value != null)
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
                        }
                    }
                    sapUserValue.Parameters = paramsInfoDictionary;

                    //user group
                    var matrix = SAPXLMSSLPositionsMatrix.GetMSSLPositionMatrixByPosition(sapDetail.Position);
                    if (matrix != null)
                    {
                        if (!string.IsNullOrWhiteSpace(matrix.User_Group))
                        {
                            bool groupExists = this.IsSAPUserGroupExists(matrix.User_Group);
                            if (groupExists)
                            {

                                sapUserValue.Groups.Add(matrix.User_Group);
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(matrix.Department_Field))
                        {
                            sapUserValue.Department = matrix.Department_Field;
                        }
                    }
                    else
                    {
                        WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, $"Position not found in the Matrix :{sapDetail.Position}");
                        
                    }
                    //user profile
                    //Get default profiles
                    string[] defaultProfiles = MasterConfigHelper.MCPDefaultProfile;

                    if (defaultProfiles != null)
                    {
                        foreach (string profile in defaultProfiles)
                        {
                            if (!string.IsNullOrWhiteSpace(profile))
                            {
                                sapUserValue.Profiles.Add(profile);
                            }
                        }
                    }

                    //OutPut Device - Default Printer
                    sapUserValue.OuputDevice = fields["OutputDevice"].Value;

                    //Valid From and Valid To

                    if (CurrentRequest.ActivationDate != null && CurrentRequest.ActivationDate != DateTime.MinValue)
                    {
                        sapUserValue.UserValidFrom = this.getSAPDateFormat((DateTime)CurrentRequest.ActivationDate);
                    }
                    else
                    {
                        sapUserValue.UserValidFrom = this.getSAPDateFormat(DateTime.Now);
                    }

                    if (CurrentRequest.DeactivationDate != null && CurrentRequest.DeactivationDate != DateTime.MinValue)
                    {
                        sapUserValue.UserValidTo = this.getSAPDateFormat((DateTime)CurrentRequest.DeactivationDate);
                    }
                    else
                    {
                        sapUserValue.UserValidTo = sapUserDefaultValidTo;
                    }
                    UpdateProvisioningField(fields, sapUserValue, user);
                    #endregion
                    isSAPAacntCreated = EndpointConnector.CreateAccount(sapUserValue, fields);
                    if (isSAPAacntCreated)
                    {
                        AddSAPUser(sapUserValue);

                        if (matrix != null && !string.IsNullOrWhiteSpace(matrix.Position_MCP))
                        {
                            bool isPositionAssigned = EndpointConnector.AssignPosition(sapUserValue, matrix.Position_MCP, matrix.PositionText, CurrentRequest.ActivationDate, CurrentRequest.DeactivationDate, hasHRInfo);

                            if (isPositionAssigned)
                            {
                                sapUserValue.Position = matrix.Position_MCP;
                            }
                        }

                        AddSAPUser(sapUserValue);

                        if (CurrentRequest.ParentRequestId == null)
                        {
                            EndpointConnector.AddEntitlement(sapUserValue, sapUserValue.UserValidFrom, sapUserValue.UserValidTo, EntitlementType.Role);
                            LogDebugStep("EndpointConnector.AddEntitlement");
                            SAPUserinfo.AddEntitlement(sapUserValue.Id, EntitlementType.Role, string.Join(",", sapUserValue.Roles));
                            LogDebugStep("SAPUserinfo.AddEntitlement");
                        }

                        sapUserValue.Save(MasterConfigHelper.ServiceUser);
                        //if (CurrentRequest.AccountType == AccountType.Service)
                        //{
                        //    sapUserValue.Password = ConstructPassword();
                        //    LogDebugStep("ConstructPassword:" + sapUserValue.Password);
                        //    EndpointConnector.ChangePassword(sapUserValue.UserLogonName, sapUserValue.Password, user.Id, true);
                        //}
                    }
                    else
                    {
                        WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreationFailed, sapUserValue.UserLogonName, Endpoint.DisplayName) + ":AdhocCreateAccount");
                    }
                }
                if (isSAPAacntCreated)
                {
                    bool Result = ProvisioningEmailHelper.SendEmailForAccountProvisioning(CurrentRequest, sapUserValue, user, CurrentRequest.Application.ApplicationType);

                    if (!Result)
                    {
                        WriteProvisioningLog(LogLevelType.Exception, "Email cannot be sent", new Exception("Error While Sending EMail in HR System for  MSSL Shared User Creation"));
                    }

                    if (CurrentRequest.AdHocAccessRequestId.HasValue)
                    {
                        WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreated, sapUserValue.UserLogonName, Endpoint.DisplayName) + ":AdhocCreateAccount");
                    }
                    else
                    {
                        WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreated, sapUserValue.UserLogonName, Endpoint.DisplayName) + ":OnBoarding");
                    }
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(Endpoint.Id, LogLevelType.Exception, ex.Message, ex);
            }
            return isSAPAacntCreated;
        }

        string getRoles(ProvisioningParamCollection fields, AdHocMSSLRequest sapDetail)
        {
            string roles = string.Empty;

            if (fields["Roles"] != null && !string.IsNullOrWhiteSpace(fields["Roles"].Value))
                roles = fields["Roles"].Value;

            if (sapDetail != null && !string.IsNullOrWhiteSpace(sapDetail.Roles) && this.CurrentRequest.IsParentRequest)
            {
                roles = this.GetRolesFromAdhocRequest(roles, sapDetail);
            }
            return roles;
        }
        private string GetRolesFromAdhocRequest(string existingRoles, AdHocMSSLRequest sapDetail)
        {
            string ItRoles = String.Empty;
            List<string> TechnicalRoles = new List<string>();

            var bussinessRoles = UtilityMethods.GetListFromString(sapDetail.Roles);

            foreach (string bussinessRole in bussinessRoles)
            {
                var roles = SAPXLMSSLRolesMatrix.GetTechnicalRolesForMSSL(bussinessRole, sapDetail.ApplicationName);
                if (roles != null && roles.Count > 0)
                    TechnicalRoles.AddRange(roles);
            }

            TechnicalRoles = TechnicalRoles.Distinct().ToList(); foreach (string technicalRole in TechnicalRoles)
            {
                if (string.IsNullOrWhiteSpace(ItRoles))
                    ItRoles = technicalRole;
                else
                    ItRoles = ItRoles + "," + technicalRole;
            }

            if (string.IsNullOrWhiteSpace(existingRoles))
            {
                return ItRoles;
            }
            else
            {
                return existingRoles + "," + ItRoles;
            }
        }
        #endregion
    }
}
