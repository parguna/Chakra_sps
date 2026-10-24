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
using System.Reflection;
using System.Text;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.User
{
    public class NPAC : FormExecutorBase
    {
        public const int UserNameLength = 40;
        public const int DesignationLength = 40;
        string _userName = string.Empty;
        public NPAC(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(idmsLogger)
        {
            Endpoint = endpoint;
            CurrentRequest = currentRequest;
        }

        protected string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }
        string GetCompanyName(DomainModel.User user)
        {
            return (user.FullTimeEmployee || user.EmployeeType == EmployeeType.TempStaff )? user.CompanyName : user.NCompany;
        }
        string GetSectionName(DomainModel.User user)
        {
            return (user.FullTimeEmployee || user.EmployeeType == EmployeeType.TempStaff) ? user.SectionName : string.Empty;
        }
        public override bool Execute(ProvisioningParamCollection fields, DomainModel.User user)
        {
            bool isExecutedSuccessfully = false;
            //DateTime maxDate = new DateTime(2999, 12, 31);
            string npacUserDefaultValidTo = DateTime.MaxValue.ToString("yyyy.MM.dd"); //"9999.12.31"; // maxDate.ToString("yyyy.MM.dd"); // 

            AdHocNPACRequest npacDetail = null;
            long endpointUserId = 0;

            if (user == null)
            {
                WriteProvisioningLog(LogLevelType.Warning, "NPAC Provisioning failed. User object is null");
                return false;
            }

            //if adhoc request, get prov details from adhoc request object
            string leveldetail = string.Empty;
            if (CurrentRequest.AdHocAccessRequestId.HasValue)
            {
                npacDetail = (AdHocNPACRequest)CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();
                if (npacDetail != null)
                {
                    endpointUserId = npacDetail.EndpointUserId;
                    leveldetail = npacDetail.Entitlement;
                }
            }

            if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                isExecutedSuccessfully = CreateAccount(fields, user, leveldetail, npacDetail);
            }
            else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount)
            {
                isExecutedSuccessfully = DeleteAccount(user, endpointUserId);
            }
            else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
            {
                isExecutedSuccessfully = AmendentAdd(user, endpointUserId, npacDetail, fields);
            }
            else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                isExecutedSuccessfully = AmendentRemove(user, endpointUserId, leveldetail);
            }

            return isExecutedSuccessfully;
        }
        //public override bool OnBoard(ProvisioningParamCollection fields, DomainModel.User user)
        //{
        //    return true;
        //}
        public override bool OffBoard(DomainModel.User user, ProvisioningParamCollection fields)
        {

            try
            {
                var endpointUser = NPACUserInfo.GetPrimaryByDIUserId(user.Id, this.Endpoint.Id);
                if (endpointUser != null)
                {
                    endpointUser = NPACUserInfo.GetByIdIncludeLevels(endpointUser.Id);
                }

                this.TakeOUSnapshot(user.Id, endpointUser, EmployeeEvents.Resignation, EndpointType.NPAC, EntitlementType.Level);
                return this.LockUser(endpointUser, user, LockScenario.OffBoard);
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing offboarding request. {0}", ex.Message), ex);
                return false;
            }
        }
        public override bool Propagate(DomainModel.User user, ProvisioningParamCollection fields)
        {
            if (CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared)
                return true;
            bool isUpdated = false;


            var npacUsr = NPACUserInfo.GetPrimaryByDIUserId(user.Id, this.Endpoint.Id);
            if (npacUsr != null)
            {
                if ((user.FullTimeEmployee || user.EmployeeType == EmployeeType.TempStaff) && string.IsNullOrWhiteSpace(user.OrganisationCode))
                {
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, $"  User {npacUsr.UserName} OrganisationCode) not found.", null);
                    return isUpdated;
                };
                if((user.FullTimeEmployee || user.EmployeeType == EmployeeType.TempStaff))
                {
                    NPACSectionInfo npacSectionInfo = null;
                    if(!string.IsNullOrWhiteSpace(user.SectionCode) && !user.SectionCode.Equals("00000000"))
                        npacSectionInfo = NPACSectionInfo.GetNPACSectionByCode(user.SectionCode);
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(user.OrganisationCode))
                        {
                            npacSectionInfo = NPACSectionInfo.GetNPACSectionByCode(user.OrganisationCode);
                        }
                    }

                    if(npacSectionInfo == null || string.IsNullOrWhiteSpace(npacSectionInfo.NPACSection))
                    {
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, $"NPAC  User {npacUsr.UserName} sectionCode not found in NPAC section.", null);
                        return isUpdated;
                    }
                    npacUsr.Section = npacSectionInfo.NPACSection;
                }
                npacUsr.Emp_Id = user.EmployeeId;
                npacUsr.UserName = user.KnownAs.ToUpper();
                npacUsr.Phone = user.TelephoneNumber;
                npacUsr.UserADID = user.UserName; //ADID              
                npacUsr.CompanyName = GetCompanyName(user); //Company Name
                npacUsr.SectionName = GetSectionName(user); //Company Name

                if (!string.IsNullOrWhiteSpace(npacUsr.UserName) && npacUsr.UserName.Length > UserNameLength)
                    npacUsr.UserName = npacUsr.UserName.Remove(UserNameLength);

                if (!string.IsNullOrWhiteSpace(user.EmailId))
                {
                    npacUsr.EmailId = user.EmailId;
                }
               
                if (user.FullTimeEmployee == true)
                {
                    if (!string.IsNullOrWhiteSpace(user.PositionName))
                        npacUsr.Designation = user.PositionName;
                }
                else
                {
                    npacUsr.Designation = MasterConfigHelper.NPACNonEmployeeDesignation;
                }

                if (npacUsr.Designation != null && npacUsr.Designation.Length > DesignationLength)
                    npacUsr.Designation = npacUsr.Designation.Remove(DesignationLength);

                //if pager empty then only update
                if (string.IsNullOrWhiteSpace(npacUsr.Pager))
                {
                    if (!string.IsNullOrWhiteSpace(user.MobileNumber))
                    {
                        if (user.MobileNumber.Trim().Length > 12)
                            npacUsr.Pager = user.MobileNumber.Trim().Substring(user.MobileNumber.Trim().Length - 12, 12);
                        else
                            npacUsr.Pager = user.MobileNumber.Trim();
                    }
                }

                isUpdated = EndpointConnector.UpdateAccount(npacUsr);

                if (isUpdated)
                {
                    npacUsr.Save(MasterConfigHelper.ServiceUser);
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, string.Format("  User {0} modified  in NPAC.", npacUsr.UserName), null);
                }
            }
            else
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest?.Id, Endpoint?.Id, LogLevelType.Info, $"No Account found for Propagation.");
                isUpdated = true;
            }
            return isUpdated;

        }
        public override bool ReOrg(DomainModel.User user, ProvisioningParamCollection fields)
        {
            var npacUsr = NPACUserInfo.GetPrimaryByDIUserId(user.Id, this.Endpoint.Id);
            if (npacUsr != null)
            {
                if (user.FullTimeEmployee == true)
                {
                    if (!string.IsNullOrEmpty(user.PositionName))
                        npacUsr.Designation = user.PositionName;
                }
                else
                {
                    npacUsr.Designation = MasterConfigHelper.NPACNonEmployeeDesignation;
                }

                if (npacUsr.Designation != null && npacUsr.Designation.Length > DesignationLength)
                    npacUsr.Designation = npacUsr.Designation.Remove(DesignationLength);

                npacUsr.CompanyName = GetCompanyName(user);
                npacUsr.SectionName = GetSectionName(user);
                //if (user.FullTimeEmployee == true)
                //{
                //    if (!string.IsNullOrEmpty(user.OrgSection))
                //        npacUsr.Section = NPACSectionInfo.GetNPACSectionCodeFromHRSectionName(user.SectionName);
                //    else
                //        npacUsr.Section = string.Empty;
                //}
                //else
                //{
                //    npacUsr.Section = MasterConfigHelper.NPACNonEmployeeSection;
                //}

                bool isChangeContianerSucess = EndpointConnector.ChangeContainer(npacUsr);

                npacUsr.Save("System");

                this._idmsLogger.WriteLog(LogLevelType.Info, string.Format("  User {0} modified  in NPAC.", npacUsr.UserName), null);
            }

            return true;
        }
        public override bool Transfer(DomainModel.User user, ProvisioningParamCollection fields)
        {
            try
            {

                NPACUserInfo npacUserValue = NPACUserInfo.GetPrimaryByDIUserId(user.Id, this.Endpoint.Id);

                if (null == npacUserValue)
                    return true;

                this.TakeOUSnapshot(user.Id, npacUserValue, EmployeeEvents.OUChange, EndpointType.NPAC, EntitlementType.Level);

                this.LockUser(npacUserValue, user, LockScenario.OUChange);

                //npacFacade.UpdateNPACUserInfoFromProvisioning(npacUserValue);
                this._idmsLogger.WriteLog(LogLevelType.Info, string.Format("  User {0} modified  in NPAC.", npacUserValue.UserName), null);

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
                NPACUserInfo npacUserValue = null;

                long endpointUserId = 0;
                string entitlement = string.Empty;

                //if adhoc request, get prov details from adhoc request object
                if (CurrentRequest.AdHocAccessRequest != null)
                {
                    var npacDetail = (AdHocNPACRequest)CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();

                    if (npacDetail != null)
                    {
                        //Entitlement field holds tech role/group for attestation remediation 
                        entitlement = npacDetail.Entitlement;
                        endpointUserId = npacDetail.EndpointUserId;
                    }
                }

                if (endpointUserId != 0)
                {
                    npacUserValue = NPACUserInfo.GetByIdIncludeLevels(endpointUserId);
                }

                if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    isExecutedSuccessfully = this.LockUser(npacUserValue, user, LockScenario.AttestationRemediation);
                }
                else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    npacUserValue.Roles = entitlement.Split(',').ToList();
                    isExecutedSuccessfully = EndpointConnector.RemoveEntitlement(npacUserValue, EntitlementType.Role);
                    NPACUserInfo.DeleteNPACUserLEVELMap(npacUserValue, entitlement);
                }

                if (isExecutedSuccessfully)
                {
                    ProvisioningEmailHelper.SendEmailAfterAttestationRemediation(user, CurrentRequest, entitlement, npacUserValue.AccountName);
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing access request - {0}.", CurrentRequest.Id), ex);
                return false;
            }

            return isExecutedSuccessfully;
        }
        public override bool Extension(DomainModel.User user, ProvisioningParamCollection fields, AccountType accountType, long accountReconId = 0)
        {
            LogDebugStep($"Extension Endpoint: {Endpoint.DisplayName}");

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
                            var npacUserInfo = NPACUserInfo.GetById(accountRecon.EndpointUserId);

                            if (npacUserInfo != null)
                            {
                                npacUserInfo.Invalid_Date = user.TerminationDate.Value;

                                npacUserInfo.LastModifiedBy = "System";
                                npacUserInfo.LastModifiedDate = DateTime.Now;

                                isUpdated = EndpointConnector.ExtendValidity(npacUserInfo);

                                if (isUpdated)
                                {
                                    npacUserInfo.Save(MasterConfigHelper.ServiceUser);
                                    base.Extension(user, fields, CurrentRequest.AccountType, accountRecon.Id);
                                    WriteProvisioningLog(LogLevelType.Info, string.Format("Validity of User {0} is extended from {1} to {2} in Active Directory.", npacUserInfo.UserName, oldTerminationDate, user.TerminationDate), null);
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

        public override bool UndoEvent(DomainModel.User user, ProvisioningParamCollection fields)
        {
            bool isUpdated = true;
            try
            {

                var npacUsr = NPACUserInfo.GetById(this.CurrentRequest.EventUndoRequest.EndpointUserId);
                if(npacUsr == null)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"Undo: NPAC User Not found.");
                }
                if (this.CurrentRequest.EventUndoRequest.EmployeeEventType == EmployeeEvents.Resignation ||
                    this.CurrentRequest.EventUndoRequest.EmployeeEventType == EmployeeEvents.OUChange)
                {
                    var entitlements = this.CurrentRequest.EventUndoRequest.TechnicalGroups;
                    string roles = string.Empty;
                    var strMsg = $"UNODO: NPAC User:{npacUsr.UserName} Section and Level is empty. Cannot Unlock. Please contact administrator";
                    if (string.IsNullOrWhiteSpace(entitlements))
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, strMsg);
                        return true;
                    }
                    if (!string.IsNullOrWhiteSpace(entitlements))
                    {
                        var details = entitlements.Split('|');
                        npacUsr.Section = details[0].Split(':')[1]?.Trim();
                        roles = details[1].Split(':')[1]?.Trim();
                        _idmsLogger.WriteLog(LogLevelType.Info, $"UNODO: NPAC User:{npacUsr.UserName} Section:{npacUsr.Section} and Level:{roles}");
                        if (string.IsNullOrWhiteSpace(npacUsr.Section) && string.IsNullOrWhiteSpace(roles))
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, strMsg);
                            return true;
                        }
                    }
                   
                    UnlockUser(npacUsr, user);
                    this.ReOrg(user, fields);

                    if (!string.IsNullOrEmpty(roles))
                    {
                        var tempRoles = UtilityMethods.GetListFromString(roles);
                        npacUsr.Roles = tempRoles.Distinct().ToList();

                        bool isAdded = EndpointConnector.AddEntitlement(npacUsr, EntitlementType.Level);
                        if (isAdded)
                        {
                            NPACUserInfo.InsertNPACUserLevel(npacUsr);
                            _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Level - {0} added successfully for the user {1}.", roles, npacUsr.AccountName), null);
                        }
                        else
                        {
                            _idmsLogger.WriteLog(LogLevelType.Error, string.Format("Error while adding level - {0} for the user {1}.", roles, npacUsr.AccountName), null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isUpdated = false;
                _idmsLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
            }
            return isUpdated;
        }
        bool CreateAccount(ProvisioningParamCollection fields, DomainModel.User user, string npacRoles, AdHocNPACRequest npacDetail)
        {
            bool isNPACAcntCreated = false;
            try
            {
                string npacUserDefaultValidTo = DateTime.MaxValue.ToString("yyyy.MM.dd"); //"9999.12.31"; // maxDate.ToString("yyyy.MM.dd"); // 
                var npacUserValue = NPACUserInfo.GetLockedPrimaryByDIUserId(user.Id, Endpoint.Id);
                string pwd = ConstructPassword();
                if (npacUserValue != null)
                {
                    isNPACAcntCreated = this.UnlockUser(npacUserValue, user);

                    if (isNPACAcntCreated)
                    {
                        CreateUserInReconTable(npacUserValue, MasterConfigHelper.ServiceUser);

                        string newUserId = this.ValidateUserNameDuringReActivation(user);
                        EndpointConnector.UpdateUserIdentity(npacUserValue.UserIdentity, newUserId);
                        npacUserValue.UserIdentity = newUserId;
                        npacUserValue.Save("System");


                        npacUserValue.Encrypt_Pwd = pwd;
                        EndpointConnector.ChangePassword(npacUserValue.UserIdentity, npacUserValue.Encrypt_Pwd, user.Id, true);

                        npacUserValue.Fax = !string.IsNullOrEmpty(npacUserValue.Fax) ? npacUserValue.Fax : string.Empty;
                        npacUserValue.Grade = string.Empty;
                        npacUserValue.Zone = npacDetail.Unit;//Unit from AccessRequest                   
                        npacUserValue.Section = npacDetail.Section; //Section from AccessRequest                    
                        npacUserValue.Pager = npacDetail.PhoneNumber;//PhoneNumber from AccessRequest
                        npacUserValue.UserADID = user.UserName; //ADID
                        npacUserValue.EmailId = npacDetail.Email; //EmailId
                        npacUserValue.CompanyName = GetCompanyName(user); //Company Name
                        npacUserValue.SectionName = GetSectionName(user); //Company Name
                        npacUserValue.UserName = user.KnownAs.ToUpper();//Known As to Capital    

                        if (!string.IsNullOrWhiteSpace(npacUserValue.UserName) && npacUserValue.UserName.Length > UserNameLength)
                            npacUserValue.UserName = npacUserValue.UserName.Remove(UserNameLength);

                        bool isSuccess = EndpointConnector.UpdateAccount(npacUserValue);

                        if (isSuccess)
                        {
                            ProvisioningEmailHelper.SendEmailForAccountProvisioning(CurrentRequest, npacUserValue, user, ApplicationType.NPAC, pwd);
                            //TODO:V5 will not have any matrix. Hence commented the below code.
                            //if (string.IsNullOrEmpty(npacUserValue.Section) || string.IsNullOrEmpty(npacUserValue.Designation))
                            //    ProvisioningEmailHelper.SendEmailForSectionOrDesignationEmpty(npacUserValue.UserIdentity, user.SectionName, user.PositionName);

                            List<string> npcLevels = new List<string>();

                            npacUserValue.Roles = new List<string>();
                            if (!string.IsNullOrEmpty(npacRoles))
                            {
                                List<string> tempLevls = npacRoles.Split(',').ToList();
                                foreach (var item in tempLevls)
                                {
                                    npacUserValue.Roles.Add(item);
                                }
                            }

                            EndpointConnector.AddEntitlement(npacUserValue, EntitlementType.Level);
                            NPACUserInfo.InsertNPACUserLevel(npacUserValue);
                        }
                    }
                }
                else
                {
                    npacUserValue = new NPACUserInfo();

                    #region SET NPAC User Data
                    foreach (ProvisioningParam gData in fields)
                    {
                        npacUserValue = setUserInfoByAtrbValue(npacUserValue, user, gData) as NPACUserInfo;
                    }

                    npacUserValue.EndPointSystemId = this.Endpoint.Id;
                    //Ticket 3929
                    npacUserValue.UserIdentity = user.UserName;
                    if (string.IsNullOrEmpty(npacUserValue.UserName))
                    {
                        this._idmsLogger.WriteLog(LogLevelType.Info, $"NPAC UserName Empty for the accessRequestId {this.CurrentRequest.Id}", null);
                        return false;
                    }

                    npacUserValue.Emp_Id = user.EmployeeId;
                    if (npacUserValue.Emp_Id != null && npacUserValue.Emp_Id.Length > 9)
                        npacUserValue.Emp_Id = npacUserValue.Emp_Id.Remove(9);

                    string userName;
                    StringBuilder userNameSB = new StringBuilder();
                    userName = removeSpecialCharacters(npacUserValue.UserName, false, false, user.Ethinicity);

                    userNameSB.Append(userName);
                    userNameSB.Replace("Mohamad", "mohd");
                    userNameSB.Replace("Mohamed", "mohd");
                    userNameSB.Replace("Mohammad", "mohd");
                    userNameSB.Replace("Mohammed", "mohd");

                    userNameSB.Replace("Muhamad", "muhd");
                    userNameSB.Replace("Muhamed", "muhd");
                    userNameSB.Replace("Muhammad", "muhd");
                    userNameSB.Replace("Muhammed", "muhd");

                    userName = userNameSB.ToString();
                    userNameSB.Clear();
                    npacUserValue.UserName = userName;



                    if (!string.IsNullOrWhiteSpace(npacUserValue.UserName) && npacUserValue.UserName.Length > UserNameLength)
                        npacUserValue.UserName = npacUserValue.UserName.Remove(UserNameLength);

                    if (!string.IsNullOrWhiteSpace(npacUserValue.Phone) && npacUserValue.Phone.Length > 12)
                        npacUserValue.Phone = npacUserValue.Phone.Remove(12);


                    if (user.FullTimeEmployee == true)
                    {
                        if (!string.IsNullOrEmpty(user.PositionName))
                            npacUserValue.Designation = user.PositionName;
                        else
                            npacUserValue.Designation = string.Empty;

                        this._idmsLogger.WriteLog(LogLevelType.Info, $"NPAC User Designation: {npacUserValue.Designation}", null);
                    }
                    else
                    {
                        npacUserValue.Designation = MasterConfigHelper.NPACNonEmployeeDesignation;
                    }

                    if (npacUserValue.Designation != null && npacUserValue.Designation.Length > DesignationLength)
                        npacUserValue.Designation = npacUserValue.Designation.Remove(DesignationLength);

                    npacUserValue.Encrypt_Pwd = pwd;
                    npacUserValue.CreatedDate = DateTime.Now;
                    npacUserValue.LastModifiedDate = DateTime.Now;
                    npacUserValue.Create_Date = DateTime.Now;
                    npacUserValue.CreatedBy = MasterConfigHelper.ServiceUser;
                    npacUserValue.LastModifiedBy = MasterConfigHelper.ServiceUser;

                    if (user.FullTimeEmployee == true)
                        npacUserValue.Invalid_Date = DateTime.Today.AddDays(7);
                    else
                        npacUserValue.Invalid_Date = user.TerminationDate;

                    npacUserValue.Fax = !string.IsNullOrEmpty(npacUserValue.Fax) ? npacUserValue.Fax : string.Empty;
                    npacUserValue.Grade = string.Empty;
                    npacUserValue.Zone = npacDetail.Unit;//Unit from AccessRequest                   
                    npacUserValue.Section = !string.IsNullOrWhiteSpace(npacDetail.Section) ? npacDetail.Section.Trim() : string.Empty; //Section from AccessRequest                    
                    npacUserValue.Pager = npacDetail.PhoneNumber;//PhoneNumber from AccessRequest
                    npacUserValue.UserADID = user.UserName; //ADID
                    npacUserValue.EmailId = npacDetail.Email; //EmailId
                    npacUserValue.CompanyName = GetCompanyName(user); //Company Name
                    npacUserValue.SectionName = GetSectionName(user); //Company Name
                    npacUserValue.UserName = user.KnownAs.ToUpper();//Known As to Capital
                    if (npacUserValue.Pager != null && npacUserValue.Pager.Length > 12)
                        npacUserValue.Pager = npacUserValue.Pager.Remove(12);

                    //NPAC Levels
                    List<string> npcLevels = new List<string>();

                    npacUserValue.Roles = new List<string>();
                    if (!string.IsNullOrEmpty(npacRoles))
                    {
                        List<string> tempLevls = npacRoles.Split(',').ToList();
                        foreach (var item in tempLevls)
                        {
                            npacUserValue.Roles.Add(item);
                        }
                    }


                    if (CurrentRequest.ActivationDate != null && CurrentRequest.ActivationDate != DateTime.MinValue)
                    {
                        npacUserValue.UserValidFrom = this.getNPACDateFormat((DateTime)CurrentRequest.ActivationDate);
                    }
                    else
                    {
                        npacUserValue.UserValidFrom = this.getNPACDateFormat(DateTime.Now);
                    }

                    if (CurrentRequest.DeactivationDate != null && CurrentRequest.DeactivationDate != DateTime.MinValue)
                    {
                        npacUserValue.UserValidTo = this.getNPACDateFormat((DateTime)CurrentRequest.DeactivationDate);
                    }
                    else
                    {
                        npacUserValue.UserValidTo = npacUserDefaultValidTo;
                    }
                    #endregion

                    isNPACAcntCreated = EndpointConnector.CreateAccount(npacUserValue, fields);

                    if (isNPACAcntCreated)
                    {
                        ProvisioningEmailHelper.SendEmailForAccountProvisioning(CurrentRequest, npacUserValue, user, ApplicationType.NPAC,pwd);
                        //TODO: Commented below code because V5 dont have matrix
                        //if (string.IsNullOrEmpty(npacUserValue.Section) || string.IsNullOrEmpty(npacUserValue.Designation))
                        //    ProvisioningEmailHelper.SendEmailForSectionOrDesignationEmpty(npacUserValue.UserIdentity, user.SectionName, user.PositionName);

                        //Save User and Create Account Recon Entry
                        npacUserValue.Save(MasterConfigHelper.ServiceUser);
                        CreateUserInReconTable(npacUserValue, MasterConfigHelper.ServiceUser);


                        EndpointConnector.AddEntitlement(npacUserValue, EntitlementType.Level);
                        NPACUserInfo.InsertNPACUserLevel(npacUserValue);

                        if (CurrentRequest.AdHocAccessRequestId.HasValue)
                        {
                            this._idmsLogger.WriteLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningNPACUserCreated, npacUserValue.UserName, this.Endpoint.Id), null);
                        }
                        else
                        {
                            this._idmsLogger.WriteLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningNPACUserCreated, npacUserValue.UserName, this.Endpoint.Id), null);
                        }

                    }
                    else
                    {
                        this._idmsLogger.WriteLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningNPACUserCreationFailed, npacUserValue.UserName, this.Endpoint.Id), null);
                        return false;
                    }

                    return isNPACAcntCreated;
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, ex.Message, ex);
                return isNPACAcntCreated;
            }

            return isNPACAcntCreated;
        }
        bool DeleteAccount(DomainModel.User user, long endpointUserId)
        {
            bool isRemovedSuccessfully = false;
            try
            {
                var npacUserValue = NPACUserInfo.GetByIdIncludeLevels(endpointUserId);
                if (npacUserValue != null)
                {
                    npacUserValue.Levels = null;
                    npacUserValue.Section = null;

                    isRemovedSuccessfully = this.LockUser(npacUserValue, user, LockScenario.AdhocRequest);

                    if (isRemovedSuccessfully)
                    {
                        this._idmsLogger.WriteLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningNPACUserDeleted, npacUserValue.UserName, npacUserValue.EndPointSystemId), null);
                    }
                    else
                    {
                        this._idmsLogger.WriteLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningNPACUserDeletionFailed, npacUserValue.UserName, npacUserValue.EndPointSystemId), null);
                    }
                }
                else
                {
                    this._idmsLogger.WriteLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_FormUserNull, "NPAC"), null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while deleting the NPAC account. {0}", ex.Message), ex);
                return false;
            }

            return isRemovedSuccessfully;
        }
        bool AmendentAdd(DomainModel.User user, long endpointUserId, AdHocNPACRequest npacDetails, ProvisioningParamCollection fields)
        {
            try
            {
                string npacUserDefaultValidTo = DateTime.MaxValue.ToString("yyyy.MM.dd"); //"9999.12.31"; // maxDate.ToString("yyyy.MM.dd"); // 

                var npacUserValue = NPACUserInfo.GetById(endpointUserId);

                if (npacUserValue == null)
                {
                    this._idmsLogger.WriteLog(LogLevelType.Error, string.Format("Endpoint User not found for the  Access Request -{0}, Endpoint System -{1}", this.CurrentRequest.Id, this.Endpoint.DisplayName), null);
                    return false;
                }

                #region fillData
                //NPAC User Levels
                List<string> npcLevels = new List<string>();
                npacUserValue.Roles = new List<string>();
                if (!string.IsNullOrEmpty(npacDetails.Entitlement))
                {
                    foreach (var item in npacDetails.Entitlement.Split(','))
                    {
                        npcLevels.Add(item);
                    }

                    npacUserValue.Roles = npcLevels.Distinct().ToList();
                }

                npacUserValue.Zone = npacDetails.Unit;//Unit from AccessRequest
                npacUserValue.Section = npacDetails.Section;
                if ((user.FullTimeEmployee || user.EmployeeType == EmployeeType.TempStaff))
                {
                    NPACSectionInfo npacSectionInfo = null;
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, $"NPAC  User {npacUserValue.UserName} User Section:{user.SectionCode}.", null);
                    if (!string.IsNullOrWhiteSpace(user.SectionCode) && !user.SectionCode.Equals("00000000"))
                        npacSectionInfo = NPACSectionInfo.GetNPACSectionByCode(user.SectionCode);
                    else
                    {
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, $"NPAC  User {npacUserValue.UserName} User dont have sectioncode hence check for OrganisationCode:{user.OrganisationCode} in NPAC section.", null);
                        if (!string.IsNullOrWhiteSpace(user.OrganisationCode))
                        {
                            npacSectionInfo = NPACSectionInfo.GetNPACSectionByCode(user.OrganisationCode);
                        }
                    }

                    if (npacSectionInfo == null || string.IsNullOrWhiteSpace(npacSectionInfo.NPACSection))
                    {
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, $"NPAC  User {npacUserValue.UserName} sectionCode not found in NPAC sectionCode.", null);
                        npacUserValue.Section = npacDetails.Section; //Section from AccessRequest
                    }
                    else
                    {
                        npacUserValue.Section = npacSectionInfo.NPACSection;
                    }
                }
                                    
                npacUserValue.Pager = npacDetails.PhoneNumber;//PhoneNumber from AccessRequest
                npacUserValue.UserADID = user.UserName; //ADID
                npacUserValue.EmailId = npacDetails.Email; //EmailId
                npacUserValue.CompanyName = GetCompanyName(user); //Company Name
                npacUserValue.SectionName = GetSectionName(user); //Company Name
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, $"NPAC  User {npacUserValue.UserName} SectionName: {npacUserValue.SectionName}" +
                    $"Section: {npacUserValue.Section}", null);
                //Valid From and Valid To
                if (CurrentRequest.ActivationDate != null && CurrentRequest.ActivationDate != DateTime.MinValue)
                {
                    npacUserValue.UserValidFrom = getNPACDateFormat((DateTime)CurrentRequest.ActivationDate);
                }
                else
                {
                    npacUserValue.UserValidFrom = getNPACDateFormat(DateTime.Now);
                }

                if (CurrentRequest.DeactivationDate != null && CurrentRequest.DeactivationDate != DateTime.MinValue)
                {
                    npacUserValue.UserValidTo = getNPACDateFormat((DateTime)CurrentRequest.DeactivationDate);
                }
                else
                {
                    npacUserValue.UserValidTo = npacUserDefaultValidTo;
                }
                #endregion

                #region Provisioning Levels
                bool islevelsAdded = false;
                if (npacUserValue.Roles.Count() > 0)
                {
                    bool isUpdated = EndpointConnector.UpdateAccount(npacUserValue);
                    if (isUpdated)
                    {
                        npacUserValue.Save("System");
                        islevelsAdded = EndpointConnector.AddEntitlement(npacUserValue, EntitlementType.Level);

                        string npacLevls = string.Join(",", npacUserValue.Roles);
                        if (islevelsAdded)
                        {
                            NPACUserInfo.InsertNPACUserLevel(npacUserValue);

                            ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentAdd(user, this.CurrentRequest, npacLevls, npacUserValue.AccountName);

                            if (this.CurrentRequest.AdHocAccessRequestId.HasValue)
                            {
                                this._idmsLogger.WriteLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningNPACUserLevelAssigned, npacUserValue.UserName, npacLevls, this.Endpoint.Id), null);
                            }
                            else
                            {
                                this._idmsLogger.WriteLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningNPACUserLevelAssigned, npacUserValue.UserName, npacLevls, this.Endpoint.Id), null);
                            }
                        }
                        else
                        {
                            this._idmsLogger.WriteLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningNPACUserLevelAssignmentFailed, npacUserValue.UserName, npacLevls, this.Endpoint.Id), null);
                            return false;
                        }
                    }
                    else
                    {
                        this._idmsLogger.WriteLog(LogLevelType.Error, $"NPAC Update account failed for user {npacUserValue.UserName}", null);
                        return false;
                    }
                }
                else
                {
                    this._idmsLogger.WriteLog(LogLevelType.Error, string.Format("User Levels empty for the Access Request -{0}, User -{1}", this.CurrentRequest.Id, npacUserValue.UserName), null);
                    return false;
                }
                #endregion
                return islevelsAdded;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while adding Levels to the NPAC account. {0}", ex.Message), ex);
                return false;
            }
        }
        bool AmendentRemove(DomainModel.User user, long endpointUserId, string npacLevels)
        {
            try
            {

                string npacUserDefaultValidTo = DateTime.MaxValue.ToString("yyyy.MM.dd"); //"9999.12.31"; // maxDate.ToString("yyyy.MM.dd"); // 

                var npacUserValue = NPACUserInfo.GetById(endpointUserId);

                if (npacUserValue == null)
                {
                    this._idmsLogger.WriteLog(LogLevelType.Error, string.Format("Endpoint User not found for the  Access Request -{0}, Endpoint System -{1}", this.CurrentRequest.Id, this.Endpoint.DisplayName), null);
                    return false;
                }

                #region fillData
                //NPAC User Levels
                List<string> npcLevels = new List<string>();
                npacUserValue.Roles = new List<string>();
                if (!string.IsNullOrEmpty(npacLevels))
                {
                    foreach (var item in npacLevels.Split(','))
                    {
                        npcLevels.Add(item);
                    }

                    npacUserValue.Roles = npcLevels.Distinct().ToList();
                }

                //Valid From and Valid To
                if (CurrentRequest.ActivationDate != null && CurrentRequest.ActivationDate != DateTime.MinValue)
                {
                    npacUserValue.UserValidFrom = getNPACDateFormat((DateTime)CurrentRequest.ActivationDate);
                }
                else
                {
                    npacUserValue.UserValidFrom = getNPACDateFormat(DateTime.Now);
                }

                if (CurrentRequest.DeactivationDate != null && CurrentRequest.DeactivationDate != DateTime.MinValue)
                {
                    npacUserValue.UserValidTo = getNPACDateFormat((DateTime)CurrentRequest.DeactivationDate);
                }
                else
                {
                    npacUserValue.UserValidTo = npacUserDefaultValidTo;
                }
                #endregion

                #region Provisioning Levels
                bool islevelsRemoved = false;
                if (npacUserValue.Roles.Count() > 0)
                {
                    islevelsRemoved = EndpointConnector.RemoveEntitlement(npacUserValue, EntitlementType.Level);

                    if (islevelsRemoved)
                    {
                        NPACUserInfo.DeleteNPACUserLEVELMap(npacUserValue, npacLevels);

                        ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentRemove(user, this.CurrentRequest, npacLevels, npacUserValue.AccountName);

                        this._idmsLogger.WriteLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningNPACUserLevelRemoved, npacUserValue.UserName, npacLevels, this.Endpoint.Id), null);

                    }
                    else
                    {
                        this._idmsLogger.WriteLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningNPACUserLevelRemoveFailed, npacUserValue.UserName, npacLevels, this.Endpoint.Id), null);
                        return false;
                    }
                }
                else
                {
                    this._idmsLogger.WriteLog(LogLevelType.Error, string.Format("User Levels empty for the Access Request -{0}, User -{1}", this.CurrentRequest.Id, npacUserValue.UserName), null);
                    return false;
                }
                #endregion

                return true;

            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while removing groups from the AD account. {0}", ex.Message), ex);
                return false;
            }
        }
        private bool LockUser(NPACUserInfo endpointUser, DomainModel.User user, LockScenario lockScenario)
        {
            bool isusrDisabled = true;

            if (endpointUser != null)
            {
                DateTime invalidDate = DateTime.Today;

                endpointUser.Invalid_Date = invalidDate;

                endpointUser.Levels = null;
                endpointUser.Section = null;

                bool islevelsRemoved = EndpointConnector.RemoveAllEntitlements(endpointUser);
                if (islevelsRemoved)
                {
                    string npacLevels = string.Join(",", endpointUser.NPACLevelInfos.Select(l => l.Level.ToString()));
                    if (!string.IsNullOrWhiteSpace(npacLevels))
                        NPACUserInfo.DeleteNPACUserLEVELMap(endpointUser, npacLevels);
                }

                isusrDisabled = EndpointConnector.LockAccount(endpointUser);
                if (isusrDisabled)
                {
                    endpointUser.LastModifiedBy = "System";
                    endpointUser.LastModifiedDate = DateTime.Now;
                    endpointUser.Save("System");
                    if (isusrDisabled)
                    {
                        this._idmsLogger.WriteLog(LogLevelType.Info, string.Format("  User {0} Disabled in NPAC.", endpointUser.UserName), null);
                    }
                }
            }
            return isusrDisabled;
        }
        private bool UnlockUser(NPACUserInfo npacUsr, DomainModel.User user)
        {
            bool isusrUnlocked = true;

            if (npacUsr != null)
            {
                isusrUnlocked = false;

                npacUsr.Invalid_Date = DateTime.Today.AddDays(7);

                if (user.TerminationDate.HasValue)
                {
                    npacUsr.Invalid_Date = user.TerminationDate.Value;
                }

                isusrUnlocked = EndpointConnector.ExtendValidity(npacUsr);

                if (isusrUnlocked)
                {
                    npacUsr.LastModifiedBy = "System";
                    npacUsr.LastModifiedDate = DateTime.Now;

                    npacUsr.Save("System");
                    this._idmsLogger.WriteLog(LogLevelType.Info, string.Format("  User {0} extended in NPAC.", npacUsr.UserName), null);
                }
            }

            return isusrUnlocked;
        }
        private string getNPACDateFormat(DateTime tmpDate)
        {
            string dateStr = string.Empty;
            try
            {
                string yearStr = tmpDate.Year.ToString();
                string monthStr = tmpDate.Month < 10 ? "0" + tmpDate.Month.ToString() : tmpDate.Month.ToString();
                string dayStr = tmpDate.Day < 10 ? "0" + tmpDate.Day.ToString() : tmpDate.Day.ToString();

                dateStr = string.Format(ProvisioningConstants.GEMSDateFormater, yearStr, monthStr, dayStr);
            }
            catch
            { }

            return dateStr;
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

            if (!string.IsNullOrEmpty(userName)) // && userName.Length >= 3)
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
            if (!string.IsNullOrEmpty(user.IdentificationNo) && user.IdentificationNo.Length >= 5)
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
        private string constructUserPassword(ProvisioningParamCollection fields)
        {
            if (!string.IsNullOrEmpty(fields["PasswordExpression"].Value) && fields["PasswordExpression"].Value.Length > 0)
                return fields["PasswordExpression"].Value;

            string passwd = ConstructPassword();
            return passwd;
        }
        protected override string ConstructUserName(DomainModel.User user)
        {
            string UserName = null;
            if (user == null)
            {
                throw new Exception("DI User is NULL in ConstructUserName");

            }
            var accountRecons = AccountReconcilation.GetByAccountTypeAndApplicationType(user.Id, AccountType.User, ApplicationType.ADSingaporePowerLocal);
            if (accountRecons != null && accountRecons.Count > 0)
            {
                var account = accountRecons.FirstOrDefault(u => u.IsLocked == false && u.IsDeleted == false);
                if (account != null)
                    UserName = account.AccountName;
            }
            if (string.IsNullOrWhiteSpace(UserName) && user.FullTimeEmployee)
            {
                var str = $"The user does not have AD User account in {UtilityMethods.GetEnumAttribute<Description>(ApplicationType.ADSingaporePowerLocal)}. Thus IDMS cannot be proceed provisioning the request - {CurrentRequest.Id}.";
                var ex = new Exception(str);
                WriteProvisioningLog(LogLevelType.Exception, str, ex);
                throw ex;
            }

            if (string.IsNullOrWhiteSpace(UserName) && !user.FullTimeEmployee)
            {
                UserName = base.ConstructUserName(user);
            }

            return UserName;
        }
    }
}
