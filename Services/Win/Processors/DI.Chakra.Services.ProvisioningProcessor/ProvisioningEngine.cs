using DI.Chakra.Connector;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.ProvisioningUtility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Services.ProvisioningProcessor
{
    public class ProvisioningEngine
    {
        private string _serviceUser = string.Empty;
        private string serviceUser
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_serviceUser))
                {
                    _serviceUser = MasterConfigHelper.ServiceUser;
                }
                return _serviceUser;
            }
        }
        public int StartProvisioning()
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Ssl3 | System.Net.SecurityProtocolType.Tls;
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object obj, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) { return true; };

            int TotalRequest = 0;
            List<AccessRequest> currentRequests = new List<AccessRequest>();
            List<AccessRequest> accessRequestList = null;
            List<long> referenceUsers = null;
            List<string> referenceUsersForEmail = null;
            List<AccountReconcilation> adAccounts = null;
            List<Group> approverGroups = null;
            IList<string> approverConfiguration = null;
            IList<Group> userGroups = null;
            RequestExecutor requestExecutor = null;
            //DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"ServiceUser: {serviceUser}");
            try
            {
                if (MasterConfigHelper.IsDeploymentInProgress == false)
                {
                    
                    requestExecutor = new RequestExecutor();

                    accessRequestList = AccessRequest.GetApprovedAccessRequestBatch();
                    if (accessRequestList != null && accessRequestList.Count > 0)
                    {

                        var requestSource = accessRequestList.FirstOrDefault().RequestSource;

                        if (requestSource != AccessRequestSource.UndoEvent && accessRequestList.FirstOrDefault().AdHocAccessRequestId.HasValue)
                        {
                            AddRequestForDependentSystems(accessRequestList);

                            // EP & CE accounts would be deleted on FLINS deletion in FLINSBase.cs as dependentsystem deltion. So no request is required to be generated.

                            //var autoCreatedSystemRequests = AddRequestForAutoCreatedSystems(accessRequestList);
                            //accessRequestList.AddRange(dependentRequests);
                            // accessRequestList.AddRange(autoCreatedSystemRequests);
                        }

                        if (requestSource == AccessRequestSource.AdhocRequest)
                        {
                            currentRequests = AlignAccessRequests(accessRequestList);
                        }
                        else if (requestSource == AccessRequestSource.UndoEvent)
                        {
                            currentRequests = AlignUndoRequests(accessRequestList);
                        }
                        else if (requestSource == AccessRequestSource.OnBoarding)
                        {
                            currentRequests = AlignOnBoardAccessRequests(accessRequestList);
                        }
                        else
                        {
                            currentRequests = accessRequestList;
                        }

                        foreach (AccessRequest accessRequest in currentRequests)
                        {
                            try
                            {
                                accessRequest.ProgressStatus = AccessRequestProgress.Executing;
                                accessRequest.Save(serviceUser);
                            }
                            catch (Exception ex)
                            {
                                DILog.ProvisioningLogger.WriteLog(accessRequest.EndpointSystemId, accessRequest.Id, LogLevelType.Exception, "Error in saving access request - foreach.", ex);
                            }
                        }

                        TotalRequest = currentRequests.Count;

                        requestExecutor.ExecuteAccessRequest(currentRequests);

                        var firstRequest = currentRequests.FirstOrDefault();

                        if (firstRequest.RequestSource != AccessRequestSource.AdhocRequest && firstRequest.RequestSource != AccessRequestSource.None)
                        {
                            if (firstRequest.RequestSource == AccessRequestSource.OffBoarding || firstRequest.RequestSource == AccessRequestSource.OUChange)
                            {
                                requestExecutor.SendEmailIfUserExistsAsApproverOrOwnerInAnyMatrix(firstRequest.UserId, firstRequest, firstRequest.EndpointSystemId.Value);
                            }
                            if (firstRequest.RequestSource == AccessRequestSource.ReOrganisation || firstRequest.RequestSource == AccessRequestSource.Propagation)
                            {
                                //Moved this logic to Propagation and reorg request generation
                                #region Old Code
                                //var referenceUser = User.GetById(firstRequest.UserId);

                                // referenceUsers = User.GetReferenceUsersIds(referenceUser.Id);
                                // var propagationFields = MasterConfigHelper.PropagationFields;
                                // bool canRunPropagationTrigger = MasterConfigHelper.CanRunPropagationTrigger;

                                // if (referenceUsers.Count > 0)
                                // {
                                //     DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"UPDATENONEMPLOYEE_CREATEPROPAGATION_REQUEST for Non-Employee - Referenced Employee:{referenceUser.EmployeeId} And Reference User count:{referenceUsers.Count} propagationFields:[{string.Join(",", propagationFields)}] - canRunPropagationTrigger:{canRunPropagationTrigger}");
                                //     foreach (int userId in referenceUsers)
                                //     {
                                //         var usr = User.GetById(userId);

                                //         usr.ReferenceUserId = referenceUser.Id;
                                //         usr.ROEmployeeId = referenceUser.ROEmployeeId;
                                //         usr.COEmployeeId = referenceUser.COEmployeeId;

                                //         usr.CompanyCode = referenceUser.CompanyCode;
                                //         usr.CompanyId = referenceUser.CompanyId;
                                //         usr.CompanyName = referenceUser.CompanyName;
                                //         usr.OrgCompany = referenceUser.OrgCompany;

                                //         usr.DepartmentCode = referenceUser.DepartmentCode;
                                //         usr.DepartmentId = referenceUser.DepartmentId;
                                //         usr.DepartmentName = referenceUser.DepartmentName;
                                //         usr.OrgDepartment = referenceUser.OrgDepartment;

                                //         usr.DivisionCode = referenceUser.DivisionCode;
                                //         usr.DivisionId = referenceUser.DivisionId;
                                //         usr.DivisionName = referenceUser.DivisionName;
                                //         usr.OrgDivision = referenceUser.OrgDivision;

                                //         usr.BranchCode = referenceUser.BranchCode;
                                //         usr.OrgBranch = referenceUser.OrgBranch;


                                //         usr.CostCenter = referenceUser.CostCenter;
                                //         usr.CostCenterText = referenceUser.CostCenterText;

                                //         usr.NOrganisationId = referenceUser.NOrganisationId;
                                //         usr.SectionCode = referenceUser.SectionCode;
                                //         usr.SectionName = referenceUser.SectionName;
                                //         usr.OrgSection = referenceUser.OrgSection;
                                //         usr.OrgUnit = referenceUser.OrgUnit;
                                //         usr.UnitCode = referenceUser.UnitCode;

                                //         usr.OrganisationCode = referenceUser.OrganisationCode;

                                //         usr.OrgBranch = referenceUser.OrgBranch;
                                //         usr.Supervisor = referenceUser.Supervisor;

                                //         User.UpdateUserFromDiscovery(usr, propagationFields, canRunPropagationTrigger, serviceUser);
                                //         User.DeriveCOEmployeeId(usr.Id);
                                //     }
                                // }
                                #endregion 
                            }
                            else if (firstRequest.RequestSource == AccessRequestSource.OffBoarding)
                            {
                                if (firstRequest.EmployeeEventTriggerId.HasValue)
                                {
                                    var user = User.GetById(firstRequest.UserId);
                                    DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"User is null: {user == null}, firstRequest.Id: {firstRequest?.UserId}");
                                    if (user != null)
                                    {

                                        try
                                        {
                                            //map service accounts to supervisor before off boarding.
                                            adAccounts = User.HandleServiceAccount(user, serviceUser, firstRequest.EmployeeEventTriggerId.Value);
                                            if (adAccounts.Count > 0)
                                            {
                                                var ep = EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                                                foreach (var adAccount in adAccounts)
                                                {
                                                    try
                                                    {
                                                        FormExecutors.Common.ADBase adBaseFormExecutor = new FormExecutors.Common.ADBase(ep, firstRequest, new ProvisioningLogger(null, ep.Id, ep.EndpointSystemTypeId, ep.DisplayName));
                                                        adBaseFormExecutor.UpdateAttributes(user, adAccount.EndpointUserId);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        DILog.ProvisioningLogger.WriteLog(null, ep.Id, LogLevelType.Exception, $"UpdateAttributes(Owner) failed for User : {adAccount.AccountName}", ex);
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, $"HandleServiceAccount failed for User : {user.DisplayName}", ex);
                                        }


                                        //List<string> nonConnectedSystems = EndPoint.GetNonConnectedAccount(firstRequest.UserId, LockScenario.OffBoard);
                                        //if (nonConnectedSystems.Count != 0)
                                        //{
                                        //    ProvisioningEmailHelper.SendEmailForDeletingNonConnectedAccount(user, nonConnectedSystems, EmployeeEvents.Resignation);
                                        //}

                                        referenceUsersForEmail = User.GetReferenceUsers(user.Id);
                                        if (referenceUsers != null && referenceUsers.Count != 0)
                                        {
                                            ProvisioningEmailHelper.SendEmailToNotifyReferenceUserChange(user, referenceUsersForEmail, EmployeeEvents.Resignation);
                                        }

                                        ProvisioningEmailHelper.SendEmailAfterOffBoardingTheUser(user);

                                        try
                                        {
                                            approverConfiguration = XLData.GetAllConfigurationOfApprover(user.EmployeeId);
                                            userGroups = Group.GetByUserIncludeGroups(user.Id);

                                            approverGroups = userGroups.Where(item => item.Id != MasterConfigHelper.GroupUser).ToList();
                                            if ((approverConfiguration != null && approverConfiguration.Count > 0) || (userGroups != null && userGroups.Count > 0))
                                                ProvisioningEmailHelper.SendEmailAfterOffBoardingTheApprover(user, approverConfiguration, approverGroups);
                                        }
                                        catch (Exception ex)
                                        {
                                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, $"{ex.ToString()}",ex);
                                        }

                                        user.IsActive = false;
                                        user.Reserved4 = Constants.EmployeeStatus.OffBoarded;
                                        user.LastModifiedBy = "System";
                                        user.LastModifiedDate = DateTime.Now;

                                        if (firstRequest.EmployeeEventTriggerId.HasValue)
                                        {
                                            var trigger = EmployeeEventTrigger.GetById(firstRequest.EmployeeEventTriggerId.Value);
                                            if (trigger != null)
                                            {
                                                if (!user.TerminationDate.HasValue || user.TerminationDate.Value > trigger.EffectiveDate)
                                                    user.TerminationDate = trigger.EffectiveDate;
                                            }
                                            //2884 fixed Why Offboard executed 1 day earlier
                                            //else
                                            //  user.TerminationDate = DateTime.Now;
                                        }
                                        //2884 fixed Why Offboard executed 1 day earlier
                                        //else
                                        // user.TerminationDate = DateTime.Now;

                                        user.Save(serviceUser);
                                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, string.Format(ProvisioningConstants.MasterUser_UserDisabled_MSG, user.UserName, user.EmployeeId, user.TerminationDate.Value.ToShortDateString()), null);
                                    }
                                }
                            }
                            else if (firstRequest.RequestSource == AccessRequestSource.OUChange)
                            {
                                var user = User.GetById(firstRequest.UserId);

                                //List<string> nonConnectedSystems = EndPoint.GetNonConnectedAccount(firstRequest.UserId, LockScenario.OUChange);
                                //if (nonConnectedSystems.Count != 0)
                                //{
                                //    ProvisioningEmailHelper.SendEmailForDeletingNonConnectedAccount(user, nonConnectedSystems, EmployeeEvents.OUChange);
                                //}

                                ProvisioningEmailHelper.SendEmailAfterExecutingOUChangeRequest(user);

                                referenceUsersForEmail = User.GetReferenceUsers(user.Id);
                                if (referenceUsers != null && referenceUsers.Count != 0)
                                {
                                    ProvisioningEmailHelper.SendEmailToNotifyReferenceUserChange(user, referenceUsersForEmail, EmployeeEvents.OUChange);
                                }

                            }
                        }
                    }
                }
                else
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, "StartProvisioning - Deployment in-progress.");
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error While Executing Generic Provisioning Method.", ex);
            }
            finally
            {
                approverConfiguration?.Clear();
                userGroups?.Clear();

                approverConfiguration = null;
                userGroups = null;

                approverGroups?.Clear();
                approverGroups = null;

                referenceUsersForEmail?.Clear();
                referenceUsersForEmail = null;

                referenceUsers?.Clear();
                referenceUsers = null;

                accessRequestList?.Clear();
                accessRequestList = null;

                currentRequests?.Clear();
                currentRequests = null;
                requestExecutor = null;
            }
            return TotalRequest;
        }

        public void StartProvisioningTest(long accessRequestId)
        {
            int TotalRequest = 0;
            //new RequestExecutor().SendEmailIfUserExistsAsApproverOrOwnerInAnyMatrix(accessRequestId);
            try
            {
                List<AccessRequest> currentRequests = new List<AccessRequest>();
                RequestExecutor requestExecutor = new RequestExecutor();

                List<AccessRequest> accessRequestList = new List<AccessRequest> { AccessRequest.GetByIdIncludeAdHocAccessRequestAndUser(accessRequestId) };

                if (accessRequestList != null && accessRequestList.Count > 0)
                {
                    var requestSource = accessRequestList.FirstOrDefault().RequestSource;

                    if (requestSource != AccessRequestSource.UndoEvent && accessRequestList.FirstOrDefault().AdHocAccessRequestId.HasValue)
                    {
                        //var dependentRequests = AddRequestForDependentSystems(accessRequestList);
                        var autoCreatedSystemRequests = AddRequestForAutoCreatedSystems(accessRequestList);
                        //accessRequestList.AddRange(dependentRequests);
                        accessRequestList.AddRange(autoCreatedSystemRequests);
                    }

                    if (requestSource == AccessRequestSource.AdhocRequest)
                    {
                        currentRequests = AlignAccessRequests(accessRequestList);
                    }
                    else if (requestSource == AccessRequestSource.UndoEvent)
                    {
                        currentRequests = AlignUndoRequests(accessRequestList);
                    }
                    else if (requestSource == AccessRequestSource.OnBoarding)
                    {
                        currentRequests = AlignOnBoardAccessRequests(accessRequestList);
                    }
                    else
                    {
                        currentRequests = accessRequestList;
                    }

                    foreach (AccessRequest accessRequest in currentRequests)
                    {
                        try
                        {
                            accessRequest.ProgressStatus = AccessRequestProgress.Executing;
                            accessRequest.Save(serviceUser);
                        }
                        catch (Exception ex)
                        {
                            DILog.ProvisioningLogger.WriteLog(accessRequest.EndpointSystemId, accessRequest.Id, LogLevelType.Exception, "Error in saving access request - foreach.", ex);
                        }
                    }

                    TotalRequest = currentRequests.Count;

                    requestExecutor.ExecuteAccessRequest(currentRequests);
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error While Executing Generic Provisioning Method.", ex);
            }
        }

        public void StartTrustedSourcePropagation()
        {
            ProvisioningTaskHelper provisioningTaskHelper = null;
            RequestExecutor requestExecutor = null;
            try
            {
                if (MasterConfigHelper.IsDeploymentInProgress == false)
                {
                    //Create tasks for the submitted requests.
                    provisioningTaskHelper = new ProvisioningTaskHelper();
                    provisioningTaskHelper.CreateProvisioningTasks();
                    provisioningTaskHelper.CreateBulkProvisioningTasks();
                    provisioningTaskHelper.CreateEmployeeEventTasks();
                    new BulkRequestHelper().CreateBulkAdhoRequestTasks();

                    if (MasterConfigHelper.CanRunOffBoardTrigger)
                    {
                        //Create off boarding tasks if admin check is not required.
                        ProvisioningFlowHelper.CreateOffBoardingTasksIfAdminCheckNotRequired();
                    }

                    if (MasterConfigHelper.CanRunOnBoardTrigger)
                    {
                        //Create on boarding tasks if admin check is not required.
                        ProvisioningFlowHelper.CreateOnBoardingTasksIfAdminCheckNotRequired();
                    }

                    if (MasterConfigHelper.CanRunRehireTrigger)
                    {
                        //Create rehiring tasks if admin check is not required.
                        ProvisioningFlowHelper.CreateRehireTasksIfAdminCheckNotRequired();
                    }
                    
                    if (MasterConfigHelper.CanRunTransferTrigger)
                    {
                        //create OU change tasks if admin check is noit required
                        ProvisioningFlowHelper.CreateOUChangeTasksIfAdminCheckNotRequired();
                    }

                    if (MasterConfigHelper.CanRunReOrgTrigger)
                    {
                        //create re org tasks 
                        ProvisioningFlowHelper.CreateReOrgTasks();
                    }

                    if (MasterConfigHelper.CanRunPropagationTrigger)
                    {
                        //create propagation tasks 
                        ProvisioningFlowHelper.CreatePropagationTasks(EmployeeEvents.Propagation);
                    }
                    if (MasterConfigHelper.CanRunPromotionTrigger)
                    {
                        //create propagation tasks 
                        ProvisioningFlowHelper.CreatePropagationTasks(EmployeeEvents.Promotion);
                    }
                    if (MasterConfigHelper.RunAttestationRemediation && !MasterConfigHelper.IsDeploymentInProgress)
                    {
                        //Attestation Remediation
                        ProvisioningFlowHelper.CreateAttestationRemediationRequests();
                        //Update Attestation Remediation status
                        ProvisioningFlowHelper.UpdateAttestationRemediationStatus();
                    }


                    ProvisioningFlowHelper.CreateEmployeeExtensionTasks();

                    try
                    {
                        if (!MasterConfigHelper.IsDeploymentInProgress)
                        {
                            requestExecutor = new RequestExecutor();
                            //Assign O365 License
                            requestExecutor.AssignO365License();
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error While Executing AssignO365License Method.", ex);
                    }
                    try
                    {
                        if (!MasterConfigHelper.IsDeploymentInProgress)
                        {
                            requestExecutor = new RequestExecutor();
                            //CreateO365MailBox
                            requestExecutor.CreateO365MailBox();
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error While Executing CreateO365MailBox Method.", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error While Executing StartTrustedSourcePropagation Method.", ex);
            }
            finally
            {
                requestExecutor = null;
                provisioningTaskHelper = null;
            }
        }

        public void StartMiscProcess()
        {
            RequestExecutor requestExecutor = null;
            try
            {
                if (MasterConfigHelper.IsDeploymentInProgress == false)
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, "StartMiscProcess() Started");

                    requestExecutor = new RequestExecutor();
                    //Create home folder for AD users.
                    //No More Home Folder Creation for user as per SP suggestion
                    // requestExecutor.CreateHomeFolderForADUsers();

                    //Assign EP group for SAPEP after flins account creation.
                    try
                    {
                        requestExecutor.AssignEPGroup();
                    }
                    catch (Exception ex)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error While Executing AssignEPGroup from Misc Service.", ex);
                    }
                    try
                    {
                        //Update email id into HR system.
                        requestExecutor.AddEmailIdAndTelephoneNumberInHR();
                    }
                    catch (Exception ex)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error While Executing AddEmailIdAndTelephoneNumberInHR from Misc Service.", ex);
                    }

                    try
                    {
                        //Update flins login id into HR system.
                        requestExecutor.AddFLINSLoginIdInHR();
                    }
                    catch (Exception ex)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error While Executing AddFLINSLoginIdInHR from Misc Service.", ex);
                    }

                    try
                    {
                        //mass propagate O365 license.
                        requestExecutor.MassPropagateO365License();
                    }
                    catch (Exception ex)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error While Executing MassPropagateO365License from Misc Service.", ex);
                    }

                    try
                    {
                        //Create rejected access request about expiry by system after provisioning
                        //The below code was commented because this is no more valid for SP.
                        //requestExecutor.CreateRemoveAccessForExpiredAccessRequest();
                    }
                    catch (Exception ex)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error While Executing CreateRemoveAccessForExpiredAccessRequest from Misc Service.", ex);
                    }

                    //try
                    //{
                    //    //For SFB,We create account we just run the enable account and wait for sync, till the time we will maintain Executing status.
                    //    //Once Sync done, we execute remaining step like policy, LineUri etc.
                    //    requestExecutor.SyncSkypeForBusinessUser();
                    //}
                    //catch (Exception ex)
                    //{
                    //    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error While Executing SyncSkypeForBusinessUser from Misc Service.", ex);
                    //}

                    try
                    {
                        //  Update Extension Number for Exisitng Users
                        requestExecutor.AddmobileNumberinOneCard();
                    }
                    catch (Exception ex)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error While Executing AddmobileNumberinOneCard from Misc Service.", ex);
                    }


                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, "StartMiscProcess() Ended");
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error While Executing StartMiscProcess Method.", ex);
            }
            finally
            {
                requestExecutor = null;
            }
        }
        
        List<AccessRequest> AlignUndoRequests(List<AccessRequest> requests)
        {
            DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, "AlignUndoRequests - Started");
            List<AccessRequest> alignedRequests = new List<AccessRequest>();
            List<AccessRequest> bottomRequests = new List<AccessRequest>();
            foreach (AccessRequest request in requests)
            {
                if (request.EventUndoRequestId.HasValue)
                {
                    DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"UndoRequest is null - {request.EventUndoRequest == null}");
                    if (request.EventUndoRequest == null)
                    {
                        request.EventUndoRequest = EventUndoRequest.GetById(Convert.ToInt64(request.EventUndoRequestId));
                    }
                    DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"UndoRequest is null - {request.EventUndoRequest == null} , EndpointSystemId: {request.EventUndoRequest.EndpointSystemId}");
                    var endpoint = EndPoint.GetById(request.EventUndoRequest.EndpointSystemId);
                    if (endpoint.ApplicationType == ApplicationType.ADSingaporePowerLocal)
                    {
                        alignedRequests.Add(request);
                    }
                    else
                    {
                        bottomRequests.Add(request);
                    }
                }
                else
                {
                    bottomRequests.Add(request);
                }
            }

            alignedRequests.AddRange(bottomRequests);
            DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, "AlignUndoRequests - Ended");

            return alignedRequests;
        }

        List<AccessRequest> AlignOnBoardAccessRequests(List<AccessRequest> requests)
        {
            List<AccessRequest> alignedRequests = new List<AccessRequest>();
            List<AccessRequest> bottomRequests = new List<AccessRequest>();

            foreach (AccessRequest request in requests)
            {
                var endpoint = EndPoint.GetById(request.EndpointSystemId.Value);
                if (endpoint.ApplicationType == ApplicationType.OneCard)
                {
                    bottomRequests.Add(request);
                }
                else
                {
                    alignedRequests.Add(request);
                }
            }

            alignedRequests.AddRange(bottomRequests);

            return alignedRequests;
        }

        List<AccessRequest> AlignAccessRequests(List<AccessRequest> requests)
        {
            List<AccessRequest> alignedRequests = new List<AccessRequest>();
            List<AccessRequest> bottomRequests = new List<AccessRequest>();

            foreach (AccessRequest request in requests)
            {
                if (request.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    alignedRequests.Insert(0, request);
                }
                else
                {
                    bottomRequests.Insert(0, request);
                }
            }

            alignedRequests.AddRange(bottomRequests);

            return alignedRequests;
        }

        //EndPoint GetDependentEndpointSystem(ApplicationType application)
        //{
        //    var dependentApplication = ApplicationType.None;

        //    switch (application)
        //    {
        //        case ApplicationType.MSSL_ISU:
        //            dependentApplication = ApplicationType.MSSL_CRM;
        //            break;
        //        case ApplicationType.MSSL_CRM:
        //            dependentApplication = ApplicationType.MSSL_ISU;
        //            break;
        //        case ApplicationType.EBS_ISU:
        //            dependentApplication = ApplicationType.EBS_CRM;
        //            break;
        //        case ApplicationType.EBS_CRM:
        //            dependentApplication = ApplicationType.EBS_ISU;
        //            break;
        //        default:
        //            break;
        //    }

        //    if (dependentApplication == ApplicationType.None)
        //        return null;

        //    return EndPoint.GetByApplicationType(dependentApplication);
        //}

        List<AccessRequest> AddRequestForDependentSystems(List<AccessRequest> requests)
        {
            DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"AddRequestForDependentSystems");
            List<AccessRequest> dependentRequests = new List<AccessRequest>();
            try
            {
                foreach (AccessRequest request in requests)
                {
                    try
                    {
                        if (request.AttestationCampaignId.HasValue) continue;

                        var adhocReq = AdHocAccessRequest.GetById(request.AdHocAccessRequestId.Value, false);
                        DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"AddRequestForDependentSystems - foreach");
                        if (adhocReq.ApplicationType == ApplicationType.EBS_CRM || adhocReq.ApplicationType == ApplicationType.EBS_ISU ||
                            adhocReq.ApplicationType == ApplicationType.MSSL_CRM || adhocReq.ApplicationType == ApplicationType.MSSL_ISU)
                        {
                            if (adhocReq.AdhocActionType == AdHocActionType.NewAccount || adhocReq.AdhocActionType == AdHocActionType.DeleteAccount)
                            {
                                if (request.IsParentRequest || request.IsDependentRequest)
                                {
                                    continue;
                                }
                                AccessRequest newRequest = AccessRequest.PopulateNewFromOldAccessRequest(request);
                                newRequest.AdHocAccessRequest = AccessRequest.PopulateNewFromOldAdhocRequest(adhocReq);
                                request.IsParentRequest = true;
                                request.IsDependentRequestCreated = true;
                                newRequest.IsDependentRequest = true;
                                newRequest.ParentRequestId = request.Id;

                                request.Save(serviceUser);

                                dependentRequests.Add(newRequest);
                            }
                            else if (adhocReq.AdhocActionType == AdHocActionType.AmendmentAdd || adhocReq.AdhocActionType == AdHocActionType.AmendmentRemove)
                            {
                                DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"AddRequestForDependentSystems");
                                bool isPositionRequest = false;
                                if (adhocReq.ApplicationType == ApplicationType.EBS_CRM || adhocReq.ApplicationType == ApplicationType.EBS_ISU)
                                {
                                    var sapDetail = (AdHocEBSRequest)adhocReq.GetEntitlementDetail();
                                    DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"sapDetail: {sapDetail == null}");
                                    isPositionRequest = !string.IsNullOrWhiteSpace(sapDetail.Position);
                                }
                                if (adhocReq.ApplicationType == ApplicationType.MSSL_CRM || adhocReq.ApplicationType == ApplicationType.MSSL_ISU)
                                {
                                    var sapDetail = (AdHocMSSLRequest)adhocReq.GetEntitlementDetail();
                                    DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"sapDetail: {sapDetail == null}");
                                    isPositionRequest = !string.IsNullOrWhiteSpace(sapDetail.Position);
                                }
                                if (isPositionRequest)
                                {
                                    if (request.IsParentRequest || request.IsDependentRequest)
                                    {
                                        continue;
                                    }

                                    AccessRequest newRequest = AccessRequest.PopulateNewFromOldAccessRequest(request);
                                    DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"newRequest: {newRequest == null}");
                                    newRequest.AdHocAccessRequest = AccessRequest.PopulateNewFromOldAdhocRequest(adhocReq);
                                    DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"AccessRequest.PopulateNewFromOldAdhocRequest");
                                    request.IsParentRequest = true;
                                    request.IsDependentRequestCreated = true;
                                    newRequest.IsDependentRequest = true;
                                    newRequest.ParentRequestId = request.Id;

                                    request.Save(serviceUser);
                                    DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"request.Save");
                                    dependentRequests.Add(newRequest);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
                    }
                }

                foreach (AccessRequest request in dependentRequests)
                {
                    try
                    {
                        request.Id = 0;
                        request.AdHocAccessRequestId = 0;
                        request.AdHocAccessRequest.Id = 0;

                        if (request.AdHocAccessRequest.ApplicationType == ApplicationType.EBS_CRM)
                        {
                            request.AdHocAccessRequest.ApplicationType = ApplicationType.EBS_ISU;
                        }
                        else if (request.AdHocAccessRequest.ApplicationType == ApplicationType.EBS_ISU)
                        {
                            request.AdHocAccessRequest.ApplicationType = ApplicationType.EBS_CRM;
                        }
                        else if (request.AdHocAccessRequest.ApplicationType == ApplicationType.MSSL_CRM)
                        {
                            request.AdHocAccessRequest.ApplicationType = ApplicationType.MSSL_ISU;
                        }
                        else if (request.AdHocAccessRequest.ApplicationType == ApplicationType.MSSL_ISU)
                        {
                            request.AdHocAccessRequest.ApplicationType = ApplicationType.MSSL_CRM;
                        }

                        request.AdHocAccessRequest.EndpointSystemId = EndPoint.GetByApplicationType(request.AdHocAccessRequest.ApplicationType).Id;
                        request.EndpointSystemId = request.AdHocAccessRequest.EndpointSystemId;
                        request.ApplicationType = request.AdHocAccessRequest.ApplicationType;

                        if (request.AdHocAccessRequest.ApplicationType == ApplicationType.EBS_CRM || request.AdHocAccessRequest.ApplicationType == ApplicationType.EBS_ISU)
                        {
                            if (request.AdHocAccessRequest.AdhocActionType != AdHocActionType.NewAccount)
                            {
                                var sapDetail = (AdHocEBSRequest)request.AdHocAccessRequest.GetEntitlementDetail();

                                var actualSAPUser = SAPUserinfo.GetById(sapDetail.EndpointUserId);
                                var sapUser = SAPUserinfo.GetByUserLogonName(actualSAPUser.UserLogonName, request.AdHocAccessRequest.EndpointSystemId.Value);

                                sapDetail.EndpointUserId = sapUser.Id;

                                request.AdHocAccessRequest.EntitlementDetail = request.AdHocAccessRequest.GetSerializedString(sapDetail);
                            }
                        }
                        else if (request.AdHocAccessRequest.ApplicationType == ApplicationType.MSSL_CRM || request.AdHocAccessRequest.ApplicationType == ApplicationType.MSSL_ISU)
                        {
                            if (request.AdHocAccessRequest.AdhocActionType != AdHocActionType.NewAccount)
                            {
                                var sapDetail = (AdHocMSSLRequest)request.AdHocAccessRequest.GetEntitlementDetail();

                                var actualSAPUser = SAPUserinfo.GetById(sapDetail.EndpointUserId);
                                var sapUser = SAPUserinfo.GetByUserLogonName(actualSAPUser.UserLogonName, request.AdHocAccessRequest.EndpointSystemId.Value);

                                sapDetail.EndpointUserId = sapUser.Id;

                                request.AdHocAccessRequest.EntitlementDetail = request.AdHocAccessRequest.GetSerializedString(sapDetail);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
                    }
                }

                AccessRequest.SaveAccessRequestList(dependentRequests);
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
            }

            return dependentRequests;
        }

        List<AccessRequest> AddRequestForAutoCreatedSystems(List<AccessRequest> requests)
        {
            List<AccessRequest> dependentRequests = new List<AccessRequest>();

            foreach (AccessRequest request in requests)
            {
                var adhocReq = AdHocAccessRequest.GetById(request.AdHocAccessRequestId.Value, false);

                if (adhocReq.ApplicationType == ApplicationType.HRMS_FLINS && adhocReq.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    if (request.IsParentRequest || request.IsDependentRequest)
                    {
                        continue;
                    }

                    var sapDetail = (AdHocSAPHRFLINSRequest)request.AdHocAccessRequest.GetEntitlementDetail();
                    var actualSAPUser = SAPUserinfo.GetById(sapDetail.EndpointUserId);

                    var sapepEndpoint = EndPoint.GetByApplicationType(ApplicationType.SAP_EP);
                    SAPEPUserInfo sapepUser = SAPEPUserInfo.GetByUserName(actualSAPUser.UserLogonName, sapepEndpoint.Id);

                    if (sapepUser != null && sapepEndpoint.IsActive)
                    {
                        AccessRequest newRequest1 = AccessRequest.PopulateNewFromOldAccessRequest(request);
                        newRequest1.AdHocAccessRequest = AccessRequest.PopulateNewFromOldAdhocRequest(adhocReq);
                        newRequest1.IsDependentRequest = true;
                        newRequest1.ParentRequestId = request.Id;
                        newRequest1.AdHocAccessRequest.ApplicationType = ApplicationType.SAP_EP;
                        newRequest1.AdHocAccessRequest.EndpointSystemId = sapepEndpoint.Id;
                        newRequest1.EndpointSystemId = sapepEndpoint.Id;
                        newRequest1.ApplicationType = ApplicationType.SAP_EP;
                        AdhocSAPEPRequest detail = new AdhocSAPEPRequest()
                        {
                            EndpointUserId = sapepUser.Id
                        };

                        newRequest1.AdHocAccessRequest.EntitlementDetail = newRequest1.AdHocAccessRequest.GetSerializedString(detail);

                        newRequest1.Id = 0;
                        newRequest1.AdHocAccessRequestId = 0;
                        newRequest1.AdHocAccessRequest.Id = 0;

                        dependentRequests.Add(newRequest1);
                    }

                    var sapceEndpoint = EndPoint.GetByApplicationType(ApplicationType.CE);
                    SAPEPUserInfo sapceUser = SAPEPUserInfo.GetByUserName(actualSAPUser.UserLogonName, sapceEndpoint.Id);

                    if (sapceUser != null && sapceEndpoint.IsActive)
                    {
                        AccessRequest newRequest2 = AccessRequest.PopulateNewFromOldAccessRequest(request);
                        newRequest2.AdHocAccessRequest = AccessRequest.PopulateNewFromOldAdhocRequest(adhocReq);
                        newRequest2.IsDependentRequest = true;
                        newRequest2.ParentRequestId = request.Id;
                        newRequest2.AdHocAccessRequest.ApplicationType = ApplicationType.CE;
                        newRequest2.AdHocAccessRequest.EndpointSystemId = sapceEndpoint.Id;
                        newRequest2.EndpointSystemId = sapceEndpoint.Id;
                        newRequest2.ApplicationType = ApplicationType.CE;
                        AdhocSAPEPRequest detail = new AdhocSAPEPRequest()
                        {
                            EndpointUserId = sapceUser.Id
                        };

                        newRequest2.AdHocAccessRequest.EntitlementDetail = newRequest2.AdHocAccessRequest.GetSerializedString(detail);

                        newRequest2.Id = 0;
                        newRequest2.AdHocAccessRequestId = 0;
                        newRequest2.AdHocAccessRequest.Id = 0;

                        dependentRequests.Add(newRequest2);
                    }

                    request.IsParentRequest = true;
                    request.IsDependentRequestCreated = true;
                    request.Save(serviceUser);

                }
            }

            AccessRequest.SaveAccessRequestList(dependentRequests);

            return dependentRequests;
        }

        public void NotifyNonEmployeeExpiration()
        {
            RequestExecutor requestExecutor = new RequestExecutor();
            //notify non employee expiry.
            requestExecutor.NotifyNonEmployeeExpiration();
        }

        public void UpdateADManagerAttribute()
        {
            RequestExecutor requestExecutor = new RequestExecutor();
            requestExecutor.UpdateADManagerAttribute();
        }

        public void ResetO365IDMSAccountPassword()
        {
            if (MasterConfigHelper.CanRessetO365Pwd)
            {
                RequestExecutor requestExecutor = new RequestExecutor();
                requestExecutor.ResetO365IDMSAccountPassword();
            }
        }

        public void PrepareTelephoneDirectorySyncData()
        {
            RequestExecutor requestExecutor = new RequestExecutor();

            if (MasterConfigHelper.CanSyncSmartDirectory)
            {
                //prepare data for telephone directory sync.
                requestExecutor.PrepareTelephoneDirectorySyncData();
            }
        }

        void LogMessage(string message)
        {
            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, message);
        }

        public void UpdateBadgeNumberInADAttribute()
        {
            RequestExecutor requestExecutor = new RequestExecutor();
            requestExecutor.UpdateOneCardBadgeNumberInADAttribute();
        }
    }
}
