
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
    class SAPHANAEBS : Common.SAPHANABase
    {
        bool hasHRInfo = true;
        public SAPHANAEBS(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(endpoint, currentRequest, idmsLogger)
        {
            Endpoint = endpoint; CurrentRequest = currentRequest;
        }
       
        public override bool Execute(ProvisioningParamCollection fields, DomainModel.User user)
        {
            bool isExecutedSuccessfully = false;
            AdHocSAPHANARequest sapDetail = null;
            long endpointUserId = 0;
            //if adhoc request, get prov details from adhoc request object
            if (CurrentRequest.AdHocAccessRequestId.HasValue)
            {
                sapDetail = (AdHocSAPHANARequest)CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();

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

        protected bool AmendentAdd(DomainModel.User user, AdHocSAPHANARequest sapDetail, ProvisioningParamCollection fields)
        {
            LogDebugStep("AmendentAdd");
            bool isExecutedSuccessfully = false;
            var sapUserValue = SAPHANAUserinfo.GetById(sapDetail.EndpointUserId);
            LogDebugStep("sapUserValue: " + sapUserValue?.Id);
            bool samePrefix = true;
            if (!string.IsNullOrWhiteSpace(sapDetail.Position))
            {
                var matrix = SAPHANAXLPositionsMatrix.GetSAPHANAPositionMatrixByPosition(EndpointSystemId, sapDetail.Position);
                samePrefix = sapUserValue.UserLogonName.StartsWith(sapDetail.PositionPrefix, StringComparison.CurrentCultureIgnoreCase);

                LogDebugStep($"samePrefix: {samePrefix}");
                if (!sapDetail.IsReallocate)
                {
                    LogDebugStep("!sapDetail.IsReallocate");
                    if (matrix != null && !string.IsNullOrWhiteSpace(matrix.Position))
                    {
                        if (!samePrefix)
                        {
                            LogDebugStep("Creating account as the prefix is different");
                            isExecutedSuccessfully = CreateAccount(fields, user, sapDetail);
                        }
                        else
                        {
                            isExecutedSuccessfully = EndpointConnector.AssignPosition(sapUserValue, matrix.Position, matrix.PositionText, CurrentRequest.ActivationDate, CurrentRequest.DeactivationDate, hasHRInfo);
                            LogDebugStep("EndpointConnector.AssignPosition:" + matrix.Position);
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
                    var existingPositions = SAPHANAHRP1001.GetPositionsByUser(sapUserValue.UserLogonName, this.Endpoint.Id);
                    LogDebugStep($"SAPHANAHRP1001.GetPositionsByUser, UserLogonName: {sapUserValue.UserLogonName}, ExistingPositions: {string.Join(",", existingPositions)}");
                    foreach (string position in existingPositions)
                    {
                        WriteProvisioningLog(Endpoint.Id, LogLevelType.DebugStep, $"position:{position},CurrentRequest.ActivationDate:{CurrentRequest.ActivationDate}, CurrentRequest.DeactivationDate: {CurrentRequest.DeactivationDate}, hasHRInfo: {hasHRInfo}");
                        isExecutedSuccessfully = EndpointConnector.RemovePosition(sapUserValue, position, CurrentRequest.ActivationDate, CurrentRequest.ActivationDate, hasHRInfo);
                        LogDebugStep("EndpointConnector.RemovePosition:" + isExecutedSuccessfully);
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
                        var existingRoles = SAPHANAUserinfo.GetByIdIncludeSAPRoleInfo(sapUserValue.Id).SAPRoleInfo.Select(item => item.ROLE_CODE).ToList();
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
                            SAPHANAUserinfo.RemoveAllEntitlements(sapUserValue.Id);
                        }

                        LogDebugStep("sapUserValue.UserLogonName.StartsWith: " + sapDetail.PositionPrefix);
                        isExecutedSuccessfully = false;
                        if (matrix != null && !string.IsNullOrWhiteSpace(matrix.Position))
                        {
                            isExecutedSuccessfully = EndpointConnector.AssignPosition(sapUserValue, matrix.Position, matrix.PositionText, CurrentRequest.ActivationDate, CurrentRequest.DeactivationDate, hasHRInfo);
                            LogDebugStep("EndpointConnector.AssignPosition");
                        }
                        else
                        {
                            WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, $"Position not found in the Matrix :{sapDetail.Position}.");
                            isExecutedSuccessfully = true;
                        }
                    }
                    else
                    {
                        this.LockUser(sapUserValue, LockScenario.AdhocRequest);
                        LogDebugStep("this.LockUser");
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
                        WriteProvisioningLog(LogLevelType.Exception, "Email cannot be sent", new Exception("Error While Sending EMail EBS ISU User Creation"));
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

        protected bool AmendentRemove(DomainModel.User user, AdHocSAPHANARequest sapDetail)
        {
            try
            {
                var positionToRemove = string.Empty;
                if (!string.IsNullOrWhiteSpace(sapDetail.Roles))
                {
                    sapDetail.Roles = this.GetRolesFromAdhocRequest(string.Empty, sapDetail);
                }
                LogDebugStep($"AmendentRemove {sapDetail.Roles}");
                var sapUser = SAPHANAUserinfo.GetById(sapDetail.EndpointUserId);
                LogDebugStep("sapUser: " + sapUser?.Id);
                bool isExecuted = false;

                if (!string.IsNullOrWhiteSpace(sapDetail.Position))
                {

                    var matrix = SAPHANAXLPositionsMatrix.GetSAPHANAPositionMatrixByPosition(this.EndpointSystemId, sapDetail.Position);
                    if (matrix != null && !string.IsNullOrWhiteSpace(matrix.Position))
                    {
                        positionToRemove = sapDetail.Position;
                        isExecuted = EndpointConnector.RemovePosition(sapUser, matrix.Position, CurrentRequest.ActivationDate, CurrentRequest.DeactivationDate, hasHRInfo);
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

                LogDebugStep("EndpointConnector.RemoveEntitlement");
                if (isExecutedSuccessfully)
                {
                    if (sapUser.Roles.Count > 0)
                    {
                        var emailString = string.Empty;
                        SAPHANAUserinfo.RemoveEntitlement(sapUser.Id, EntitlementType.Role, grpListStr);
                        LogDebugStep("SAPHANAUserinfo.RemoveEntitlement");
                        if (!string.IsNullOrWhiteSpace(positionToRemove))
                        {
                            emailString = $"Position:{positionToRemove},Role(s):{grpListStr}";
                        }
                        else
                        {
                            emailString = $"Role(s):{grpListStr}";
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
        private bool CreateAccount(ProvisioningParamCollection fields, DomainModel.User user, AdHocSAPHANARequest sapDetail)
        {
            bool isExecutedSuccessfully = false;
            LogDebugStep("CreateAccount");
            string newUsername = string.Empty;
            try
            {
                
                sapDetail.Roles = getRoles(fields,sapDetail);

                SAPHANAUserinfo sapUserValue = SAPHANAUserinfo.GetPositionPrefixMatchedUser(user.Id, Endpoint.Id, sapDetail.PositionPrefix);

                LogDebugStep("SAPHANAUserinfo.GetLockedAccount: " + user.Id);
                var samePrefix = sapUserValue != null ? sapUserValue.UserLogonName.StartsWith(sapDetail.PositionPrefix, StringComparison.CurrentCultureIgnoreCase) : false;
                if (sapUserValue != null && samePrefix)
                {
                    AccountReactivation(fields, user, sapUserValue, sapDetail, ref isExecutedSuccessfully);
                }
                else
                {
                    newUsername = ConstructUserNameForMSSLAndEBS(user);
                    LogDebugStep($"sapUsers, newUsername: {newUsername}");
                    sapUserValue = SAPHANAUserinfo.GetLockedAccountByName(newUsername, user.Id, this.Endpoint.Id, AccountType.User);

                    if (sapUserValue != null)
                    {
                        AccountReactivation(fields, user, sapUserValue, sapDetail, ref isExecutedSuccessfully);
                    } 
                    else
                    { 
                        var sapUsers = SAPHANAUserinfo.GetActiveAccounts(user.Id, Endpoint.Id, CurrentRequest.AccountType); 

                        if (sapUsers.Any(p => p.UserLogonName == newUsername))
                        {
                            WriteProvisioningLog(LogLevelType.Error, string.Format("User already have an active EBS account - {0}. Please apply for a secondary account in {1}.", sapUsers.FirstOrDefault().UserLogonName, Endpoint.DisplayName), null);
                            return false;
                        }

                        sapUserValue = new SAPHANAUserinfo();
                        if (sapUserValue.Profiles == null)
                            sapUserValue.Profiles = new List<string>();
                        if (sapUserValue.Groups == null)
                            sapUserValue.Groups = new List<string>();

                        string accountName = string.Empty;

                        #region Set SAP User object Values

                        foreach (ProvisioningParam gData in fields)
                        {
                            sapUserValue = setUserInfoByAtrbValue(sapUserValue, user, gData) as SAPHANAUserinfo;
                        }

                        sapUserValue.IsActive = true;
                        sapUserValue.EndPointSystemId = Endpoint.Id;
                        sapUserValue.UserLogonName = newUsername;
                        LogDebugStep("ConstructUserNameForMSSLAndEBS:" + sapUserValue.UserLogonName);
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

                        var matrix = SAPHANAXLPositionsMatrix.GetSAPHANAPositionMatrixByPosition(this.EndpointSystemId, sapDetail.Position);

                        //user group
                        if (matrix != null)
                        {
                            if (!string.IsNullOrWhiteSpace(matrix.User_Group))
                            {
                                bool groupExists = this.IsSAPHANAUserGroupExists(matrix.User_Group);
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
                            WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, $"Position not found in the Matrix :{sapDetail.Position}.");
                        }
                        string[] defaultProfiles = MasterConfigHelper.SAPHANAMSPDefaultProfile;

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

                        sapUserValue.UserValidTo = this.getSAPDateFormat(GetValidTo(user.Id).Value);
                        #endregion

                        LogDebugStep("after set sap user object values");
                        isExecutedSuccessfully = EndpointConnector.CreateAccount(sapUserValue, fields);
                        LogDebugStep("EndpointConnector.CreateAccount");
                        if (isExecutedSuccessfully)
                        {
                            user.Save(MasterConfigHelper.ServiceUser);
                            AddSAPHANAUser(sapUserValue);
                            if (matrix != null && !string.IsNullOrWhiteSpace(matrix.Position))
                            {
                                bool isPositionAssigned = EndpointConnector.AssignPosition(sapUserValue, matrix.Position, matrix.PositionText, CurrentRequest.ActivationDate, CurrentRequest.DeactivationDate, hasHRInfo);
                                LogDebugStep("EndpointConnector.AssignPosition: result " + isPositionAssigned);
                                if (isPositionAssigned)
                                {
                                    sapUserValue.Position = matrix.Position;
                                }
                            }
                            else
                            {
                                WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, $"Position not found in the Matrix :{sapDetail.Position}.");
                            }
                            bool Result = ProvisioningEmailHelper.SendEmailForAccountProvisioning(CurrentRequest, sapUserValue, user, CurrentRequest.Application.ApplicationType);
                            LogDebugStep("ProvisioningEmailHelper.SendEmailForAccountProvisioning");
                            if (!Result)
                            {
                                WriteProvisioningLog(LogLevelType.Exception, "Email cannot be sent", new Exception("Error While Sending EMail in EBS ISU User Creation"));
                            }

                            AddSAPHANAUser(sapUserValue);

                            if (CurrentRequest.ParentRequestId == null)
                            {
                                LogDebugStep("EndpointConnector.AddEntitlement");
                                EndpointConnector.AddEntitlement(sapUserValue, sapUserValue.UserValidFrom, sapUserValue.UserValidTo, EntitlementType.Role);
                                LogDebugStep("SAPHANAUserinfo.AddEntitlement");
                                SAPHANAUserinfo.AddEntitlement(sapUserValue.Id, EntitlementType.Role, string.Join(",", sapUserValue.Roles));
                            }

                            WriteProvisioningLog(LogLevelType.Info, "AssignBusinessPartner Started");
                            EndpointConnector.AssignBusinessPartner(sapUserValue);
                            WriteProvisioningLog(LogLevelType.Info, "AssignBusinessPartner completed");

                            string taskId = MasterConfigHelper.EBSSAPHANAPhoneTaskId;
                            LogDebugStep("taskId: " + taskId);

                            WriteProvisioningLog(LogLevelType.Info, "CreateSAPPhone Started");
                            EndpointConnector.CreateSAPPhone(sapUserValue, taskId);
                            WriteProvisioningLog(LogLevelType.Info, "CreateSAPPhone completed");

                            //CreateDependentSystemEntriesInIDMS(sapUserValue);

                            if (CurrentRequest.AdHocAccessRequestId.HasValue)
                            {
                                WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreated, sapUserValue.UserLogonName, Endpoint.DisplayName), null);
                            }
                            else
                            {
                                WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreated, sapUserValue.UserLogonName, Endpoint.DisplayName), null);
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
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while creating EBS-ISU account for the user {0}", user.DisplayName), ex);
            }

            return isExecutedSuccessfully;
        }

        void AccountReactivation(ProvisioningParamCollection fields, DomainModel.User user, SAPHANAUserinfo sapUserValue, AdHocSAPHANARequest sapDetail, ref bool isExecutedSuccessfully)
        {
            var matrix = SAPHANAXLPositionsMatrix.GetSAPHANAPositionMatrixByPosition(this.EndpointSystemId, sapDetail.Position);
            //user group
            if (matrix != null)
            {
                if (!string.IsNullOrWhiteSpace(matrix.User_Group))
                {
                    bool groupExists = this.IsSAPHANAUserGroupExists(matrix.User_Group);
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
                WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, $"Position not found in the Matrix :{sapDetail.Position}.");
            }
            sapUserValue.UserType = SAPUserType.Dialog;
            sapUserValue.SapUserType = GetSAPUserType(sapUserValue.UserType);
            isExecutedSuccessfully = UnlockUser(sapUserValue, false);
            LogDebugStep("UnlockUser");
            if (isExecutedSuccessfully)
            {
                this.ReOrg(user, fields);
                LogDebugStep("ReOrg");

                if (matrix != null && !string.IsNullOrWhiteSpace(matrix.Position))
                {
                    bool isPositionAssigned = EndpointConnector.AssignPosition(sapUserValue, matrix.Position, matrix.PositionText, CurrentRequest.ActivationDate, CurrentRequest.DeactivationDate, hasHRInfo);
                    LogDebugStep("EndpointConnector.AssignPosition: result " + isPositionAssigned);
                    if (isPositionAssigned)
                    {
                        sapUserValue.Position = matrix.Position;
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
                    SAPHANAUserinfo.AddEntitlement(sapUserValue.Id, EntitlementType.Role, string.Join(",", sapUserValue.Roles));
                    LogDebugStep("SAPHANAUserinfo.AddEntitlement");
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
                    WriteProvisioningLog(LogLevelType.Exception, "Email cannot be sent", new Exception("Error While Sending EMail EBS ISU User Creation"));
                }
            }
        }

        private string GetRolesFromAdhocRequest(string existingRoles, AdHocSAPHANARequest sapDetail)
        {
            string ItRoles = String.Empty;
            List<string> TechnicalRoles = new List<string>();

            var bussinessRoles = UtilityMethods.GetListFromString(sapDetail.Roles);

            foreach (string bussinessRole in bussinessRoles)
            {
                var roles = SAPHANAXLRolesMatrix.GetTechnicalRolesForSAPHANA(bussinessRole, sapDetail.ApplicationName);
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
                var sapUsr = SAPHANAUserinfo.GetPrimaryByDIUserId(user.Id, Endpoint.Id);

                //sapUsr.CostCenter = string.IsNullOrWhiteSpace(sapUsr.CostCenter) ? user.CostCenter : sapUsr.CostCenter;
                //sapUsr.Function0 = user.IdentificationNo;
                sapUsr.Department = user.OrgDepartment;

                this.TakeOUSnapshot(user, sapUsr, EmployeeEvents.OUChange);

                var existingRoles = SAPHANAUserinfo.GetByIdIncludeSAPRoleInfo(sapUsr.Id).SAPRoleInfo.Select(item => item.ROLE_CODE).ToList();

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
                    SAPHANAUserinfo.RemoveAllEntitlements(sapUsr.Id);
                    WriteProvisioningLog(LogLevelType.Info, string.Format("Roles - {0} removed successfully for the user {1}.", roleListStr, sapUsr.UserLogonName), null);
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.Error, string.Format("Error while removing roles - {0} for the user {1}.", roleListStr, sapUsr.UserLogonName), null);
                }

                //Remove all positions
                var existingPositions = SAPHANAHRP1001.GetPositionsByUser(sapUsr.UserLogonName, this.Endpoint.Id);
                LogDebugStep("SAPHANAHRP1001.GetPositionsByUser");
                foreach (string position in existingPositions)
                {
                    isSucces = EndpointConnector.RemovePosition(sapUsr, position, CurrentRequest.CreatedDate, CurrentRequest.CreatedDate, hasHRInfo);
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
        bool DeleteAccount(DomainModel.User user, long endpointUserId)
        {
            try
            {
                var sapUser = SAPHANAUserinfo.GetById(endpointUserId);

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
        protected bool LockUser(SAPHANAUserinfo sapUsr, LockScenario lockScenario)
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

                        var existingPositions = SAPHANAHRP1001.GetPositionsByUser(sapUsr.UserLogonName, this.Endpoint.Id);
                        if(existingPositions != null)
                            WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, $"List of Position going to remove:{existingPositions.Count} and Position(s):{string.Join(",", existingPositions)}");

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

                        SAPHANAUserinfo.RemoveAllEntitlements(sapUsr.Id);

                        _idmsLogger.WriteLog(LogLevelType.Info, string.Format("User {0} Disabled in SAP.", sapUsr.UserLogonName), null);
                    }
                }
            }

            return isusrDisabled;
        }

        protected bool LockUser(SAPHANAUserinfo sapUsr, LockScenario lockScenario, DomainModel.User user)
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

                        var existingPositions = SAPHANAHRP1001.GetPositionsByUser(sapUsr.UserLogonName, this.Endpoint.Id);
                        if (existingPositions != null)
                            WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, $"List of Position going to remove:{existingPositions.Count} and Position(s):{string.Join(",", existingPositions)}");

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

                        SAPHANAUserinfo.RemoveAllEntitlements(sapUsr.Id);

                        _idmsLogger.WriteLog(LogLevelType.Info, string.Format("User {0} Disabled in SAPHANA.", sapUsr.UserLogonName), null);
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
                var sapUserValue = SAPHANAUserinfo.GetById(this.CurrentRequest.EventUndoRequest.EndpointUserId);
                sapUserValue.UserValidFrom = this.getSAPDateFormat(DateTime.Now);
                sapUserValue.UserValidTo = this.CurrentRequest.DeactivationDate.HasValue ? this.CurrentRequest.DeactivationDate.Value.ToString() : sapUserDefaultValidTo;
                
                if (!string.IsNullOrEmpty(positions))
                {
                    sapUserValue.Positions = UtilityMethods.GetListFromString(positions);
                    sapUserValue.Roles = sapUserValue.Roles.Distinct().ToList();
                    foreach (var position in sapUserValue.Positions)
                    {
                        var matrix = SAPHANAXLPositionsMatrix.GetSAPHANAPositionMatrixByPositionCode(position,this.EndpointSystemId);
                        if (matrix != null && !string.IsNullOrWhiteSpace(matrix.Position))
                        {
                            bool isPositionAssigned = EndpointConnector.AssignPosition(sapUserValue, matrix.Position, matrix.PositionText, CurrentRequest.ActivationDate, CurrentRequest.DeactivationDate, hasHRInfo);
                            LogDebugStep("EndpointConnector.AssignPosition: result " + isPositionAssigned);
                            if (isPositionAssigned)
                            {
                                sapUserValue.Position = matrix.Position;
                            }
                        }
                    }
                    WriteProvisioningLog(LogLevelType.Info, string.Format("Position - {0} added successfully for the user {1}.", positions, sapUserValue.UserLogonName));
                }
                if (isExecutedSuccessfully)
                {
                    LogDebugStep("Account unlock Successfully");
                    AddSAPHANAUser(sapUserValue);
                }
                else
                {
                    LogDebugStep("Account unlock failed");
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, ex.Message, ex);
                return false;
            }

            return isExecutedSuccessfully;
        }

        string getRoles(ProvisioningParamCollection fields, AdHocSAPHANARequest sapDetail)
        {
            string roles = string.Empty;

            if (fields["Roles"] != null && !string.IsNullOrWhiteSpace(fields["Roles"].Value))
                roles = fields["Roles"].Value;

            if (sapDetail != null && !string.IsNullOrWhiteSpace(sapDetail.Roles))
            {
                roles = this.GetRolesFromAdhocRequest(roles, sapDetail);
            }
            return roles;
        }

        public override bool OffBoard(DomainModel.User user, ProvisioningParamCollection fields)
        {
            if (CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared)
                return true;

            try
            {
                var sapUsrs = SAPHANAUserinfo.GetActiveAccounts(user.Id, Endpoint.Id, CurrentRequest.AccountType);

                foreach (SAPHANAUserinfo sapUsr in sapUsrs)
                {
                    this.TakeOUSnapshot(user, sapUsr, EmployeeEvents.Resignation);

                    bool success = this.LockUser(sapUsr, LockScenario.OffBoard, user);
                    if (success)
                        WriteProvisioningLog(LogLevelType.Info, string.Format("User {0} modified in SAPHANA.", sapUsr.UserLogonName), null);
                    else
                        WriteProvisioningLog(LogLevelType.Error, string.Format("Error while locking User {0} in SAPHANA.", sapUsr.UserLogonName), null);
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
