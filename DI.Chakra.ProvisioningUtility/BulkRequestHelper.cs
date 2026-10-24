
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;

namespace DI.Chakra.ProvisioningUtility
{
    public class BulkRequestHelper
    {
        public void CreateBulkAdhoRequestTasksTest(long requestId)
        {
            try
            {
                List<AccessRequest> accessRequests = new List<AccessRequest>();

                var bulkRequest = BulkRequest.GetById(requestId, true);

                if (bulkRequest != null)
                {
                    Guid transactionId = Guid.NewGuid();

                    foreach (BulkRequestDetail detail in bulkRequest.BulkRequestDetails)
                    {
                        AccessRequest newRequest = this.PopulateAccessRequestFromBulkRequest(bulkRequest, detail, transactionId);
                        newRequest.AdHocAccessRequest = this.PopulateAdhocRequestFromBulkRequest(bulkRequest, detail);

                        newRequest.AdHocAccessRequest = this.UpdateEntitlementDetailObject(bulkRequest, detail, newRequest.AdHocAccessRequest);

                        //save new request
                        accessRequests.Add(newRequest);
                    }

                    AccessRequest.SaveAccessRequestList(accessRequests);

                    //bulkRequest.ProgressStatus = AccessRequestProgress.Completed;
                    //bulkRequest.LastModifiedBy = "System";
                    //bulkRequest.LastModifiedDate = DateTime.Now;

                    //bulkRequest.Save(MasterConfigHelper.ServiceUser);
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, string.Format("Error while creating access request for bulk request - {0}", requestId.ToString()), ex);
            }
        }


        public void CreateBulkAdhoRequestTasks()
         {
            List<AccessRequest> accessRequests = new List<AccessRequest>();
            List<BulkRequest> bulkRequests = null;
            try
            {
                bulkRequests = BulkRequest.GetAllBulkRequestsToExecute();

                foreach (var bulkRequest in bulkRequests)
                {
                    if (MasterConfigHelper.IsDeploymentInProgress)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"CreateBulkAdhoRequestTasks - Deployment InProgress");
                        return;
                    }
                    try
                    {
                        if (bulkRequest != null)
                        {
                            Guid transactionId = Guid.NewGuid();

                            foreach (BulkRequestDetail detail in bulkRequest.BulkRequestDetails)
                            {
                                try
                                {
                                    
                                    AccessRequest newRequest = this.PopulateAccessRequestFromBulkRequest(bulkRequest, detail, transactionId);
                                    newRequest.AdHocAccessRequest = this.PopulateAdhocRequestFromBulkRequest(bulkRequest, detail);
                                    if (AccessRequest.IsSameAccessRequestExistsForBulkRequestId(bulkRequest.Id, newRequest.UserId))
                                    {
                                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"Same AccessRequest already created for this user:{detail?.Name} - {bulkRequest.Id}");
                                        continue;
                                    }
                                    var user = User.GetById(newRequest.UserId);

                                    if (newRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount ||
                                        newRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                                    {
                                        if (user.FullTimeEmployee)
                                        {
                                            newRequest.DeactivationDate = DateTime.MaxValue;
                                            newRequest.AdHocAccessRequest.ValidTo = DateTime.MaxValue;
                                        }
                                        else
                                        {
                                            newRequest.DeactivationDate = user.TerminationDate ?? DateTime.MaxValue;
                                            newRequest.AdHocAccessRequest.ValidTo = user.TerminationDate ?? DateTime.MaxValue;
                                        }
                                    }
                                    newRequest.AdHocAccessRequest = this.UpdateEntitlementDetailObject(bulkRequest, detail, newRequest.AdHocAccessRequest);

                                    if (user.IsActive == false)
                                    {
                                        newRequest.Comments = $"The user with PERNR-[{user?.EmployeeId}] is inactive while populate access request from Bulk Request Id - {bulkRequest.Id}. Hence IDMS withdrawn the request.";

                                        newRequest.Remarks = DateTime.Now.ToString();
                                        newRequest.RequestStatus = AccessRequestStatus.Withdrawn;
                                        newRequest.ProgressStatus = AccessRequestProgress.Withdrawn;

                                    }
                                    //save new request
                                    accessRequests.Add(newRequest);
                                }
                                catch (Exception ex)
                                {
                                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, string.Format("Error while creating access request for bulk request - {0}", bulkRequest.Id.ToString()), ex);
                                }
                            }

                            AccessRequest.SaveAccessRequestList(accessRequests);

                            bulkRequest.ProgressStatus = AccessRequestProgress.Executing;
                            bulkRequest.LastModifiedBy = "System";
                            bulkRequest.LastModifiedDate = DateTime.Now;
                            bulkRequest.Save(MasterConfigHelper.ServiceUser);
                        }
                    }
                    catch(Exception ex)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, $"Error while creating access request for bulk request.{bulkRequest?.Id}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while creating access request for bulk request.", ex);
            }
            finally
            {
                bulkRequests?.Clear();
                bulkRequests = null;

                accessRequests?.Clear();
                accessRequests = null;
            }
        }




        private AccessRequest PopulateAccessRequestFromBulkRequest(BulkRequest bulkRequest, BulkRequestDetail bulkRequestDetail, Guid transactionId)
        {
            var request = new AccessRequest()
            {
                Id = 0,
                AdHocAccessRequestId = 0,
                RequestSource = AccessRequestSource.AdhocRequest,
                AccountType = AccountType.User,
                ApplicationType = bulkRequest.ApplicationType,
                ActivationDate = bulkRequest.ActivationDate,
                ApproverType = bulkRequest.ApproverType,
                Comments = bulkRequest.Description,
                CreatedBy = bulkRequest.CreatedBy,
                CreatedDate = bulkRequest.CreatedDate,
                DeactivationDate = bulkRequest.DeactivationDate,
                Description = bulkRequest.Description,
                ExecutionTime = bulkRequest.ExecutionTime,
                IsCurrentLevelApproved = bulkRequest.IsCurrentLevelApproved,
                LastModifiedBy = bulkRequest.LastModifiedBy,
                LastModifiedDate = bulkRequest.LastModifiedDate,
                ProgressStatus = bulkRequest.ProgressStatus,
                RequestStatus = bulkRequest.RequestStatus,
                TransactionId = transactionId,

                RequesterId = bulkRequest.RequestedBy,
                UserId = bulkRequestDetail.UserId,
                BulkRequestId = bulkRequest.Id,

                OrgBranch = bulkRequestDetail.OrgBranch,
                OrgCompany = bulkRequestDetail.OrgCompany,
                OrgDepartment = bulkRequestDetail.OrgDepartment,
                OrgDivision = bulkRequestDetail.OrgDivision,
                OrgSection = bulkRequestDetail.OrgSection,
                OrgUnit = bulkRequestDetail.OrgUnit,
                EndpointSystemId = bulkRequest.EndpointSystemId,
                Remarks = "This request is created by system to create access request related to the bulk request - " + bulkRequest.Id,

            };

            return request;
        }

        private AdHocAccessRequest PopulateAdhocRequestFromBulkRequest(BulkRequest bulkRequest, BulkRequestDetail bulkRequestDetail)
        {
            return new AdHocAccessRequest()
            {
                Id = 0,
                AccountType = AccountType.User,
                IsSelfRequest = false,

                AdhocActionType = bulkRequest.AdhocActionType,
                Comments = bulkRequest.Description,
                CreatedBy = bulkRequest.CreatedBy,
                CreatedDate = bulkRequest.CreatedDate,
                EndpointSystemType = bulkRequest.EndpointType,
                ApplicationType = bulkRequest.ApplicationType,
                RequesteeUserId = bulkRequestDetail.UserId,
                RequesterUserId = bulkRequest.RequestedBy,
                ValidFrom = bulkRequest.ActivationDate,
                ValidTo = bulkRequest.DeactivationDate,
                EntitlementDetail = bulkRequest.EntitlementDetail,
                EndpointSystemId = bulkRequest.EndpointSystemId,
            };
        }

        private AdHocAccessRequest UpdateEntitlementDetailObject(BulkRequest bulkRequest, BulkRequestDetail bulkRequestDetail, AdHocAccessRequest adhocRequest)
        {
            EndPoint endpoint = null;



            if (adhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                if (adhocRequest.ApplicationType == ApplicationType.SSIDS_SMS)
                {
                    AdHocSSIDSRequest adHocSSIDSRequest = (AdHocSSIDSRequest)adhocRequest.GetEntitlementDetail();
                    if (adHocSSIDSRequest != null && adHocSSIDSRequest.EntitlementType == EntitlementType.HandPhone)
                    {
                        adHocSSIDSRequest.Entitlement = bulkRequestDetail.HandPhone;
                        adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(adHocSSIDSRequest);
                    }
                    return adhocRequest;
                }
                else if (adhocRequest.ApplicationType == ApplicationType.VPN)
                {
                    endpoint = EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                    var vpnUser = ADSUserInfo.GetActivePrimaryByDIUserId(bulkRequestDetail.UserId, endpoint.Id);

                    if (vpnUser != null)
                    {
                        var user = User.GetById(bulkRequestDetail.UserId);
                        var entitlementDetailObject = UtilityMethods.Deserialize<AdHocVPNRequest>(adhocRequest.EntitlementDetail);
                        entitlementDetailObject.EndpointUserId = vpnUser.Id;
                        entitlementDetailObject.OneKey = user.OneKeyTokenId;
                        adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(entitlementDetailObject);
                    }

                    return adhocRequest;
                }
         
                else if (adhocRequest.ApplicationType == ApplicationType.SRM)
                {

                    endpoint = EndPoint.GetByApplicationType(ApplicationType.HRMS_FLINS);
                    var srmUser = SAPUserinfo.GetPrimaryByDIUserId(bulkRequestDetail.UserId, endpoint.Id);

                    if (srmUser != null)
                    {
                        var entitlementDetailObject = UtilityMethods.Deserialize<AdHocSAPSRMRequest>(adhocRequest.EntitlementDetail);
                        if (bulkRequestDetail.AccountName == string.Empty)
                            entitlementDetailObject.AccountName = srmUser.UserLogonName;
                        else
                        {
                            entitlementDetailObject.AccountName = bulkRequestDetail.AccountName;

                        }
                        adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(entitlementDetailObject);
                    }

                    return adhocRequest;
                }

                else if (adhocRequest.ApplicationType == ApplicationType.InhouseSystems)
                {
                    var inhUser = InhouseUserInfo.GetPrimaryByDIUserId(bulkRequestDetail.UserId, bulkRequest.EndpointSystemId);

                    if (inhUser != null)
                    {
                        var entitlementDetailObject = UtilityMethods.Deserialize<AdHocINHRequest>(adhocRequest.EntitlementDetail);
                        DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"Before Update entitlementDetailObject:{adhocRequest.EntitlementDetail}");
                        if (!bulkRequestDetail.EndpointUserId.HasValue)
                        {
                            entitlementDetailObject.EndpointUserId = inhUser.Id;
                        }
                        else
                        {
                            entitlementDetailObject.EndpointUserId = bulkRequestDetail.EndpointUserId.Value;
                        }
                        adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(entitlementDetailObject);
                        DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"Before Update entitlementDetailObject:{adhocRequest.EntitlementDetail}");
                    }
                    return adhocRequest;
                }
                else
                {
                    adhocRequest.EntitlementDetail = bulkRequest.EntitlementDetail;
                    return adhocRequest;
                }
            }
            else
            {
                switch (adhocRequest.ApplicationType)
                {
                    case ApplicationType.ADSingaporePowerLocal:

                        var adUser = ADSUserInfo.GetActivePrimaryByDIUserId(bulkRequestDetail.UserId, bulkRequest.EndpointSystemId);

                        if (adUser != null)
                        {
                            var entitlementDetailObject = UtilityMethods.Deserialize<AdHocADRequest>(adhocRequest.EntitlementDetail);
                            if (!bulkRequestDetail.EndpointUserId.HasValue)
                            {
                                entitlementDetailObject.EndpointUserId = adUser.Id;
                            }
                            else
                            {
                                entitlementDetailObject.EndpointUserId = bulkRequestDetail.EndpointUserId.Value;
                            }
                            adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(entitlementDetailObject);
                        }

                        return adhocRequest;

                    case ApplicationType.Office365:
                        var o365User = Office365UserInfo.GetPrimaryByDIUserId(bulkRequestDetail.UserId, bulkRequest.EndpointSystemId);

                        if (o365User != null)
                        {
                            var entitlementDetailObject = UtilityMethods.Deserialize<AdHocO365Request>(adhocRequest.EntitlementDetail);
                            if (!bulkRequestDetail.EndpointUserId.HasValue)
                            {
                                entitlementDetailObject.EndpointUserId = o365User.Id;
                            }
                            else
                            {
                                entitlementDetailObject.EndpointUserId = bulkRequestDetail.EndpointUserId.Value;
                            }
                            adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(entitlementDetailObject);
                        }
                        return adhocRequest;

                    case ApplicationType.HRMS_FLINS:
                        var sapUser = SAPUserinfo.GetPrimaryByDIUserId(bulkRequestDetail.UserId, bulkRequest.EndpointSystemId);

                        if (sapUser != null)
                        {
                            var entitlementDetailObject = UtilityMethods.Deserialize<AdHocSAPHRFLINSRequest>(adhocRequest.EntitlementDetail);
                            if (!bulkRequestDetail.EndpointUserId.HasValue)
                            {
                                entitlementDetailObject.EndpointUserId = sapUser.Id;
                            }
                            else
                            {
                                entitlementDetailObject.EndpointUserId = bulkRequestDetail.EndpointUserId.Value;
                            }
                            adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(entitlementDetailObject);
                        }
                        return adhocRequest;

                    case ApplicationType.SRM:
                        endpoint = EndPoint.GetByApplicationType(ApplicationType.HRMS_FLINS);
                        var srmUser = SAPUserinfo.GetPrimaryByDIUserId(bulkRequestDetail.UserId, endpoint.Id);

                        if (srmUser != null)
                        {
                            var entitlementDetailObject = UtilityMethods.Deserialize<AdHocSAPSRMRequest>(adhocRequest.EntitlementDetail);
                            if (bulkRequestDetail.AccountName == string.Empty)
                                entitlementDetailObject.AccountName = srmUser.UserLogonName;
                            else
                            {
                                entitlementDetailObject.AccountName = bulkRequestDetail.AccountName;

                            }
                            adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(entitlementDetailObject);
                        }
                        return adhocRequest;

                    case ApplicationType.MSSL_ISU:
                    case ApplicationType.MSSL_CRM:
                        var msslUser = SAPUserinfo.GetPrimaryByDIUserId(bulkRequestDetail.UserId, bulkRequest.EndpointSystemId);

                        if (msslUser != null)
                        {
                            var entitlementDetailObject = UtilityMethods.Deserialize<AdHocMSSLRequest>(adhocRequest.EntitlementDetail);
                            if (!bulkRequestDetail.EndpointUserId.HasValue)
                            {
                                entitlementDetailObject.EndpointUserId = msslUser.Id;
                            }
                            else
                            {
                                entitlementDetailObject.EndpointUserId = bulkRequestDetail.EndpointUserId.Value;
                            }
                            adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(entitlementDetailObject);
                        }
                        return adhocRequest;

                    case ApplicationType.EBS_ISU:
                    case ApplicationType.EBS_CRM:
                        var ebsUser = SAPUserinfo.GetPrimaryByDIUserId(bulkRequestDetail.UserId, bulkRequest.EndpointSystemId);

                        if (ebsUser != null)
                        {
                            var entitlementDetailObject = UtilityMethods.Deserialize<AdHocEBSRequest>(adhocRequest.EntitlementDetail);
                            if (!bulkRequestDetail.EndpointUserId.HasValue)
                            {
                                entitlementDetailObject.EndpointUserId = ebsUser.Id;
                            }
                            else
                            {
                                entitlementDetailObject.EndpointUserId = bulkRequestDetail.EndpointUserId.Value;
                            }
                            adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(entitlementDetailObject);
                        }
                        return adhocRequest;

                    case ApplicationType.SAP_PI:
                        var piUser = SAPUserinfo.GetPrimaryByDIUserId(bulkRequestDetail.UserId, bulkRequest.EndpointSystemId);

                        if (piUser != null)
                        {
                            var entitlementDetailObject = UtilityMethods.Deserialize<AdHocSAPPIRequest>(adhocRequest.EntitlementDetail);
                            if (!bulkRequestDetail.EndpointUserId.HasValue)
                            {
                                entitlementDetailObject.EndpointUserId = piUser.Id;
                            }
                            else
                            {
                                entitlementDetailObject.EndpointUserId = bulkRequestDetail.EndpointUserId.Value;
                            }
                            adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(entitlementDetailObject);
                        }
                        return adhocRequest;

                    case ApplicationType.SAP_GRC:
                        var grcUser = SAPUserinfo.GetPrimaryByDIUserId(bulkRequestDetail.UserId, bulkRequest.EndpointSystemId);

                        if (grcUser != null)
                        {
                            var entitlementDetailObject = UtilityMethods.Deserialize<AdHocSAPGRCRequest>(adhocRequest.EntitlementDetail);
                            if (!bulkRequestDetail.EndpointUserId.HasValue)
                            {
                                entitlementDetailObject.EndpointUserId = grcUser.Id;
                            }
                            else
                            {
                                entitlementDetailObject.EndpointUserId = bulkRequestDetail.EndpointUserId.Value;
                            }
                            adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(entitlementDetailObject);
                        }
                        return adhocRequest;

                    case ApplicationType.FLINS_BI:
                        var biUser = SAPUserinfo.GetPrimaryByDIUserId(bulkRequestDetail.UserId, bulkRequest.EndpointSystemId);

                        if (biUser != null)
                        {
                            var entitlementDetailObject = UtilityMethods.Deserialize<AdHocSAPFLINSBIRequest>(adhocRequest.EntitlementDetail);
                            if (!bulkRequestDetail.EndpointUserId.HasValue)
                            {
                                entitlementDetailObject.EndpointUserId = biUser.Id;
                            }
                            else
                            {
                                entitlementDetailObject.EndpointUserId = bulkRequestDetail.EndpointUserId.Value;
                            }
                            adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(entitlementDetailObject);
                        }
                        return adhocRequest;

                    case ApplicationType.VPN:
                        endpoint = EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                        var vpnUser = VPNUserInfo.GetByEndpointUserId(bulkRequestDetail.EndpointUserId.Value);

                        if (vpnUser != null)
                        {
                            var user = User.GetById(bulkRequestDetail.UserId);
                            var entitlementDetailObject = UtilityMethods.Deserialize<AdHocVPNRequest>(adhocRequest.EntitlementDetail);
                            if (!bulkRequestDetail.EndpointUserId.HasValue)
                            {
                                entitlementDetailObject.EndpointUserId = vpnUser.Id;
                            }
                            else
                            {
                                entitlementDetailObject.EndpointUserId = bulkRequestDetail.EndpointUserId.Value;
                            }
                            entitlementDetailObject.OneKey = user.OneKeyTokenId;
                            adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(entitlementDetailObject);
                        }

                        return adhocRequest;

                    case ApplicationType.SSIDS_SMS:
                        AdHocSSIDSRequest adHocSSIDSRequest = (AdHocSSIDSRequest)adhocRequest.GetEntitlementDetail();
                        var epUser = SSIDSUserInfo.GetPrimaryByDIUserId(bulkRequestDetail.UserId, bulkRequest.EndpointSystemId);
                        if (epUser != null)
                        {
                            adHocSSIDSRequest.EndpointUserId = epUser.Id;
                        }

                        if (adHocSSIDSRequest != null && adHocSSIDSRequest.EntitlementType == EntitlementType.HandPhone)
                        {
                            adHocSSIDSRequest.Entitlement = bulkRequestDetail.HandPhone;
                            adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(adHocSSIDSRequest);
                        }
                        return adhocRequest;
                    case ApplicationType.SSIDS_GUI:
                        var adHocSSIDSReq = (AdHocSSIDSRequest)adhocRequest.GetEntitlementDetail();
                        var epUsr = SSIDSUserInfo.GetPrimaryByDIUserId(bulkRequestDetail.UserId, bulkRequest.EndpointSystemId);
                        if (epUsr != null)
                        {
                            adHocSSIDSReq.EndpointUserId = epUsr.Id;
                        }

                        if (adHocSSIDSReq != null && adHocSSIDSReq.EntitlementType == EntitlementType.Role)
                        {
                            adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(adHocSSIDSReq);
                        }
                        return adhocRequest;
                    case ApplicationType.NPAC:
                        {
                            var entitlementDetailObject = UtilityMethods.Deserialize<AdHocNPACRequest>(adhocRequest.EntitlementDetail);
                            if (!bulkRequestDetail.EndpointUserId.HasValue)
                            {
                                var npacUser = NPACUserInfo.GetPrimaryByDIUserId(bulkRequestDetail.UserId, bulkRequest.EndpointSystemId);
                                if (npacUser != null)
                                    entitlementDetailObject.EndpointUserId = npacUser.Id;
                            }
                            else
                            {
                                entitlementDetailObject.EndpointUserId = bulkRequestDetail.EndpointUserId.Value;
                            }
                            adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(entitlementDetailObject);
                        }
                        return adhocRequest;

                    case ApplicationType.GEMS:
                        {
                            var entitlementDetailObject = UtilityMethods.Deserialize<AdHocGEMSRequest>(adhocRequest.EntitlementDetail);
                            if (!bulkRequestDetail.EndpointUserId.HasValue)
                            {
                                var gemUser = GEMSUserInfo.GetPrimaryByDIUserId(bulkRequestDetail.UserId, bulkRequest.EndpointSystemId);
                                if (gemUser != null)
                                    entitlementDetailObject.EndpointUserId = gemUser.Id;
                            }
                            else
                            {
                                entitlementDetailObject.EndpointUserId = bulkRequestDetail.EndpointUserId.Value;
                            }
                            adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(entitlementDetailObject);
                        }
                        return adhocRequest;

                    case ApplicationType.SAP_EP:
                    case ApplicationType.CE:
                        var sapepUser = SAPEPUserInfo.GetPrimaryByDIUserId(bulkRequestDetail.UserId, bulkRequest.EndpointSystemId);

                        if (sapepUser != null)
                        {
                            var entitlementDetailObject = UtilityMethods.Deserialize<AdhocSAPEPRequest>(adhocRequest.EntitlementDetail);
                            if (!bulkRequestDetail.EndpointUserId.HasValue)
                            {
                                entitlementDetailObject.EndpointUserId = sapepUser.Id;
                            }
                            else
                            {
                                entitlementDetailObject.EndpointUserId = bulkRequestDetail.EndpointUserId.Value;
                            }
                            adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(entitlementDetailObject);
                        }
                        return adhocRequest;

                    case ApplicationType.InhouseSystems:
                        var inhUser = InhouseUserInfo.GetPrimaryByDIUserId(bulkRequestDetail.UserId, bulkRequest.EndpointSystemId);

                        if (inhUser != null)
                        {
                            var entitlementDetailObject = UtilityMethods.Deserialize<AdHocINHRequest>(adhocRequest.EntitlementDetail);
                            DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"Before Update entitlementDetailObject:{entitlementDetailObject}");
                            if (!bulkRequestDetail.EndpointUserId.HasValue)
                            {
                                entitlementDetailObject.EndpointUserId = inhUser.Id;
                            }
                            else
                            {
                                entitlementDetailObject.EndpointUserId = bulkRequestDetail.EndpointUserId.Value;
                            }
                            DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"After Update entitlementDetailObject:{entitlementDetailObject}");
                            adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(entitlementDetailObject);
                        }
                        return adhocRequest;

                    case ApplicationType.OneCard:
                        {
                            var entitlementDetailObject = UtilityMethods.Deserialize<AdHocSMSRequest>(adhocRequest.EntitlementDetail);
                            if (!bulkRequestDetail.EndpointUserId.HasValue)
                            {
                                var smsUser = SMSUserInfo.GetPrimaryByDIUserId(bulkRequestDetail.UserId, bulkRequest.EndpointSystemId);
                                if (smsUser != null)
                                    entitlementDetailObject.EndpointUserId = smsUser.UserId;
                            }
                            else
                            {
                                entitlementDetailObject.EndpointUserId = bulkRequestDetail.EndpointUserId.Value;
                            }
                            adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(entitlementDetailObject);
                        }
                        return adhocRequest;
                    case ApplicationType.MicrosoftTeams:
                        var sfbUser = SkypeForBusinessUserInfo.GetPrimaryByDIUserId(bulkRequestDetail.UserId, bulkRequest.EndpointSystemId);
                        if (sfbUser != null)
                        {
                            var entitlementDetailObject = UtilityMethods.Deserialize<AdhocSkypeForBusinessRequest>(adhocRequest.EntitlementDetail);
                            if (!bulkRequestDetail.EndpointUserId.HasValue)
                            {
                                entitlementDetailObject.EndpointUserId = sfbUser.Id;
                            }
                            else
                            {
                                entitlementDetailObject.EndpointUserId = bulkRequestDetail.EndpointUserId.Value;
                            }
                            adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(entitlementDetailObject);
                        }
                        return adhocRequest;

                    case ApplicationType.IDMS:

                        var IDMSUser = IACMUserInfo.GetPrimaryByDIUserId(bulkRequestDetail.UserId, bulkRequest.EndpointSystemId);

                        if (IDMSUser != null)
                        {
                            var entitlementDetailObject = UtilityMethods.Deserialize<AdHocIACMRequest>(adhocRequest.EntitlementDetail);
                            if (!bulkRequestDetail.EndpointUserId.HasValue)
                            {
                                entitlementDetailObject.EndpointUserId = IDMSUser.Id;
                            }
                            else
                            {
                                entitlementDetailObject.EndpointUserId = bulkRequestDetail.EndpointUserId.Value;
                            }
                            adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(entitlementDetailObject);
                        }

                        return adhocRequest;
                    case ApplicationType.EBS:
                    case ApplicationType.MSSL:
                        var hanaUser = SAPHANAUserinfo.GetPrimaryByDIUserId(bulkRequestDetail.UserId, bulkRequest.EndpointSystemId);

                        if (hanaUser != null)
                        {
                            var entitlementDetailObject = UtilityMethods.Deserialize<AdHocSAPHANARequest>(adhocRequest.EntitlementDetail);
                            if (!bulkRequestDetail.EndpointUserId.HasValue)
                            {
                                entitlementDetailObject.EndpointUserId = hanaUser.Id;
                            }
                            else
                            {
                                entitlementDetailObject.EndpointUserId = bulkRequestDetail.EndpointUserId.Value;
                            }
                            adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(entitlementDetailObject);
                        }
                        return adhocRequest;
                    default:
                        return null;
                }
            }
        }

    }
}

