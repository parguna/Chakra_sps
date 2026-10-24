
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Workflow.Processor.Common;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Workflow.Processor
{
    public class IsPositionPrefixMissing
    {
        UtilityWFFunctions utility = new UtilityWFFunctions();
        public IsPositionPrefixMissingResponse Execute(Task task)
        {
            utility.LogDebugStep("IsPositionPrefixMissingResponse: task" + task);

            var accountTypesNotApplicableForPrefix = new List<AccountType>() { AccountType.Service, AccountType.Shared };

            bool positionPrefixMissing = false;
            if (task == null)
                return new IsPositionPrefixMissingResponse()
                {
                    PositionPrefixMissing = true
                };

            try
            {
                var isAttestationCampaign = false;
                utility.LogDebugStep("IsPositionPrefixMissingResponse: task NOT null");
                var requestType = (RequestType)task.RequestTypeId;
                utility.LogDebugStep("requestType: " + requestType);
                ApplicationType appType = ApplicationType.None;
                AdHocActionType actionType = AdHocActionType.None;
                AccountType accountType = AccountType.None;
                IEntitlementDetail detail = null;
                AccessRequestSource requestSource = AccessRequestSource.None;

                if (requestType == RequestType.Provisioning)
                {
                    utility.LogDebugStep("requestType: " + requestType);
                    var accessRequest = AccessRequest.GetById(task.AccessRequestId.Value, true);
                    isAttestationCampaign = accessRequest.AttestationCampaignId.HasValue && accessRequest.AttestationCampaignId > 0;
                    appType = accessRequest.ApplicationType;
                    actionType = accessRequest.AdHocAccessRequest.AdhocActionType;
                    detail = accessRequest.AdHocAccessRequest.GetEntitlementDetail();
                    accountType = accessRequest.AccountType;
                    requestSource = accessRequest.RequestSource;
                    utility.LogDebugStep($"appType:{appType}, actionType: {actionType}, accountType: {accountType}");
                }
                else if (requestType == RequestType.BulkRequest)
                {
                    utility.LogDebugStep("requestType: " + requestType);
                    var bulkRequest = BulkRequest.GetById(task.BulkRequestId.Value, true);
                    appType = bulkRequest.ApplicationType;
                    actionType = bulkRequest.AdhocActionType;
                    detail = bulkRequest.GetEntitlementDetail();
                    accountType = AccountType.User;
                    requestSource = bulkRequest.RequestSource;
                    utility.LogDebugStep($"appType:{appType}, actionType: {actionType}, accountType: {accountType}");
                }
                if (requestType == RequestType.Provisioning && appType == ApplicationType.VPN && actionType == AdHocActionType.DeleteAccount)
                {

                    var accessRequest = AccessRequest.GetById(task.AccessRequestId.Value, true);
                    if (accessRequest.AdHocAccessRequest != null &&
                        (accessRequest.RequestSource == AccessRequestSource.OUChange ||
                            accessRequest.RequestSource == AccessRequestSource.OffBoarding || accessRequest.AttestationCampaignId.HasValue
                            )
                        )
                    {
                        utility.LogDebugStep($"VPN DeleteAccount Scenario: appType:{appType}, actionType: {actionType}, accountType: {accountType}");
                        var vpnDetails = accessRequest.AdHocAccessRequest.GetEntitlementDetail() as AdHocVPNRequest;
                        if (vpnDetails != null && (vpnDetails.IsOffboardOrTransferRequest || accessRequest.AttestationCampaignId.HasValue))
                        {
                            utility.LogDebugStep($"VPN DeleteAccount Scenario: RequestSource:{accessRequest.RequestSource}, actionType: {actionType}, accountType: {accountType}");
                            return new IsPositionPrefixMissingResponse()
                            {
                                PositionPrefixMissing = false
                            };
                        }
                    }
                }

                if (requestType == RequestType.Provisioning && appType == ApplicationType.InhouseSystems &&
                    (requestSource == AccessRequestSource.OffBoarding ||
                    requestSource == AccessRequestSource.OUChange ||
                    requestSource == AccessRequestSource.UndoEvent ||
                    requestSource == AccessRequestSource.NonEmployeeExtension || isAttestationCampaign))
                {
                    utility.LogDebugStep($"InHouse Systems Skipping the current level: RequestSource:{requestSource}, actionType: {actionType}, accountType: {accountType}");
                    if (task != null && task.IsActionOnlyRequiredTask == true)
                    {
                        return new IsPositionPrefixMissingResponse()
                        {
                            PositionPrefixMissing = true
                        };
                    }
                    else
                    {
                        return new IsPositionPrefixMissingResponse()
                        {
                            PositionPrefixMissing = false
                        };
                    }
                }
                if (requestType == RequestType.Provisioning && isAttestationCampaign && appType == ApplicationType.Office365 && accountType == AccountType.Shared && actionType == AdHocActionType.DeleteAccount)
                {
                    utility.LogDebugStep($"Office365 SharedAccount Skipping the current level for Attestation: RequestSource:{requestSource}, actionType: {actionType}, accountType: {accountType}");

                    return new IsPositionPrefixMissingResponse()
                    {
                        PositionPrefixMissing = false
                    };
                }
                if (requestType == RequestType.Provisioning && isAttestationCampaign && appType == ApplicationType.MicrosoftTeams && (actionType == AdHocActionType.DeleteAccount || actionType == AdHocActionType.AmendmentRemove))
                {
                    var sfbDetails = detail as AdhocSkypeForBusinessRequest;
                    if(sfbDetails != null)
                    {
                        if(sfbDetails.EntitlementType == EntitlementType.IDD)
                        {
                            utility.LogDebugStep($"SkypeForBusiness Skipping the current level for Attestation: RequestSource:{requestSource}, actionType: {actionType}, accountType: {accountType}");

                            return new IsPositionPrefixMissingResponse()
                            {
                                PositionPrefixMissing = false
                            };
                        }
                    }
                }
                if (requestType == RequestType.Provisioning && appType == ApplicationType.VPN && actionType == AdHocActionType.NewAccount)
                {
                    var accessRequest = AccessRequest.GetById(task.AccessRequestId.Value, true);
                    if (accessRequest.AdHocAccessRequest != null && accessRequest.RequestSource == AccessRequestSource.AdhocRequest && accessRequest.EventUndoRequestId.HasValue && accessRequest.EventUndoRequestId > 0)
                    {
                        return new IsPositionPrefixMissingResponse()
                        {
                            PositionPrefixMissing = false
                        };
                    }
                }
                if (((appType != ApplicationType.EBS_CRM && appType != ApplicationType.EBS_ISU && appType != ApplicationType.MSSL_CRM && appType != ApplicationType.MSSL_ISU && appType != ApplicationType.EBS && appType != ApplicationType.MSSL)) ||
                        (accountTypesNotApplicableForPrefix.Contains(accountType)) || actionType == AdHocActionType.DeleteAccount || actionType == AdHocActionType.AmendmentRemove)
                {
                    //utility.LogDebugStep("AppType is neither MSSL nor EBS, hence returning skipping the task");
                    return new IsPositionPrefixMissingResponse()
                    {
                        PositionPrefixMissing = true
                    };
                }

                //Below case only applicable to User account type not for special account
                if (actionType == AdHocActionType.NewAccount && accountType == AccountType.User)
                {
                    if (appType == ApplicationType.EBS || appType == ApplicationType.MSSL)
                    {
                        AdHocSAPHANARequest sapDetail = null;

                        if (requestType == RequestType.Provisioning)
                        {
                            sapDetail = (AdHocSAPHANARequest)detail;
                        }
                        else if (requestType == RequestType.BulkRequest)
                        {
                            sapDetail = (AdHocSAPHANARequest)detail;
                        }
                        if (string.IsNullOrWhiteSpace(sapDetail.PositionPrefix))
                        {
                            positionPrefixMissing = true;
                        }
                    }
                    if (appType == ApplicationType.EBS_CRM || appType == ApplicationType.EBS_ISU)
                    {
                        AdHocEBSRequest sapDetail = null;

                        if (requestType == RequestType.Provisioning)
                        {
                            sapDetail = (AdHocEBSRequest)detail;
                        }
                        else if (requestType == RequestType.BulkRequest)
                        {
                            sapDetail = (AdHocEBSRequest)detail;
                        }
                        if (string.IsNullOrWhiteSpace(sapDetail.PositionPrefix))
                        {
                            positionPrefixMissing = true;
                        }
                    }
                    else if (appType == ApplicationType.MSSL_CRM || appType == ApplicationType.MSSL_ISU)
                    {
                        AdHocMSSLRequest sapDetail = null;

                        if (requestType == RequestType.Provisioning)
                        {
                            sapDetail = (AdHocMSSLRequest)detail;
                        }
                        else if (requestType == RequestType.BulkRequest)
                        {
                            sapDetail = (AdHocMSSLRequest)detail;
                        }
                        if (string.IsNullOrWhiteSpace(sapDetail.PositionPrefix))
                        {
                            positionPrefixMissing = true;
                        }
                    }
                }
                else if (actionType == AdHocActionType.AmendmentAdd && accountType == AccountType.User)
                {
                    if (appType == ApplicationType.EBS || appType == ApplicationType.MSSL)
                    {
                        AdHocSAPHANARequest sapDetail = null;

                        if (requestType == RequestType.Provisioning)
                        {
                            sapDetail = (AdHocSAPHANARequest)detail;
                        }
                        else if (requestType == RequestType.BulkRequest)
                        {
                            sapDetail = (AdHocSAPHANARequest)detail;
                        }
                        if (string.IsNullOrEmpty(sapDetail.PositionPrefix) && sapDetail.IsReallocate && !string.IsNullOrEmpty(sapDetail.Position))
                        {
                            positionPrefixMissing = true;
                        }
                    }
                    if (appType == ApplicationType.EBS_CRM || appType == ApplicationType.EBS_ISU)
                    {
                        AdHocEBSRequest sapDetail = null;

                        if (requestType == RequestType.Provisioning)
                        {
                            sapDetail = (AdHocEBSRequest)detail;
                        }
                        else if (requestType == RequestType.BulkRequest)
                        {
                            sapDetail = (AdHocEBSRequest)detail;
                        }
                        if (string.IsNullOrEmpty(sapDetail.PositionPrefix) && sapDetail.IsReallocate && !string.IsNullOrEmpty(sapDetail.Position))
                        {
                            positionPrefixMissing = true;
                        }
                    }
                    else if (appType == ApplicationType.MSSL_CRM || appType == ApplicationType.MSSL_ISU)
                    {
                        AdHocMSSLRequest sapDetail = null;

                        if (requestType == RequestType.Provisioning)
                        {
                            sapDetail = (AdHocMSSLRequest)detail;
                        }
                        else if (requestType == RequestType.BulkRequest)
                        {
                            sapDetail = (AdHocMSSLRequest)detail;
                        }
                        if (string.IsNullOrEmpty(sapDetail.PositionPrefix) && sapDetail.IsReallocate && !string.IsNullOrEmpty(sapDetail.Position))
                        {
                            positionPrefixMissing = true;
                        }
                    }
                }

                utility.LogDebugStep("positionPrefixMissing: " + positionPrefixMissing);
                if (positionPrefixMissing)
                {
                    if (requestType == RequestType.Provisioning)
                    {
                        var accRequest = AccessRequest.GetById(task.AccessRequestId.Value, true);
                        accRequest.Remarks = "IT security team has to enter the position prefix as the prefix is not available for the selected position.";
                        accRequest.LastModifiedBy = "System";
                        accRequest.LastModifiedDate = DateTime.Now;
                        accRequest.Save(MasterConfigHelper.ServiceUser);
                    }
                    else if (requestType == RequestType.BulkRequest)
                    {
                        var bulkRequest = BulkRequest.GetById(task.BulkRequestId.Value, true);
                        bulkRequest.Remarks = "IT security team has to enter the position prefix as the prefix is not available for the selected position.";
                        bulkRequest.LastModifiedBy = "System";
                        bulkRequest.LastModifiedDate = DateTime.Now;
                        bulkRequest.Save(MasterConfigHelper.ServiceUser);
                    }
                    utility.LogDebugStep("positionPrefixMissing: Task updated with Remarks for IT department");
                }
            }
            catch (Exception ex)
            {
                string message = "Error while executing the IsPositionPrefixMissing activity.";
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, message, ex);
            }

            return new IsPositionPrefixMissingResponse()
            {
                PositionPrefixMissing = positionPrefixMissing
            };
        }
    }
}
