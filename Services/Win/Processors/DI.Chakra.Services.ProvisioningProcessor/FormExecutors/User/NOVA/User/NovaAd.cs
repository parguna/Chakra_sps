using DI.Chakra.Connector;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.ProvisioningUtility;
using DI.Chakra.Services.EmailingProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Services.ProvisioningProcessor.FormExecutors.User;
using System.Text.RegularExpressions;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.User.NovaAd.User
{
    class NovaAd : SPLocal, INovaAdSubsystem
    {
        public NovaAd(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(endpoint, currentRequest, idmsLogger)
        {

        }
        public bool NovaAdAmendAdd(DomainModel.User user, long endpointUserId, string groups)
        {
            bool success = false;
            try
            {
                WriteProvisioningLog(LogLevelType.Info, $"Successfully AmendAdd Ad groups for the user: {user.DisplayName}-({user.Id}) in NovaAdAmendAdd. Groups are - {groups}");

                success = AmendentAdd(user, endpointUserId, groups);
                if(success)
                    WriteProvisioningLog(LogLevelType.Info, $"Successfully AmendAdd Ad groups for the user: {user.DisplayName}-({user.Id}) in NovaAdAmendAdd. Groups are - {groups}");
                else
                    WriteProvisioningLog(LogLevelType.Info, $"Failed in AmendAdd Ad groups for the user: {user.DisplayName}-({user.Id}) in NovaAdAmendAdd. Groups are - {groups}"); 
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"Error while adding Ad groups for the user: {user.DisplayName}-({user.Id}) in NovaAdAmendAdd. {ex.Message}", ex);
                throw ex;
            }
            return success;
        }

        protected override bool AmendentAdd(DomainModel.User user, long endpointUserId, string groups)
        {
            try
            {
                var adUserValue = ADSUserInfo.GetById(endpointUserId);

                adUserValue.Groups = groups.Split(',').ToList();

                ConnectorBase ADConnector = ConnectorFactory.GetConnector(Endpoint, CurrentRequest);

                bool isExecutedSuccessfully = ADConnector.AddEntitlement(adUserValue, EntitlementType.Group);
                string grpListStr = string.Join(",", adUserValue.Groups);

                if (isExecutedSuccessfully)
                {
                    ADSUserInfo.AddEntitlement(endpointUserId, EntitlementType.Group, grpListStr);

                    //ProvisioningUtility.ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentAdd(user, CurrentRequest, grpListStr, adUserValue.sAMAccountName);
                    WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningADUserAddedInToGroup, adUserValue.sAMAccountName, grpListStr, Endpoint.DisplayName), null);
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningADUserAddedInToGroupFailed, adUserValue.sAMAccountName, grpListStr, Endpoint.DisplayName), null);
                }

                return isExecutedSuccessfully;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while adding groups to the AD account. {0}", ex.Message), ex);
                throw ex;
            }
        }
        public bool NovaAdAmendRemove(DomainModel.User user, long endpointUserId, string groups)
        {
            bool success = false;
            try
            {
                success = AmendentRemove(user, endpointUserId, groups);
                if(success)
                    WriteProvisioningLog(LogLevelType.Info, $"Successfully AmendRemove Ad groups for the user: {user.DisplayName}-({user.Id}) in NovaAdAmendRemove. Groups are {groups}");
                else
                    WriteProvisioningLog(LogLevelType.Info, $"Failed in AmendRemove Ad groups for the user: {user.DisplayName}-({user.Id}) in NovaAdAmendRemove. Groups are {groups}");

            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"Error while removing Ad groups for the user: {user.DisplayName}-({user.Id}) in NovaAdAmendRemove. {ex.Message}", ex);
                throw ex;
            }
            return success;
        }

        protected override bool AmendentRemove(DomainModel.User user, long endpointUserId, string groups)
        {
            try
            {
                var adUserValue = ADSUserInfo.GetById(endpointUserId);

                ConnectorBase ADConnector = ConnectorFactory.GetConnector(Endpoint, CurrentRequest);

                var groupsToRemove = groups.Split(',').ToList();

                var existingGroups = ADSUserInfo.GetByIdIncludeADSGroups(adUserValue.Id).ADSGroups.Select(item => item.name).ToList();

                foreach (string groupName in groupsToRemove)
                {
                    if (existingGroups.Contains(groupName))
                    {
                        adUserValue.Groups.Add(groupName);
                    }
                }

                string grpListStr = string.Join(",", adUserValue.Groups);

                bool isExecutedSuccessfully = ADConnector.RemoveEntitlement(adUserValue, EntitlementType.Group);
                if (isExecutedSuccessfully)
                {
                    ADSUserInfo.RemoveEntitlement(adUserValue.Id, EntitlementType.Group, grpListStr);

                   // ProvisioningUtility.ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentRemove(user, CurrentRequest, grpListStr, adUserValue.sAMAccountName);
                    WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningADUserRemovedFromGroup, adUserValue.sAMAccountName, grpListStr, Endpoint.DisplayName), null);
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningADUserRemoveFromGroupFailed, adUserValue.sAMAccountName, grpListStr, Endpoint.DisplayName), null);
                    throw new Exception(string.Format(ProvisioningConstants.MSG_ProvisioningADUserRemoveFromGroupFailed, adUserValue.sAMAccountName, grpListStr, Endpoint.DisplayName));
                }

                return isExecutedSuccessfully;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while removing groups from the AD account. {0}", ex.Message), ex);
                throw ex; 
            }
        }

        public bool NovaAdCreateAccount(DomainModel.User user, string groups, out ADSUserInfo adUserValue)
        {
            bool isExecutedSuccessfully = false;

            ProvisioningParamCollection fields = NProvisioningField.GetProvisioningDefaults(this.Endpoint.Id, AccountType.User);
            adUserValue = null;

            try
            {

                //Get OU for the user from AD config matrix
                var matrixData = ADSXLProvisioningMatrix.GetByUser(user);

                if (matrixData == null)
                {
                    var errormessage = $@"Default OU for the user({user.DisplayName}) is not found. For the Department : {user.DepartmentCode}, Division : {user.DivisionCode},
                      SectionCode : {user.SectionCode}, CompanyCode : {user.CompanyCode}, CostCenter :{user.CostCenter} ";

                    WriteProvisioningLog(LogLevelType.Error, errormessage, null);
                    throw new NovaException(errormessage);
                }

                string ou = matrixData.OU;

                ConnectorBase ADConnector = ConnectorFactory.GetConnector(Endpoint, CurrentRequest);

                adUserValue = ADSUserInfo.GetLockedAccount(user.Id, Endpoint.Id, AccountType.User);

                if (adUserValue != null)
                {
                    if (user.FullTimeEmployee)
                    {
                        adUserValue.ou = ou;
                        //office365user - This will sync with Office 365 
                        adUserValue.postOfficeBox = MasterConfigHelper.ADPostOfficeBoxValue;
                        adUserValue.extensionAttribute10 = user.OrgSection;
                    }
                    else
                    {
                        adUserValue.ou = MasterConfigHelper.ADNonEmployeeAccountOU;
                    }
                    if (!ADConnector.IsUserPropertyExists("OU", adUserValue.ou, null))
                    {
                        throw new Exception($"OU:{ adUserValue.ou} does not exists. Please contact endpoint system owner");
                    }
                    //No More Home Folder Creation for user as per SP suggestion
                    adUserValue.homeDirectory = string.Empty;//string.Format(@"{0}\{1}", fields["HomeFolderPath"].Value, accountName);
                    adUserValue.homeDrive = string.Empty;//fields["HomeDrive"].Value;

                    //extensionAttribute11 - Account Type
                    if (this.CurrentRequest.AccountType != AccountType.None)
                        adUserValue.extensionAttribute11 = this.CurrentRequest.AccountType.GetDescription();

                    //extensionAttribute12 - Account Owner                    
                    adUserValue.extensionAttribute12 = !string.IsNullOrWhiteSpace(user.KnownAs) ? user.KnownAs : user.FirstName + user.LastName;

                    if (user.FullTimeEmployee)
                    {
                        if (user.IsExecutiveEmployee)
                        {
                            adUserValue.otherFascimileTelephoneNumber = "Exec";
                        }
                        else
                        {
                            adUserValue.otherFascimileTelephoneNumber = "Non-Exec";
                        }
                        adUserValue.extensionAttribute13 = ADSUserInfo.GetExtensionAttribute13(user);
                        //if (user.EEGroup == "0")
                        //{
                        //    adUserValue.extensionAttribute13 = "VP&Above";
                        //}
                        //else
                        //{
                        //    adUserValue.extensionAttribute13 = "Director&Below";
                        //}
                    }
                    else
                    {
                        adUserValue.otherFascimileTelephoneNumber = "Non-Employees";
                    }

                    isExecutedSuccessfully = EnableUser(adUserValue);
                    ReOrg(user, adUserValue);

                    adUserValue.NewUsrPassword = ConstructPassword();

                    ADConnector.ChangePassword(adUserValue.sAMAccountName, adUserValue.NewUsrPassword, user.Id, true);

                    adUserValue.accountExpires = CurrentRequest.DeactivationDate == DateTime.MinValue ? null : (DateTime?)CurrentRequest.DeactivationDate;
                    ADConnector.PostReactivationProcedure(adUserValue);

                    bool Result = ProvisioningEmailHelper.SendEmailForAccountProvisioning(CurrentRequest, adUserValue, user, ApplicationType.ADSingaporePowerLocal);

                    if (!Result)
                    {
                        WriteProvisioningLog(LogLevelType.Exception, "Email cannot be sent", new Exception("Error While Sending EMail in HR System for  Active Directory User Creation"));
                    }
                    adUserValue.Save(MasterConfigHelper.ServiceUser);

                    if (user.FullTimeEmployee)
                    {
                        adUserValue.Groups = GetAdditionalGroupsByUser(user, adUserValue.EndPointSystemId, false);
                    }
                }
                else
                {
                    var adUsers = ADSUserInfo.GetActiveAccounts(user.Id, Endpoint.Id, AccountType.User);

                    if (adUsers.Count > 0)
                    {
                        var errormessage = string.Format("User already have an active AD account - {0}. Please apply for a secondary account in {1}.", adUsers.FirstOrDefault().sAMAccountName, Endpoint.DisplayName);

                        WriteProvisioningLog(LogLevelType.Error, errormessage, null);
                        throw new NovaException(errormessage); 
                    }

                    adUserValue = new ADSUserInfo();
                    string accountName = string.Empty;

                    #region Set AD Object

                    foreach (ProvisioningParam gData in fields)
                    {
                        adUserValue = setUserInfoByAtrbValue(adUserValue, user, gData) as ADSUserInfo;
                    }

                    adUserValue.EndPointSystemId = Endpoint.Id;
                    accountName = ConstructUserName(user);
                    adUserValue.sAMAccountName = accountName;
                    if (string.IsNullOrEmpty(adUserValue.sAMAccountName))
                    {
                        return false;
                    }
                    adUserValue.NewUsrPassword = ConstructPassword();
                    adUserValue.whenCreated = DateTime.Now;
                    adUserValue.whenChanged = DateTime.Now;
                    //No More Home Folder Creation for user as per SP suggestion
                    //adUserValue.homeDirectory = string.Format(@"{0}\{1}", fields["HomeFolderPath"].Value, accountName);
                    //adUserValue.homeDrive = fields["HomeDrive"].Value;
                    adUserValue.accountExpires = CurrentRequest.DeactivationDate == DateTime.MinValue ? null : (DateTime?)CurrentRequest.DeactivationDate;
                    adUserValue.sn = user.KnownAs;
                    adUserValue.cn = accountName;
                    adUserValue.profilePath = fields["UserProfilePath"].Value;
                    adUserValue.otherPager = user.OrgUnit;
                    adUserValue.extensionAttribute10 = user.OrgSection;
                    adUserValue.extensionAttribute6 = user.CostCenter;
                    adUserValue.businessCategory = user.OrgBranch;

                    //extensionAttribute11 - Account Type
                    if (this.CurrentRequest.AccountType != AccountType.None)
                        adUserValue.extensionAttribute11 = this.CurrentRequest.AccountType.GetDescription();

                    //extensionAttribute12 - Account Owner                    
                    adUserValue.extensionAttribute12 = !string.IsNullOrWhiteSpace(user.KnownAs) ? user.KnownAs : user.FirstName + user.LastName;

                    adUserValue.info += $"Created {DateTime.Now.ToString("dd MMM yyyy")} by IDMS. Request Id {this.CurrentRequest.Id};";

                    if (user.FullTimeEmployee)
                    {
                        if (user.IsExecutiveEmployee)
                        {
                            adUserValue.otherFascimileTelephoneNumber = "Exec";
                        }
                        else
                        {
                            adUserValue.otherFascimileTelephoneNumber = "Non-Exec";
                        }
                        adUserValue.extensionAttribute13 = ADSUserInfo.GetExtensionAttribute13(user);
                        //if (user.EEGroup == "0")
                        //{
                        //    adUserValue.extensionAttribute13 = "VP&Above";
                        //}
                        //else
                        //{
                        //    adUserValue.extensionAttribute13 = "Director&Below";
                        //}
                    }
                    else
                    {
                        adUserValue.otherFascimileTelephoneNumber = "Non-Employees";
                    }

                    if (user.FullTimeEmployee)
                    {
                        if (user.IsExecutiveEmployee)
                        {
                            adUserValue.extensionAttribute4 = MasterConfigHelper.O365AllowExternalEmail;
                        }
                        else
                        {
                            adUserValue.extensionAttribute4 = MasterConfigHelper.O365DenyExternalEmail;
                        }
                    }
                    else
                    {
                        adUserValue.extensionAttribute4 = MasterConfigHelper.O365DenyExternalEmail;
                    }



                    if (user.FullTimeEmployee)
                    {
                        adUserValue.ou = ou;
                        adUserValue.postOfficeBox = MasterConfigHelper.ADPostOfficeBoxValue;
                    }
                    else
                    {
                        adUserValue.ou = MasterConfigHelper.ADNonEmployeeAccountOU;
                    }
                    if (!ADConnector.IsUserPropertyExists("OU", adUserValue.ou, null))
                    {
                        throw new Exception($"OU:{ adUserValue.ou} does not exists. Please contact endpoint system owner");
                    }
                    //manager update
                    adUserValue.manager = GetRODistinguishedName(user);

                    if (!user.FullTimeEmployee)
                    {
                        adUserValue.description = user.ProjectDescription;
                    }

                    if (user.FullTimeEmployee)
                    {
                        adUserValue.Groups = GetAdditionalGroupsByUser(user, adUserValue.EndPointSystemId, false);
                    }

                    if (!string.IsNullOrWhiteSpace(fields["Groups"].Value))
                    {
                        string[] defaultADGrps = fields["Groups"].Value.Split(',');

                        if (defaultADGrps != null)
                        {
                            foreach (string tmpGrpValue in defaultADGrps)
                            {
                                if (!string.IsNullOrEmpty(tmpGrpValue))
                                {
                                    adUserValue.Groups.Add(tmpGrpValue.Trim());
                                }
                            }
                        }
                    }

                    #endregion

                    isExecutedSuccessfully = ADConnector.CreateAccount(adUserValue, fields);

                    if (isExecutedSuccessfully)
                    {
                        if (user.FullTimeEmployee)
                        {
                            user.NeedEmailUpdateToHR = true;
                        }
                        user.EmailId = adUserValue.mail;
                        user.AuthenticationMode = "AD";
                        user.UserName = adUserValue.sAMAccountName;

                        user.Save(MasterConfigHelper.ServiceUser);

                        //TODO: Need to change this logic.
                        adUserValue.IsHomeFolderPermissionSetFailed = true;

                        AddADUser(adUserValue);

                        bool Result = ProvisioningEmailHelper.SendEmailForAccountProvisioning(CurrentRequest, adUserValue, user, ApplicationType.ADSingaporePowerLocal);
                        if (!Result)
                        {
                            WriteProvisioningLog(LogLevelType.Exception, "Email cannot be sent", new Exception("Error While Sending EMail in HR System for  Active Directory User Creation"));
                        }
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningADUserCreationFailed, adUserValue.sAMAccountName, Endpoint.DisplayName), null);

                        return false;
                    }
                }

                if (isExecutedSuccessfully)
                {
                    if (adUserValue.Groups == null)
                    {
                        adUserValue.Groups = new List<string>();
                    }
                    if (!string.IsNullOrWhiteSpace(groups))
                    {
                        var grps = groups.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        if (grps.Count > 0)
                            adUserValue.Groups.AddRange(grps);
                    }
                    WriteProvisioningLog(LogLevelType.Info, $"Groups count:{adUserValue.Groups.Count}");
                    #region Add user in to Group if group exist for user
                    //Check if user has group add user in AD Groups
                    if (adUserValue.Groups != null && adUserValue.Groups.Count > 0)
                    {
                        WriteProvisioningLog(LogLevelType.Info, "AddEntitlement started");
                        isExecutedSuccessfully = ADConnector.AddEntitlement(adUserValue, EntitlementType.Group);
                        string grpListStr = string.Join(",", adUserValue.Groups);

                        if (isExecutedSuccessfully)
                        {
                            ADSUserInfo.AddEntitlement(adUserValue.Id, EntitlementType.Group, grpListStr);

                            WriteProvisioningLog(LogLevelType.Info, string.Format("AD groups sucessfully added in the AD account - {0}. The groups are : {1}", adUserValue.AccountName, string.Join(",", adUserValue.Groups)), null);

                            WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningADUserAddedInToGroup, adUserValue.sAMAccountName, grpListStr, Endpoint.Description), null);
                        }
                        else
                        {
                            WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningADUserAddedInToGroupFailed, adUserValue.sAMAccountName, grpListStr, Endpoint.Description), null);
                            throw new Exception(string.Format(ProvisioningConstants.MSG_ProvisioningADUserAddedInToGroupFailed, adUserValue.sAMAccountName, grpListStr, Endpoint.Description));
                        }
                    }
                    #endregion

                    WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningADUserCreated, adUserValue.sAMAccountName, Endpoint.Description), null);
                }
            }
            catch (Exception ex)
            {
                var err = string.Format("Error while creating the AD account. {0}", ex.Message);
                WriteProvisioningLog(LogLevelType.Exception, err, ex);
                throw new NovaException(err);

            }


            return isExecutedSuccessfully;
        }

        public bool NovaAdDeleteAccount(DomainModel.User user, long endpointUserId)
        {
           return DeleteAccount(user, endpointUserId);
        }


        protected override bool DeleteAccount(DomainModel.User user, long endpointUserId)
        {
            try
            {
                var adUser = ADSUserInfo.GetById(endpointUserId);

                bool isSuccess = DisableUser(adUser, LockScenario.AdhocRequest);

                if (isSuccess)
                {                   
                    EndpointConnector.MoveOU(adUser, MasterConfigHelper.ADDisabledAccountsOU);
                }
                return isSuccess;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while deleting the EAM NOVA AD account. {0}", ex.Message), ex);
                throw ex;
            }
        }

    }
}
