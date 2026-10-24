 
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Workflow.Processor.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Workflow.Processor
{
    public class CreateADRequestForGEMS
    {
        UtilityWFFunctions utility = new UtilityWFFunctions();
       
        public void Execute(Task task)
        {
            try
            {
                
                if (task == null) return;
                var existingTask = Task.GetProvisioningTask(task.Id);
                if(existingTask != null)
                {
                    var requestType = (RequestType)existingTask.RequestTypeId;

                    AdHocAccessRequest adhocRequest = null;

                    ApplicationType appType = ApplicationType.None;
                    AccessRequestStatus status = AccessRequestStatus.None;
                    AdHocActionType actionType = AdHocActionType.None;
                    AdHocGEMSRequest gemsDetail = null;
                    adhocRequest = existingTask.AccessRequest.AdHocAccessRequest;
                    status = existingTask.AccessRequest.RequestStatus;
                    actionType = adhocRequest.AdhocActionType;
                    appType = adhocRequest.ApplicationType;

                    if (appType == ApplicationType.GEMS)
                    {
                        if(requestType == RequestType.Provisioning)
                        {
                            if (status == AccessRequestStatus.Approved)
                            {
                                if (actionType == AdHocActionType.NewAccount)
                                {
                                    gemsDetail = (AdHocGEMSRequest)adhocRequest.GetEntitlementDetail();
                                    if (gemsDetail.EndpointUserId == 0)
                                    {
                                        this.CreateADAccountRequest(existingTask.AccessRequest, adhocRequest);
                                    }
                                }
                            }
                        }
                    }
                    
                }
                
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null,null,LogLevelType.Exception, "Workflow.Processor - CreateADRequestForGEMS.Execute", ex);
            }
        }

        private void CreateADAccountRequest(AccessRequest request, AdHocAccessRequest adhocAccessRequest)
        {
            try
            {
                var currentUser = MasterConfigHelper.ServiceUser;
                var adEndpoint = EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                var adUsers = ADSUserInfo.GetADUsersByDIUserId(request.AdHocAccessRequest.RequesteeUserId, adEndpoint.Id,AccountType.User);
                if (adUsers.Count > 0)
                {
                    return;
                }
                AccessRequest newRequest = utility.PopulateNewFromOldAccessRequest(request);
                newRequest.AdHocAccessRequest = utility.PopulateNewFromOldAdhocRequest(adhocAccessRequest);
                newRequest.IsDependentRequest = true;
                newRequest.TransactionId = Guid.NewGuid();
                newRequest.AdHocAccessRequest.AdhocActionType = AdHocActionType.NewAccount;
                newRequest.AdHocAccessRequest.EndpointSystemId = adEndpoint.Id;
                newRequest.EndpointSystemId = adEndpoint.Id;
                AdHocADRequest adDetail = new AdHocADRequest()
                {
                    EndpointUserId = 0,
                    Entitlement = string.Empty,
                    EntitlementType = EntitlementType.Group,
                };

                newRequest.AdHocAccessRequest.EntitlementDetail = newRequest.AdHocAccessRequest.GetSerializedString(adDetail);
                newRequest.Save(currentUser);

                request.IsParentRequest = true;
                request.IsDependentRequestCreated = true;
                request.ParentRequestId = newRequest.Id;
                request.Save(currentUser);
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Workflow.Processor - CreateADAccountRequestForGEMS", ex);
            }
        }
        //private void RevertADAccountCreationForVPN(AccessRequest request, AdHocAccessRequest adhocAccessRequest)
        //{
        //    try
        //    {
        //        var currentUser = MasterConfigHelper.ServiceUser;
        //        AccessRequest newRequest = utility.PopulateNewFromOldAccessRequest(request);
        //        if (newRequest.IsParentRequest && newRequest.IsDependentRequestCreated && newRequest.ParentRequestId.HasValue && newRequest.ParentRequestId > 0)
        //        {
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Provisioning Engine - RevertADAccountCreationForVPN", ex);
        //    }
        //}
    }
}
