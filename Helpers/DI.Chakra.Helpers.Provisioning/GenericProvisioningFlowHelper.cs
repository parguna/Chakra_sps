using DI.Chakra.DBConfigHelper;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Services.EmailingProcessor;
using DI.Chakra.DomainModel;
using DI.Chakra.DomainModel.Presentation;
using System.Dynamic;
using Newtonsoft.Json;

namespace DI.Chakra.Helpers.Provisioning
{
    public class GenericProvisioningFlowHelper
    {
        public static void CreateOnBoardingTasksIfAdminCheckNotRequired()
        {
            try
            {
                var users = User.GetOnBoardingUsers(false);
                foreach (User user in users)
                {
                    User currentUser = user;
                    var trigger = EmployeeEventTrigger.GetByUserAndEvent(user.Id, EmployeeEvents.NewJoinee, null);


                    PopulateOnBoardAccessRequests(currentUser, trigger);
                    trigger.IsRequestCreated = true;
                    trigger.Save(MasterConfigHelper.ServiceUser);

                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, "Error while creating on-board tasks.", ex);
            }
        }

        public static void CreatePromotionTasksIfAdminCheckNotRequired()
        {
            try
            {
                var users = User.GetPromotionUsers(false);
                foreach (User user in users)
                {
                    User currentUser = user;
                    var trigger = EmployeeEventTrigger.GetByUserAndEvent(user.Id, EmployeeEvents.Promotion, null);


                    PopulatePromotionAccessRequests(currentUser, trigger);
                    trigger.IsRequestCreated = true;
                    trigger.Save(MasterConfigHelper.ServiceUser);

                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, "Error while creating on-board tasks.", ex);
            }
        }

        public static void CreateRehireTasksIfAdminCheckNotRequired()
        {
            try
            {
                var users = User.GetRehireUsers(false);
                foreach (User user in users)
                {
                    User currentUser = user;
                    var trigger = EmployeeEventTrigger.GetByUserAndEvent(user.Id, EmployeeEvents.Rehire, null);


                    currentUser = UtilityMethods.Deserialize<User>(trigger.UserDetails);

                    PopulateRehireAccessRequests(currentUser, trigger);
                    trigger.IsRequestCreated = true;
                    trigger.Save(MasterConfigHelper.ServiceUser);


                    currentUser.IsActive = true;
                    //currentUser.LastModifiedBy = "System";
                    //currentUser.LastModifiedDate = DateTime.Now;
                    currentUser.Save(MasterConfigHelper.ServiceUser);
                }


            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, "Error while creating on-board tasks.", ex);
            }
        }
        public static void CreateOUChangeTasksIfAdminCheckNotRequired()
        {
            try
            {
                var triggerUserDetailsList = User.GetOUChangeUsers();

                foreach (User triggerUserDetail in triggerUserDetailsList)
                {
                    var trigger = EmployeeEventTrigger.GetByUserAndEvent(triggerUserDetail.Id, EmployeeEvents.OUChange, null);

                    PopulateOUChangeAccessRequests(triggerUserDetail, trigger);

                    var currentUserObject = User.GetById(triggerUserDetail.Id);
                    
                    currentUserObject.NOrganisationId = triggerUserDetail.NOrganisationId;
                    currentUserObject.NBusinessRoleId = triggerUserDetail.NBusinessRoleId;
                    currentUserObject.Save(MasterConfigHelper.ServiceUser);

                    trigger.IsRequestCreated = true;
                    //trigger.LastModifiedBy = "System";
                    //trigger.LastModifiedDate = DateTime.Now;
                    trigger.Save(MasterConfigHelper.ServiceUser);
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, "Error while creating org-transfer tasks.", ex);
            }
        }

        public static void CreateReOrgTasks()
        {
            try
            {
                var triggerUserDetailsList = User.GetReOrgUsers();

                foreach (User triggerUserDetail in triggerUserDetailsList)
                {
                    var trigger = EmployeeEventTrigger.GetByUserAndEvent(triggerUserDetail.Id, EmployeeEvents.ReOrganisation, null);
                    PopulateReOrgAccessRequests(triggerUserDetail, trigger);

                    var currentUserObject = User.GetById(triggerUserDetail.Id);

                    currentUserObject.NOrganisationId = triggerUserDetail.NOrganisationId;
                    currentUserObject.NBusinessRoleId = triggerUserDetail.NBusinessRoleId;
                    currentUserObject.Save(MasterConfigHelper.ServiceUser);

                    trigger.IsRequestCreated = true;
                    //trigger.LastModifiedBy = "System";
                    //trigger.LastModifiedDate = DateTime.Now;
                    trigger.Save(MasterConfigHelper.ServiceUser);
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, "Error while creating re-org tasks.", ex);
            }
        }

        public static void CreatePropagationTasks()
        {
            try
            {
                var triggers = EmployeeEventTrigger.GetUsersForPropagation();
                Guid transactionId = Guid.NewGuid();

                foreach (EmployeeEventTrigger trigger in triggers)
                {
                    PopulatePropagationAccessRequests(trigger.UserId, transactionId, trigger);

                    trigger.IsRequestCreated = true;
                    //trigger.LastModifiedBy = "System";
                    //trigger.LastModifiedDate = DateTime.Now;
                    trigger.Save(MasterConfigHelper.ServiceUser);
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, "Error while creating propagation tasks.", ex);
            }
        }

        public static void CreateOffBoardingTasksIfAdminCheckNotRequired()
        {
            try
            {
                var users = User.GetOffBoardingUsers();

                foreach (User user in users)
                {
                    var trigger = EmployeeEventTrigger.GetByUserAndEvent(user.Id, EmployeeEvents.Resignation, null);
                    PopulateOffBoardAccessRequests(user, trigger);
                    trigger.IsRequestCreated = true;
                    //trigger.LastModifiedBy = "System";
                    //trigger.LastModifiedDate = DateTime.Now;
                    trigger.Save(MasterConfigHelper.ServiceUser);
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, "Error while creating off-board tasks.", ex);
            }
        }

        public static void CreateAttestationRemediationRequests()
        {
            try
            {
                var reviews = AttestationSnapshotAction.GetPendingRemediations();

                var actionOnAccounts = reviews.Where(a => a.AttestationSnapshot.EntitlementType == EntitlementType.None).ToList();
                var entity = actionOnAccounts.Select(a => a.AttestationSnapshot.Entity).Distinct().ToList();
                var actionOnEntitlements = reviews.Where(a => a.AttestationSnapshot.EntitlementType != EntitlementType.None && !entity.Contains(a.AttestationSnapshot.ParentEntity)).ToList();

                foreach (var actionOnAccount in actionOnAccounts)
                {
                    PopulateRemediationAccessRequests(actionOnAccount);
                    actionOnAccount.RemediationStatus = AttestationRemediationStatus.Completed;
                    actionOnAccount.LastModifiedBy = "System";
                    actionOnAccount.LastModifiedDate = DateTime.Now;
                    actionOnAccount.Save(MasterConfigHelper.ServiceUser);
                }

                foreach (var actionOnEntitlement in actionOnEntitlements)
                {
                    PopulateRemediationAccessRequests(actionOnEntitlement);
                    actionOnEntitlement.RemediationStatus = AttestationRemediationStatus.Completed;
                    actionOnEntitlement.LastModifiedBy = "System";
                    actionOnEntitlement.LastModifiedDate = DateTime.Now;
                    actionOnEntitlement.Save(MasterConfigHelper.ServiceUser);
            }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, "Error while creating attestation remediation tasks.", ex);
            }
        }

        public static int PopulateOnBoardAccessRequests(User user, EmployeeEventTrigger trigger)
        {
            List<AccessRequest> requests = new List<AccessRequest>();
            AccessRequest newRequest = new AccessRequest();
            AdHocAccessRequest adhocrequst = new AdHocAccessRequest();
            Guid transactionId = Guid.NewGuid();
            try
            {
                var endpointSystems = NRulesetForApplication.GetRulesetForApplication(user.Id, trigger).Distinct();

                foreach (var endpointSystem in endpointSystems)
                {
                    newRequest = new AccessRequest();

                    newRequest.UserId = user.Id;
                    newRequest.ProvisioningFormId = null;
                    newRequest.EndpointSystemId = endpointSystem.EndpointSystemId;
                    newRequest.RequestStatus = AccessRequestStatus.Granted;
                    newRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                    newRequest.RequestSource = AccessRequestSource.OnBoarding;
                    newRequest.RequesterId = null;
                    newRequest.ActivationDate = user.JoinDate.HasValue ? user.JoinDate.Value : DateTime.Now;
                    newRequest.DeactivationDate = user.TerminationDate;
                    newRequest.ExecutionTime = DateTime.Now;
                    newRequest.Description = "This request is created automatically for On-Boarding.";
                    newRequest.Remarks = string.Empty;
                    newRequest.TransactionId = transactionId;
                    newRequest.EmployeeEventTriggerId = trigger.Id;
                    newRequest.AccountType = AccountType.User;
                    
                    adhocrequst = new AdHocAccessRequest();
                    adhocrequst.RequesteeUserId = user.Id;

                    EndPoint ep = EndPoint.GetByIdIncludingEndPointSystemType(endpointSystem.EndpointSystemId);

                    adhocrequst.EndpointSystemType = (DI.Chakra.Infrastructure.Enums.EndpointType)ep.EndPointSystemType.EndpointSystemTypeName;
                    adhocrequst.EndpointSystemId = endpointSystem.EndpointSystemId;
                    adhocrequst.AdhocActionType = AdHocActionType.NewAccount;
                    adhocrequst.AccountType = AccountType.User;
                    adhocrequst.ValidFrom = user.JoinDate.HasValue ? user.JoinDate.Value : DateTime.Now;
                    adhocrequst.ValidTo = user.TerminationDate;

                    setRequestItemDetail(adhocrequst, null);

                    adhocrequst.Comments = "AdhocAccessRequest created automatically for onboarding";
                    adhocrequst.Save(MasterConfigHelper.ServiceUser);
                    newRequest.AdHocAccessRequestId = adhocrequst.Id;
                    requests.Add(newRequest);
                }
                requests.ForEach(lq => lq.Save(MasterConfigHelper.ServiceUser));
                return requests.Count;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static int PopulateRehireAccessRequests(User user, EmployeeEventTrigger trigger)
        {
            List<AccessRequest> requests = new List<AccessRequest>();
            AccessRequest newRequest = new AccessRequest();
            AdHocAccessRequest adhocrequst = new AdHocAccessRequest();
            Guid transactionId = Guid.NewGuid();
            try
            {
                var endpointSystems = NRulesetForApplication.GetRulesetForApplication(UtilityMethods.Deserialize<User>(trigger.UserDetails));

                foreach (var endpointSystem in endpointSystems)
                {
                    newRequest = new AccessRequest();

                    newRequest.UserId = user.Id;
                    newRequest.ProvisioningFormId = null;
                    newRequest.EndpointSystemId = endpointSystem.EndpointSystemId;
                    newRequest.RequestStatus = AccessRequestStatus.Granted;
                    newRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                    newRequest.RequestSource = AccessRequestSource.Rehire;
                    newRequest.RequesterId = null;
                    newRequest.ActivationDate = user.JoinDate.HasValue ? user.JoinDate.Value : DateTime.Now;
                    newRequest.DeactivationDate = user.TerminationDate;
                    newRequest.ExecutionTime = DateTime.Now;
                    newRequest.Description = "This request is created automatically for Rehire.";
                    newRequest.Remarks = string.Empty;
                    newRequest.TransactionId = transactionId;
                    newRequest.EmployeeEventTriggerId = trigger.Id;
                    newRequest.AccountType = AccountType.User;


                    adhocrequst = new AdHocAccessRequest();
                    adhocrequst.RequesteeUserId = user.Id;

                    EndPoint ep = EndPoint.GetByIdIncludingEndPointSystemType(endpointSystem.EndpointSystemId);

                    adhocrequst.EndpointSystemType = (DI.Chakra.Infrastructure.Enums.EndpointType)ep.EndPointSystemType.EndpointSystemTypeName;
                    adhocrequst.EndpointSystemId = endpointSystem.EndpointSystemId;
                    adhocrequst.AccountType = AccountType.User;
                    adhocrequst.ValidFrom = user.JoinDate.HasValue ? user.JoinDate.Value : DateTime.Now;
                    adhocrequst.ValidTo = user.TerminationDate;

                    setRequestItemDetail(adhocrequst, null);

                    adhocrequst.Comments = "AdhocAccessRequest created automatically for Rehire";
                    adhocrequst.Save(MasterConfigHelper.ServiceUser);
                    newRequest.AdHocAccessRequestId = adhocrequst.Id;
                    requests.Add(newRequest);
                }
                requests.ForEach(lq => lq.Save(MasterConfigHelper.ServiceUser));
                return requests.Count;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static void setRequestItemDetail(AdHocAccessRequest adhocRequest, AccountReconcilation accountReconcilation)
        {
            dynamic detail = new ExpandoObject();

            detail = ExpandoHelper.AddProperty(detail, "AccountName", accountReconcilation == null ? "" : accountReconcilation.AccountName);
            string detailString = JsonConvert.SerializeObject(detail);

            PresentationAccessRequestItem arItem = new PresentationAccessRequestItem
            {
                EndpointSystemId = adhocRequest.EndpointSystemId.Value,
                AccessRequestItemType = AccessRequestItemType.Application,
                AdditionalInfo = detailString,
            };
            adhocRequest.SetRequestedItems = new List<PresentationAccessRequestItem>() { arItem };
        }
        public static int PopulateOffBoardAccessRequests(User user, EmployeeEventTrigger trigger)
        {
            List<AccessRequest> requests = new List<AccessRequest>();
            AccessRequest newRequest = new AccessRequest();
            AdHocAccessRequest adhocrequst = new AdHocAccessRequest();
            Guid transactionId = Guid.NewGuid();
            try
            {
                var listAcs = AccountReconcilation.GetAllByUser(user.Id, false);

                if (listAcs != null && listAcs.Count > 0)
                {
                    listAcs = listAcs.Where(t => t.IsLocked == false && t.IsDeleted == false).ToList();
                    
                    foreach (var listAc in listAcs)
                    {
                        newRequest = new AccessRequest();

                        newRequest.UserId = user.Id;
                        newRequest.EndpointSystemId = listAc.EndpointSystemId;

                        newRequest.RequestStatus = AccessRequestStatus.Granted;
                        newRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                        newRequest.RequestSource = AccessRequestSource.OffBoarding;
                        newRequest.AccountType = listAc.AccountType;

                        newRequest.RequesterId = null;

                        newRequest.ExecutionTime = DateTime.Now;
                        newRequest.Description = "This request is created automatically for Off-Boarding.";
                        newRequest.Remarks = string.Empty;
                        newRequest.TransactionId = transactionId;
                        newRequest.EmployeeEventTriggerId = trigger.Id;



                        adhocrequst = new AdHocAccessRequest();
                        adhocrequst.RequesteeUserId = user.Id;

                        EndPoint ep = EndPoint.GetByIdIncludingEndPointSystemType(listAc.EndpointSystemId);

                        adhocrequst.EndpointSystemType = (DI.Chakra.Infrastructure.Enums.EndpointType)ep.EndPointSystemType.EndpointSystemTypeName;
                        adhocrequst.EndpointSystemId = listAc.EndpointSystemId;
                        adhocrequst.AccountType = listAc.AccountType;
                        adhocrequst.ValidFrom = user.JoinDate;
                        adhocrequst.ValidTo = DateTime.Now;                        

                        setRequestItemDetail(adhocrequst, listAc);

                        adhocrequst.Comments = "AdhocAccessRequest created automatically for offboarding";
                        adhocrequst.Save(MasterConfigHelper.ServiceUser);
                        newRequest.AdHocAccessRequestId = adhocrequst.Id;
                        requests.Add(newRequest);
                    }
                }

                requests.ForEach(lq => lq.Save(MasterConfigHelper.ServiceUser));
                return requests.Count;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static void PopulateRemediationAccessRequests(AttestationSnapshotAction review)
        {
            AccessRequest newRequest = new AccessRequest();
            AdHocAccessRequest newAdhocRequest = new AdHocAccessRequest();
            Guid transactionId = Guid.NewGuid();

            try
            {
                        newRequest = new AccessRequest();
                        newAdhocRequest = new AdHocAccessRequest();

                newRequest.UserId = review.AttestationSnapshot.UserId;
                newRequest.EndpointSystemId = review.AttestationSnapshot.EndpointId;

                        newRequest.RequestStatus = AccessRequestStatus.Granted;
                        newRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                        newRequest.RequestSource = AccessRequestSource.AttestationRemediation;
                newRequest.AccountType = review.AttestationSnapshot.AccountType;

                        newRequest.RequesterId = null;

                if (review.IsActionExtendable && review.EffectiveDate.HasValue)
                {
                    newRequest.DeactivationDate = review.EffectiveDate;
                    newRequest.ExecutionTime = review.EffectiveDate;
                }
                else
                {
                    newRequest.DeactivationDate = DateTime.Now;
                    newRequest.ExecutionTime = DateTime.Now;

                        newRequest.Description = "This request is created automatically for Attestation Remediation.";

                        newRequest.TransactionId = transactionId;
                newRequest.AttestationReviewId = review.AttestationSnapshotId;

                        //adhoc request
                newAdhocRequest.AccountType = newRequest.AccountType;
                newAdhocRequest.AdhocActionType = review.AttestationSnapshot.EntitlementType == EntitlementType.None ? AdHocActionType.DeleteAccount : AdHocActionType.AmendmentRemove;
                        newAdhocRequest.Comments = newRequest.Description;
                        newAdhocRequest.EndpointSystemId = newRequest.EndpointSystemId;
                newAdhocRequest.EndpointSystemType = review.AttestationSnapshot.EndpointType;

                        newAdhocRequest.IsSelfRequest = false;
                newAdhocRequest.RequesteeUserId = review.AttestationSnapshot.UserId;
                        newAdhocRequest.RequesterUserId = -1;
                        newAdhocRequest.ValidTo = DateTime.Now;

                var detail = GetEntitlementDetailFromAttestationReview(review, review.AttestationSnapshot.EntitlementType == EntitlementType.None, review.AttestationSnapshot.EntitlementType != EntitlementType.None);
                        newAdhocRequest.SetRequestedItems = detail;
                        newRequest.AdHocAccessRequest = newAdhocRequest;
                newRequest.Save(MasterConfigHelper.ServiceUser);
                    }
            catch
            {
                throw;
            }
        }

        private static List<PresentationAccessRequestItem> GetEntitlementDetailFromAttestationReview(AttestationSnapshotAction review, bool isAccountRevoked, bool isEntitlementRevoked)
        {
            List<PresentationAccessRequestItem> liItems = new List<PresentationAccessRequestItem>();

            if (isAccountRevoked)
            {
                var accountRecon = AccountReconcilation.GetById(review.AttestationSnapshot.KeyId);
                var ar = new PresentationAccessRequestItem()
                {
                    EndpointUserId = accountRecon.EndpointUserId,
                    EndpointSystemId = review.AttestationSnapshot.EndpointId,
                    AccessRequestItemType = AccessRequestItemType.Application,
                };
                liItems.Add(ar);
            }
            else
            {
                    var ar = new PresentationAccessRequestItem()
                    {
                    EntitlementType = review.AttestationSnapshot.EntitlementType,
                    EndpointSystemId = review.AttestationSnapshot.EndpointId,
                    ItemName = review.AttestationSnapshot.Name,
                    ItemDetail = review.AttestationSnapshot.Name,
                    ItemDescription = review.AttestationSnapshot.Description,
                        AccessRequestItemType = AccessRequestItemType.Entitlement,
                        AccessItemType = AccessItemType.TechnicalRole
                    };
                    liItems.Add(ar);
                }
            return liItems;
        }

        public static int PopulateOUChangeAccessRequests(User user, EmployeeEventTrigger trigger)
        {
            List<AccessRequest> requests = new List<AccessRequest>();
            AccessRequest newRequest = new AccessRequest();
            AdHocAccessRequest adhocrequst = new AdHocAccessRequest();
            Guid transactionId = Guid.NewGuid();

            try
            {
                var listAcs = AccountReconcilation.GetAllByUser(user.Id, false);

                if (listAcs != null && listAcs.Count > 0)
                {
                    listAcs = listAcs.Where(t => t.IsLocked == false && t.IsDeleted == false).ToList();

                    List<long> userAccountEndpointSystemIds = listAcs.Select(item => item.EndpointSystemId).Distinct().ToList();
                    List<long> newEndpointNeeded = new List<long>();
                    if (trigger != null && !string.IsNullOrEmpty(trigger.UserDetails))
                    {
                        var rulesetForApplication = NRulesetForApplication.GetRulesetForApplication(UtilityMethods.Deserialize<User>(trigger.UserDetails));
                        newEndpointNeeded = rulesetForApplication.Where(a => !userAccountEndpointSystemIds.Contains(a.EndpointSystemId)).Select(a => a.EndpointSystemId).ToList();
                    }

                    foreach (var listAc in listAcs)
                    {
                        newRequest = new AccessRequest();

                        newRequest.UserId = user.Id;
                        newRequest.EndpointSystemId = listAc.EndpointSystemId;

                        newRequest.RequestStatus = AccessRequestStatus.Granted;
                        newRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                        newRequest.RequestSource = AccessRequestSource.OUChange;
                        newRequest.AccountType = listAc.AccountType;

                        newRequest.RequesterId = null;

                        newRequest.ExecutionTime = DateTime.Now;
                        newRequest.Description = "This request is created automatically for OU Change.";
                        newRequest.Remarks = string.Empty;

                        newRequest.TransactionId = transactionId;
                        newRequest.EmployeeEventTriggerId = trigger.Id;



                        adhocrequst = new AdHocAccessRequest();
                        adhocrequst.RequesteeUserId = user.Id;

                        EndPoint ep = EndPoint.GetByIdIncludingEndPointSystemType(listAc.EndpointSystemId);

                        adhocrequst.EndpointSystemType = (DI.Chakra.Infrastructure.Enums.EndpointType)ep.EndPointSystemType.EndpointSystemTypeName;
                        adhocrequst.EndpointSystemId = listAc.EndpointSystemId;
                        adhocrequst.AccountType = listAc.AccountType;
                        adhocrequst.ValidFrom = user.JoinDate;
                        adhocrequst.ValidTo = user.TerminationDate;
                        setRequestItemDetail(adhocrequst, listAc);
                        adhocrequst.Comments = "AdhocAccessRequest created automatically for OU transfer";
                        adhocrequst.Save(MasterConfigHelper.ServiceUser);
                        newRequest.AdHocAccessRequestId = adhocrequst.Id;
                        requests.Add(newRequest);
                    }

                    foreach (var newEp in newEndpointNeeded)
                    {
                        newRequest = new AccessRequest();

                        newRequest.UserId = user.Id;
                        newRequest.EndpointSystemId = newEp;

                        newRequest.RequestStatus = AccessRequestStatus.Granted;
                        newRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                        newRequest.RequestSource = AccessRequestSource.OUChange;
                        newRequest.AccountType = AccountType.User;

                        newRequest.RequesterId = null;

                        newRequest.ExecutionTime = DateTime.Now;
                        newRequest.Description = "This request is created automatically for OU Change.";
                        newRequest.Remarks = string.Empty;

                        newRequest.TransactionId = transactionId;
                        newRequest.EmployeeEventTriggerId = trigger.Id;



                        adhocrequst = new AdHocAccessRequest();
                        adhocrequst.RequesteeUserId = user.Id;

                        EndPoint ep = EndPoint.GetByIdIncludingEndPointSystemType(newEp);

                        adhocrequst.EndpointSystemType = (DI.Chakra.Infrastructure.Enums.EndpointType)ep.EndPointSystemType.EndpointSystemTypeName;
                        adhocrequst.EndpointSystemId = newEp;
                        adhocrequst.AccountType = AccountType.User;
                        adhocrequst.ValidFrom = user.JoinDate;
                        adhocrequst.ValidTo = user.TerminationDate;
                        setRequestItemDetail(adhocrequst, null);
                        adhocrequst.Comments = "AdhocAccessRequest created automatically for OU transfer";
                        adhocrequst.Save(MasterConfigHelper.ServiceUser);
                        newRequest.AdHocAccessRequestId = adhocrequst.Id;
                        requests.Add(newRequest);
                    }
                }

                requests.ForEach(lq => lq.Save(MasterConfigHelper.ServiceUser));
                return requests.Count;
            }
            catch
            {
                throw;
            }
        }

        public static int PopulatePromotionAccessRequests(User user, EmployeeEventTrigger trigger)
        {
            List<AccessRequest> requests = new List<AccessRequest>();
            AccessRequest newRequest = new AccessRequest();
            AdHocAccessRequest adhocrequst = new AdHocAccessRequest();
            Guid transactionId = Guid.NewGuid();

            try
            {
                var listAcs = AccountReconcilation.GetAllByUser(user.Id, false);

                if (listAcs != null && listAcs.Count > 0)
                {
                    listAcs = listAcs.Where(t => t.IsLocked == false && t.IsDeleted == false).ToList();
                    List<long> userAccountEndpointSystemIds = listAcs.Select(item => item.EndpointSystemId).Distinct().ToList();
                    List<long> newEndpointNeeded = new List<long>();

                    if (trigger != null && !string.IsNullOrEmpty(trigger.UserDetails))
                    {
                        var rulesetForApplication = NRulesetForApplication.GetRulesetForApplication(UtilityMethods.Deserialize<User>(trigger.UserDetails));
                        newEndpointNeeded = rulesetForApplication.Where(a => !userAccountEndpointSystemIds.Contains(a.EndpointSystemId)).Select(a => a.EndpointSystemId).ToList();
                    }

                    foreach (var listAc in listAcs)
                    {
                        newRequest = new AccessRequest();

                        newRequest.UserId = user.Id;
                        newRequest.EndpointSystemId = listAc.EndpointSystemId;

                        newRequest.RequestStatus = AccessRequestStatus.Granted;
                        newRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                        newRequest.RequestSource = AccessRequestSource.Promotion;
                        newRequest.AccountType = listAc.AccountType;

                        newRequest.RequesterId = null;

                        newRequest.ExecutionTime = DateTime.Now;
                        newRequest.Description = "This request is created automatically for Promotion Change.";
                        newRequest.Remarks = string.Empty;

                        newRequest.TransactionId = transactionId;
                        newRequest.EmployeeEventTriggerId = trigger.Id;



                        adhocrequst = new AdHocAccessRequest();
                        adhocrequst.RequesteeUserId = user.Id;

                        EndPoint ep = EndPoint.GetByIdIncludingEndPointSystemType(listAc.EndpointSystemId);

                        adhocrequst.EndpointSystemType = (DI.Chakra.Infrastructure.Enums.EndpointType)ep.EndPointSystemType.EndpointSystemTypeName;
                        adhocrequst.EndpointSystemId = listAc.EndpointSystemId;
                        adhocrequst.AccountType = listAc.AccountType;
                        adhocrequst.ValidFrom = user.JoinDate;
                        adhocrequst.ValidTo = user.TerminationDate;
                        setRequestItemDetail(adhocrequst, listAc);
                        adhocrequst.Comments = "AdhocAccessRequest created automatically for Promotion";
                        adhocrequst.Save(MasterConfigHelper.ServiceUser);
                        newRequest.AdHocAccessRequestId = adhocrequst.Id;
                        requests.Add(newRequest);
                    }
                    
                    //for account needed in new department                    
                    foreach (var newEp in newEndpointNeeded)
                    {
                        newRequest = new AccessRequest();

                        newRequest.UserId = user.Id;
                        newRequest.EndpointSystemId = newEp;

                        newRequest.RequestStatus = AccessRequestStatus.Granted;
                        newRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                        newRequest.RequestSource = AccessRequestSource.Promotion;
                        newRequest.AccountType = AccountType.User;

                        newRequest.RequesterId = null;

                        newRequest.ExecutionTime = DateTime.Now;
                        newRequest.Description = "This request is created automatically for Promotion.";
                        newRequest.Remarks = string.Empty;

                        newRequest.TransactionId = transactionId;
                        newRequest.EmployeeEventTriggerId = trigger.Id;



                        adhocrequst = new AdHocAccessRequest();
                        adhocrequst.RequesteeUserId = user.Id;

                        EndPoint ep = EndPoint.GetByIdIncludingEndPointSystemType(newEp);

                        adhocrequst.EndpointSystemType = (DI.Chakra.Infrastructure.Enums.EndpointType)ep.EndPointSystemType.EndpointSystemTypeName;
                        adhocrequst.EndpointSystemId = newEp;
                        adhocrequst.AccountType = AccountType.User;
                        adhocrequst.ValidFrom = user.JoinDate;
                        adhocrequst.ValidTo = user.TerminationDate;
                        setRequestItemDetail(adhocrequst, null);
                        adhocrequst.Comments = "AdhocAccessRequest created automatically for Promotion";
                        adhocrequst.Save(MasterConfigHelper.ServiceUser);
                        newRequest.AdHocAccessRequestId = adhocrequst.Id;
                        requests.Add(newRequest);
                    }
                }

                requests.ForEach(lq => lq.Save(MasterConfigHelper.ServiceUser));
                return requests.Count;
            }
            catch
            {
                throw;
            }
        }

        public static int PopulateReOrgAccessRequests(User user, EmployeeEventTrigger trigger)
        {
            List<AccessRequest> requests = new List<AccessRequest>();
            AccessRequest newRequest = new AccessRequest();
            AdHocAccessRequest adhocrequst = new AdHocAccessRequest();
            Guid transactionId = Guid.NewGuid();

            try
            {
                var listAcs = AccountReconcilation.GetAllByUser(user.Id, false);

                if (listAcs != null && listAcs.Count > 0)
                {
                    listAcs = listAcs.Where(t => t.IsLocked == false && t.IsDeleted == false).ToList();
                    
                    foreach (var listAc in listAcs)
                    {
                        newRequest = new AccessRequest();

                        newRequest.UserId = user.Id;
                        newRequest.EndpointSystemId = listAc.EndpointSystemId;

                        newRequest.RequestStatus = AccessRequestStatus.Granted;
                        newRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                        newRequest.RequestSource = AccessRequestSource.ReOrganisation;
                        newRequest.AccountType = listAc.AccountType;

                        newRequest.RequesterId = null;

                        newRequest.ExecutionTime = DateTime.Now;
                        newRequest.Description = "This request is created automatically for Re Org Request.";
                        newRequest.Remarks = string.Empty;
                        newRequest.TransactionId = transactionId;
                        newRequest.EmployeeEventTriggerId = trigger.Id;

                        adhocrequst = new AdHocAccessRequest();
                        adhocrequst.RequesteeUserId = user.Id;

                        EndPoint ep = EndPoint.GetByIdIncludingEndPointSystemType(listAc.EndpointSystemId);

                        adhocrequst.EndpointSystemType = (DI.Chakra.Infrastructure.Enums.EndpointType)ep.EndPointSystemType.EndpointSystemTypeName;
                        adhocrequst.EndpointSystemId = listAc.EndpointSystemId;
                        adhocrequst.AccountType = listAc.AccountType;
                        adhocrequst.ValidFrom = user.JoinDate;
                        adhocrequst.ValidTo = user.TerminationDate;
                        setRequestItemDetail(adhocrequst, listAc);
                        adhocrequst.Comments = "AdhocAccessRequest created automatically for Reorganisation";
                        adhocrequst.Save(MasterConfigHelper.ServiceUser);
                        newRequest.AdHocAccessRequestId = adhocrequst.Id;
                        requests.Add(newRequest);
                    }
                }
                requests.ForEach(l => l.Save(MasterConfigHelper.ServiceUser));
                return requests.Count;
            }
            catch
            {
                throw;
            }
        }

        public static int PopulatePropagationAccessRequests(long userId, Guid transactionId, EmployeeEventTrigger trigger)
        {
            List<AccessRequest> requests = new List<AccessRequest>();
            AccessRequest newRequest = new AccessRequest();

            try
            {
                var listAc = AccountReconcilation.GetAllByUser(userId, false);

                if (listAc != null && listAc.Count > 0)
                {
                    listAc = listAc.Where(t => t.IsLocked == false && t.IsDeleted == false && (t.AccountType == AccountType.User || t.AccountType == AccountType.Secondary)).ToList();
                    List<long> endpointSystemIds = listAc.Select(item => item.EndpointSystemId).Distinct().ToList();

                    foreach (int endpointSystemId in endpointSystemIds)
                    {
                        newRequest = new AccessRequest();
                        newRequest.UserId = userId;
                        newRequest.EndpointSystemId = endpointSystemId;
                        newRequest.RequestStatus = AccessRequestStatus.Granted;
                        newRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                        newRequest.RequestSource = AccessRequestSource.Propagation;
                        newRequest.RequesterId = null;
                        newRequest.ExecutionTime = DateTime.Now;
                        newRequest.Description = "This request is created automatically for Propagation";
                        newRequest.Remarks = string.Empty;
                        newRequest.TransactionId = transactionId;
                        newRequest.EmployeeEventTriggerId = trigger.Id;

                        var userAccountType = listAc.Where(usr => usr.EndpointSystemId == endpointSystemId && usr.UserId == userId).FirstOrDefault();
                        if (userAccountType != null)
                            newRequest.AccountType = userAccountType.AccountType;

                        requests.Add(newRequest);
                    }
                }
                requests.ForEach(l => l.Save(MasterConfigHelper.ServiceUser));
                return requests.Count;
            }
            catch
            {
                throw;
            }
        }

        private static void CreateOUChangeTrigger(User user, User oldUser)
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
                newTrigger.Save(MasterConfigHelper.ServiceUser);
            }
            catch
            {
                throw;
            }
        }
    }
}
