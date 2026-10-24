
using DI.Chakra.Connector;
using DI.Chakra.DomainModel;
using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.DomainModel.UserHelper;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Workflow.ServiceHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.ProvisioningUtility
{
    public class ProvisioningFlowHelper
    {
        static List<ApplicationType> excludedApplicationTypes = new List<ApplicationType>() { ApplicationType.VPN, ApplicationType.InhouseSystems };
        static string serviceUser = MasterConfigHelper.ServiceUser;
        static List<long> SuccessFactor_NC = MasterConfigHelper.SuccessFactorEndpointSystemIDs;
        static string[] applicableStatusForOnBoard = new[] { Constants.EmployeeStatus.OffBoarded.ToUpper(), Constants.EmployeeStatus.NewEmployee.ToUpper() };
        static string[] applicableStatusForOffBoard = new[] { Constants.EmployeeStatus.OnBoarded.ToUpper(), Constants.EmployeeStatus.NewEmployee.ToUpper() };
        static string transferNotes = MasterConfigHelper.TranferTriggersNotes;
        public static void CreateOnBoardingTasksIfAdminCheckNotRequired()
        {
            List<User> users = null;
            try
            {
                users = User.GetOnBoardingUsers(false);
                DateTime dt = DateTime.Now;
                foreach (User user in users)
                {
                    try
                    {
                        if (MasterConfigHelper.IsDeploymentInProgress)
                        {
                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"CreateOnBoardingTasks - Deployment InProgress");
                            return;
                        }
                        User currentUser = user;

                        var trigger = EmployeeEventTrigger.GetByEffectiveDateYetToBeExecuted(user.Id, EmployeeEvents.NewJoinee, dt, false);
                        if (trigger == null)
                            continue;

                        bool isTriggerValid = IsTriggerValid(currentUser, trigger.EventType, trigger);

                        if (isTriggerValid && !string.IsNullOrWhiteSpace(user.Reserved4) && applicableStatusForOnBoard.Contains(user.Reserved4.Trim().ToUpper()))
                        {
                            DILog.ProvisioningLogger.WriteLog(LogLevelType.Info, $"Trigger valid and Proceed for ProvisioningDefaultExists check.", null);
                            string validationMessage = ProvisioningValidator.IsProvisioningDefaultExists(currentUser, EmployeeEvents.NewJoinee);

                            if (string.IsNullOrWhiteSpace(validationMessage))
                            {
                                DILog.ProvisioningLogger.WriteLog(LogLevelType.Info, $"PopulateOnBoardAccessRequests for onboarding started", null);
                                PopulateOnBoardAccessRequests(currentUser, trigger);
                                DILog.ProvisioningLogger.WriteLog(LogLevelType.Info, $"PopulateOnBoardAccessRequests for onboarding completed", null);

                                trigger.IsRequestCreated = true;
                                trigger.LastModifiedBy = "System";
                                trigger.LastModifiedDate = DateTime.Now;
                                trigger.Save(serviceUser);

                                user.Reserved4 = Constants.EmployeeStatus.OnBoarded;
                                user.IsActive = true;
                                user.TerminationDate = null;
                                user.Save(serviceUser);
                                continue;
                            }
                            else
                            {
                                DILog.ProvisioningLogger.WriteLog(LogLevelType.Info, $"OnBoard Admin Check Message for the PERNR -{currentUser.EmployeeId}/ {validationMessage}", null);
                                trigger.IsAdminCheckRequired = true;
                                trigger.LastModifiedBy = "System";
                                trigger.LastModifiedDate = DateTime.Now;
                                trigger.Save(serviceUser);
                                continue;
                            }
                        }
                        else
                        {
                            trigger.IsActive = false;
                            trigger.LastModifiedBy = "System";
                            trigger.LastModifiedDate = DateTime.Now;

                            trigger.Save(serviceUser);

                            if (isTriggerValid == false)
                            {
                                DILog.ProvisioningLogger.WriteLog(LogLevelType.Info, $"OnBoard Trigger valid is false. Hence inactivate the PERNR -{currentUser.EmployeeId}", null);
                                user.IsActive = false;
                                user.Save(serviceUser);

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, $"Error while creating on-board task for user:{user.DisplayName}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while creating on-board tasks.", ex);
            }
            finally
            {
                users?.Clear();
                users = null;
            }
        }

        public static void CreateRehireTasksIfAdminCheckNotRequired()
        {
            List<KeyValuePair<EmployeeEventTrigger, User>> triggerUsers = null;
            try
            {
                //DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"CreateRehireTasksIfAdminCheckNotRequired");
                triggerUsers = User.GetRehireTriggerUsers();
                foreach (var triggerUser in triggerUsers)
                {
                    User currentUser = triggerUser.Value;
                    try
                    {
                        if (MasterConfigHelper.IsDeploymentInProgress)
                        {
                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"CreateRehireTasks - Deployment InProgress");
                            return;
                        }
                        // Process the trigger this user data came from, not the user's first pending trigger.
                        var trigger = triggerUser.Key;

                        DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"Trigger exists {currentUser.UserName}");

                        User oldUser = User.GetById(currentUser.Id);
                        if (oldUser == null)
                            continue;

                        var triggerValid = IsTriggerValid(currentUser, EmployeeEvents.Rehire, trigger);
                        DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"Trigger valid {triggerValid}");


                        if (triggerValid)
                        {
                            bool canCreateTransferTrigger = oldUser.IsActive && oldUser.OrganisationCode != currentUser.OrganisationCode;
                            if (canCreateTransferTrigger)
                            {
                                // if the user is active and difference in OU, then create OU change requests
                                CreateOUChangeTasksIfNotRehire(currentUser, trigger);
                            }
                            else if (oldUser.IsActive == false && !string.IsNullOrWhiteSpace(oldUser.Reserved4) && applicableStatusForOnBoard.Contains(oldUser.Reserved4.Trim().ToUpper()))
                            {
                                string validationMessage = ProvisioningValidator.IsProvisioningDefaultExists(currentUser, EmployeeEvents.NewJoinee);

                                if (string.IsNullOrWhiteSpace(validationMessage))
                                {
                                    PopulateRehireAccessRequests(currentUser, trigger);
                                    currentUser.UserName = currentUser.EmployeeId;
                                    currentUser.TerminationDate = null;
                                    currentUser.IsActive = true;
                                    currentUser.Reserved4 = Constants.EmployeeStatus.OnBoarded;
                                    currentUser.EmailId = string.Empty;
                                    currentUser.Save(serviceUser);

                                    User.ExecuteQuery($@" UPDATE A Set A.Supervisor = B.DisplayName FROM DI_M_User A inner join DI_M_User B on A.ROEmployeeId = B.EmployeeId WHERE   A.IsActive=1 AND  A.EmployeeId='{currentUser.EmployeeId}' ");

                                    User.DeriveCOEmployeeId(currentUser.Id);

                                    User.DoAfterSaveUser(currentUser, serviceUser);

                                    trigger.IsRequestCreated = true;
                                    trigger.LastModifiedBy = "System";
                                    trigger.LastModifiedDate = DateTime.Now;
                                    trigger.Save(serviceUser);
                                }
                                else
                                {
                                    DILog.ProvisioningLogger.WriteLog(LogLevelType.Info, $"Rejoin Admin Check Message for the PERNR -{currentUser.EmployeeId}/ {validationMessage}", null);
                                    trigger.IsRequestCreated = false;
                                    trigger.IsAdminCheckRequired = true;
                                    trigger.LastModifiedBy = "System";
                                    trigger.LastModifiedDate = DateTime.Now;
                                    trigger.Save(serviceUser);
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            trigger.IsActive = false;
                            trigger.LastModifiedBy = "System";
                            trigger.LastModifiedDate = DateTime.Now;
                            trigger.Save(serviceUser);
                        }


                        #region Commented By Ashok

                        //if (triggerValid)
                        //    {
                        //        //if SAP account is not locked then don't run on-baord event for rejoin user.
                        //        var endpoint = EndPoint.GetByApplicationType(ApplicationType.HRMS_FLINS);
                        //        var sapUser = SAPUserinfo.GetPrimaryByDIUserId(currentUser.Id, endpoint.Id);

                        //        currentUser.TerminationDate = null;
                        //        currentUser.Save(serviceUser);

                        //        if (sapUser == null)
                        //        {
                        //            currentUser.NeedEmailUpdateToHR = true;
                        //            currentUser.NeedFLINSLoginIdUpdateToHR = true;
                        //            currentUser.Save(serviceUser);

                        //            string validationMessage = ProvisioningValidator.IsProvisioningDefaultExists(currentUser, EmployeeEvents.Rehire);
                        //            DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"Rejoin Admin Check Message for the UserId -{currentUser.Id}/ {validationMessage}", null);
                        //            if (string.IsNullOrEmpty(validationMessage))
                        //            {
                        //                PopulateOnBoardAccessRequests(currentUser, trigger);

                        //                trigger.IsRequestCreated = true;
                        //                trigger.LastModifiedBy = "System";
                        //                trigger.LastModifiedDate = DateTime.Now;

                        //                trigger.Save(serviceUser);
                        //            }
                        //        }
                        //        else
                        //        {
                        //            //if any change in org unit then trigger OU change else propagate the changes and leave the accounts as it is.
                        //            if (oldUser.SectionCode.Equals(currentUser.SectionCode))
                        //            {
                        //                var propagationFields = MasterConfigHelper.PropagationFields;
                        //                bool canRunPropagationTrigger = MasterConfigHelper.CanRunPropagationTrigger;
                        //                User.UpdateUserFromDiscovery(currentUser, propagationFields, canRunPropagationTrigger, serviceUser);

                        //                trigger.IsRequestCreated = true;
                        //                trigger.LastModifiedBy = "System";
                        //                trigger.LastModifiedDate = DateTime.Now;
                        //                trigger.Comments = "Since No Section change identitfied, created equalant Propagation request and marked IsRequesCreated = 1";
                        //                trigger.Save(serviceUser);
                        //            }
                        //            else
                        //            {
                        //                CreateOUChangeTrigger(currentUser, oldUser);

                        //                trigger.IsRequestCreated = true;
                        //                trigger.LastModifiedBy = "System";
                        //                trigger.LastModifiedDate = DateTime.Now;
                        //                trigger.Comments = "Since Section change identitfied, created equalant OU Chnage request and marked  IsRequesCreated = 1";
                        //                trigger.Save(serviceUser);
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        trigger.IsActive = false;
                        //        trigger.LastModifiedBy = "System";
                        //        trigger.LastModifiedDate = DateTime.Now;

                        //        trigger.Save(serviceUser);
                        //    }


                        #endregion Commented By Ashok
                    }
                    catch (Exception ex)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, $"Error while creating Rehire task for user:{currentUser?.DisplayName}.", ex);
                    }

                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while creating on-board tasks.", ex);
            }
            finally
            {
                triggerUsers?.Clear();
                triggerUsers = null;
            }
        }

        static void CreateOUChangeTasksIfNotRehire(User user, EmployeeEventTrigger trigger)
        {
            PopulateOUChangeAccessRequests(user, trigger);
            List<AccountReconcilation> adAccounts = null;
            var oldUser = User.GetById(user.Id);
            try
            {
                //map service accounts to supervisor before OU Change.
                adAccounts = User.HandleServiceAccount(oldUser, serviceUser, trigger.Id);
                if (adAccounts != null && adAccounts.Count > 0)
                {
                    //var ep = EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                    foreach (var adAccount in adAccounts)
                    {
                        var ep = EndPoint.GetById(adAccount.EndpointSystemId);
                        DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"Updating ExtensionAttribute12:{adAccount.AccountName} EndpointUserId:{adAccount.EndpointUserId} ");
                        try
                        {
                            Services.ProvisioningProcessor.FormExecutors.Common.ADBase adBaseFormExecutor = new Services.ProvisioningProcessor.FormExecutors.Common.ADBase(ep, null, new ProvisioningLogger(null, adAccount.EndpointSystemId, ep.EndpointSystemTypeId, ep.DisplayName));
                            adBaseFormExecutor.UpdateAttributes(user, adAccount.EndpointUserId);
                        }
                        catch (Exception ex)
                        {
                            DILog.ProvisioningLogger.WriteLog(null, ep.Id, LogLevelType.Exception, $"UpdateAttributes(Owner) failed for User : {user.DisplayName}", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, $"UpdateAttributes(Owner) failed for User : {user?.DisplayName}", ex);
            }
            finally
            {
                adAccounts?.Clear();
                adAccounts = null;
            }
            updateUser(user, EmployeeEvents.OUChange);

            trigger.IsRequestCreated = true;
            trigger.LastModifiedBy = "System";
            trigger.LastModifiedDate = DateTime.Now;
            trigger.Comments = $"Since the User with PERNR:{user.EmployeeId} is active and found difference in Organisation Code. IDMS had populated OU Changes Access Requests.";

            trigger.Save(serviceUser);
        }

        public static void CreateOUChangeTasksIfAdminCheckNotRequired()
        {
            List<KeyValuePair<EmployeeEventTrigger, User>> triggerUsers = null;
            try
            {
                triggerUsers = User.GetOUChangeTriggerUsers();
                foreach (var triggerUser in triggerUsers)
                {
                    User user = triggerUser.Value;
                    try
                    {
                        if (MasterConfigHelper.IsDeploymentInProgress)
                        {
                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"CreateOUChangeTasks - Deployment InProgress");
                            return;
                        }
                        // Process the trigger this user data came from, not the user's first pending trigger.
                        var trigger = triggerUser.Key;

                        if (IsTriggerValid(user, EmployeeEvents.OUChange, trigger))
                        {
                            // Transfr Dont do validation here, do at the Provisioning Form
                            //  string message = ProvisioningValidator.IsProvisioningDefaultExists(user, EmployeeEvents.OUChange);
                            //Check trigger sequence because there might be chance of multiple trigger on same day.
                            if (trigger.EventSequence > 1)
                            {
                                var activeEventExists = EmployeeEventTrigger.IsMultipleEventOnSameDay(trigger);
                                if (activeEventExists)
                                {
                                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"Multiple Event on same day exists for User {user.KnownAs}, EmployeeId {user.EmployeeId}. Current Event Sequence :{trigger.EventSequence}. Waiting for previous event completion. TriggerId:{trigger.Id}");
                                    continue;
                                }
                            }
                            //if (string.IsNullOrEmpty(message))
                            {
                                if(trigger.IsTransferWithNotes == null || trigger.IsTransferWithNotes == false)
                                {
                                    try
                                    {
                                        if (!string.IsNullOrWhiteSpace(trigger.Comments))
                                        {
                                            var obj = EmployeeEventTriggerComment.GetEmployeeEventTriggerComment(trigger.Comments);
                                            if (obj != null)
                                            {
                                                obj.RetainAccessForTransfer = "";
                                                trigger.Comments = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }
                                    PopulateOUChangeAccessRequests(user, trigger);

                                    var oldUser = User.GetById(user.Id);
                                    //map service accounts to supervisor before OU Change.
                                    try
                                    {
                                        var adAccounts = User.HandleServiceAccount(oldUser, serviceUser, trigger.Id);
                                        if (adAccounts.Count > 0)
                                        {
                                            //var ep = EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                                            foreach (var adAccount in adAccounts)
                                            {
                                                var ep = EndPoint.GetById(adAccount.EndpointSystemId);
                                                DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"Updating ExtensionAttribute12:{adAccount.AccountName} EndpointUserId:{adAccount.EndpointUserId} ");
                                                try
                                                {
                                                    Services.ProvisioningProcessor.FormExecutors.Common.ADBase adBaseFormExecutor = new Services.ProvisioningProcessor.FormExecutors.Common.ADBase(ep, null, new ProvisioningLogger(null, adAccount.EndpointSystemId, ep.EndpointSystemTypeId, ep.DisplayName));
                                                    adBaseFormExecutor.UpdateAttributes(user, adAccount.EndpointUserId);
                                                }
                                                catch (Exception ex)
                                                {
                                                    DILog.ProvisioningLogger.WriteLog(null, ep.Id, LogLevelType.Exception, $"UpdateAttributes(Owner) failed for User : {user?.DisplayName}", ex);
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, $"Transfer HandleServiceAccount for User {user?.KnownAs}, EmployeeId {user?.EmployeeId} failed", ex);
                                    }
                                    //Update the new org stucture
                                    updateUser(user, EmployeeEvents.OUChange);
                                    trigger.IsRequestCreated = true;
                                    trigger.LastModifiedBy = "System";
                                    trigger.LastModifiedDate = DateTime.Now;
                                    trigger.Save(serviceUser);
                                }
                                else if (trigger.IsTransferWithNotes != null && trigger.IsTransferWithNotes.Value)
                                {
                                    try
                                    {
                                        if (!string.IsNullOrWhiteSpace(trigger.Comments))
                                        {
                                            var obj = EmployeeEventTriggerComment.GetEmployeeEventTriggerComment(trigger.Comments);
                                            if (obj != null && !string.IsNullOrWhiteSpace(obj.RetainAccessForTransfer) && obj.RetainAccessForTransfer.ToUpper() != MasterConfigHelper.TranferTriggersNotes.ToUpper())
                                            {
                                                obj.EventReason = MasterConfigHelper.TranferTriggersNotes;
                                                trigger.Comments = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                                            }
                                        }
                                    }
                                    catch
                                    {
                                    }
                                    trigger.IsRequestCreated = true;
                                    trigger.LastModifiedBy = "System";
                                    trigger.LastModifiedDate = DateTime.Now;
                                    trigger.Save(serviceUser);

                                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"This Trigger:{trigger.EventType} is Retain Access for Transfer. Hence creating Reorg-trigger for User: {user.KnownAs}, EmployeeId {user.EmployeeId}. Current Event Sequence :{trigger.EventSequence}. OU TriggerId:{trigger.Id}");
                                    var reOrgtrigger = new EmployeeEventTrigger();
                                    reOrgtrigger.UserId = trigger.UserId;
                                    reOrgtrigger.EventType = EmployeeEvents.ReOrganisation;
                                    reOrgtrigger.EffectiveDate = trigger.EffectiveDate;
                                    reOrgtrigger.IsRequestCreated = false;
                                    reOrgtrigger.IsAdminCheckRequired = trigger.IsAdminCheckRequired;
                                    reOrgtrigger.UserDetails = trigger.UserDetails;
                                    reOrgtrigger.OldUserDetails = trigger.OldUserDetails;
                                    reOrgtrigger.IsActive = true;
                                    reOrgtrigger.CreatedBy = ProvisioningConstants.ReorgCreatedBy;
                                    reOrgtrigger.CreatedDate = DateTime.Now;
                                    reOrgtrigger.IsMailSent = trigger.IsMailSent;
                                    reOrgtrigger.RequestStatus = trigger.RequestStatus;
                                    reOrgtrigger.ActualEventDate = trigger.ActualEventDate;
                                    reOrgtrigger.Comments = trigger.Comments;
                                    reOrgtrigger.EventSequence = trigger.EventSequence;
                                    reOrgtrigger.Save(ProvisioningConstants.ReorgCreatedBy);
                                }
                            }
                        }
                        else
                        {
                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, $"Transfer Action not found at HRMS for User {user.KnownAs}, EmployeeId {user.EmployeeId}");

                            trigger.IsActive = false;
                            trigger.LastModifiedBy = "System";
                            trigger.LastModifiedDate = DateTime.Now;
                            trigger.Save(serviceUser);
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, $"Error while creating org-transfer task for user:{user?.DisplayName}.", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while creating org-transfer tasks.", ex);
            }
            finally
            {
                triggerUsers?.Clear();
                triggerUsers = null;
            }
        }

        public static void CreateReOrgTasks()
        {
            List<KeyValuePair<EmployeeEventTrigger, User>> triggerUsers = null;
            try
            {
                triggerUsers = User.GetReOrgTriggerUsers();
                foreach (var triggerUser in triggerUsers)
                {
                    User triggerUserDetail = triggerUser.Value;
                    try
                    {
                        if (MasterConfigHelper.IsDeploymentInProgress)
                        {
                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"CreateReOrgTasks - Deployment InProgress");
                            return;
                        }
                        // Process the trigger this user data came from, not the user's first pending trigger.
                        var trigger = triggerUser.Key;
                        // GetReOrgUsers has no admin-check filter; triggers awaiting admin check are handled from the UI.
                        if (trigger.IsAdminCheckRequired)
                            continue;

                        if (IsTriggerValid(triggerUserDetail, EmployeeEvents.ReOrganisation, trigger))
                        {

                            // Transfr Dont do validation here, do at the Provisioning Form
                            //string message = ProvisioningValidator.IsProvisioningDefaultExists(triggerUserDetail, EmployeeEvents.ReOrganisation);

                            //if (string.IsNullOrEmpty(message))

                            //Check trigger sequence because there might be chance of multiple trigger on same day.
                            if (trigger.EventSequence > 1)
                            {
                                var activeEventExists = EmployeeEventTrigger.IsMultipleEventOnSameDay(trigger);
                                if (activeEventExists)
                                {
                                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, $"Multiple Event on same day exists for User {triggerUserDetail.KnownAs}, EmployeeId {triggerUserDetail.EmployeeId}. Current Event Sequence :{trigger.EventSequence}. Waiting for previous event completion. TriggerId:{trigger.Id}");
                                    continue;
                                }
                            }
                            {
                                PopulateReOrgAccessRequests(triggerUserDetail, trigger);

                                triggerUserDetail.Save(serviceUser);

                                if (triggerUserDetail != null && triggerUserDetail.FullTimeEmployee)
                                    UpdateReferenceUser(triggerUserDetail.Id);

                                trigger.IsRequestCreated = true;
                                trigger.LastModifiedBy = "System";
                                trigger.LastModifiedDate = DateTime.Now;
                                trigger.Save(serviceUser);
                            }
                        }
                        else
                        {
                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, $"Re-Org Action not found at HRMS for User {triggerUserDetail.KnownAs}, EmployeeId {triggerUserDetail.EmployeeId}");

                            trigger.IsActive = false;
                            trigger.LastModifiedBy = "System";
                            trigger.LastModifiedDate = DateTime.Now;
                            trigger.Save(serviceUser);
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, $"Error while creating org-transfer task for user:{triggerUserDetail?.DisplayName}.", ex);
                    }

                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while creating re-org tasks.", ex);
            }
            finally
            {
                triggerUsers?.Clear();
                triggerUsers = null;
            }
        }

        static void updateUser(User user, EmployeeEvents eventType)
        {
            if (user == null) return;
            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"{UtilityMethods.GetEnumAttribute<Description>(eventType)} Event: Global profile updation started for the  User {user?.KnownAs}, EmployeeId {user?.EmployeeId}");
            var tmpUser = User.GetById(user.Id);
            if (tmpUser != null)
            {
                tmpUser.CompanyId = user.CompanyId;
                tmpUser.CompanyCode = user.CompanyCode;
                tmpUser.CompanyName = user.CompanyName;
                tmpUser.DepartmentId = user.DepartmentId;
                tmpUser.DepartmentCode = user.DepartmentCode;
                tmpUser.DepartmentName = user.DepartmentName;
                tmpUser.DivisionId = user.DivisionId;
                tmpUser.DivisionCode = user.DivisionCode;
                tmpUser.DivisionName = user.DivisionName;
                tmpUser.NOrganisationId = user.NOrganisationId;
                tmpUser.SectionCode = user.SectionCode;
                tmpUser.SectionName = user.SectionName;
                tmpUser.OrgUnit = user.OrgUnit;
                tmpUser.OrgBranch = user.OrgBranch;
                tmpUser.OrgCompany = user.OrgCompany;
                tmpUser.OrgDepartment = user.OrgDepartment;
                tmpUser.OrgDivision = user.OrgDivision;
                tmpUser.OrgSection = user.OrgSection;
                tmpUser.BranchCode = user.BranchCode;
                tmpUser.UnitCode = user.UnitCode;
                tmpUser.OrganisationCode = user.OrganisationCode;
                tmpUser.PositionCode = user.PositionCode;
                tmpUser.PositionName = user.PositionName;
                tmpUser.PostalAddress = user.PostalAddress;
                tmpUser.CostCenter = user.CostCenter;
                tmpUser.CostCenterText = user.CostCenterText;
                tmpUser.ROEmployeeId = user.ROEmployeeId;
                tmpUser.TerminationDate = user.TerminationDate;
                tmpUser.ROEmployeeId = user.ROEmployeeId;
                tmpUser.FirstName = user.FirstName;
                tmpUser.MiddleName = user.MiddleName;
                tmpUser.LastName = user.LastName;
                tmpUser.DisplayName = user.DisplayName;
                tmpUser.KnownAs = user.KnownAs;
                tmpUser.EmailId = user.EmailId;
                tmpUser.HomephoneNumber = user.HomephoneNumber;
                tmpUser.TelephoneNumber = user.TelephoneNumber;
                tmpUser.MobileNumber = user.MobileNumber;
                tmpUser.Fax = user.Fax;
                tmpUser.PostalCode = user.PostalCode;
                tmpUser.PostalAddress = user.PostalAddress;
                tmpUser.Ethinicity = user.Ethinicity;
                tmpUser.Country = user.Country;
                tmpUser.EmployeeType = user.EmployeeType;
                tmpUser.LastModifiedBy = "System";
                tmpUser.LastModifiedDate = DateTime.Now;
                tmpUser.Save(serviceUser);
            }
            else
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"User doesnt exists:{user.DisplayName}");
            }
            User.ExecuteQuery($@" UPDATE A Set A.Supervisor = B.DisplayName FROM DI_M_User A inner join DI_M_User B on A.ROEmployeeId = B.EmployeeId WHERE   A.IsActive=1 AND  A.EmployeeId='{user.EmployeeId}' ");

            User.DeriveCOEmployeeId(user.Id);

            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"{ UtilityMethods.GetEnumAttribute<Description>(eventType)} Event: Global profile updation ended for the  User {user?.KnownAs}, EmployeeId {user?.EmployeeId}");
        }

        public static void CreatePropagationTasks(EmployeeEvents employeeEvent)
        {
            List<EmployeeEventTrigger> triggers = new List<EmployeeEventTrigger>();
            try
            {
                triggers = EmployeeEventTrigger.GetUsersForPropagation(employeeEvent);
                Guid transactionId = Guid.NewGuid();
                bool canProceed = true;
                User usr = null;
                foreach (EmployeeEventTrigger trigger in triggers)
                {
                    try
                    {

                        if (MasterConfigHelper.IsDeploymentInProgress)
                        {
                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"CreatePropagationTasks/Promotion - Deployment InProgress");
                            return;
                        }

                        var tmpUser = User.GetById(trigger.UserId);
                        if (tmpUser != null && tmpUser.IsActive == false)
                        {
                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, $"User:{tmpUser?.DisplayName} is inactive.So marking trigger as inactive");
                            trigger.IsActive = false;
                            trigger.LastModifiedBy = "System";
                            trigger.LastModifiedDate = DateTime.Now;
                            trigger.Save(serviceUser);
                            continue;
                        }

                        canProceed = true;
                        usr = null;
                        if (!string.IsNullOrWhiteSpace(trigger.UserDetails))
                        {
                            usr = UtilityMethods.Deserialize<User>(trigger.UserDetails);
                            if (usr != null)
                                canProceed = IsTriggerValid(usr, employeeEvent, trigger, EmployeeEventTriggerComment.GetEmployeeEventTriggerComment(trigger.Comments));
                        }
                        //Check for Active User
                        if (canProceed)
                        {
                            var user = User.GetById(trigger.UserId);
                            if (user != null && user.IsActive)
                            {
                                if (trigger.EventSequence > 1)
                                {
                                    var activeEventExists = EmployeeEventTrigger.IsMultipleEventOnSameDay(trigger);
                                    if (activeEventExists)
                                    {
                                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, $"CreatePropagationTasks - Multiple Event on same day exists for User {user.KnownAs}, EmployeeId {user.EmployeeId}. Current Event Sequence :{trigger.EventSequence}. Waiting for previous event completion. TriggerId:{trigger.Id}");
                                        continue;
                                    }
                                }
                                PopulatePropagationAccessRequests(trigger.UserId, transactionId, trigger, usr);

                                trigger.IsRequestCreated = true;
                                trigger.LastModifiedBy = "System";
                                trigger.LastModifiedDate = DateTime.Now;
                                trigger.Save(serviceUser);
                            }

                        }
                        else
                        {
                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, $"Propagation Action not found at HRMS for EmployeeEventTrigger Id:[{trigger.Id}. Hence the trigger is set to be inactive.");
                            trigger.IsActive = false;
                            trigger.LastModifiedBy = "System";
                            trigger.LastModifiedDate = DateTime.Now;
                            trigger.Save(serviceUser);
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, $"Error while creating propagation task. TriggerId:{trigger.Id}.", ex);
                    }

                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, $"Error while creating propagation tasks for the EmployeeEvent - {UtilityMethods.GetEnumAttribute<Description>(employeeEvent)}.", ex);
            }
            finally
            {
                triggers?.Clear();
                triggers = null;
            }
        }

        public static void CreateOffBoardingTasksIfAdminCheckNotRequired()
        {
            List<User> users = new List<User>();
            try
            {
                users = User.GetOffBoardingUsers();
                DateTime dt = DateTime.Now;
                foreach (User user in users)
                {
                    try
                    {
                        var trigger = EmployeeEventTrigger.GetDueTriggerByUserAndEvent(user.Id, EmployeeEvents.Resignation, dt);
                        if (trigger == null)
                        {
                            continue;
                        }
                        if(user.FullTimeEmployee == false && trigger != null)
                        {
                            if((trigger.EffectiveDate.Date.AddHours(Constants.MaxHour) <= DateTime.Now) == false)
                            {
                                continue;
                            }
                        }

                        if (IsTriggerValid(user, EmployeeEvents.Resignation, trigger) && !string.IsNullOrWhiteSpace(user.Reserved4) && applicableStatusForOffBoard.Contains(user.Reserved4.Trim().ToUpper()))
                        {
                            if(user.FullTimeEmployee == false && trigger.CreatedBy != null && trigger.CreatedBy.ToUpper() == "System".ToUpper())
                            {
                            }
                            else
                            {
                                User.ExecuteQuery($"UPDATE DI_M_User Set TerminationDate =(SELECT EffectiveDate FROM DI_M_EmployeeEventTrigger WHERE ID={trigger.Id}) Where Id = {user.Id}");
                            }

                            int requestCount = PopulateOffBoardAccessRequests(user, trigger);

                            if (requestCount == 0)
                            {
                                user.TerminationDate = trigger.EffectiveDate;
                                user.IsActive = false;
                                user.LastModifiedBy = "System";
                                user.LastModifiedDate = DateTime.Now;
                                user.Reserved4 = Constants.EmployeeStatus.OffBoarded;
                                user.Save(serviceUser);
                            }

                            trigger.IsRequestCreated = true;
                            trigger.LastModifiedBy = "System";
                            trigger.LastModifiedDate = DateTime.Now;

                            trigger.Save(serviceUser);
                            WithDrawnAllPendingRequests(user);
                        }
                        else
                        {
                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, $"Offboard Action not found at HRMS for User {user.KnownAs}, EmployeeId {user.EmployeeId}");

                            trigger.IsActive = false;
                            trigger.LastModifiedBy = "System";
                            trigger.LastModifiedDate = DateTime.Now;

                            trigger.Save(serviceUser);
                            if(user.FullTimeEmployee)
                                user.TerminationDate = null;

                            user.Save(serviceUser);
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, $"Error while creating off-board tasks for user:{user.DisplayName}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while creating off-board tasks.", ex);
            }
            finally
            {
                users?.Clear();
                users = null;
            }
        }



        public static void CreateEmployeeExtensionTasks()
        {
            List<EmployeeEventTrigger> triggers = new List<EmployeeEventTrigger>();
            try
            {
                triggers = EmployeeEventTrigger.GetUsersForExtension();

                foreach (EmployeeEventTrigger trigger in triggers)
                {
                    if (MasterConfigHelper.IsDeploymentInProgress)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"CreateEmployeeExtensionTasks - Deployment InProgress");
                        return;
                    }
                    PopulateEmployeeExtensionAccessRequests(trigger.UserId, trigger);

                    trigger.IsRequestCreated = true;
                    trigger.LastModifiedBy = "System";
                    trigger.LastModifiedDate = DateTime.Now;
                    trigger.Save(serviceUser);
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while creating employee extension tasks.", ex);
            }
            finally
            {
                triggers?.Clear();
                triggers = null;
            }
        }

        public static void CreateAttestationRemediationRequests()
        {
            IList<AttestationSnapshotAction> reviews = new List<AttestationSnapshotAction>();
            List<AttestationSnapshotAction> actionOnAccounts = new List<AttestationSnapshotAction>();
            List<AttestationSnapshotAction> actionOnEntitlements = new List<AttestationSnapshotAction>();
            List<long> distinctCampaigns = new List<long>();
            try
            {
                reviews = AttestationSnapshotAction.GetPendingRemediations();
                actionOnAccounts = reviews.Where(a => a.AttestationSnapshot.EntitlementType == EntitlementType.Account).ToList();
                var entity = actionOnAccounts.Select(a => new { a.AttestationSnapshot.Entity, a.AttestationSnapshot.EndpointId }).Distinct().ToList();
                actionOnEntitlements = reviews.Where(a => a.AttestationSnapshot.EntitlementType != EntitlementType.Account && !entity.Exists(j=>j.Entity == a.AttestationSnapshot.ParentEntity && j.EndpointId == a.AttestationSnapshot.EndpointId)).ToList();

                foreach (var actionOnAccount in actionOnAccounts)
                {
                    var attestationCampaign = AttestationCampaign.GetById(actionOnAccount.AttestationSnapshot.AttestationCampaignId);
                    actionOnAccount.AttestationSnapshot.AttestationCampaign = attestationCampaign;
                    try
                    {
                        PopulateRemediationAccessRequests(actionOnAccount);
                    }
                    catch (OutOfMemoryException ex)
                    {
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        DILog.AttestationLogger.WriteLog(LogLevelType.Exception, $"Exception while creating RemediationRequest for account:{actionOnAccount.AttestationSnapshot.AccountName} Campaign Name:{actionOnAccount.AttestationSnapshot.AttestationCampaign.CampaignName}.Message:{ex.Message}", ex);
                    }

                    actionOnAccount.RemediationStatus = AttestationRemediationStatus.Completed;
                    actionOnAccount.LastModifiedBy = "System";
                    actionOnAccount.LastModifiedDate = DateTime.Now;
                    actionOnAccount.Save(serviceUser);

                    var entilementToUpdate = reviews.Where(a => a.AttestationSnapshot.EntitlementType != EntitlementType.Account && (actionOnAccount.AttestationSnapshot.Entity == a.AttestationSnapshot.ParentEntity && actionOnAccount.AttestationSnapshot.EndpointId == a.AttestationSnapshot.EndpointId)).ToList();
                    foreach (var entilementToMark in entilementToUpdate)
                    {
                        entilementToMark.RemediationStatus = AttestationRemediationStatus.Completed;
                        entilementToMark.LastModifiedBy = "System";
                        entilementToMark.LastModifiedDate = DateTime.Now;
                        entilementToMark.Save(serviceUser);
                    }
                }

                foreach (var actionOnEntitlement in actionOnEntitlements)
                {
                    var attestationCampaign = AttestationCampaign.GetById(actionOnEntitlement.AttestationSnapshot.AttestationCampaignId);
                    actionOnEntitlement.AttestationSnapshot.AttestationCampaign = attestationCampaign;

                    try
                    {
                        PopulateRemediationAccessRequests(actionOnEntitlement);
                    }
                    catch (Exception ex)
                    {
                        DILog.AttestationLogger.WriteLog(LogLevelType.Exception, $"Exception while creating RemediationRequest Entitlement Remove:{actionOnEntitlement.AttestationSnapshot.Name} & Account:{actionOnEntitlement.AttestationSnapshot.AccountName} Campaign Name:{actionOnEntitlement.AttestationSnapshot.AttestationCampaign.CampaignName}.Message:{ex.Message}", ex);
                    }
                    actionOnEntitlement.RemediationStatus = AttestationRemediationStatus.Completed;
                    actionOnEntitlement.LastModifiedBy = "System";
                    actionOnEntitlement.LastModifiedDate = DateTime.Now;
                    actionOnEntitlement.Save(serviceUser);
                }

                //update the campaignStatus
                distinctCampaigns = reviews.Select(a => a.AttestationSnapshot.AttestationCampaignId).Distinct().ToList();
                foreach (var distinctCampaign in distinctCampaigns)
                {
                    var campaign = AttestationCampaign.GetByCampaignId(distinctCampaign);
                    if (campaign != null)
                    {
                        campaign.Status = AttestationCampaignStatus.Remediation;
                        campaign.LastModifiedBy = "System";
                        campaign.LastModifiedDate = DateTime.Now;
                        campaign.Save(serviceUser);
                    }
                }

                entity = null;

            }
            catch (OutOfMemoryException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while creating attestation remediation tasks.", ex);
            }
            finally
            {
                distinctCampaigns?.Clear();
                distinctCampaigns = null;

                reviews?.Clear();
                reviews = null;

                actionOnAccounts?.Clear();
                actionOnAccounts = null;

                actionOnEntitlements?.Clear();
                actionOnEntitlements = null;
            }
        }

        public static void UpdateAttestationRemediationStatus()
        {
            List<AttestationCampaign> activeParentCampaigns = new List<AttestationCampaign>();
            try
            {
                activeParentCampaigns = AttestationCampaign.GetAllParentCampaign();
                if (activeParentCampaigns != null && activeParentCampaigns.Count > 0)
                {
                    foreach (var activeParentCampaign in activeParentCampaigns)
                    {
                        var childCampaigns = AttestationCampaign.GetChildCampaigns(activeParentCampaign.Id);
                        if (childCampaigns != null && childCampaigns.Count > 0)
                        {
                            foreach (var childCampaign in childCampaigns)
                            {
                                if (childCampaign.Status == AttestationCampaignStatus.Completed || childCampaign.Status == AttestationCampaignStatus.Remediation)
                                {
                                    var pendingAccessRequest = AccessRequest.GetByRemediationProgressStatus(childCampaign.Id);
                                    if (pendingAccessRequest == null || pendingAccessRequest.Count == 0)
                                    {
                                        childCampaign.Status = AttestationCampaignStatus.Completed;
                                        childCampaign.LastModifiedBy = "System";
                                        childCampaign.LastModifiedDate = DateTime.Now;
                                        childCampaign.Save(serviceUser);

                                        if (childCampaign.ParentCampaign.HasValue)
                                        {
                                            var pendingAttestationCampaign = AttestationCampaign.GetChildCampaigns(childCampaign.ParentCampaign.Value).Where(a => a.Status != AttestationCampaignStatus.Completed).ToList();
                                            if (pendingAttestationCampaign == null || pendingAttestationCampaign.Count() == 0)
                                            {
                                                var parentCampaign = AttestationCampaign.GetById(childCampaign.ParentCampaign.Value);
                                                if (parentCampaign != null)
                                                {
                                                    parentCampaign.Status = AttestationCampaignStatus.Completed;
                                                    parentCampaign.EndDate = DateTime.Now;
                                                    parentCampaign.LastModifiedBy = "System";
                                                    parentCampaign.LastModifiedDate = DateTime.Now;
                                                    parentCampaign.Save(serviceUser);
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (activeParentCampaign.RequiresAttestationRemediation && activeParentCampaign.Status == AttestationCampaignStatus.Remediation)
                            {
                                var pendingAccessRequest = AccessRequest.GetByRemediationProgressStatus(activeParentCampaign.Id);
                                if (pendingAccessRequest == null || pendingAccessRequest.Count == 0)
                                {
                                    activeParentCampaign.Status = AttestationCampaignStatus.Completed;
                                    activeParentCampaign.EndDate = DateTime.Now;
                                    activeParentCampaign.LastModifiedBy = "System";
                                    activeParentCampaign.LastModifiedDate = DateTime.Now;
                                    activeParentCampaign.Save(serviceUser);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while updating Remediation status.", ex);
            }
            finally
            {
                activeParentCampaigns?.Clear();
                activeParentCampaigns = null;
            }
        }

        static int PopulateOnBoardAccessRequests(User user, EmployeeEventTrigger trigger,
            AccessRequestSource accessRequestSource = AccessRequestSource.OnBoarding)
        {
            List<AccessRequest> requests = new List<AccessRequest>();
            AccessRequest newRequest = new AccessRequest();
            Guid transactionId = Guid.NewGuid();
            IEnumerable<NRulesetForApplication> endpointSystems = null;
            try
            {
                endpointSystems = NRulesetForApplication.GetRulesetForApplication(user.Id, trigger).Distinct();

                if (MasterConfigHelper.EnableOnBoardingRequestSequenceInProdution)
                    endpointSystems = endpointSystems.Where(a => a.EndPoint.IsActive && a.EndPoint.EndpointSystemTypeId.HasValue).OrderBy(o => o.EndPoint.Id);
                else
                    endpointSystems = endpointSystems.Where(a => a.EndPoint.IsActive && a.EndPoint.EndpointSystemTypeId.HasValue).OrderBy(o => o.EndPoint.EndpointSystemTypeId);

                long o365AccessId = 0;
                long adAccessId = 0;
                foreach (var endpointSystem in endpointSystems)
                {

                    newRequest = new AccessRequest();

                    newRequest.UserId = user.Id;
                    newRequest.EndpointSystemId = endpointSystem.EndpointSystemId;

                    newRequest.RequestStatus = AccessRequestStatus.Granted;
                    newRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                    newRequest.RequesterId = null;

                    newRequest.RequestSource = accessRequestSource;
                    newRequest.ActivationDate = user.JoinDate.HasValue ? user.JoinDate.Value : DateTime.Now;
                    newRequest.DeactivationDate = user.TerminationDate;

                    newRequest.ExecutionTime = DateTime.Now;
                    newRequest.Description = accessRequestSource == AccessRequestSource.EmployeeConversion ? "This request is created automatically for Employee Conversion On-Boarding." : "This request is created automatically for On-Boarding.";
                    newRequest.Remarks = string.Empty;
                    newRequest.TransactionId = transactionId;
                    newRequest.EmployeeEventTriggerId = trigger.Id;
                    newRequest.AccountType = AccountType.User;
                    newRequest.ApplicationType = EndPoint.GetById(endpointSystem.EndpointSystemId).ApplicationType;

                    if (newRequest != null)
                    {
                        if (AccessRequest.IsSameAccessRequestExistsForUser(newRequest.RequestSource, newRequest.UserId, newRequest.EmployeeEventTriggerId.Value, newRequest.EndpointSystemId.Value, newRequest.ActivationDate, newRequest.ApplicationType, newRequest.AccountType))
                        {
                            DILog.ProvisioningLogger.WriteLog(LogLevelType.Info, $"Same OnBoarding AccessRequest already exists for this user:{user.UserName} application:{newRequest.ApplicationType} TriggerId:{trigger.Id}");
                            continue;
                        }

                        if (newRequest.ApplicationType == ApplicationType.ADSingaporePowerLocal)
                        {
                            newRequest.TransactionId = Guid.NewGuid();
                            newRequest.IsDependentRequest = true;
                        }
                        if (newRequest.ApplicationType == ApplicationType.IDMS)
                        {
                            newRequest.TransactionId = Guid.NewGuid();
                            newRequest.IsDependentRequest = false;
                        }
                        newRequest.Save(serviceUser);

                        if (newRequest.ApplicationType == ApplicationType.Office365)
                        {
                            o365AccessId = newRequest.Id;
                        }
                        if (newRequest.ApplicationType == ApplicationType.ADSingaporePowerLocal)
                        {
                            adAccessId = newRequest.Id;
                        }

                        requests.Add(newRequest);
                    }
                }

                //Setting SFB request as dependent request of O365
                foreach (var req in requests)
                {
                    if (req.ApplicationType == ApplicationType.MicrosoftTeams && o365AccessId > 0)
                    {
                        req.IsParentRequest = true;
                        req.TransactionId = Guid.NewGuid();
                        req.ParentRequestId = o365AccessId;
                        req.Save(serviceUser);
                        continue;
                    }

                    if ((req.ApplicationType != ApplicationType.ADSingaporePowerLocal) && adAccessId > 0)
                    {
                        if (req.ApplicationType == ApplicationType.IDMS)
                        {
                            continue;
                        }
                        else
                        {
                            req.ParentRequestId = adAccessId;
                            req.IsDependentRequestCreated = true;
                            req.IsParentRequest = true;
                        }
                        req.Save(serviceUser);
                    }
                }

                AccessRequest.AddAccessRequest(requests);

                return requests.Count;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, $"PopulateOnBoardAccessRequests for onboarding failed Message:{ex?.Message}", ex);
                throw ex;
            }
            finally
            {
                requests?.Clear();
                requests = null;

                endpointSystems = null;
            }
        }

        static int PopulateRehireAccessRequests(User user, EmployeeEventTrigger trigger)
        {
            List<AccessRequest> requests = new List<AccessRequest>();
            AccessRequest newRequest = new AccessRequest();
            Guid transactionId = Guid.NewGuid();
            List<NRulesetForApplication> endpointSystems = new List<NRulesetForApplication>();
            try
            {
                endpointSystems = NRulesetForApplication.GetRulesetForApplication(UtilityMethods.Deserialize<User>(trigger.UserDetails));
                endpointSystems = endpointSystems.Where(a => a.EndPoint.EndpointSystemTypeId.HasValue).OrderBy(o => o.EndPoint.EndpointSystemTypeId).ToList();
                long o365AccessId = 0; long adAccessId = 0;
                foreach (var endpointSystem in endpointSystems)
                {

                    newRequest = new AccessRequest();
                    newRequest.UserId = user.Id;
                    newRequest.EndpointSystemId = endpointSystem.EndpointSystemId;
                    newRequest.RequestStatus = AccessRequestStatus.Granted;
                    newRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                    newRequest.RequestSource = AccessRequestSource.OnBoarding;
                    newRequest.RequesterId = null;
                    newRequest.ActivationDate = user.JoinDate.HasValue ? user.JoinDate.Value : DateTime.Now;
                    newRequest.DeactivationDate = user.TerminationDate;

                    newRequest.ExecutionTime = DateTime.Now;
                    newRequest.Description = "This request is created automatically for Rehire.";
                    newRequest.Remarks = string.Empty;
                    newRequest.TransactionId = transactionId;
                    newRequest.EmployeeEventTriggerId = trigger.Id;
                    newRequest.AccountType = AccountType.User;
                    newRequest.ApplicationType = EndPoint.GetById(endpointSystem.EndpointSystemId).ApplicationType;

                    if (newRequest != null)
                    {
                        if (AccessRequest.IsSameAccessRequestExistsForUser(newRequest.RequestSource, newRequest.UserId, newRequest.EmployeeEventTriggerId.Value, newRequest.EndpointSystemId.Value, newRequest.ActivationDate, newRequest.ApplicationType, newRequest.AccountType))
                        {
                            DILog.ProvisioningLogger.WriteLog(LogLevelType.Info, $"Same Rehire Access Requests already exists for this user:{user.UserName} application:{newRequest.ApplicationType} TriggerId:{trigger.Id}");
                            continue;
                        }

                        if (newRequest.ApplicationType == ApplicationType.ADSingaporePowerLocal)
                        {
                            newRequest.TransactionId = Guid.NewGuid();
                            newRequest.IsDependentRequest = true;
                        }

                        newRequest.Save(serviceUser);
                        if (newRequest.ApplicationType == ApplicationType.Office365)
                        {
                            o365AccessId = newRequest.Id;
                        }
                        if (newRequest.ApplicationType == ApplicationType.ADSingaporePowerLocal)
                        {
                            adAccessId = newRequest.Id;
                        }


                        requests.Add(newRequest);
                    }
                }

                //Setting SFB request as dependent request of O365
                foreach (var req in requests)
                {
                    if (req.ApplicationType == ApplicationType.MicrosoftTeams && o365AccessId > 0)
                    {
                        req.TransactionId = Guid.NewGuid();
                        req.IsParentRequest = true;
                        req.ParentRequestId = o365AccessId;
                        req.Save(serviceUser);
                        continue;
                    }

                    if ((req.ApplicationType != ApplicationType.ADSingaporePowerLocal) && adAccessId > 0)
                    {
                        req.ParentRequestId = adAccessId;
                        req.IsDependentRequestCreated = true;
                        req.IsParentRequest = true;
                        //req.TransactionId = Guid.NewGuid();
                        req.Save(serviceUser);
                    }
                }

                AccessRequest.AddAccessRequest(requests);

                return requests.Count;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                requests?.Clear();
                requests = null;

                endpointSystems?.Clear();
                endpointSystems = null;
            }
        }

        static void PopulateRemediationAccessRequests(AttestationSnapshotAction review)
        {
            AccessRequest newRequest = new AccessRequest();
            AdHocAccessRequest newAdhocRequest = new AdHocAccessRequest();
            Guid transactionId = Guid.NewGuid();
            string systemUser = serviceUser;
            try
            {
                //if (review.AttestationSnapshot.KeyId == 0)
                //{
                //    throw new Exception("AT0001NOKey: Implementation issue");
                //}
                var usr = User.GetById(review.AttestationSnapshot.UserId);
                if (usr == null || usr.IsActive == false)
                {
                    UpdateRemediationStausAsCompleted(review);
                    throw new Exception($"User: {review.AttestationSnapshot.DIUserDisplayName} is Inactive. Hence remediation request cannot be created");
                }
                EntitlementReconcilation entitlementRecon = null;
                AccountReconcilation accountRec = null;
                bool validateAccount = false;
                var endpoint = EndPoint.GetById(review.AttestationSnapshot.EndpointId);
                if (endpoint.IsActive == false)
                {
                    UpdateRemediationStausAsCompleted(review);
                    throw new Exception($"Endpoint system:{endpoint.DisplayName} InActive");
                }

                if (review.AttestationSnapshot.EntitlementType == EntitlementType.Account)
                {
                    accountRec = AccountReconcilation.GetById(review.AttestationSnapshot.KeyId);
                    validateAccount = true;
                }
                else
                {
                    accountRec = AccountReconcilation.GetActiveAccountByAccountName(review.AttestationSnapshot.ParentEntity, review.AttestationSnapshot.EndpointId);
                    if (endpoint.ApplicationType != ApplicationType.FLINS_BI && endpoint.ApplicationType != ApplicationType.SAP_GRC)
                    {
                        entitlementRecon = EntitlementReconcilation.GetById(review.AttestationSnapshot.KeyId);
                        validateAccount = true;
                    }
                }

                if (validateAccount && (accountRec == null || accountRec.IsDeleted == true))
                {
                    if (string.IsNullOrWhiteSpace(review.Comment))
                    {
                        review.Comment = string.Empty;
                    }
                    review.Comment = review.Comment + Environment.NewLine + $"Remediation Requests comment by IDMS: Account:{accountRec?.AccountName} is Inactive. Cannot create a Remediation Requests";
                    DILog.AttestationLogger.WriteLog(LogLevelType.Exception, $"Remediation Requests comment: Account:{ accountRec?.AccountName} is Inactive.Cannot create a Remediation Requests");
                    UpdateRemediationStausAsCompleted(review);
                    throw new Exception($"Remediation Requests comment: Account:{ accountRec?.AccountName} is Inactive.Cannot create a Remediation Requests");
                }
                if (review.AttestationSnapshot.EntitlementType != EntitlementType.BusinessRole && review.AttestationSnapshot.EntitlementType != EntitlementType.Account && validateAccount && (entitlementRecon == null || entitlementRecon.IsDeleted))
                {
                    if (string.IsNullOrWhiteSpace(review.Comment))
                    {
                        review.Comment = string.Empty;
                    }
                    review.Comment = review.Comment + Environment.NewLine + $"Remediation Requests comment by IDMS: Entitlement:{review.AttestationSnapshot.Name} is deleted. Cannot create a Remediation Requests";
                    DILog.AttestationLogger.WriteLog(LogLevelType.Exception, $"Remediation Requests comment: Entitlement:{ review.AttestationSnapshot.Name} is deleted.Cannot create a Remediation Requests");
                    UpdateRemediationStausAsCompleted(review);
                    throw new Exception($"Remediation Requests comment: Account:{ accountRec?.AccountName} is Inactive.Cannot create a Remediation Requests");
                }
                var ep = EndPoint.GetById(review.AttestationSnapshot.EndpointId);

                newRequest = new AccessRequest();
                newAdhocRequest = new AdHocAccessRequest();
                newRequest.UserId = review.AttestationSnapshot.UserId;
                newRequest.EndpointSystemId = review.AttestationSnapshot.EndpointId;

                if ((ep.ApplicationType == ApplicationType.InhouseSystems && ep.CanAutomate == false) || (ep.ApplicationType == ApplicationType.VPN && review.AttestationSnapshot.EntitlementType == EntitlementType.Account))
                {
                    newRequest.RequestStatus = AccessRequestStatus.Submitted;
                    newRequest.ProgressStatus = AccessRequestProgress.Submitted;

                }
                else
                {
                    newRequest.RequestStatus = AccessRequestStatus.Granted;
                    newRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                }

                newRequest.RequestSource = AccessRequestSource.AdhocRequest;
                newRequest.AccountType = review.AttestationSnapshot.AccountType;
                newRequest.ApplicationType = EndPoint.GetById(newRequest.EndpointSystemId.Value).ApplicationType;


                newRequest.RequesterId = (review.ReviewerId.HasValue && review.ReviewerId.Value >= 0) ? review.ReviewerId : 1;

                if (review.IsActionExtendable && review.EffectiveDate.HasValue)
                {
                    newRequest.DeactivationDate = review.EffectiveDate;
                    newRequest.ExecutionTime = review.EffectiveDate;
                    newRequest.ActivationDate = review.EffectiveDate;
                }
                else
                {
                    newRequest.ActivationDate = DateTime.Now;
                    newRequest.DeactivationDate = DateTime.Now;
                    newRequest.ExecutionTime = DateTime.Now;
                }

                newRequest.OrgCompany = usr.OrgCompany;
                newRequest.OrgDepartment = usr.OrgDepartment;
                newRequest.OrgSection = usr.OrgSection;
                newRequest.OrgUnit = usr.OrgUnit;
                newRequest.OrgBranch = usr.OrgBranch;

                newRequest.AttestationCampaignId = review.AttestationSnapshot.AttestationCampaignId;
                newRequest.Description = $"This request is created automatically for Attestation Remediation. AttestationCampaign : {review.AttestationSnapshot.AttestationCampaign?.CampaignName}.";
                try
                {
                    newRequest.Comments = AttestationSnapshotActionHistory.GetAttestationActionReviewComments(review.Id);
                    newRequest.Description = $"{newRequest.Description}{Environment.NewLine} {newRequest.Comments}";
                }
                catch (Exception ex)
                {
                    DILog.AttestationLogger.WriteLog(LogLevelType.Info, $"AttestationCampaignId {review.AttestationSnapshot.AttestationCampaignId} Error while fetching review comments", ex);
                }
                newRequest.TransactionId = transactionId;
                newRequest.AttestationReviewId = review.AttestationSnapshotId;

                //adhoc request
                newAdhocRequest.ApplicationType = newRequest.ApplicationType;
                newAdhocRequest.AccountType = newRequest.AccountType;
                newAdhocRequest.AdhocActionType = review.AttestationSnapshot.EntitlementType == EntitlementType.Account ? AdHocActionType.DeleteAccount : AdHocActionType.AmendmentRemove;
                newAdhocRequest.Comments = $"{newRequest.Description}";
                newAdhocRequest.EndpointSystemId = newRequest.EndpointSystemId;
                newAdhocRequest.EndpointSystemType = review.AttestationSnapshot.EndpointType;
                newAdhocRequest.CreatedBy = systemUser;
                newAdhocRequest.CreatedDate = DateTime.Now;
                newAdhocRequest.IsSelfRequest = false;
                newAdhocRequest.RequesteeUserId = review.AttestationSnapshot.UserId;
                //Added by Siva. -1 is irrelavant. Hence added IACMAdmin as Sysetm
                newAdhocRequest.RequesterUserId = User.GetByName("IACMAdmin").Id;
                if (review.ReviewerId.HasValue)
                    newAdhocRequest.RequesterUserId = review.ReviewerId.Value;
                newAdhocRequest.ValidTo = DateTime.Now;

                var detail = GetEntitlementsByType(accountRec, entitlementRecon, review, review.AttestationSnapshot.EntitlementType == EntitlementType.Account, (review.AttestationSnapshot.EntitlementType != EntitlementType.None && review.AttestationSnapshot.EntitlementType != EntitlementType.Account), usr);

                if (newAdhocRequest.ApplicationType == ApplicationType.Office365)
                {
                    if (review.AttestationSnapshot.AccountType == AccountType.Shared && review.AttestationSnapshot.EntitlementType == EntitlementType.Account)
                    {
                        //Office365 Shared account if its licenced then it should be routed to IT helpDeskAdmin for approval
                        var adhoc365 = detail as AdHocO365Request;
                        if (adhoc365 != null && adhoc365.MailBoxConversionType == MailBoxConversionType.UserToShared)
                        {
                            newRequest.RequestStatus = AccessRequestStatus.Submitted;
                            newRequest.ProgressStatus = AccessRequestProgress.Submitted;
                        }
                    }
                    newAdhocRequest.EntitlementDetail = AdHocAccessRequest.GetSerializedStringForO365(detail);
                }
                else if (newAdhocRequest.ApplicationType == ApplicationType.MicrosoftTeams)
                {
                    var sfbdetail = detail as AdhocSkypeForBusinessRequest;
                    if (sfbdetail.EntitlementType == EntitlementType.IDD)
                    {
                        newRequest.RequestStatus = AccessRequestStatus.Submitted;
                        newRequest.ProgressStatus = AccessRequestProgress.Submitted;
                    }
                    newAdhocRequest.EntitlementDetail = newAdhocRequest.GetSerializedString(detail);
                }
                else
                {
                    newAdhocRequest.EntitlementDetail = newAdhocRequest.GetSerializedString(detail);
                }
                try
                {
                    //If same request already exists then dont create a request again
                    if (IsSameRequestExists(newAdhocRequest, newRequest.UserId, newRequest.AttestationCampaignId))
                    {
                        UpdateRemediationStausAsCompleted(review);
                        return;
                    }
                }
                catch
                {
                }
                if(newAdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    try
                    {
                        var requestId = newAdhocRequest.IsRemediationDeleteAccountRequestExists(newAdhocRequest, newRequest.AttestationCampaignId, newRequest.UserId);
                        if (requestId != null && requestId.Value > 0)
                        {
                            UpdateRemediationStausAsCompleted(review);
                            DILog.AttestationLogger.WriteLog(LogLevelType.Warning, $"Delete Request already exists(RequestId:{requestId}) for this user:{usr.UserName} && Accountname:{accountRec?.AccountName}. Hence cannot create Amendment Remove request");
                            return;
                        }
                    }
                    catch
                    {
                    }
                }
                newAdhocRequest.Save(systemUser);
                newRequest.CreatedBy = systemUser;
                newRequest.CreatedDate = DateTime.Now;

                newRequest.AdHocAccessRequestId = newAdhocRequest.Id;
                newRequest.Save(systemUser);
                UpdateRemediationStausAsCompleted(review);

            }
            catch (OutOfMemoryException exception)
            {
                DILog.AttestationLogger.WriteLog(LogLevelType.Exception, $"{exception.Message}", exception);
                throw exception;
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(LogLevelType.Exception, string.Format("Exception thorwed while saving Request AttestationCampaignId. Reference AttestationCampaignId  - {0} Message:{1}", review.AttestationSnapshot.AttestationCampaignId, ex.Message), ex);
                throw new Exception($"Attestation Remediation request generation Failed:Revoked item: {review?.AttestationSnapshot?.Name} {ex?.Message}", ex);
            }
        }
        private static void UpdateRemediationStausAsCompleted(AttestationSnapshotAction action)
        {
            try
            {
                AttestationSnapshotAction.ExecuteQuery($"UPDATE DI_M_AttestationSnapshotAction Set RemediationStatus = 3 WHERE ID = {action.Id}");
                if (action.AttestationSnapshot.EntitlementType == EntitlementType.Account)
                {
                    AttestationSnapshotAction.ExecuteQuery($"Update DI_M_AttestationSnapshotAction Set RemediationStatus = 3 WHERE AttestationSnapshotId IN(SELECT ID FROM DI_M_AttestationSnapshot WHERE EntitlementType != 14 AND ParentEntity = '{action.AttestationSnapshot.Entity}' AND AttestationCampaignId={action.AttestationSnapshot.AttestationCampaignId})");
                }
            }
            catch
            {
            }
        }
        /// <summary>
        /// This method is for remediation request generation. Before we create the request check already exists or not.
        /// </summary>
        /// <param name="adHocAccessRequest"></param>
        /// <param name="userId"></param>
        /// <param name="campaignId"></param>
        /// <returns></returns>
        private static bool IsSameRequestExists(AdHocAccessRequest adHocAccessRequest, long userId, long? campaignId)
        {
            bool isExists = false;
            try
            {
                var existingRequest = DomainModel.AdHocAccessRequest.IsSameRemediationRequestPending(adHocAccessRequest, campaignId, userId);
                if (!String.IsNullOrEmpty(existingRequest))
                {
                    isExists = true;
                    var RequesterID = Convert.ToInt64(existingRequest.Split(';')[0]);
                    var AccessRequestID = Convert.ToInt64(existingRequest.Split(';')[1]);
                    var RequesterName = User.GetById(RequesterID)?.DisplayName;
                    DILog.AttestationLogger.WriteLog(LogLevelType.Info, $"Same access already created for the campaignId:{campaignId}. Access Request Id {AccessRequestID}");
                }
            }
            catch (Exception ex)
            {
                //DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, $"IsSameRequestExists failed", ex);
            }
            return isExists;
        }

        static IEntitlementDetail GetEntitlementsByType(AccountReconcilation accountRecon, EntitlementReconcilation entitlementRecon, AttestationSnapshotAction review, bool isAccountRevoked, bool isEntitlementRevoked, User usr)
        {
            var entitlement = review.AttestationSnapshot.Name;
            IEntitlementDetail entitlementDetail = null;
            var ep = EndPoint.GetById(review.AttestationSnapshot.EndpointId);
            var epUserId = accountRecon != null ? accountRecon.EndpointUserId : (entitlementRecon.EndpointUserId);
            switch (ep.ApplicationType)
            {
                case ApplicationType.ADSingaporePowerLocal:
                    {
                        AdHocADRequest adhocAD = new AdHocADRequest();
                        adhocAD.EndpointUserId = epUserId;
                        var adUsr = ADSUserInfo.GetById(epUserId);
                        adhocAD.DisplayAccountName = adUsr?.AccountName;
                        adhocAD.EntitlementType = EntitlementType.Group;
                        //adhocAD.RequesterComment = $"Attestation Campaign:{review.AttestationSnapshot.AttestationCampaign?.CampaignName}/Id:{review.AttestationSnapshot.AttestationCampaignId}{Environment.NewLine} Comments:{review.Comment}";
                        adhocAD.Entitlement = isAccountRevoked == false ? entitlement : string.Empty;
                        entitlementDetail = adhocAD;
                    }
                    break;
                case ApplicationType.SAPLocal:
                    {
                        AdHocSAPLocalRequest ADSapLocalRequest = new AdHocSAPLocalRequest();
                        ADSapLocalRequest.EndpointUserId = epUserId;
                        var adUsr = ADSUserInfo.GetById(epUserId);
                        ADSapLocalRequest.DisplayAccountName = adUsr?.AccountName;
                        ADSapLocalRequest.EntitlementType = EntitlementType.Group;
                        //ADSapLocalRequest.RequesterComment = $"Attestation Campaign:{review.AttestationSnapshot.AttestationCampaign?.CampaignName}/Campaign Id:{review.AttestationSnapshot.AttestationCampaignId}{Environment.NewLine} Comments:{review.Comment}";
                        ADSapLocalRequest.Entitlement = isAccountRevoked == false ? entitlement : string.Empty;
                        entitlementDetail = ADSapLocalRequest;
                    }
                    break;
                case ApplicationType.HRMS_FLINS:
                    #region FLINS
                    if (isEntitlementRevoked)
                    {
                        throw new NotImplementedException($"Attestation for {ApplicationType.HRMS_FLINS} does not support entitlement removal");
                    }
                    var sapFlinsUsr = SAPUserinfo.GetById(epUserId);
                    AdHocSAPHRFLINSRequest flinsReq = new AdHocSAPHRFLINSRequest();
                    flinsReq.EndpointUserId = epUserId;
                    flinsReq.DisplayAccountName = sapFlinsUsr.AccountName;
                    entitlementDetail = flinsReq;
                    #endregion
                    break;
                case ApplicationType.FLINS_BI:
                    #region BI
                    var sapBIUsr = SAPUserinfo.GetById(epUserId);
                    AdHocSAPFLINSBIRequest sapBIReq = new AdHocSAPFLINSBIRequest();
                    sapBIReq.EndpointUserId = epUserId;
                    sapBIReq.DisplayAccountName = sapBIUsr.AccountName;
                    if (isEntitlementRevoked)
                    {
                        var sapMatrix = SAPBIRoleCategory.GetFLINSBIMatrixByRequest(entitlement);
                        sapBIReq.Module = sapMatrix.Module;
                        sapBIReq.SubModule = sapMatrix.SubModule;
                        sapBIReq.EntitlementType = EntitlementType.Role;
                        sapBIReq.Entitlement = entitlement;
                    }
                    entitlementDetail = sapBIReq;
                    #endregion
                    break;
                case ApplicationType.SAP_GRC:
                    #region GRC
                    AdHocSAPGRCRequest sapReq = new AdHocSAPGRCRequest();
                    var sapUsr = SAPUserinfo.GetById(epUserId);
                    sapReq.EndpointUserId = epUserId;
                    sapReq.DisplayAccountName = sapUsr.AccountName;
                    if (isEntitlementRevoked)
                    {
                        var sapMatrix = SAPGRCRole.GetGRCMatrixByRequest(entitlement);
                        if (sapMatrix == null)
                        {
                            throw new EntityNotFoundException($"GRC Matrix Data:{entitlement} not found.");
                        }
                        sapReq.Module = sapMatrix.Module;
                        sapReq.SubModule = sapMatrix.SubModule;
                        sapReq.EntitlementType = EntitlementType.Role;
                        sapReq.Entitlement = entitlement;
                    }
                    entitlementDetail = sapReq;
                    #endregion
                    break;
                case ApplicationType.MSSL_ISU:
                case ApplicationType.MSSL_CRM:
                    #region MSSL
                    AdHocMSSLRequest msslRequest = new AdHocMSSLRequest();
                    var msslUsr = SAPUserinfo.GetById(epUserId);
                    if (msslUsr == null)
                    {
                        throw new Exception($"{ep.ApplicationType} account doesn't exists.");
                    }

                    msslRequest.EndpointUserId = msslUsr.Id;
                    msslRequest.DisplayAccountName = accountRecon.AccountName;
                    msslRequest.AccName = accountRecon.AccountName;
                    msslRequest.ApplicationName = ep.ApplicationType == ApplicationType.MSSL_ISU ? Constants.MSSLMSP : ep.ApplicationType == ApplicationType.MSSL_CRM ? Constants.MSSLMCP : Constants.MSSLBI;
                    if (isEntitlementRevoked)
                    {
                        if (review.AttestationSnapshot.EntitlementType == EntitlementType.Position)
                        {
                            var matrix = SAPXLMSSLPositionsMatrix.GetMSSLPositionMatrixByPosition(entitlement);
                            msslRequest.Position = review.AttestationSnapshot.Description;
                            if (matrix != null)
                            {
                                msslRequest.PositionPrefix = usr.FullTimeEmployee ? matrix.Prefix_Employees : matrix.Prefix_TempStaff;
                                msslRequest.PositionValue = $"{matrix?.Id}";
                            }
                        }
                        else if (review.AttestationSnapshot.EntitlementType == EntitlementType.BusinessRole)
                        {
                            msslRequest.Roles = entitlement;
                        }
                    }
                    entitlementDetail = msslRequest;
                    #endregion
                    break;
                case ApplicationType.EBS_ISU:
                case ApplicationType.EBS_CRM:
                    #region EBS
                    AdHocEBSRequest ebsRequest = new AdHocEBSRequest();
                    var ebsUsr = SAPUserinfo.GetById(epUserId);
                    if (ebsUsr == null)
                    {
                        throw new Exception($"{ep.ApplicationType} account doesn't exists.");
                    }

                    ebsRequest.EndpointUserId = ebsUsr.Id;
                    ebsRequest.DisplayAccountName = accountRecon.AccountName;
                    //ebsRequest.AccName = accountRecon.AccountName;
                    ebsRequest.ApplicationName = ep.ApplicationType == ApplicationType.EBS_ISU ? Constants.EBSISP : ep.ApplicationType == ApplicationType.EBS_CRM ? Constants.EBSCRP : Constants.EBSBI;
                    if (isEntitlementRevoked)
                    {
                        if (review.AttestationSnapshot.EntitlementType == EntitlementType.Position)
                        {
                            var matrix = SAPEBSXLPositionsMatrix.GetEBSPositionMatrixByPosition(entitlement);
                            ebsRequest.Position = review.AttestationSnapshot.Description;
                            if (matrix != null)
                            {
                                ebsRequest.PositionPrefix = usr.FullTimeEmployee ? matrix.Prefix_Employees : matrix.Prefix_TempStaff;
                                ebsRequest.PositionValue = $"{matrix?.Id}";
                            }
                        }
                        else if (review.AttestationSnapshot.EntitlementType == EntitlementType.BusinessRole)
                        {
                            ebsRequest.Roles = entitlement;
                        }
                    }
                    entitlementDetail = ebsRequest;
                    #endregion
                    break;
                case ApplicationType.SAP_SRP:
                    break;
                case ApplicationType.SAP_SUS:
                    break;
                case ApplicationType.SAP_EP:
                case ApplicationType.CE:
                    #region SAPPortal
                    var portalUsr = SAPEPUserInfo.GetById(epUserId);
                    if (portalUsr == null)
                    {
                        DILog.AttestationLogger.WriteLog(LogLevelType.Info, $"SAPEP Account({entitlement}) not found");
                        throw new Exception($"SAPEP Account({entitlement}) not found");
                    }
                    var portalRequest = new AdhocSAPEPRequest();
                    portalRequest.DisplayAccountName = portalUsr.AccountName;
                    portalRequest.EndpointUserId = epUserId;
                    if (isEntitlementRevoked && entitlementRecon != null)
                    {
                        if (entitlementRecon.EntitlementType == (int)EntitlementType.Group)
                            portalRequest.Groups = entitlement;
                        if (entitlementRecon.EntitlementType == (int)EntitlementType.Role)
                        {
                            var sapRoleInfo = SAPEPRoleInfo.GetByRoleName(accountRecon.EndpointSystemId, entitlement)?.FirstOrDefault(i => !string.IsNullOrWhiteSpace(i.UniqueRoleName));
                            if (sapRoleInfo == null) throw new Exception($"Cannot find {ep.ApplicationType} role. Please check discovery/ upload whether uniquename is empty for the role:{entitlement}");
                            portalRequest.Roles = sapRoleInfo.UniqueRoleName;
                        }
                    }
                    portalRequest.RequestDetail = $"Attestation Campaign:{review.AttestationSnapshot.AttestationCampaign?.CampaignName}/Campaign Id:{review.AttestationSnapshot.AttestationCampaignId}{Environment.NewLine} Comments:{review.Comment}";
                    entitlementDetail = portalRequest;
                    #endregion
                    break;
                case ApplicationType.NPAC:
                    #region npac
                    var npacUsr = NPACUserInfo.GetByUserByEndpointSystemAndId(review.AttestationSnapshot.EndpointId, epUserId);
                    if (npacUsr == null)
                    {
                        DILog.AttestationLogger.WriteLog(LogLevelType.Info, $"NPAC Account({entitlement}) not found");
                        throw new Exception($"NPAC Account({entitlement}) not found");
                    }
                    var npacRequest = new AdHocNPACRequest();
                    npacRequest.EndpointUserId = epUserId;
                    npacRequest.DisplayAccountName = npacUsr?.AccountName;
                    if (isEntitlementRevoked)
                    {
                        npacRequest.EntitlementType = EntitlementType.Level;
                        npacRequest.Entitlement = entitlement;
                    }
                    entitlementDetail = npacRequest;
                    #endregion
                    break;
                case ApplicationType.GEMS:
                    #region Gems
                    var gemsUsr = GEMSUserInfo.GetByUserByEndpointSystemAndId(epUserId, review.AttestationSnapshot.EndpointId);
                    if (gemsUsr == null)
                    {
                        DILog.AttestationLogger.WriteLog(LogLevelType.Info, $"GEMS Account({entitlement}) not found");
                        throw new Exception($"GEMS Account({entitlement}) not found");
                    }
                    AdHocGEMSRequest gemsRequest = new AdHocGEMSRequest();
                    gemsRequest.EndpointUserId = epUserId;
                    gemsRequest.DisplayAccountName = gemsUsr?.AccountName;
                    gemsRequest.EntitlementType = EntitlementType.Role;
                    gemsRequest.Entitlement = isAccountRevoked == false ? entitlement : string.Empty;
                    entitlementDetail = gemsRequest;
                    #endregion
                    break;
                case ApplicationType.VPN:
                    #region VPN
                    var vpnUsr = VPNUserInfo.GetByOnekeyTokenId(accountRecon.AccountName);
                    if (vpnUsr == null)
                    {
                        DILog.AttestationLogger.WriteLog(LogLevelType.Info, $"VPN Account({entitlement}) not found");
                        throw new Exception($"VPN Account({entitlement}) not found");
                    }
                    if (review.AttestationSnapshot.UserId != vpnUsr.UserId)
                    {
                        DILog.AttestationLogger.WriteLog(LogLevelType.Info, $"VPN Token({entitlement}) is reused to another User{vpnUsr.DisplayName}({vpnUsr.UserName}). Hence cannot be revoked for {review.AttestationSnapshot.DIUserDisplayName}");
                        throw new Exception($"VPN Account({entitlement}) not found");
                    }
                    var vpnReq = new AdHocVPNRequest();
                    vpnReq.DisplayAccountName = vpnUsr.OneKeyTokenId;
                    vpnReq.EndpointUserId = vpnUsr.Id;
                    vpnReq.OneKey = vpnUsr.OneKeyTokenId;
                    if (isEntitlementRevoked)
                    {
                        bool production = false, qaEnv = false, development = false, project = false;

                        var vpntype = (VPNMatrixEnviornmentType)Enum.Parse(typeof(VPNMatrixEnviornmentType), entitlementRecon.Functions);
                        if (vpntype == VPNMatrixEnviornmentType.ADGroup)
                        {
                            project = true;
                        }
                        else if (vpntype == VPNMatrixEnviornmentType.Production)
                        {
                            production = true;
                        }
                        else if (vpntype == VPNMatrixEnviornmentType.Development)
                        {
                            development = true;
                        }
                        else if (vpntype == VPNMatrixEnviornmentType.QA)
                        {
                            qaEnv = true;
                        }

                        var vpnMatrixType = review.AttestationSnapshot.EntitlementType == EntitlementType.Project ? VPNMatrixType.Project : VPNMatrixType.Application;

                        var ent = entitlement;
                        if (project != true)
                        {
                            ent = entitlement.Substring(0, entitlement.LastIndexOf('-'));
                        }
                        var matrix = VPNMatrix.GetByName(ent, vpnMatrixType);
                        if (matrix == null)
                        {
                            DILog.AttestationLogger.WriteLog(LogLevelType.Info, $"VPNMatrix data({entitlement}) not found");
                            throw new Exception($"VPN Matrix Data({entitlement}) not found");
                        }

                        vpnReq.AdHocVPNRequestDetails.Add(new AdHocVPNRequestDetail()
                        {
                            VPNMatrixId = matrix.Id,
                            MatrixType = vpnMatrixType.ToString(),
                            VPNType = review.AttestationSnapshot.EntitlementType == EntitlementType.Project ? VPNType.VDI : VPNType.SSLVPNOnly,
                            ApplicationName = matrix.Name,
                            QA = qaEnv,
                            QAADgroups = qaEnv ? matrix.QAADGroups : null,
                            Production = production,
                            ProductionADGroups = production ? matrix.ProductionADgroups : null,
                            Dev = development,
                            DevelopmentADGroups = development ? matrix.DevelpomentADGroups : null,
                            isOthers = project,
                            ProjectADGroups = project ? matrix.ProductionADgroups : null
                        });
                    }
                    entitlementDetail = vpnReq;
                    #endregion
                    break;
                case ApplicationType.Office365:
                    #region Office365
                    var o365user = Office365UserInfo.GetById(epUserId);
                    if (o365user == null)
                    {
                        DILog.AttestationLogger.WriteLog(LogLevelType.Info, $"Office365 Account({entitlement}) not found");
                        throw new Exception($"Office365 Account({entitlement}) not found");
                    }
                    var office365 = new AdHocO365Request();
                    office365.EndpointUserId = epUserId;
                    office365.DisplayAccountName = o365user.AccountName;
                    if (accountRecon.AccountType != AccountType.User)
                        office365.MailBoxConversionType = o365user.ServicePlanObject != null ? MailBoxConversionType.UserToShared : MailBoxConversionType.SharedToUser;
                    entitlementDetail = office365;
                    #endregion
                    break;
                case ApplicationType.OneCard:
                    #region OneCard
                    var smsUserInfo = SMSUserInfo.GetById(epUserId);
                    if (smsUserInfo == null)
                    {
                        DILog.AttestationLogger.WriteLog(LogLevelType.Info, $"OneCard Account({entitlement}) not found");
                        throw new Exception($"OneCard Account({entitlement}) not found");
                    }
                    AdHocSMSRequest smsRequest = new AdHocSMSRequest();
                    smsRequest.DisplayAccountName = smsUserInfo.AccountName;
                    smsRequest.EndpointUserId = smsUserInfo.Id;
                    smsRequest.EntitlementType = EntitlementType.AccessLevel;
                    smsRequest.GridInfo = new List<AdhocSmsGridInfo>();
                    if (isEntitlementRevoked)
                    {
                        var matrix = SMSXLRoleCatalogue.GetBuildingandSectionByTechRoles(entitlement);
                        if (matrix != null)
                        {
                            smsRequest.GridInfo.Add(new AdhocSmsGridInfo() { Building = matrix.Building, Section = matrix.Section, Description = matrix.Description, TechnicalRole = matrix.TechnicalRole });
                            smsRequest.Entitlement = matrix.TechnicalRole;
                        }
                        else
                        {
                            DILog.AttestationLogger.WriteLog(LogLevelType.Info, $"Onecard Level {entitlement} matrix not found.");
                            throw new Exception($"Onecard Level {entitlement} matrix not found.");
                        }
                    }
                    entitlementDetail = smsRequest;
                    #endregion
                    break;
                case ApplicationType.SRM:
                    #region SRM
                    AdHocSAPSRMRequest srmRequest = new AdHocSAPSRMRequest();
                    var srmUsr = SAPUserinfo.GetById(epUserId);
                    srmRequest.AccountName = srmUsr.UserLogonName;
                    srmRequest.EndpointUserId = epUserId;
                    srmRequest.DisplayAccountName = srmUsr.UserLogonName;
                    if (isEntitlementRevoked)
                    {
                        var snapShot = review.AttestationSnapshot;
                        var retain = AttestationSnapshot.IsAllRoleSelectedForSRM(snapShot.AttestationCampaignId, snapShot.AccountName, snapShot.Description, snapShot.EntitlementType);
                        srmRequest.AdHocSAPSRMRequestDetails = new List<AdHocSAPSRMRequestDetail>();
                        srmRequest.AdHocSAPSRMRequestDetails.Add(new AdHocSAPSRMRequestDetail()
                        {
                            Entitlement = review.AttestationSnapshot.Description,
                            EntitlementType = EntitlementType.Role,
                            PurchaseOrg = review.AttestationSnapshot.Reserved5,
                            PurchaseGroup = review.AttestationSnapshot.Reserved6,
                            RetainMainRole = retain
                        });
                    }
                    entitlementDetail = srmRequest;
                    #endregion
                    break;
                case ApplicationType.InhouseSystems:
                    #region Inhouse
                    {
                        var userInfo = InhouseUserInfo.GetById(epUserId);
                        if (userInfo == null)
                        {
                            DILog.AttestationLogger.WriteLog(LogLevelType.Info, $"Inhouse System Account({entitlement}) not found");
                            throw new Exception($"Inhouse System Account({entitlement}) not found");
                        }
                        AdHocINHSharedAccount sharedAccount = null;
                        var isSepecialAccount = accountRecon.AccountType == AccountType.Secondary || accountRecon.AccountType == AccountType.Service || accountRecon.AccountType == AccountType.Shared;
                        if (isSepecialAccount)
                        {
                            sharedAccount = new AdHocINHSharedAccount();
                            sharedAccount.AccountName = accountRecon.AccountName;
                            sharedAccount.DisplayName = accountRecon.FullName;
                            sharedAccount.Description = sharedAccount.Description;
                        }
                        entitlementDetail = new AdHocINHRequest
                        {
                            EntitlementType = EntitlementType.Permission,
                            EndpointUserId = epUserId,
                            AdHocINHSharedAccount = sharedAccount,
                            Entitlement = isAccountRevoked == false ? entitlement : string.Empty
                        };
                        entitlementDetail.DisplayAccountName = userInfo?.AccountName;
                    }
                    #endregion
                    break;
                case ApplicationType.ADSDCLocal:
                    break;
                case ApplicationType.IDMS:
                    #region IDMS
                    AdHocIACMRequest request = new AdHocIACMRequest();
                    request.EndpointUserId = epUserId;
                    var epUsr = IACMUserInfo.GetById(epUserId);
                    request.DisplayAccountName = epUsr.AccountName;
                    request.AdHocIACMRequestDetails = new List<AdHocIACMRequestDetail>();
                    if (isEntitlementRevoked)
                    {
                        var requestDetail = new List<AdHocIACMRequestDetail>();
                        requestDetail.Add(new AdHocIACMRequestDetail()
                        {
                            Entitlement = entitlement,
                            EntitlementType = review.AttestationSnapshot.EntitlementType
                        });
                        request.AdHocIACMRequestDetails.AddRange(requestDetail);
                    }
                    entitlementDetail = request;
                    #endregion
                    break;
                case ApplicationType.SSIDS_GUI:
                case ApplicationType.SSIDS_SMS:
                    #region SSIDS
                    var ssidsAdhocRequest = new AdHocSSIDSRequest();
                    ssidsAdhocRequest.EndpointUserId = epUserId;
                    ssidsAdhocRequest.DisplayAccountName = accountRecon.AccountName;
                    if (isEntitlementRevoked)
                    {
                        if (ep.ApplicationType == ApplicationType.SSIDS_GUI)
                        {
                            ssidsAdhocRequest.EntitlementType = EntitlementType.Role;
                            ssidsAdhocRequest.Entitlement = entitlement;
                        }
                        else
                        {
                            ssidsAdhocRequest.EntitlementType = EntitlementType.HandPhone;
                            ssidsAdhocRequest.Entitlement = string.Empty;
                        }
                    }
                    entitlementDetail = ssidsAdhocRequest;
                    #endregion
                    break;
                case ApplicationType.EAM:
                    var eamRequest = new AdHocNovaRequest();
                    eamRequest.EndpointUserId = epUserId;
                    var eamUser = SAPEamNovaUserInfo.GetById(epUserId);
                    if (eamUser != null)
                    {
                        eamRequest.DisplayAccountName = eamUser.AccountName;
                    }
                    if (isEntitlementRevoked)
                    {
                        var entitlementReconcilation = EntitlementReconcilation.GetById(review.AttestationSnapshot.KeyId);
                        var matrix = SAPEamNovaUserRoleInfo.GetById(entitlementReconcilation.EntitlementId);
                        eamRequest.RequestDetails = new List<AdHocNovaRequestDetails>();
                        var reqDetail = new AdHocNovaRequestDetails();
                        reqDetail.NovaMatrixType = matrix.NovaPositionMatrixType;
                        if (reqDetail.NovaMatrixType != NovaMatrixType.Exceptional)
                        {
                            if (reqDetail.NovaMatrixType == NovaMatrixType.User)
                            {
                                reqDetail.JobDescription = NOVAUserRoleMatrix.Get(i => i.Position == matrix.RoleName && i.CompanyCode == matrix.Company && i.SectionCode == matrix.Section, false, 0, 0).FirstOrDefault()?.JobDescription;

                            }
                            else if (reqDetail.NovaMatrixType == NovaMatrixType.Management)
                            {
                                reqDetail.JobDescription = NOVAUserRoleMatrix.Get(i => i.Position == matrix.RoleName && i.CompanyCode == matrix.Company && i.SectionCode == matrix.Section, false, 0, 0).FirstOrDefault()?.JobDescription;
                                reqDetail.DepartmentCode = matrix.DepartmentBranch;
                            }
                        }
                        else
                        {
                            reqDetail.JobDescription = entitlementRecon.EntitlementDescription;
                        }
                        reqDetail.RoleName = matrix.RoleName;
                        reqDetail.CompanyCode = matrix.Company;
                        reqDetail.SectionCode = matrix.Section;
                        reqDetail.CompanyName = usr?.CompanyName;
                        reqDetail.SectionName = usr?.SectionName;
                        eamRequest.RequestDetails.Add(reqDetail);
                    }
                    entitlementDetail = eamRequest;
                    break;
                case ApplicationType.MicrosoftTeams:
                    #region SFB
                    var sfbUsr = SkypeForBusinessUserInfo.GetById(epUserId);
                    var sfbRequest = new AdhocSkypeForBusinessRequest();
                    sfbRequest.EndpointUserId = epUserId;
                    sfbRequest.DisplayAccountName = sfbUsr.SamAccountName;
                    sfbRequest.DisplayCurrentLineURI = sfbUsr.AnyOneLineURI;
                    if (isAccountRevoked)
                        sfbRequest.EntitlementType = sfbUsr.UserType;

                    if (isEntitlementRevoked)
                    {
                        ///Support only removal of ResponseGroup, IDD,DCC and DCCS
                        if (review.AttestationSnapshot.EntitlementType == EntitlementType.ResponseGroup)
                        {
                            sfbRequest.EntitlementType = review.AttestationSnapshot.EntitlementType;
                            var group = SkypeForBusinessResponseGroupInfo.GetGroupByName(entitlement);
                            if (group == null)
                            {
                                throw new EntityNotFoundException($"Response Group: {entitlement} does not exists.");
                            }
                            var responseGroup = new ResponseGroup();
                            responseGroup.Id = group.Id;
                            responseGroup.Identity = group.Identity;
                            responseGroup.Name = group.Name;
                            responseGroup.Mask = false;
                            sfbRequest.SkypeForBusinessResponseGroups.Add(responseGroup);
                        }
                        else if (review.AttestationSnapshot.EntitlementType == EntitlementType.IDD || review.AttestationSnapshot.EntitlementType == EntitlementType.DCC
                            || review.AttestationSnapshot.EntitlementType == EntitlementType.DCCS
                            )
                        {
                            sfbRequest.EntitlementType = review.AttestationSnapshot.EntitlementType;
                        }
                        else
                        {
                            throw new NotImplementedException($"Attestation for {ApplicationType.MicrosoftTeams} does not support entitlement Remove of type:{review.AttestationSnapshot.EntitlementType}");
                        }
                        entitlementDetail = sfbRequest;
                    }
                    #endregion
                    break;
                case ApplicationType.EBS:
                case ApplicationType.MSSL:
                    #region EBS
                    var sapHANARequest = new AdHocSAPHANARequest();
                    var hanaUser = SAPHANAUserinfo.GetById(epUserId);
                    if (hanaUser == null)
                    {
                        throw new Exception($"{ep.ApplicationType} account doesn't exists.");
                    }

                    sapHANARequest.EndpointUserId = hanaUser.Id;
                    sapHANARequest.DisplayAccountName = accountRecon.AccountName;
                    //ebsRequest.AccName = accountRecon.AccountName;
                    sapHANARequest.ApplicationName = ep.ApplicationCode;
                    if (isEntitlementRevoked)
                    {
                        if (review.AttestationSnapshot.EntitlementType == EntitlementType.Position)
                        {
                            var matrix = SAPHANAXLPositionsMatrix.GetSAPHANAPositionMatrixByPosition(ep.Id, entitlement);
                            sapHANARequest.Position = review.AttestationSnapshot.Description;
                            if (matrix != null)
                            {
                                sapHANARequest.PositionPrefix = usr.FullTimeEmployee ? matrix.Prefix_Employees : matrix.Prefix_TempStaff;
                                sapHANARequest.PositionValue = $"{matrix?.Id}";
                            }
                        }
                        else if (review.AttestationSnapshot.EntitlementType == EntitlementType.BusinessRole)
                        {
                            sapHANARequest.Roles = entitlement;
                        }
                    }
                    entitlementDetail = sapHANARequest;
                    #endregion
                    break;
                default:
                    break;
            }
            return entitlementDetail;
        }

        static int PopulateOffBoardAccessRequests(User user, EmployeeEventTrigger trigger)
        {
            List<AccessRequest> requests = new List<AccessRequest>();
            AccessRequest newRequest = new AccessRequest();
            Guid transactionId = Guid.NewGuid();
            IList<AccountReconcilation> listAcs = null;
            List<AccountReconcilation> nonInhouseAccounts = null;
            List<AccountReconcilation> inhouseAccounts = null;
            try
            {
                listAcs = AccountReconcilation.GetAllByUser(user.Id, true);

                if (listAcs != null && listAcs.Count > 0)
                {
                    nonInhouseAccounts = new List<AccountReconcilation>();
                    nonInhouseAccounts = listAcs.Where(t => t.IsLocked == false && t.IsDeleted == false &&
                            (t.AccountType == AccountType.User || t.AccountType == AccountType.Secondary)
                            && t.EndPoint.EndpointSystemTypeId.Value != (long)EndpointType.InhouseSystems)
                            .OrderByDescending(a => a.EndpointSystemId).ToList();

                    foreach (var nonInhouseAccount in nonInhouseAccounts)
                    {
                        newRequest = new AccessRequest();

                        newRequest.UserId = user.Id;
                        newRequest.EndpointSystemId = nonInhouseAccount.EndpointSystemId;

                        newRequest.RequestStatus = AccessRequestStatus.Granted;
                        newRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;

                        newRequest.RequesterId = null;

                        newRequest.RequestSource = AccessRequestSource.OffBoarding;

                        newRequest.AccountType = nonInhouseAccount.AccountType;
                        newRequest.ApplicationType = EndPoint.GetById(nonInhouseAccount.EndpointSystemId).ApplicationType;

                        newRequest.ExecutionTime = DateTime.Now;
                        newRequest.Description = "This request is created automatically for Off-Boarding.";
                        newRequest.Remarks = string.Empty;
                        newRequest.TransactionId = transactionId;
                        newRequest.EmployeeEventTriggerId = trigger.Id;
                        newRequest.DeactivationDate = trigger.EffectiveDate;

                        if (AccessRequest.IsSameOffboardAccessRequestExistsForUser(newRequest.RequestSource, newRequest.UserId, newRequest.EmployeeEventTriggerId.Value, newRequest.EndpointSystemId.Value, newRequest.DeactivationDate, newRequest.ApplicationType, newRequest.AccountType))
                        {
                            DILog.ProvisioningLogger.WriteLog(LogLevelType.Info, $"Same OffBoard AccessRequest already exists for this user:{user.UserName} application:{newRequest.ApplicationType} TriggerId:{trigger.Id}");
                            continue;
                        }
                        requests.Add(newRequest);
                    }

                    inhouseAccounts = new List<AccountReconcilation>();
                    inhouseAccounts = listAcs.Where(t => t.IsLocked == false && t.IsDeleted == false
                                        && (t.AccountType == AccountType.User || t.AccountType == AccountType.Secondary)
                                        && t.EndPoint.EndpointSystemTypeId.Value == (long)EndpointType.InhouseSystems).ToList();

                    foreach (var inhouseAccount in inhouseAccounts)
                    {
                        var endpoint = EndPoint.GetById(inhouseAccount.EndpointSystemId);
                        newRequest = new AccessRequest();

                        newRequest.UserId = user.Id;
                        newRequest.EndpointSystemId = inhouseAccount.EndpointSystemId;

                        newRequest.RequestStatus = AccessRequestStatus.Granted;
                        newRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;

                        newRequest.RequesterId = null;

                        newRequest.RequestSource = AccessRequestSource.OffBoarding;

                        newRequest.AccountType = inhouseAccount.AccountType;
                        newRequest.ApplicationType = endpoint.ApplicationType;

                        newRequest.ExecutionTime = DateTime.Now;
                        newRequest.Description = "This request is created automatically for Off-Boarding.";
                        newRequest.Remarks = string.Empty;
                        newRequest.TransactionId = transactionId;
                        newRequest.EmployeeEventTriggerId = trigger.Id;
                        newRequest.DeactivationDate = trigger.EffectiveDate;
                        IEntitlementDetail adhocEntitlmentDetails = new AdHocINHRequest() { ApplicationCode = endpoint.ApplicationCode, EndpointUserId = inhouseAccount.EndpointUserId, DisplayAccountName = inhouseAccount.AccountName, EntitlementType = EntitlementType.Permission };
                        var adhocReq = new AdHocAccessRequest() { ApplicationType = ApplicationType.InhouseSystems };
                        newRequest.AdditionalInfo = adhocReq.GetSerializedString(adhocEntitlmentDetails);

                        if(CheckSameAccessRequest(inhouseAccount, newRequest))
                        {
                            DILog.ProvisioningLogger.WriteLog(LogLevelType.Info, $"Same OffBoard AccessRequest already exists for this user:{user.UserName} application:{newRequest.ApplicationType} TriggerId:{trigger.Id}");
                            continue;
                        }
                        requests.Add(newRequest);
                    }
                }

                AccessRequest.AddAccessRequest(requests);
                return requests.Count;

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                nonInhouseAccounts?.Clear();
                nonInhouseAccounts = null;

                inhouseAccounts?.Clear();
                inhouseAccounts = null;

                listAcs?.Clear();
                listAcs = null;
            }
        }

        private static bool CheckSameAccessRequest(AccountReconcilation inhouseAccount, AccessRequest newRequest)
        {
            bool IsSameRequestExists = false;
            try
            {
                var reqs = AccessRequest.SameOffboardAccessRequestExistsForUser(newRequest.RequestSource, newRequest.UserId, newRequest.EmployeeEventTriggerId.Value, newRequest.EndpointSystemId.Value, newRequest.DeactivationDate, newRequest.ApplicationType, newRequest.AccountType);

                foreach (var req in reqs)
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(req.AdditionalInfo))
                        {
                            var adhoc = new AdHocAccessRequest() { ApplicationType = req.ApplicationType };
                            adhoc.EntitlementDetail = req.AdditionalInfo;
                            var entitlementDetaisl = adhoc.GetEntitlementDetail();
                            if (entitlementDetaisl != null && entitlementDetaisl.EndpointUserId > 0 && entitlementDetaisl.EndpointUserId == inhouseAccount.EndpointUserId)
                            {
                                IsSameRequestExists = true;
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.ProvisioningLogger.WriteLog(LogLevelType.Info, $"CheckSameAccessRequest failed:{ex} for this user:{inhouseAccount?.AccountName} application:{newRequest?.ApplicationType} TriggerId:{newRequest?.EmployeeEventTriggerId}");
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(LogLevelType.Info, $"CheckSameAccessRequest failed:{ex} for this user:{inhouseAccount?.AccountName} application:{newRequest?.ApplicationType} TriggerId:{newRequest?.EmployeeEventTriggerId}");
            }

            return IsSameRequestExists;
        }

        static int PopulateOUChangeAccessRequests(User user, EmployeeEventTrigger trigger)
        {
            List<AccessRequest> requests = new List<AccessRequest>();
            AccessRequest newRequest = new AccessRequest();
            Guid transactionId = Guid.NewGuid();
            IList<AccountReconcilation> listAcs = null;
            List<AccountReconcilation> nonInhouseAccounts = null;
            List<AccountReconcilation> inhouseAccounts = null;
            try
            {
                listAcs = AccountReconcilation.GetAllByUser(user.Id, true);

                if (listAcs != null && listAcs.Count > 0)
                {
                    nonInhouseAccounts = new List<AccountReconcilation>();

                    nonInhouseAccounts = listAcs.Where(t => t.IsLocked == false && t.IsDeleted == false &&
                            (t.AccountType == AccountType.User || t.AccountType == AccountType.Secondary)
                            && t.EndPoint.EndpointSystemTypeId.Value != (long)EndpointType.InhouseSystems)
                            .OrderByDescending(a => a.EndPoint.EndpointSystemTypeId.Value).ToList();

                    foreach (var nonInhouseAccount in nonInhouseAccounts)
                    {
                        newRequest = new AccessRequest();

                        newRequest.UserId = user.Id;
                        newRequest.EndpointSystemId = nonInhouseAccount.EndpointSystemId;

                        newRequest.RequestStatus = AccessRequestStatus.Granted;
                        newRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;

                        newRequest.RequestSource = AccessRequestSource.OUChange;
                        newRequest.AccountType = nonInhouseAccount.AccountType;
                        newRequest.ApplicationType = EndPoint.GetById(nonInhouseAccount.EndpointSystemId).ApplicationType;

                        newRequest.RequesterId = null;

                        newRequest.ExecutionTime = DateTime.Now;
                        newRequest.Description = "This request is created automatically for OU Change.";
                        newRequest.Remarks = string.Empty;

                        newRequest.TransactionId = transactionId;
                        newRequest.EmployeeEventTriggerId = trigger.Id;

                        if (AccessRequest.IsSameAccessRequestExistsForUser(newRequest.RequestSource, newRequest.UserId, newRequest.EmployeeEventTriggerId.Value, newRequest.EndpointSystemId.Value, newRequest.ActivationDate, newRequest.ApplicationType, newRequest.AccountType))
                        {
                            DILog.ProvisioningLogger.WriteLog(LogLevelType.Info, $"Same OUChange AccessRequest already exists for this user:{user.UserName} application:{newRequest.ApplicationType} TriggerId:{trigger.Id}");
                            continue;
                        }
                        requests.Add(newRequest);
                    }

                    inhouseAccounts = new List<AccountReconcilation>();
                    inhouseAccounts = listAcs.Where(t => t.IsLocked == false && t.IsDeleted == false
                                        && (t.AccountType == AccountType.User || t.AccountType == AccountType.Secondary)
                                        && t.EndPoint.EndpointSystemTypeId.Value == (long)EndpointType.InhouseSystems)
                                        .ToList();

                    foreach (var inhouseAccount in inhouseAccounts)
                    {
                        var endpoint = EndPoint.GetById(inhouseAccount.EndpointSystemId);
                        newRequest = new AccessRequest();

                        newRequest.UserId = user.Id;
                        newRequest.EndpointSystemId = inhouseAccount.EndpointSystemId;

                        newRequest.RequestStatus = AccessRequestStatus.Granted;
                        newRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;

                        newRequest.RequestSource = AccessRequestSource.OUChange;
                        newRequest.AccountType = inhouseAccount.AccountType;
                        newRequest.ApplicationType = endpoint.ApplicationType;

                        newRequest.RequesterId = null;

                        newRequest.ExecutionTime = DateTime.Now;
                        newRequest.Description = "This request is created automatically for OU Change.";
                        newRequest.Remarks = string.Empty;

                        newRequest.TransactionId = transactionId;
                        newRequest.EmployeeEventTriggerId = trigger.Id;
                        IEntitlementDetail adhocEntitlmentDetails = new AdHocINHRequest() { ApplicationCode = endpoint.ApplicationCode, EndpointUserId = inhouseAccount.EndpointUserId, DisplayAccountName = inhouseAccount.AccountName, EntitlementType = EntitlementType.Permission };
                        var adhocReq = new AdHocAccessRequest() { ApplicationType = ApplicationType.InhouseSystems };
                        newRequest.AdditionalInfo = adhocReq.GetSerializedString(adhocEntitlmentDetails);

                        if (CheckSameAccessRequest(inhouseAccount, newRequest))
                        {
                            DILog.ProvisioningLogger.WriteLog(LogLevelType.Info, $"Same OUChange AccessRequest already exists for this user:{user.UserName} application:{newRequest.ApplicationType} TriggerId:{trigger.Id}");
                            continue;
                        }
                        requests.Add(newRequest);
                    }
                }

                AccessRequest.AddAccessRequest(requests);
                return requests.Count;
            }
            catch
            {
                throw;
            }
            finally
            {
                nonInhouseAccounts?.Clear();
                nonInhouseAccounts = null;

                inhouseAccounts?.Clear();
                inhouseAccounts = null;

                listAcs?.Clear();
                listAcs = null;
            }
        }

        static int PopulateReOrgAccessRequests(User user, EmployeeEventTrigger trigger)
        {
            List<AccessRequest> requests = new List<AccessRequest>();
            AccessRequest newRequest = new AccessRequest();
            Guid transactionId = Guid.NewGuid();
            IList<AccountReconcilation> listAcs = null;
            try
            {
                listAcs = AccountReconcilation.GetAllByUser(user.Id, true);

                if (listAcs != null && listAcs.Count > 0)
                {
                    listAcs = listAcs.Where(t => t.IsLocked == false && t.IsDeleted == false &&
                            (t.AccountType == AccountType.User || t.AccountType == AccountType.Secondary)).ToList();
                    listAcs = listAcs.OrderByDescending(a => a.EndPoint.EndpointSystemTypeId.Value).ToList();
                    foreach (var listAc in listAcs)
                    {
                        newRequest = new AccessRequest();

                        newRequest.UserId = user.Id;
                        newRequest.EndpointSystemId = listAc.EndpointSystemId;

                        newRequest.RequestStatus = AccessRequestStatus.Granted;
                        newRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                        newRequest.RequestSource = AccessRequestSource.ReOrganisation;
                        newRequest.AccountType = listAc.AccountType;
                        newRequest.ApplicationType = EndPoint.GetById(listAc.EndpointSystemId).ApplicationType;

                        newRequest.RequesterId = null;

                        newRequest.ExecutionTime = DateTime.Now;
                        if (trigger.CreatedBy == ProvisioningConstants.ReorgCreatedBy)
                        {
                            newRequest.Description = "This request is created automatically for Re Org Request(Retain Access for Transfer).";
                        }
                        else
                        {
                            newRequest.Description = "This request is created automatically for Re Org Request.";
                        }
                        newRequest.Remarks = string.Empty;
                        newRequest.TransactionId = transactionId;
                        newRequest.EmployeeEventTriggerId = trigger.Id;
                        requests.Add(newRequest);
                    }
                }
                //requests.ForEach(l => l.Save(serviceUser));
                AccessRequest.AddAccessRequest(requests);

                return requests.Count;
            }
            catch
            {
                throw;
            }
            finally
            {
                listAcs?.Clear();
                listAcs = null;
            }
        }

        static int PopulatePropagationAccessRequests(long userId, Guid transactionId, EmployeeEventTrigger trigger, User usr)
        {
            List<AccessRequest> requests = new List<AccessRequest>();
            AccessRequest newRequest = new AccessRequest();
            IList<AccountReconcilation> listAcs = null;

            try
            {
                listAcs = AccountReconcilation.GetAllByUser(userId, true);

                if (listAcs != null && listAcs.Count > 0)
                {
                    listAcs = listAcs.Where(t => t.IsLocked == false && t.IsDeleted == false && (t.AccountType == AccountType.User)).ToList();
                    if (listAcs.Count > 0)
                    {
                        listAcs = listAcs.OrderByDescending(a => a.EndPoint.EndpointSystemTypeId).ToList();
                    }

                    foreach (var listAc in listAcs)
                    {
                        newRequest = new AccessRequest();
                        newRequest.UserId = userId;
                        newRequest.AccountType = listAc.AccountType;
                        newRequest.ApplicationType = EndPoint.GetById(listAc.EndpointSystemId).ApplicationType;
                        newRequest.EndpointSystemId = listAc.EndpointSystemId;
                        newRequest.RequestStatus = AccessRequestStatus.Granted;
                        newRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                        newRequest.RequestSource = AccessRequestSource.Propagation;
                        newRequest.RequesterId = null;
                        //SETTING 1 MIN delay because user details must be updated before request execution
                        newRequest.ExecutionTime = DateTime.Now.AddMinutes(1);
                        newRequest.Description = "This request is created automatically for Propagation";
                        newRequest.Remarks = string.Empty;
                        newRequest.TransactionId = transactionId;
                        newRequest.EmployeeEventTriggerId = trigger.Id;

                        requests.Add(newRequest);
                    }
                }

                //  requests.ForEach(l => l.Save(serviceUser));
                AccessRequest.AddAccessRequest(requests);
                if (usr != null)
                {
                    updateUser(usr, trigger.EventType);
                    if(usr.FullTimeEmployee)
                        UpdateReferenceUser(usr.Id);
                }
                return requests.Count;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while creating Populating PropagationTask.", ex);
                throw;
            }
            finally
            {
                listAcs?.Clear();
                listAcs = null;
            }
        }

        public static int PopulateEmployeeExtensionAccessRequests(long userId, EmployeeEventTrigger trigger)
        {
            List<AccessRequest> requests = new List<AccessRequest>();
            AccessRequest newRequest = new AccessRequest();
            Guid transactionId = Guid.NewGuid();


            var canPropagateExtensionAccessRequests = true;

            if (!string.IsNullOrWhiteSpace(trigger.UserDetails))
            {
                var user = UtilityMethods.Deserialize<User>(trigger.UserDetails);
                if (user != null && user.TerminationDate.HasValue && user.TerminationDate.Value < DateTime.Now.AddDays(1))
                {
                    canPropagateExtensionAccessRequests = false;
                }
            }


            IList<AccountReconcilation> listAcs = null;
            List<AccountReconcilation> nonInhouseAccounts = null;
            List<AccountReconcilation> inhouseAccounts = null;
            try
            {
                listAcs = AccountReconcilation.GetAllByUser(userId, true);

                if (listAcs != null && listAcs.Count > 0)
                {
                    nonInhouseAccounts = new List<AccountReconcilation>();
                    nonInhouseAccounts = listAcs.Where(t => t.IsLocked == false && t.IsDeleted == false &&
                                        (t.AccountType == AccountType.User || t.AccountType == AccountType.Secondary) &&
                                        t.EndPoint.EndpointSystemTypeId.Value != (long)EndpointType.InhouseSystems)
                                        .OrderByDescending(a => a.EndPoint.EndpointSystemTypeId.Value).ToList();

                    if (!canPropagateExtensionAccessRequests)
                    {
                        nonInhouseAccounts = nonInhouseAccounts.Where(a => a.EndPoint != null && a.EndPoint.ApplicationType == ApplicationType.IDMS).Select(a => a).ToList();
                    }

                    foreach (var nonInhouseAccount in nonInhouseAccounts)
                    {
                        newRequest = new AccessRequest();

                        newRequest.UserId = userId;
                        newRequest.AccountType = nonInhouseAccount.AccountType;
                        newRequest.ApplicationType = EndPoint.GetById(nonInhouseAccount.EndpointSystemId).ApplicationType;
                        newRequest.EndpointSystemId = nonInhouseAccount.EndpointSystemId;
                        newRequest.EmployeeEventTriggerId = trigger.Id;

                        newRequest.RequestStatus = AccessRequestStatus.Granted;
                        newRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                        newRequest.RequestSource = AccessRequestSource.NonEmployeeExtension;

                        newRequest.RequesterId = null;

                        newRequest.ExecutionTime = DateTime.Now;
                        newRequest.Description = "This request is created automatically for employee extension/shortening.";
                        newRequest.Remarks = string.Empty;
                        newRequest.CreatedBy = "System";
                        newRequest.CreatedDate = DateTime.Now;

                        newRequest.TransactionId = transactionId;
                        requests.Add(newRequest);
                    }

                    if (canPropagateExtensionAccessRequests)
                    {

                        inhouseAccounts = new List<AccountReconcilation>();
                        inhouseAccounts = listAcs.Where(t => t.IsLocked == false && t.IsDeleted == false &&
                                            (t.AccountType == AccountType.User || t.AccountType == AccountType.Secondary)
                                            && t.EndPoint.EndpointSystemTypeId.Value == (long)EndpointType.InhouseSystems)
                                            .GroupBy(x => new { x.EndpointSystemId, x.AccountType })
                                            .Select(x => x.FirstOrDefault()).ToList();

                        foreach (var inhouseAccount in inhouseAccounts)
                        {
                            newRequest = new AccessRequest();

                            newRequest.UserId = userId;
                            newRequest.AccountType = inhouseAccount.AccountType;
                            newRequest.ApplicationType = EndPoint.GetById(inhouseAccount.EndpointSystemId).ApplicationType;
                            newRequest.EndpointSystemId = inhouseAccount.EndpointSystemId;
                            newRequest.EmployeeEventTriggerId = trigger.Id;

                            newRequest.RequestStatus = AccessRequestStatus.Granted;
                            newRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                            newRequest.RequestSource = AccessRequestSource.NonEmployeeExtension;

                            newRequest.RequesterId = null;

                            newRequest.ExecutionTime = DateTime.Now;
                            newRequest.Description = "This request is created automatically for employee extension/shortening.";
                            newRequest.Remarks = string.Empty;
                            newRequest.CreatedBy = "System";
                            newRequest.CreatedDate = DateTime.Now;

                            newRequest.TransactionId = transactionId;
                            requests.Add(newRequest);
                        }

                    }
                }
                AccessRequest.AddAccessRequest(requests);

                return requests.Count;
            }
            catch
            {
                throw;
            }
            finally
            {
                nonInhouseAccounts?.Clear();
                nonInhouseAccounts = null;

                inhouseAccounts?.Clear();
                inhouseAccounts = null;

                listAcs?.Clear();
                listAcs = null;
            }
        }

        static void CreateOUChangeTrigger(User user, User oldUser)
        {
            try
            {
                EmployeeEventTrigger newTrigger = new EmployeeEventTrigger()
                {
                    EffectiveDate = DateTime.Now,
                    EventType = EmployeeEvents.OUChange,
                    IsRequestCreated = false,
                    UserId = user.Id,
                    IsActive = true
                };

                newTrigger.OldUserDetails = UtilityMethods.Serialize<User>(oldUser);
                newTrigger.UserDetails = UtilityMethods.Serialize<User>(user);
                newTrigger.Save(serviceUser);
            }
            catch
            {
                throw;
            }
        }



        static bool IsTriggerValid(User user, EmployeeEvents employeeEvent, EmployeeEventTrigger trigger, EmployeeEventTriggerComment eventTriggerComment = null)
        {
            var comment = $"PERNR-[{user}],EmployeeEvent :[{employeeEvent.ToString()}], Trigger Efective Date:[{trigger.EffectiveDate}]";
            if (eventTriggerComment != null)
            {
                comment = eventTriggerComment.ToString();
            }
            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"IsTriggerValid check for TriggerId:[{trigger.Id}],{comment}");
            bool triggerExists = false;
            try
            {
                if (user.FullTimeEmployee && trigger != null && trigger.CreatedBy == "System")
                {
                    var endpoint = EndPoint.GetTrustedSource();

                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"Is the end point Null :{endpoint == null}");

                    if (endpoint != null && endpoint.EndpointSystemTypeId == (long)EndpointType.SAP && (employeeEvent == EmployeeEvents.Promotion || employeeEvent == EmployeeEvents.Propagation))
                    {
                        return true;
                    }

                    var connector = ConnectorFactory.GetConnector(endpoint, null);
                    try
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"The Connector type is {connector.GetType().FullName}");
                        if (connector.Connect() == false)
                        {
                            throw new Exception("Could not establish the connection to the endpoint.");
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                    if (employeeEvent == EmployeeEvents.OUChange && trigger.Comments == "A3")
                    {
                        employeeEvent = EmployeeEvents.Rehire;
                    }
                    bool isTransferWithRetain = false;
                    DateTime effectiveDate = connector.GetEffectiveDateByPernr(user.EmployeeId, employeeEvent, trigger.EffectiveDate,out isTransferWithRetain, eventTriggerComment);

                    if (employeeEvent == EmployeeEvents.NewJoinee || employeeEvent == EmployeeEvents.Rehire)
                    {
                        if (user.JoinDate.HasValue)
                        {
                            triggerExists = effectiveDate == user.JoinDate.Value;
                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"IsTriggerValid: Efective date received is {effectiveDate} and the user's joinging date is {user.JoinDate.Value}. The return value is {triggerExists} ");
                        }
                        else
                        {
                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"IsTriggerValid: Efective date received is {effectiveDate} and the user's joinging date does not has value. The return value is {triggerExists}");
                        }
                    }
                    else if (employeeEvent == EmployeeEvents.OUChange || employeeEvent == EmployeeEvents.ReOrganisation || employeeEvent == EmployeeEvents.Promotion || employeeEvent == EmployeeEvents.Propagation)
                    {
                        //if (employeeEvent == EmployeeEvents.OUChange && trigger.Comments == "A3")
                        //{                           
                        //    triggerExists = effectiveDate == trigger.EffectiveDate;
                        //}
                        //else
                        {
                            triggerExists = effectiveDate == trigger.EffectiveDate;
                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"IsTriggerValid: Effective date received is {effectiveDate} and the return value is {triggerExists} ");
                            if(employeeEvent == EmployeeEvents.OUChange && isTransferWithRetain)
                            {
                                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"This Event:{employeeEvent} Effective date received is {effectiveDate} and it is valid:{triggerExists} with RetainAccessforTransfer");
                                trigger.IsTransferWithNotes = isTransferWithRetain;
                            }
                        }
                    }
                    else if (employeeEvent == EmployeeEvents.Resignation)
                    {
                        if (endpoint.EndpointSystemTypeId == (long)EndpointType.SAP && user.TerminationDate.HasValue)
                        {
                            triggerExists = effectiveDate == user.TerminationDate.Value;
                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"IsTriggerValid: Efective date received is {effectiveDate}, the user's termination date is {user.TerminationDate.Value} and the return value is {triggerExists} ");
                        }

                        if (endpoint.EndpointSystemTypeId == (long)EndpointType.SuccessFactor)
                        {
                            triggerExists = effectiveDate == trigger.EffectiveDate;
                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"IsTriggerValid: Efective date received is {effectiveDate} and the return value is {triggerExists} ");
                        }



                    }

                    if (!triggerExists)
                    {
                        ProvisioningEmailHelper.NotifyDeletedTrigger(user, employeeEvent, trigger);
                    }
                    return triggerExists;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, string.Format("Error while checking the trigger existence for the user - {0} from HRMS system.", user.EmployeeId), ex);
                throw new Exception(string.Format("Error while checking the trigger existence for the user - {0} from HRMS system.", user?.EmployeeId), ex);
            }
        }

        static void WithDrawnAllPendingRequests(User user)
        {
            string remarks = $"The User with PERNR : {user.EmployeeId} is OffBoarded. Hence his/her pending requests are withdrawn by IDMS";
            List<AccessRequest> pendingRequestList = new List<AccessRequest>();
            List<Task> taskList = null;
            try
            {
                pendingRequestList = AccessRequest.GetByUserId(user.Id, new List<AccessRequestProgress> { AccessRequestProgress.Submitted,
                    AccessRequestProgress.ValidationFailed,
                    AccessRequestProgress.WaitingforApproval,
                    AccessRequestProgress.ReadytoExecute
                   }).Where(x => x.RequestSource == AccessRequestSource.AdhocRequest && !excludedApplicationTypes.Contains(x.ApplicationType)).ToList();

                foreach (var accessRequest in pendingRequestList)
                {
                    try
                    {
                        WithdrawActionCriteria _criteria = null;
                        accessRequest.Remarks = DateTime.Now.ToString();
                        accessRequest.Comments = remarks;
                        accessRequest.RequestStatus = AccessRequestStatus.Withdrawn;

                        taskList = Task.GekByAccessRequestId(accessRequest.Id);
                        if (taskList != null && taskList.Count > 0 && taskList.Any(t => t.IsCompleted == false && t.IsActive == true))
                        {
                            var l = taskList.FirstOrDefault();
                            if (l != null)
                            {
                                _criteria = new WithdrawActionCriteria()
                                {
                                    RequestType = l.AttestationId != null ? RequestType.Attestation : RequestType.Provisioning,
                                    WorkflowInstanceId = l.WorkflowInstanceId
                                };

                                ServiceAccessStrategy _serviceAccessStrategy = new ServiceAccessStrategy(_criteria);
                                bool status = _serviceAccessStrategy.AccessWorkflowService();

                            }
                        }

                        accessRequest.ProgressStatus = AccessRequestProgress.Withdrawn;
                        accessRequest.Save(serviceUser);
                    }
                    catch (Exception ex)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, $"Exception occur while withdrawing existing access request Id : [{accessRequest.Id}] during OffBoard of the user - {user.EmployeeId} from HRMS system.", ex);

                    }
                }
            }
            catch (Exception ee)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, string.Format("Error while withdrawing existing access requests during OffBoard of the user - {0} from HRMS system.", user.EmployeeId), ee);
                return;
            }
            finally
            {
                taskList?.Clear();
                taskList = null;

                pendingRequestList?.Clear();
                pendingRequestList = null;
            }

        }
        static List<PresentationAccessRequestItem> GetEntitlementDetailFromAttestationReview(AttestationSnapshotAction review, bool isAccountRevoked, bool isEntitlementRevoked)
        {
            List<PresentationAccessRequestItem> liItems = new List<PresentationAccessRequestItem>();

            if (isAccountRevoked)
            {
                var accountRecon = AccountReconcilation.GetById(review.AttestationSnapshot.KeyId, true);
                var ar = new PresentationAccessRequestItem()
                {
                    EndpointUserId = accountRecon.EndpointUserId,
                    EndpointSystemId = review.AttestationSnapshot.EndpointId,
                    AccessRequestItemType = AccessRequestItemType.Application,
                    Enabled = true,
                    RemoveAccess = true,
                };
                liItems.Add(ar);

                var entitlementRecon = EntitlementReconcilation.GetByAccountReconcilationId(review.AttestationSnapshot.KeyId, review.AttestationSnapshot.EndpointId);
                if (entitlementRecon != null && entitlementRecon.Count > 0)
                {
                    entitlementRecon = entitlementRecon.Where(a => !a.IsDeleted).ToList();
                    foreach (var entitlement in entitlementRecon)
                    {
                        var entitle = new PresentationAccessRequestItem()
                        {
                            EndpointUserId = entitlement.EndpointUserId,
                            EndpointAccountName = entitlement.AccountName,
                            AccessRequestItemType = AccessRequestItemType.Entitlement,
                            AccessItemType = AccessItemType.TechnicalRole,
                            KeyId = entitlement.EntitlementId,
                            EndpointSystemId = entitlement.EndpointSystemId,
                            EndpointSystemTypeId = (long)entitlement.EndpointType,
                            EndpointName = accountRecon.EndPoint.DisplayName,
                            EndpointType = entitlement.EndpointType,
                            ItemName = entitlement.Entitlement,
                            ItemDetail = entitlement.Entitlement,
                            ItemDescription = entitlement.EntitlementDescription,
                            ItemDisplayName = entitlement.Entitlement,
                            ItemDetailDescription = entitlement.EntitlementDescription,
                            EntitlementType = (EntitlementType)entitlement.EntitlementType,
                            IsApproved = true,
                            AccountType = accountRecon.AccountType,
                            ACRCategory = ACRCategory.Default,
                            RequestFor = Convert.ToInt64(entitlement.UserId),
                            Enabled = true,
                            RemoveAccess = true,
                        };
                        liItems.Add(entitle);
                    }
                }
            }
            else
            {
                var entitlement = EntitlementReconcilation.GetById(review.AttestationSnapshot.KeyId);
                var ar = new PresentationAccessRequestItem()
                {
                    EndpointUserId = entitlement.EndpointUserId,
                    EndpointAccountName = review.AttestationSnapshot.AccountName,
                    AccessRequestItemType = AccessRequestItemType.Entitlement,
                    AccessItemType = AccessItemType.TechnicalRole,
                    KeyId = entitlement != null ? entitlement.EntitlementId : review.AttestationSnapshot.KeyId,
                    EndpointSystemId = review.AttestationSnapshot.EndpointId,
                    EndpointSystemTypeId = (long)review.AttestationSnapshot.EndpointType,
                    EndpointName = review.AttestationSnapshot.EndpointName,
                    EndpointType = review.AttestationSnapshot.EndpointType,
                    ItemName = review.AttestationSnapshot.Name,
                    ItemDetail = review.AttestationSnapshot.Name,
                    ItemDescription = review.AttestationSnapshot.Description,
                    ItemDisplayName = review.AttestationSnapshot.Name,
                    ItemDetailDescription = review.AttestationSnapshot.Description,
                    EntitlementType = review.AttestationSnapshot.EntitlementType,
                    IsApproved = true,
                    AccountType = review.AttestationSnapshot.AccountType,
                    ACRCategory = ACRCategory.Default,
                    RequestFor = Convert.ToInt64(review.AttestationSnapshot.UserId),
                    Enabled = true,
                    RemoveAccess = true,
                };
                liItems.Add(ar);
            }
            return liItems;
        }

        static void UpdateReferenceUser(long UserId)
        {
            List<long> referenceUsers = null;
            User referenceUser = null;
            try
            {
                referenceUser = User.GetById(UserId);

                referenceUsers = User.GetReferenceUsersIds(referenceUser.Id);
                var propagationFields = MasterConfigHelper.PropagationFields;
                bool canRunPropagationTrigger = MasterConfigHelper.CanRunPropagationTrigger;

                if (referenceUsers.Count > 0)
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"UPDATENONEMPLOYEE_CREATEPROPAGATION_REQUEST for Non-Employee - Referenced Employee:{referenceUser.EmployeeId} And Reference User count:{referenceUsers.Count} propagationFields:[{string.Join(",", propagationFields)}] - canRunPropagationTrigger:{canRunPropagationTrigger}");
                    foreach (int userId in referenceUsers)
                    {
                        var usr = User.GetById(userId);

                        usr.ReferenceUserId = referenceUser.Id;
                        usr.ROEmployeeId = referenceUser.ROEmployeeId;
                        usr.COEmployeeId = referenceUser.COEmployeeId;

                        usr.CompanyCode = referenceUser.CompanyCode;
                        usr.CompanyId = referenceUser.CompanyId;
                        usr.CompanyName = referenceUser.CompanyName;
                        usr.OrgCompany = referenceUser.OrgCompany;

                        usr.DepartmentCode = referenceUser.DepartmentCode;
                        usr.DepartmentId = referenceUser.DepartmentId;
                        usr.DepartmentName = referenceUser.DepartmentName;
                        usr.OrgDepartment = referenceUser.OrgDepartment;

                        usr.DivisionCode = referenceUser.DivisionCode;
                        usr.DivisionId = referenceUser.DivisionId;
                        usr.DivisionName = referenceUser.DivisionName;
                        usr.OrgDivision = referenceUser.OrgDivision;

                        usr.BranchCode = referenceUser.BranchCode;
                        usr.OrgBranch = referenceUser.OrgBranch;


                        usr.CostCenter = referenceUser.CostCenter;
                        usr.CostCenterText = referenceUser.CostCenterText;

                        usr.NOrganisationId = referenceUser.NOrganisationId;
                        usr.SectionCode = referenceUser.SectionCode;
                        usr.SectionName = referenceUser.SectionName;
                        usr.OrgSection = referenceUser.OrgSection;
                        usr.OrgUnit = referenceUser.OrgUnit;
                        usr.UnitCode = referenceUser.UnitCode;

                        usr.OrganisationCode = referenceUser.OrganisationCode;

                        usr.OrgBranch = referenceUser.OrgBranch;
                        usr.Supervisor = referenceUser.Supervisor;

                        User.UpdateUserFromDiscovery(usr, propagationFields, canRunPropagationTrigger, serviceUser);
                        User.DeriveCOEmployeeId(usr.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, $"UpdateReferenceUser { referenceUser?.EmployeeId} failed {ex.ToString()}", ex);
            }
            finally
            {
                referenceUsers?.Clear();
                referenceUsers = null;
            }
        }
    }
}
