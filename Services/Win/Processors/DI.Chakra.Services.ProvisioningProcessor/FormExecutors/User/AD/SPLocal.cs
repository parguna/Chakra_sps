using DI.Chakra.Connector;
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
    class SPLocal : Common.ADBase
    {

        public SPLocal(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(endpoint, currentRequest, idmsLogger)
        {
            Endpoint = endpoint; CurrentRequest = currentRequest;
        }

        public override bool Execute(ProvisioningParamCollection fields, DomainModel.User user)
        {
            bool isExecutedSuccessfully = false;
            AdHocADRequest adDetail = null;
            string groups = string.Empty;

            long endpointUserId = 0;

            //if adhoc request, get prov details from adhoc request object
            if (CurrentRequest.AdHocAccessRequestId.HasValue)
            {
                adDetail = (AdHocADRequest)CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();

                if (adDetail != null)
                {
                    groups = adDetail.Entitlement;
                    endpointUserId = adDetail.EndpointUserId;
                }
            }

            if (CurrentRequest.RequestSource == AccessRequestSource.AdhocRequest)
            {
                if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    isExecutedSuccessfully = CreateAccount(fields, user, groups);
                }
                else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    isExecutedSuccessfully = DeleteAccount(user, endpointUserId);
                }
                else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                {
                    isExecutedSuccessfully = AmendentAdd(user, endpointUserId, groups);
                }
                else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    isExecutedSuccessfully = AmendentRemove(user, endpointUserId, groups);
                }
            }
            else if (CurrentRequest.RequestSource == AccessRequestSource.OnBoarding || CurrentRequest.RequestSource == AccessRequestSource.Rehire)
            {
                isExecutedSuccessfully = CreateAccount(fields, user, groups);
                if (isExecutedSuccessfully)
                {
                    user.IsActive = true;
                    if (user.TerminationDate.HasValue)
                    {
                        if (user.TerminationDate.Value <= DateTime.Today)
                            user.TerminationDate = null;
                    }
                    user.LastModifiedBy = "System";
                    user.LastModifiedDate = DateTime.Now;
                    user.Save("System");
                }
            }

            return isExecutedSuccessfully;
        }

        public override bool Propagate(DomainModel.User user, ProvisioningParamCollection fields)
        {
            try
            {
                bool isUpdated = true;

                var adUsr = ADSUserInfo.GetActivePrimaryByDIUserId(user.Id, Endpoint.Id);
                if(adUsr == null)
                {
                    WriteProvisioningLog(LogLevelType.Info, $"{this.Endpoint?.DisplayName}: User:{user.DisplayName} dont have any active account for propagation.", null);
                    return isUpdated;
                }
                adUsr.title = user.PositionName;
                adUsr.sn = user.KnownAs;
                adUsr.displayName = user.KnownAs;

                if (!user.FullTimeEmployee)
                {
                    adUsr.description = user.ProjectDescription;
                }
                else
                {
                    adUsr.description = user.KnownAs;
                }

                adUsr.department = user.OrgDepartment;
                adUsr.division = user.OrgDivision;
                adUsr.company = user.OrgCompany;
                adUsr.businessCategory = user.OrgBranch;
                adUsr.pager = user.OrgSection;

                adUsr.postalAddress = user.PostalAddress;
                adUsr.postalCode = user.PostalCode;
                adUsr.countryCode = user.Country;

                adUsr.telephoneNumber = user.TelephoneNumber;
                adUsr.mobile = user.MobileNumber;
                adUsr.facsimileTelephoneNumber = user.Fax;
                adUsr.mail = user.EmailId;
                adUsr.physicalDeliveryOfficeName = user.EmployeeId;

                adUsr.otherPager = user.OrgUnit;
                adUsr.extensionAttribute6 = user.CostCenter;
                adUsr.extensionAttribute10 = user.OrgSection;

                if (user.FullTimeEmployee)
                {
                    if (user.IsExecutiveEmployee)
                    {
                        adUsr.otherFascimileTelephoneNumber = "Exec";
                    }
                    else
                    {
                        adUsr.otherFascimileTelephoneNumber = "Non-Exec";
                    }

                    adUsr.extensionAttribute13 = ADSUserInfo.GetExtensionAttribute13(user);
                    //if (user.EEGroup == "0")
                    //{
                    //    adUsr.extensionAttribute13 = "VP&Above";
                    //}
                    //else
                    //{
                    //    adUsr.extensionAttribute13 = "Director&Below";
                    //}
                }
                else
                {
                    adUsr.otherFascimileTelephoneNumber = "Non-Employees";
                }

                adUsr.LastModifiedBy = "System";
                adUsr.LastModifiedDate = DateTime.Now;

                //manager update
                adUsr.manager = GetRODistinguishedName(user);
                UpdateExtenstionAttribute8(user, adUsr);
                isUpdated = EndpointConnector.UpdateAccount(adUsr);

                if (isUpdated)
                {
                    adUsr.Save(MasterConfigHelper.ServiceUser);
                    WriteProvisioningLog(LogLevelType.Info, string.Format("User {0} modified  in Active Directory.", adUsr.sAMAccountName), null);
                }

                return isUpdated;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing the propagation request. {0}", ex.Message), ex);
                return false;
            }
        }

        public override bool ReOrg(DomainModel.User user, ProvisioningParamCollection fields)
        {
            try
            {
                ADSUserInfo adUserValue = null;
                if (this.CurrentRequest.EventUndoRequestId.HasValue && this.CurrentRequest.EventUndoRequest != null && this.CurrentRequest.EventUndoRequest.EndpointUserId != 0)
                {
                    adUserValue = ADSUserInfo.GetById(this.CurrentRequest.EventUndoRequest.EndpointUserId);
                }
                else
                {
                    adUserValue = ADSUserInfo.GetActivePrimaryByDIUserId(user.Id, Endpoint.Id);
                }
                if(adUserValue != null)
                {

                    //set new ou values to AD user object
                    adUserValue.title = user.PositionName;
                    adUserValue.sn = user.KnownAs;
                    adUserValue.displayName = user.KnownAs;

                    if (!user.FullTimeEmployee)
                    {
                        adUserValue.description = user.ProjectDescription;
                    }
                    else
                    {
                        adUserValue.description = user.KnownAs;
                    }

                    adUserValue.department = user.OrgDepartment;
                    adUserValue.division = user.OrgDivision;
                    adUserValue.company = user.OrgCompany;
                    adUserValue.businessCategory = user.OrgBranch;
                    adUserValue.pager = user.OrgSection;

                    adUserValue.postalAddress = user.PostalAddress;
                    adUserValue.postalCode = user.PostalCode;
                    adUserValue.countryCode = user.Country;

                    adUserValue.telephoneNumber = user.TelephoneNumber;
                    adUserValue.mobile = user.MobileNumber;
                    adUserValue.facsimileTelephoneNumber = user.Fax;
                    adUserValue.mail = user.EmailId;
                    adUserValue.physicalDeliveryOfficeName = user.EmployeeId;

                    adUserValue.otherPager = user.OrgUnit;
                    adUserValue.extensionAttribute6 = user.CostCenter;
                    adUserValue.extensionAttribute10 = user.OrgSection;

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

                    adUserValue.LastModifiedBy = "System";
                    adUserValue.LastModifiedDate = DateTime.Now;

                    string message = ProvisioningValidator.IsADDefaultsExists(user);
                    if (message.Length > 0)
                    {
                        WriteProvisioningLog(LogLevelType.Warning, $"ADSXLProvisioningMatrix is NULL for  the ReOrg request-{this.CurrentRequest.Id} user- {user.KnownAs}");
                        ProvisioningValidator.AlertITAdmin(user, message, EmployeeEvents.ReOrganisation);

                        return false;
                    }
                    if (user.FullTimeEmployee)
                    {
                        var matrix = ADSXLProvisioningMatrix.GetByUser(user);
                        if (matrix != null && !string.IsNullOrWhiteSpace(matrix.OU))
                        {
                            adUserValue.ou = matrix.OU;
                        }
                        else
                        {
                            WriteProvisioningLog(LogLevelType.Error, $"ADSXLProvisioningMatrix is NULL for  the Re-Org (Override Method) request-{this.CurrentRequest.Id} user- {user.KnownAs}");
                        }
                    }
                    else
                    {
                        adUserValue.ou = MasterConfigHelper.ADNonEmployeeAccountOU;
                    }

                    //manager update
                    adUserValue.manager = GetRODistinguishedName(user);
                    UpdateExtenstionAttribute8(user, adUserValue);
                    EndpointConnector.ChangeContainer(adUserValue);

                    adUserValue.Save(MasterConfigHelper.ServiceUser);

                    //add default groups for the new org.
                    if (user.FullTimeEmployee)
                    {
                        adUserValue.Groups = GetAdditionalGroupsByUser(user, adUserValue.EndPointSystemId, true);

                        bool isAdded = EndpointConnector.AddEntitlement(adUserValue, EntitlementType.Group);
                        if (isAdded)
                        {
                            ADSUserInfo.AddEntitlement(adUserValue.Id, EntitlementType.Group, string.Join(",", adUserValue.Groups));
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing the re-org request. {0}", ex.Message), ex);
                return false;
            }
        }

        public override bool Transfer(DomainModel.User user, ProvisioningParamCollection fields)
        {
            try
            {
                var adUserValue = ADSUserInfo.GetActivePrimaryByDIUserId(user.Id, Endpoint.Id);

                if (adUserValue == null)
                {
                    WriteProvisioningLog(LogLevelType.Info, $"Transfer request- AD User not found for User {user.KnownAs}, Request Id {this.CurrentRequest.Id}");
                }

                //Start Removing Existing Groups
                WriteProvisioningLog(LogLevelType.Info, $"Transfer request- Started Removing Exisitng Groups AD User {user.KnownAs}");

                TakeOUSnapshot(user.Id, adUserValue, EmployeeEvents.OUChange, EndpointType.ActiveDirectory, EntitlementType.Group);

                var existingGroups = ADSUserInfo.GetByIdIncludeADSGroups(adUserValue.Id).ADSGroups.Select(item => item.name).ToList();

                //retain groups created using prov form

                var defaultGroups = fields["Groups"].Value;

                if (!string.IsNullOrEmpty(defaultGroups))
                {
                    foreach (string group in existingGroups)
                    {
                        if (!defaultGroups.Contains(group))
                        {
                            adUserValue.Groups.Add(group);
                        }
                    }
                }
                else
                {
                    adUserValue.Groups = existingGroups;
                }

                EndpointConnector.RemoveEntitlement(adUserValue, EntitlementType.Group);

                ADSUserInfo.RemoveEntitlement(adUserValue.Id, EntitlementType.Group, string.Join(",", adUserValue.Groups));

                WriteProvisioningLog(LogLevelType.Info, $"Transfer request- Completed Removing Exisitng Groups AD User {user.KnownAs}");
                //Removing Existing Groups Completed


                //Started Adding New Groups and OU

                WriteProvisioningLog(LogLevelType.Info, $"Transfer request- Started Updating AD User {user.KnownAs}");

                //set new ou values to AD user object
                adUserValue.title = user.PositionName;
                adUserValue.sn = user.KnownAs;
                adUserValue.displayName = user.KnownAs;

                if (!user.FullTimeEmployee)
                {
                    adUserValue.description = user.ProjectDescription;
                }
                else
                {
                    adUserValue.description = user.KnownAs;
                }

                adUserValue.department = user.OrgDepartment;
                adUserValue.division = user.OrgDivision;
                adUserValue.company = user.OrgCompany;
                adUserValue.businessCategory = user.OrgBranch;
                adUserValue.pager = user.OrgSection;

                adUserValue.postalAddress = user.PostalAddress;
                adUserValue.postalCode = user.PostalCode;
                adUserValue.countryCode = user.Country;

                adUserValue.telephoneNumber = user.TelephoneNumber;
                adUserValue.mobile = user.MobileNumber;
                adUserValue.facsimileTelephoneNumber = user.Fax;
                adUserValue.mail = user.EmailId;
                adUserValue.physicalDeliveryOfficeName = user.EmployeeId;

                adUserValue.otherPager = user.OrgUnit;
                adUserValue.extensionAttribute6 = user.CostCenter;
                adUserValue.extensionAttribute10 = user.OrgSection;

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

                string message = ProvisioningValidator.IsADDefaultsExists(user);
                if (message.Length > 0)
                {
                    WriteProvisioningLog(LogLevelType.Warning, $"ADSXLProvisioningMatrix is NULL for  the Transfer request-{this.CurrentRequest.Id} user- {user.KnownAs}");
                    ProvisioningValidator.AlertITAdmin(user, message, EmployeeEvents.OUChange);

                    return false;
                }
                if (user.FullTimeEmployee)
                {
                    var matrix = ADSXLProvisioningMatrix.GetByUser(user);
                    if (matrix != null && !string.IsNullOrWhiteSpace(matrix.OU))
                    {
                        adUserValue.ou = matrix.OU;
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Warning, $"ADSXLProvisioningMatrix is NULL for  the Transfer request-{this.CurrentRequest.Id} user- {user.KnownAs}");
                    }
                }
                else
                {
                    adUserValue.ou = MasterConfigHelper.ADNonEmployeeAccountOU;
                }



                adUserValue.LastModifiedBy = "System";
                adUserValue.LastModifiedDate = DateTime.Now;

                //manager update
                adUserValue.manager = GetRODistinguishedName(user);
                UpdateExtenstionAttribute8(user, adUserValue);
                EndpointConnector.ChangeContainer(adUserValue);

                WriteProvisioningLog(LogLevelType.Info, $"Transfer request- Exisiting AD Groups {string.Join(",", adUserValue.Groups)} Removed Successfully for user {user.KnownAs}");

                //add default groups for the new org.
                if (user.FullTimeEmployee)
                {
                    adUserValue.Groups = new List<string>();
                    adUserValue.Groups = GetAdditionalGroupsByUser(user, adUserValue.EndPointSystemId, false);

                    WriteProvisioningLog(LogLevelType.Info, $"Transfer request- Started Assigning Default AD Groups {string.Join(",", adUserValue.Groups)} for user {user.KnownAs}");

                    bool isAdded = EndpointConnector.AddEntitlement(adUserValue, EntitlementType.Group);
                    if (isAdded)
                    {
                        ADSUserInfo.AddEntitlement(adUserValue.Id, EntitlementType.Group, string.Join(",", adUserValue.Groups));
                        WriteProvisioningLog(LogLevelType.Info, $"Transfer request- Completed Assigning Default AD Groups {string.Join(",", adUserValue.Groups)} for user {user.KnownAs}");
                    }
                }
                adUserValue.Save(MasterConfigHelper.ServiceUser);

                return true;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing the transfer request. {0}", ex.Message), ex);
                return false;
            }
        }

        public override bool Retain(DomainModel.User user, ProvisioningParamCollection fields)
        {
            throw new NotImplementedException();
        }

        public override bool UndoEvent(DomainModel.User user, ProvisioningParamCollection fields)
        {
            bool isUpdated = true;
            try
            {
                var adUserValue = ADSUserInfo.GetById(CurrentRequest.EventUndoRequest.EndpointUserId);

                if (CurrentRequest.EventUndoRequest.EmployeeEventType == EmployeeEvents.Resignation)
                {
                    EnableUser(adUserValue);
                    ReOrg(user, fields);

                    if (user.FullTimeEmployee)
                    {
                        adUserValue.accountExpires = null;
                    }
                    else
                    {
                        adUserValue.accountExpires = user.TerminationDate;
                    }

                    EndpointConnector.PostReactivationProcedure(adUserValue);
                }
                else if (CurrentRequest.EventUndoRequest.EmployeeEventType == EmployeeEvents.OUChange)
                {
                    ReOrg(user, fields);
                    var existingGroups = ADSUserInfo.GetByIdIncludeADSGroups(CurrentRequest.EventUndoRequest.EndpointUserId).ADSGroups.Select(item => item.name).ToList();

                    if (existingGroups.Count > 0)
                    {
                        adUserValue.Groups = existingGroups;

                        EndpointConnector.RemoveEntitlement(adUserValue, EntitlementType.Group);

                        ADSUserInfo.RemoveEntitlement(adUserValue.Id, EntitlementType.Group, string.Join(",", adUserValue.Groups));
                    }
                }

                string groups = CurrentRequest.EventUndoRequest.TechnicalGroups;

                if (!string.IsNullOrEmpty(groups))
                {
                    adUserValue.Groups = UtilityMethods.GetListFromString(groups);
                }

                //Check if user has group add user in AD Groups
                if (adUserValue.Groups != null && adUserValue.Groups.Count > 0)
                {
                    bool isExecutedSuccessfully = EndpointConnector.AddEntitlement(adUserValue, EntitlementType.Group);
                    string grpListStr = string.Join(",", adUserValue.Groups);

                    if (isExecutedSuccessfully)
                    {
                        ADSUserInfo.AddEntitlement(adUserValue.Id, EntitlementType.Group, grpListStr);

                        WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningADUserAddedInToGroup, adUserValue.sAMAccountName, groups, Endpoint), null);
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningADUserAddedInToGroupFailed, adUserValue.sAMAccountName, groups, Endpoint), null);
                    }
                }

            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing the undo request. {0}", ex.Message), ex);
                isUpdated = false;
            }

            return isUpdated;
        }

        public override bool RemediateAttestation(DomainModel.User user, ProvisioningParamCollection fields)
        {
            throw new NotImplementedException();
        }

        bool CreateAccount(ProvisioningParamCollection fields, DomainModel.User user, string groups)
        {

            bool isExecutedSuccessfully = false;
            try
            {

                //Get OU for the user from AD config matrix
                var matrixData = ADSXLProvisioningMatrix.GetByUser(user);

                if (matrixData == null)
                {
                    WriteProvisioningLog(LogLevelType.Error, "Matrix Data is not found for SP", null);
                    return false;
                }

                string ou = matrixData.OU;

                ConnectorBase ADConnector = ConnectorFactory.GetConnector(Endpoint, CurrentRequest);
                var adUsers = ADSUserInfo.GetActiveAccounts(user.Id, Endpoint.Id, AccountType.User);
                if (adUsers.Count > 0)
                {
                    WriteProvisioningLog(LogLevelType.Info, string.Format("User already have an active AD account - {0}. Please apply for a secondary account in {1}.Marking current request completed.", adUsers.FirstOrDefault().sAMAccountName, Endpoint.DisplayName), null);
                    return true;
                }
                var adUserValue = ADSUserInfo.GetLockedAccount(user.Id, Endpoint.Id, AccountType.User);
                if (adUserValue != null)
                {
                    UpdateExtenstionAttribute8(user, adUserValue);
                    if (user.FullTimeEmployee)
                    {
                        adUserValue.ou = ou;
                        //office365user - This will sync with Office 365 
                        adUserValue.postOfficeBox = MasterConfigHelper.ADPostOfficeBoxValue;
                        adUserValue.extensionAttribute10 = user.OrgSection;
                        adUserValue.NeedO365License = true;
                        if (string.IsNullOrWhiteSpace(adUserValue.mail))
                        {
                            if (!string.IsNullOrWhiteSpace(user.EmailId))
                            {
                                WriteProvisioningLog(LogLevelType.Info, $"AccountName:{adUserValue.sAMAccountName} and EmailId from User:{user.EmailId}");
                                adUserValue.mail = user.EmailId;
                            }
                            else
                            {
                                adUserValue.mail = ConstructEmail(user.KnownAs);
                            }
                            WriteProvisioningLog(LogLevelType.Info, $"AccountName:{adUserValue.sAMAccountName} and New EmailId:{adUserValue.mail}");
                        }
                            
                        else
                        {
                            WriteProvisioningLog(LogLevelType.Info, $"AccountName:{adUserValue.sAMAccountName} and Existing EmailId:{adUserValue.mail}");
                        }
                    }
                    else
                    {
                        adUserValue.ou = MasterConfigHelper.ADNonEmployeeAccountOU;
                    }

                    if(!ADConnector.IsUserPropertyExists("OU", adUserValue.ou, null))
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

                    if (isExecutedSuccessfully)
                    {
                        if (user.FullTimeEmployee)
                        {
                            user.NeedEmailUpdateToHR = true;
                        }
                        WriteProvisioningLog(LogLevelType.Info, $"AccountName:{adUserValue.sAMAccountName} and Existing EmailId:{adUserValue.mail}");
                        user.EmailId = adUserValue.mail;

                        user.AuthenticationMode = "AD";
                        user.UserName = adUserValue.sAMAccountName;

                        user.Save(MasterConfigHelper.ServiceUser);

                        // To update in the account Recon for the IDMS user

                        IACMUserInfo iACMUserInfo = IACMUserInfo.GetByEmployeeId(user.EmployeeId);
                        iACMUserInfo.UserName = user.UserName;
                        iACMUserInfo.Save(MasterConfigHelper.ServiceUser);
                        DomainModel.User.DoAfterSaveUser(user, MasterConfigHelper.ServiceUser);

                    }


                    ReOrg(user, adUserValue);

                    adUserValue.NewUsrPassword = ConstructPassword();

                    ADConnector.ChangePassword(adUserValue.sAMAccountName, adUserValue.NewUsrPassword, user.Id, true);

                    adUserValue.accountExpires = CurrentRequest.DeactivationDate == DateTime.MinValue ? null : (DateTime?)CurrentRequest.DeactivationDate;
                    ADConnector.PostReactivationProcedure(adUserValue);

                    bool Result = ProvisioningEmailHelper.SendEmailForAccountProvisioning(CurrentRequest, adUserValue, user, CurrentRequest.Application.ApplicationType);

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
                        adUserValue.NeedO365License = true;
                        adUserValue.mail = ConstructEmail(user.KnownAs);
                    }
                    else
                    {
                        adUserValue.ou = MasterConfigHelper.ADNonEmployeeAccountOU;
                    }
                    if (!ADConnector.IsUserPropertyExists("OU", adUserValue.ou,null))
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

                    UpdateExtenstionAttribute8(user, adUserValue);


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

                        // To update in the account Recon for the IDMS user

                        IACMUserInfo iACMUserInfo = IACMUserInfo.GetByEmployeeId(user.EmployeeId);
                        iACMUserInfo.UserName = user.UserName;
                        iACMUserInfo.Save(MasterConfigHelper.ServiceUser);
                        DomainModel.User.DoAfterSaveUser(user, MasterConfigHelper.ServiceUser);

                        //TODO: Need to change this logic.
                        adUserValue.IsHomeFolderPermissionSetFailed = true;

                        AddADUser(adUserValue);

                        bool Result = ProvisioningEmailHelper.SendEmailForAccountProvisioning(CurrentRequest, adUserValue, user, CurrentRequest.Application.ApplicationType);
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

                    string[] defaultADGrps = groups.Split(',');
                    if (defaultADGrps != null)
                    {
                        foreach (string tmpGrpValue in defaultADGrps)
                        {
                            if (!string.IsNullOrEmpty(tmpGrpValue) && !adUserValue.Groups.Contains(tmpGrpValue.Trim()))
                            {
                                adUserValue.Groups.Add(tmpGrpValue.Trim());
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(fields["Groups"].Value))
                    {
                        foreach (var item in fields["Groups"].Value.Split(','))
                        {
                            if (!string.IsNullOrWhiteSpace(item) && !adUserValue.Groups.Contains(item.Trim()))
                            {
                                adUserValue.Groups.Add(item);
                            }
                        }
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

                            WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningADUserAddedInToGroup, adUserValue.sAMAccountName, grpListStr, Endpoint.Description), null);
                        }
                        else
                        {
                            WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningADUserAddedInToGroupFailed, adUserValue.sAMAccountName, grpListStr, Endpoint.Description), null);
                        }
                    }
                    #endregion

                    WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningADUserCreated, adUserValue.sAMAccountName, Endpoint.Description), null);
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while creating the AD account. {0}", ex.Message), ex);
                return false;
            }

            return isExecutedSuccessfully;
        }

        protected virtual bool DeleteAccount(DomainModel.User user, long endpointUserId)
        {
            try
            {
                var adUser = ADSUserInfo.GetById(endpointUserId);

                bool isSuccess = DisableUser(adUser, LockScenario.AdhocRequest);
                var disabledOU = MasterConfigHelper.ADDisabledAccountsOU;
                if (!EndpointConnector.IsUserPropertyExists("OU", disabledOU, null))
                {
                    throw new Exception($"OU:{ disabledOU} does not exists. Please contact endpoint system owner");
                }
                if (isSuccess)
                {
                    ProvisioningEmailHelper.SendEmailAfterAdhocUserRemoval(user, Endpoint, CurrentRequest, adUser.sAMAccountName);
                    EndpointConnector.MoveOU(adUser, disabledOU);

                }
                return isSuccess;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while deleting the AD account. {0}", ex.Message), ex);
                return false;
            }
        }

        protected virtual bool AmendentAdd(DomainModel.User user, long endpointUserId, string groups)
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

                    ProvisioningUtility.ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentAdd(user, CurrentRequest, grpListStr, adUserValue.sAMAccountName);
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
                return false;
            }
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

                    ProvisioningUtility.ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentRemove(user, CurrentRequest, grpListStr, adUserValue.sAMAccountName);
                    WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningADUserRemovedFromGroup, adUserValue.sAMAccountName, grpListStr, Endpoint.DisplayName), null);
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningADUserRemoveFromGroupFailed, adUserValue.sAMAccountName, grpListStr, Endpoint.DisplayName), null);
                }

                return isExecutedSuccessfully;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while removing groups from the AD account. {0}", ex.Message), ex);
                return false;
            }
        }

        protected bool ReOrg(DomainModel.User user, ADSUserInfo adUserValue)
        {
            //set new ou values to AD user object
            adUserValue.title = user.PositionName;
            adUserValue.sn = user.KnownAs;
            adUserValue.displayName = user.KnownAs;

            if (!user.FullTimeEmployee)
            {
                adUserValue.description = user.ProjectDescription;
            }
            else
            {
                adUserValue.description = user.KnownAs;
            }

            adUserValue.department = user.OrgDepartment;
            adUserValue.division = user.OrgDivision;
            adUserValue.company = user.OrgCompany;
            adUserValue.businessCategory = user.OrgBranch;
            adUserValue.pager = user.OrgSection;

            adUserValue.postalAddress = user.PostalAddress;
            adUserValue.postalCode = user.PostalCode;
            adUserValue.countryCode = user.Country;

            adUserValue.telephoneNumber = user.TelephoneNumber;
            adUserValue.mobile = user.MobileNumber;
            adUserValue.facsimileTelephoneNumber = user.Fax;
            adUserValue.mail = user.EmailId;
            adUserValue.physicalDeliveryOfficeName = user.EmployeeId;

            adUserValue.otherPager = user.Reserved8;
            adUserValue.extensionAttribute6 = user.CostCenter;

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

            adUserValue.LastModifiedBy = "System";
            adUserValue.LastModifiedDate = DateTime.Now;

            if (user.FullTimeEmployee)
            {
                var matrix = ADSXLProvisioningMatrix.GetByUser(user);
                if (matrix != null && !string.IsNullOrWhiteSpace(matrix.OU))
                {
                    adUserValue.ou = matrix.OU;
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.Error, $"ADSXLProvisioningMatrix is NULL for  the ReOrg request-{this.CurrentRequest.Id} user- {user.KnownAs}");
                }
            }
            else
            {
                adUserValue.ou = MasterConfigHelper.ADNonEmployeeAccountOU;
            }

            if (!EndpointConnector.IsUserPropertyExists("OU", adUserValue.ou, null))
            {
                throw new Exception($"OU:{ adUserValue.ou} does not exists. Please contact endpoint system owner");
            }
            //manager update
            adUserValue.manager = GetRODistinguishedName(user);

            EndpointConnector.ChangeContainer(adUserValue);

            adUserValue.Save(MasterConfigHelper.ServiceUser);

            //add default groups for the new org.
            if (user.FullTimeEmployee)
            {
                adUserValue.Groups = GetAdditionalGroupsByUser(user, adUserValue.EndPointSystemId, true);

                bool isAdded = EndpointConnector.AddEntitlement(adUserValue, EntitlementType.Group);
                if (isAdded)
                {
                    ADSUserInfo.AddEntitlement(adUserValue.Id, EntitlementType.Group, string.Join(",", adUserValue.Groups));
                }
            }

            return true;
        }

        protected string GetRODistinguishedName(DomainModel.User user)
        {
            try
            {
                if (!string.IsNullOrEmpty(user.ROEmployeeId))
                {
                    var ro = DomainModel.User.GetByEmployeeId(user.ROEmployeeId);

                    if (ro != null)
                    {
                        var roAD = ADSUserInfo.GetActivePrimaryByDIUserId(ro.Id, Endpoint.Id);

                        if (roAD != null)
                        {
                            return roAD.distinguishedName;
                        }
                        else
                        {
                            return string.Empty;
                        }
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while finding RO AD distinguished name for the User {0}.", user.DisplayName), ex);
                return string.Empty;
            }
        }

        protected List<string> GetAdditionalGroupsByUser(DomainModel.User user, long endPointSystemId, bool isReOrg)
        {
            List<string> groups = new List<string>();

            var matrix = ADSXLProvisioningMatrix.GetByUser(user);


            string[] groupsFromMatrix = matrix != null ? matrix.DefaultGroups.Split(',') : null;

            if (!isReOrg)
            {
                //Get default groups for the user from AD config matrix
                string[] executiveEmployeeGroups = MasterConfigHelper.ExecutiveEmployeeADGroups;

                if (executiveEmployeeGroups != null)
                {
                    foreach (string grp in executiveEmployeeGroups)
                    {
                        if (!string.IsNullOrEmpty(grp))
                        {
                            groups.Add(grp.Trim());
                        }
                    }
                }
            }

            if (groupsFromMatrix != null)
            {
                foreach (string grp in groupsFromMatrix)
                {
                    if (!string.IsNullOrEmpty(grp))
                    {
                        groups.Add(grp.Trim());
                    }
                }
            }

            return groups;
        }

        protected string ConstructEmail(string UName)
        {

            string retString = string.Empty;

            string spaceRemovedString = RemoveSplCharsWithEmptyString(UName);

            int len = spaceRemovedString.Length;

            if (len <= 12)
            {
                retString = spaceRemovedString.ToLower();
            }
            else
            {
                if (UName.Contains(" "))
                {
                    string firstWord = UName.Split(' ').FirstOrDefault();

                    string remainingWords = UName.Substring(firstWord.Length + 1);

                    var v = remainingWords.Split(' ').Select(y => y[0]).ToArray();

                    string initials = string.Join("", v);

                    retString = string.Format("{0}{1}", firstWord, initials).ToLower();

                    retString = RemoveSplCharsWithEmptyString(retString);
                }
                else
                {
                    retString = UName.Substring(0, 12).ToLower();
                    retString = RemoveSplCharsWithEmptyString(retString);
                }
            }
            return CheckMailAddressExists(retString);
        }

        string CheckMailAddressExists(string nickName)
        {
            string email = nickName;
            string domain = MasterConfigHelper.O365EmailDomainName;
            var ep = EndPoint.GetByApplicationType(ApplicationType.Office365);
            var tempEmail = email + "@" + domain;
            int no = 1;
           
            bool isExist = AccountReconcilation.IsEmailIdAlreadyExists(ep.Id, tempEmail);
           
            while (isExist == true)
            {
                WriteProvisioningLog(LogLevelType.Warning, $"EmailId:{tempEmail} already exists. Hence try regenerate the new EmailId.");
                email = nickName;

                email = string.Format("{0}{1}", email, no.ToString());
                tempEmail = email + "@" + domain;
                isExist = AccountReconcilation.IsEmailIdAlreadyExists(ep.Id, tempEmail);
                no++;
            }

            return email + "@" + domain;
        }

        string RemoveSplCharsWithEmptyString(string UserName)
        {
            UserName = Regex.Replace(UserName, "[^0-9A-Za-z]+", "");
            return UserName;
        }

        private void UpdateExtenstionAttribute8(DI.Chakra.DomainModel.User user, ADSUserInfo adUserValue)
        {
            try
            {
                if (user != null && !string.IsNullOrWhiteSpace(user.OrganisationCode) && adUserValue != null && this.CurrentRequest != null && this.CurrentRequest.AccountType == AccountType.User)
                {
                    var org = NOrganisation.GetByOrgCode(user.OrganisationCode);
                    if (org != null && org.ScopeId.HasValue)
                    {
                        var orgHead = NOrganisationHead.GetByOrganisationIdAndHead(org.Id, user.Id);
                        var scope = NOrganisationScope.GetById(org.ScopeId.Value);
                        if (orgHead != 0 && scope != null)
                        {
                            adUserValue.extensionAttribute8 = scope.Scope;
                        }
                        else
                        {
                            adUserValue.extensionAttribute8 = null;
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }
}
