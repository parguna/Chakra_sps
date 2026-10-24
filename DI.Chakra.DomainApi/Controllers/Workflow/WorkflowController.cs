using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.ProvisioningUtility;
using DI.Chakra.Workflow.ServiceHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers.Workflow
{
    [RoutePrefix("WorkflowController")]
    public class WorkflowController : ApiController
    {
        [HttpPost]
        [Route("SubmitWorkflowCriteria")]
        public bool SubmitWorkflowCriteria([FromBody]Dto.Presentation.PresentationWorkflowCriteriaBase workflowCriteria)
        {
            bool status = false;
            DomainModel.Presentation.PresentationWorkflowCriteriaBase criteria =
                AutoMapperHelper.DtoUnMap<Dto.Presentation.PresentationWorkflowCriteriaBase, DomainModel.Presentation.PresentationWorkflowCriteriaBase>(workflowCriteria);
            ServiceAccessStrategy _serviceAccessStrategy = new ServiceAccessStrategy(criteria);
            status = _serviceAccessStrategy.AccessWorkflowService();
            if (workflowCriteria is Dto.Presentation.ApprovalActionCriteria)
            {
                if ((((Dto.Presentation.ApprovalActionCriteria)workflowCriteria).accessType == AccessType.Revoke) && ((Dto.Presentation.ApprovalActionCriteria)workflowCriteria).TaskId != 0)
                {
                    if (workflowCriteria.RequestType == RequestType.BulkRequest)
                    {
                        ProvisioningEmailHelper.SendMailOnBulkRequestRejection(((Dto.Presentation.ApprovalActionCriteria)workflowCriteria).TaskId);
                    }
                    if (workflowCriteria.RequestType == RequestType.Attestation)
                    {

                    }
                    else
                    {
                        ProvisioningEmailHelper.SendMailOnAccessRequestRejection(((Dto.Presentation.ApprovalActionCriteria)workflowCriteria).TaskId);
                    }

                    //new EmailComposer().Compose(EmailTemplate.AccessRequestRejected, (long)(((Dto.Presentation.ApprovalActionCriteria)workflowCriteria).AccesRequestId));
                }
            }

            return status;

        }

        [HttpPost]
        [Route("WithDrawnRequest")]
        public bool WithDrawnRequest([FromBody]Dto.Presentation.PresentationWorkflowCriteriaBase withdrawActionCriteria)
        {
            bool status = false;
            try
            {
                DomainModel.Presentation.PresentationWorkflowCriteriaBase criteria =
                AutoMapperHelper.DtoUnMap<Dto.Presentation.PresentationWorkflowCriteriaBase, DomainModel.Presentation.PresentationWorkflowCriteriaBase>(withdrawActionCriteria);
                ServiceAccessStrategy _serviceAccessStrategy = new ServiceAccessStrategy(criteria);
                status = _serviceAccessStrategy.AccessWorkflowService();
            }
            catch (Exception ex)
            {
            }

            //var wCriteria = withdrawActionCriteria as Dto.Presentation.WithdrawActionCriteria;
            //if (wCriteria != null)
            //{
            //    if (wCriteria.RequestType == RequestType.Provisioning)
            //    {
            //        if ((wCriteria.AccessRequest.ProgressStatus == AccessRequestProgress.WaitingforApproval) && wCriteria.AccessRequest.Id != null)
            //        {
            //            //The scope of the Request Withdrawn is Task
            //            long taskId = DomainModel.Task.GetTaskIdByAccessRequestId((long)(wCriteria.AccessRequest.Id));
            //            // new EmailComposer().Compose(EmailTemplate.RequestWithdrawn, (long)(((Dto.Presentation.WithdrawActionCriteria)withdrawActionCriteria).AccessRequest.Id));
            //            //new EmailComposer().Compose(EmailTemplate.RequestWithdrawn, taskId);
            //        }
            //    }
            //}
            return status;
        }

        [HttpPost]
        [Route("CanProceedToSecondStep")]
        public BooleanStatus CanProceedToSecondStep([FromBody]Dto.Presentation.CanProceedSecondStepCriteria criteriaParameters)
        {
            DomainModel.WorkflowConfiguration workFlow = null;
            BooleanStatus booleanStatus = new BooleanStatus() { CanProceed = true };
            AccountStatus accountStatus;
            try
            {
                workFlow = DomainModel.WorkflowConfiguration.GetByEndpointSystemId(
                    criteriaParameters.RequestTypeId,
                    criteriaParameters.EndpointSystemId,
                    Convert.ToString(Convert.ToByte(criteriaParameters.UserType)),
                    Convert.ToString(Convert.ToByte(criteriaParameters.AccountType)),
                    Convert.ToString(Convert.ToByte(criteriaParameters.ActionType)),
                    criteriaParameters.RequestDate);
            }
            catch (Exception ex)
            {
                return new BooleanStatus() { CanProceed = false, Message = "Exception occured in workflow selection. Hence you cannot proceed.Please contact IDMS Administrator." };
            }

            booleanStatus.CanProceed = workFlow != null;
            booleanStatus.Message = workFlow != null ? "" : "No WorkFlow defined for this request.Hence you cannot proceed.Please contact IDMS Administrator.";

            if (!booleanStatus.CanProceed) return booleanStatus;

            switch (criteriaParameters.AccountType)
            {
                case AccountType.User:

                    if (criteriaParameters.ActionType == AdHocActionType.NewAccount)
                    {
                        var isUserAccount = DomainModel.User.IsUserAccount(criteriaParameters.UserId, criteriaParameters.EndpointSystemId, (long)criteriaParameters.EndPointSystemType, criteriaParameters.AccountType);

                        if (isUserAccount)
                        {
                            booleanStatus.CanProceed = false;
                            booleanStatus.Message = "User cannot have more than one User account for this system.";
                            break;
                        }
                        ApplicationType[] applicationTypes = new[] { ApplicationType.SRM, ApplicationType.MicrosoftTeams };
                        if (criteriaParameters.ApplicationType == ApplicationType.Office365)
                        {
                            var user = DomainModel.User.GetById(criteriaParameters.UserId);
                            //if (user.FullTimeEmployee == false)
                            //{
                            //    //AD account should be created as part of approval.
                            //}
                            //else
                            //{

                            //}
                            applicationTypes = new[] { ApplicationType.SRM, ApplicationType.Office365, ApplicationType.MicrosoftTeams };
                        }
                        if (criteriaParameters.ApplicationType == ApplicationType.VPN)
                        {
                            applicationTypes = new[] { ApplicationType.SRM, ApplicationType.MicrosoftTeams,ApplicationType.VPN };
                        }
                        if (applicationTypes.Contains(criteriaParameters.ApplicationType))
                        {
                            if (criteriaParameters.ApplicationType == ApplicationType.SRM)
                            {

                                DomainModel.EndPoint ep_AD = DomainModel.EndPoint.GetEndPointByApplicationTypeNonTrusted(ApplicationType.ADSingaporePowerLocal);
                                var isUserAccountAvailableInAD = DomainModel.User.IsUserAccount(criteriaParameters.UserId, ep_AD.Id, ep_AD.EndpointSystemTypeId.HasValue ? ep_AD.EndpointSystemTypeId.Value : 0, criteriaParameters.AccountType);

                                DomainModel.EndPoint ep_FLINS = DomainModel.EndPoint.GetEndPointByApplicationTypeNonTrusted(ApplicationType.HRMS_FLINS);
                                var isUserAccountAvailableInFLINS = DomainModel.User.IsUserAccount(criteriaParameters.UserId, ep_FLINS.Id, ep_FLINS.EndpointSystemTypeId.HasValue ? ep_FLINS.EndpointSystemTypeId.Value : 0, criteriaParameters.AccountType);

                                if (!isUserAccountAvailableInAD || !isUserAccountAvailableInFLINS)
                                {
                                    List<string> endpoint = new List<string>();
                                    booleanStatus.CanProceed = false;

                                    if (!isUserAccountAvailableInAD)
                                        endpoint.Add("Singaporepower.Local AD");

                                    if (!isUserAccountAvailableInFLINS)
                                        endpoint.Add("FLINS");

                                    booleanStatus.Message = $"Cannot raise New Account request. User needs account in {string.Join(",", endpoint)}";
                                    break;
                                }
                            }
                            else
                            {
                                ApplicationType dependentApplicationType = (criteriaParameters.ApplicationType == ApplicationType.VPN || criteriaParameters.ApplicationType == ApplicationType.MicrosoftTeams || criteriaParameters.ApplicationType == ApplicationType.SRM || criteriaParameters.ApplicationType == ApplicationType.Office365) ? ApplicationType.ADSingaporePowerLocal : ApplicationType.HRMS_FLINS;
                                DomainModel.EndPoint ep = DomainModel.EndPoint.GetEndPointByApplicationTypeNonTrusted(dependentApplicationType);

                                var isUserAccountAvailable = DomainModel.User.IsUserAccount(criteriaParameters.UserId, ep.Id, ep.EndpointSystemTypeId.HasValue ? ep.EndpointSystemTypeId.Value : 0, criteriaParameters.AccountType);

                                if (!isUserAccountAvailable)
                                {
                                    booleanStatus.CanProceed = false;
                                    var s = dependentApplicationType == ApplicationType.ADSingaporePowerLocal ? "AD" : "SAP Flins";
                                    booleanStatus.Message = $"Cannot raise New Account request. User needs account in {s}.";
                                    break;
                                }
                            }
                        }
                        if (criteriaParameters.ApplicationType == ApplicationType.MicrosoftTeams)
                        {
                            DomainModel.EndPoint ep = DomainModel.EndPoint.GetEndPointByApplicationTypeNonTrusted(ApplicationType.Office365);
                            var isUserAccountAvailable = DomainModel.User.IsUserAccount(criteriaParameters.UserId, ep.Id, ep.EndpointSystemTypeId.HasValue ? ep.EndpointSystemTypeId.Value : 0, criteriaParameters.AccountType);

                            if (!isUserAccountAvailable)
                            {
                                booleanStatus.CanProceed = false;
                                booleanStatus.Message = $"Cannot raise New Account request. User needs account in Office365.";
                                break;
                            }
                        }

                        if (criteriaParameters.ApplicationType == ApplicationType.MSSL_CRM || criteriaParameters.ApplicationType == ApplicationType.MSSL_ISU)
                        {
                            ApplicationType dependentApplicationType = criteriaParameters.ApplicationType == ApplicationType.MSSL_CRM ? ApplicationType.MSSL_ISU : ApplicationType.MSSL_CRM;
                            DomainModel.EndPoint ep = DomainModel.EndPoint.GetByApplicationType(dependentApplicationType);
                            var isUserAccountAvailable = DomainModel.User.IsUserAccount(criteriaParameters.UserId, ep.Id, ep.EndpointSystemTypeId.HasValue ? ep.EndpointSystemTypeId.Value : 0, criteriaParameters.AccountType);

                            if (isUserAccountAvailable)
                            {
                                booleanStatus.CanProceed = false;
                                var s = dependentApplicationType == ApplicationType.MSSL_CRM ? "MSSL CRM" : "MSSL ISU";
                                booleanStatus.Message = $"User cannot have more than one User account for this system. User account exists in {s}";
                                break;
                            }
                        }

                    }
                    else if (criteriaParameters.ActionType == AdHocActionType.DeleteAccount)
                    {

                        List<long> endpointSystemIDs = new List<long> { criteriaParameters.EndpointSystemId };
                        int endpointType = (int)criteriaParameters.EndPointSystemType;
                        if (criteriaParameters.ApplicationType == ApplicationType.VPN)
                        {
                            DomainModel.EndPoint ep = DomainModel.EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                            endpointSystemIDs = new List<long> { ep.Id };
                            endpointType = (int)EndpointType.ActiveDirectory;
                        }

                        if (criteriaParameters.ApplicationType == ApplicationType.MSSL_CRM || criteriaParameters.ApplicationType == ApplicationType.MSSL_ISU)
                        {
                            DomainModel.EndPoint ep1 = DomainModel.EndPoint.GetByApplicationType(ApplicationType.MSSL_CRM);
                            DomainModel.EndPoint ep2 = DomainModel.EndPoint.GetByApplicationType(ApplicationType.MSSL_ISU);
                            endpointSystemIDs = new List<long> { ep1.Id, ep2.Id };
                        }

                        accountStatus = DomainModel.ADSUserInfo.IsAccountLockedByEndPointId(criteriaParameters.UserId, endpointSystemIDs, (int)criteriaParameters.AccountType, endpointType);

                        if (accountStatus.IsLocked)
                        {
                            booleanStatus.CanProceed = false;
                            booleanStatus.Message = "Account is deactivated.  Cannot raise an Delete request.";
                            break;
                        }
                        else if (!string.IsNullOrEmpty(accountStatus.Message))
                        {
                            booleanStatus.CanProceed = false;
                            booleanStatus.Message = "User does not hold account. Hence cannot raise an Delete request.";
                            break;
                        }

                        var accountRecons = DomainModel.AccountReconcilation.GetActiveAccountRecons(criteriaParameters.UserId, criteriaParameters.EndpointSystemId, AccountType.Secondary).ToList();

                        if (accountRecons.Count > 0)
                        {
                            booleanStatus.CanProceed = false;
                            booleanStatus.Message = "Active secondary account exists for user, hence user account cannot be deleted";
                            break;
                        }
                    }
                    else if (criteriaParameters.ActionType == AdHocActionType.AmendmentAdd || criteriaParameters.ActionType == AdHocActionType.AmendmentRemove)
                    {
                        List<long> endpointSystemIDs = new List<long> { criteriaParameters.EndpointSystemId };
                        int endpointType = (int)criteriaParameters.EndPointSystemType;

                        ApplicationType[] applicationTypes = new[] { ApplicationType.VPN, ApplicationType.MicrosoftTeams };
                        if (applicationTypes.Contains(criteriaParameters.ApplicationType))
                        {
                            DomainModel.EndPoint ep = DomainModel.EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                            endpointSystemIDs = new List<long> { ep.Id };
                            endpointType = (int)EndpointType.ActiveDirectory;
                        }

                        accountStatus = DomainModel.ADSUserInfo.IsAccountLockedByEndPointId(criteriaParameters.UserId, endpointSystemIDs, (int)criteriaParameters.AccountType, endpointType);

                        if (accountStatus.IsLocked)
                        {
                            booleanStatus.CanProceed = false;
                            booleanStatus.Message = "Account is deactivated.  Cannot raise an Amend request.";
                            break;
                        }
                        else if (!string.IsNullOrEmpty(accountStatus.Message))
                        {
                            booleanStatus.CanProceed = false;
                            if (applicationTypes.Contains(criteriaParameters.ApplicationType))
                                booleanStatus.Message = "User does not hold account in AD. Hence cannot raise an Amend request.";
                            else
                                booleanStatus.Message = "User does not hold account. Hence cannot raise an Amend request.";
                            break;
                        }
                        if (criteriaParameters.ApplicationType == ApplicationType.MicrosoftTeams || criteriaParameters.ApplicationType == ApplicationType.VPN)
                        {
                            var isUserAccount = DomainModel.User.IsUserAccount(criteriaParameters.UserId, criteriaParameters.EndpointSystemId, (long)criteriaParameters.EndPointSystemType, criteriaParameters.AccountType);
                            if (isUserAccount == false)
                            {
                                booleanStatus.CanProceed = false;
                                booleanStatus.Message = "User does not hold account. Hence cannot raise an Amend request.";
                                break;
                            }
                        }

                        if (criteriaParameters.ApplicationType == ApplicationType.MicrosoftTeams)
                        {
                            DomainModel.EndPoint ep = DomainModel.EndPoint.GetByApplicationType(ApplicationType.Office365);
                            endpointSystemIDs = new List<long> { ep.Id };
                            endpointType = (int)EndpointType.Office365;
                            accountStatus = DomainModel.ADSUserInfo.IsAccountLockedByEndPointId(criteriaParameters.UserId, endpointSystemIDs, (int)criteriaParameters.AccountType, endpointType);

                            if (accountStatus.IsLocked)
                            {
                                booleanStatus.CanProceed = false;
                                booleanStatus.Message = "Office365 Account is deactivated.  Cannot raise an Amend request.";
                                break;
                            }
                            else if (!string.IsNullOrEmpty(accountStatus.Message))
                            {
                                booleanStatus.CanProceed = false;
                                booleanStatus.Message = "User does not hold account in Office365. Hence cannot raise an Amend request.";
                                break;
                            }
                        }

                    }

                    break;

                case AccountType.Service:
                case AccountType.Secondary:
                case AccountType.Shared:
                    {
                        if (criteriaParameters.ActionType == AdHocActionType.NewAccount)
                        {
                            if (criteriaParameters.AccountType == AccountType.Secondary)
                            {
                                //var isUserAccount = DomainModel.User.IsUserAccount(criteriaParameters.UserId, criteriaParameters.EndpointSystemId, (long)criteriaParameters.EndPointSystemType, AccountType.User);

                                //if (!isUserAccount)
                                //{
                                //    booleanStatus.CanProceed = false;
                                //    booleanStatus.Message = "User does not have User account. Please raise a User account.";
                                //    break;
                                //}
                            }

                            //ApplicationType[] applicationTypes = new[] { ApplicationType.SRM };
                            //if (applicationTypes.Contains(criteriaParameters.ApplicationType))
                            //{
                            //    ApplicationType dependentApplicationType = (criteriaParameters.ApplicationType == ApplicationType.SRM  || criteriaParameters.ApplicationType == ApplicationType.GEMS) ? ApplicationType.ADSingaporePowerLocal : ApplicationType.HRMS_FLINS;
                            //    DomainModel.EndPoint ep = DomainModel.EndPoint.GetEndPointByApplicationTypeNonTrusted(dependentApplicationType);

                            //    var isUserAccountAvailable = DomainModel.User.IsUserAccount(criteriaParameters.UserId, ep.Id, ep.EndpointSystemTypeId.HasValue ? ep.EndpointSystemTypeId.Value : 0, criteriaParameters.AccountType);

                            //    if (!isUserAccountAvailable)
                            //    {
                            //        booleanStatus.CanProceed = false;
                            //        var s = dependentApplicationType == ApplicationType.ADSingaporePowerLocal ? "AD" : "SAP Flins";
                            //        booleanStatus.Message = $"Cannot raise New Account request. User needs account in {s}.";
                            //        break;
                            //    }
                            //}
                        }
                        else if (criteriaParameters.ActionType == AdHocActionType.DeleteAccount)
                        {
                            List<long> endpointSystemIDs = new List<long> { criteriaParameters.EndpointSystemId };
                            int endpointType = (int)criteriaParameters.EndPointSystemType;
                            if (criteriaParameters.ApplicationType == ApplicationType.VPN)
                            {
                                DomainModel.EndPoint ep = DomainModel.EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                                endpointSystemIDs = new List<long> { ep.Id };
                                endpointType = (int)EndpointType.ActiveDirectory;
                            }

                            if (criteriaParameters.ApplicationType == ApplicationType.MSSL_CRM || criteriaParameters.ApplicationType == ApplicationType.MSSL_ISU)
                            {
                                DomainModel.EndPoint ep1 = DomainModel.EndPoint.GetByApplicationType(ApplicationType.MSSL_CRM);
                                DomainModel.EndPoint ep2 = DomainModel.EndPoint.GetByApplicationType(ApplicationType.MSSL_ISU);
                                endpointSystemIDs = new List<long> { ep1.Id, ep2.Id };
                            }

                            accountStatus = DomainModel.ADSUserInfo.IsAccountLockedByEndPointId(criteriaParameters.UserId, endpointSystemIDs, (int)criteriaParameters.AccountType, endpointType);

                            if (accountStatus.IsLocked)
                            {
                                booleanStatus.CanProceed = false;
                                booleanStatus.Message = "Account is deactivated.  Cannot raise an Delete request.";
                                break;
                            }
                            else if (!string.IsNullOrEmpty(accountStatus.Message))
                            {
                                booleanStatus.CanProceed = false;
                                booleanStatus.Message = "User does not hold account. Hence cannot raise an Delete request.";
                                break;
                            }
                        }

                        else if (criteriaParameters.ActionType == AdHocActionType.AmendmentAdd || criteriaParameters.ActionType == AdHocActionType.AmendmentRemove)
                        {
                            List<long> endpointSystemIDs = new List<long> { criteriaParameters.EndpointSystemId };
                            int endpointType = (int)criteriaParameters.EndPointSystemType;

                            accountStatus = DomainModel.ADSUserInfo.IsAccountLockedByEndPointId(criteriaParameters.UserId, endpointSystemIDs, (int)criteriaParameters.AccountType, endpointType);

                            if (accountStatus.IsLocked)
                            {
                                booleanStatus.CanProceed = false;
                                booleanStatus.Message = "Account is deactivated.  Cannot raise an Amend request.";
                                break;
                            }
                            else if (!string.IsNullOrEmpty(accountStatus.Message))
                            {
                                booleanStatus.CanProceed = false;
                                booleanStatus.Message = "User does not hold account. Hence cannot raise an Amend request.";
                                break;
                            }

                        }
                    }
                    break;
            }


            //if (booleanStatus.CanProceed && (criteriaParameters.ActionType == AdHocActionType.NewAccount || criteriaParameters.ActionType == AdHocActionType.DeleteAccount ) && criteriaParameters.AccountType == AccountType.User)
            //{
            //    booleanStatus = DomainModel.AccessRequest.CheckForDuplicate(criteriaParameters.ApplicationType, criteriaParameters.EndpointSystemId, criteriaParameters.ActionType, criteriaParameters.AccountType, criteriaParameters.UserId);
            //}

            return booleanStatus;
        }

        [HttpPost]
        [Route("CanProceedToSecondStepBulkRequest")]
        public BooleanStatus CanProceedToSecondStepBulkRequest([FromBody]Dto.Presentation.CanProceedSecondStepCriteria criteriaParameters)
        {
            DomainModel.WorkflowConfiguration workFlow = null;
            BooleanStatus booleanStatus = new BooleanStatus() { CanProceed = true };
            try
            {
                workFlow = DomainModel.WorkflowConfiguration.GetByEndpointSystemId(
                    criteriaParameters.RequestTypeId,
                    criteriaParameters.EndpointSystemId,
                    Convert.ToString(Convert.ToByte(criteriaParameters.UserType)),
                    Convert.ToString(Convert.ToByte(criteriaParameters.AccountType)),
                    Convert.ToString(Convert.ToByte(criteriaParameters.ActionType)),
                    criteriaParameters.RequestDate);
            }
            catch (Exception ex)
            {
                return new BooleanStatus() { CanProceed = false, Message = "Exception occured in workflow selection. Hence you cannot proceed.Please contact IDMS Administrator." };
            }

            booleanStatus.CanProceed = workFlow != null;
            booleanStatus.Message = workFlow != null ? "" : "No WorkFlow defined for this request.Hence you cannot proceed.Please contact IDMS Administrator.";

            return booleanStatus;
        }
    }
}
