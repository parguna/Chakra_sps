
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.ProvisioningUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.User
{
    class EBSCRM : Common.FLINSBase
    {
        bool hasHRInfo = false;
        public EBSCRM(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(endpoint, currentRequest, idmsLogger)
        {
            Endpoint = endpoint; CurrentRequest = currentRequest;
        }

        public override bool Execute(ProvisioningParamCollection fields, DomainModel.User user)
        {
            bool isExecutedSuccessfully = false;
            AdHocEBSRequest sapDetail = null;
            
            long endpointUserId = 0;
            //if adhoc request, get prov details from adhoc request object
            if (CurrentRequest.AdHocAccessRequestId.HasValue)
            {
                sapDetail = (AdHocEBSRequest)CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();

                if (sapDetail != null)
                {
                    endpointUserId = sapDetail.EndpointUserId;
                    //assign prefix to reserved10 field for user name generation
                    user.Reserved10 = sapDetail.PositionPrefix;
                }
            }

            var roles = getRoles(fields, sapDetail);

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

        protected bool AmendentAdd(DomainModel.User user, AdHocEBSRequest sapDetail, ProvisioningParamCollection fields)
        {
            LogDebugStep("AmendentAdd");
            bool isExecutedSuccessfully = false;
            var sapUserValue = SAPUserinfo.GetById(sapDetail.EndpointUserId);
            LogDebugStep("sapUserValue:" + sapUserValue?.Id);
            bool samePrefix = true;
            if (!string.IsNullOrWhiteSpace(sapDetail.Position))
            {
                var matrix = SAPEBSXLPositionsMatrix.GetEBSPositionMatrixByPosition(sapDetail.Position);
                samePrefix = sapUserValue.UserLogonName.StartsWith(sapDetail.PositionPrefix, StringComparison.CurrentCultureIgnoreCase);
                LogDebugStep($"samePrefix: {samePrefix}");

                if (!sapDetail.IsReallocate)
                {
                    LogDebugStep("!sapDetail.IsReallocate");
                    if (matrix != null && !string.IsNullOrWhiteSpace(matrix.Position_CRP))
                    {
                        if (!samePrefix)
                        {
                            LogDebugStep("Creating account as the prefix is different");
                            isExecutedSuccessfully = CreateAccount(fields, user, sapDetail);
                        }
                        else
                        {
                            isExecutedSuccessfully = EndpointConnector.AssignPosition(sapUserValue, matrix.Position_CRP, matrix.PositionText, CurrentRequest.ActivationDate, CurrentRequest.DeactivationDate, hasHRInfo);
                            LogDebugStep("EndpointConnector.AssignPosition");
                        }
                    }
                    else
                    {
                        WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, $"Position not found in the Matrix :{sapDetail.Position}");
                        isExecutedSuccessfully = true;
                    }
                }
                else
                {
                    var existingPositions = SAPHRP1001.GetPositionsByUser(sapUserValue.UserLogonName, this.Endpoint.Id);
                    LogDebugStep("SAPHRP1001.GetPositionsByUser");
                    foreach (string position in existingPositions)
                    {
                        isExecutedSuccessfully = EndpointConnector.RemovePosition(sapUserValue, position, CurrentRequest.ActivationDate, CurrentRequest.ActivationDate, hasHRInfo);
                        LogDebugStep("EndpointConnector.RemovePosition");
                        if (isExecutedSuccessfully)
                        {
                            WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, string.Format("Position {0} removed successfully for the user {1}", position, sapUserValue.UserLogonName));
                        }
                        else
                        {
                            WriteProvisioningLog(Endpoint.Id, LogLevelType.Error, string.Format("Error while removing position {0} removed for the user {1}", position, sapUserValue.UserLogonName));
                        }
                    }

                    if (samePrefix)
                    {
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
                        LogDebugStep("UserLogonName.StartsWith: " + sapDetail.PositionPrefix);
                        isExecutedSuccessfully = false;
                        if (matrix != null && !string.IsNullOrWhiteSpace(matrix.Position_CRP))
                        {
                            isExecutedSuccessfully = EndpointConnector.AssignPosition(sapUserValue, matrix.Position_CRP, matrix.PositionText, CurrentRequest.ActivationDate, CurrentRequest.DeactivationDate, hasHRInfo);
                            LogDebugStep("EndpointConnector.AssignPosition");
                        }
                        else
                        {
                            WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, $"Position not found in the Matrix :{sapDetail.Position}");
                            isExecutedSuccessfully = true;
                        }
                    }
                    else
                    {
                        this.LockUser(sapUserValue, LockScenario.AdhocRequest);
                        LogDebugStep("LockUser");
                        isExecutedSuccessfully = this.CreateAccount(fields, user, sapDetail);
                        LogDebugStep("this.CreateAccount");
                    }
                }

                if (isExecutedSuccessfully)
                {
                    bool Result = ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentAdd(user, CurrentRequest, matrix.PositionText, sapUserValue.UserLogonName);
                    LogDebugStep("ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentAdd");
                    if (!Result)
                    {
                        WriteProvisioningLog(LogLevelType.Exception, "Email cannot be sent", new Exception("Error While Sending EMail in EBS CRM User Creation"));
                    }
                    WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, string.Format("Position {0} assigned successfully for the user {1}", sapDetail.Position, sapUserValue.UserLogonName));
                }
                else
                {
                    WriteProvisioningLog(Endpoint.Id, LogLevelType.Error, string.Format("Error while assigning position {0} for the user {1}", sapDetail.Position, sapUserValue.UserLogonName));
                }
            }

            if (CurrentRequest.ParentRequestId == null && samePrefix) // add entitlement if it is a parent request and has same prefix
            {
                if (!string.IsNullOrWhiteSpace(sapDetail.Roles))
                {
                    sapDetail.Roles = this.GetRolesFromAdhocRequest(string.Empty, sapDetail);
                }
                isExecutedSuccessfully = base.AmendentAdd(user, sapDetail.EndpointUserId, sapDetail.Roles);
                LogDebugStep($"base.AmendentAdd {sapDetail.Roles}");
            }
            return isExecutedSuccessfully;
        }

        protected bool AmendentRemove(DomainModel.User user, AdHocEBSRequest sapDetail)
        {
            try
            {
                var positionToRemove = string.Empty;
                if (!string.IsNullOrWhiteSpace(sapDetail.Roles))
                {
                    sapDetail.Roles = this.GetRolesFromAdhocRequest(string.Empty, sapDetail);
                }
                LogDebugStep("AmendentRemove");
                var sapUser = SAPUserinfo.GetById(sapDetail.EndpointUserId);
                LogDebugStep("sapUser: " + sapUser?.Id);
                bool isExecuted = false;

                if (!string.IsNullOrWhiteSpace(sapDetail.Position))
                {

                    var matrix = SAPEBSXLPositionsMatrix.GetEBSPositionMatrixByPosition(sapDetail.Position);
                    if (matrix != null && !string.IsNullOrWhiteSpace(matrix.Position_CRP))
                    {
                        positionToRemove = sapDetail.Position;
                        isExecuted = EndpointConnector.RemovePosition(sapUser, matrix.Position_CRP, CurrentRequest.ActivationDate, CurrentRequest.DeactivationDate, hasHRInfo);
                    }
                    else
                    {
                        WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, $"Position not found in the Matrix :{sapDetail.Position}");
                        isExecuted = true;
                    }
                    if (isExecuted)
                    {
                        WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, string.Format("Position {0} removed successfully for the user {1}", sapDetail.Position, sapUser.UserLogonName));
                    }
                    else
                    {
                        WriteProvisioningLog(Endpoint.Id, LogLevelType.Error, string.Format("Unassignment of position {0} for the user {1} is un-successful at the endpoint", sapDetail.Position, sapUser.UserLogonName));
                    }
                }
                else
                {
                    WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, $"No position requested for dependent request.");
                    isExecuted = true;
                }
                if (CurrentRequest.ParentRequestId != null)
                    return isExecuted;

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

                bool isExecutedSuccessfully = EndpointConnector.RemoveEntitlement(sapUser, EntitlementType.Role);

                string grpListStr = string.Join(",", sapUser.Roles);

                LogDebugStep("EndpointConnector.RemoveEntitlement: " + isExecutedSuccessfully);
                if (isExecutedSuccessfully)
                {
                    if (sapUser.Roles.Count > 0)
                    {
                        var emailString = string.Empty;
                        SAPUserinfo.RemoveEntitlement(sapUser.Id, EntitlementType.Role, grpListStr);
                        LogDebugStep("SAPUserinfo.RemoveEntitlement");
                        if (!string.IsNullOrWhiteSpace(positionToRemove))
                        {
                            emailString = $"Position: {positionToRemove},Role: {grpListStr}";
                        }
                        else
                        {
                            emailString = $"Role: {grpListStr}";
                        }
                        ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentRemove(user, CurrentRequest, emailString, sapUser.UserLogonName);
                        LogDebugStep("ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentRemove");
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

                return isExecutedSuccessfully;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while removing roles from the sap account. {0}", ex.Message), ex);
                return false;
            }
        }

        private bool CreateAccount(ProvisioningParamCollection fields, DomainModel.User user, AdHocEBSRequest sapDetail)
        {
            bool isExecutedSuccessfully = false;
            LogDebugStep("CreateAccount");
            string newUsername = string.Empty;
            try
            { 
                sapDetail.Roles = getRoles(fields,sapDetail);
                //SAPUserinfo sapUserValue = SAPUserinfo.GetUserAccountByUser(user.Id, Endpoint.Id);
                SAPUserinfo sapUserValue = SAPUserinfo.GetPositionPrefixMatchedUser(user.Id, Endpoint.Id, sapDetail.PositionPrefix);
                LogDebugStep("sapUserValue: " + sapUserValue?.Id);
                var samePrefix = sapUserValue != null ? sapUserValue.UserLogonName.StartsWith(sapDetail.PositionPrefix, StringComparison.CurrentCultureIgnoreCase) : false;
                if (sapUserValue != null && samePrefix)
                {
                    AccountReactivation(fields, user, sapUserValue, sapDetail, ref isExecutedSuccessfully);

                }
                else
                {
                    newUsername = ConstructUserNameForMSSLAndEBS(user);

                    LogDebugStep($"sapUsers, newUsername: {newUsername}");

                    sapUserValue = SAPUserinfo.GetLockedAccountByName(newUsername, user.Id, this.Endpoint.Id, AccountType.User);

                    if (sapUserValue != null)
                    {
                        AccountReactivation(fields, user, sapUserValue, sapDetail, ref isExecutedSuccessfully);
                    }


                    else
                    {
                        var sapUsers = SAPUserinfo.GetActiveAccounts(user.Id, Endpoint.Id, CurrentRequest.AccountType);
                       

                        if (sapUsers.Any(p => p.UserLogonName == newUsername))
                        {
                            WriteProvisioningLog(LogLevelType.Error, string.Format("User already have an active EBS account - {0}. Please apply for a secondary account in {1}.", sapUsers.FirstOrDefault().UserLogonName, Endpoint.DisplayName), null);
                            return false;
                        }

                        sapUserValue = new SAPUserinfo();
                        if (sapUserValue.Profiles == null)
                            sapUserValue.Profiles = new List<string>();
                        if (sapUserValue.Groups == null)
                            sapUserValue.Groups = new List<string>();

                        string accountName = string.Empty;

                        #region Set SAP User object Values

                        foreach (ProvisioningParam gData in fields)
                        {
                            sapUserValue = setUserInfoByAtrbValue(sapUserValue, user, gData) as SAPUserinfo;
                        }

                        sapUserValue.IsActive = true;
                        sapUserValue.EndPointSystemId = Endpoint.Id;
                        sapUserValue.UserLogonName = newUsername;
                        if (string.IsNullOrWhiteSpace(sapUserValue.UserLogonName))
                        {
                            return false;
                        }
                        sapUserValue.Password = ConstructPassword();
                        sapUserValue.UserType = SAPUserType.Dialog;
                        sapUserValue.SapUserType = GetSAPUserType(sapUserValue.UserType);
                        if (!string.IsNullOrWhiteSpace(sapDetail.Roles))
                        {
                            sapUserValue.Roles = sapDetail.Roles.Split(',').Distinct().ToList();
                        }

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

                                //paramsInfoDictionary.Add("CYMP_PERSON", "");
                            }
                        }
                        sapUserValue.Parameters = paramsInfoDictionary;

                        var matrix = SAPEBSXLPositionsMatrix.GetEBSPositionMatrixByPosition(sapDetail.Position);
                        LogDebugStep("SAPEBSXLPositionsMatrix.GetEBSPositionMatrixByPosition: " + sapDetail.Position);
                        //user group
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
                        sapUserValue.OuputDevice = fields["OutputDevice"]?.Value;

                        if (matrix != null && matrix.User_Group.ToUpper() == Constants.USERTYPE_EXTERNAL)
                        {
                            sapUserValue.OuputDevice = Constants.OUTPUT_DEVICE;
                        }

                        //Valid From and Valid To

                        if (CurrentRequest.ActivationDate != null && CurrentRequest.ActivationDate != DateTime.MinValue)
                        {
                            sapUserValue.UserValidFrom = this.getSAPDateFormat((DateTime)CurrentRequest.ActivationDate);
                        }
                        else
                        {
                            sapUserValue.UserValidFrom = this.getSAPDateFormat(DateTime.Now);
                        }

                        //if (CurrentRequest.DeactivationDate != null && CurrentRequest.DeactivationDate != DateTime.MinValue)
                        //{
                        //    sapUserValue.UserValidTo = this.getSAPDateFormat((DateTime)CurrentRequest.DeactivationDate);
                        //}
                        //else
                        //{
                        //    sapUserValue.UserValidTo = sapUserDefaultValidTo;
                        //}
                        sapUserValue.UserValidTo = this.getSAPDateFormat(GetValidTo(user.Id).Value);
                        #endregion

                        isExecutedSuccessfully = EndpointConnector.CreateAccount(sapUserValue, fields);
                        LogDebugStep("EndpointConnector.CreateAccount");

                        if (isExecutedSuccessfully)
                        {
                            user.Save(MasterConfigHelper.ServiceUser);
                            AddSAPUser(sapUserValue);
                            if (matrix != null && !string.IsNullOrWhiteSpace(matrix.Position_CRP))
                            {
                                bool isPositionAssigned = EndpointConnector.AssignPosition(sapUserValue, matrix.Position_CRP, matrix.PositionText, CurrentRequest.ActivationDate, CurrentRequest.DeactivationDate, hasHRInfo);
                                LogDebugStep("EndpointConnector.AssignPosition: result " + isPositionAssigned);
                                if (isPositionAssigned)
                                {
                                    sapUserValue.Position = matrix.Position_CRP;
                                }
                            }

                            AddSAPUser(sapUserValue);
                            LogDebugStep("sapUserValue");

                            if (CurrentRequest.ParentRequestId == null)
                            {
                                EndpointConnector.AddEntitlement(sapUserValue, sapUserValue.UserValidFrom, sapUserValue.UserValidTo, EntitlementType.Role);
                                LogDebugStep("EndpointConnector.AddEntitlement");
                                SAPUserinfo.AddEntitlement(sapUserValue.Id, EntitlementType.Role, string.Join(",", sapUserValue.Roles));
                                LogDebugStep("SAPUserinfo.AddEntitlement");
                            }
                            //CreateDependentSystemEntriesInIDMS(sapUserValue);

                            if (CurrentRequest.AdHocAccessRequestId.HasValue)
                            {
                                WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreated, sapUserValue.UserLogonName, Endpoint.DisplayName), null);
                            }
                            else
                            {
                                WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreated, sapUserValue.UserLogonName, Endpoint.DisplayName), null);
                            }

                            EndpointConnector.AssignBusinessPartner(sapUserValue);
                            LogDebugStep("EndpointConnector.AssignBusinessPartner");

                            string taskId = MasterConfigHelper.EBSSAPPhoneTaskId;
                            LogDebugStep("taskId: " + taskId);

                            EndpointConnector.CreateSAPPhone(sapUserValue, taskId);
                            LogDebugStep("CreateSAPPhone");

                            bool Result = ProvisioningEmailHelper.SendEmailForAccountProvisioning(CurrentRequest, sapUserValue, user, CurrentRequest.Application.ApplicationType);
                            LogDebugStep("ProvisioningEmailHelper.SendEmailForAccountProvisioning");

                            if (!Result)
                            {
                                WriteProvisioningLog(LogLevelType.Exception, "Email cannot be sent", new Exception("Error While Sending EMail in EBS CRM User Creation"));
                            }
                        }
                        else
                        {
                            WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreationFailed, sapUserValue.UserLogonName, Endpoint.DisplayName), null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while creating EBS-CRM account for the user {0}", user.DisplayName), ex);
            }

            return isExecutedSuccessfully;
        }


        string getRoles(ProvisioningParamCollection fields, AdHocEBSRequest sapDetail)
        {
            string roles = string.Empty;

            if (fields["Roles"] != null && !string.IsNullOrWhiteSpace(fields["Roles"].Value))
                roles = fields["Roles"].Value;

            if (sapDetail!=null && !string.IsNullOrWhiteSpace(sapDetail.Roles) && this.CurrentRequest.IsParentRequest)
            {
                roles = this.GetRolesFromAdhocRequest(roles, sapDetail);
            }
            return roles;
        }

        void AccountReactivation(ProvisioningParamCollection fields, DomainModel.User user,SAPUserinfo sapUserValue, AdHocEBSRequest sapDetail,ref bool isExecutedSuccessfully)
        {
            var matrix = SAPEBSXLPositionsMatrix.GetEBSPositionMatrixByPosition(sapDetail.Position);
            LogDebugStep("SAPEBSXLPositionsMatrix.GetEBSPositionMatrixByPosition: " + sapDetail.Position);
            //user group
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

            sapUserValue.UserType = SAPUserType.Dialog;
            sapUserValue.SapUserType = GetSAPUserType(sapUserValue.UserType);
            isExecutedSuccessfully = UnlockUser(sapUserValue, false);
            LogDebugStep("UnlockUser");
            if (isExecutedSuccessfully)
            {
                this.ReOrg(user, fields);
                LogDebugStep("ReOrg");
                if (matrix != null && !string.IsNullOrWhiteSpace(matrix.Position_CRP))
                {
                    bool isPositionAssigned = EndpointConnector.AssignPosition(sapUserValue, matrix.Position_CRP, matrix.PositionText, CurrentRequest.ActivationDate, CurrentRequest.DeactivationDate, hasHRInfo);
                    LogDebugStep("EndpointConnector.AssignPosition: result " + isPositionAssigned);
                    if (isPositionAssigned)
                    {
                        sapUserValue.Position = matrix.Position_CRP;
                    }
                }

                if (!string.IsNullOrWhiteSpace(sapDetail.Roles))
                {
                    sapUserValue.Roles = sapDetail.Roles.Split(',').Distinct().ToList();
                }
                if (CurrentRequest.ActivationDate != null && CurrentRequest.ActivationDate != DateTime.MinValue)
                {
                    sapUserValue.UserValidFrom = this.getSAPDateFormat((DateTime)CurrentRequest.ActivationDate);
                }
                else
                {
                    sapUserValue.UserValidFrom = this.getSAPDateFormat(DateTime.Now);
                }


                sapUserValue.UserValidTo = this.getSAPDateFormat(GetValidTo(user.Id).Value);
                if (CurrentRequest.ParentRequestId == null)
                {
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

                bool Result = ProvisioningEmailHelper.SendEmailForAccountProvisioning(CurrentRequest, sapUserValue, user, CurrentRequest.Application.ApplicationType);
                LogDebugStep("ProvisioningEmailHelper.SendEmailForAccountProvisioning");
                if (!Result)
                {
                    WriteProvisioningLog(LogLevelType.Exception, "Email cannot be sent", new Exception("Error While Sending EMail in EBS CRM Create Account"));
                }
            }
        }
        DateTime? GetValidTo(Nullable<long> userId)
        {
            var validTo = DateTime.MaxValue;
            if ((CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared) && !CurrentRequest.DeactivationDate.HasValue) return validTo;

            if (CurrentRequest != null && CurrentRequest.DeactivationDate.HasValue)
                return CurrentRequest.DeactivationDate.Value;
            else
            {
                if (userId.HasValue && userId.Value > 0)
                {
                    var user = DomainModel.User.GetById(userId.Value);
                    if (user != null && user.TerminationDate.HasValue) validTo = user.TerminationDate.Value;
                }
            }
            return validTo;
        }
        private string GetRolesFromAdhocRequest(string existingRoles, AdHocEBSRequest sapDetail)
        {
            string ItRoles = String.Empty;
            List<string> TechnicalRoles = new List<string>();

            var bussinessRoles = UtilityMethods.GetListFromString(sapDetail.Roles);

            foreach (string bussinessRole in bussinessRoles)
            {
                var roles = SAPEBSXLRoleMatrix.GetTechnicalRolesForEBS(bussinessRole, sapDetail.ApplicationName);
                if (roles != null && roles.Count > 0)
                    TechnicalRoles.AddRange(roles);
            }

            TechnicalRoles = TechnicalRoles.Distinct().ToList();

            foreach (string technicalRole in TechnicalRoles)
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

        public override bool RemediateAttestation(DomainModel.User user, ProvisioningParamCollection fields)
        {
            throw new NotImplementedException();
        }

        public override bool Retain(DomainModel.User user, ProvisioningParamCollection fields)
        {
            throw new NotImplementedException();
        }

        public override bool Transfer(DomainModel.User user, ProvisioningParamCollection fields)
        {
            try
            {
                var sapUsr = SAPUserinfo.GetPrimaryByDIUserId(user.Id, Endpoint.Id);

                //sapUsr.CostCenter = string.IsNullOrWhiteSpace(sapUsr.CostCenter) ? user.CostCenter : sapUsr.CostCenter;
                //sapUsr.Function0 = user.IdentificationNo;
                sapUsr.Department = user.OrgDepartment;

                this.TakeOUSnapshot(user, sapUsr, EmployeeEvents.OUChange);

                var existingRoles = SAPUserinfo.GetByIdIncludeSAPRoleInfo(sapUsr.Id).SAPRoleInfo.Select(item => item.ROLE_CODE).ToList();

                //retain groups created using prov form

                var defaultRoles = fields["Roles"].Value;

                if (!string.IsNullOrWhiteSpace(defaultRoles))
                {
                    foreach (string role in existingRoles)
                    {
                        if (!defaultRoles.Contains(role))
                        {
                            sapUsr.Roles.Add(role);
                        }
                    }
                }
                else
                {
                    sapUsr.Roles = existingRoles;
                }

                bool isSucces = EndpointConnector.RemoveAllEntitlements(sapUsr);
                string roleListStr = string.Join(",", sapUsr.Roles.Select(item => item));

                if (isSucces)
                {
                    SAPUserinfo.RemoveAllEntitlements(sapUsr.Id);
                    WriteProvisioningLog(LogLevelType.Info, string.Format("Roles - {0} removed successfully for the user {1}.", roleListStr, sapUsr.UserLogonName), null);
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.Error, string.Format("Error while removing roles - {0} for the user {1}.", roleListStr, sapUsr.UserLogonName), null);
                }

                //Remove all positions
                var existingPositions = SAPHRP1001.GetPositionsByUser(sapUsr.UserLogonName, this.Endpoint.Id);
                LogDebugStep("SAPHRP1001.GetPositionsByUser");
                foreach (string position in existingPositions)
                {
                    isSucces = EndpointConnector.RemovePosition(sapUsr, position, CurrentRequest.ActivationDate, CurrentRequest.ActivationDate, hasHRInfo);
                    LogDebugStep("EndpointConnector.RemovePosition:" + isSucces);
                    if (isSucces)
                    {
                        WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, string.Format("Position {0} removed successfully for the user {1}", position, sapUsr.UserLogonName));
                    }
                    else
                    {
                        WriteProvisioningLog(Endpoint.Id, LogLevelType.Error, string.Format("Error while removing position {0} removed for the user {1}", position, sapUsr.UserLogonName));
                    }
                }

                LockUser(sapUsr, LockScenario.OUChange);
                return true;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing the transfer request. {0}", ex.Message), ex);
                return false;
            }
        }
        new bool DeleteAccount(DomainModel.User user, long endpointUserId)
        {
            try
            {
                var sapUser = SAPUserinfo.GetById(endpointUserId);

                bool isSuccess = LockUser(sapUser, LockScenario.AdhocRequest);

                if (isSuccess)
                {
                    ProvisioningEmailHelper.SendEmailAfterAdhocUserRemoval(user, Endpoint, CurrentRequest, sapUser.UserLogonName);
                }
                return isSuccess;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while deleting the {1} account. {0}", ex.Message, Endpoint.DisplayName), ex);
                return false;
            }
        }
        protected bool LockUser(SAPUserinfo sapUsr, LockScenario lockScenario)
        {
            bool isusrDisabled = true;

            if (sapUsr != null)
            {
                isusrDisabled = false;

                sapUsr.UserValidTo = this.getSAPDateFormat(DateTime.Today); //lockScenario != LockScenario.AdhocRequest ? this.getSAPDateFormat(DateTime.Today.AddDays(-1)) : this.getSAPDateFormat(DateTime.Today); 

                EndpointConnector.UpdateAccount(sapUsr);

                isusrDisabled = EndpointConnector.LockAccount(sapUsr);
                if (isusrDisabled)
                {
                    sapUsr.IsUserLocked = true;
                    sapUsr.LastModifiedBy = "System";
                    sapUsr.LastModifiedDate = DateTime.Now;

                    sapUsr.Save(MasterConfigHelper.ServiceUser);
                    if (isusrDisabled)
                    {
                        var existingPositions = SAPHRP1001.GetPositionsByUser(sapUsr.UserLogonName, this.Endpoint.Id);

                        foreach (string position in existingPositions)
                        {
                            bool isPositionRemoved = EndpointConnector.RemovePosition(sapUsr, position, DateTime.Today, null, hasHRInfo);

                            if (isPositionRemoved)
                            {
                                WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, string.Format("Position {0} removed successfully for the user {1}", position, sapUsr.UserLogonName));
                            }
                            else
                            {
                                WriteProvisioningLog(Endpoint.Id, LogLevelType.Error, string.Format("Error while removing position {0} removed for the user {1}", position, sapUsr.UserLogonName));
                            }
                        }

                        EndpointConnector.RemoveAllEntitlements(sapUsr);

                        SAPUserinfo.RemoveAllEntitlements(sapUsr.Id);

                        _idmsLogger.WriteLog(LogLevelType.Info, string.Format("User {0} Disabled in SAP.", sapUsr.UserLogonName), null);
                    }
                }
            }

            return isusrDisabled;
        }

        protected bool LockUser(SAPUserinfo sapUsr, LockScenario lockScenario, DomainModel.User user)
        {
            bool isusrDisabled = true;

            if (sapUsr != null)
            {
                isusrDisabled = false;

                sapUsr.UserValidTo = this.getSAPDateFormat(DateTime.Today); //(lockScenario != LockScenario.AdhocRequest && user.FullTimeEmployee) ? this.getSAPDateFormat(DateTime.Today.AddDays(-1)) : this.getSAPDateFormat(DateTime.Today);

                EndpointConnector.UpdateAccount(sapUsr);

                isusrDisabled = EndpointConnector.LockAccount(sapUsr);
                if (isusrDisabled)
                {
                    sapUsr.IsUserLocked = true;
                    sapUsr.LastModifiedBy = "System";
                    sapUsr.LastModifiedDate = DateTime.Now;

                    sapUsr.Save(MasterConfigHelper.ServiceUser);
                    if (isusrDisabled)
                    {
                        var existingPositions = SAPHRP1001.GetPositionsByUser(sapUsr.UserLogonName, this.Endpoint.Id);

                        foreach (string position in existingPositions)
                        {
                            bool isPositionRemoved = EndpointConnector.RemovePosition(sapUsr, position, DateTime.Today, null, hasHRInfo);

                            if (isPositionRemoved)
                            {
                                WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, string.Format("Position {0} removed successfully for the user {1}", position, sapUsr.UserLogonName));
                            }
                            else
                            {
                                WriteProvisioningLog(Endpoint.Id, LogLevelType.Error, string.Format("Error while removing position {0} removed for the user {1}", position, sapUsr.UserLogonName));
                            }
                        }

                        EndpointConnector.RemoveAllEntitlements(sapUsr);

                        SAPUserinfo.RemoveAllEntitlements(sapUsr.Id);

                        _idmsLogger.WriteLog(LogLevelType.Info, string.Format("User {0} Disabled in SAP.", sapUsr.UserLogonName), null);
                    }
                }
            }

            return isusrDisabled;
        }
        public override bool UndoEvent(DomainModel.User user, ProvisioningParamCollection fields)
        {
            var isExecutedSuccessfully = false;
            try
            {
                isExecutedSuccessfully = base.UndoEvent(user, fields);
                string positions = this.CurrentRequest.EventUndoRequest.TechnicalPositions;
                var sapUserValue = SAPUserinfo.GetById(this.CurrentRequest.EventUndoRequest.EndpointUserId);
                sapUserValue.UserValidFrom = this.getSAPDateFormat(DateTime.Now);
                sapUserValue.UserValidTo = this.CurrentRequest.DeactivationDate.HasValue ? this.CurrentRequest.DeactivationDate.Value.ToString() : sapUserDefaultValidTo;
                
                if (!string.IsNullOrEmpty(positions))
                {
                    sapUserValue.Positions = UtilityMethods.GetListFromString(positions);
                    sapUserValue.Roles = sapUserValue.Roles.Distinct().ToList();
                    foreach (var position in sapUserValue.Positions)
                    {
                        var matrix = SAPEBSXLPositionsMatrix.GetEBSPositionMatrixByPositionCode(position,true);
                        if (matrix != null && !string.IsNullOrWhiteSpace(matrix.Position_ISP))
                        {
                            bool isPositionAssigned = EndpointConnector.AssignPosition(sapUserValue, matrix.Position_ISP, matrix.PositionText, CurrentRequest.ActivationDate, CurrentRequest.DeactivationDate, hasHRInfo);
                            LogDebugStep("EndpointConnector.AssignPosition: result " + isPositionAssigned);
                            if (isPositionAssigned)
                            {
                                sapUserValue.Position = matrix.Position_ISP;
                            }
                        }
                    }
                    WriteProvisioningLog(LogLevelType.Info, string.Format("Position - {0} added successfully for the user {1}.", positions, sapUserValue.UserLogonName));
                }
                if (isExecutedSuccessfully)
                {
                    WriteProvisioningLog(LogLevelType.Info, string.Format("User unlocked successfully for the user {0}.", sapUserValue.UserLogonName));
                    AddSAPUser(sapUserValue);
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.Info, string.Format("User unlock failed for the user {0}.",sapUserValue.UserLogonName));
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, ex.Message, ex);
                return false;
            }

            return true;
        }

        public override bool OffBoard(DomainModel.User user, ProvisioningParamCollection fields)
        {
            if (CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared)
                return true;

            try
            {
                var sapUsrs = SAPUserinfo.GetActiveAccounts(user.Id, Endpoint.Id, CurrentRequest.AccountType);

                foreach (SAPUserinfo sapUsr in sapUsrs)
                {
                    this.TakeOUSnapshot(user, sapUsr, EmployeeEvents.Resignation);

                    bool success = this.LockUser(sapUsr, LockScenario.OffBoard,user);
                    if (success)
                        WriteProvisioningLog(LogLevelType.Info, string.Format("User {0} modified in SAP.", sapUsr.UserLogonName), null);
                    else
                        WriteProvisioningLog(LogLevelType.Error, string.Format("Error while locking User {0} in SAP.", sapUsr.UserLogonName), null);
                }

                return true;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing offboarding request. {0}", ex.Message), ex);
                return false;
            }
        }
    }
}
