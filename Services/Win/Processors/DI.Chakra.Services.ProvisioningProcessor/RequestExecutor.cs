
using DI.Chakra.Connector;
using DI.Chakra.DomainModel;
using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.ProvisioningUtility;
using DI.Chakra.Services.EmailingProcessor;
using DI.Chakra.Services.ProvisioningProcessor.FormExecutors;
using DI.Chakra.Workflow.ServiceHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DI.Chakra.Services.ProvisioningProcessor
{
    public class RequestExecutor
    {
        readonly bool canCheckUserActive;

        public RequestExecutor()
        {
            canCheckUserActive = MasterConfigHelper.CanCheckUserActiveForProvisioning;
        }

        public bool ExecuteAccessRequest(List<AccessRequest> accessRequests)
        {
            bool isExecutedSuccessfully = false;

            IdmsLogger idmsLogger;
           foreach (AccessRequest accessRequest in accessRequests)
            {

                try
                {
                    var endpoint = EndPoint.GetById(accessRequest.EndpointSystemId.Value, true);

                    idmsLogger = new ProvisioningLogger(accessRequest.Id, accessRequest.EndpointSystemId.Value, endpoint.EndpointSystemTypeId, endpoint.DisplayName);
                    DILog.ProvisioningLogger = (ProvisioningLogger)idmsLogger;
                    idmsLogger.WriteLog(LogLevelType.Info, "AccessRequest Execution started, ID: " + accessRequest.Id);


                    if (accessRequest.RequestSource == AccessRequestSource.OnBoarding || accessRequest.RequestSource == AccessRequestSource.AdhocRequest)
                    {
                        isExecutedSuccessfully = ExecuteOnBoardingAndAdhocRequest(accessRequest, idmsLogger);
                        //var ep = EndPoint.GetByApplicationType(ApplicationType.Office365);
                        //if (accessRequest.EndpointSystemId.HasValue && accessRequest.EndpointSystemId.Value == ep.Id && accessRequest.AccountType == AccountType.User
                        //    && (accessRequest?.AdHocAccessRequest?.AdhocActionType == AdHocActionType.NewAccount || accessRequest.RequestSource == AccessRequestSource.OnBoarding))
                        //{
                        //    var user = User.GetById(accessRequest.UserId);
                        //    if (!user.FullTimeEmployee && accessRequest?.AdHocAccessRequest?.AdhocActionType == AdHocActionType.NewAccount)
                        //    {
                        //        isExecutedSuccessfully = ExecuteOnBoardingAndAdhocRequest(accessRequest, idmsLogger);
                        //    }
                        //    else
                        //    {
                        //        if (!string.IsNullOrEmpty(accessRequest.AdditionalInfo) && accessRequest.AdditionalInfo == "MailBox")
                        //        {
                        //            isExecutedSuccessfully = ExecuteO365MailBoxSettingsRequest(accessRequest, ep, idmsLogger);
                        //        }
                        //        else
                        //        {
                        //            isExecutedSuccessfully = ExecuteO365LicenseAssignmentRequest(accessRequest, ep, idmsLogger);
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    isExecutedSuccessfully = ExecuteOnBoardingAndAdhocRequest(accessRequest, idmsLogger);
                        //}
                    }
                    else if (accessRequest.RequestSource == AccessRequestSource.OffBoarding)
                    {
                        isExecutedSuccessfully = ExecuteOffBoardingRequest(accessRequest, endpoint, idmsLogger);
                    }
                    else if (accessRequest.RequestSource == AccessRequestSource.OUChange)
                    {
                        isExecutedSuccessfully = ExecuteOUChangeRequest(accessRequest, endpoint, idmsLogger);
                    }
                    else if (accessRequest.RequestSource == AccessRequestSource.ReOrganisation)
                    {
                        isExecutedSuccessfully = ExecuteReOrgRequest(accessRequest, endpoint, idmsLogger);
                    }
                    else if (accessRequest.RequestSource == AccessRequestSource.Propagation)
                    {
                        isExecutedSuccessfully = ExecutePropagationRequest(accessRequest, endpoint, idmsLogger);
                    }
                    else if (accessRequest.RequestSource == AccessRequestSource.NonEmployeeExtension)
                    {
                        isExecutedSuccessfully = ExecuteExtensionRequest(accessRequest, endpoint, idmsLogger);
                    }
                    else if (accessRequest.RequestSource == AccessRequestSource.UndoEvent)
                    {
                        isExecutedSuccessfully = ExecuteUndoEventRequest(accessRequest, idmsLogger);
                    }
                    else if (accessRequest.RequestSource == AccessRequestSource.AttestationRemediation)
                    {
                        isExecutedSuccessfully = ExecuteRemediationRequest(accessRequest, endpoint, idmsLogger);
                    }

                    //if (!isExecutedSuccessfully)
                    //{
                    //    ProvisioningEmailHelper.AccessRequestFailureNotification(accessRequest, usr);
                    //}

                    idmsLogger.WriteLog(LogLevelType.Info, $"AccessRequest Execution {(isExecutedSuccessfully ? "Completed" : "failed")} - ID:{ accessRequest.Id}");
                }
                catch (FormExecutorValidationFailedException fve)
                {
                    DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, string.Format("Validation Failed while executing access request - {0}.", accessRequest), fve);
                }
                catch (Exception ex)
                {
                    DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, string.Format("Error while executing access request - {0}.", accessRequest), ex);
                }
            }

            return isExecutedSuccessfully;
        }

        void UpdateBulkRequestStatus(long bulkRequestId)
        {
            IList<AccessRequest> childRequests = null;
            BulkRequest bulkRequest = null;
            try
            {
                bulkRequest = BulkRequest.GetById(bulkRequestId);
                childRequests = AccessRequest.GetChildRequestByBulkRequestId(bulkRequestId);
                AccessRequestProgress status = bulkRequest.ProgressStatus;
                if (childRequests != null)
                {
                    if (childRequests.All(p => (p.ProgressStatus == AccessRequestProgress.Completed || p.ProgressStatus == AccessRequestProgress.Withdrawn || p.ProgressStatus == AccessRequestProgress.RequestRejected || p.ProgressStatus == AccessRequestProgress.ManuallyResolved)))
                        status = AccessRequestProgress.Completed;
                    else if (childRequests.Any(p => p.ProgressStatus == AccessRequestProgress.ExecutionFailed))
                        status = AccessRequestProgress.ExecutionFailed;
                    if (childRequests.Any(p => p.ProgressStatus == AccessRequestProgress.ValidationFailed))
                        status = AccessRequestProgress.ValidationFailed;
                }

                if (status == AccessRequestProgress.ExecutionFailed || status == AccessRequestProgress.Completed ||
                    status == AccessRequestProgress.ValidationFailed)
                {
                    if (bulkRequest != null)
                    {
                        bulkRequest.ProgressStatus = status;
                        bulkRequest.LastModifiedBy = MasterConfigHelper.ServiceUser;
                        bulkRequest.LastModifiedDate = DateTime.Now;
                        bulkRequest.Save(MasterConfigHelper.ServiceUser);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, string.Format("Error while executing UpdateBulkRequestStatus, Bulk Request Id - {0}.", bulkRequestId), ex);
            }
            finally
            {
                childRequests?.Clear();
                childRequests = null;

                bulkRequest = null;
            }
        }

        bool ExecuteOnBoardingAndAdhocRequest(AccessRequest accessRequest, IdmsLogger idmsLogger)
        {
            EndPoint endpoint = EndPoint.GetById(accessRequest.EndpointSystemId.Value, true);
            var user = User.GetById(accessRequest.UserId);
            ProvisioningParamCollection fields = null;
            ConnectorBase connector = null;
            FormExecutorBase formExecutor = null;
            try
            {
                bool isProvisioningDefaultsExists = IsProvisioningDefaultsExists(accessRequest);

                if (!isProvisioningDefaultsExists)
                {
                    var exceptionMessage = string.Format("Validation Failed for the Access Request Id:{0}in the endpoint - {1}", accessRequest.Id, endpoint.DisplayName);
                    idmsLogger.WriteLog(LogLevelType.Exception, exceptionMessage, null);
                    throw new FormExecutorValidationFailedException(exceptionMessage);
                }

                fields = NProvisioningField.GetProvisioningDefaults(endpoint.Id, accessRequest.AccountType);

                
                bool isConnectionExists = CheckEndpointConnection(endpoint, accessRequest, out connector);
                if (!isConnectionExists)
                {
                    UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ExecutionFailed);
                    if (accessRequest.BulkRequestId.HasValue && accessRequest.BulkRequestId.Value > 0)
                    {
                        UpdateBulkRequestStatus(accessRequest.BulkRequestId.Value);
                    }
                    idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Connection not established to the endpoint - {0} AccessRequestId:{1}", endpoint.DisplayName, accessRequest.Id), null);
                    idmsLogger.WriteLog(LogLevelType.Error, "Error occurred in execution of Accessrequest, ID: " + accessRequest.Id, null);
                    return false;
                }

                formExecutor = FormExecutorFactory.Get(endpoint, accessRequest, idmsLogger);
                // formExecutor.EndpointConnector = connector;
                bool isExecutedSuccessfully = false;

                

                if ( (!canCheckUserActive || isUserActive(accessRequest, idmsLogger) ) &&
                     canExecuteAmendOrDelete(endpoint, accessRequest, idmsLogger))
                {
                    isExecutedSuccessfully = formExecutor.Execute(fields, user);
                }

                if (isExecutedSuccessfully)
                {
                    var accessReq = AccessRequest.GetByIdIncludeAdHocAccessRequest(accessRequest.Id);
                    if ((accessRequest.ApplicationType == ApplicationType.Office365) && (accessRequest.RequestSource == AccessRequestSource.OnBoarding || (accessRequest.AdHocAccessRequest != null && accessRequest.AdHocAccessRequest.AccountType == AccountType.User && accessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount)))
                    {
                        AccessRequestProgress status = AccessRequestProgress.WaitingForSync;
                        accessRequest.ExecutionTime = DateTime.Now.AddHours(MasterConfigHelper.O365ADSyncWaitTime);
                        if (accessRequest.ApplicationType == ApplicationType.Office365)
                        {
                            accessRequest.AdditionalInfo = Constants.RequestAdditionalInfoLicense;
                            status = AccessRequestProgress.WaitingForLicenseAssignment;
                        }
                        UpdateAccessRequestStatus(user, accessRequest, status);
                    }
                    else
                    {
                        UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.Completed);
                        if (accessReq.BulkRequestId.HasValue && accessReq.BulkRequestId.Value > 0)
                        {
                            UpdateBulkRequestStatus(accessReq.BulkRequestId.Value);
                        }
                        idmsLogger.WriteLog(LogLevelType.Info, "AccessRequest Completed successfully !, ID: " + accessRequest.Id, null);
                    }
                }
                else
                {
                    UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ExecutionFailed);
                    if (accessRequest.BulkRequestId.HasValue && accessRequest.BulkRequestId.Value > 0)
                    {
                        UpdateBulkRequestStatus(accessRequest.BulkRequestId.Value);
                    }
                    idmsLogger.WriteLog(LogLevelType.Info, "Error occurred in execution of Accessrequest, ID: " + accessRequest.Id, null);
                }
                if (endpoint.EndpointSystemTypeId == Convert.ToInt64(EndpointType.SAP) && connector != null)
                {

                    connector.Disconnect();
                }

                return isExecutedSuccessfully;
            }
            catch(FormExecutorAlreadyDeletedAccountException alreadyDeleted)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, $"Account is already InActive/Deleted. Hence the current request status is set to successful.", alreadyDeleted);
                UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ExecutionFailed, alreadyDeleted.Message);
                if (accessRequest.BulkRequestId.HasValue && accessRequest.BulkRequestId.Value > 0)
                {
                    UpdateBulkRequestStatus(accessRequest.BulkRequestId.Value);
                }
                return true;
            }
            catch (FormExecutorInActiveUserException feInActiveUser)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Found inactive User while executing access request - {0}.", accessRequest?.Id), feInActiveUser);
                UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.Withdrawn, feInActiveUser.Message);
                if (accessRequest.BulkRequestId.HasValue && accessRequest.BulkRequestId.Value > 0)
                {
                    UpdateBulkRequestStatus(accessRequest.BulkRequestId.Value);
                }
                throw feInActiveUser;
            }
            catch (FormExecutorValidationFailedException fev)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Validation Failed while executing access request - {0}.", accessRequest?.Id), fev);
                UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ValidationFailed);
                if (accessRequest.BulkRequestId.HasValue && accessRequest.BulkRequestId.Value > 0)
                {
                    UpdateBulkRequestStatus(accessRequest.BulkRequestId.Value);
                }
                throw fev;
            }
            catch (Exception ex)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while executing access request - {0}.", accessRequest?.Id), ex);
                UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ExecutionFailed);
                if (accessRequest.BulkRequestId.HasValue && accessRequest.BulkRequestId.Value > 0)
                {
                    UpdateBulkRequestStatus(accessRequest.BulkRequestId.Value);
                }
                return false;
            }
            finally
            {
                connector = null;
                fields = null;
                formExecutor = null;
            }
        }


        bool canExecuteAmendOrDelete(EndPoint endPoint, AccessRequest accessRequest, IdmsLogger idmsLogger)
        {
            bool canDo = true;

            var adhocRequest = accessRequest.AdHocAccessRequest;

            if (adhocRequest == null || adhocRequest.AdhocActionType == AdHocActionType.NewAccount) return canDo;

            var entitlementDetails = adhocRequest.GetEntitlementDetail();
            long? endpointUserId = entitlementDetails?.EndpointUserId;
            var accountName = entitlementDetails?.DisplayAccountName;
            var applicationType = accessRequest.ApplicationType;

            if (!endpointUserId.HasValue || endpointUserId == 0) return canDo;

            var accountRecon = AccountReconcilation.GetActiveByEndpointIdAndEndpointUserId(endPoint.Id, endpointUserId.Value);

            if (accountRecon == null)
            {
                if (!string.IsNullOrWhiteSpace(accountName))
                {
                    accountRecon = AccountReconcilation.GetActiveByEndpointIdAndAccountName(endPoint.Id, accountName);
                    if (accountRecon != null && accountRecon.EndpointUserId > 0 && accountRecon.EndpointUserId != endpointUserId)
                    {
                        try
                        {
                            idmsLogger.WriteLog(LogLevelType.Warning, $"User:{accountName} - key:{accountRecon.EndpointUserId} for account:{accountRecon.AccountName} and old Key:{endpointUserId}");
                            if (applicationType == ApplicationType.EBS_CRM || applicationType == ApplicationType.EBS_ISU)
                            {
                                var adHocEBSRequest = (AdHocEBSRequest)adhocRequest.GetEntitlementDetail();
                                adHocEBSRequest.EndpointUserId = accountRecon.EndpointUserId;
                                adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(adHocEBSRequest);
                                adhocRequest.Save(MasterConfigHelper.ServiceUser);
                                return canDo;
                            }
                            else if (applicationType == ApplicationType.MSSL_CRM || applicationType == ApplicationType.MSSL_ISU)
                            {
                                var adHocMSSLRequest = (AdHocMSSLRequest)adhocRequest.GetEntitlementDetail();
                                adHocMSSLRequest.EndpointUserId = accountRecon.EndpointUserId;
                                adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(adHocMSSLRequest);
                                adhocRequest.Save(MasterConfigHelper.ServiceUser);
                                return canDo;
                            }
                            else if (applicationType == ApplicationType.ADSingaporePowerLocal || applicationType == ApplicationType.SAPLocal)
                            {
                                var adHocADRequest = (AdHocADRequest)adhocRequest.GetEntitlementDetail();
                                adHocADRequest.EndpointUserId = accountRecon.EndpointUserId;
                                adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(adHocADRequest);
                                adhocRequest.Save(MasterConfigHelper.ServiceUser);
                                return canDo;
                            }
                            else if (applicationType == ApplicationType.GEMS)
                            {
                                var adHocGemsRequest = (AdHocGEMSRequest)adhocRequest.GetEntitlementDetail();
                                adHocGemsRequest.EndpointUserId = accountRecon.EndpointUserId;
                                adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(adHocGemsRequest);
                                adhocRequest.Save(MasterConfigHelper.ServiceUser);
                                return canDo;
                            }
                            else if (applicationType == ApplicationType.OneCard)
                            {
                                var adHocSMSRequest = (AdHocSMSRequest)adhocRequest.GetEntitlementDetail();
                                adHocSMSRequest.EndpointUserId = accountRecon.EndpointUserId;
                                adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(adHocSMSRequest);
                                adhocRequest.Save(MasterConfigHelper.ServiceUser);
                                return canDo;
                            }
                            else if (applicationType == ApplicationType.FLINS_BI)
                            {
                                var adHocBIRequest = (AdHocSAPFLINSBIRequest)adhocRequest.GetEntitlementDetail();
                                adHocBIRequest.EndpointUserId = accountRecon.EndpointUserId;
                                adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(adHocBIRequest);
                                adhocRequest.Save(MasterConfigHelper.ServiceUser);
                                return canDo;
                            }
                            else if (applicationType == ApplicationType.HRMS_FLINS)
                            {
                                var adHocSAPHRFLINSRequest = (AdHocSAPHRFLINSRequest)adhocRequest.GetEntitlementDetail();
                                adHocSAPHRFLINSRequest.EndpointUserId = accountRecon.EndpointUserId;
                                adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(adHocSAPHRFLINSRequest);
                                adhocRequest.Save(MasterConfigHelper.ServiceUser);
                                return canDo;
                            }
                            else if (applicationType == ApplicationType.SSIDS_GUI || applicationType == ApplicationType.SSIDS_SMS)
                            {
                                var adHocSSIDSRequest = (AdHocSSIDSRequest)adhocRequest.GetEntitlementDetail();
                                adHocSSIDSRequest.EndpointUserId = accountRecon.EndpointUserId;
                                adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(adHocSSIDSRequest);
                                adhocRequest.Save(MasterConfigHelper.ServiceUser);
                                return canDo;
                            }
                            else if (applicationType == ApplicationType.SRM)
                            {
                                var adHocSAPSRMRequest = (AdHocSAPSRMRequest)adhocRequest.GetEntitlementDetail();
                                adHocSAPSRMRequest.EndpointUserId = accountRecon.EndpointUserId;
                                adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(adHocSAPSRMRequest);
                                adhocRequest.Save(MasterConfigHelper.ServiceUser);
                                return canDo;
                            }
                            else if (applicationType == ApplicationType.NPAC)
                            {
                                var adHocNPACRequest = (AdHocNPACRequest)adhocRequest.GetEntitlementDetail();
                                adHocNPACRequest.EndpointUserId = accountRecon.EndpointUserId;
                                adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(adHocNPACRequest);
                                adhocRequest.Save(MasterConfigHelper.ServiceUser);
                                return canDo;
                            }
                            else if (applicationType == ApplicationType.EAM)
                            {
                                var adHocNOvaRequest = (AdHocNovaRequest)adhocRequest.GetEntitlementDetail();
                                adHocNOvaRequest.EndpointUserId = accountRecon.EndpointUserId;
                                adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(adHocNOvaRequest);
                                adhocRequest.Save(MasterConfigHelper.ServiceUser);
                                return canDo;
                            }
                            else if (applicationType == ApplicationType.InhouseSystems)
                            {
                                var adhocInh = (AdHocINHRequest)adhocRequest.GetEntitlementDetail();
                                adhocInh.EndpointUserId = accountRecon.EndpointUserId;
                                adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(adhocInh);
                                adhocRequest.Save(MasterConfigHelper.ServiceUser);
                                return canDo;
                            }
                            if (applicationType == ApplicationType.Office365)
                            {
                                var adHocO365Request = (AdHocO365Request)adhocRequest.GetEntitlementDetail();
                                adHocO365Request.EndpointUserId = accountRecon.EndpointUserId;
                                adhocRequest.EntitlementDetail = adhocRequest.GetOffice365SerializedString(adHocO365Request);
                                adhocRequest.Save(MasterConfigHelper.ServiceUser);
                                return canDo;
                            }
                            if (applicationType == ApplicationType.EBS || applicationType == ApplicationType.MSSL)
                            {
                                var adHocHanaRequest = (AdHocSAPHANARequest)adhocRequest.GetEntitlementDetail();
                                adHocHanaRequest.EndpointUserId = accountRecon.EndpointUserId;
                                adhocRequest.EntitlementDetail = adhocRequest.GetSerializedString(adHocHanaRequest);
                                adhocRequest.Save(MasterConfigHelper.ServiceUser);
                                return canDo;
                            }
                        }
                        catch (Exception ex)
                        {
                            idmsLogger.WriteLog(LogLevelType.Warning, $"User:{accountName} key:{endpointUserId}", ex);
                        }
                    }
                }
                idmsLogger.WriteLog(LogLevelType.Exception, $"Access Request:{accessRequest.Id} -Cannot Perform Amendment/Delete action on Inactive account.");
                throw new FormExecutorAlreadyDeletedAccountException($"Access Request:{accessRequest.Id} - Cannot Perform Amendment/Delete action on Inactive account. The access request");
            }
            else
                return canDo;
        }

        bool isUserActive(AccessRequest accessRequest, IdmsLogger idmsLogger)
        {
            try
            {
                var applicationTypesExcluded = new[] { ApplicationType.VPN, ApplicationType.InhouseSystems };

                if (accessRequest == null || accessRequest.UserId <= 0) return false;

                if (accessRequest.EmployeeEventTriggerId.HasValue == true) return true;

                if (applicationTypesExcluded.Contains(accessRequest.ApplicationType)) return true;

                var usr = User.GetById(accessRequest.UserId);

                var activeUser = usr != null && usr.IsActive;


                if (activeUser)
                {
                    return true;
                }
                else
                {

                    var remarks = $"Since the user with PERNR-[{usr?.EmployeeId}] is inactive. IDMS cannot provision the access request ID :{accessRequest.Id} and set to withdrawn.";

                    idmsLogger.WriteLog(LogLevelType.Exception, remarks);

                    throw new FormExecutorInActiveUserException(remarks);
                }
            }
            catch (Exception e)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, $"Exception occured in checking active User in the   method isUserActive.");
                return true;
            }
        }

        bool ExecuteO365LicenseAssignmentRequest(AccessRequest accessRequest, EndPoint endpoint, IdmsLogger idmsLogger)
        {
            var user = User.GetById(accessRequest.UserId);
            ProvisioningParamCollection fields = null;
            try
            {
                fields = NProvisioningField.GetProvisioningDefaults(endpoint.Id, accessRequest.AccountType);
                FormExecutorBase formExecutor = FormExecutorFactory.Get(endpoint, accessRequest, idmsLogger);

                bool success = formExecutor.AssignO365License(user, fields);

                if (success)
                {
                    UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.Completed);
                }
                else
                {
                    UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ExecutionFailed);
                }
                return success;
            }
            catch (Exception ex)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while executing o365 license assignment access request - {0}.", accessRequest.Id), ex);
                UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ExecutionFailed);
                return false;
            }
            finally
            {
                fields = null;
            }
        }

        bool ExecuteO365MailBoxSettingsRequest(AccessRequest accessRequest, EndPoint endpoint, IdmsLogger idmsLogger)
        {
            var user = User.GetById(accessRequest.UserId);
            ProvisioningParamCollection fields = null;
            try
            {
                fields = NProvisioningField.GetProvisioningDefaults(endpoint.Id, accessRequest.AccountType);
                FormExecutorBase formExecutor = FormExecutorFactory.Get(endpoint, accessRequest, idmsLogger);

                bool success = formExecutor.UpdateO365MailBoxSettings(user, fields);

                if (success)
                {
                    UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.Completed);
                }
                else
                {
                    UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ExecutionFailed);
                }
                return success;
            }
            catch (Exception ex)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while executing o365 update mailbox settings access request - {0}.", accessRequest.Id), ex);
                UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ExecutionFailed);
                return false;
            }
            finally
            {
                fields = null;
            }
        }

        bool ExecuteOffBoardingRequest(AccessRequest accessRequest, EndPoint endpoint, IdmsLogger idmsLogger)
        {
            var user = User.GetById(accessRequest.UserId);
            ProvisioningParamCollection fields = null;
            FormExecutorBase formExecutor = null;
            try
            {
                formExecutor = FormExecutorFactory.Get(endpoint, accessRequest, idmsLogger);
                fields = NProvisioningField.GetProvisioningDefaults(endpoint.Id, accessRequest.AccountType);
                bool success = formExecutor.OffBoard(user, fields);

                if (success)
                {
                    UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.Completed);
                    //SendEmailIfUserExistsAsApproverOrOwnerInAnyMatrix(accessRequest.UserId, accessRequest, endpoint);
                }
                else
                {
                    UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ExecutionFailed);
                }
                return success;
            }
            catch (Exception ex)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while executing off boarding access request - {0}.", accessRequest.Id), ex);
                UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ExecutionFailed);
                return false;
            }
            finally
            {
                formExecutor = null;
                fields = null;
            }
        }

        bool ExecuteExtensionRequest(AccessRequest accessRequest, EndPoint endpoint, IdmsLogger idmsLogger)
        {
            var user = User.GetById(accessRequest.UserId);
            ProvisioningParamCollection fields = null;
            FormExecutorBase formExecutor = null;
            try
            {
                formExecutor = FormExecutorFactory.Get(endpoint, accessRequest, idmsLogger);
                fields = NProvisioningField.GetProvisioningDefaults(endpoint.Id, accessRequest.AccountType);
                bool success = formExecutor.Extension(user, fields, accessRequest.AccountType);

                if (success)
                {
                    UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.Completed);
                }
                else
                {
                    UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ExecutionFailed);
                }
                return success;
            }
            catch (Exception ex)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while executing extension access request - {0}.", accessRequest.Id), ex);
                UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ExecutionFailed);
                return false;
            }
            finally
            {
                formExecutor = null;
                fields = null;
            }
        }

        bool ExecutePropagationRequest(AccessRequest accessRequest, EndPoint endpoint, IdmsLogger idmsLogger)
        {

            var user = User.GetById(accessRequest.UserId);
            ProvisioningParamCollection fields = null;
            FormExecutorBase formExecutor = null;
            try
            {
                formExecutor = FormExecutorFactory.Get(endpoint, accessRequest, idmsLogger);
                fields = NProvisioningField.GetProvisioningDefaults(endpoint.Id, accessRequest.AccountType);
                bool success = formExecutor.Propagate(user, fields);

                if (success)
                {
                    UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.Completed);
                }
                else
                {
                    UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ExecutionFailed);
                }
                return success;
            }
            catch (Exception ex)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while executing propagation access request - {0}.", accessRequest), ex);
                UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ExecutionFailed);
                return false;
            }
            finally
            {
                formExecutor = null;
                fields = null;
            }
        }

        bool ExecuteRemediationRequest(AccessRequest accessRequest, EndPoint endpoint, IdmsLogger logger)
        {

            var user = User.GetById(accessRequest.UserId);
            ProvisioningParamCollection fields = null;
            try
            {
                FormExecutorBase formExecutor = FormExecutorFactory.Get(endpoint, accessRequest, logger);
                fields = NProvisioningField.GetProvisioningDefaults(endpoint.Id, accessRequest.AccountType);
                bool isExecutedSuccessfully = formExecutor.RemediateAttestation(user, fields);

                if (isExecutedSuccessfully)
                {
                    UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.Completed);
                    DILog.ProvisioningLogger.WriteLog(accessRequest.Id, endpoint.Id, LogLevelType.Info, "AccessRequest Completed successfully !, ID: " + accessRequest.Id);
                }
                else
                {
                    UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ExecutionFailed);
                    DILog.ProvisioningLogger.WriteLog(accessRequest.Id, endpoint.Id, LogLevelType.Info, "Error occur in excution of Accessrequest, ID: " + accessRequest.Id);
                }

                return isExecutedSuccessfully;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(accessRequest.Id, endpoint.Id, LogLevelType.Exception, string.Format("Error while executing access request - {0}.", accessRequest), ex);
                UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ExecutionFailed);
                return false;
            }
            finally
            {
                fields = null;
            }
        }

        public void SendEmailIfUserExistsAsApproverOrOwnerInAnyMatrix(long userId, AccessRequest accessRequest, long endpointId)
        {
            List<UserTableInfo> listUserTableInfo = null;
            var lstCaptions = new List<string>();
            System.Data.DataTable data = null;
            try
            {
                var user = User.GetById(userId);
                //if (user.IsActive)
                {
                    listUserTableInfo = UserTableInfo.GetAll();
                    
                    foreach (var uInfo in listUserTableInfo)
                    {
                        try
                        {
                            var condition = string.Empty;
                            
                            var columnValue = user.EmployeeId;
                            if (uInfo.ColumnType == "UserId")
                            {
                                condition = $"{user.Id}";
                            }
                            else
                            {
                                condition = $"'{user.EmployeeId}'";
                            }
                            var query = string.Format(uInfo.Caption, condition);

                            try
                            {
                                data = UserTableInfo.GetDataTableByQuery(query);
                                if (data != null && data.Rows.Count > 0)
                                {
                                    var str = data.Rows[0][0]?.ToString();
                                    if (!string.IsNullOrWhiteSpace(str) && str != "NULL")
                                    {
                                        lstCaptions.Add(str);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, $"SendEmailIfUserExistsAsApproverOrOwnerInAnyMatrix:{query} failed");
                            }
                        }
                        catch (Exception ex)
                        {
                            DILog.ProvisioningLogger.WriteLog(null, endpointId, LogLevelType.Exception,  $"SendEmailIfUserExistsAsApproverOrOwnerInAnyMatrix error occured.{uInfo.Columns} {uInfo.TableName} ", ex);
                        }
                    }

                    if (lstCaptions.Count > 0)
                    {
                        //var caption = string.Join("<br/>", lstCaptions);
                        ProvisioningEmailHelper.SendEmailAfterOffBoardingTheApprover(user, lstCaptions,null);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, endpointId, LogLevelType.Exception, string.Format("Error while executing SendEmailIfUserExistsAsApproverOrOwnerInAnyMatrix() - {0}.", accessRequest.Id), ex);

            }
            finally
            {
                data = null;

                listUserTableInfo?.Clear();
                listUserTableInfo = null;

                lstCaptions?.Clear();
                lstCaptions = null;
            }
        }

        bool ExecuteOUChangeRequest(AccessRequest accessRequest, EndPoint endpoint, IdmsLogger idmsLogger)
        {
            ProvisioningParamCollection fields = null;
            var user = User.GetById(accessRequest.UserId);
            FormExecutorBase formExecutor = null;
            try
            {
                formExecutor = FormExecutorFactory.Get(endpoint, accessRequest, idmsLogger);
                fields = NProvisioningField.GetProvisioningDefaults(endpoint.Id, accessRequest.AccountType);
                bool success = formExecutor.Transfer(user, fields);

                if (success)
                {
                    UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.Completed);
                    //SendEmailIfUserExistsAsApproverOrOwnerInAnyMatrix(accessRequest.UserId, accessRequest, endpoint.Id);
                }
                else
                {
                    UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ExecutionFailed);
                }
                return success;
            }
            catch (Exception ex)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while executing transfer access request - {0}.", accessRequest.Id), ex);
                UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ExecutionFailed);
                return false;
            }
            finally
            {
                formExecutor = null;
                fields = null;
            }
        }

        bool ExecuteReOrgRequest(AccessRequest accessRequest, EndPoint endpoint, IdmsLogger idmsLogger)
        {

            var user = User.GetById(accessRequest.UserId);
            ProvisioningParamCollection fields = null;
            FormExecutorBase formExecutor = null;
            try
            {
                formExecutor = FormExecutorFactory.Get(endpoint, accessRequest, idmsLogger);
                if(formExecutor != null)
                {
                    fields = NProvisioningField.GetProvisioningDefaults(endpoint.Id, accessRequest.AccountType);
                    bool success = formExecutor.ReOrg(user, fields);

                    if (success)
                    {
                        UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.Completed);
                    }
                    else
                    {
                        UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ExecutionFailed);
                    }
                    return success;
                }
                else
                {
                    idmsLogger.WriteLog(LogLevelType.Exception, $"Possible reason, Account type is not supported by Endpoint. Please make sure Account type is configured correctly or not.Access RequestId - {accessRequest.Id} AccountType:{accessRequest.AccountType}.");
                    UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ExecutionFailed);
                    return false;
                }
            }
            catch (Exception ex)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while executing re-org access request - {0}.", accessRequest.Id), ex);
                UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ExecutionFailed);
                return false;
            }
            finally
            {
                formExecutor = null;
                fields = null;
            }
        }

        bool ExecuteUndoEventRequest(AccessRequest accessRequest, IdmsLogger idmsLogger)
        {
            EndPoint endpoint = null;
            var user = User.GetById(accessRequest.UserId);
            ProvisioningParamCollection fields = null;
            FormExecutorBase formExecutor = null;
            try
            {
                if (accessRequest.EndpointSystemId.HasValue)
                {
                    endpoint = EndPoint.GetById(accessRequest.EndpointSystemId.Value);
                }
                else if (accessRequest.EventUndoRequestId.HasValue)
                {
                    endpoint = EndPoint.GetById(accessRequest.EventUndoRequest.EndpointSystemId);
                }
                else
                {
                    endpoint = EndPoint.GetById(accessRequest.AdHocAccessRequest.EndpointSystemId.Value);
                }
                if(idmsLogger == null)
                    idmsLogger = new ProvisioningLogger(accessRequest.Id, endpoint.Id, endpoint.EndpointSystemTypeId, endpoint.DisplayName);

                formExecutor = FormExecutorFactory.Get(endpoint, accessRequest, idmsLogger);
                fields = NProvisioningField.GetProvisioningDefaults(endpoint.Id, accessRequest.AccountType);
                bool success = formExecutor.UndoEvent(user, fields);

                if (success)
                {
                    UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.Completed);
                }
                else
                {
                    UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ExecutionFailed);
                }
                return success;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(accessRequest.Id, endpoint.Id, LogLevelType.Exception, string.Format("Error while executing event undo access request - {0}.", accessRequest.Id), ex);
                UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ExecutionFailed);
                return false;
            }
            finally
            {
                formExecutor = null;
                fields = null;
            }
        }
        public void CreateRemoveAccessForExpiredAccessRequest()
        {
            try
            {
                List<AdHocAccessRequest> expiredAdocRequests = AdHocAccessRequest.GetExpiredAdocRequest();

                foreach (AdHocAccessRequest adHocAccessRequest in expiredAdocRequests)
                {
                    if (MasterConfigHelper.IsDeploymentInProgress)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"CreateRemoveAccessForExpiredAccessRequest - Deployment InProgress");
                        return;
                    }
                    ProcessRemoveAccessRequest(adHocAccessRequest);
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, "Error while create remove access request expired request", ex);
            }
        }
        void ProcessRemoveAccessRequest(AdHocAccessRequest adHocAccessRequest)
        {
            EndPointAutoExpiry endPointAutoExpirys = null;

            endPointAutoExpirys = EndPointAutoExpiry.GetByEndpointSystemTypeId((long)adHocAccessRequest.EndpointSystemType);

            if (endPointAutoExpirys == null)
                return;

            if ((endPointAutoExpirys.IsAccountAutoExpirySupported == true && adHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount) ||
                (endPointAutoExpirys.IsEntitlementAutoExpirySupported == true && adHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
               )
            {
                AccessRequest accessRequest = AccessRequest.GetByAdHocAccessRequestId(adHocAccessRequest.Id);
                //if (AdHocAccessRequest.GetExpiredRemoveAdhocRequest(adHocAccessRequest))
                {
                    SaveAutoRejectAdhocRequest(adHocAccessRequest, accessRequest);
                    adHocAccessRequest.IsÉxpiryHandled = true;
                    adHocAccessRequest.IsÉxpiryMailSend = true;
                    adHocAccessRequest.Save(MasterConfigHelper.ServiceUser);
                }

                if (accessRequest != null)
                {
                    //Get Extended requests
                    List<AccessRequest> extendedAccessRequests = AccessRequest.GetByParentRequestFoExtendedRequest((long)accessRequest.Id);

                    foreach (var rquestItem in extendedAccessRequests)
                    {
                        if (rquestItem == null || rquestItem.ProgressStatus == AccessRequestProgress.Completed) continue;

                        try
                        {
                            string comments = string.Format("Parent Access request {0} has been expired, so system automatically rejected", accessRequest.Id);
                            List<Task> extendedTask = Task.GetByRequestId(rquestItem.Id);
                            foreach (Task currentTask in extendedTask)
                            {

                                // Temp Fix : In Group Task - the User IACMAdmin is need to impliment with valid Scenario user
                                var criteria = PresentationWorkflowCriteriaBase.GetWorkFlowCriteria(AccessType.Revoke, "IACMAdmin", currentTask.Id, comments, MasterConfigHelper.ServiceUser);

                                if (criteria != null)
                                {
                                    ServiceAccessStrategy _serviceAccessStrategy = new ServiceAccessStrategy(criteria);
                                    bool status = _serviceAccessStrategy.AccessWorkflowService();
                                }

                                //new EmailComposer().Compose(EmailTemplate.AccessRequestRejected, (long)(currentTask.AccessRequestId));
                            }
                        }
                        catch (Exception ex)
                        {
                            DILog.ProvisioningLogger.WriteLog(accessRequest.Id, null, LogLevelType.Exception, "Error while revoke task for Extended expired request", ex);
                        }

                        //not needed at all, just in case
                        if (rquestItem.AdHocAccessRequest != null)
                        {
                            AdHocAccessRequest adhocRequest = rquestItem.AdHocAccessRequest;
                            adhocRequest.IsÉxpiryHandled = false;
                            adhocRequest.IsÉxpiryMailSend = false;
                            adhocRequest.Save(MasterConfigHelper.ServiceUser);
                        }
                    }
                }
            }
        }
        void SaveAutoRejectAdhocRequest(AdHocAccessRequest adHocAccessRequest, AccessRequest accessRequest)
        {
            AccessRequest accessRequestNew = new AccessRequest();
            AdHocAccessRequest adHocAccessRequestNew = new AdHocAccessRequest();
            accessRequestNew.Id = 0;
            accessRequestNew.RequesterId = accessRequest.RequesterId;
            accessRequestNew.UserId = accessRequest.UserId;
            accessRequestNew.IsParentRequest = false;
            accessRequestNew.ParentRequestId = accessRequest.Id;
            accessRequestNew.EndpointSystemId = accessRequest.EndpointSystemId;
            accessRequestNew.AccountType = accessRequest.AccountType;
            accessRequestNew.ApplicationType = accessRequest.ApplicationType;

            EndPointAutoExpiry endPointAutoExpiry = null;
            PresentationWorkflowCriteriaBase criteria = null;

            endPointAutoExpiry = EndPointAutoExpiry.GetByEndPointSystemId((long)adHocAccessRequest.EndpointSystemId);

            if (endPointAutoExpiry == null)
            {
                endPointAutoExpiry = EndPointAutoExpiry.GetByEndpointSystemTypeId((long)adHocAccessRequest.EndpointSystemType);
            }

            if (endPointAutoExpiry.IsAutoExpiryApprovalRequired)
            {
                accessRequestNew.RequestStatus = AccessRequestStatus.Granted;
                accessRequestNew.ProgressStatus = AccessRequestProgress.ReadytoExecute;
            }
            else
            {
                accessRequestNew.RequestStatus = AccessRequestStatus.Submitted;
                accessRequestNew.ProgressStatus = AccessRequestProgress.WaitingforApproval;
            }

            accessRequestNew.RequestSource = AccessRequestSource.AdhocRequest;
            accessRequestNew.ActivationDate = DateTime.Now;
            accessRequestNew.TransactionId = Guid.NewGuid();
            accessRequestNew.IsCurrentLevelApproved = false;
            accessRequestNew.Comments = "This Account / Amendment has been validity expired,so system automatically created the removal request";
            accessRequestNew.IsDependentRequest = false;
            //accessRequestNew.IsDependentRequest = false;

            adHocAccessRequestNew.RequesteeUserId = adHocAccessRequest.RequesteeUserId;
            adHocAccessRequestNew.RequesterUserId = adHocAccessRequest.RequesterUserId;
            adHocAccessRequestNew.EndpointSystemType = adHocAccessRequest.EndpointSystemType;
            adHocAccessRequestNew.EndpointSystemId = adHocAccessRequest.EndpointSystemId;
            adHocAccessRequestNew.AdhocActionType = adHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount ? AdHocActionType.DeleteAccount : AdHocActionType.AmendmentRemove;
            adHocAccessRequestNew.AccountType = adHocAccessRequest.AccountType;
            adHocAccessRequestNew.IsSelfRequest = adHocAccessRequest.IsSelfRequest;
            adHocAccessRequestNew.ValidFrom = DateTime.Now;
            adHocAccessRequestNew.Comments = "This Account / Amendment validity has been expired,so system automatically created this removal access request";
            adHocAccessRequestNew.EntitlementDetail = adHocAccessRequest.EntitlementDetail;

            var entitlementList = adHocAccessRequestNew.GetAccessRequestDetail();
            if (entitlementList != null && entitlementList.Count > 0)
            {
                entitlementList.ForEach(a => a.RemoveAccess = true);
                adHocAccessRequestNew.SetRequestedItems = entitlementList;
            }

            adHocAccessRequestNew.IsÉxpiryHandled = false;
            adHocAccessRequestNew.IsÉxpiryMailSend = false;
            adHocAccessRequestNew.Save(MasterConfigHelper.ServiceUser);
            accessRequestNew.AdHocAccessRequestId = adHocAccessRequestNew.Id;
            accessRequestNew.Save(MasterConfigHelper.ServiceUser);
        }

        bool CheckEndpointConnection(EndPoint endpoint, AccessRequest accessRequest, out ConnectorBase connector)
        {
            DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"CheckEndpointConnection, EndpointSystemId: {endpoint.Id}, EndpointType: {endpoint.EndpointSystemTypeId}, AccessRequestId: {accessRequest.Id}");
            bool isConnection = false;
            connector = null;
            if (endpoint == null)
                return isConnection;

            try
            {
                if (endpoint.ApplicationType == ApplicationType.SRM)
                    return true;

                connector = ConnectorFactory.GetConnector(endpoint, accessRequest, null);
                isConnection = connector.Connect();


                DILog.ProvisioningLogger.WriteLog(LogLevelType.Info, $"ConnectionStatus: {isConnection}");

                if (!isConnection)
                    return isConnection;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(accessRequest.Id, endpoint.Id, LogLevelType.Exception, string.Format("Connection not established to the endpoint - {0}", endpoint.DisplayName), ex);
                isConnection = false;
            }
            return isConnection;
        }
        bool Disconnect(EndPoint endpoint, AccessRequest accessRequest)
        {
            DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"Disconnect, EndpointSystemId: {endpoint.Id}, EndpointType: {endpoint.EndpointSystemTypeId}, AccessRequestId: {accessRequest.Id}");
            bool isConnection = false;
            if (endpoint == null)
                return isConnection;

            try
            {
                if (endpoint.ApplicationType == ApplicationType.SRM)
                    return true;

                ConnectorBase _connector = ConnectorFactory.GetConnector(endpoint, accessRequest, null);
                _connector.Disconnect();

                DILog.ProvisioningLogger.WriteLog(LogLevelType.Info, $"ConnectionStatus: {isConnection}");

                if (!isConnection)
                    return isConnection;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(accessRequest.Id, endpoint.Id, LogLevelType.Exception, string.Format("Connection not established to the endpoint - {0}", endpoint.DisplayName), ex);
                isConnection = false;
            }
            return isConnection;
        }
        void UpdateAccessRequestStatus(DomainModel.User user, AccessRequest accessRequest, AccessRequestProgress ARProgress, string Comments = null)
        {
            if (ARProgress == AccessRequestProgress.ExecutionFailed)
            {
                ProvisioningEmailHelper.AccessRequestFailureNotification(accessRequest, user);
            }

            if (!string.IsNullOrWhiteSpace(Comments))
            {
                accessRequest.Comments = Comments;
                accessRequest.Remarks = DateTime.Now.ToString();
            }
            accessRequest.ProgressStatus = ARProgress;
            accessRequest.LastModifiedBy = ProvisioningConstants.ProcessName;
            accessRequest.LastModifiedDate = DateTime.Now;
            accessRequest.Save(MasterConfigHelper.ServiceUser);
        }

        bool IsProvisioningDefaultsExists(AccessRequest accessRequest)
        {
            string message = string.Empty;
            if (accessRequest.AdHocAccessRequestId.HasValue)
            {
                if (accessRequest.AdHocAccessRequest.AccountType == AccountType.User && accessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    var user = User.GetById(accessRequest.UserId);
                    if (accessRequest.AdHocAccessRequest.ApplicationType == ApplicationType.ADSingaporePowerLocal)
                    {
                        message = ProvisioningValidator.IsADDefaultsExists(user);
                    }
                    //else if (adhocRequest.EntitlementDetailType == ApplicationType.OneCard)
                    //{
                    //    message = ProvisioningValidator.IsSMSDefaultsExists(user);
                    //}
                    else if (accessRequest.AdHocAccessRequest.ApplicationType == ApplicationType.HRMS_FLINS)
                    {
                        message = ProvisioningValidator.IsFLINSDefaultsExists(user);
                    }

                    if (!string.IsNullOrEmpty(message))
                    {
                        EmployeeEvents employeeEvent = EmployeeEvents.None;

                        if (accessRequest.RequestSource == AccessRequestSource.OnBoarding)
                        {
                            employeeEvent = EmployeeEvents.NewJoinee;
                        }
                        else if (accessRequest.RequestSource == AccessRequestSource.OUChange)
                        {
                            employeeEvent = EmployeeEvents.OUChange;
                        }
                        else if (accessRequest.RequestSource == AccessRequestSource.AdhocRequest)
                        {
                            employeeEvent = EmployeeEvents.AdhocRequest;
                        }

                        ProvisioningValidator.AlertITAdmin(user, message, employeeEvent);

                        accessRequest.Remarks = message;
                        UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ValidationFailed);

                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        bool CheckforExtendedRequestInProgress(long adhocRequestId)
        {
            bool IsExtendedRequestProgress = false;
            AccessRequest accessRequest = AccessRequest.GetByAdHocAccessRequestId(adhocRequestId);

            if (accessRequest != null && AccessRequest.CheckForExtendedRequestInProgress(accessRequest))
            {
                IsExtendedRequestProgress = true;
            }

            return IsExtendedRequestProgress;
        }

        public void UpdateADManagerAttribute()
        {
            List<AccountReconcilation> adAccountReconUsers = new List<AccountReconcilation>();
            IdmsLogger idmsLogger = null;
            try
            {

                bool canrunADManagerJob = MasterConfigHelper.CanRunUpdateADManagerJob;

                if (canrunADManagerJob)
                {
                    var endpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.ADSingaporePowerLocal);
                    adAccountReconUsers = AccountReconcilation.GetAllActiveByEndpointIdByAccountType(endpoint.Id, AccountType.User);
                    idmsLogger = new ProvisioningLogger(null, endpoint.Id, endpoint.EndpointSystemTypeId, endpoint.DisplayName);
                    ConnectorBase _connector = ConnectorFactory.GetConnector(endpoint,null, idmsLogger);

                    idmsLogger.WriteLog(LogLevelType.Info, $"Started updating AD Manager attribute. Total users count:{adAccountReconUsers?.Count}");

                    //var allDIMUsers = User.GetAllByEndPointId(endpoint.Id);
                    //var allADUsers = ADSUserInfo.GetAllEndPointSystemIdWithoutEagerLoad(endpoint.Id);
                    //var allSMSUsers = SMSUserInfo.GetAllByEndpointSystemId(endpoint.Id);
                    //var allSMSBadges = SMSBadgeInfo.GetAllByEndpointSystemId(endpoint.Id);

                    ADSUserInfo adsuser = null; SMSUserInfo smsUser = null;
                    User user = null;
                    foreach (var item in adAccountReconUsers)
                    {
                        if (MasterConfigHelper.IsDeploymentInProgress)
                        {
                            idmsLogger?.WriteLog(LogLevelType.Info, $"UpdateADManagerAttribute - Deployment InProgress");
                            return;
                        }
                        try
                        {
                            adsuser = ADSUserInfo.GetById(item.EndpointUserId);
                            user = User.GetById(item.UserId.Value);
                            //adsuser = allADUsers.Where(ad => ad.Id == item.EndpointUserId).FirstOrDefault();
                            //user = allDIMUsers.Where(usr => usr.Id == item.UserId.Value).FirstOrDefault();
                            if (user == null)
                                continue;

                            if (!string.IsNullOrEmpty(user.ROEmployeeId))
                            {
                                var ro = User.GetByEmployeeId(user.ROEmployeeId);

                                if (ro != null)
                                {
                                    var roAD = ADSUserInfo.GetActivePrimaryByDIUserId(ro.Id, endpoint.Id);

                                    if (roAD != null)
                                    {
                                        adsuser.manager = roAD.distinguishedName;
                                    }
                                    else
                                    {
                                        adsuser.manager = string.Empty;
                                    }
                                }
                                else
                                {
                                    adsuser.manager = string.Empty;
                                }
                            }
                            else
                            {
                                adsuser.manager = string.Empty;
                            }

                            if (user.FullTimeEmployee)
                            {
                                if (user.IsExecutiveEmployee)
                                {
                                    adsuser.otherFascimileTelephoneNumber = "Exec";
                                }
                                else
                                {
                                    adsuser.otherFascimileTelephoneNumber = "Non-Exec";
                                }
                                adsuser.extensionAttribute13 = ADSUserInfo.GetExtensionAttribute13(user);
                                //if (user.EEGroup == "0")
                                //{
                                //    adsuser.extensionAttribute13 = "VP&Above";
                                //}
                                //else
                                //{
                                //    adsuser.extensionAttribute13 = "Director&Below";
                                //}
                            }
                            else
                            {
                                adsuser.otherFascimileTelephoneNumber = "Non-Employees";
                            }

                            //update lenel badge id.                            
                            smsUser = SMSUserInfo.GetSMSUserByADID(adsuser.sAMAccountName);
                            //smsUser = allSMSUsers.Where(sms => sms.OPHONE.Equals(adsuser.sAMAccountName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                            if (smsUser != null)
                            {
                                // DILog.ProvisioningLogger.WriteLog(null, endpoint.Id, LogLevelType.Info, $"SMS User found for user - {adsuser.sAMAccountName}");
                                var smsuserBadges = SMSBadgeInfo.GetByBadgeUserId(smsUser.UserId);
                                //  var smsuserBadges = allSMSBadges.Where(badge => badge.UserId == smsUser.UserId && badge.STATUS == 1).FirstOrDefault();
                                if (smsuserBadges != null)
                                {
                                    adsuser.extensionAttribute7 = smsuserBadges.BadgeNumber.ToString();
                                }
                                else
                                {
                                    DILog.ProvisioningLogger.WriteLog(null, endpoint.Id, LogLevelType.Info, $"SMS User Badge  not found for user - {smsUser.OPHONE}");
                                }

                                //DILog.ProvisioningLogger.WriteLog(null, endpoint.Id, LogLevelType.Info, $"adsuser.extensionAttribute7 - {adsuser.extensionAttribute7}");
                            }
                            //else
                            //    DILog.ProvisioningLogger.WriteLog(null, endpoint.Id, LogLevelType.Info, $"SMS User not found for user - {adsuser.sAMAccountName}");

                            var org = NOrganisation.GetByOrgCode(user.SectionCode);

                            if (org != null)
                            {
                                var orgHead = NOrganisationHead.GetByOrganisationIdAndHead(org.Id, user.Id);
                                if (orgHead != 0)
                                {
                                    adsuser.extensionAttribute8 = NOrganisationScope.GetById(org.ScopeId.Value).Scope;
                                }
                            }

                            adsuser.extensionAttribute10 = user.OrgSection;
                            adsuser.extensionAttribute6 = user.CostCenter;
                            adsuser.extensionAttribute1 = user.OrgBranch;
                            adsuser.extensionAttribute2 = user.OrgUnit;

                            adsuser.otherPager = user.OrgUnit;
                            adsuser.businessCategory = user.OrgBranch;

                            adsuser.title = user.PositionName;
                            adsuser.department = user.OrgDepartment;
                            adsuser.division = user.OrgDivision;
                            adsuser.company = user.OrgCompany;
                            adsuser.businessCategory = user.OrgBranch;
                            adsuser.pager = user.OrgSection;


                            adsuser.postalAddress = user.PostalAddress;
                            adsuser.postalCode = user.PostalCode;
                            adsuser.countryCode = user.Country;

                            adsuser.telephoneNumber = user.TelephoneNumber;
                            adsuser.mobile = user.MobileNumber;
                            adsuser.facsimileTelephoneNumber = user.Fax;
                            adsuser.mail = user.EmailId;
                            adsuser.physicalDeliveryOfficeName = user.EmployeeId;

                            adsuser.LastModifiedBy = "System";
                            adsuser.LastModifiedDate = DateTime.Now;


                            if (_connector.UpdateADManagerAttribute(adsuser))
                            {
                                adsuser.Save("System");
                            }
                        }
                        catch (Exception ex)
                        {
                            idmsLogger?.WriteLog(LogLevelType.Exception, string.Format("Error while updating AD attributes for users - {0}", user.UserName), ex);
                        }

                    }

                    idmsLogger?.WriteLog(LogLevelType.DebugStep, $"Completed updating total {adAccountReconUsers.Count()} User's AD Manager attribute.", null);
                }
            }
            catch (Exception ex)
            {
                idmsLogger?.WriteLog(LogLevelType.Exception, "Error while updating manager for AD users.", ex);
            }
            finally
            {
                adAccountReconUsers?.Clear();
                adAccountReconUsers = null;
            }

        }

        public void NotifyNonEmployeeExpiration()
        {
            List<User> users = new List<User>();
            IdmsLogger idmsLogger = null;
            List<int> alertDays = null;
            try
            {
                alertDays = MasterConfigHelper.NonEmployeeNoticePeriod;
                users = User.GetExpiringNonEmployees();
                idmsLogger = new ProvisioningLogger(null, null, null,null);
                idmsLogger.WriteLog(LogLevelType.DebugStep, "Started sending non employee expiry notification.", null);

                foreach (User user in users)
                {
                    if (MasterConfigHelper.IsDeploymentInProgress)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"UpdateADManagerAttribute - Deployment InProgress");
                        return;
                    }
                    try
                    {
                        int expiryDays = user.TerminationDate.Value.Subtract(DateTime.Today).Days;
                        if (alertDays.Contains(expiryDays))
                        {
                            this.SendNonEmployeeExpiryNotification(user, expiryDays);
                        }
                    }
                    catch (Exception ex)
                    {
                        idmsLogger?.WriteLog(LogLevelType.Exception, $"Error while sending non employee expiry notitfication for the user {user.UserName}", ex);
                    }
                }
                idmsLogger?.WriteLog(LogLevelType.DebugStep, $"Completed sending non employee expiry notification. Total - {users.Count()}", null);
            }
            catch (Exception ex)
            {
                idmsLogger?.WriteLog(LogLevelType.Exception, "Error while sending non employee expiry notitfication.", ex);
            }
            finally
            {
                users?.Clear();
                users = null;

                alertDays?.Clear();
                alertDays = null;
            }
        }
        public void SendNonEmployeeExpiryNotification(User user, int expiryDays)
        {
            IList<EmailInfo> listMails = new List<EmailInfo>();
            EmailInfo mail = null;

            string strMailTo = string.Empty;
            try
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.DebugStep, "Non employee expiry notification for user : " + user.KnownAs);

                mail = new EmailInfo();

                string mailContent = string.Empty;
                string mailSubject = string.Empty;

                MailTemplate mailTemplate = MailTemplate.GetByTemplateName(Constants.NonEmployeeExpiryNotification);
                if (mailTemplate == null) return;

                mailContent = mailTemplate.MailContent;
                mailSubject = mailTemplate.MailSubject;

                User supervisor = User.GetROByUser(user.Id);

                if (supervisor != null)
                {
                    strMailTo = supervisor.EmailId;
                }

                if (string.IsNullOrEmpty(strMailTo))
                {
                    strMailTo = string.Join(",", MasterConfigHelper.AlertNotificationReceivers);
                }

                StringBuilder subject = new StringBuilder(mailSubject);
                subject.Replace("@@userDisplayName@@", user.DisplayName);
                subject.Replace("@@pernr@@", user.EmployeeId);

                StringBuilder bldrAD = new StringBuilder(mailContent);

                bldrAD.Replace("@@userDisplayName@@", user.DisplayName);
                bldrAD.Replace("@@pernr@@", user.EmployeeId);
                bldrAD.Replace("@@expiry_days@@", expiryDays.ToString());
                bldrAD.Replace("@@EMPTYPE@@", user.EmployeeType.ToString());
                bldrAD.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);
                bldrAD.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);

                if (supervisor != null)
                {
                    bldrAD.Replace("@@approver@@", supervisor.DisplayName);
                }
                else
                {
                    bldrAD.Replace("@@approver@@", "IT Security");
                }

                if (user.TerminationDate.HasValue)
                {
                    bldrAD.Replace("@@expiry_date@@", user.TerminationDate.Value.MailDateFormat());
                }
                else
                {
                    bldrAD.Replace("@@expiry_date@@", string.Empty);
                }
                mail.CreatedDate = DateTime.Now;
                mail.From = mailTemplate.MailFrom;
                mail.To = strMailTo;
                mail.Subject = subject.ToString();
                mail.Body = bldrAD.ToString();
                mail.IsHtml = true;
                listMails.Add(mail);
                EmailHandler.Send(listMails);
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Non employee expiry notification failed for user : " + user.KnownAs, ex);
            }
            finally
            {
                listMails?.Clear();
                listMails = null;
            }
        }
        public void ResetO365IDMSAccountPassword()
        {
            try
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, "Provisioning Job - ResetO365IDMSAccountPassword Started.");

                EndPoint o365Endpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.Office365);
                EndpointConfiguration o365EPConfig = EndpointConfiguration.GetByEndpointSystemId(o365Endpoint.Id);

                EndPoint sfbEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.MicrosoftTeams);
                EndpointConfiguration sfbEPConfig = EndpointConfiguration.GetByEndpointSystemId(o365Endpoint.Id);

                int pwdChangeThreshold = MasterConfigHelper.O365SvcAccountPwdChangeDuration;
                int daysAfterLastChange = DateTime.Now.Subtract(MasterConfigHelper.LastPwdChangeTime).Days;

                if (pwdChangeThreshold <= daysAfterLastChange)
                {

                    EndPoint adEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.ADSingaporePowerLocal);

                    var fields = NProvisioningField.GetProvisioningDefaults(adEndpoint.Id, AccountType.User);

                    string accountName = Configuration(EndpointKey.UserName, o365EPConfig).Split('@')[0];
                    string newPassword = this.ConstructPassword(adEndpoint.Id);
                    string oldPassword = RSAHelper.DecryptData(Configuration(EndpointKey.Password, o365EPConfig));

                    ConnectorBase adConnector = ConnectorFactory.GetConnector(adEndpoint, null);
                    adConnector.ChangePasswordUsingOldPassword(accountName, newPassword, oldPassword);

                    SetPasswordConfiguration(EndpointKey.Password, o365EPConfig, RSAHelper.EncryptData(newPassword));
                    Office365UserInfo.O365LastPwdChange(DateTime.Now.ToString("YYYY-MM-DD HH:MI:SS"));
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, "IDMS O365 service account user password changed successfully.", null);
                    //Update SFB Password as well
                    SetPasswordConfiguration(EndpointKey.Password, sfbEPConfig, RSAHelper.EncryptData(newPassword));
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, "IDMS SFB service account user password changed successfully.", null);

                    
                }

                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.DebugStep, "Provisioning Job - ResetO365IDMSAccountPassword Ended.");
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while resetting password for IDMS O365 service account.", ex);
            }
        }

        public void PrepareTelephoneDirectorySyncData()
        {
            try
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, "Provisioning Job - PrepareTelephoneDirectorySyncData Started.");

                var endpoint = EndPoint.GetTrustedSourceEndPoint();
                var hrConnector = ConnectorFactory.GetConnector(endpoint, null);

                hrConnector.SyncTelephoneDirectory();

                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, "Provisioning Job - PrepareTelephoneDirectorySyncData Ended.");
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while preparing data for telephone directory sync.", ex);
            }
        }
        private string Configuration(EndpointKey Key, EndpointConfiguration endpointConfiguration)
        {
            var config = endpointConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
            return config != null ? config.Value : "";
        }
        private void SetPasswordConfiguration(EndpointKey Key, EndpointConfiguration endpointConfiguration, string password)
        {
            var config = endpointConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
            if (config != null)
            {
                config.Value = password;
                config.Save("System");
            }
        }
        private string ConstructPassword(long endpointId)
        {
            var endpointPolicy = PasswordPolicyEndpoint.GetEndpointPasswordPolicy(endpointId);

            var policy = endpointPolicy.PasswordPolicy;

            string password = new PasswordResolver().ConstructGeneralUserPassword
                (
                    policy.PasswordLength.HasValue ? policy.PasswordLength.Value : 8,
                    policy.NoOfSpecialCharacters.HasValue ? policy.NoOfSpecialCharacters.Value : 0,
                    policy.NoOfLowerCaseAlphabets.HasValue ? policy.NoOfLowerCaseAlphabets.Value : 0,
                    policy.NoOfUpperCaseAlphabets.HasValue ? policy.NoOfUpperCaseAlphabets.Value : 0,
                    policy.NoOfNumericCharacters.HasValue ? policy.NoOfNumericCharacters.Value : 0
                );

            return password;
        }
        public void CreateHomeFolderForADUsers()
        {
            try
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.DebugStep, "Provisioning Misc Job - Create Home Folders for Users Started.");

                var adUsers = ADSUserInfo.GetHomeFolderAccessAssignmentFailedUsers();

                EndPoint ep = null;
                ConnectorBase ADConnector = null;

                foreach (var item in adUsers)
                {
                    try
                    {
                        ep = EndPoint.GetById(item.EndPointSystemId);
                        ADConnector = ConnectorFactory.GetConnector(ep, null);
                        ADConnector.AssignRightsToHomeFolder(item.sAMAccountName);

                        item.IsHomeFolderPermissionSetFailed = false;
                        item.Save("System");
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"AssignRightstoHomeFolder for User {item.sAMAccountName} Ended.");
                    }
                    catch (Exception ex)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, $"Error while assigning home folder for AD user {item.sAMAccountName}.", ex);
                    }
                }

                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.DebugStep, "Provisioning Misc Job - Create Home Folders for Users Started.");
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while assigning home folder for AD users.", ex);
            }
        }
        public void AssignEPGroup()
        {
            List<PresentationSAPUserDetails> sapUsers = new List<PresentationSAPUserDetails>();
            try
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.DebugStep, "Provisioning Misc Job - AssignEPGroup Started.");

                var flinsEndpoint = EndPoint.GetEndPointByApplicationTypeNonTrusted(ApplicationType.HRMS_FLINS);
                var sapepEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.SAP_EP);

                sapUsers = PresentationSAPUserDetails.GetSAPUsersToAssignEPGroup(flinsEndpoint.Id);

                ConnectorBase sapepConnector = ConnectorFactory.GetConnector(sapepEndpoint, null);
                SAPEPUserInfo sapepUser = null;
                foreach (var item in sapUsers)
                {
                    if (MasterConfigHelper.IsDeploymentInProgress)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"AssignEPGroup - Deployment InProgress");
                        return;
                    }
                    try
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.DebugStep, $"Trying AssignEPGroup For User {item.UserLogonName}.");

                        sapepUser = SAPEPUserInfo.GetByUserName(item.UserLogonName, sapepEndpoint.Id);

                        if (sapepUser != null)
                        {
                            var accountRecon = AccountReconcilation.GetMappedAccReconByEpuserId(sapepEndpoint.Id, sapepUser.Id);

                            if (accountRecon == null)
                            {
                                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"Account Recon not exists For User {item.UserLogonName}, or does not have mapped account");
                                continue;
                            }

                            var user = User.GetByIdIncludeBusinessRoleAndOrganisation(accountRecon.UserId.Value);

                            string groupsBasedOnOrg = ProvisioningConfigMatrixSAPOrg.GetSAPEPGroupBasedOnUserOrg(user);
                            //string groupsBasedOnOrg = "Y_PG_SPHCO";

                            if (!string.IsNullOrEmpty(groupsBasedOnOrg))
                            {
                                sapepUser.Groups = groupsBasedOnOrg.Split(',').Distinct().ToList();
                                sapepUser.Groups = sapepUser.Groups.Distinct().ToList();

                                groupsBasedOnOrg = string.Join(",", sapepUser.Groups);
                                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"Default Groups  {groupsBasedOnOrg}.");

                                bool assigned = sapepConnector.AddEntitlement(sapepUser, EntitlementType.Group);

                                if (assigned)
                                {

                                    SAPEPUserInfo.AddEntitlement(sapepUser.Id, EntitlementType.Group, groupsBasedOnOrg);
                                    SAPEPUserInfo.ExecuteQuery($"UPDATE DI_SAP_UserInfo SET NeedEPGroupAssignment=0 WHERE id = {item.Id} AND EndPointSystemId={flinsEndpoint.Id}");
                                }
                            }
                            else
                            {
                                DILog.ProvisioningLogger.WriteLog(null, sapepEndpoint.Id, LogLevelType.Warning, string.Format("No default SAPEP group found for the user {0} from FLINS matrix.", sapepUser.AccountName), null);
                                SAPEPUserInfo.ExecuteQuery($"UPDATE DI_SAP_UserInfo SET NeedEPGroupAssignment=0 WHERE id = {item.Id} AND EndPointSystemId={flinsEndpoint.Id}");
                            }
                        }
                        else
                        {
                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.DebugStep, $"EP User is not exists {item.UserLogonName}.");
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, string.Format("Exception in assigning EP group for User - {0}", item.UserLogonName), ex);
                    }
                }

                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.DebugStep, "Provisioning Misc Job - AssignEPGroup Ended.");
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while assigning EP group.", ex);
            }
            finally
            {
                sapUsers?.Clear();
                sapUsers = null;
            }
        }
        //[Obsolete]
        //public void SyncSkypeForBusinessUser()
        //{
        //    EndPoint endpoint = null;
        //    var accessRequests = new List<AccessRequest>();
        //    try
        //    {
        //        endpoint = EndPoint.GetByApplicationType(ApplicationType.MicrosoftTeams);
        //        if (endpoint == null || endpoint.IsActive == false) return;
                  
        //        DILog.ProvisioningLogger.WriteLog(null, endpoint.Id, LogLevelType.DebugStep, $"Provisioning Misc Job {endpoint.DisplayName} Apply Policy");
        //        accessRequests = AccessRequest.GetMSTeamRequestForApplyPolicy();
        //        if (accessRequests != null && accessRequests.Count > 0)
        //        {
        //            foreach (var accessRequest in accessRequests)
        //            {

        //                if (MasterConfigHelper.IsDeploymentInProgress)
        //                {
        //                    DILog.ProvisioningLogger.WriteLog(null, endpoint.Id, LogLevelType.Info, $"Sync {endpoint.DisplayName} User- Deployment InProgress");
        //                    return;
        //                }
        //                try
        //                {
        //                    DILog.ProvisioningLogger.WriteLog(accessRequest.Id, endpoint.Id, LogLevelType.Info, $"{endpoint.DisplayName} Applying Policy started");
                           
        //                    var idmsLogger = new ProvisioningLogger(accessRequest.Id, endpoint.Id, endpoint.EndpointSystemTypeId,endpoint.DisplayName);
        //                    var user = User.GetById(accessRequest.UserId);
        //                    FormExecutorBase formExecutor = FormExecutorFactory.Get(endpoint, accessRequest, idmsLogger);
        //                    var fields = NProvisioningField.GetProvisioningDefaults(endpoint.Id, accessRequest.AccountType);
        //                    bool accountExists = false;
        //                    bool isExecutedSuccessfully = formExecutor.ExecuteUserAccountPolicy(fields, user, accountExists);

        //                    if (isExecutedSuccessfully)
        //                    {
        //                        UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.Completed);
        //                        DILog.ProvisioningLogger.WriteLog(accessRequest.Id, endpoint.Id, LogLevelType.Info, "AccessRequest Completed successfully !, ID: " + accessRequest.Id, null);
        //                    }
        //                    else
        //                    {
        //                        UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ExecutionFailed);
        //                        DILog.ProvisioningLogger.WriteLog(accessRequest.Id, endpoint.Id, LogLevelType.Info, "Error occurred in execution of Accessrequest, ID: " + accessRequest.Id, null);
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    DILog.ProvisioningLogger.WriteLog(accessRequest?.Id, accessRequest?.EndpointSystemId, LogLevelType.Info, $"Provisioning Misc Job Apply Policy failed for the stack trace:{ex?.StackTrace}", ex);
        //                }
        //            }
        //        }
        //        DILog.ProvisioningLogger.WriteLog(null, endpoint.Id, LogLevelType.DebugStep, $"Provisioning Misc Job {endpoint.DisplayName} Apply Policy completed");
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.ProvisioningLogger.WriteLog(null, endpoint?.Id, LogLevelType.Exception, $"Error while Syncing {endpoint.DisplayName} NewAccount.", ex);
        //    }
        //    finally
        //    {
        //        accessRequests?.Clear();
        //        accessRequests = null;
        //    }
        //}
        public void AddEmailIdAndTelephoneNumberInHR()
        {
            var users = new List<PresentationUserDetails>();
            IdmsLogger idmsLogger = null;
            EndPoint endpoint = null;
            EndPoint adEndpoint = null;
            ConnectorBase hrConnector = null;
            ADSUserInfo adUser = null;
            EndPoint sfbEndpoint = null;
            SkypeForBusinessUserInfo sfbUser = null;
            try
            {
                users = PresentationUserDetails.GetPresentationUsersToUpdateEmailIdInHR(false,true);
                endpoint = EndPoint.GetTrustedSourceEndPoint();
                adEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.ADSingaporePowerLocal);
                hrConnector = ConnectorFactory.GetConnector(endpoint, null);
                idmsLogger = new ProvisioningLogger(null, endpoint?.Id, endpoint?.EndpointSystemTypeId, endpoint?.DisplayName);
                idmsLogger.WriteLog(LogLevelType.DebugStep, "Provisioning Misc Job - AddEmailIdInHR Started");
                foreach (var user in users)
                {
                    if (MasterConfigHelper.IsDeploymentInProgress)
                    {
                        idmsLogger.WriteLog(LogLevelType.Info, $"AddEmailIdAndTelephoneNumberInHR - Deployment InProgress");
                        return;
                    }
                    #region EmailId Update
                    try
                    {
                        idmsLogger.WriteLog(LogLevelType.Info, $"AddEmailIdInHR For User {user.DisplayName}.");

                        int employeeId;
                        bool isValidPernr = int.TryParse(user.EmployeeId, out employeeId);
                        if (isValidPernr)
                        {
                            adUser = ADSUserInfo.GetActivePrimaryByDIUserId(user.Id, adEndpoint.Id);

                            if (adUser != null)
                            {
                                bool isAdded = hrConnector.AddEmailIdInHR(user.EmployeeId, adUser.mail, adUser.CreatedDate);

                                if (isAdded)
                                {
                                    idmsLogger.WriteLog(LogLevelType.Info, $"Successfully Added  EmailId In HR  For User {user.DisplayName}.");
                                }
                            }
                        }
                        else
                        {
                            idmsLogger.WriteLog(LogLevelType.Info, $"Invalid Pernnr , AddEmailIdInHR Failed For User {user.DisplayName}.");
                        }
                    }
                    catch (Exception ex)
                    {
                        idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while updating emailid in HR system for the User -{0}", user.UserName), ex);
                    }
                    User.ExecuteQuery($"UPDATE DI_M_User SET NeedEmailUpdateToHR=0,LastModifiedBy='System',LastModifiedDate=GETDATE() WHERE Id = {user.Id}");
                    #endregion
                }
                
                var canUpdateInHRMS = MasterConfigHelper.UpdateTelehoneNumberInHRMS;

                if (canUpdateInHRMS)
                {
                    sfbEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.MicrosoftTeams);
                    var delimiter = new string[] { ";ext=" };
                    users = PresentationUserDetails.GetPresentationUsersToUpdateEmailIdInHR(true,false);
                    var query = string.Empty;
                    foreach (var user in users)
                    {
                        
                        #region Update TelephoneNumber
                        try
                        {
                            idmsLogger.WriteLog(LogLevelType.Info, $"AddTelephoneNumberInHR For UserName: {user.UserName} {user.DisplayName}.");

                            sfbUser = SkypeForBusinessUserInfo.GetPrimaryByDIUserIdForUpdateTelephoneNumber(user.Id, sfbEndpoint.Id);
                            if (sfbUser == null || string.IsNullOrWhiteSpace(sfbUser.AnyOneLineURI))
                            {
                                idmsLogger.WriteLog(LogLevelType.Info, $"Possibly MicrosoftTeams User not found or LineURI is empty for user:{user.DisplayName} && TelephoneNumber:{user.TelephoneNumber}");
                                User.ExecuteQuery($"UPDATE DI_M_User SET NeedTelephoneUpdateToHR=0,TelephoneNumber='',LastModifiedBy='System',LastModifiedDate=GETDATE() WHERE Id = {user.Id}");
                                continue;
                            }

                            query = $"UPDATE DI_M_User SET NeedTelephoneUpdateToHR=0, LastModifiedBy='System',LastModifiedDate=GETDATE() WHERE Id = {user.Id}";

                            var lineURI = sfbUser.AnyOneLineURI;

                            if (!sfbUser.LineURIExtension.HasValue || sfbUser.LineURIExtension.Value == 0)
                            {
                                User.ExecuteQuery(query);
                                idmsLogger.WriteLog(LogLevelType.Info, $"AddTelephoneNumberInHR  failed:Extension is empty  For User {user.UserName}.");
                                continue;
                            }

                            if (lineURI.IndexOf("tel:") == 0)
                            {
                                lineURI = lineURI.Replace("tel:", "").Trim();
                            }
                            var numbers = lineURI.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                            if (numbers != null && numbers.Length > 0)
                            {
                                var temp = numbers[0];
                                if (!string.IsNullOrWhiteSpace(temp))
                                {
                                    lineURI = temp;
                                    idmsLogger.WriteLog(LogLevelType.DebugStep, $"AddTelephoneNumberInHR  TelephoneNumber is {lineURI}  For User {user.UserName}.");
                                    if (user.FullTimeEmployee)
                                    {
                                        if(hrConnector.AddTelephoneInHR(user.EmployeeId, lineURI, sfbUser.LineURIExtension.ToString(), user.JoinDate.Value))
                                        {
                                            idmsLogger.WriteLog(LogLevelType.Info, $"Successfully Added  TelephoneNumber In HR  For User {user.UserName}.");
                                        }
                                        User.ExecuteQuery(query);
                                    }
                                    else
                                    {
                                               User.ExecuteQuery($"UPDATE DI_M_User SET TelephoneNumber={{0}}, NeedTelephoneUpdateToHR=0, LastModifiedBy='System',LastModifiedDate=GETDATE() WHERE Id = {user.Id}", lineURI);
                                        idmsLogger.WriteLog(LogLevelType.Info, $"Successfully updated TelephoneNumber in Non-Employee profile for  User {user.UserName}.");
                                    }
                                }
                                else
                                {
                                    User.ExecuteQuery(query);
                                    idmsLogger.WriteLog(LogLevelType.DebugStep, $"AddEmailIdAndTelephoneNumberInHR   TelephoneNumber {lineURI} format is invalid For User {user.UserName}.");
                                    continue;
                                }
                            }
                            else
                            {
                                User.ExecuteQuery(query);
                                idmsLogger.WriteLog(LogLevelType.DebugStep, $"AddEmailIdAndTelephoneNumberInHR   TelephoneNumber {lineURI} format is invalid For User {user.UserName}.");
                                continue;
                            }
                        }
                        catch (Exception ex)
                        {
                            idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while updating Telephone Number in HR system for the User -{0}", user.UserName), ex);
                        }
                        #endregion
                    }
                }

                idmsLogger.WriteLog(LogLevelType.DebugStep, "Provisioning Misc Job - AddEmailIdInHR & AddTelephoneNumberInHR Ended");
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while updating emailid in HR system.", ex);
            }
            finally
            {
                sfbUser = null;
                adUser = null;
                sfbEndpoint = null;

                endpoint = null;
                adEndpoint = null;
                hrConnector = null;

                users?.Clear();
                users = null;
            }
        }
        public void AddmobileNumberinOneCard()
        {
            List<SMSUserInfo> onecardusers = new List<SMSUserInfo>();
            ConnectorBase onecardConnector = null;
            IdmsLogger idmsLogger = null;
            try
            {
                
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.DebugStep, "Provisioning Misc Job - AddmobileNumberinOneCard Started");

                bool canrunonecarduserJob = MasterConfigHelper.CanRunUpdateOneCardUsers;

                if (!canrunonecarduserJob) return;

                var onecardEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.OneCard);
                if (onecardEndpoint == null)
                    return;
                idmsLogger = new ProvisioningLogger(null, onecardEndpoint.Id, onecardEndpoint.EndpointSystemTypeId, onecardEndpoint.DisplayName);
                onecardConnector = ConnectorFactory.GetConnector(onecardEndpoint, null);
                //TODO
                onecardusers = SMSUserInfo.GetSMSUserInfoForUpdateMobileNumber(onecardEndpoint.Id);

                string usrMobileNumber = null;
                foreach (var smsUser in onecardusers)
                {
                    if (MasterConfigHelper.IsDeploymentInProgress)
                    {
                        idmsLogger.WriteLog(LogLevelType.Info, $"AddmobileNumberinOneCard - Deployment InProgress");
                        return;
                    }
                    try
                    {
                        if (string.IsNullOrWhiteSpace(smsUser.SSNO))continue;

                        usrMobileNumber = User.GetUserMobileNumber(smsUser.SSNO);

                        if (!string.IsNullOrWhiteSpace(usrMobileNumber) && string.IsNullOrWhiteSpace(smsUser.Extension))
                        {
                            if (!string.IsNullOrWhiteSpace(usrMobileNumber))
                            {
                                if (usrMobileNumber.Trim().Length > 8)
                                    smsUser.Extension = usrMobileNumber.Trim().Substring(usrMobileNumber.Trim().Length - 8, 8);
                                else
                                    smsUser.Extension = usrMobileNumber.Trim();
                            }
                            smsUser.LastModifiedBy = "System";
                            smsUser.LastModifiedDate = DateTime.Now;

                            bool isUpdated = onecardConnector.UpdateAccount(smsUser);

                            if (isUpdated)
                            {
                                smsUser.Save("System");
                                idmsLogger.WriteLog(LogLevelType.Info, string.Format("User {0} updated in OneCard.", smsUser.SSNO), null);
                            }
                            else
                            {
                                DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, string.Format("Failed to update User {0} in OneCard.", smsUser.SSNO));
                            }
                        }
                    }
                    catch (Exception ex1)
                    {
                        idmsLogger?.WriteLog(LogLevelType.Info, string.Format("Exception in update User {0} in OneCard.", smsUser.SSNO), ex1);
                    }
                }

                idmsLogger?.WriteLog(LogLevelType.DebugStep, "Provisioning Misc Job - AddmobileNumberinOneCard Ended");
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while updating Mobile Number AddmobileNumber in OneCard system.", ex);
            }
            finally
            {
                onecardusers?.Clear();
                onecardusers = null;
                onecardConnector = null;
            }
        }
        public void AddFLINSLoginIdInHR()
        {
            var users = new List<PresentationUserDetails>();
            IdmsLogger idmsLogger = null;
            EndPoint endpoint = null;
            EndPoint sapEndpoint = null; 
            try
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.DebugStep, "Provisioning Misc Job - AddFLINSLoginIdInHR Started");
                //var users = User.GetUsersToUpdateFLINSLoginIdInHR();
                users = PresentationUserDetails.GetPresentationUsersToUpdateEmailIdInHR(false, false,takeUpdateFLINSLoginIdInHR:true);
                endpoint = EndPoint.GetTrustedSourceEndPoint();
                sapEndpoint = EndPoint.GetEndPointByApplicationTypeNonTrusted(ApplicationType.HRMS_FLINS);
                var hrConnector = ConnectorFactory.GetConnector(endpoint, null);
                idmsLogger = new ProvisioningLogger(null, endpoint.Id, endpoint.EndpointSystemTypeId, endpoint.DisplayName);
                foreach (var user in users)
                {
                    if (MasterConfigHelper.IsDeploymentInProgress)
                    {
                        idmsLogger.WriteLog(LogLevelType.Info, $"AddFLINSLoginIdInHR - Deployment InProgress");
                        return;
                    }
                    try
                    {
                        int employeeId;
                        bool isValidPernr = int.TryParse(user.EmployeeId, out employeeId);
                        if (isValidPernr)
                        {
                            var sapUser = SAPUserinfo.GetPrimaryByDIUserId(user.Id, sapEndpoint.Id);

                            if (sapUser != null)
                            {
                                bool isAdded = hrConnector.AddFLINSLogonIdInHR(user.EmployeeId, sapUser.UserLogonName, sapUser.CreatedDate);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while updating FLINS login id in HR system for the User - {0}", user.UserName), ex);
                    }
                    User.ExecuteQuery($"UPDATE DI_M_User SET NeedFLINSLoginIdUpdateToHR=0,LastModifiedBy='System',LastModifiedDate=GETDATE() WHERE Id = {user.Id}");
                }

                idmsLogger?.WriteLog(LogLevelType.DebugStep, "Provisioning Misc Job - AddFLINSLoginIdInHR Ended");
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while updating FLINS login id in HR system.", ex);
            }
            finally
            {
                endpoint = null;
                sapEndpoint = null;
                users?.Clear();
                users = null;
            }
        }
        public void MassPropagateO365License()
        {
            List<Office365UserInfo> o365Users = null;
            Office365MassLicense request = null;
            try
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.DebugStep, "Provisioning Misc Job - MassPropagateO365License Started");

                request = Office365MassLicense.GetMassLicensePropagationRequest();

                if (request != null)
                {
                    var endpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.Office365);
                    var o365Connector = ConnectorFactory.GetConnector(endpoint, null);

                    o365Users = Office365UserInfo.GetUsersForLicenseAssignment(endpoint.Id);

                    foreach (Office365UserInfo o365Usr in o365Users)
                    {
                        try
                        {
                            o365Connector.DownloadUser(o365Usr.UserPrincipalName);
                            var o365User = Office365UserInfo.GetById(o365Usr.Id);

                            if (request.IsMassAdd)
                            {
                                try
                                {
                                    Dictionary<string, string> licenses = new Dictionary<string, string>();
                                    Dictionary<string, List<string>> exDisabledPlans = new Dictionary<string, List<string>>();

                                    if (o365User.ServicePlanObject != null)
                                    {
                                        List<string> existingLicenses = o365User.ServicePlanObject.PlanDetails.Select(item => item.AccountSkuId).Distinct().ToList();

                                        foreach (string lic in existingLicenses)
                                        {
                                            var plans = o365User.ServicePlanObject.PlanDetails.Where(item => item.AccountSkuId == lic && item.ProvisioningStatus == "Disabled").Select(item => item.ServiceName).ToList();
                                            exDisabledPlans.Add(lic, plans);
                                        }
                                    }

                                    foreach (ProvMassLicenseInfoDetail detail in request.LicenseInfoObject.LicenseDetails)
                                    {
                                        string key = detail.License;
                                        string value = string.Empty;

                                        List<string> disabledPlans = new List<string>();
                                        List<string> plansToAdd = detail.ServiceName.Split(',').ToList();

                                        if (exDisabledPlans.ContainsKey(key))
                                        {
                                            List<string> tmpDisabledPlans = exDisabledPlans[key];

                                            foreach (string plan in tmpDisabledPlans)
                                            {
                                                if (!plansToAdd.Contains(plan))
                                                {
                                                    disabledPlans.Add(plan);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var servicePlans = Office365ServicePlan.GetServicePlanByAccountSkuId(key);

                                            foreach (Office365ServicePlan plan in servicePlans)
                                            {
                                                if (!plansToAdd.Contains(plan.ServiceName))
                                                {
                                                    disabledPlans.Add(plan.ServiceName);
                                                }
                                            }
                                        }

                                        value = string.Join(",", disabledPlans);
                                        licenses.Add(key, value);
                                    }

                                    o365Connector.AssignLicense(o365User.UserPrincipalName, licenses);
                                    o365Connector.DownloadUser(o365User.UserPrincipalName);
                                }
                                catch (Exception ex)
                                {
                                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, string.Format("Error while assigning license/service plans for the user {0} as part of mass o365 license propagation.", o365User.UserPrincipalName), ex);
                                }
                            }
                            else
                            {
                                try
                                {
                                    Dictionary<string, string> licenses = new Dictionary<string, string>();
                                    Dictionary<string, List<string>> exDisabledPlans = new Dictionary<string, List<string>>();

                                    var o365Detail = request.LicenseInfoObject;

                                    List<string> licensesToRemove = o365Detail.LicenseDetails.Where(item => item.IsRemoveLicense).Select(item => item.License).ToList();

                                    var licensesRequestedToRemove = o365Detail.LicenseDetails;

                                    if (o365User.ServicePlanObject != null)
                                    {
                                        List<string> existingLicenses = o365User.ServicePlanObject.PlanDetails.Select(item => item.AccountSkuId).Distinct().ToList();

                                        foreach (string lic in existingLicenses)
                                        {
                                            var plans = o365User.ServicePlanObject.PlanDetails.Where(item => item.AccountSkuId == lic && item.ProvisioningStatus == "Disabled").Select(item => item.ServiceName).ToList();
                                            exDisabledPlans.Add(lic, plans);
                                        }
                                    }

                                    foreach (ProvMassLicenseInfoDetail detail in licensesRequestedToRemove)
                                    {
                                        string key = detail.License;
                                        string value = string.Empty;

                                        if (detail.ServiceName != null)
                                        {
                                            List<string> plansToRemove = detail.ServiceName.Split(',').ToList();

                                            if (exDisabledPlans.ContainsKey(key))
                                            {
                                                List<string> disabledPlans = exDisabledPlans[key];

                                                foreach (string plan in plansToRemove)
                                                {
                                                    if (!disabledPlans.Contains(plan))
                                                    {
                                                        disabledPlans.Add(plan);
                                                    }
                                                }
                                                value = string.Join(",", disabledPlans);
                                            }
                                        }

                                        licenses.Add(key, value);
                                    }

                                    o365Connector.RemoveLicense(o365User.UserPrincipalName, licenses, licensesToRemove);
                                    o365Connector.DownloadUser(o365User.UserPrincipalName);
                                }
                                catch (Exception ex)
                                {
                                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, string.Format("Error while removing license/service plans for the user {0} as part of mass o365 license propagation.", o365User.UserPrincipalName), ex);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, string.Format("Error while downloading information from O365 for the user {0} as part of mass o365 license propagation.", o365Usr.UserPrincipalName), ex);
                        }
                    }

                    request.IsCompleted = true;
                    request.LastModifiedBy = "System";
                    request.LastModifiedDate = DateTime.Now;

                    request.Save("System");
                }

                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.DebugStep, "Provisioning Misc Job - MassPropagateO365License Ended");
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while executing mass o365 license propagation.", ex);
            }
            finally
            {
                request = null;
                o365Users?.Clear();
                o365Users = null;
            }
        }

        public void AssignO365License()
        {
            EndPoint adEndpoint = null;
            EndPoint o365Endpoint = null;
            ProvisioningParamCollection fields = null;
            List<AccessRequest> pendingLicenseReqs = new List<AccessRequest>();
            try
            {
                o365Endpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.Office365);
                adEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.ADSingaporePowerLocal);
                pendingLicenseReqs = AccessRequest.GetAccessRequestByStatus(AccessRequestProgress.WaitingForLicenseAssignment, o365Endpoint.Id);
                fields = NProvisioningField.GetProvisioningDefaults(o365Endpoint.Id, AccountType.User);
                foreach (var accessRequest in pendingLicenseReqs)
                {
                    var idmsLogger = new ProvisioningLogger(accessRequest.Id, accessRequest.EndpointSystemId.Value, o365Endpoint.EndpointSystemTypeId, o365Endpoint.DisplayName);

                    if (MasterConfigHelper.IsDeploymentInProgress)
                    {
                        idmsLogger.WriteLog(LogLevelType.Info, $"Deployment InProgress");
                        return;
                    }
                    var user = User.GetById(accessRequest.UserId);
                    FormExecutorBase formExecutor = null;
                    try
                    {
                        if (string.IsNullOrWhiteSpace(accessRequest.Remarks))
                        {
                            idmsLogger.WriteLog(LogLevelType.Warning, $"AccountName not found in AccessRequest:{accessRequest.Id}");
                            continue;
                        }

                        var adUsr = ADSUserInfo.GetBysAMAccountNameEndPointSystemId(accessRequest.Remarks, adEndpoint.Id);

                        if (adUsr == null)
                        {
                            idmsLogger.WriteLog(LogLevelType.Warning, $"AD AccountName:{accessRequest.Remarks} not found for the AccessRequest:{accessRequest.Id}");
                            continue;
                        }

                        formExecutor = FormExecutorFactory.Get(o365Endpoint, accessRequest, idmsLogger);
                        if (formExecutor.IsUserExistsInOnline(accessRequest.Remarks))
                        {
                            //Commented below code because we are assigning License via ADGroups
                            //bool success = formExecutor.AssignO365License(user, fields);
                            bool success = true;
                            if (success)
                            {
                                //idmsLogger.WriteLog(accessRequest.Id, accessRequest.EndpointSystemId,LogLevelType.Warning, $"AssignO365License completed and change the status to WaitingForMailBoxCreation. AccessRequest:{accessRequest.Id}");
                                idmsLogger.WriteLog(accessRequest.Id, accessRequest.EndpointSystemId, LogLevelType.Warning, $"Online Account found and License Assigned. Hence changing status to WaitingForMailBoxCreation. AccessRequest:{accessRequest.Id}");
                                accessRequest.AdditionalInfo = Constants.RequestAdditionalInfoMailBox;
                                accessRequest.ExecutionTime = DateTime.Now.AddMinutes(10);
                                UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.WaitingForMailBoxCreation);
                            }
                            
                        }
                        else
                        {
                            UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ExecutionFailed);
                        }
                    }
                    catch (Exception ex)
                    {
                        idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while executing o365 license assignment access request - {0}.", accessRequest.Id), ex);
                        UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ExecutionFailed);
                        //return false;
                    }
                    finally
                    {
                        formExecutor = null;
                       
                    }
                }
                
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while creating O365 license assignment request.", ex);
            }
            finally
            {
                fields = null;
                adEndpoint = null;
                pendingLicenseReqs?.Clear();
                pendingLicenseReqs = null;
            }
        }
        public void CreateO365MailBox()
        {
            EndPoint adEndpoint = null;
            EndPoint o365Endpoint = null;
            ProvisioningParamCollection fields = null;
            List<AccessRequest> pendingLicenseReqs = new List<AccessRequest>();
            try
            {
                o365Endpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.Office365);
                adEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.ADSingaporePowerLocal);
                pendingLicenseReqs = AccessRequest.GetAccessRequestByStatus(AccessRequestProgress.WaitingForMailBoxCreation, o365Endpoint.Id);
                if (pendingLicenseReqs != null && pendingLicenseReqs.Count > 0)
                {
                    fields = NProvisioningField.GetProvisioningDefaults(o365Endpoint.Id, AccountType.User);
                    foreach (var accessRequest in pendingLicenseReqs)
                    {
                        var idmsLogger = new ProvisioningLogger(accessRequest.Id, accessRequest.EndpointSystemId.Value, o365Endpoint.EndpointSystemTypeId, o365Endpoint.DisplayName);
                        if (MasterConfigHelper.IsDeploymentInProgress)
                        {
                            idmsLogger.WriteLog(LogLevelType.Info, $"Deployment InProgress");
                            return;
                        }

                        var user = User.GetById(accessRequest.UserId);
                        FormExecutorBase formExecutor = null;
                        try
                        {

                            if (string.IsNullOrWhiteSpace(accessRequest.Remarks))
                            {
                                idmsLogger.WriteLog(accessRequest.Id, accessRequest.EndpointSystemId, LogLevelType.Warning, $"AccountName not found in AccessRequest:{accessRequest.Id}");
                                continue;
                            }
                            var adUsr = ADSUserInfo.GetBysAMAccountNameEndPointSystemId(accessRequest.Remarks, adEndpoint.Id);

                            if (adUsr == null)
                            {
                                idmsLogger.WriteLog(accessRequest.Id, accessRequest.EndpointSystemId, LogLevelType.Warning, $"AD AccountName:{accessRequest.Remarks} not found for the AccessRequest:{accessRequest.Id}");
                                continue;
                            }

                            formExecutor = FormExecutorFactory.Get(o365Endpoint, accessRequest, idmsLogger);
                            if (formExecutor.IsMailBoxExists(accessRequest.Remarks))
                            {
                                bool success = formExecutor.UpdateO365MailBoxSettings(user, fields);
                                if (success)
                                {
                                    idmsLogger.WriteLog(accessRequest.Id, accessRequest.EndpointSystemId, LogLevelType.Warning, $"UpdateO365MailBoxSettings completed");
                                    UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.Completed);
                                }
                                else
                                {
                                    UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ExecutionFailed);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            idmsLogger.WriteLog(accessRequest.Id, accessRequest.EndpointSystemId, LogLevelType.Exception, string.Format("Error while executing o365 MailBox Creation request - {0}.", accessRequest.Id), ex);
                            UpdateAccessRequestStatus(user, accessRequest, AccessRequestProgress.ExecutionFailed);
                            //return false;
                        }
                        finally
                        {
                            formExecutor = null;
                            fields = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while creating O365 O365MailBox request.", ex);
            }
            finally
            {
                pendingLicenseReqs?.Clear();
                pendingLicenseReqs = null;
                adEndpoint = null;
                fields = null;
            }
        }
        private string GetRandomString(int size)
        {
            // A=65, Z=90, a=97, z=122
            StringBuilder sb = new StringBuilder();
            Random random = new Random();

            char tmpCh;
            for (int tmpInt = 0; tmpInt < size; tmpInt++)
            {
                tmpCh = Convert.ToChar(random.Next(97, 122));
                sb.Append(tmpCh);
            }

            return sb.ToString();
        }

        private AccessRequest PopulateO365LicenseAssignmentRequest(User user, Guid transactionId, string sAMAccountName, EndPoint endpoint)
        {
            List<AccessRequest> requests = new List<AccessRequest>();
            AccessRequest newRequest = new AccessRequest();

            try
            {
                newRequest = new AccessRequest();

                newRequest.UserId = user.Id;

                newRequest.EndpointSystemId = endpoint.Id;
                newRequest.RequestStatus = AccessRequestStatus.Granted;
                newRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                newRequest.RequestSource = AccessRequestSource.OnBoarding;

                newRequest.RequesterId = null;
                newRequest.ActivationDate = user.JoinDate.HasValue ? user.JoinDate.Value : DateTime.Now;
                newRequest.DeactivationDate = user.TerminationDate;
                newRequest.Description = "This LicenseAssignment request is created automatically for On-Boarding/New Account creation.";
                newRequest.Remarks = sAMAccountName;
                newRequest.CreatedBy = "System";
                newRequest.CreatedDate = DateTime.Now;
                newRequest.TransactionId = transactionId;
                newRequest.ExecutionTime = DateTime.Now.AddHours(MasterConfigHelper.O365ADSyncWaitTime);
                newRequest.AccountType = AccountType.User;

                //requests.Add(newRequest);
                AccessRequest.AddAccessRequest(newRequest);


                return newRequest;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while saving O365 license assignment request.", ex);
                return newRequest;
            }
        }

        private bool PopulateSFBLicenseAssignmentRequest(User user, Guid transactionId,EndPoint endpoint, AccessRequest O365Request)
        {
            List<AccessRequest> requests = new List<AccessRequest>();
            AccessRequest newRequest = new AccessRequest();

            try
            {
                newRequest = new AccessRequest();

                newRequest.UserId = user.Id;

                newRequest.EndpointSystemId = endpoint.Id;
                newRequest.RequestStatus = AccessRequestStatus.Granted;
                newRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                newRequest.RequestSource = AccessRequestSource.OnBoarding;
                newRequest.ApplicationType = ApplicationType.MicrosoftTeams;
                newRequest.RequesterId = null;
                newRequest.ActivationDate = user.JoinDate.HasValue ? user.JoinDate.Value : DateTime.Now;
                newRequest.DeactivationDate = user.TerminationDate;
                newRequest.Description = "This request is created automatically for On-Boarding.";
                newRequest.CreatedBy = "System";
                newRequest.CreatedDate = DateTime.Now;
                newRequest.TransactionId = transactionId;
                newRequest.ExecutionTime = DateTime.Now.AddHours(MasterConfigHelper.O365ADSyncWaitTime);
                newRequest.AccountType = AccountType.User;
                if (O365Request != null)
                {
                    newRequest.IsParentRequest = true;
                    newRequest.ParentRequestId = O365Request.Id;
                }

                requests.Add(newRequest);
                AccessRequest.AddAccessRequest(requests);

                return true;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while saving Onboard SFB request.", ex);
                return false;
            }
        }

        /// <summary>
        /// Update OneCard BadgeNumber  in AD extension attribute7.
        /// </summary>
        public void UpdateOneCardBadgeNumberInADAttribute()
        {
            List<AccountReconcilation> adAccountReconUsers = new List<AccountReconcilation>();
            IdmsLogger idmsLogger = null;
            try
            {

                bool canrunADManagerJob = MasterConfigHelper.CanRunUpdateOneCardBadgeInADAttribute;

                if (canrunADManagerJob)
                {
                    var endpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.ADSingaporePowerLocal);
                    adAccountReconUsers = AccountReconcilation.GetAllActiveByEndpointIdByAccountType(endpoint.Id, AccountType.User);
                    idmsLogger = new ProvisioningLogger(null, endpoint.Id, endpoint.EndpointSystemTypeId, endpoint.DisplayName);
                    ConnectorBase _connector = ConnectorFactory.GetConnector(endpoint, null, idmsLogger);

                    idmsLogger.WriteLog(LogLevelType.DebugStep, "Started UpdateOneCardBadgeNumberInADAttribute.");

                    ADSUserInfo adsuser = null; SMSUserInfo smsUser = null;
                    User user = null;
                    foreach (var item in adAccountReconUsers)
                    {
                        if (MasterConfigHelper.IsDeploymentInProgress)
                        {
                            idmsLogger?.WriteLog(LogLevelType.Info, $"UpdateADManagerAttribute - Deployment InProgress");
                            return;
                        }
                        try
                        {
                            adsuser = ADSUserInfo.GetById(item.EndpointUserId);
                            user = User.GetById(item.UserId.Value);
                           
                            if (user == null)
                                continue;

                            //update lenel badge id.                            
                            smsUser = SMSUserInfo.GetSMSUserByADID(adsuser.sAMAccountName);
                            if (smsUser != null)
                            {
                                var smsuserBadges = SMSBadgeInfo.GetByBadgeUserId(smsUser.UserId);
                                if (smsuserBadges == null)
                                {
                                    DILog.ProvisioningLogger.WriteLog(null, endpoint.Id, LogLevelType.Info, $"SMS User Badge  not found for user - {smsUser.OPHONE}");
                                    continue;
                                }
                                if (!string.IsNullOrWhiteSpace(adsuser.extensionAttribute7) && smsuserBadges.BadgeNumber > 0 && adsuser.extensionAttribute7 == smsuserBadges.BadgeNumber.ToString())
                                {
                                    continue;
                                }
                                adsuser.extensionAttribute7 = smsuserBadges.BadgeNumber.ToString();
                            }

                            List<KeyValuePair<string, string>> keyValuePair = new List<KeyValuePair<string, string>>();
                            keyValuePair.Add(new KeyValuePair<string, string>("extensionAttribute7", adsuser.extensionAttribute7));
                            if (_connector.UpdateSpecificAttribute(adsuser, keyValuePair))
                            {
                                adsuser.Save("System");
                            }
                        }
                        catch (Exception ex)
                        {
                            idmsLogger?.WriteLog(LogLevelType.Exception, string.Format("Error while updating BageNumber in AD attributes for users - {0}", user.UserName), ex);
                        }
                    }
                    idmsLogger?.WriteLog(LogLevelType.DebugStep, $"Completed updating total {adAccountReconUsers.Count()} User's updating BageNumber in AD attributes.", null);
                }
            }
            catch (Exception ex)
            {
                idmsLogger?.WriteLog(LogLevelType.Exception, "Error while updating BageNumber in AD attributes.", ex);
            }
            finally
            {
                adAccountReconUsers?.Clear();
                adAccountReconUsers = null;
            }

        }
    }
}