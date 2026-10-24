using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.ProvisioningUtility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.User
{
    public class SSIDS : FormExecutorBase
    {
        public SSIDS(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(idmsLogger)
        {
            Endpoint = endpoint;
            CurrentRequest = currentRequest;
        }

        public override bool Execute(ProvisioningParamCollection fields, DomainModel.User user)
        {
            bool isExecutedSuccessfully = false;

            AdHocSSIDSRequest ssidsDetail = null;
            long endpointUserId = 0;

            if (user == null)
            {
                WriteProvisioningLog(LogLevelType.Warning, "SSIDS Provisioning failed. User object is null");
                return false;
            }

            //if adhoc request, get prov details from adhoc request object
            string roledetail = string.Empty;
            if (CurrentRequest.AdHocAccessRequestId.HasValue)
            {
                ssidsDetail = (AdHocSSIDSRequest)CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();
                if (ssidsDetail != null)
                {
                    endpointUserId = ssidsDetail.EndpointUserId;
                    roledetail = ssidsDetail.Entitlement;
                }
            }

            if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                isExecutedSuccessfully = CreateAccount(fields, user, roledetail, ssidsDetail);
            }
            else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount)
            {
                isExecutedSuccessfully = DeleteAccount(user, endpointUserId);
            }
            else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
            {
                isExecutedSuccessfully = AmendentAdd(user, endpointUserId, roledetail, fields);
            }
            else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                isExecutedSuccessfully = AmendentRemove(user, endpointUserId, roledetail);
            }

            return isExecutedSuccessfully;
        }

        public override bool OffBoard(DomainModel.User user, ProvisioningParamCollection fields)
        {
            try
            {
                var endpointUser = SSIDSUserInfo.GetPrimaryByDIUserId(user.Id, this.Endpoint.Id);
                if (endpointUser != null)
                {
                    endpointUser = SSIDSUserInfo.GetByIdIncludeRoles(endpointUser.Id);
                }
                if (this.Endpoint.ApplicationType == ApplicationType.SSIDS_GUI)
                {
                    this.TakeOUSnapshot(user.Id, endpointUser, EmployeeEvents.Resignation, EndpointType.SSIDS, EntitlementType.Role);
                }
                else if (this.Endpoint.ApplicationType == ApplicationType.SSIDS_SMS)
                {
                    this.TakeOUSnapshot(user.Id, endpointUser, EmployeeEvents.Resignation, EndpointType.SSIDS, EntitlementType.HandPhone);
                }
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

            bool isRemovedSuccessfully = false;
            try
            {
                var ssidsUserValue = SSIDSUserInfo.GetPrimaryByDIUserId(user.Id, this.Endpoint.Id);
                if (ssidsUserValue != null)
                {
                    ssidsUserValue.Emp_Id = user.EmployeeId;
                    ssidsUserValue.Company = user.OrgCompany;
                    ssidsUserValue.Department = user.OrgDepartment;
                    ssidsUserValue.Branch = user.OrgBranch;
                    ssidsUserValue.Section = user.SectionName;
                    ssidsUserValue.OrgUnit = user.OrgUnit;
                    ssidsUserValue.OfficeTelePhone = user.TelephoneNumber;
                    ssidsUserValue.Designation = user.PositionName;
                    ssidsUserValue.Email_Id = !string.IsNullOrWhiteSpace(user.EmailId) ? user.EmailId : null;
                    string mobileNumber = user.MobileNumber;
                    if (Endpoint.ApplicationType == ApplicationType.SSIDS_SMS)
                    {
                        ssidsUserValue.UserName = user.KnownAs;
                        var entitlementRecon = EntitlementReconcilation.GetByUserId(user.Id, Endpoint.Id).FirstOrDefault();
                        if (entitlementRecon != null)
                            mobileNumber = entitlementRecon.Entitlement;
                    }

                    ssidsUserValue.HandPhone = mobileNumber;

                    isRemovedSuccessfully = EndpointConnector.UpdateAccount(ssidsUserValue);

                    if (isRemovedSuccessfully)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"SSIDS Update Account {ssidsUserValue.UserName} is successfull.");
                    }
                    else
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"SSIDS Update Account {ssidsUserValue.UserName} is failed.");
                    }
                }
                else
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_FormUserNull, "SSIDS"), null);
                    return true;
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while Propagation the SSIDS account. {0}", ex.Message), ex);
                return false;
            }

            return isRemovedSuccessfully;

        }

        public override bool ReOrg(DomainModel.User user, ProvisioningParamCollection fields)
        {
            return Propagate(user, fields);
        }

        public override bool Transfer(DomainModel.User user, ProvisioningParamCollection fields)
        {
            bool isRemovedSuccessfully = false;
            try
            {
                var ssidsUserValue = SSIDSUserInfo.GetPrimaryByDIUserId(user.Id, this.Endpoint.Id);
                if (ssidsUserValue != null)
                {
                    if (this.Endpoint.ApplicationType == ApplicationType.SSIDS_GUI)
                    {
                        this.TakeOUSnapshot(user.Id, ssidsUserValue, EmployeeEvents.OUChange, EndpointType.SSIDS, EntitlementType.Role);
                    }
                    else if (this.Endpoint.ApplicationType == ApplicationType.SSIDS_SMS)
                    {
                        this.TakeOUSnapshot(user.Id, ssidsUserValue, EmployeeEvents.OUChange, EndpointType.SSIDS, EntitlementType.HandPhone);
                    }
                    isRemovedSuccessfully = this.LockUser(ssidsUserValue, user, LockScenario.AdhocRequest);

                    if (isRemovedSuccessfully)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSSIDSUserDeleted, ssidsUserValue.UserName, ssidsUserValue.EndPointSystemId), null);
                    }
                    else
                    {
                        _idmsLogger.WriteLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningSSIDSUserDeletionFailed, ssidsUserValue.UserName, ssidsUserValue.EndPointSystemId), null);
                    }
                }
                else
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_FormUserNull, "SSIDS"), null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while Transfer/deleting the SSIDS account. {0}", ex.Message), ex);
                return false;
            }

            return isRemovedSuccessfully;
        }

        public override bool Retain(DomainModel.User user, ProvisioningParamCollection fields)
        {
            throw new NotImplementedException();
        }

        public override bool RemediateAttestation(DomainModel.User user, ProvisioningParamCollection fields)
        {
            //bool isExecutedSuccessfully = true;

            //try
            //{
            //    //if adhoc request, get prov details from adhoc request object
            //    if (CurrentRequest.AdHocAccessRequest != null)
            //    {
            //        var npacDetail = (AdHocNPACRequest)CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();

            //        if (npacDetail != null)
            //        {
            //            //Entitlement field holds tech role/group for attestation remediation 
            //            entitlement = npacDetail.Entitlement;
            //            endpointUserId = npacDetail.EndpointUserId;
            //        }
            //    }

            //    if (isExecutedSuccessfully)
            //    {
            //        ProvisioningEmailHelper.SendEmailAfterAttestationRemediation(user, CurrentRequest, entitlement, npacUserValue.AccountName);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing access request - {0}.", CurrentRequest.Id), ex);
            //    return false;
            //}

            //return isExecutedSuccessfully;

            return true;
        }

        public override bool UndoEvent(DomainModel.User user, ProvisioningParamCollection fields)
        {
            bool isAcntCreated = true;
            try
            {
                var currentUser = MasterConfigHelper.ServiceUser;
                _idmsLogger.WriteLog(LogLevelType.DebugStep, $"{this.Endpoint.DisplayName} UndoEvent started");
                var ssidsUserValue = SSIDSUserInfo.GetLockedPrimaryByDIUserId(user.Id, Endpoint.Id);
                _idmsLogger.WriteLog(LogLevelType.DebugStep, $"{this.Endpoint.DisplayName} UndoEvent User Object found:{ssidsUserValue != null}");
                if (ssidsUserValue != null)
                {
                    #region fill basic info
                    foreach (ProvisioningParam gData in fields)
                    {
                        ssidsUserValue = setUserInfoByAtrbValue(ssidsUserValue, user, gData) as SSIDSUserInfo;
                    }
                    ssidsUserValue.User_Id = user.UserName;
                    ssidsUserValue.EndPointSystemId = this.Endpoint.Id;
                    ssidsUserValue.Emp_Id = user.EmployeeId;
                    ssidsUserValue.UserName = user.KnownAs;
                    ssidsUserValue.Company = user.OrgCompany;
                    ssidsUserValue.Department = user.OrgDepartment;
                    ssidsUserValue.Branch = user.OrgBranch;
                    ssidsUserValue.Section = user.SectionName;
                    ssidsUserValue.OrgUnit = user.OrgUnit;
                    ssidsUserValue.OfficeTelePhone = user.TelephoneNumber;
                    ssidsUserValue.Designation = user.PositionName;
                    ssidsUserValue.Email_Id = !string.IsNullOrWhiteSpace(user.EmailId) ? user.EmailId : null;
                    _idmsLogger.WriteLog(LogLevelType.Info, $"Before Update : Handphone: {ssidsUserValue.HandPhone}");

                    if (ssidsUserValue.HandPhone != null && ssidsUserValue.HandPhone.Length > 8)
                        ssidsUserValue.HandPhone = ssidsUserValue.HandPhone.Remove(8);

                    if (ssidsUserValue.OfficeTelePhone != null && ssidsUserValue.OfficeTelePhone.Length > 15)
                        ssidsUserValue.OfficeTelePhone = ssidsUserValue.OfficeTelePhone.Remove(15);
                    #endregion

                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"{this.Endpoint.DisplayName} UnlockUser User :{ssidsUserValue.AccountName} started");
                    isAcntCreated = this.UnlockUser(ssidsUserValue, user);
                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"{this.Endpoint.DisplayName} UnlockUser User :{ssidsUserValue.AccountName} Successfull:{isAcntCreated}");
                    if (isAcntCreated)
                    {
                        _idmsLogger.WriteLog(LogLevelType.DebugStep, $"{this.Endpoint.DisplayName} UnlockUser User :{ssidsUserValue.AccountName} Activate User");
                        CreateUserInReconTable(ssidsUserValue, currentUser);
                        ssidsUserValue.LastModifiedBy = currentUser;
                        ssidsUserValue.LastModifiedDate = DateTime.Now;
                        ssidsUserValue.Save(currentUser);
                        _idmsLogger.WriteLog(LogLevelType.DebugStep, $"{this.Endpoint.DisplayName} UnlockUser User :{ssidsUserValue.AccountName}Get snapshot");
                        _idmsLogger.WriteLog(LogLevelType.DebugStep, $"{this.Endpoint.DisplayName} UnlockUser User snapshot roles :{this.CurrentRequest.EventUndoRequest.TechnicalRoles}");
                        if (!string.IsNullOrWhiteSpace(this.CurrentRequest.EventUndoRequest.TechnicalRoles))
                        {
                            List<string> roles = new List<string>();
                            ssidsUserValue.Roles = new List<string>();
                            if (Endpoint.ApplicationType == ApplicationType.SSIDS_GUI)
                            {
                                List<string> tempLevls = this.CurrentRequest.EventUndoRequest.TechnicalRoles.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                foreach (var item in tempLevls)
                                {
                                    ssidsUserValue.Roles.Add(item);
                                }
                            }
                            else
                            {
                                ssidsUserValue.HandPhone = ssidsUserValue.Roles.First().Trim();
                            }
                            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"{this.Endpoint.DisplayName} UnlockUser User AddEntitlement started");
                            var success = EndpointConnector.AddEntitlement(ssidsUserValue, Endpoint.ApplicationType == ApplicationType.SSIDS_GUI ? EntitlementType.Role : EntitlementType.HandPhone);
                            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"{this.Endpoint.DisplayName} UnlockUser User AddEntitlement Success:{success}");
                            SSIDSUserInfo.InsertUserRole(ssidsUserValue);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"UndoEvent {ex.Message}", ex);
            }
            return isAcntCreated;
        }

        bool CreateAccount(ProvisioningParamCollection fields, DomainModel.User user, string ssidsRoles, AdHocSSIDSRequest ssidsEntDetail)
        {
            bool isAcntCreated = false;
            try
            {
                var ssidsUserValue = SSIDSUserInfo.GetLockedPrimaryByDIUserId(user.Id, Endpoint.Id);

                if (ssidsUserValue != null)
                {
                    foreach (ProvisioningParam gData in fields)
                    {
                        ssidsUserValue = setUserInfoByAtrbValue(ssidsUserValue, user, gData) as SSIDSUserInfo;
                    }
                    ssidsUserValue.User_Id = user.UserName;
                    ssidsUserValue.EndPointSystemId = this.Endpoint.Id;
                    ssidsUserValue.Emp_Id = user.EmployeeId;
                    ssidsUserValue.UserName = user.KnownAs;
                    ssidsUserValue.Company = user.OrgCompany;
                    ssidsUserValue.Department = user.OrgDepartment;
                    ssidsUserValue.Branch = user.OrgBranch;
                    ssidsUserValue.Section = user.SectionName;
                    ssidsUserValue.OrgUnit = user.OrgUnit;
                    ssidsUserValue.OfficeTelePhone = user.TelephoneNumber;
                    ssidsUserValue.Designation = user.PositionName;
                    ssidsUserValue.Email_Id = !string.IsNullOrWhiteSpace(user.EmailId) ? user.EmailId : null;
                    if (!string.IsNullOrWhiteSpace(user.MobileNumber))
                    {
                        if (user.MobileNumber.Trim().Length > 8)
                            ssidsUserValue.HandPhone = user.MobileNumber.Trim().Substring(user.MobileNumber.Trim().Length - 8, 8);
                        else
                            ssidsUserValue.HandPhone = user.MobileNumber.Trim();
                    }

                    if (!string.IsNullOrWhiteSpace(user.TelephoneNumber))
                    {
                        if (user.TelephoneNumber.Trim().Length > 8)
                            ssidsUserValue.OfficeTelePhone = user.TelephoneNumber.Trim().Substring(user.TelephoneNumber.Trim().Length - 8, 8);
                        else
                            ssidsUserValue.OfficeTelePhone = user.TelephoneNumber.Trim();
                    }

                    if (Endpoint.ApplicationType == ApplicationType.SSIDS_SMS && ssidsUserValue.Roles != null && ssidsUserValue.Roles.Count > 0)
                    {
                        ssidsUserValue.HandPhone = ssidsUserValue.Roles.First().Trim();
                    }

                    _idmsLogger.WriteLog(LogLevelType.Info, $"Before Update : Handphone: {ssidsUserValue.HandPhone}");

                    if (ssidsUserValue.HandPhone != null && ssidsUserValue.HandPhone.Length > 8)
                        ssidsUserValue.HandPhone = ssidsUserValue.HandPhone.Remove(8);

                    if (ssidsUserValue.OfficeTelePhone != null && ssidsUserValue.OfficeTelePhone.Length > 15)
                        ssidsUserValue.OfficeTelePhone = ssidsUserValue.OfficeTelePhone.Remove(15);

                    isAcntCreated = this.UnlockUser(ssidsUserValue, user);

                    if (isAcntCreated)
                    {
                        CreateUserInReconTable(ssidsUserValue, MasterConfigHelper.ServiceUser);
                        ProvisioningEmailHelper.SendEmailForAccountProvisioning(CurrentRequest, ssidsUserValue, user, this.Endpoint.ApplicationType);

                        List<string> roles = new List<string>();
                        ssidsUserValue.Roles = new List<string>();

                        if (!string.IsNullOrEmpty(ssidsRoles))
                        {
                            List<string> tempLevls = ssidsRoles.Split(',').ToList();
                            foreach (var item in tempLevls)
                            {
                                ssidsUserValue.Roles.Add(item);
                            }
                        }

                        if (Endpoint.ApplicationType == ApplicationType.SSIDS_SMS && ssidsUserValue.Roles != null && ssidsUserValue.Roles.Count > 0)
                        {
                            ssidsUserValue.HandPhone = ssidsUserValue.Roles.First().Trim();
                        }

                        EndpointConnector.AddEntitlement(ssidsUserValue, Endpoint.ApplicationType == ApplicationType.SSIDS_GUI ? EntitlementType.Role : EntitlementType.HandPhone);
                        SSIDSUserInfo.InsertUserRole(ssidsUserValue);
                    }
                }
                else
                {
                    ssidsUserValue = new SSIDSUserInfo();

                    #region SET SSIDS User Data
                    foreach (ProvisioningParam gData in fields)
                    {
                        ssidsUserValue = setUserInfoByAtrbValue(ssidsUserValue, user, gData) as SSIDSUserInfo;
                    }

                    ssidsUserValue.EndPointSystemId = this.Endpoint.Id;
                    ssidsUserValue.Emp_Id = user.EmployeeId;
                    ssidsUserValue.User_Id = user.UserName;
                    ssidsUserValue.UserName = user.KnownAs;
                    ssidsUserValue.Company = user.OrgCompany;
                    ssidsUserValue.Department = user.OrgDepartment;
                    ssidsUserValue.Branch = user.OrgBranch;
                    ssidsUserValue.Section = user.SectionName;
                    ssidsUserValue.OrgUnit = user.OrgUnit;
                    ssidsUserValue.OfficeTelePhone = user.TelephoneNumber;
                    ssidsUserValue.Designation = user.PositionName;
                    ssidsUserValue.Email_Id = !string.IsNullOrWhiteSpace(user.EmailId) ? user.EmailId : null;
                    if (!string.IsNullOrWhiteSpace(user.MobileNumber))
                    {
                        if (user.MobileNumber.Trim().Length > 8)
                            ssidsUserValue.HandPhone = user.MobileNumber.Trim().Substring(user.MobileNumber.Trim().Length - 8, 8);
                        else
                            ssidsUserValue.HandPhone = user.MobileNumber.Trim();
                    }

                    if (!string.IsNullOrWhiteSpace(user.TelephoneNumber))
                    {
                        if (user.TelephoneNumber.Trim().Length > 8)
                            ssidsUserValue.OfficeTelePhone = user.TelephoneNumber.Trim().Substring(user.TelephoneNumber.Trim().Length - 8, 8);
                        else
                            ssidsUserValue.OfficeTelePhone = user.TelephoneNumber.Trim();
                    }

                    if (string.IsNullOrWhiteSpace(ssidsUserValue.User_Id))
                    {
                        _idmsLogger.WriteLog(LogLevelType.Warning, $"User Name is NULL");
                        return false;
                    }

                    ssidsUserValue.Roles = new List<string>();
                    if (!string.IsNullOrEmpty(ssidsRoles))
                    {
                        List<string> tempLevls = ssidsRoles.Split(',').ToList();
                        foreach (var item in tempLevls)
                        {
                            ssidsUserValue.Roles.Add(item);
                        }
                    }

                    if (Endpoint.ApplicationType == ApplicationType.SSIDS_SMS && ssidsUserValue.Roles != null && ssidsUserValue.Roles.Count > 0)
                    {
                        ssidsUserValue.HandPhone = ssidsUserValue.Roles.First().Trim();
                    }

                    _idmsLogger.WriteLog(LogLevelType.Info, $"Before Update : Handphone: {ssidsUserValue.HandPhone}");

                    //if (ssidsUserValue.User_Id != null && ssidsUserValue.User_Id.Length > 9)
                    //    ssidsUserValue.User_Id = ssidsUserValue.User_Id.Remove(9);

                    if (ssidsUserValue.HandPhone != null && ssidsUserValue.HandPhone.Length > 8)
                        ssidsUserValue.HandPhone = ssidsUserValue.HandPhone.Remove(8);

                    if (ssidsUserValue.OfficeTelePhone != null && ssidsUserValue.OfficeTelePhone.Length > 15)
                        ssidsUserValue.OfficeTelePhone = ssidsUserValue.OfficeTelePhone.Remove(15);


                    ssidsUserValue.CreatedDate = DateTime.Now;
                    ssidsUserValue.LastModifiedDate = DateTime.Now;
                    ssidsUserValue.CreatedBy = MasterConfigHelper.ServiceUser;
                    ssidsUserValue.LastModifiedBy = MasterConfigHelper.ServiceUser;

                    #endregion

                    isAcntCreated = EndpointConnector.CreateAccount(ssidsUserValue, fields);

                    if (isAcntCreated)
                    {
                        ProvisioningEmailHelper.SendEmailForAccountProvisioning(CurrentRequest, ssidsUserValue, user, this.Endpoint.ApplicationType);

                        //Save User and Create Account Recon Entry
                        ssidsUserValue.Save(MasterConfigHelper.ServiceUser);
                        CreateUserInReconTable(ssidsUserValue, MasterConfigHelper.ServiceUser);

                        EndpointConnector.AddEntitlement(ssidsUserValue, Endpoint.ApplicationType == ApplicationType.SSIDS_GUI ? EntitlementType.Role : EntitlementType.HandPhone);
                        SSIDSUserInfo.InsertUserRole(ssidsUserValue);

                        if (CurrentRequest.AdHocAccessRequestId.HasValue)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSSIDSUserCreated, ssidsUserValue.UserName, this.Endpoint.Id), null);
                        }
                        else
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSSIDSUserCreated, ssidsUserValue.UserName, this.Endpoint.Id), null);
                        }

                    }
                    else
                    {
                        _idmsLogger.WriteLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningSSIDSUserCreationFailed, ssidsUserValue.UserName, this.Endpoint.Id), null);
                        return false;
                    }

                    return isAcntCreated;
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, ex.Message, ex);
                return isAcntCreated;
            }

            return isAcntCreated;
        }

        bool DeleteAccount(DomainModel.User user, long endpointUserId)
        {
            bool isRemovedSuccessfully = false;
            try
            {
                var ssidsUserValue = SSIDSUserInfo.GetByIdIncludeRoles(endpointUserId);
                if (ssidsUserValue != null)
                {
                    isRemovedSuccessfully = this.LockUser(ssidsUserValue, user, LockScenario.AdhocRequest);

                    if (isRemovedSuccessfully)
                    {
                        ProvisioningEmailHelper.SendEmailAfterAdhocUserRemoval(user, Endpoint, CurrentRequest, user.AccountName);
                        _idmsLogger.WriteLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSSIDSUserDeleted, ssidsUserValue.UserName, ssidsUserValue.EndPointSystemId), null);
                    }
                    else
                    {
                        _idmsLogger.WriteLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningSSIDSUserDeletionFailed, ssidsUserValue.UserName, ssidsUserValue.EndPointSystemId), null);
                    }
                }
                else
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_FormUserNull, "NPAC"), null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while deleting the SSIDS account. {0}", ex.Message), ex);
                return false;
            }

            return isRemovedSuccessfully;
        }

        bool AmendentAdd(DomainModel.User user, long endpointUserId, string ssidsroles, ProvisioningParamCollection fields)
        {
            try
            {
                var msg = Endpoint.ApplicationType == ApplicationType.SSIDS_GUI ? $"Role" : $"HandPhone";
                var ssidsUserValue = SSIDSUserInfo.GetById(endpointUserId);

                if (ssidsUserValue == null)
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, string.Format("Endpoint User not found for the  Access Request -{0}, Endpoint System -{1}", this.CurrentRequest.Id, this.Endpoint.DisplayName), null);
                    return false;
                }
                #region fillData
                foreach (ProvisioningParam gData in fields)
                {
                    ssidsUserValue = setUserInfoByAtrbValue(ssidsUserValue, user, gData) as SSIDSUserInfo;
                }

                ssidsUserValue.EndPointSystemId = this.Endpoint.Id;
                ssidsUserValue.Emp_Id = user.EmployeeId;
                ssidsUserValue.User_Id = user.UserName;
                ssidsUserValue.UserName = user.KnownAs;
                ssidsUserValue.Company = user.OrgCompany;
                ssidsUserValue.Department = user.OrgDepartment;
                ssidsUserValue.Branch = user.OrgBranch;
                ssidsUserValue.Section = user.SectionName;
                ssidsUserValue.OrgUnit = user.OrgUnit;
                ssidsUserValue.OfficeTelePhone = user.TelephoneNumber;
                ssidsUserValue.Designation = user.PositionName;
                ssidsUserValue.Email_Id = !string.IsNullOrWhiteSpace(user.EmailId) ? user.EmailId : null;
                if (!string.IsNullOrWhiteSpace(user.MobileNumber))
                {
                    if (user.MobileNumber.Trim().Length > 8)
                        ssidsUserValue.HandPhone = user.MobileNumber.Trim().Substring(user.MobileNumber.Trim().Length - 8, 8);
                    else
                        ssidsUserValue.HandPhone = user.MobileNumber.Trim();
                }

                if (!string.IsNullOrWhiteSpace(user.TelephoneNumber))
                {
                    if (user.TelephoneNumber.Trim().Length > 8)
                        ssidsUserValue.OfficeTelePhone = user.TelephoneNumber.Trim().Substring(user.TelephoneNumber.Trim().Length - 8, 8);
                    else
                        ssidsUserValue.OfficeTelePhone = user.TelephoneNumber.Trim();
                }

                if (string.IsNullOrWhiteSpace(ssidsUserValue.User_Id))
                {
                    _idmsLogger.WriteLog(LogLevelType.Warning, $"User Name is NULL");
                    return false;
                }

                

                //if (ssidsUserValue.User_Id != null && ssidsUserValue.User_Id.Length > 9)
                //    ssidsUserValue.User_Id = ssidsUserValue.User_Id.Remove(9);

                if (ssidsUserValue.OfficeTelePhone != null && ssidsUserValue.OfficeTelePhone.Length > 15)
                    ssidsUserValue.OfficeTelePhone = ssidsUserValue.OfficeTelePhone.Remove(15);


                List<string> roles = new List<string>();
                ssidsUserValue.Roles = new List<string>();
                if (!string.IsNullOrEmpty(ssidsroles))
                {
                    foreach (var item in ssidsroles.Split(','))
                    {
                        roles.Add(item);
                    }

                    ssidsUserValue.Roles = roles.Distinct().ToList();
                }

                if (Endpoint.ApplicationType == ApplicationType.SSIDS_SMS && ssidsUserValue.Roles != null && ssidsUserValue.Roles.Count > 0)
                {
                    ssidsUserValue.HandPhone = ssidsUserValue.Roles.First().Trim();
                }
                if (ssidsUserValue.HandPhone != null && ssidsUserValue.HandPhone.Length > 8)
                    ssidsUserValue.HandPhone = ssidsUserValue.HandPhone.Remove(8);

                _idmsLogger.WriteLog(LogLevelType.Info, $"Handphone: {ssidsUserValue.HandPhone}");
                #endregion

                #region Provisioning Levels
                bool islevelsAdded = false;
                if (ssidsUserValue.Roles.Count() > 0)
                {
                    islevelsAdded = EndpointConnector.AddEntitlement(ssidsUserValue, Endpoint.ApplicationType == ApplicationType.SSIDS_GUI ? EntitlementType.Role : EntitlementType.HandPhone);

                    if (islevelsAdded)
                    {
                        SSIDSUserInfo.DeleteAllUserRoleMaps(ssidsUserValue.Id);
                        SSIDSUserInfo.InsertUserRole(ssidsUserValue);

                        ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentAdd(user, this.CurrentRequest, ssidsroles, ssidsUserValue.AccountName);

                        if (this.CurrentRequest.AdHocAccessRequestId.HasValue)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSSIDSUserLevelAssigned, ssidsUserValue.UserName, ssidsroles, this.Endpoint.Id, msg), null);
                        }
                        else
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSSIDSUserLevelAssigned, ssidsUserValue.UserName, ssidsroles, this.Endpoint.Id, msg), null);
                        }
                    }
                    else
                    {
                        _idmsLogger.WriteLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningSSIDSUserLevelAssignmentFailed, ssidsUserValue.UserName, ssidsroles, this.Endpoint.Id, msg), null);
                        return false;
                    }
                }
                else
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, string.Format("User {2} empty for the Access Request -{0}, User -{1}", this.CurrentRequest.Id, ssidsUserValue.UserName, msg), null);
                    return false;
                }
                #endregion
                return islevelsAdded;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while adding groups to the AD account. {0}", ex.Message), ex);
                return false;
            }
        }

        bool AmendentRemove(DomainModel.User user, long endpointUserId, string ssidsRoles)
        {
            try
            {
                var msg = Endpoint.ApplicationType == ApplicationType.SSIDS_GUI ? $"Role" : $"HandPhone";
                var ssidsUserValue = SSIDSUserInfo.GetById(endpointUserId);

                if (ssidsUserValue == null)
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, string.Format("Endpoint User not found for the  Access Request -{0}, Endpoint System -{1}", this.CurrentRequest.Id, this.Endpoint.DisplayName), null);
                    return false;
                }

                #region fillData

                List<string> roles = new List<string>();
                ssidsUserValue.Roles = new List<string>();
                if (!string.IsNullOrEmpty(ssidsRoles))
                {
                    foreach (var item in ssidsRoles.Split(','))
                    {
                        roles.Add(item);
                    }

                    ssidsUserValue.Roles = roles.Distinct().ToList();
                }


                #endregion

                #region Provisioning Levels
                bool isrolesRemoved = false;
                if (ssidsUserValue.Roles.Count() > 0)
                {
                    isrolesRemoved = EndpointConnector.RemoveEntitlement(ssidsUserValue, Endpoint.ApplicationType == ApplicationType.SSIDS_GUI ? EntitlementType.Role : EntitlementType.HandPhone);

                    if (isrolesRemoved)
                    {
                        SSIDSUserInfo.DeleteUserRoleMap(ssidsUserValue, ssidsRoles);

                        ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentRemove(user, this.CurrentRequest, ssidsRoles, ssidsUserValue.AccountName);

                        _idmsLogger.WriteLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSSIDSUserLevelRemoved, ssidsUserValue.UserName, ssidsRoles, this.Endpoint.Id, msg), null);

                    }
                    else
                    {
                        _idmsLogger.WriteLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningSSIDSUserLevelRemoveFailed, ssidsUserValue.UserName, ssidsRoles, this.Endpoint.Id, msg), null);
                        return false;
                    }
                }
                else
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, string.Format("User {2} empty for the Access Request -{0}, User -{1}", this.CurrentRequest.Id, ssidsUserValue.UserName, msg), null);
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

        private bool LockUser(SSIDSUserInfo endpointUser, DomainModel.User user, LockScenario lockScenario)
        {
            bool isusrDisabled = true;

            if (endpointUser != null)
            {
                isusrDisabled = EndpointConnector.LockAccount(endpointUser);
                if (isusrDisabled)
                {
                    endpointUser.IsActive = false;
                    endpointUser.LastModifiedBy = "System";
                    endpointUser.LastModifiedDate = DateTime.Now;
                    endpointUser.Save("System");

                    SSIDSUserInfo.DeleteAllUserRoleMaps(endpointUser.Id);
                    _idmsLogger.WriteLog(LogLevelType.Info, string.Format("  User {0} Locked in SSIDS.", endpointUser.UserName), null);
                }
            }
            return isusrDisabled;
        }

        private bool UnlockUser(SSIDSUserInfo ssidsUsr, DomainModel.User user)
        {
            bool isusrUnlocked = true;

            if (ssidsUsr != null)
            {
                isusrUnlocked = false;
                isusrUnlocked = EndpointConnector.UnLockAccount(ssidsUsr);

                if (isusrUnlocked)
                {
                    ssidsUsr.IsActive = true;
                    ssidsUsr.LastModifiedBy = "System";
                    ssidsUsr.LastModifiedDate = DateTime.Now;

                    ssidsUsr.Save("System");
                    _idmsLogger.WriteLog(LogLevelType.Info, string.Format("  User {0} Unlocked in SSIDS.", ssidsUsr.UserName), null);
                }
            }
            return isusrUnlocked;
        }
    }
}
