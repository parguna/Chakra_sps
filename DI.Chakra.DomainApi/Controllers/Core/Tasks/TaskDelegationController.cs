using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.ProvisioningUtility;
using DI.Chakra.Services.EmailingProcessor;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{

    [RoutePrefix("TaskDelegation")]
    public class TaskDelegationController : BaseController<DomainModel.TaskDelegation, EntityModel.TaskDelegation, Dto.TaskDelegation>
    {

        [HttpPost]
        [Route("CreateSelectedTasksDelegation")]
        public void CreateSelectedTasksDelegation([FromBody]TaskDelegationWrapper taskDelegationWrapper)
        {
            DomainModel.TaskDelegation.CreateSelectedTasksDelegation(DtoUnMap(taskDelegationWrapper.Delegation), taskDelegationWrapper.SelectedtaskList,User.Identity.Name);

            var taskDelegationTo = DtoUnMap(taskDelegationWrapper.Delegation);
            long selectedTaskId = 0;

            try
            {
                if (taskDelegationWrapper.SelectedtaskList.Count > 0)
                {
                    foreach (var Selectedtask in taskDelegationWrapper.SelectedtaskList)
                    {
                        selectedTaskId = Selectedtask;
                        if (selectedTaskId > 0 && taskDelegationTo.IsActive)
                        {
                            var currentTask = DomainModel.Task.GetById(selectedTaskId);
                            if (currentTask != null && currentTask.IsDelegated)
                            {
                                DomainModel.EmailInfo emailInfo = null;
                                if (currentTask.RequestTypeId == Convert.ToInt16(RequestType.BulkRequest))
                                {
                                    var deepLoadedTask = DomainModel.Task.GetBulkRequestTask(currentTask.Id);
                                    emailInfo = DomainModel.EmailInfo.GetEmailInfoForSendingEmailForBulkRequest(deepLoadedTask);
                                    ProvisioningEmailHelper.GetEmailInfoForSendingEmailForBulkRequest(emailInfo);
                                }
                                else if (currentTask.RequestTypeId == Convert.ToInt16(RequestType.Attestation))
                                {
                                    var deepLoadedTask = DomainModel.Task.GetAttestationTask(currentTask.Id);
                                    emailInfo = DomainModel.EmailInfo.GetEmailInfoForDelegationTask(deepLoadedTask, taskDelegationTo.DelegationComments);
                                    ProvisioningEmailHelper.GetEmailInfoForSendingEmailForProvisioningRequest(emailInfo);
                                    //new EmailComposer().Compose(EmailTemplate.DelegateAttestationTask, task.Id);
                                    //TODO: During attestation this should be implement
                                    //ProvisioningEmailHelper.GetEmailInfoForSendingEmailForProvisioningRequest(emailInfo);
                                }
                                else if (currentTask.RequestTypeId == Convert.ToInt16(RequestType.EmployeeEvent) &&
                                    currentTask.EmployeeEventTriggerId.HasValue &&
                                    currentTask.EmployeeEventTriggerId > 0)
                                {
                                    var deepLoadedTask = DomainModel.Task.GetProvisioningTask(currentTask.Id);
                                    var employeeEvent = DomainModel.EmployeeEventTrigger.GetById(deepLoadedTask.EmployeeEventTriggerId.Value);
                                    emailInfo = DomainModel.EmailInfo.GetEmailInfoForSendEmailForEmployeeEventApproved(deepLoadedTask, employeeEvent);
                                    ProvisioningEmailHelper.GetEmailInfoForSendEmailForEmployeeEventApproved(emailInfo);
                                }
                                else if (currentTask.RequestTypeId == Convert.ToInt16(RequestType.Provisioning))
                                {
                                    var deepLoadedTask = DomainModel.Task.GetProvisioningTask(currentTask.Id);
                                    emailInfo = DomainModel.EmailInfo.GetEmailInfoForSendingEmailForProvisioningRequest(deepLoadedTask);
                                    ProvisioningEmailHelper.GetEmailInfoForSendingEmailForProvisioningRequest(emailInfo);
                                    //new EmailComposer().Compose(EmailTemplate.DelegateAttestationTask, task.Id);
                                }
                            }
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
            }
        }
       
        [HttpGet]
        [Route("GetByActionUser")]
        public List<TaskDelegation> GetByActionUser(long actionUser,Nullable<bool> isAdmin=null)
        {
            return DtoMapList(DomainModel.TaskDelegation.GetByActionUser(actionUser, isAdmin));
        }
        [HttpGet]
        [Route("IsDelegationAllowed")]
        public bool IsDelegationAllowed(RequestType requestType, long actionUser)
        {
            return DomainModel.TaskDelegation.IsDelegationAllowed(requestType, actionUser);
        }
        [HttpGet]
        [Route("GetAdvanceDelegationByActionUser")]
        public Dto.TaskDelegation GetAdvanceDelegationByActionUser(RequestType requestType, long actionUser, string startDate=null, string endDate = null)
        {
            DateTime? stDate = null;
            DateTime? edDate = null;
            if (!string.IsNullOrWhiteSpace(startDate))
            {
                stDate = DateTime.ParseExact(startDate, "dd-MM-yyyy 00:00:00.000", System.Globalization.CultureInfo.InvariantCulture);
                stDate = new DateTime(stDate.Value.Year, stDate.Value.Month, stDate.Value.Day, 00, 00, 01);
            }
            if (!string.IsNullOrWhiteSpace(endDate))
            {
                edDate = DateTime.ParseExact(endDate, "dd-MM-yyyy 23:59:59.999", System.Globalization.CultureInfo.InvariantCulture);
                edDate = new DateTime(edDate.Value.Year, edDate.Value.Month, edDate.Value.Day, 23, 59, 59);
            }
            return DtoMap(DomainModel.TaskDelegation.GetAdvanceDelegationByActionUser(requestType, actionUser, stDate, edDate));
        }
        [HttpPost]
        [Route("CreateAdvanceTasksDelegationEmail")]
        public void CreateAdvanceTasksDelegationEmail([FromBody]TaskDelegationWrapper taskDelegationWrapper)
        {
            try
            {
                var task = taskDelegationWrapper.Delegation;

                if (task != null && task.IsActive)
                {
                    task = DtoMap(DomainModel.TaskDelegation.GetByAllProperty(task.ActionUser, task.DelegatedTo,
                        task.StartDate.Value, task.EndDate.Value, task.DelegateAttestationTask, task.DelegateProvisioningTask,
                        task.DelegationComments));
                }

                if (task.IsActive && task.Id>0)
                {
                    var emailInfo = DomainModel.EmailInfo.GetEmailInfoForAdvanceDelegation(task);
                    ProvisioningEmailHelper.SendEmailForAdvanceDelegation(emailInfo);
                }                
            }
            catch (Exception ex)
            {

            }
        }
        [HttpGet]
        [Route("DeactivateExpiredAdvanceDelegation")]
        public string DeactivateAdvancedDelegation([FromBody]string modifiedBy)
        {
            return DomainModel.TaskDelegation.DeactivateAdvancedDelegation(modifiedBy);
        }
    }
}
