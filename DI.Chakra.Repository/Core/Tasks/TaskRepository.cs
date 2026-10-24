using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using DI.Chakra.Logger;
using System.Linq.Dynamic;
using System.Data.Entity.Core.Objects;

namespace DI.Chakra.Repository
{
    public class TaskRepository : RepositoryBase<Task>
    {
        public TaskRepository(DataAccessTransaction transaction) : base(transaction) { }
        
        public int GetResponseTime(long id)
        {
            return (from tasks in CurrentContext.Tasks
                    join attestattion in CurrentContext.Attestations
                        on tasks.AttestationId equals attestattion.Id
                    join wfConfig in CurrentContext.WorkflowConfigurations
                        on attestattion.WorkflowConfigurationId equals wfConfig.Id
                    from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                    where tasks.Id == id
                    select wfConfigDetail.ResponseTimeInDays).FirstOrDefault();
        }

        public int GetResponseTimeIncludingEndpoint(long id)
        {
            return (from tasks in CurrentContext.Tasks
                    join acsrqst in CurrentContext.AccessRequest
                    on tasks.AccessRequestId equals acsrqst.Id
                    join adhocrqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                    on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                    join rqsteduser in CurrentContext.User
                        on adhocrqst.RequesterUserId equals rqsteduser.Id into requester
                    from rqsteduser in requester.DefaultIfEmpty()
                    join wfConfig in CurrentContext.WorkflowConfigurations
                    on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                    from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                    where tasks.Id == id
                    select wfConfigDetail.ResponseTimeInDays).FirstOrDefault();
        }

        public IList<Task> GetTaskInfo(string attestationName)
        {
            return (from task in CurrentContext.Tasks
                    join attestation in CurrentContext.Attestations on task.AttestationId equals attestation.Id
                    where attestation.AttestationName == attestationName
                    select task).ToList();
        }

        public List<Task> GetExpiredTask()
        {
            return (from task in CurrentContext.Tasks
                    join accRequest in CurrentContext.AccessRequest on task.AccessRequestId equals accRequest.Id
                    join adhocReq in CurrentContext.AdHocAccessRequests on accRequest.AdHocAccessRequestId equals adhocReq.Id
                    let ValidToEnd = DbFunctions.CreateDateTime(adhocReq.ValidTo.Value.Year, adhocReq.ValidTo.Value.Month, adhocReq.ValidTo.Value.Day, 23, 59, 0)
                    where ValidToEnd < DateTime.Now && task.IsActive == true && task.IsCompleted == false && adhocReq.ValidTo != null &&
                          (adhocReq.AdhocActionType == AdHocActionType.NewAccount || adhocReq.AdhocActionType == AdHocActionType.AmendmentAdd)
                    select task).ToList();
        }

        private DateTime ConvertDateTimeToTicks(DateTime dtInput)
        {
            DateTime output = dtInput;
            DateTime a1 = output.AddHours(1).AddTicks(-1);
            return a1;
        }

        public int GetByAttestation(string attestationName)
        {
            return (from task in CurrentContext.Tasks
                    join attestation in CurrentContext.Attestations on task.AttestationId equals attestation.Id
                    where attestation.AttestationName == attestationName
                    select task).Count();
        }



        public List<Task> GetByCampaign(string campaignName)
        {
            return (from task in CurrentContext.Tasks
                    join campaign in CurrentContext.AttestationCampaigns.Include("Attestation") on task.AttestationCampaignId equals campaign.Id
                    where campaign.CampaignName == campaignName
                    select task).ToList();
        }
        public int GetCompletedTaskByAttestation(string attestationName)
        {
            return (from task in CurrentContext.Tasks
                    join attestation in CurrentContext.Attestations on task.AttestationId equals attestation.Id
                    where attestation.AttestationName == attestationName && task.IsCompleted == true
                    select task).Count();
        }
        public int GetAllCompletedTask()
        {
            return (from task in CurrentContext.Tasks
                    where task.IsGroupTask == true
                    select task).Count();
        }

        public List<PresentationPreviousApprovers> GetByWorkflowInstanceId(Guid workflowInstanceId, int workflowStateId)
        {
            int neededStateId = workflowStateId - 1;
            return (from task in CurrentContext.Tasks
                    join user in CurrentContext.User
                        on task.ActionUserId equals user.Id
                    where task.WorkflowInstanceId == workflowInstanceId && task.WorkflowStateId == (byte)neededStateId && task.IsCompleted
                    select new PresentationPreviousApprovers
                    {
                        ApprovedDate = task.LastModifiedDate.Value,
                        UserName = user.DisplayName
                    }).OrderByDescending(rvw => rvw.ApprovedDate).ToList();
        }
        public List<PresentationTask> GetPendingAttestationTasksByActionUserId(long actionUserId, DateTime fromDate)
        {
            return (from task in CurrentContext.Tasks
                    join user in CurrentContext.User
                      on task.ActionUserId equals user.Id
                    join attestattion in CurrentContext.Attestations
                        on task.AttestationId equals attestattion.Id
                    join wfConfig in CurrentContext.WorkflowConfigurations
                        on attestattion.WorkflowConfigurationId equals wfConfig.Id
                    from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).DefaultIfEmpty()
                        //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == task.WorkflowStateId
                    where task.CreatedDate >= fromDate &&
                          task.IsActive &&
                          task.IsCompleted == false &&
                          task.ActionUserId == actionUserId
                    select new PresentationTask
                    {
                        TaskId = task.Id,
                        RequestTypeId = task.RequestTypeId,
                        UserId = user.Id,
                        UserName = user.DisplayName,
                        AttestationId = task.AttestationId,
                        AttestationName = attestattion.AttestationName,
                        CreatedDate = task.CreatedDate,
                        IsActive = task.IsActive,
                        IsCompleted = task.IsCompleted,
                        ReponseTimeInDays = wfConfigDetail != null ? wfConfigDetail.ResponseTimeInDays : 1,
                        StartDate = null,
                        EndDate = null,
                        RequestedDate = task.CreatedDate,
                    }).ToList();
        }

        public List<PresentationTask> GetPendingProvisioningTasksByActionUserId(long actionUserId, DateTime fromDate)
        {
            return (from tsk in CurrentContext.Tasks
                    join acsrqst in CurrentContext.AccessRequest
                    on tsk.AccessRequestId equals acsrqst.Id
                    join adhocrqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                    on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                    join rqsteduser in CurrentContext.User
                    on adhocrqst.RequesterUserId equals rqsteduser.Id into requester
                    join wfConfig in CurrentContext.WorkflowConfigurations
                    on tsk.WorkflowConfigurationId equals wfConfig.Id
                    from rqsteduser in requester.DefaultIfEmpty()
                    from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == tsk.WorkflowStateId).DefaultIfEmpty()
                        //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == tsk.WorkflowStateId
                    where tsk.CreatedDate >= fromDate &&
                        tsk.IsActive &&
                        tsk.IsCompleted == false &&
                        tsk.ActionUserId == actionUserId
                    select new PresentationTask
                    {
                        TaskId = tsk.Id,
                        RequestTypeId = tsk.RequestTypeId,
                        AttestationId = tsk.AttestationId,
                        AttestationName = string.Empty,
                        EventName = adhocrqst.EndPoint.DisplayName + " - " + ((int)adhocrqst.AdhocActionType == 1 ? "Add New Account" : ((int)adhocrqst.AdhocActionType == 2) ? "Amendment Add" : ((int)adhocrqst.AdhocActionType == 3 ? "Amendment Remove" : ((int)adhocrqst.AdhocActionType == 4 ? "Delete Account" : "None"))),
                        CreatedDate = tsk.CreatedDate,
                        IsActive = tsk.IsActive,
                        IsCompleted = tsk.IsCompleted,
                        ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                        UserName = string.Empty,
                        AdHocAccessRequestId = adhocrqst.Id,
                        RequestedBy = rqsteduser != null ? rqsteduser.DisplayName : "System",
                        StartDate = adhocrqst.ValidFrom,
                        EndDate = adhocrqst.ValidTo,
                        RequestedDate = tsk.CreatedDate,
                    }).ToList();
        }

        public List<PresentationTask> GetPendingAttestationTasks(DateTime fromDate)
        {
            return (from task in CurrentContext.Tasks
                    join user in CurrentContext.User
                      on task.ActionUserId equals user.Id
                    join attestattion in CurrentContext.Attestations
                        on task.AttestationId equals attestattion.Id
                    join wfConfig in CurrentContext.WorkflowConfigurations
                        on attestattion.WorkflowConfigurationId equals wfConfig.Id
                    from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).DefaultIfEmpty()
                        //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == task.WorkflowStateId
                    where task.CreatedDate >= fromDate &&
                         task.IsActive &&
                         task.IsCompleted == false
                    select new PresentationTask
                    {
                        TaskId = task.Id,
                        RequestTypeId = task.RequestTypeId,
                        UserId = user.Id,
                        UserName = user.DisplayName,
                        AttestationId = task.AttestationId,
                        AttestationName = attestattion.AttestationName,
                        CreatedDate = task.CreatedDate,
                        IsActive = task.IsActive,
                        IsCompleted = task.IsCompleted,
                        ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                        StartDate = null,
                        EndDate = null,
                        RequestedDate = task.CreatedDate,
                    }).ToList();
        }

        public List<PresentationTask> GetPendingProvisioningTasks(DateTime fromDate)
        {
            return (from tsk in CurrentContext.Tasks
                    join acsrqst in CurrentContext.AccessRequest
                    on tsk.AccessRequestId equals acsrqst.Id
                    join adhocrqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                    on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                    join rqsteduser in CurrentContext.User
                    on adhocrqst.RequesterUserId equals rqsteduser.Id into requester
                    join wfConfig in CurrentContext.WorkflowConfigurations
                    on tsk.WorkflowConfigurationId equals wfConfig.Id
                    from rqsteduser in requester.DefaultIfEmpty()
                    from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == tsk.WorkflowStateId).DefaultIfEmpty()
                        //from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                        //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == tsk.WorkflowStateId
                    where tsk.CreatedDate >= fromDate &&
                        tsk.IsActive &&
                        tsk.IsCompleted == false
                    select new PresentationTask
                    {
                        TaskId = tsk.Id,
                        RequestTypeId = tsk.RequestTypeId,
                        AttestationId = tsk.AttestationId,
                        AttestationName = string.Empty,
                        EventName = adhocrqst.EndPoint.DisplayName + " - " + ((int)adhocrqst.AdhocActionType == 1 ? "Add New Account" : ((int)adhocrqst.AdhocActionType == 2) ? "Amendment Add" : ((int)adhocrqst.AdhocActionType == 3 ? "Amendment Remove" : ((int)adhocrqst.AdhocActionType == 4 ? "Delete Account" : "None"))),
                        CreatedDate = tsk.CreatedDate,
                        IsActive = tsk.IsActive,
                        IsCompleted = tsk.IsCompleted,
                        ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                        UserName = string.Empty,
                        AdHocAccessRequestId = adhocrqst.Id,
                        RequestedBy = rqsteduser != null ? rqsteduser.DisplayName : "System",
                        StartDate = adhocrqst.ValidFrom,
                        EndDate = adhocrqst.ValidTo,
                        RequestedDate = tsk.CreatedDate,
                    }).ToList();
        }

        public List<PresentationTask> GetCompletedAttestationTasksByActionUserId(long actionUserId, DateTime fromDate)
        {
            return (from task in CurrentContext.Tasks
                    join user in CurrentContext.User
                      on task.ActionUserId equals user.Id
                    join attestattion in CurrentContext.Attestations
                        on task.AttestationId equals attestattion.Id
                    join wfConfig in CurrentContext.WorkflowConfigurations
                        on attestattion.WorkflowConfigurationId equals wfConfig.Id
                    from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).DefaultIfEmpty()
                        //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == task.WorkflowStateId
                    where task.CreatedDate >= fromDate &&
                          task.IsActive &&
                          task.IsCompleted &&
                          task.ActionUserId == actionUserId
                    select new PresentationTask
                    {
                        TaskId = task.Id,
                        RequestTypeId = task.RequestTypeId,
                        UserId = user.Id,
                        UserName = user.DisplayName,
                        AttestationId = task.AttestationId,
                        AttestationName = attestattion.AttestationName,
                        CreatedDate = task.CreatedDate,
                        IsActive = task.IsActive,
                        IsCompleted = task.IsCompleted,
                        ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                        StartDate = null,
                        EndDate = null,
                        RequestedDate = task.CreatedDate,
                    }).ToList();
        }

        public List<PresentationTask> GetCompletedProvisioningTasksByActionUserId(long actionUserId, DateTime fromDate)
        {
            return (from task in CurrentContext.Tasks
                    join acsrqst in CurrentContext.AccessRequest
                    on task.AccessRequestId equals acsrqst.Id
                    join adhocrqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                    on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                    join rqsteduser in CurrentContext.User
                    on adhocrqst.RequesterUserId equals rqsteduser.Id into requester
                    join wfConfig in CurrentContext.WorkflowConfigurations
                    on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                    from rqsteduser in requester.DefaultIfEmpty()
                    from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).DefaultIfEmpty()
                        //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == task.WorkflowStateId
                    where task.CreatedDate >= fromDate &&
                       task.IsActive &&
                       task.IsCompleted &&
                       task.ActionUserId == actionUserId
                    select new PresentationTask
                    {
                        TaskId = task.Id,
                        RequestTypeId = task.RequestTypeId,
                        AttestationId = task.AttestationId,
                        AttestationName = string.Empty,
                        EventName = adhocrqst.EndPoint.DisplayName + " - " + ((int)adhocrqst.AdhocActionType == 1 ? "Add New Account" : ((int)adhocrqst.AdhocActionType == 2) ? "Amendment Add" : ((int)adhocrqst.AdhocActionType == 3 ? "Amendment Remove" : ((int)adhocrqst.AdhocActionType == 4 ? "Delete Account" : "None"))),
                        CreatedDate = task.CreatedDate,
                        IsActive = task.IsActive,
                        IsCompleted = task.IsCompleted,
                        ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                        UserName = string.Empty,
                        AdHocAccessRequestId = adhocrqst.Id,
                        RequestedBy = rqsteduser != null ? rqsteduser.DisplayName : "System",
                        StartDate = adhocrqst.ValidFrom,
                        EndDate = adhocrqst.ValidTo,
                        RequestedDate = task.CreatedDate,
                    }).ToList();

        }

        public List<PresentationTask> GetCompletedAttestationTasks(DateTime fromDate)
        {
            return (from task in CurrentContext.Tasks
                    join user in CurrentContext.User.Include("NBusinessRole").Include("NOrganisation")
                      on task.ActionUserId equals user.Id
                    join attestattion in CurrentContext.Attestations
                        on task.AttestationId equals attestattion.Id
                    join attCampaign in CurrentContext.AttestationCampaigns
                        on task.AttestationCampaignId equals attCampaign.Id
                    join wfConfig in CurrentContext.WorkflowConfigurations
                        on attestattion.WorkflowConfigurationId equals wfConfig.Id
                    from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).DefaultIfEmpty()
                        //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == task.WorkflowStateId
                    where task.CreatedDate >= fromDate &&
                         task.IsActive &&
                         task.IsCompleted
                    select new PresentationTask
                    {
                        TaskId = task.Id,
                        RequestTypeId = task.RequestTypeId,
                        UserId = user.Id,
                        UserName = user.DisplayName,
                        AttestationId = task.AttestationId,
                        AttestationName = attestattion.AttestationName,
                        AttestationCampaignId = task.AttestationCampaignId,
                        CampaignName = attCampaign.CampaignName,
                        CreatedDate = task.CreatedDate,
                        IsActive = task.IsActive,
                        RequestedFor = user.DisplayName,
                        RequestForDept = user.NOrganisation != null ? user.NOrganisation.Name : "",
                        RequestForDesignation = user.NBusinessRole != null ? user.NBusinessRole.Description : "",
                        IsCompleted = task.IsCompleted,
                        ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                        StartDate = null,
                        EndDate = null,
                        RequestedDate = task.CreatedDate,
                    }).ToList();
        }

        public List<PresentationTask> GetCompletedProvisioningTasks(DateTime fromDate)
        {
            return (from task in CurrentContext.Tasks
                    join acsrqst in CurrentContext.AccessRequest
                    on task.AccessRequestId equals acsrqst.Id
                    join adhocrqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                    on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                    join rqsteduser in CurrentContext.User
                    on adhocrqst.RequesterUserId equals rqsteduser.Id into requester
                    join wfConfig in CurrentContext.WorkflowConfigurations
                    on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                    from rqsteduser in requester.DefaultIfEmpty()
                    from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).DefaultIfEmpty()
                        //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == task.WorkflowStateId
                    where task.CreatedDate >= fromDate &&
                       task.IsActive &&
                       task.IsCompleted
                    select new PresentationTask
                    {
                        TaskId = task.Id,
                        RequestTypeId = task.RequestTypeId,
                        AttestationId = task.AttestationId,
                        AttestationName = string.Empty,
                        EventName = adhocrqst.EndPoint.DisplayName + " - " + ((int)adhocrqst.AdhocActionType == 1 ? "Add New Account" : ((int)adhocrqst.AdhocActionType == 2) ? "Amendment Add" : ((int)adhocrqst.AdhocActionType == 3 ? "Amendment Remove" : ((int)adhocrqst.AdhocActionType == 4 ? "Delete Account" : "None"))),
                        CreatedDate = task.CreatedDate,
                        IsActive = task.IsActive,
                        IsCompleted = task.IsCompleted,
                        ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                        UserName = string.Empty,
                        AdHocAccessRequestId = adhocrqst.Id,
                        RequestedBy = rqsteduser != null ? rqsteduser.DisplayName : "System",
                        StartDate = adhocrqst.ValidFrom,
                        EndDate = adhocrqst.ValidTo,
                        RequestedDate = task.CreatedDate,
                    }).ToList();
        }

        public List<PresentationTask> GetAttestationTasksByActionUserId(long actionUserId, DateTime fromDate)
        {
            return (from task in CurrentContext.Tasks
                    join user in CurrentContext.User.Include("NBusinessRole").Include("NOrganisation")
                      on task.ActionUserId equals user.Id
                    join attestattion in CurrentContext.Attestations
                        on task.AttestationId equals attestattion.Id
                    join attCampaign in CurrentContext.AttestationCampaigns
                        on task.AttestationCampaignId equals attCampaign.Id
                    join wfConfig in CurrentContext.WorkflowConfigurations
                        on attestattion.WorkflowConfigurationId equals wfConfig.Id
                    from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).DefaultIfEmpty()
                        //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == task.WorkflowStateId
                    where task.CreatedDate >= fromDate &&
                          task.IsActive == false &&
                          task.IsCompleted == false &&
                          task.ActionUserId == actionUserId
                    select new PresentationTask
                    {
                        TaskId = task.Id,
                        RequestTypeId = task.RequestTypeId,
                        UserId = user.Id,
                        UserName = user.DisplayName,
                        AttestationId = task.AttestationId,
                        AttestationName = attestattion.AttestationName,
                        AttestationCampaignId = task.AttestationCampaignId,
                        CampaignName = attCampaign.CampaignName,
                        CreatedDate = task.CreatedDate,
                        IsActive = task.IsActive,
                        IsCompleted = task.IsCompleted,
                        RequestedFor = user.DisplayName,
                        RequestForDept = user.NOrganisation != null ? user.NOrganisation.Name : "",
                        RequestForDesignation = user.NBusinessRole != null ? user.NBusinessRole.Description : "",
                        ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                        StartDate = null,
                        EndDate = null,
                        RequestedDate = task.CreatedDate,
                    }).ToList();
        }

        public List<PresentationTask> GetProvisioningTasksByActionUserId(long actionUserId, DateTime fromDate)
        {
            return (from task in CurrentContext.Tasks
                    join acsrqst in CurrentContext.AccessRequest
                    on task.AccessRequestId equals acsrqst.Id
                    join adhocrqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                    on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                    join rqsteduser in CurrentContext.User
                    on adhocrqst.RequesterUserId equals rqsteduser.Id into requester
                    join wfConfig in CurrentContext.WorkflowConfigurations
                    on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                    from rqsteduser in requester.DefaultIfEmpty()
                    from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).DefaultIfEmpty()
                        //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == task.WorkflowStateId
                    where task.CreatedDate >= fromDate &&
                       task.IsActive == false &&
                       task.IsCompleted == false &&
                       task.ActionUserId == actionUserId
                    select new PresentationTask
                    {
                        TaskId = task.Id,
                        RequestTypeId = task.RequestTypeId,
                        AttestationId = task.AttestationId,
                        AttestationName = string.Empty,
                        EventName = adhocrqst.EndPoint.DisplayName + " - " + ((int)adhocrqst.AdhocActionType == 1 ? "Add New Account" : ((int)adhocrqst.AdhocActionType == 2) ? "Amendment Add" : ((int)adhocrqst.AdhocActionType == 3 ? "Amendment Remove" : ((int)adhocrqst.AdhocActionType == 4 ? "Delete Account" : "None"))),
                        CreatedDate = task.CreatedDate,
                        IsActive = task.IsActive,
                        IsCompleted = task.IsCompleted,
                        ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                        UserName = string.Empty,
                        AdHocAccessRequestId = adhocrqst.Id,
                        RequestedBy = rqsteduser != null ? rqsteduser.DisplayName : "System",
                        StartDate = adhocrqst.ValidFrom,
                        EndDate = adhocrqst.ValidTo,
                        RequestedDate = task.CreatedDate,
                    }).ToList();
        }

        public List<PresentationTask> GetAttestationTasks(DateTime fromDate)
        {
            return (from task in CurrentContext.Tasks
                    join user in CurrentContext.User.Include("NBusinessRole").Include("NOrganisation")
                      on task.ActionUserId equals user.Id
                    join attestattion in CurrentContext.Attestations
                        on task.AttestationId equals attestattion.Id
                    join attCampaign in CurrentContext.AttestationCampaigns
                                    on task.AttestationCampaignId equals attCampaign.Id
                    join wfConfig in CurrentContext.WorkflowConfigurations
                        on attestattion.WorkflowConfigurationId equals wfConfig.Id
                    from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).DefaultIfEmpty()
                        //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == task.WorkflowStateId
                    where task.CreatedDate >= fromDate &&
                         task.IsActive == false &&
                         task.IsCompleted == false
                    select new PresentationTask
                    {
                        TaskId = task.Id,
                        RequestTypeId = task.RequestTypeId,
                        UserId = user.Id,
                        UserName = user.DisplayName,
                        AttestationId = task.AttestationId,
                        AttestationName = attestattion.AttestationName,
                        AttestationCampaignId = task.AttestationCampaignId,
                        CampaignName = attCampaign.CampaignName,
                        CreatedDate = task.CreatedDate,
                        RequestedFor = user.DisplayName,
                        RequestForDept = user.NOrganisation != null ? user.NOrganisation.Name : "",
                        RequestForDesignation = user.NBusinessRole != null ? user.NBusinessRole.Description : "",
                        IsActive = task.IsActive,
                        IsCompleted = task.IsCompleted,
                        ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                        StartDate = null,
                        EndDate = null,
                        RequestedDate = task.CreatedDate,
                    }).ToList();
        }

        public List<PresentationTask> GetProvisioningTasks(DateTime fromDate)
        {
            return (from task in CurrentContext.Tasks
                    join acsrqst in CurrentContext.AccessRequest
                    on task.AccessRequestId equals acsrqst.Id
                    join adhocrqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                    on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                    join rqsteduser in CurrentContext.User
                    on adhocrqst.RequesterUserId equals rqsteduser.Id into requester
                    join wfConfig in CurrentContext.WorkflowConfigurations
                    on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                    from rqsteduser in requester.DefaultIfEmpty()
                    from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).DefaultIfEmpty()
                        //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == task.WorkflowStateId
                    where task.CreatedDate >= fromDate &&
                       task.IsActive == false &&
                       task.IsCompleted == false
                    select new PresentationTask
                    {
                        TaskId = task.Id,
                        RequestTypeId = task.RequestTypeId,
                        AttestationId = task.AttestationId,
                        AttestationName = string.Empty,
                        EventName = adhocrqst.EndPoint.DisplayName + " - " + ((int)adhocrqst.AdhocActionType == 1 ? "Add New Account" : ((int)adhocrqst.AdhocActionType == 2) ? "Amendment Add" : ((int)adhocrqst.AdhocActionType == 3 ? "Amendment Remove" : ((int)adhocrqst.AdhocActionType == 4 ? "Delete Account" : "None"))),
                        CreatedDate = task.CreatedDate,
                        IsActive = task.IsActive,
                        IsCompleted = task.IsCompleted,
                        ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                        UserName = string.Empty,
                        AdHocAccessRequestId = adhocrqst.Id,
                        RequestedBy = rqsteduser != null ? rqsteduser.DisplayName : "System",
                        StartDate = adhocrqst.ValidFrom,
                        EndDate = adhocrqst.ValidTo,
                        RequestedDate = task.CreatedDate,
                    }).ToList();
        }

        public IList<PresentationTask> GetByStatus(StatusType taskStatus, DateTime? dtFrom, DateTime? dtTo, long requestFor = 0, ApplicationType applicationType = ApplicationType.None)
        {
            var userQuery = from reqFor in CurrentContext.User
                            where (requestFor == 0 ? true : reqFor.Id == requestFor)
                            select reqFor;

            var bulkrequest = from blkReq in CurrentContext.BulkRequests
                              join blkDetail in CurrentContext.BulkRequestDetails on blkReq.Id equals blkDetail.BulkRequestId
                              where (requestFor == 0 ? true : blkDetail.UserId == requestFor)
                              select blkReq;
            IQueryable<AccessRequest> currentAccessRequest = null;
            if (applicationType != ApplicationType.None)
            {
                currentAccessRequest = from accessReq in CurrentContext.AccessRequest
                                       join ep in CurrentContext.EndPoint on accessReq.EndpointSystemId equals ep.Id
                                       where ep.ApplicationType == applicationType
                                       select accessReq;
            }
            else
            {
                currentAccessRequest = from accessReq in CurrentContext.AccessRequest
                                       select accessReq;
            }
            var attestationTasksForUser = (from task in CurrentContext.Tasks
                                           join attestattion in CurrentContext.Attestations
                                               on task.AttestationId equals attestattion.Id
                                           join attCampaign in CurrentContext.AttestationCampaigns
                                               on task.AttestationCampaignId equals attCampaign.Id
                                           join user in CurrentContext.User
                                               on task.ActionUserId equals user.Id
                                           join assignedby in CurrentContext.User
                                                  on (long)task.AssignedBy equals assignedby.Id into Adetail
                                           join assignedUser in CurrentContext.User
                                             on (long)task.AssignedTo equals assignedUser.Id into detail
                                           from assignedUser in detail.DefaultIfEmpty()
                                           from assignedby in Adetail.DefaultIfEmpty()
                                           join wfConfig in CurrentContext.WorkflowConfigurations
                                               on attestattion.WorkflowConfigurationId equals wfConfig.Id
                                           from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).DefaultIfEmpty()
                                               //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                           where task.Action == taskStatus && !task.IsGroupTask &&
                                            (!dtFrom.HasValue || task.CreatedDate >= dtFrom.Value) && (!dtTo.HasValue || task.CreatedDate <= dtTo.Value)
                                           select new PresentationTask
                                           {
                                               TaskId = task.Id,
                                               RequestTypeId = task.RequestTypeId,
                                               TaskSource = "Attestation",
                                               AttestationId = task.AttestationId,
                                               AttestationName = attestattion.AttestationName,
                                               AttestationCampaignId = task.AttestationCampaignId,
                                               CampaignName = attCampaign.CampaignName,
                                               EventName = String.Empty,
                                               CreatedDate = task.CreatedDate,
                                               IsActive = task.IsActive,
                                               IsCompleted = task.IsCompleted,
                                               ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                               AdHocAccessRequestId = 0,
                                               RequestedBy = "System",
                                               AssignedTo = task.AssignedTo,
                                               AssignedBy = task.AssignedBy,
                                               AccountType = AccountType.None.ToString(),
                                               IsDelegated = task.IsDelegated,
                                               IsGroup = task.IsGroupTask,
                                               RequestedFor = attestattion.AttestationName,
                                               AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                               AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                               AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                               StartDate = task.CreatedDate,
                                               EndDate = DbFunctions.AddDays(task.CreatedDate,
                                                (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault() == null ? 1 :
                                                task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().ResponseTimeInDays
                                         //+ (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().IsEscalationRequired ?
                                         //task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().DaysToWaitAfterEscalation : 0)
                                         )),
                                               TaskAccessRequest = DI.Chakra.Infrastructure.Enums.AccessRequestStatus.None,
                                               AccessRequestID = attestattion.Id,
                                               Type = attCampaign.CampaignName,
                                               StatusType = task.Action.ToString(),
                                               RequestReason = string.Empty,
                                               DelegatedBy = task.DelegatedBy,
                                               DelegatedComment = task.DelegationComments,
                                               UserName = user.UserName,
                                               ActionUser = user.DisplayName,
                                               ActionUserADID = user.UserName,
                                               RequestedDate = task.CreatedDate,
                                           }).Distinct().ToList();
            var attestationTasksForGroup = (from task in CurrentContext.Tasks
                                            join attestattion in CurrentContext.Attestations
                                                on task.AttestationId equals attestattion.Id
                                            join attCampaign in CurrentContext.AttestationCampaigns
                                                on task.AttestationCampaignId equals attCampaign.Id
                                            join grp in CurrentContext.Group
                                                on task.ActionGroupId equals grp.Id
                                            join assignedby in CurrentContext.User
                                                   on (long)task.AssignedBy equals assignedby.Id into Adetail
                                            join assignedUser in CurrentContext.User
                                              on (long)task.AssignedTo equals assignedUser.Id into detail
                                            join actionuser in CurrentContext.User
                                                on task.ActionUserId equals actionuser.Id into actionUser
                                            from assignedUser in detail.DefaultIfEmpty()
                                            from assignedby in Adetail.DefaultIfEmpty()
                                            from actionuser in actionUser.DefaultIfEmpty()
                                            join wfConfig in CurrentContext.WorkflowConfigurations
                                                on attestattion.WorkflowConfigurationId equals wfConfig.Id
                                            from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).DefaultIfEmpty()
                                                //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                            where task.Action == taskStatus && task.IsGroupTask &&
                                            (!dtFrom.HasValue || task.CreatedDate >= dtFrom.Value) && (!dtTo.HasValue || task.CreatedDate <= dtTo.Value)
                                            select new PresentationTask
                                            {
                                                TaskId = task.Id,
                                                RequestTypeId = task.RequestTypeId,
                                                TaskSource = "Attestation",
                                                AttestationId = task.AttestationId,
                                                AttestationName = attestattion.AttestationName,
                                                AttestationCampaignId = task.AttestationCampaignId,
                                                CampaignName = attCampaign.CampaignName,
                                                EventName = String.Empty,
                                                CreatedDate = task.CreatedDate,
                                                IsActive = task.IsActive,
                                                IsCompleted = task.IsCompleted,
                                                ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                                AdHocAccessRequestId = 0,
                                                RequestedBy = "System",
                                                AssignedTo = task.AssignedTo,
                                                AssignedBy = task.AssignedBy,
                                                AccountType = AccountType.None.ToString(),
                                                IsDelegated = task.IsDelegated,
                                                IsGroup = task.IsGroupTask,
                                                RequestedFor = grp.GroupName,
                                                AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                                AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                                AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                                StartDate = task.CreatedDate,
                                                EndDate = DbFunctions.AddDays(task.CreatedDate,
                                            (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault() == null ? 1 :
                                            task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().ResponseTimeInDays
                                          //+ (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().IsEscalationRequired ?
                                          //task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().DaysToWaitAfterEscalation : 0)
                                          )),
                                                TaskAccessRequest = DI.Chakra.Infrastructure.Enums.AccessRequestStatus.None,
                                                AccessRequestID = attestattion.Id,
                                                Type = attCampaign.CampaignName,
                                                StatusType = task.Action.ToString(),
                                                RequestReason = string.Empty,
                                                DelegatedBy = task.DelegatedBy,
                                                DelegatedComment = task.DelegationComments,
                                                UserName = task.Approver,
                                                Approver = task.Approver,
                                                ActionUser = task.IsDelegated ? actionuser.DisplayName : (assignedUser != null ? assignedUser.DisplayName : ""),
                                                ActionUserADID = task.IsDelegated ? actionuser.UserName : (assignedUser != null ? assignedUser.UserName : ""),
                                                RequestedDate = task.CreatedDate,
                                            }).Distinct().ToList();
            var attestationTasks = attestationTasksForGroup.Union(attestationTasksForUser).ToList();

            var provisioningTasks = (from tsk in CurrentContext.Tasks
                                     join acsrqst in currentAccessRequest
                                     on tsk.AccessRequestId equals acsrqst.Id
                                     join adhocrqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                                     on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                                     join rqsteduser in CurrentContext.User
                                        on adhocrqst.RequesterUserId equals rqsteduser.Id into requester
                                     from rqsteduser in requester.DefaultIfEmpty()
                                     join rqsteruser in userQuery.Include("NBusinessRole").Include("NOrganisation")
                                        on adhocrqst.RequesteeUserId equals rqsteruser.Id
                                     join wfConfig in CurrentContext.WorkflowConfigurations
                                     on tsk.WorkflowConfigurationId equals wfConfig.Id
                                     join actionuser in CurrentContext.User
                                        on (long)tsk.ActionUserId equals actionuser.Id into actionUser
                                     from actionuser in actionUser.DefaultIfEmpty()
                                     join assignedby in CurrentContext.User
                                           on (long)tsk.AssignedBy equals assignedby.Id into Adetail
                                     join assignedUser in CurrentContext.User
                                       on (long)tsk.AssignedTo equals assignedUser.Id into detail
                                     from assignedUser in detail.DefaultIfEmpty()
                                     from assignedby in Adetail.DefaultIfEmpty()
                                     from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == tsk.WorkflowStateId).DefaultIfEmpty()
                                     join actiongroup in CurrentContext.Group
                                     on (long)tsk.ActionGroupId equals actiongroup.Id into actionGroup
                                     from actiongroup in actionGroup.DefaultIfEmpty()
                                         //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == tsk.WorkflowStateId
                                     where tsk.Action == taskStatus && (!dtFrom.HasValue || tsk.CreatedDate >= dtFrom.Value) && (!dtTo.HasValue || tsk.CreatedDate <= dtTo.Value)
                                     select new PresentationTask
                                     {
                                         TaskId = tsk.Id,
                                         RequestTypeId = tsk.RequestTypeId,
                                         TaskSource = "Access Request",
                                         AttestationId = tsk.AttestationId,
                                         AttestationName = string.Empty,
                                         AttestationCampaignId = tsk.AttestationCampaignId,
                                         CampaignName = String.Empty,
                                         EventName = adhocrqst.EndPoint.DisplayName,
                                         ApplicationType = adhocrqst.EndPoint.ApplicationType,
                                         CreatedDate = tsk.CreatedDate,
                                         IsActive = tsk.IsActive,
                                         IsCompleted = tsk.IsCompleted,
                                         ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                         AdHocAccessRequestId = adhocrqst.Id,
                                         RequestedBy = acsrqst.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.AdhocRequest ? (rqsteduser != null ? rqsteduser.DisplayName : "System") : "System",
                                         AssignedTo = tsk.AssignedTo,
                                         AssignedBy = tsk.AssignedBy,
                                         AccountType = adhocrqst.AccountType.ToString(),
                                         IsDelegated = tsk.IsDelegated,
                                         IsGroup = tsk.IsGroupTask,
                                         GroupName = actiongroup.GroupName,
                                         RequestedFor = rqsteruser.DisplayName,
                                         RequestForDept = rqsteruser.NOrganisation != null ? rqsteruser.NOrganisation.Name : "",
                                         RequestForDesignation = rqsteruser.NBusinessRole != null ? rqsteruser.NBusinessRole.Description : "",
                                         AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                         AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                         AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                         StartDate = tsk.CreatedDate,
                                         EndDate = DbFunctions.AddDays(tsk.CreatedDate,
                                  (tsk.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == tsk.WorkflowStateId).FirstOrDefault() == null ? 1 :
                                  tsk.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == tsk.WorkflowStateId).FirstOrDefault().ResponseTimeInDays
                                  //+ (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().IsEscalationRequired ?
                                  //task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().DaysToWaitAfterEscalation : 0)
                                  )),
                                         TaskAccessRequest = acsrqst.RequestStatus,
                                         AccessRequestID = acsrqst.Id,
                                         Type = ((int)adhocrqst.AdhocActionType == 1 ? "Add New Account" : ((int)adhocrqst.AdhocActionType == 2) ? "Amendment Add" : ((int)adhocrqst.AdhocActionType == 3 ? "Amendment Remove" : ((int)adhocrqst.AdhocActionType == 4 ? "Delete Account" : "None"))),


                                         /* ((int)adhocrqst.AdhocActionType == 2 || (int)adhocrqst.AdhocActionType == 3) ? ((int)adhocrqst.AdhocActionType == 2 ? "Add Role" : "Remove Role") : ((int)adhocrqst.AdhocActionType == 1 ? "Create Login-ID" : "Remove Login-ID"),*/
                                         StatusType = tsk.Action.ToString(),
                                         RequestReason = adhocrqst.Comments, //acsrqst.Comments ,
                                         DelegatedBy = tsk.DelegatedBy,
                                         DelegatedComment = tsk.DelegationComments,
                                         UserName = rqsteduser.UserName,
                                         Approver = tsk.Approver,
                                         ActionUser = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedby != null ? assignedby.DisplayName : "") : actionuser.DisplayName,
                                         ActionUserADID = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedby != null ? assignedby.UserName : "") : actionuser.UserName,
                                         RequestedDate = tsk.CreatedDate,
                                         EntitlementXML = adhocrqst.EntitlementDetail
                                     }).Distinct().ToList();

            var nonEmployeeEventTasksQuery = (from tsk in CurrentContext.Tasks
                                              join trigger in CurrentContext.EmployeeEventTriggers
                                              on tsk.EmployeeEventTriggerId equals trigger.Id
                                              join rqsteduser in CurrentContext.User
                                              on trigger.CreatedBy equals rqsteduser.UserName
                                              join actionuser in CurrentContext.User
                                              on (long)tsk.ActionUserId equals actionuser.Id into actionUser
                                              from actionuser in actionUser.DefaultIfEmpty()
                                              join rqsteruser in userQuery
                                                 on trigger.UserId equals rqsteruser.Id
                                              join assignedby in CurrentContext.User
                                                     on tsk.AssignedBy equals assignedby.Id into Adetail
                                              join assignedUser in CurrentContext.User
                                                on tsk.AssignedTo equals assignedUser.Id into detail
                                              from assignedUser in detail.DefaultIfEmpty()
                                              from assignedby in Adetail.DefaultIfEmpty()
                                              where
                                              (!dtFrom.HasValue || tsk.CreatedDate >= dtFrom.Value) && (!dtTo.HasValue || tsk.CreatedDate <= dtTo.Value) &&
                                              ((tsk.Action == taskStatus && tsk.EmployeeEventTriggerId.HasValue) && (trigger.EventType == EmployeeEvents.NonEmployeeExtension || trigger.EventType == EmployeeEvents.NonEmployeeCreation))
                                              select new PresentationTask
                                              {
                                                  TaskId = tsk.Id,
                                                  RequestTypeId = tsk.RequestTypeId,
                                                  TaskSource = "Employee Creation",
                                                  AttestationId = tsk.AttestationId,
                                                  AttestationName = string.Empty,
                                                  EventName = trigger.EventType == EmployeeEvents.NonEmployeeCreation ? "Non Employee Creation" : (trigger.EventType == EmployeeEvents.NonEmployeeExtension ? "Non Employee Extension / Shortening" : string.Empty),
                                                  CreatedDate = tsk.CreatedDate,
                                                  IsActive = tsk.IsActive,
                                                  IsCompleted = tsk.IsCompleted,
                                                  ReponseTimeInDays = 0,
                                                  UserName = string.Empty,
                                                  AdHocAccessRequestId = null,
                                                  AccessRequestID = null,
                                                  RequestedBy = rqsteduser.DisplayName,
                                                  AssignedTo = tsk.AssignedTo,
                                                  AssignedBy = tsk.AssignedBy,
                                                  AccountType = AccountType.None.ToString(),
                                                  IsDelegated = tsk.IsDelegated,
                                                  IsGroup = tsk.IsGroupTask,
                                                  RequestedFor = rqsteruser.DisplayName,
                                                  AssignedToName = assignedUser.DisplayName,
                                                  AssignedByName = assignedby.DisplayName,
                                                  StatusType = tsk.Action.ToString(),
                                                  RequestedDate = trigger.CreatedDate,
                                                  Type = trigger.EventType == EmployeeEvents.NonEmployeeCreation ? "Non Employee Creation" : (trigger.EventType == EmployeeEvents.NonEmployeeExtension ? "Non Employee Extension / Shortening" : string.Empty),
                                                  ActionUser = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : "") : actionuser.DisplayName,
                                                  ActionUserADID = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.UserName : "") : actionuser.UserName
                                              });

            var nonEmployeeEventTasks = nonEmployeeEventTasksQuery.ToList();

            var provisioningTasksFromBulkRequest = (from tsk in CurrentContext.Tasks
                                                    join blkReq in bulkrequest.Include("BulkRequestDetails").Include("EndPoint")
                                                    on tsk.BulkRequestId equals blkReq.Id
                                                    join rqsteduser in CurrentContext.User
                                                    on blkReq.RequestedBy equals rqsteduser.Id into requester
                                                    from rqsteduser in requester.DefaultIfEmpty()
                                                    join rqsteruser in CurrentContext.User.Include("NBusinessRole").Include("NOrganisation")
                                                    on blkReq.SelectedSupervisor equals rqsteruser.Id
                                                    join assignedby in CurrentContext.User
                                                           on (long)tsk.AssignedBy equals assignedby.Id into Adetail
                                                    join assignedUser in CurrentContext.User
                                                      on (long)tsk.AssignedTo equals assignedUser.Id into detail
                                                    from assignedUser in detail.DefaultIfEmpty()
                                                    from assignedby in Adetail.DefaultIfEmpty()
                                                    join wfConfig in CurrentContext.WorkflowConfigurations
                                                    on tsk.WorkflowConfigurationId equals wfConfig.Id
                                                    from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == tsk.WorkflowStateId).DefaultIfEmpty()

                                                    where (!dtFrom.HasValue || tsk.CreatedDate >= dtFrom.Value) && (!dtTo.HasValue || tsk.CreatedDate <= dtTo.Value) && tsk.Action == taskStatus && !tsk.IsGroupTask
                                                    select new PresentationTask
                                                    {
                                                        TaskId = tsk.Id,
                                                        RequestTypeId = tsk.RequestTypeId,
                                                        TaskSource = DI.Chakra.Infrastructure.Constants.BulkRequest,
                                                        AttestationId = tsk.AttestationId,
                                                        AttestationName = string.Empty,
                                                        AttestationCampaignId = tsk.AttestationCampaignId,
                                                        CampaignName = String.Empty,
                                                        EventName = blkReq.EndPoint.DisplayName,
                                                        ApplicationType = blkReq.EndPoint.ApplicationType,
                                                        CreatedDate = tsk.CreatedDate,
                                                        IsActive = tsk.IsActive,
                                                        IsCompleted = tsk.IsCompleted,
                                                        ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                                        UserName = string.Empty,
                                                        AdHocAccessRequestId = null,
                                                        AccessRequestID = blkReq.Id,
                                                        RequestedBy = blkReq.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.AdhocRequest ? (rqsteduser != null ? rqsteduser.DisplayName : "System") : "System",
                                                        AssignedTo = tsk.AssignedTo,
                                                        AssignedBy = tsk.AssignedBy,
                                                        AccountType = "User",
                                                        IsDelegated = tsk.IsDelegated,
                                                        IsGroup = tsk.IsGroupTask,
                                                        RequestedFor = "Bulk Request",
                                                        RequestForDept = rqsteruser.NOrganisation != null ? rqsteruser.NOrganisation.Name : "",
                                                        RequestForDesignation = rqsteruser.NBusinessRole != null ? rqsteruser.NBusinessRole.Description : "",
                                                        AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                                        AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                                        AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                                        StartDate = blkReq.ActivationDate,
                                                        EndDate = blkReq.DeactivationDate,
                                                        RequestedDate = blkReq.CreatedDate,
                                                        //Type = ((int)adhocrqst.AdhocActionType == 2 || (int)adhocrqst.AdhocActionType == 3) ? ((int)adhocrqst.AdhocActionType == 2 ? "Add Role" : "Remove Role") : ((int)adhocrqst.AdhocActionType == 1 ? "Create Login-ID" : "Remove Login-ID") ,
                                                        Type = ((int)blkReq.AdhocActionType == 1 ? "Add New Account" : ((int)blkReq.AdhocActionType == 2) ? "Amendment Add" : ((int)blkReq.AdhocActionType == 3 ? "Amendment Remove" : ((int)blkReq.AdhocActionType == 4 ? "Delete Account" : "None"))),

                                                        //((int)blkReq.AdhocActionType == 2 || (int)blkReq.AdhocActionType == 3) ? ((int)blkReq.AdhocActionType == 2 ? "Add Role" : "Remove Role") : ((int)blkReq.AdhocActionType == 1 ? "Create Login-ID" : (int)blkReq.AdhocActionType == 4 ? "Remove Login-ID" : (int)blkReq.AdhocActionType == 5 ? "Standing Order" : ""),
                                                        StatusType = tsk.Action.ToString(),
                                                        RequestReason = blkReq.Comments, //acsrqst.Comments ,
                                                        DelegatedBy = tsk.DelegatedBy,
                                                        DelegatedComment = tsk.DelegationComments,
                                                        BulkReqDetails = blkReq.BulkRequestDetails,
                                                        EntitlementXML = blkReq.EntitlementDetail
                                                    }).ToList();


            foreach (var item in provisioningTasksFromBulkRequest)
            {
                item.RequestedFor = string.Join(",", item.BulkReqDetails.Select(i => i.Name));
            }

            var tasks = attestationTasks.Union(provisioningTasks).Union(nonEmployeeEventTasks).Distinct();

            tasks = tasks.OrderByDescending(rvw => rvw.CreatedDate).ThenBy(rvw1 => rvw1.IsCompleted);

            return tasks.ToList();
        }

        public IList<PresentationTask> GetAll(DateTime? dtFrom, DateTime? dtTo, long requestFor = 0, long endpointSystemId = 0, StatusType statusType = StatusType.None)
        {
            var currentTask = from tsk in CurrentContext.Tasks
                              where statusType == StatusType.None ? true : tsk.Action == statusType
                              select tsk;
            var userQuery = from reqFor in CurrentContext.User
                            where (requestFor == 0 ? true : reqFor.Id == requestFor)
                            select reqFor;
            var bulkrequest = from blkReq in CurrentContext.BulkRequests
                              join blkDetail in CurrentContext.BulkRequestDetails on blkReq.Id equals blkDetail.BulkRequestId
                              where (requestFor == 0 ? true : blkDetail.UserId == requestFor)
                              && (endpointSystemId > 0) ? blkReq.EndpointSystemId == endpointSystemId : true
                              select blkReq;
            IQueryable<AccessRequest> currentAccessRequest = from accessReq in CurrentContext.AccessRequest
                                                             where (requestFor == 0 ? true : accessReq.UserId == requestFor)
                                                             && (endpointSystemId > 0) ? accessReq.EndpointSystemId == endpointSystemId : true
                                                             select accessReq;

            var attestationTasksForUser = (from task in currentTask
                                           join attestattion in CurrentContext.Attestations
                                               on task.AttestationId equals attestattion.Id
                                           join attCampaign in CurrentContext.AttestationCampaigns
                                               on task.AttestationCampaignId equals attCampaign.Id
                                           join user in CurrentContext.User
                                               on task.ActionUserId equals user.Id
                                           join assignedby in CurrentContext.User
                                                  on (long)task.AssignedBy equals assignedby.Id into Adetail
                                           join assignedUser in CurrentContext.User
                                             on (long)task.AssignedTo equals assignedUser.Id into detail
                                           from assignedUser in detail.DefaultIfEmpty()
                                           from assignedby in Adetail.DefaultIfEmpty()
                                           join wfConfig in CurrentContext.WorkflowConfigurations
                                               on attestattion.WorkflowConfigurationId equals wfConfig.Id
                                           from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).DefaultIfEmpty()
                                           where !task.IsGroupTask && (!dtFrom.HasValue || task.CreatedDate >= dtFrom.Value) && (!dtTo.HasValue || task.CreatedDate <= dtTo.Value)
                                           select new PresentationTask
                                           {
                                               EventName = attCampaign.CampaignName,
                                               TaskId = task.Id,
                                               RequestTypeId = task.RequestTypeId,
                                               TaskSource = "Attestation",
                                               AttestationId = task.AttestationId,
                                               AttestationName = attestattion.AttestationName,
                                               AttestationCampaignId = task.AttestationCampaignId,
                                               CampaignName = attCampaign.CampaignName,
                                               CreatedDate = task.CreatedDate,
                                               IsActive = task.IsActive,
                                               IsCompleted = task.IsCompleted,
                                               ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                               AdHocAccessRequestId = 0,
                                               RequestedBy = "System",
                                               AssignedTo = task.AssignedTo,
                                               AssignedBy = task.AssignedBy,
                                               AccountType = AccountType.None.ToString(),
                                               IsDelegated = task.IsDelegated,
                                               IsGroup = task.IsGroupTask,
                                               RequestedFor = attestattion.AttestationName,
                                               AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                               AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                               AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                               StartDate = task.CreatedDate,
                                               EndDate = DbFunctions.AddDays(task.CreatedDate,
                                         (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault() == null ? 1 :
                                  task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().ResponseTimeInDays
                                         //+ (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().IsEscalationRequired ?
                                         //task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().DaysToWaitAfterEscalation : 0)
                                         )),
                                               TaskAccessRequest = DI.Chakra.Infrastructure.Enums.AccessRequestStatus.None,
                                               AccessRequestID = null,
                                               Type = attCampaign.CampaignName,
                                               StatusType = task.Action.ToString(),
                                               RequestReason = string.Empty,
                                               DelegatedBy = task.DelegatedBy,
                                               DelegatedComment = task.DelegationComments,
                                               UserName = user.UserName,
                                               Approver = task.Approver,
                                               ActionUser = user.DisplayName,
                                               ActionUserADID = user.UserName,
                                               RequestedDate = task.CreatedDate,
                                           }).ToList();


            var attestationTasksForGroup = (from task in currentTask
                                            join attestattion in CurrentContext.Attestations
                                                on task.AttestationId equals attestattion.Id
                                            join attCampaign in CurrentContext.AttestationCampaigns
                                                on task.AttestationCampaignId equals attCampaign.Id
                                            join grp in CurrentContext.Group
                                                on task.ActionGroupId equals grp.Id
                                            join assignedby in CurrentContext.User
                                                   on (long)task.AssignedBy equals assignedby.Id into Adetail
                                            join assignedUser in CurrentContext.User
                                              on (long)task.AssignedTo equals assignedUser.Id into detail
                                            join actionuser in CurrentContext.User
                                             on task.ActionUserId equals actionuser.Id into actionUser
                                            from assignedUser in detail.DefaultIfEmpty()
                                            from assignedby in Adetail.DefaultIfEmpty()
                                            from actionuser in actionUser.DefaultIfEmpty()
                                            join wfConfig in CurrentContext.WorkflowConfigurations
                                                on attestattion.WorkflowConfigurationId equals wfConfig.Id
                                            from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).DefaultIfEmpty()
                                            where task.IsGroupTask && (!dtFrom.HasValue || task.CreatedDate >= dtFrom.Value) && (!dtTo.HasValue || task.CreatedDate <= dtTo.Value)
                                            select new PresentationTask
                                            {
                                                TaskId = task.Id,
                                                RequestTypeId = task.RequestTypeId,
                                                TaskSource = "Attestation",
                                                AttestationId = task.AttestationId,
                                                AttestationName = attestattion.AttestationName,
                                                AttestationCampaignId = task.AttestationCampaignId,
                                                CampaignName = attCampaign.CampaignName,
                                                EventName = attCampaign.CampaignName,
                                                CreatedDate = task.CreatedDate,
                                                IsActive = task.IsActive,
                                                IsCompleted = task.IsCompleted,
                                                ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                                AdHocAccessRequestId = 0,
                                                RequestedBy = "System",
                                                AssignedTo = task.AssignedTo,
                                                AssignedBy = task.AssignedBy,
                                                AccountType = AccountType.None.ToString(),
                                                IsDelegated = task.IsDelegated,
                                                IsGroup = task.IsGroupTask,
                                                RequestedFor = attestattion.AttestationName,
                                                AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                                AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                                AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                                StartDate = task.CreatedDate,
                                                EndDate = DbFunctions.AddDays(task.CreatedDate,
                                  (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault() == null ? 1 :
                                  task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().ResponseTimeInDays

                                  )),
                                                TaskAccessRequest = DI.Chakra.Infrastructure.Enums.AccessRequestStatus.None,
                                                AccessRequestID = null,
                                                Type = attCampaign.CampaignName,
                                                StatusType = task.Action.ToString(),
                                                RequestReason = string.Empty,
                                                DelegatedBy = task.DelegatedBy,
                                                DelegatedComment = task.DelegationComments,
                                                UserName = task.Approver,
                                                Approver = task.Approver,
                                                ActionUser = task.IsDelegated ? actionuser.DisplayName : (assignedUser != null ? assignedUser.DisplayName : grp != null ? grp.GroupName :""),
                                                ActionUserADID = task.IsDelegated ? actionuser.UserName : (assignedUser != null ? assignedUser.UserName : ""),
                                                RequestedDate = task.CreatedDate,
                                            }).ToList();
            var attestationTasks = attestationTasksForGroup.Union(attestationTasksForUser).ToList();


            var provisioningTasks = (from tsk in currentTask
                                     join acsrqst in currentAccessRequest
                                     on tsk.AccessRequestId equals acsrqst.Id
                                     join adhocrqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                                     on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                                     join rqsteduser in CurrentContext.User
                                        on adhocrqst.RequesterUserId equals rqsteduser.Id into requester
                                     from rqsteduser in requester.DefaultIfEmpty()
                                     join rqsteruser in userQuery//.Include("NBusinessRole").Include("NOrganisation")
                                        on adhocrqst.RequesteeUserId equals rqsteruser.Id
                                     join actionuser in CurrentContext.User
                                     on (long)tsk.ActionUserId equals actionuser.Id into actionUser
                                     from actionuser in actionUser.DefaultIfEmpty()
                                     join assignedby in CurrentContext.User
                                           on (long)tsk.AssignedBy equals assignedby.Id into Adetail
                                     join assignedUser in CurrentContext.User
                                       on (long)tsk.AssignedTo equals assignedUser.Id into detail
                                     from assignedUser in detail.DefaultIfEmpty()
                                     from assignedby in Adetail.DefaultIfEmpty()
                                     join wfConfig in CurrentContext.WorkflowConfigurations
                                     on tsk.WorkflowConfigurationId equals wfConfig.Id
                                     from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == tsk.WorkflowStateId).DefaultIfEmpty()
                                     join actiongroup in CurrentContext.Group
                                     on (long)tsk.ActionGroupId equals actiongroup.Id into actionGroup
                                     from actiongroup in actionGroup.DefaultIfEmpty()

                                     where 1 == 1 && (!dtFrom.HasValue || tsk.CreatedDate >= dtFrom.Value) && (!dtTo.HasValue || tsk.CreatedDate <= dtTo.Value)
                                     select new PresentationTask
                                     {
                                         TaskId = tsk.Id,
                                         RequestTypeId = tsk.RequestTypeId,
                                         TaskSource = tsk.AttestationCampaignId != null? "Attestation Remediation" : "Access Request",
                                         AttestationId = tsk.AttestationId,
                                         AttestationName = string.Empty,
                                         AttestationCampaignId = tsk.AttestationCampaignId,
                                         CampaignName = String.Empty,
                                         EventName = adhocrqst.EndPoint.DisplayName,
                                         CreatedDate = tsk.CreatedDate,
                                         IsActive = tsk.IsActive,
                                         IsCompleted = tsk.IsCompleted,
                                         ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                         AdHocAccessRequestId = adhocrqst.Id,
                                         RequestedBy = acsrqst.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.AdhocRequest ? (rqsteduser != null ? rqsteduser.DisplayName : "System") : "System",
                                         AssignedTo = tsk.AssignedTo,
                                         AssignedBy = tsk.AssignedBy,
                                         AccountType = adhocrqst.AccountType.ToString(),
                                         IsDelegated = tsk.IsDelegated,
                                         IsGroup = tsk.IsGroupTask,
                                         GroupName = actiongroup.GroupName,
                                         RequestedFor = rqsteruser.DisplayName,
                                         //RequestForDept = rqsteruser.NOrganisation != null ? rqsteruser.NOrganisation.Name : "",
                                         //RequestForDesignation = rqsteruser.NBusinessRole != null ? rqsteruser.NBusinessRole.Description : "",
                                         AssignedToName = string.Empty,
                                         AssignedToADID = string.Empty,
                                         AssignedByName = String.Empty,
                                         StartDate = tsk.CreatedDate,
                                         EndDate = DbFunctions.AddDays(tsk.CreatedDate,
                                  (tsk.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == tsk.WorkflowStateId).FirstOrDefault() == null ? 1 :
                                  tsk.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == tsk.WorkflowStateId).FirstOrDefault().ResponseTimeInDays
                                  )),
                                         TaskAccessRequest = acsrqst.RequestStatus,
                                         AccessRequestID = acsrqst.Id,
                                         Type = ((int)adhocrqst.AdhocActionType == 1 ? "Add New Account" : ((int)adhocrqst.AdhocActionType == 2) ? "Amendment Add" : ((int)adhocrqst.AdhocActionType == 3 ? "Amendment Remove" : ((int)adhocrqst.AdhocActionType == 4 ? "Delete Account" : "None"))),
                                         StatusType = tsk.Action.ToString(),
                                         RequestReason = adhocrqst.Comments, //acsrqst.Comments ,
                                         DelegatedBy = tsk.DelegatedBy,
                                         DelegatedComment = tsk.DelegationComments,
                                         UserName = rqsteduser.UserName,
                                         Approver = tsk.Approver,
                                         ActionUser = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : tsk.Approver) : actionuser.DisplayName,
                                         ActionUserADID = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.UserName : "") : actionuser.UserName,
                                         RequestedDate = acsrqst.CreatedDate,
                                     }).Distinct().ToList();

            var provisioningTasksFromBulkRequest = (from tsk in currentTask
                                                    join blkReq in bulkrequest.Include("BulkRequestDetails").Include("EndPoint")
                                                    on tsk.BulkRequestId equals blkReq.Id
                                                    join rqsteduser in CurrentContext.User
                                                    on blkReq.RequestedBy equals rqsteduser.Id into requester
                                                    from rqsteduser in requester.DefaultIfEmpty()
                                                    join rqsteruser in CurrentContext.User//.Include("NBusinessRole").Include("NOrganisation")
                                                    on blkReq.SelectedSupervisor equals rqsteruser.Id
                                                    join assignedby in CurrentContext.User
                                                           on (long)tsk.AssignedBy equals assignedby.Id into Adetail
                                                    join assignedUser in CurrentContext.User
                                                      on (long)tsk.AssignedTo equals assignedUser.Id into detail
                                                    from assignedUser in detail.DefaultIfEmpty()
                                                    from assignedby in Adetail.DefaultIfEmpty()
                                                    join wfConfig in CurrentContext.WorkflowConfigurations
                                                    on tsk.WorkflowConfigurationId equals wfConfig.Id
                                                    join actionUser in CurrentContext.User on tsk.ActionUserId equals actionUser.Id into actUser
                                                    from actionUser in actUser.DefaultIfEmpty()
                                                    from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == tsk.WorkflowStateId).DefaultIfEmpty()
                                                    where
                                                     (!dtFrom.HasValue || tsk.CreatedDate >= dtFrom.Value) && (!dtTo.HasValue || tsk.CreatedDate <= dtTo.Value)
                                                     && (requestFor == 0 ? true : blkReq.BulkRequestDetails.Select(i => i.UserId).Contains(requestFor))
                                                    select new PresentationTask
                                                    {
                                                        TaskId = tsk.Id,
                                                        RequestTypeId = tsk.RequestTypeId,
                                                        TaskSource = DI.Chakra.Infrastructure.Constants.BulkRequest,
                                                        AttestationId = tsk.AttestationId,
                                                        AttestationName = string.Empty,
                                                        AttestationCampaignId = tsk.AttestationCampaignId,
                                                        CampaignName = String.Empty,
                                                        EventName = blkReq.EndPoint.DisplayName,
                                                        ApplicationType = blkReq.EndPoint.ApplicationType,
                                                        CreatedDate = tsk.CreatedDate,
                                                        IsActive = tsk.IsActive,
                                                        IsCompleted = tsk.IsCompleted,
                                                        ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                                        UserName = string.Empty,
                                                        AdHocAccessRequestId = null,
                                                        AccessRequestID = blkReq.Id,
                                                        RequestedBy = blkReq.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.AdhocRequest ? (rqsteduser != null ? rqsteduser.DisplayName : "System") : "System",
                                                        AssignedTo = tsk.AssignedTo,
                                                        AssignedBy = tsk.AssignedBy,
                                                        AccountType = "User",
                                                        IsDelegated = tsk.IsDelegated,
                                                        IsGroup = tsk.IsGroupTask,
                                                        RequestedFor = "Bulk Request",
                                                        //RequestForDept = rqsteruser.NOrganisation != null ? rqsteruser.NOrganisation.Name : "",
                                                        //RequestForDesignation = rqsteruser.NBusinessRole != null ? rqsteruser.NBusinessRole.Description : "",
                                                        AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                                        AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                                        AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                                        StartDate = blkReq.ActivationDate,
                                                        EndDate = blkReq.DeactivationDate,
                                                        RequestedDate = blkReq.CreatedDate,

                                                        Type = ((int)blkReq.AdhocActionType == 1 ? "Add New Account" : ((int)blkReq.AdhocActionType == 2) ? "Amendment Add" : ((int)blkReq.AdhocActionType == 3 ? "Amendment Remove" : ((int)blkReq.AdhocActionType == 4 ? "Delete Account" : "None"))),


                                                        StatusType = tsk.Action.ToString(),
                                                        RequestReason = blkReq.Comments, //acsrqst.Comments ,
                                                        DelegatedBy = tsk.DelegatedBy,
                                                        DelegatedComment = tsk.DelegationComments,
                                                        ActionUser = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : tsk.Approver) : actionUser.DisplayName,
                                                        ActionUserADID = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.UserName : "") : "",
                                                        BulkReqDetails = blkReq.BulkRequestDetails,
                                                        EntitlementXML = blkReq.EntitlementDetail
                                                    }).ToList();


            IQueryable<PresentationTask> nonEmployeeEventTasksQuery = null;
            if (endpointSystemId == 0)
            {
                var empTrigger = CurrentContext.EmployeeEventTriggers.Where(i=> i.EventType == EmployeeEvents.NonEmployeeExtension || i.EventType == EmployeeEvents.NonEmployeeCreation);
                currentTask = currentTask.Where(tsk => tsk.EmployeeEventTriggerId.HasValue && (!dtFrom.HasValue || tsk.CreatedDate >= dtFrom.Value) && (!dtTo.HasValue || tsk.CreatedDate <= dtTo.Value));

                nonEmployeeEventTasksQuery = (from trigger in empTrigger 
                                              join tsk in currentTask on trigger.Id equals tsk.EmployeeEventTriggerId
                                              join rqsteduser in CurrentContext.User
                                              on trigger.CreatedBy equals rqsteduser.UserName
                                              join rqsteruser in userQuery
                                                 on trigger.UserId equals rqsteruser.Id
                                              join actionuser in CurrentContext.User
                                                on (long)tsk.ActionUserId equals actionuser.Id into actionUser
                                              from actionuser in actionUser.DefaultIfEmpty()
                                              join assignedby in CurrentContext.User
                                                     on tsk.AssignedBy equals assignedby.Id into Adetail
                                              join assignedUser in CurrentContext.User
                                                on tsk.AssignedTo equals assignedUser.Id into detail
                                              from assignedUser in detail.DefaultIfEmpty()
                                              from assignedby in Adetail.DefaultIfEmpty()
                                              //&& (trigger.EventType == EmployeeEvents.NonEmployeeExtension || trigger.EventType == EmployeeEvents.NonEmployeeCreation)
                                              select new PresentationTask
                                              {
                                                  TaskId = tsk.Id,
                                                  RequestTypeId = tsk.RequestTypeId,
                                                  TaskSource = "Employee Creation",
                                                  AttestationId = tsk.AttestationId,
                                                  AttestationName = string.Empty,
                                                  EventName = trigger.EventType == EmployeeEvents.NonEmployeeCreation ? "Non Employee Creation" : (trigger.EventType == EmployeeEvents.NonEmployeeExtension ? "Non Employee Extension / Shortening" : string.Empty),
                                                  CreatedDate = tsk.CreatedDate,
                                                  IsActive = tsk.IsActive,
                                                  IsCompleted = tsk.IsCompleted,
                                                  ReponseTimeInDays = 0,
                                                  UserName = string.Empty,
                                                  AdHocAccessRequestId = null,
                                                  AccessRequestID = null,
                                                  RequestedBy = rqsteduser.DisplayName,
                                                  AssignedTo = tsk.AssignedTo,
                                                  AssignedBy = tsk.AssignedBy,
                                                  AccountType = AccountType.None.ToString(),
                                                  IsDelegated = tsk.IsDelegated,
                                                  IsGroup = tsk.IsGroupTask,
                                                  RequestedFor = rqsteruser.DisplayName,
                                                  AssignedToName = assignedUser.DisplayName,
                                                  AssignedByName = assignedby.DisplayName,
                                                  StatusType = tsk.Action.ToString(),
                                                  //StartDate = tsk.AdHocAccessRequest.ValidFrom,
                                                  //EndDate = tsk.AdHocAccessRequest.ValidTo,
                                                  RequestedDate = trigger.CreatedDate,
                                                  Type = trigger.EventType == EmployeeEvents.NonEmployeeCreation ? "Non Employee Creation" : (trigger.EventType == EmployeeEvents.NonEmployeeExtension ? "Non Employee Extension / Shortening" : string.Empty),
                                                  ActionUser = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : tsk.Approver) : actionuser.DisplayName,
                                                  ActionUserADID = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.UserName : "") : actionuser.UserName,
                                                  //,
                                                  //  EEmployeeEventTriggerId = tsk.EmployeeEventTriggerId,
                                              });
            }
            foreach (var item in provisioningTasksFromBulkRequest)
            {
                item.RequestedFor = string.Join(",", item.BulkReqDetails.Select(i => i.Name));
            }
            var tasks = attestationTasks.Union(provisioningTasks).Union(provisioningTasksFromBulkRequest);
            if (nonEmployeeEventTasksQuery != null)
            {
                tasks = tasks.Union(nonEmployeeEventTasksQuery);
            }
            tasks = tasks.Distinct();
            tasks = tasks.OrderByDescending(rvw => rvw.CreatedDate).ThenBy(rvw1 => rvw1.IsCompleted);

            return tasks.ToList();
        }

        public IList<PresentationTask> GetByActionUserId(long actionUserId, DateTime? dtFrom, DateTime? dtTo, long endpointSystemId = 0)
        {
            var currentAccessRequest = from accReq in CurrentContext.AccessRequest
                                       where endpointSystemId == 0 ? true : accReq.EndpointSystemId == endpointSystemId
                                       select accReq;
            var currentBulk = from blk in CurrentContext.BulkRequests
                              where endpointSystemId == 0 ? true : blk.EndpointSystemId == endpointSystemId
                              select blk;
            var actionnUser = CurrentContext.User.Where(a => a.Id == actionUserId).FirstOrDefault();
            var actionString = (actionnUser != null ? actionnUser.DisplayName : "");
            var attestationTasks = (from task in CurrentContext.Tasks
                                    join attestattion in CurrentContext.Attestations
                                    on task.AttestationId equals attestattion.Id
                                    join wfConfig in CurrentContext.WorkflowConfigurations
                                    on attestattion.WorkflowConfigurationId equals wfConfig.Id
                                    join attCampaign in CurrentContext.AttestationCampaigns
                                        on task.AttestationCampaignId equals attCampaign.Id
                                    join user in CurrentContext.User
                                        on task.ActionUserId equals user.Id
                                    join assignedby in CurrentContext.User
                                           on (long)task.AssignedBy equals assignedby.Id into Adetail
                                    join assignedUser in CurrentContext.User
                                      on (long)task.AssignedTo equals assignedUser.Id into detail
                                    from assignedUser in detail.DefaultIfEmpty()
                                    from assignedby in Adetail.DefaultIfEmpty()
                                    from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).DefaultIfEmpty()
                                    where task.AttestationId.HasValue &&
                                     (!dtFrom.HasValue || task.CreatedDate >= dtFrom.Value) && (!dtTo.HasValue || task.CreatedDate <= dtTo.Value) &&
                                    ((task.ActionUserId == actionUserId && !task.IsGroupTask) || ((task.IsGroupTask && task.AssignedTo == actionUserId)))
                                    select new PresentationTask
                                    {
                                        TaskId = task.Id,
                                        TaskSource = "Attestation",
                                        RequestTypeId = task.RequestTypeId,
                                        AttestationId = task.AttestationId,
                                        AttestationName = attestattion.AttestationName,
                                        AttestationCampaignId = task.AttestationCampaignId,
                                        CampaignName = attCampaign.CampaignName,
                                        EventName = attCampaign.CampaignName,
                                        CreatedDate = task.CreatedDate,
                                        IsActive = task.IsActive,
                                        IsCompleted = task.IsCompleted,
                                        ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                        UserName = String.Empty,
                                        AdHocAccessRequestId = 0,
                                        AccessRequestID = null, //requests.AccessRequestId,
                                        RequestedBy = "System",
                                        AssignedTo = task.AssignedTo,
                                        AssignedBy = task.AssignedBy,
                                        AccountType = AccountType.None.ToString(),
                                        IsDelegated = task.IsDelegated,
                                        IsGroup = task.IsGroupTask,
                                        RequestedFor = attestattion.AttestationName,
                                        AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                        AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                        AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                        StartDate = task.CreatedDate,
                                        EndDate = DbFunctions.AddDays(task.CreatedDate,
                                  (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault() == null ? 1 :
                                  task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().ResponseTimeInDays
                                  //+ (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().IsEscalationRequired ?
                                  //task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().DaysToWaitAfterEscalation : 0)
                                  )),
                                        RequestedDate = task.CreatedDate,
                                        Type = attCampaign.CampaignName,
                                        StatusType = task.Action.ToString(),
                                        RequestReason = string.Empty,
                                        DelegatedBy = task.DelegatedBy,
                                        DelegatedComment = task.DelegationComments,
                                        ActionUser = (task.IsGroupTask && !task.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : "") : user.DisplayName,
                                        ActionUserADID = (task.IsGroupTask && !task.IsDelegated) ? (assignedUser != null ? assignedUser.UserName : "") : user.UserName,
                                    }).Distinct().ToList();

            var provisioningTasks = (from tsk in CurrentContext.Tasks
                                     join acsrqst in currentAccessRequest
                                     on tsk.AccessRequestId equals acsrqst.Id
                                     join adhocrqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                                     on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                                     join rqsteduser in CurrentContext.User
                                        on adhocrqst.RequesterUserId equals rqsteduser.Id into requester
                                     join rqsteruser in CurrentContext.User.Include("NBusinessRole").Include("NOrganisation")
                                        on adhocrqst.RequesteeUserId equals rqsteruser.Id
                                     join assignedby in CurrentContext.User
                                            on (long)tsk.AssignedBy equals assignedby.Id into Adetail
                                     join assignedUser in CurrentContext.User
                                       on (long)tsk.AssignedTo equals assignedUser.Id into detail
                                     from rqsteduser in requester.DefaultIfEmpty()
                                     from assignedUser in detail.DefaultIfEmpty()
                                     from assignedby in Adetail.DefaultIfEmpty()
                                     join wfConfig in CurrentContext.WorkflowConfigurations
                                     on tsk.WorkflowConfigurationId equals wfConfig.Id
                                     from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == tsk.WorkflowStateId).DefaultIfEmpty()
                                         //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == tsk.WorkflowStateId
                                     where
                                     (!dtFrom.HasValue || tsk.CreatedDate >= dtFrom.Value) && (!dtTo.HasValue || tsk.CreatedDate <= dtTo.Value) &&
                                     (((tsk.ActionUserId == actionUserId && !tsk.IsGroupTask) || (tsk.IsGroupTask && tsk.AssignedTo == actionUserId)) && acsrqst.AdHocAccessRequestId.HasValue)
                                     select new PresentationTask
                                     {
                                         TaskId = tsk.Id,
                                         TaskSource = "Access Request",
                                         RequestTypeId = tsk.RequestTypeId,
                                         AttestationId = tsk.AttestationId,
                                         AttestationName = String.Empty,
                                         AttestationCampaignId = tsk.AttestationCampaignId,
                                         CampaignName = String.Empty,
                                         EventName = adhocrqst.EndPoint.DisplayName,
                                         ApplicationType = adhocrqst.EndPoint.ApplicationType,
                                         CreatedDate = tsk.CreatedDate,
                                         IsActive = tsk.IsActive,
                                         IsCompleted = tsk.IsCompleted,
                                         ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                         UserName = string.Empty,
                                         AdHocAccessRequestId = adhocrqst.Id,
                                         AccessRequestID = acsrqst.Id,
                                         RequestedBy = acsrqst.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.AdhocRequest ? (rqsteduser != null ? rqsteduser.DisplayName : "System") : "System",
                                         AssignedTo = tsk.AssignedTo,
                                         AssignedBy = tsk.AssignedBy,
                                         AccountType = adhocrqst.AccountType.ToString(),
                                         IsDelegated = tsk.IsDelegated,
                                         IsGroup = tsk.IsGroupTask,
                                         RequestedFor = rqsteruser.DisplayName,
                                         RequestForDept = rqsteruser.NOrganisation != null ? rqsteruser.NOrganisation.Name : "",
                                         RequestForDesignation = rqsteruser.NBusinessRole != null ? rqsteruser.NBusinessRole.Description : "",
                                         AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                         AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                         AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                         StartDate = adhocrqst.ValidFrom,
                                         EndDate = adhocrqst.ValidTo,
                                         RequestedDate = adhocrqst.CreatedDate,
                                         Type = ((int)adhocrqst.AdhocActionType == 1 ? "Add New Account" : ((int)adhocrqst.AdhocActionType == 2) ? "Amendment Add" : ((int)adhocrqst.AdhocActionType == 3 ? "Amendment Remove" : ((int)adhocrqst.AdhocActionType == 4 ? "Delete Account" : "None"))),

                                         //((int)adhocrqst.AdhocActionType == 2 || (int)adhocrqst.AdhocActionType == 3) ? ((int)adhocrqst.AdhocActionType == 2 ? "Add Role" : "Remove Role") : ((int)adhocrqst.AdhocActionType == 1 ? "Create Login-ID" : "Remove Login-ID"),
                                         StatusType = tsk.Action.ToString(),
                                         RequestReason = adhocrqst.Comments, //acsrqst.Comments ,
                                         DelegatedBy = tsk.DelegatedBy,
                                         DelegatedComment = tsk.DelegationComments,
                                         ActionUser = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : tsk.Approver) : actionString,
                                         ActionUserADID = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.UserName : "") : "",
                                         EntitlementXML = adhocrqst.EntitlementDetail
                                     }).Distinct().ToList();

            var provisioningTasksFromBulkRequest = (from tsk in CurrentContext.Tasks
                                                    join blkReq in currentBulk.Include("BulkRequestDetails").Include("EndPoint")
                                                    on tsk.BulkRequestId equals blkReq.Id
                                                    join rqsteduser in CurrentContext.User
                                                    on blkReq.RequestedBy equals rqsteduser.Id into requester
                                                    from rqsteduser in requester.DefaultIfEmpty()
                                                    join rqsteruser in CurrentContext.User.Include("NBusinessRole").Include("NOrganisation")
                                                    on blkReq.SelectedSupervisor equals rqsteruser.Id
                                                    join assignedby in CurrentContext.User
                                                           on (long)tsk.AssignedBy equals assignedby.Id into Adetail
                                                    join assignedUser in CurrentContext.User
                                                      on (long)tsk.AssignedTo equals assignedUser.Id into detail
                                                    from assignedUser in detail.DefaultIfEmpty()
                                                    from assignedby in Adetail.DefaultIfEmpty()
                                                    join wfConfig in CurrentContext.WorkflowConfigurations
                                                    on tsk.WorkflowConfigurationId equals wfConfig.Id
                                                    from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == tsk.WorkflowStateId).DefaultIfEmpty()
                                                    where
                                                     (!dtFrom.HasValue || tsk.CreatedDate >= dtFrom.Value) && (!dtTo.HasValue || tsk.CreatedDate <= dtTo.Value) &&

                                                    (((tsk.ActionUserId == actionUserId && !tsk.IsGroupTask) || ((tsk.IsGroupTask && tsk.AssignedTo == actionUserId))))
                                                    select new PresentationTask
                                                    {
                                                        TaskId = tsk.Id,
                                                        RequestTypeId = tsk.RequestTypeId,
                                                        TaskSource = DI.Chakra.Infrastructure.Constants.BulkRequest,
                                                        AttestationId = tsk.AttestationId,
                                                        AttestationName = string.Empty,
                                                        AttestationCampaignId = tsk.AttestationCampaignId,
                                                        CampaignName = String.Empty,
                                                        EventName = blkReq.EndPoint.DisplayName,
                                                        ApplicationType = blkReq.EndPoint.ApplicationType,
                                                        CreatedDate = tsk.CreatedDate,
                                                        IsActive = tsk.IsActive,
                                                        IsCompleted = tsk.IsCompleted,
                                                        ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                                        UserName = string.Empty,
                                                        AdHocAccessRequestId = null,
                                                        AccessRequestID = blkReq.Id,
                                                        RequestedBy = blkReq.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.AdhocRequest ? (rqsteduser != null ? rqsteduser.DisplayName : "System") : "System",
                                                        AssignedTo = tsk.AssignedTo,
                                                        AssignedBy = tsk.AssignedBy,
                                                        AccountType = "User",
                                                        IsDelegated = tsk.IsDelegated,
                                                        IsGroup = tsk.IsGroupTask,
                                                        RequestedFor = "Bulk Request",
                                                        RequestForDept = rqsteruser.NOrganisation != null ? rqsteruser.NOrganisation.Name : "",
                                                        RequestForDesignation = rqsteruser.NBusinessRole != null ? rqsteruser.NBusinessRole.Description : "",
                                                        AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                                        AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                                        AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                                        StartDate = blkReq.ActivationDate,
                                                        EndDate = blkReq.DeactivationDate,
                                                        RequestedDate = blkReq.CreatedDate,

                                                        Type = ((int)blkReq.AdhocActionType == 1 ? "Add New Account" : ((int)blkReq.AdhocActionType == 2) ? "Amendment Add" : ((int)blkReq.AdhocActionType == 3 ? "Amendment Remove" : ((int)blkReq.AdhocActionType == 4 ? "Delete Account" : "None"))),


                                                        StatusType = tsk.Action.ToString(),
                                                        RequestReason = blkReq.Comments, //acsrqst.Comments ,
                                                        DelegatedBy = tsk.DelegatedBy,
                                                        DelegatedComment = tsk.DelegationComments,
                                                        ActionUser = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : tsk.Approver) : actionString,
                                                        ActionUserADID = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.UserName : "") : "",
                                                        BulkReqDetails = blkReq.BulkRequestDetails,
                                                        EntitlementXML = blkReq.EntitlementDetail
                                                    }).ToList();
            IQueryable<PresentationTask> nonEmployeeEventTasksQuery = null;
            if (endpointSystemId == 0)
                nonEmployeeEventTasksQuery = (from tsk in CurrentContext.Tasks
                                              join trigger in CurrentContext.EmployeeEventTriggers
                                              on tsk.EmployeeEventTriggerId equals trigger.Id
                                              join rqsteduser in CurrentContext.User
                                              on trigger.CreatedBy equals rqsteduser.UserName
                                              join rqsteruser in CurrentContext.User
                                                 on trigger.UserId equals rqsteruser.Id
                                              join assignedby in CurrentContext.User
                                                     on tsk.AssignedBy equals assignedby.Id into Adetail
                                              join assignedUser in CurrentContext.User
                                                on tsk.AssignedTo equals assignedUser.Id into detail
                                              from assignedUser in detail.DefaultIfEmpty()
                                              from assignedby in Adetail.DefaultIfEmpty()
                                              where
                                              (!dtFrom.HasValue || tsk.CreatedDate >= dtFrom.Value) && (!dtTo.HasValue || tsk.CreatedDate <= dtTo.Value) &&
                                              ((((tsk.ActionUserId == actionUserId && !tsk.IsGroupTask) || ((tsk.IsGroupTask && tsk.AssignedTo == actionUserId))) && tsk.EmployeeEventTriggerId.HasValue) && (trigger.EventType == EmployeeEvents.NonEmployeeExtension || trigger.EventType == EmployeeEvents.NonEmployeeCreation))
                                              select new PresentationTask
                                              {
                                                  TaskId = tsk.Id,
                                                  RequestTypeId = tsk.RequestTypeId,
                                                  TaskSource = "Employee Creation/Extension/Shortening",
                                                  AttestationId = tsk.AttestationId,
                                                  AttestationName = string.Empty,
                                                  EventName = trigger.EventType == EmployeeEvents.NonEmployeeCreation ? "Non Employee Creation" : (trigger.EventType == EmployeeEvents.NonEmployeeExtension ? "Non Employee Extension / Shortening" : string.Empty),
                                                  CreatedDate = tsk.CreatedDate,
                                                  IsActive = tsk.IsActive,
                                                  IsCompleted = tsk.IsCompleted,
                                                  ReponseTimeInDays = 0,
                                                  UserName = string.Empty,
                                                  AdHocAccessRequestId = null,
                                                  AccessRequestID = null,
                                                  RequestedBy = rqsteduser.DisplayName,
                                                  AssignedTo = tsk.AssignedTo,
                                                  AssignedBy = tsk.AssignedBy,
                                                  AccountType = AccountType.None.ToString(),
                                                  IsDelegated = tsk.IsDelegated,
                                                  IsGroup = tsk.IsGroupTask,
                                                  RequestedFor = rqsteruser.DisplayName,
                                                  AssignedToName = assignedUser.DisplayName,
                                                  AssignedByName = assignedby.DisplayName,
                                                  StatusType = tsk.Action.ToString(),
                                                  //StartDate = tsk.AdHocAccessRequest.ValidFrom,
                                                  //EndDate = tsk.AdHocAccessRequest.ValidTo,
                                                  RequestedDate = trigger.CreatedDate,
                                                  Type = trigger.EventType == EmployeeEvents.NonEmployeeCreation ? "Non Employee Creation" : (trigger.EventType == EmployeeEvents.NonEmployeeExtension ? "Non Employee Extension / Shortening" : string.Empty),
                                                  ActionUser = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : tsk.Approver) : actionString,
                                                  ActionUserADID = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.UserName : "") : "",
                                                  //,
                                                  //  EEmployeeEventTriggerId = tsk.EmployeeEventTriggerId,
                                              });

            foreach (var item in provisioningTasksFromBulkRequest)
            {
                item.RequestedFor = string.Join(",", item.BulkReqDetails.Select(i => i.Name));
            }
            var tasks = attestationTasks.Union(provisioningTasks).Union(provisioningTasksFromBulkRequest);
            if (nonEmployeeEventTasksQuery != null)
                tasks = tasks.Union(nonEmployeeEventTasksQuery);


            tasks = tasks.Distinct().OrderByDescending(rvw => rvw.CreatedDate).ThenBy(rvw1 => rvw1.IsCompleted);

            return tasks.ToList();
        }

        IQueryable<Task> getTasks(DateTime? dtFrom, DateTime? dtTo)
        {
            IQueryable<Task> tasks = CurrentContext.Tasks.Select(t => t);

            if (dtFrom != null)
                tasks = tasks.Where(t => t.CreatedDate >= dtFrom.Value).Select(t => t);

            if (dtTo != null)
                tasks = tasks.Where(t => t.CreatedDate <= dtTo.Value).Select(t => t);

            return tasks;
        }

        public IList<PresentationTask> GetTaskDetailsByTaskStatus(long actionUserId, StatusType taskStatus, DateTime? dtFrom, DateTime? dtTo, long endpointSystemId = 0)
        {
            var currentAccessRequest = from accReq in CurrentContext.AccessRequest
                                       where endpointSystemId == 0 ? true : accReq.EndpointSystemId == endpointSystemId
                                       select accReq;
            var currentBulk = from blk in CurrentContext.BulkRequests
                              where endpointSystemId == 0 ? true : blk.EndpointSystemId == endpointSystemId
                              select blk;

            var actionnUser = CurrentContext.User.Where(a => a.Id == actionUserId).FirstOrDefault();
            var actionString = (actionnUser != null ? actionnUser.DisplayName : "");



            var attestationTasksForUser = (from task in getTasks(dtFrom, dtTo)
                                           join attestattion in CurrentContext.Attestations
                                               on task.AttestationId equals attestattion.Id
                                           join attCampaign in CurrentContext.AttestationCampaigns
                                               on task.AttestationCampaignId equals attCampaign.Id
                                           join user in CurrentContext.User
                                               on task.ActionUserId equals user.Id
                                           join assignedby in CurrentContext.User
                                                  on (long)task.AssignedBy equals assignedby.Id into Adetail
                                           join assignedUser in CurrentContext.User
                                             on (long)task.AssignedTo equals assignedUser.Id into detail
                                           from assignedUser in detail.DefaultIfEmpty()
                                           from assignedby in Adetail.DefaultIfEmpty()
                                           join wfConfig in CurrentContext.WorkflowConfigurations
                                               on attestattion.WorkflowConfigurationId equals wfConfig.Id
                                           from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).DefaultIfEmpty()
                                               //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                           where (((task.ActionUserId == actionUserId && !task.IsGroupTask) || ((task.IsGroupTask && task.AssignedTo == actionUserId))) && task.Action == taskStatus)
                                           where !task.IsGroupTask
                                           select new PresentationTask
                                           {
                                               TaskId = task.Id,
                                               TaskSource = "Attestation",
                                               RequestTypeId = task.RequestTypeId,
                                               AttestationId = task.AttestationId,
                                               AttestationName = attestattion.AttestationName,
                                               AttestationCampaignId = task.AttestationCampaignId,
                                               CampaignName = attCampaign.CampaignName,
                                               EventName = attCampaign.CampaignName,
                                               CreatedDate = task.CreatedDate,
                                               IsActive = task.IsActive,
                                               IsCompleted = task.IsCompleted,
                                               ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                               AdHocAccessRequestId = 0,
                                               RequestedBy = "System",
                                               AssignedTo = task.AssignedTo,
                                               AssignedBy = task.AssignedBy,
                                               AccountType = AccountType.None.ToString(),
                                               IsDelegated = task.IsDelegated,
                                               IsGroup = task.IsGroupTask,
                                               RequestedFor = attestattion.AttestationName,
                                               AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                               AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                               AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                               StartDate = task.CreatedDate,
                                               EndDate = DbFunctions.AddDays(task.CreatedDate,
                                         (wfConfigDetail != null ? wfConfigDetail.ResponseTimeInDays : 1
                                         //+ (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().IsEscalationRequired ?
                                         //task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().DaysToWaitAfterEscalation : 0)
                                         )),
                                               TaskAccessRequest = DI.Chakra.Infrastructure.Enums.AccessRequestStatus.None,
                                               AccessRequestID = null,
                                               Type = attCampaign.CampaignName,
                                               StatusType = task.Action.ToString(),
                                               RequestReason = string.Empty,
                                               DelegatedBy = task.DelegatedBy,
                                               DelegatedComment = task.DelegationComments,
                                               UserName = user.UserName,
                                               RequestedDate = task.CreatedDate,
                                               ActionUser = (task.IsGroupTask && !task.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : "") : user.DisplayName,
                                               ActionUserADID = (task.IsGroupTask && !task.IsDelegated) ? (assignedUser != null ? assignedUser.UserName : "") : user.UserName,
                                           }).Distinct().ToList();


            var attestationTasksForGroup = (from task in getTasks(dtFrom, dtTo)
                                            join attestattion in CurrentContext.Attestations
                                                on task.AttestationId equals attestattion.Id
                                            join attCampaign in CurrentContext.AttestationCampaigns
                                                on task.AttestationCampaignId equals attCampaign.Id
                                            join grp in CurrentContext.Group
                                                on task.ActionGroupId equals grp.Id
                                            join assignedby in CurrentContext.User
                                                   on (long)task.AssignedBy equals assignedby.Id into Adetail
                                            join assignedUser in CurrentContext.User
                                              on (long)task.AssignedTo equals assignedUser.Id into detail
                                            from assignedUser in detail.DefaultIfEmpty()
                                            from assignedby in Adetail.DefaultIfEmpty()
                                            join wfConfig in CurrentContext.WorkflowConfigurations
                                                on attestattion.WorkflowConfigurationId equals wfConfig.Id
                                            from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).DefaultIfEmpty()
                                                //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                            where (((task.ActionUserId == actionUserId && !task.IsGroupTask) || ((task.IsGroupTask && task.AssignedTo == actionUserId))) && task.Action == taskStatus)
                                            where task.IsGroupTask
                                            select new PresentationTask
                                            {
                                                TaskId = task.Id,
                                                TaskSource = "Attestation",
                                                RequestTypeId = task.RequestTypeId,
                                                AttestationId = task.AttestationId,
                                                AttestationName = attestattion.AttestationName,
                                                AttestationCampaignId = task.AttestationCampaignId,
                                                CampaignName = attCampaign.CampaignName,
                                                EventName = attCampaign.CampaignName,
                                                CreatedDate = task.CreatedDate,
                                                IsActive = task.IsActive,
                                                IsCompleted = task.IsCompleted,
                                                ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                                AdHocAccessRequestId = 0,
                                                RequestedBy = "System",
                                                AssignedTo = task.AssignedTo,
                                                AssignedBy = task.AssignedBy,
                                                AccountType = AccountType.None.ToString(),
                                                IsDelegated = task.IsDelegated,
                                                IsGroup = task.IsGroupTask,
                                                RequestedFor = grp.GroupName,
                                                AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                                AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                                AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                                StartDate = task.CreatedDate,
                                                EndDate = DbFunctions.AddDays(task.CreatedDate,
                                  (wfConfigDetail != null ? wfConfigDetail.ResponseTimeInDays : 1
                                  //+ (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().IsEscalationRequired ?
                                  //task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().DaysToWaitAfterEscalation : 0)
                                  )),
                                                TaskAccessRequest = DI.Chakra.Infrastructure.Enums.AccessRequestStatus.None,
                                                AccessRequestID = null,
                                                Type = attCampaign.CampaignName,
                                                StatusType = task.Action.ToString(),
                                                RequestReason = string.Empty,
                                                DelegatedBy = task.DelegatedBy,
                                                DelegatedComment = task.DelegationComments,
                                                UserName = task.Approver,
                                                RequestedDate = task.CreatedDate,
                                                ActionUser = (task.IsGroupTask && !task.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : "") : actionString,
                                                ActionUserADID = (task.IsGroupTask && !task.IsDelegated) ? (assignedUser != null ? assignedUser.UserName : "") : "",
                                            }).Distinct().ToList();

            // var attestationTasks = attestationTasksForGroup.Union(attestationTasksForUser);//.ToList();

            var provisioningTasksQuery = (from tsk in getTasks(dtFrom, dtTo)
                                          join acsrqst in currentAccessRequest
                                     on tsk.AccessRequestId equals acsrqst.Id
                                          join adhocrqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                                          on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                                          join rqsteduser in CurrentContext.User
                                              on adhocrqst.RequesterUserId equals rqsteduser.Id into requester
                                          from rqsteduser in requester.DefaultIfEmpty()
                                          join rqsteruser in CurrentContext.User.Include("NBusinessRole").Include("NOrganisation")
                                             on adhocrqst.RequesteeUserId equals rqsteruser.Id
                                          join assignedby in CurrentContext.User
                                                 on (long)tsk.AssignedBy equals assignedby.Id into Adetail
                                          join assignedUser in CurrentContext.User
                                            on (long)tsk.AssignedTo equals assignedUser.Id into detail
                                          from assignedUser in detail.DefaultIfEmpty()
                                          from assignedby in Adetail.DefaultIfEmpty()
                                          join wfConfig in CurrentContext.WorkflowConfigurations
                                          on tsk.WorkflowConfigurationId equals wfConfig.Id
                                          from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == tsk.WorkflowStateId).DefaultIfEmpty()
                                              //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == tsk.WorkflowStateId
                                              //where (tsk.ActionUserId == actionUserId && acsrqst.AdHocAccessRequestId.HasValue && tsk.Action == taskStatus && (!tsk.IsGroupTask || (tsk.IsGroupTask && tsk.AssignedTo == actionUserId)))
                                          where

                                          (((tsk.ActionUserId == actionUserId && !tsk.IsGroupTask) || ((tsk.IsGroupTask && tsk.AssignedTo == actionUserId))) && tsk.Action == taskStatus && acsrqst.AdHocAccessRequestId.HasValue)
                                          select new PresentationTask
                                          {
                                              TaskId = tsk.Id,
                                              RequestTypeId = tsk.RequestTypeId,
                                              TaskSource = "Access Request",
                                              AttestationId = tsk.AttestationId,
                                              AttestationName = string.Empty,
                                              AttestationCampaignId = tsk.AttestationCampaignId,
                                              CampaignName = String.Empty,
                                              EventName = adhocrqst.EndPoint.DisplayName,
                                              ApplicationType = adhocrqst.EndPoint.ApplicationType,
                                              CreatedDate = tsk.CreatedDate,
                                              IsActive = tsk.IsActive,
                                              IsCompleted = tsk.IsCompleted,
                                              ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                              UserName = string.Empty,
                                              AdHocAccessRequestId = adhocrqst.Id,
                                              AccessRequestID = acsrqst.Id,
                                              RequestedBy = acsrqst.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.AdhocRequest ? (rqsteduser != null ? rqsteduser.DisplayName : "System") : "System",
                                              AssignedTo = tsk.AssignedTo,
                                              AssignedBy = tsk.AssignedBy,
                                              AccountType = adhocrqst.AccountType.ToString(),
                                              IsDelegated = tsk.IsDelegated,
                                              IsGroup = tsk.IsGroupTask,
                                              RequestedFor = rqsteruser.DisplayName,
                                              RequestForDept = rqsteruser.NOrganisation != null ? rqsteruser.NOrganisation.Name : "",
                                              RequestForDesignation = rqsteruser.NBusinessRole != null ? rqsteruser.NBusinessRole.Description : "",
                                              AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                              AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                              AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                              StartDate = adhocrqst.ValidFrom,
                                              EndDate = adhocrqst.ValidTo,
                                              RequestedDate = adhocrqst.CreatedDate,
                                              Type = ((int)adhocrqst.AdhocActionType == 1 ? "Add New Account" : ((int)adhocrqst.AdhocActionType == 2) ? "Amendment Add" : ((int)adhocrqst.AdhocActionType == 3 ? "Amendment Remove" : ((int)adhocrqst.AdhocActionType == 4 ? "Delete Account" : "None"))),
                                              StatusType = tsk.Action.ToString(),
                                              RequestReason = adhocrqst.Comments, //acsrqst.Comments ,
                                              DelegatedBy = tsk.DelegatedBy,
                                              DelegatedComment = tsk.DelegationComments,
                                              ActionUser = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : tsk.Approver) : actionString,
                                              ActionUserADID = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.UserName : "") : "",
                                              EntitlementXML = adhocrqst.EntitlementDetail
                                          });

            var provisioningTasks = provisioningTasksQuery.Distinct().ToList();
            var provisioningTasksFromBulkRequest = (from tsk in getTasks(dtFrom, dtTo)
                                                    join blkReq in currentBulk.Include("BulkRequestDetails").Include("EndPoint")
                                                    on tsk.BulkRequestId equals blkReq.Id
                                                    join rqsteduser in CurrentContext.User
                                                    on blkReq.RequestedBy equals rqsteduser.Id into requester
                                                    from rqsteduser in requester.DefaultIfEmpty()
                                                    join rqsteruser in CurrentContext.User.Include("NBusinessRole").Include("NOrganisation")
                                                    on blkReq.SelectedSupervisor equals rqsteruser.Id
                                                    join assignedby in CurrentContext.User
                                                           on (long)tsk.AssignedBy equals assignedby.Id into Adetail
                                                    join assignedUser in CurrentContext.User
                                                      on (long)tsk.AssignedTo equals assignedUser.Id into detail
                                                    from assignedUser in detail.DefaultIfEmpty()
                                                    from assignedby in Adetail.DefaultIfEmpty()
                                                    join wfConfig in CurrentContext.WorkflowConfigurations
                                                    on tsk.WorkflowConfigurationId equals wfConfig.Id
                                                    from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == tsk.WorkflowStateId).DefaultIfEmpty()
                                                        //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == tsk.WorkflowStateId
                                                        //where (tsk.ActionUserId == actionUserId && acsrqst.AdHocAccessRequestId.HasValue && tsk.Action == taskStatus && (!tsk.IsGroupTask || (tsk.IsGroupTask && tsk.AssignedTo == actionUserId)))
                                                    where

                                                    (((tsk.ActionUserId == actionUserId && !tsk.IsGroupTask) || ((tsk.IsGroupTask && tsk.AssignedTo == actionUserId))) && tsk.Action == taskStatus)
                                                    select new PresentationTask
                                                    {
                                                        TaskId = tsk.Id,
                                                        RequestTypeId = tsk.RequestTypeId,
                                                        TaskSource = DI.Chakra.Infrastructure.Constants.BulkRequest,
                                                        AttestationId = tsk.AttestationId,
                                                        AttestationName = string.Empty,
                                                        AttestationCampaignId = tsk.AttestationCampaignId,
                                                        CampaignName = String.Empty,
                                                        EventName = blkReq.EndPoint.DisplayName,
                                                        ApplicationType = blkReq.EndPoint.ApplicationType,
                                                        CreatedDate = tsk.CreatedDate,
                                                        IsActive = tsk.IsActive,
                                                        IsCompleted = tsk.IsCompleted,
                                                        ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                                        UserName = string.Empty,
                                                        AdHocAccessRequestId = null,
                                                        AccessRequestID = blkReq.Id,
                                                        RequestedBy = blkReq.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.AdhocRequest ? (rqsteduser != null ? rqsteduser.DisplayName : "System") : "System",
                                                        AssignedTo = tsk.AssignedTo,
                                                        AssignedBy = tsk.AssignedBy,
                                                        AccountType = "User",
                                                        IsDelegated = tsk.IsDelegated,
                                                        IsGroup = tsk.IsGroupTask,
                                                        RequestedFor = "Bulk Request",
                                                        RequestForDept = rqsteruser.NOrganisation != null ? rqsteruser.NOrganisation.Name : "",
                                                        RequestForDesignation = rqsteruser.NBusinessRole != null ? rqsteruser.NBusinessRole.Description : "",
                                                        AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                                        AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                                        AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                                        StartDate = blkReq.ActivationDate,
                                                        EndDate = blkReq.DeactivationDate,
                                                        RequestedDate = blkReq.CreatedDate,
                                                        Type = ((int)blkReq.AdhocActionType == 1 ? "Add New Account" : ((int)blkReq.AdhocActionType == 2) ? "Amendment Add" : ((int)blkReq.AdhocActionType == 3 ? "Amendment Remove" : ((int)blkReq.AdhocActionType == 4 ? "Delete Account" : "None"))),
                                                        StatusType = tsk.Action.ToString(),
                                                        RequestReason = blkReq.Comments, //acsrqst.Comments ,
                                                        DelegatedBy = tsk.DelegatedBy,
                                                        DelegatedComment = tsk.DelegationComments,
                                                        ActionUser = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : tsk.Approver) : actionString,
                                                        ActionUserADID = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.UserName : "") : "",
                                                        BulkReqDetails = blkReq.BulkRequestDetails,
                                                        EntitlementXML = blkReq.EntitlementDetail
                                                    }).ToList();


            foreach (var item in provisioningTasksFromBulkRequest)
            {
                item.RequestedFor = string.Join(",", item.BulkReqDetails.Select(i => i.Name));
            }

            // List<PresentationTask> lst = provisioningTasks.ToList();
            IQueryable<PresentationTask> nonEmployeeEventTasksQuery = null;
            if (endpointSystemId == 0)
                nonEmployeeEventTasksQuery = (from tsk in getTasks(dtFrom, dtTo)
                                              join trigger in CurrentContext.EmployeeEventTriggers
                                              on tsk.EmployeeEventTriggerId equals trigger.Id
                                              join rqsteduser in CurrentContext.User
                                              on trigger.CreatedBy equals rqsteduser.UserName
                                              join rqsteruser in CurrentContext.User
                                                 on trigger.UserId equals rqsteruser.Id
                                              join assignedby in CurrentContext.User
                                                     on tsk.AssignedBy equals assignedby.Id into Adetail
                                              join assignedUser in CurrentContext.User
                                                on tsk.AssignedTo equals assignedUser.Id into detail
                                              from assignedUser in detail.DefaultIfEmpty()
                                              from assignedby in Adetail.DefaultIfEmpty()
                                              where

                                              ((((tsk.ActionUserId == actionUserId && !tsk.IsGroupTask) || ((tsk.IsGroupTask && tsk.AssignedTo == actionUserId))) && tsk.Action == taskStatus && tsk.EmployeeEventTriggerId.HasValue) && (trigger.EventType == EmployeeEvents.NonEmployeeExtension || trigger.EventType == EmployeeEvents.NonEmployeeCreation))
                                              select new PresentationTask
                                              {
                                                  TaskId = tsk.Id,
                                                  RequestTypeId = tsk.RequestTypeId,
                                                  TaskSource = "Employee Creation",
                                                  AttestationId = tsk.AttestationId,
                                                  AttestationName = string.Empty,
                                                  EventName = trigger.EventType == EmployeeEvents.NonEmployeeCreation ? "Non Employee Creation" : (trigger.EventType == EmployeeEvents.NonEmployeeExtension ? "Non Employee Extension / Shortening" : string.Empty),
                                                  CreatedDate = tsk.CreatedDate,
                                                  IsActive = tsk.IsActive,
                                                  IsCompleted = tsk.IsCompleted,
                                                  ReponseTimeInDays = 0,
                                                  UserName = string.Empty,
                                                  AdHocAccessRequestId = null,
                                                  AccessRequestID = null,
                                                  RequestedBy = rqsteduser.DisplayName,
                                                  AssignedTo = tsk.AssignedTo,
                                                  AssignedBy = tsk.AssignedBy,
                                                  AccountType = AccountType.None.ToString(),
                                                  IsDelegated = tsk.IsDelegated,
                                                  IsGroup = tsk.IsGroupTask,
                                                  RequestedFor = rqsteruser.DisplayName,
                                                  AssignedToName = assignedUser.DisplayName,
                                                  AssignedByName = assignedby.DisplayName,
                                                  StatusType = tsk.Action.ToString(),
                                                  //StartDate = tsk.AdHocAccessRequest.ValidFrom,
                                                  //EndDate = tsk.AdHocAccessRequest.ValidTo,
                                                  RequestedDate = trigger.CreatedDate,
                                                  Type = trigger.EventType == EmployeeEvents.NonEmployeeCreation ? "Non Employee Creation" : (trigger.EventType == EmployeeEvents.NonEmployeeExtension ? "Non Employee Extension / Shortening" : string.Empty),
                                                  ActionUser = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : tsk.Approver) : actionString,
                                                  ActionUserADID = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.UserName : "") : ""
                                                  //,
                                                  //  EEmployeeEventTriggerId = tsk.EmployeeEventTriggerId,
                                              });

            var nonEmployeeEventTasks = nonEmployeeEventTasksQuery?.ToList();

            var tasks = attestationTasksForUser.Union(attestationTasksForGroup).Union(provisioningTasks).Union(provisioningTasksFromBulkRequest);

            if (nonEmployeeEventTasks != null)
                tasks = tasks.Union(nonEmployeeEventTasks);

            tasks = tasks.Distinct().OrderByDescending(rvw => rvw.CreatedDate).ThenBy(rvw1 => rvw1.IsCompleted);


            return tasks.ToList();
        }
        public PresentationGridPagedData<PresentationTask> GetGroupTaskDetailsByActionUserId(long actionUserId, StatusType? type, int startRowIndex, int maximumRows, string filterExpression, string sortExpression, long requestFor = 0, DateTime? startDate = null, DateTime? endDate = null, long endpointId = 0)
        {
            PresentationGridPagedData<PresentationTask> dataResult = new PresentationGridPagedData<PresentationTask>();

            var actionnUser = CurrentContext.User.Where(a => a.Id == actionUserId).FirstOrDefault();

            var actionString = (actionnUser != null ? actionnUser.DisplayName : "");
            List<long> userGroups = new List<long>();
            if (requestFor == 0)
                userGroups = CurrentContext.User.Include("Groups").FirstOrDefault(item => item.Id == actionUserId).Groups.Select(item => item.Id).ToList();
            else
            {
                userGroups = CurrentContext.Group.Select(i => i.Id).ToList();
            }
            IQueryable<Task> currentTask = from tsk in CurrentContext.Tasks
                                           select tsk;

            if (startDate != null && endDate != null)
            {
                currentTask = currentTask.Where(i => i.CreatedDate >= startDate && i.CreatedDate <= endDate);
            }
            IQueryable<PresentationTask> attestationTasks = null;
            if (requestFor == 0)
            {

                attestationTasks = (from task in currentTask
                                    join attestattion in CurrentContext.Attestations
                                        on task.AttestationId equals attestattion.Id
                                    join attCampaign in CurrentContext.AttestationCampaigns
                                        on task.AttestationCampaignId equals attCampaign.Id
                                    join grp in CurrentContext.Group
                                               on task.ActionGroupId equals grp.Id
                                    join assignedby in CurrentContext.User
                                           on (long)task.AssignedBy equals assignedby.Id into Adetail
                                    join assignedUser in CurrentContext.User
                                      on (long)task.AssignedTo equals assignedUser.Id into detail
                                    from assignedUser in detail.DefaultIfEmpty()
                                    from assignedby in Adetail.DefaultIfEmpty()
                                    join wfConfig in CurrentContext.WorkflowConfigurations
                                        on attestattion.WorkflowConfigurationId equals wfConfig.Id
                                    from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).DefaultIfEmpty()
                                    where (userGroups.Contains((int)task.ActionGroupId.Value) && task.IsGroupTask && (type == null ? true : task.Action == type.Value)
                                    )
                                    select new PresentationTask
                                    {
                                        TaskId = task.Id,
                                        RequestTypeId = task.RequestTypeId,
                                        TaskSource = "Attestation",
                                        AttestationId = task.AttestationId,
                                        AttestationName = attestattion.AttestationName,
                                        AttestationCampaignId = task.AttestationCampaignId,
                                        CampaignName = attCampaign.CampaignName,
                                        EventName = attCampaign.CampaignName,
                                        ApplicationType = ApplicationType.None,
                                        CreatedDate = task.CreatedDate,
                                        IsActive = task.IsActive,
                                        IsCompleted = task.IsCompleted,
                                        ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                        UserName = string.Empty,
                                        AdHocAccessRequestId = 0,
                                        AccessRequestID = null, //requests.AccessRequestId,
                                        RequestedBy = "System",
                                        AssignedTo = task.AssignedTo,
                                        AssignedBy = task.AssignedBy,
                                        DelegatedBy = task.DelegatedBy,
                                        AccountType = AccountType.None.ToString(),
                                        IsDelegated = task.IsDelegated,
                                        IsGroup = task.IsGroupTask,
                                        RequestedFor = attestattion.AttestationName,
                                        RequestForDept = "",
                                        AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                        AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                        AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                        StartDate = task.CreatedDate,
                                        EndDate = DbFunctions.AddDays(task.CreatedDate,
                                        (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault() == null ? 1 :
                                        task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().ResponseTimeInDays
                                        )),
                                        RequestedDate = task.CreatedDate,
                                        Type = attCampaign.CampaignName,
                                        ActionUser = (task.IsGroupTask && !task.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : "") : actionString,
                                        StatusType = task.Action.ToString(),
                                        RequestReason = "",
                                        DelegatedComment = "",
                                        EntitlementXML = null,
                                        ActionUserADID = "",
                                        CompletionDate = task.ActionDate
                                    });
            }
            var provisioningTasks = (from tsk in currentTask
                                     join acsrqst in CurrentContext.AccessRequest
                                     on tsk.AccessRequestId equals acsrqst.Id
                                     join adhocrqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                                     on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                                     join rqsteduser in CurrentContext.User
                                         on adhocrqst.RequesterUserId equals rqsteduser.Id into requester
                                     from rqsteduser in requester.DefaultIfEmpty()
                                     join rqsteruser in CurrentContext.User.Include("NBusinessRole").Include("NOrganisation")
                                        on adhocrqst.RequesteeUserId equals rqsteruser.Id
                                     join assignedby in CurrentContext.User
                                            on (long)tsk.AssignedBy equals assignedby.Id into Adetail
                                     join assignedUser in CurrentContext.User
                                       on (long)tsk.AssignedTo equals assignedUser.Id into detail
                                     from assignedUser in detail.DefaultIfEmpty()
                                     from assignedby in Adetail.DefaultIfEmpty()
                                     join wfConfig in CurrentContext.WorkflowConfigurations
                                     on tsk.WorkflowConfigurationId equals wfConfig.Id
                                     from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == tsk.WorkflowStateId).DefaultIfEmpty()
                                     where (userGroups.Contains((Int32)tsk.ActionGroupId.Value) && acsrqst.AdHocAccessRequestId.HasValue && tsk.IsGroupTask
                                     && (type == null ? true : tsk.Action == type.Value)
                                     ) && (requestFor > 0 ? (rqsteruser != null ? rqsteruser.Id == requestFor : true) : true)
                                     && (endpointId > 0 ? acsrqst.EndpointSystemId == endpointId : true)
                                     select new PresentationTask
                                     {
                                         TaskId = tsk.Id,
                                         RequestTypeId = tsk.RequestTypeId,
                                         TaskSource = "Access Request",
                                         AttestationId = tsk.AttestationId,
                                         AttestationName = string.Empty,
                                         AttestationCampaignId = tsk.AttestationCampaignId,
                                         CampaignName = String.Empty,
                                         EventName = adhocrqst.EndPoint.DisplayName,
                                         ApplicationType = adhocrqst.EndPoint.ApplicationType,
                                         CreatedDate = tsk.CreatedDate,
                                         IsActive = tsk.IsActive,
                                         IsCompleted = tsk.IsCompleted,
                                         ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                         UserName = string.Empty,
                                         AdHocAccessRequestId = adhocrqst.Id,
                                         AccessRequestID = acsrqst.Id,
                                         RequestedBy = acsrqst.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.AdhocRequest ? (rqsteduser != null ? rqsteduser.DisplayName : "System") : "System",
                                         AssignedTo = tsk.AssignedTo,
                                         AssignedBy = tsk.AssignedBy,
                                         DelegatedBy = tsk.DelegatedBy,
                                         AccountType = adhocrqst.AccountType.ToString(),
                                         IsDelegated = tsk.IsDelegated,
                                         IsGroup = tsk.IsGroupTask,
                                         RequestedFor = rqsteruser.DisplayName,
                                         RequestForDept = rqsteruser.NOrganisation != null ? rqsteruser.NOrganisation.Name : "",
                                         //RequestForDesignation = rqsteruser.NBusinessRole != null ? rqsteruser.NBusinessRole.Description : "",
                                         AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                         AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                         AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                         StartDate = adhocrqst.ValidFrom,
                                         EndDate = adhocrqst.ValidTo,
                                         RequestedDate = adhocrqst.CreatedDate,
                                         Type = ((int)adhocrqst.AdhocActionType == 1 ? "Add New Account" : ((int)adhocrqst.AdhocActionType == 2) ? "Amendment Add" : ((int)adhocrqst.AdhocActionType == 3 ? "Amendment Remove" : ((int)adhocrqst.AdhocActionType == 4 ? "Delete Account" : "None"))),
                                         ActionUser = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : "") : actionString,
                                         StatusType = tsk.Action.ToString(),
                                         RequestReason = string.Empty,
                                         DelegatedComment = tsk.DelegationComments,
                                         EntitlementXML = adhocrqst.EntitlementDetail,
                                         ActionUserADID = null,
                                         CompletionDate = tsk.ActionDate
                                     });

            var provisioningTasksFromBulkRequest = (from tsk in CurrentContext.Tasks
                                                    join blkReq in CurrentContext.BulkRequests.Include("BulkRequestDetails").Include("EndPoint")
                                                    on tsk.BulkRequestId equals blkReq.Id
                                                    join rqsteduser in CurrentContext.User
                                                    on blkReq.RequestedBy equals rqsteduser.Id into requester
                                                    from rqsteduser in requester.DefaultIfEmpty()
                                                    join rqsteruser in CurrentContext.User.Include("NBusinessRole").Include("NOrganisation")
                                                    on blkReq.SelectedSupervisor equals rqsteruser.Id
                                                    join assignedby in CurrentContext.User
                                                           on (long)tsk.AssignedBy equals assignedby.Id into Adetail
                                                    join assignedUser in CurrentContext.User
                                                      on (long)tsk.AssignedTo equals assignedUser.Id into detail
                                                    from assignedUser in detail.DefaultIfEmpty()
                                                    from assignedby in Adetail.DefaultIfEmpty()
                                                    join wfConfig in CurrentContext.WorkflowConfigurations
                                                    on tsk.WorkflowConfigurationId equals wfConfig.Id
                                                    from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == tsk.WorkflowStateId).DefaultIfEmpty()
                                                    where (userGroups.Contains((int)tsk.ActionGroupId.Value) && tsk.IsGroupTask
                                                    && (type == null ? true : tsk.Action == type.Value)
                                                    ) && (requestFor > 0 ? (rqsteruser != null ? blkReq.BulkRequestDetails.FirstOrDefault(j => j.UserId == requestFor) != null : true) : true)
                                                    && (endpointId > 0 ? blkReq.EndpointSystemId == endpointId : true)
                                                    select new PresentationTask
                                                    {
                                                        TaskId = tsk.Id,
                                                        RequestTypeId = tsk.RequestTypeId,
                                                        TaskSource = DI.Chakra.Infrastructure.Constants.BulkRequest,
                                                        AttestationId = tsk.AttestationId,
                                                        AttestationName = string.Empty,
                                                        AttestationCampaignId = tsk.AttestationCampaignId,
                                                        CampaignName = String.Empty,
                                                        EventName = blkReq.EndPoint.DisplayName,
                                                        ApplicationType = blkReq.EndPoint.ApplicationType,
                                                        CreatedDate = tsk.CreatedDate,
                                                        IsActive = tsk.IsActive,
                                                        IsCompleted = tsk.IsCompleted,
                                                        ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                                        UserName = string.Empty,
                                                        AdHocAccessRequestId = null,
                                                        AccessRequestID = blkReq.Id,
                                                        RequestedBy = blkReq.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.AdhocRequest ? (rqsteduser != null ? rqsteduser.DisplayName : "System") : "System",
                                                        AssignedTo = tsk.AssignedTo,
                                                        AssignedBy = tsk.AssignedBy,
                                                        DelegatedBy = tsk.DelegatedBy,
                                                        AccountType = "User",
                                                        IsDelegated = tsk.IsDelegated,
                                                        IsGroup = tsk.IsGroupTask,
                                                        //RequestedFor = "Bulk Request",
                                                        RequestedFor = "",
                                                        RequestForDept = rqsteruser.NOrganisation != null ? rqsteruser.NOrganisation.Name : "",
                                                        //RequestForDesignation = rqsteruser.NBusinessRole != null ? rqsteruser.NBusinessRole.Description : "",
                                                        AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                                        AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                                        AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                                        StartDate = blkReq.ActivationDate,
                                                        EndDate = blkReq.DeactivationDate,
                                                        RequestedDate = blkReq.CreatedDate,
                                                        Type = ((int)blkReq.AdhocActionType == 1 ? "Add New Account" : ((int)blkReq.AdhocActionType == 2) ? "Amendment Add" : ((int)blkReq.AdhocActionType == 3 ? "Amendment Remove" : ((int)blkReq.AdhocActionType == 4 ? "Delete Account" : "None"))),
                                                        ActionUser = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : "") : actionString,
                                                        StatusType = tsk.Action.ToString(),
                                                        RequestReason = blkReq.Comments, //acsrqst.Comments ,
                                                        DelegatedComment = tsk.DelegationComments,
                                                        EntitlementXML = blkReq.EntitlementDetail,
                                                        ActionUserADID = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.UserName : "") : "",
                                                        CompletionDate = tsk.ActionDate
                                                    });
            IQueryable<PresentationTask> query = null;
            if(attestationTasks != null)
                query = attestationTasks.Union(provisioningTasks).Union(provisioningTasksFromBulkRequest);
            else
            {
                query = provisioningTasks.Union(provisioningTasksFromBulkRequest);
            }
            var temp = new List<PresentationTask>();
            if (!string.IsNullOrWhiteSpace(filterExpression))
            {
                query = query.Where(filterExpression);
            }
            dataResult.TotalCount = query.Count();
            if (!string.IsNullOrWhiteSpace(sortExpression))
            {
                query = query.OrderBy(sortExpression);
            }
            else
            {
                query = query.OrderByDescending(i => i.CreatedDate).ThenBy(rvw1 => rvw1.IsCompleted);
            }

            if (maximumRows == -1)
                dataResult.Data = query.ToList();
            else
                dataResult.Data = query.Skip(startRowIndex).Take(maximumRows).ToList();
            return dataResult;
        }
        public IList<PresentationTask> GetGroupTaskDetailsByActionUserId(long actionUserId)
        {
            var actionnUser = CurrentContext.User.Where(a => a.Id == actionUserId).FirstOrDefault();
            var actionString = (actionnUser != null ? actionnUser.DisplayName : "");
            List<long> userGroups = CurrentContext.User.Include("Groups").FirstOrDefault(item => item.Id == actionUserId).Groups.Select(item => item.Id).ToList();
            var attestationTasks = (from task in CurrentContext.Tasks
                                    join attestattion in CurrentContext.Attestations
                                        on task.AttestationId equals attestattion.Id
                                    join attCampaign in CurrentContext.AttestationCampaigns
                                        on task.AttestationCampaignId equals attCampaign.Id
                                    join user in CurrentContext.User
                                        on task.ActionUserId equals user.Id
                                    join assignedby in CurrentContext.User
                                           on (long)task.AssignedBy equals assignedby.Id into Adetail
                                    join assignedUser in CurrentContext.User
                                      on (long)task.AssignedTo equals assignedUser.Id into detail
                                    from assignedUser in detail.DefaultIfEmpty()
                                    from assignedby in Adetail.DefaultIfEmpty()
                                    join wfConfig in CurrentContext.WorkflowConfigurations
                                        on attestattion.WorkflowConfigurationId equals wfConfig.Id
                                    from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).DefaultIfEmpty()
                                        //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                    where (userGroups.Contains((int)task.ActionGroupId.Value) && task.IsGroupTask // && (task.AssignedTo == null || (task.AssignedTo != null && task.AssignedTo != actionUserId))
                                    )
                                    select new PresentationTask
                                    {
                                        TaskId = task.Id,
                                        RequestTypeId = task.RequestTypeId,
                                        TaskSource = "Attestation",
                                        AttestationId = task.AttestationId,
                                        AttestationName = attestattion.AttestationName,
                                        AttestationCampaignId = task.AttestationCampaignId,
                                        CampaignName = attCampaign.CampaignName,
                                        EventName = string.Empty,
                                        CreatedDate = task.CreatedDate,
                                        IsActive = task.IsActive,
                                        IsCompleted = task.IsCompleted,
                                        ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                        UserName = string.Empty,
                                        AdHocAccessRequestId = 0,
                                        AccessRequestID = attestattion.Id, //requests.AccessRequestId,
                                        RequestedBy = string.Empty,
                                        AssignedTo = task.AssignedTo,
                                        AssignedBy = task.AssignedBy,
                                        DelegatedBy = task.DelegatedBy,
                                        AccountType = AccountType.None.ToString(),
                                        IsDelegated = task.IsDelegated,
                                        IsGroup = task.IsGroupTask,

                                        //where (tsk.ActionUserId == actionUserId && acsrqst.AdHocAccessRequestId.HasValue && tsk.Action == taskStatus && (!tsk.IsGroupTask || (tsk.IsGroupTask && tsk.AssignedTo == actionUserId)))
                                        RequestedFor = attestattion.AttestationName,
                                        AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                        AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                        AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                        StartDate = task.CreatedDate,
                                        EndDate = DbFunctions.AddDays(task.CreatedDate,
                                  (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault() == null ? 1 :
                                  task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().ResponseTimeInDays
                                  //+ (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().IsEscalationRequired ?
                                  //task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().DaysToWaitAfterEscalation : 0)
                                  )),
                                        RequestedDate = task.CreatedDate,
                                        Type = attCampaign.CampaignName,
                                        ActionUser = (task.IsGroupTask && !task.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : "") : actionString,
                                        StatusType = task.Action.ToString(),
                                    }).Distinct().ToList();

            var provisioningTasks = (from tsk in CurrentContext.Tasks
                                     join acsrqst in CurrentContext.AccessRequest
                                     on tsk.AccessRequestId equals acsrqst.Id
                                     join adhocrqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                                     on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                                     join rqsteduser in CurrentContext.User
                                         on adhocrqst.RequesterUserId equals rqsteduser.Id into requester
                                     from rqsteduser in requester.DefaultIfEmpty()
                                     join rqsteruser in CurrentContext.User.Include("NBusinessRole").Include("NOrganisation")
                                        on adhocrqst.RequesteeUserId equals rqsteruser.Id
                                     join assignedby in CurrentContext.User
                                            on (long)tsk.AssignedBy equals assignedby.Id into Adetail
                                     join assignedUser in CurrentContext.User
                                       on (long)tsk.AssignedTo equals assignedUser.Id into detail
                                     from assignedUser in detail.DefaultIfEmpty()
                                     from assignedby in Adetail.DefaultIfEmpty()
                                     join wfConfig in CurrentContext.WorkflowConfigurations
                                     on tsk.WorkflowConfigurationId equals wfConfig.Id
                                     from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == tsk.WorkflowStateId).DefaultIfEmpty()
                                         //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == tsk.WorkflowStateId
                                     where (userGroups.Contains((Int32)tsk.ActionGroupId.Value) && acsrqst.AdHocAccessRequestId.HasValue && tsk.IsGroupTask // && (tsk.AssignedTo == null || (tsk.AssignedTo != null && tsk.AssignedTo != actionUserId))
                                     )
                                     select new PresentationTask
                                     {
                                         TaskId = tsk.Id,
                                         RequestTypeId = tsk.RequestTypeId,
                                         TaskSource = "Access Request",
                                         AttestationId = tsk.AttestationId,
                                         AttestationName = string.Empty,
                                         AttestationCampaignId = tsk.AttestationCampaignId,
                                         CampaignName = String.Empty,
                                         EventName = adhocrqst.EndPoint.DisplayName,
                                         ApplicationType = adhocrqst.EndPoint.ApplicationType,
                                         CreatedDate = tsk.CreatedDate,
                                         IsActive = tsk.IsActive,
                                         IsCompleted = tsk.IsCompleted,
                                         ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                         UserName = string.Empty,
                                         AdHocAccessRequestId = adhocrqst.Id,
                                         AccessRequestID = acsrqst.Id,
                                         RequestedBy = acsrqst.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.AdhocRequest ? (rqsteduser != null ? rqsteduser.DisplayName : "System") : "System",
                                         AssignedTo = tsk.AssignedTo,
                                         AssignedBy = tsk.AssignedBy,
                                         DelegatedBy = tsk.DelegatedBy,
                                         AccountType = adhocrqst.AccountType.ToString(),
                                         IsDelegated = tsk.IsDelegated,
                                         IsGroup = tsk.IsGroupTask,
                                         RequestedFor = rqsteruser.DisplayName,
                                         RequestForDept = rqsteruser.NOrganisation != null ? rqsteruser.NOrganisation.Name : "",
                                         RequestForDesignation = rqsteruser.NBusinessRole != null ? rqsteruser.NBusinessRole.Description : "",
                                         AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                         AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                         AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                         StartDate = adhocrqst.ValidFrom,
                                         EndDate = adhocrqst.ValidTo,
                                         RequestedDate = adhocrqst.CreatedDate,
                                         Type = ((int)adhocrqst.AdhocActionType == 1 ? "Add New Account" : ((int)adhocrqst.AdhocActionType == 2) ? "Amendment Add" : ((int)adhocrqst.AdhocActionType == 3 ? "Amendment Remove" : ((int)adhocrqst.AdhocActionType == 4 ? "Delete Account" : "None"))),
                                         ActionUser = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : "") : actionString,
                                         StatusType = tsk.Action.ToString(),
                                         EntitlementXML = adhocrqst.EntitlementDetail
                                     }).Distinct().ToList();

            var provisioningTasksFromBulkRequest = (from tsk in CurrentContext.Tasks
                                                    join blkReq in CurrentContext.BulkRequests.Include("BulkRequestDetails").Include("EndPoint")
                                                    on tsk.BulkRequestId equals blkReq.Id
                                                    join rqsteduser in CurrentContext.User
                                                    on blkReq.RequestedBy equals rqsteduser.Id into requester
                                                    from rqsteduser in requester.DefaultIfEmpty()
                                                    join rqsteruser in CurrentContext.User.Include("NBusinessRole").Include("NOrganisation")
                                                    on blkReq.SelectedSupervisor equals rqsteruser.Id
                                                    join assignedby in CurrentContext.User
                                                           on (long)tsk.AssignedBy equals assignedby.Id into Adetail
                                                    join assignedUser in CurrentContext.User
                                                      on (long)tsk.AssignedTo equals assignedUser.Id into detail
                                                    from assignedUser in detail.DefaultIfEmpty()
                                                    from assignedby in Adetail.DefaultIfEmpty()
                                                    join wfConfig in CurrentContext.WorkflowConfigurations
                                                    on tsk.WorkflowConfigurationId equals wfConfig.Id
                                                    from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == tsk.WorkflowStateId).DefaultIfEmpty()
                                                    where (userGroups.Contains((int)tsk.ActionGroupId.Value) && tsk.IsGroupTask)
                                                    select new PresentationTask
                                                    {
                                                        TaskId = tsk.Id,
                                                        RequestTypeId = tsk.RequestTypeId,
                                                        TaskSource = DI.Chakra.Infrastructure.Constants.BulkRequest,
                                                        AttestationId = tsk.AttestationId,
                                                        AttestationName = string.Empty,
                                                        AttestationCampaignId = tsk.AttestationCampaignId,
                                                        CampaignName = String.Empty,
                                                        EventName = blkReq.EndPoint.DisplayName,
                                                        ApplicationType = blkReq.EndPoint.ApplicationType,
                                                        CreatedDate = tsk.CreatedDate,
                                                        IsActive = tsk.IsActive,
                                                        IsCompleted = tsk.IsCompleted,
                                                        ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                                        UserName = string.Empty,
                                                        AdHocAccessRequestId = null,
                                                        AccessRequestID = blkReq.Id,
                                                        RequestedBy = blkReq.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.AdhocRequest ? (rqsteduser != null ? rqsteduser.DisplayName : "System") : "System",
                                                        AssignedTo = tsk.AssignedTo,
                                                        AssignedBy = tsk.AssignedBy,
                                                        AccountType = "User",
                                                        IsDelegated = tsk.IsDelegated,
                                                        IsGroup = tsk.IsGroupTask,
                                                        RequestedFor = "Bulk Request",
                                                        RequestForDept = rqsteruser.NOrganisation != null ? rqsteruser.NOrganisation.Name : "",
                                                        RequestForDesignation = rqsteruser.NBusinessRole != null ? rqsteruser.NBusinessRole.Description : "",
                                                        AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                                        AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                                        AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                                        StartDate = blkReq.ActivationDate,
                                                        EndDate = blkReq.DeactivationDate,
                                                        RequestedDate = blkReq.CreatedDate,
                                                        Type = ((int)blkReq.AdhocActionType == 1 ? "Add New Account" : ((int)blkReq.AdhocActionType == 2) ? "Amendment Add" : ((int)blkReq.AdhocActionType == 3 ? "Amendment Remove" : ((int)blkReq.AdhocActionType == 4 ? "Delete Account" : "None"))),
                                                        StatusType = tsk.Action.ToString(),
                                                        RequestReason = blkReq.Comments, //acsrqst.Comments ,
                                                        DelegatedBy = tsk.DelegatedBy,
                                                        DelegatedComment = tsk.DelegationComments,
                                                        ActionUser = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : "") : actionString,
                                                        ActionUserADID = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.UserName : "") : "",
                                                        BulkReqDetails = blkReq.BulkRequestDetails,
                                                        EntitlementXML = blkReq.EntitlementDetail
                                                    }).ToList();


            foreach (var item in provisioningTasksFromBulkRequest)
            {
                item.RequestedFor = string.Join(",", item.BulkReqDetails.Select(i => i.Name));
            }

            var tasks = attestationTasks.Union(provisioningTasks).Union(provisioningTasksFromBulkRequest).Distinct();

            tasks = tasks.OrderByDescending(rvw => rvw.CreatedDate).ThenBy(rvw1 => rvw1.IsCompleted);

            return tasks.ToList();
        }

        public IList<PresentationTask> GetAllGroupTask(long actionUserId)
        {
            try
            {
                List<long> userGroups = CurrentContext.User.Include("Groups").FirstOrDefault(item => item.Id == actionUserId).Groups.Select(item => item.Id).ToList();

                var attestationTasksForGroup = (from task in CurrentContext.Tasks
                                                join attestattion in CurrentContext.Attestations
                                                    on task.AttestationId equals attestattion.Id
                                                join attCampaign in CurrentContext.AttestationCampaigns
                                                    on task.AttestationCampaignId equals attCampaign.Id
                                                join grp in CurrentContext.Group
                                                    on task.ActionGroupId equals grp.Id
                                                join assignedby in CurrentContext.User
                                                       on (long)task.AssignedBy equals assignedby.Id into Adetail
                                                join assignedUser in CurrentContext.User
                                                  on (long)task.AssignedTo equals assignedUser.Id into detail
                                                from assignedUser in detail.DefaultIfEmpty()
                                                from assignedby in Adetail.DefaultIfEmpty()
                                                join wfConfig in CurrentContext.WorkflowConfigurations
                                                    on attestattion.WorkflowConfigurationId equals wfConfig.Id
                                                from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).DefaultIfEmpty()
                                                    //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                                where task.IsGroupTask == true && userGroups.Contains((int)task.ActionGroupId.Value) && !task.IsCompleted && !task.IsDelegated && (!task.AssignedTo.HasValue || task.AssignedTo != actionUserId)
                                                    && (task.Action == StatusType.None || task.Action == StatusType.Pending || task.Action == StatusType.NotInitiated)
                                                select new PresentationTask
                                                {
                                                    TaskId = task.Id,
                                                    RequestTypeId = task.RequestTypeId,
                                                    TaskSource = "Attestation",
                                                    AttestationId = task.AttestationId,
                                                    AttestationName = attestattion.AttestationName,
                                                    AttestationCampaignId = task.AttestationCampaignId,
                                                    CampaignName = attCampaign.CampaignName,
                                                    EventName = String.Empty,
                                                    CreatedDate = task.CreatedDate,
                                                    IsActive = task.IsActive,
                                                    IsCompleted = task.IsCompleted,

                                                    ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                                    AdHocAccessRequestId = 0,
                                                    //AccessRequestID = attestattion.Id,
                                                    RequestedBy = "System",
                                                    AssignedTo = task.AssignedTo,
                                                    AssignedBy = task.AssignedBy,
                                                    AccountType = AccountType.None.ToString(),
                                                    IsDelegated = task.IsDelegated,
                                                    IsGroup = task.IsGroupTask,
                                                    RequestedFor = grp.GroupName,
                                                    AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                                    AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                                    AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                                    StartDate = task.CreatedDate,
                                                    EndDate = DbFunctions.AddDays(task.CreatedDate,
                                      (wfConfigDetail != null ? wfConfigDetail.ResponseTimeInDays : 1
                                      //+ (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().IsEscalationRequired ?
                                      //task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().DaysToWaitAfterEscalation : 0)
                                      )),
                                                    TaskAccessRequest = DI.Chakra.Infrastructure.Enums.AccessRequestStatus.None,
                                                    Type = attCampaign.CampaignName,
                                                    StatusType = task.Action.ToString(),
                                                    RequestReason = string.Empty,
                                                    DelegatedBy = task.DelegatedBy,
                                                    DelegatedComment = task.DelegationComments,
                                                    UserName = task.Approver,
                                                    RequestedDate = task.CreatedDate,
                                                    ActionUser = (task.IsGroupTask && !task.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : "") : "",
                                                    ActionUserADID = (task.IsGroupTask && !task.IsDelegated) ? (assignedUser != null ? assignedUser.UserName : "") : "",
                                                }).Distinct().ToList();
                List<PresentationTask> attestationTasks = attestationTasksForGroup.ToList();

                var provisioningTasks = (from tsk in CurrentContext.Tasks
                                         join acsrqst in CurrentContext.AccessRequest
                                         on tsk.AccessRequestId equals acsrqst.Id
                                         join adhocrqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                                         on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                                         join rqsteduser in CurrentContext.User
                                             on adhocrqst.RequesterUserId equals rqsteduser.Id into requester
                                         from rqsteduser in requester.DefaultIfEmpty()
                                         join rqsteruser in CurrentContext.User.Include("NBusinessRole").Include("NOrganisation")
                                            on adhocrqst.RequesteeUserId equals rqsteruser.Id
                                         join assignedby in CurrentContext.User
                                                on (long)tsk.AssignedBy equals assignedby.Id into Adetail
                                         join assignedUser in CurrentContext.User
                                           on (long)tsk.AssignedTo equals assignedUser.Id into detail
                                         from assignedUser in detail.DefaultIfEmpty()
                                         from assignedby in Adetail.DefaultIfEmpty()
                                         join wfConfig in CurrentContext.WorkflowConfigurations
                                         on tsk.WorkflowConfigurationId equals wfConfig.Id
                                         from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == tsk.WorkflowStateId).DefaultIfEmpty()
                                             //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == tsk.WorkflowStateId
                                             //where (tsk.ActionUserId == actionUserId && acsrqst.AdHocAccessRequestId.HasValue && tsk.Action == taskStatus && (!tsk.IsGroupTask || (tsk.IsGroupTask && tsk.AssignedTo == actionUserId)))
                                         where tsk.IsGroupTask == true && userGroups.Contains((int)tsk.ActionGroupId.Value) && !tsk.IsCompleted //&& !tsk.IsDelegated && (!tsk.AssignedTo.HasValue || tsk.AssignedTo != actionUserId)
                                                && (tsk.Action == StatusType.None || tsk.Action == StatusType.Pending || tsk.Action == StatusType.NotInitiated)
                                         select new PresentationTask
                                         {
                                             TaskId = tsk.Id,
                                             RequestTypeId = tsk.RequestTypeId,
                                             TaskSource = "Access Request",
                                             AttestationId = tsk.AttestationId,
                                             AttestationName = string.Empty,
                                             AttestationCampaignId = tsk.AttestationCampaignId,
                                             CampaignName = String.Empty,
                                             EventName = adhocrqst.EndPoint.DisplayName,
                                             CreatedDate = tsk.CreatedDate,
                                             IsActive = tsk.IsActive,
                                             IsCompleted = tsk.IsCompleted,
                                             ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                             UserName = string.Empty,
                                             AdHocAccessRequestId = adhocrqst.Id,
                                             AccessRequestID = acsrqst.Id,
                                             RequestedBy = acsrqst.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.AdhocRequest ? (rqsteduser != null ? rqsteduser.DisplayName : "System") : "System",
                                             AssignedTo = tsk.AssignedTo,
                                             AssignedBy = tsk.AssignedBy,
                                             AccountType = adhocrqst.AccountType.ToString(),
                                             IsDelegated = tsk.IsDelegated,
                                             IsGroup = tsk.IsGroupTask,
                                             RequestedFor = rqsteruser.DisplayName,
                                             RequestForDept = rqsteruser.NOrganisation != null ? rqsteruser.NOrganisation.Name : "",
                                             RequestForDesignation = rqsteruser.NBusinessRole != null ? rqsteruser.NBusinessRole.Description : "",
                                             AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                             AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                             AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                             StartDate = adhocrqst.ValidFrom,
                                             EndDate = adhocrqst.ValidTo,
                                             RequestedDate = adhocrqst.CreatedDate,
                                             Type = ((int)adhocrqst.AdhocActionType == 1 ? "Add New Account" : ((int)adhocrqst.AdhocActionType == 2) ? "Amendment Add" : ((int)adhocrqst.AdhocActionType == 3 ? "Amendment Remove" : ((int)adhocrqst.AdhocActionType == 4 ? "Delete Account" : "None"))),
                                             StatusType = tsk.Action.ToString(),
                                             RequestReason = adhocrqst.Comments, //acsrqst.Comments ,
                                             DelegatedBy = tsk.DelegatedBy,
                                             DelegatedComment = tsk.DelegationComments,
                                             ActionUser = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : "") : "",
                                             ActionUserADID = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.UserName : "") : "",
                                         }).Distinct().ToList();
                var provisioningTasksFromBulkRequest = (from tsk in CurrentContext.Tasks
                                                        join blkReq in CurrentContext.BulkRequests.Include("BulkRequestDetails").Include("EndPoint")
                                                        on tsk.BulkRequestId equals blkReq.Id
                                                        join rqsteduser in CurrentContext.User
                                                        on blkReq.RequestedBy equals rqsteduser.Id into requester
                                                        from rqsteduser in requester.DefaultIfEmpty()
                                                        join rqsteruser in CurrentContext.User.Include("NBusinessRole").Include("NOrganisation")
                                                        on blkReq.SelectedSupervisor equals rqsteruser.Id
                                                        join assignedby in CurrentContext.User
                                                               on (long)tsk.AssignedBy equals assignedby.Id into Adetail
                                                        join assignedUser in CurrentContext.User
                                                          on (long)tsk.AssignedTo equals assignedUser.Id into detail
                                                        from assignedUser in detail.DefaultIfEmpty()
                                                        from assignedby in Adetail.DefaultIfEmpty()
                                                        join wfConfig in CurrentContext.WorkflowConfigurations
                                                        on tsk.WorkflowConfigurationId equals wfConfig.Id
                                                        from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == tsk.WorkflowStateId).DefaultIfEmpty()
                                                        where tsk.IsGroupTask == true && userGroups.Contains((int)tsk.ActionGroupId.Value) && !tsk.IsCompleted //&& !tsk.IsDelegated && (!tsk.AssignedTo.HasValue || tsk.AssignedTo != actionUserId)
                                                        && (tsk.Action == StatusType.None || tsk.Action == StatusType.Pending || tsk.Action == StatusType.NotInitiated)
                                                        select new PresentationTask
                                                        {
                                                            TaskId = tsk.Id,
                                                            RequestTypeId = tsk.RequestTypeId,
                                                            TaskSource = DI.Chakra.Infrastructure.Constants.BulkRequest,
                                                            AttestationId = tsk.AttestationId,
                                                            AttestationName = string.Empty,
                                                            AttestationCampaignId = tsk.AttestationCampaignId,
                                                            CampaignName = String.Empty,
                                                            EventName = blkReq.EndPoint.DisplayName,
                                                            ApplicationType = blkReq.EndPoint.ApplicationType,
                                                            CreatedDate = tsk.CreatedDate,
                                                            IsActive = tsk.IsActive,
                                                            IsCompleted = tsk.IsCompleted,
                                                            ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                                            UserName = string.Empty,
                                                            AdHocAccessRequestId = null,
                                                            AccessRequestID = blkReq.Id,
                                                            RequestedBy = blkReq.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.AdhocRequest ? (rqsteduser != null ? rqsteduser.DisplayName : "System") : "System",
                                                            AssignedTo = tsk.AssignedTo,
                                                            AssignedBy = tsk.AssignedBy,
                                                            AccountType = "User",
                                                            IsDelegated = tsk.IsDelegated,
                                                            IsGroup = tsk.IsGroupTask,
                                                            RequestedFor = "Bulk Request",
                                                            RequestForDept = rqsteruser.NOrganisation != null ? rqsteruser.NOrganisation.Name : "",
                                                            RequestForDesignation = rqsteruser.NBusinessRole != null ? rqsteruser.NBusinessRole.Description : "",
                                                            AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                                            AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                                            AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                                            StartDate = blkReq.ActivationDate,
                                                            EndDate = blkReq.DeactivationDate,
                                                            RequestedDate = blkReq.CreatedDate,
                                                            Type = ((int)blkReq.AdhocActionType == 1 ? "Add New Account" : ((int)blkReq.AdhocActionType == 2) ? "Amendment Add" : ((int)blkReq.AdhocActionType == 3 ? "Amendment Remove" : ((int)blkReq.AdhocActionType == 4 ? "Delete Account" : "None"))),
                                                            StatusType = tsk.Action.ToString(),
                                                            RequestReason = blkReq.Comments, //acsrqst.Comments ,
                                                            DelegatedBy = tsk.DelegatedBy,
                                                            DelegatedComment = tsk.DelegationComments,
                                                            ActionUser = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : "") : "",
                                                            ActionUserADID = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.UserName : "") : "",
                                                            BulkReqDetails = blkReq.BulkRequestDetails,
                                                            EntitlementXML = blkReq.EntitlementDetail
                                                        }).ToList();
                List<PresentationTask> lst = provisioningTasks.ToList();
                var grpTask = lst.Union(attestationTasks).Union(provisioningTasksFromBulkRequest.Distinct()).ToList();
                return grpTask;
            }
            catch (Exception ex)
            {
                return new List<PresentationTask>();
            }
        }
        public IList<PresentationTask> GetGroupTaskDetailsByTaskStatus(long actionUserId, StatusType taskStatus)
        {
            List<long> userGroups = CurrentContext.User.Include("Groups").FirstOrDefault(item => item.Id == actionUserId).Groups.Select(item => item.Id).ToList();
            var actionnUser = CurrentContext.User.Where(a => a.Id == actionUserId).FirstOrDefault();
            var actionString = (actionnUser != null ? actionnUser.DisplayName : "");
            var attestationTasks = (from task in CurrentContext.Tasks
                                    join attestattion in CurrentContext.Attestations
                                        on task.AttestationId equals attestattion.Id
                                    join attCampaign in CurrentContext.AttestationCampaigns
                                        on task.AttestationCampaignId equals attCampaign.Id
                                    join user in CurrentContext.Group
                                        on task.ActionGroupId equals user.Id
                                    join assignedby in CurrentContext.User
                                           on (long)task.AssignedBy equals assignedby.Id into Adetail
                                    join assignedUser in CurrentContext.User
                                      on (long)task.AssignedTo equals assignedUser.Id into detail
                                    from assignedUser in detail.DefaultIfEmpty()
                                    from assignedby in Adetail.DefaultIfEmpty()
                                    join wfConfig in CurrentContext.WorkflowConfigurations
                                        on attestattion.WorkflowConfigurationId equals wfConfig.Id
                                    from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).DefaultIfEmpty()
                                        //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                    where (userGroups.Contains((int)task.ActionGroupId.Value) && task.IsGroupTask // && (task.AssignedTo == null || (task.AssignedTo != null && task.AssignedTo != actionUserId)) 
                                    && task.Action == taskStatus)
                                    select new PresentationTask
                                    {
                                        TaskId = task.Id,
                                        RequestTypeId = task.RequestTypeId,
                                        TaskSource = "Attestation",
                                        AttestationId = task.AttestationId,
                                        AttestationName = attCampaign.Attestation.AttestationName,
                                        AttestationCampaignId = task.AttestationCampaignId,
                                        CampaignName = attCampaign.CampaignName,
                                        EventName = string.Empty,
                                        CreatedDate = task.CreatedDate,
                                        IsActive = task.IsActive,
                                        IsCompleted = task.IsCompleted,
                                        ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                        UserName = string.Empty,
                                        AdHocAccessRequestId = null,
                                        AccessRequestID = attestattion.Id,
                                        RequestedBy = "System",
                                        AssignedTo = task.AssignedTo,
                                        AssignedBy = task.AssignedBy,
                                        DelegatedBy = task.DelegatedBy,
                                        AccountType = AccountType.None.ToString(),
                                        IsDelegated = task.IsDelegated,
                                        IsGroup = task.IsGroupTask,
                                        RequestedFor = user.GroupName,
                                        AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                        AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                        AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                        StartDate = task.CreatedDate,
                                        EndDate = DbFunctions.AddDays(task.CreatedDate,
                                  (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault() == null ? 1 :
                                  task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().ResponseTimeInDays
                                  //+ (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().IsEscalationRequired ?
                                  //task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().DaysToWaitAfterEscalation : 0)
                                  )),
                                        RequestedDate = task.CreatedDate,
                                        Type = attCampaign.CampaignName,
                                        ActionUser = (task.IsGroupTask && !task.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : "") : actionString,
                                        ActionUserADID = (task.IsGroupTask && !task.IsDelegated) ? (assignedUser != null ? assignedUser.UserName : "") : "",
                                        StatusType = task.Action.ToString(),
                                    }).Distinct().ToList();

            var provisioningTasks = (from tsk in CurrentContext.Tasks
                                     join acsrqst in CurrentContext.AccessRequest
                                     on tsk.AccessRequestId equals acsrqst.Id
                                     join adhocrqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                                     on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                                     join rqsteduser in CurrentContext.User
                                         on adhocrqst.RequesterUserId equals rqsteduser.Id into requester
                                     from rqsteduser in requester.DefaultIfEmpty()
                                     join rqsteruser in CurrentContext.User.Include("NBusinessRole").Include("NOrganisation")
                                        on adhocrqst.RequesteeUserId equals rqsteruser.Id
                                     join assignedby in CurrentContext.User
                                            on (long)tsk.AssignedBy equals assignedby.Id into Adetail
                                     join assignedUser in CurrentContext.User
                                       on (long)tsk.AssignedTo equals assignedUser.Id into detail
                                     from assignedUser in detail.DefaultIfEmpty()
                                     from assignedby in Adetail.DefaultIfEmpty()
                                     join wfConfig in CurrentContext.WorkflowConfigurations
                                     on tsk.WorkflowConfigurationId equals wfConfig.Id
                                     from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == tsk.WorkflowStateId).DefaultIfEmpty()
                                         //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == tsk.WorkflowStateId
                                     where (userGroups.Contains((Int32)tsk.ActionGroupId.Value) && acsrqst.AdHocAccessRequestId.HasValue && tsk.IsGroupTask // && (tsk.AssignedTo == null || (tsk.AssignedTo != null && tsk.AssignedTo != actionUserId)) 
                                     && tsk.Action == taskStatus)
                                     select new PresentationTask
                                     {
                                         TaskId = tsk.Id,
                                         RequestTypeId = tsk.RequestTypeId,
                                         TaskSource = "Access Request",
                                         AttestationId = tsk.AttestationId,
                                         AttestationName = string.Empty,
                                         AttestationCampaignId = tsk.AttestationCampaignId,
                                         CampaignName = String.Empty,
                                         EventName = adhocrqst.EndPoint.DisplayName,
                                         ApplicationType = adhocrqst.EndPoint.ApplicationType,
                                         CreatedDate = tsk.CreatedDate,
                                         IsActive = tsk.IsActive,
                                         IsCompleted = tsk.IsCompleted,
                                         ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                         UserName = string.Empty,
                                         AdHocAccessRequestId = adhocrqst.Id,
                                         AccessRequestID = acsrqst.Id,
                                         RequestedBy = acsrqst.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.AdhocRequest ? (rqsteduser != null ? rqsteduser.DisplayName : "System") : "System",
                                         AssignedTo = tsk.AssignedTo,
                                         AssignedBy = tsk.AssignedBy,
                                         DelegatedBy = tsk.DelegatedBy,
                                         AccountType = adhocrqst.AccountType.ToString(),
                                         IsDelegated = tsk.IsDelegated,
                                         IsGroup = tsk.IsGroupTask,
                                         RequestedFor = rqsteruser.DisplayName,
                                         RequestForDept = rqsteruser.NOrganisation != null ? rqsteruser.NOrganisation.Name : "",
                                         RequestForDesignation = rqsteruser.NBusinessRole != null ? rqsteruser.NBusinessRole.Description : "",
                                         AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                         AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                         AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                         StartDate = adhocrqst.ValidFrom,
                                         EndDate = adhocrqst.ValidTo,
                                         RequestedDate = adhocrqst.CreatedDate,
                                         Type = ((int)adhocrqst.AdhocActionType == 1 ? "Add New Account" : ((int)adhocrqst.AdhocActionType == 2) ? "Amendment Add" : ((int)adhocrqst.AdhocActionType == 3 ? "Amendment Remove" : ((int)adhocrqst.AdhocActionType == 4 ? "Delete Account" : "None"))),
                                         ActionUser = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : "") : actionString,
                                         StatusType = tsk.Action.ToString(),
                                         EntitlementXML = adhocrqst.EntitlementDetail
                                     }).Distinct().ToList();

            var tasks = attestationTasks.Union(provisioningTasks);
            var provisioningTasksFromBulkRequest = (from tsk in CurrentContext.Tasks
                                                    join blkReq in CurrentContext.BulkRequests.Include("EndPoint")
                                                    on tsk.BulkRequestId equals blkReq.Id
                                                    join rqsteduser in CurrentContext.User
                                                    on blkReq.RequestedBy equals rqsteduser.Id into requester
                                                    from rqsteduser in requester.DefaultIfEmpty()
                                                    join rqsteruser in CurrentContext.User.Include("NBusinessRole").Include("NOrganisation")
                                                    on blkReq.SelectedSupervisor equals rqsteruser.Id
                                                    join assignedby in CurrentContext.User
                                                           on (long)tsk.AssignedBy equals assignedby.Id into Adetail
                                                    join assignedUser in CurrentContext.User
                                                      on (long)tsk.AssignedTo equals assignedUser.Id into detail
                                                    from assignedUser in detail.DefaultIfEmpty()
                                                    from assignedby in Adetail.DefaultIfEmpty()
                                                    join wfConfig in CurrentContext.WorkflowConfigurations
                                                    on tsk.WorkflowConfigurationId equals wfConfig.Id
                                                    from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == tsk.WorkflowStateId).DefaultIfEmpty()
                                                        //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == tsk.WorkflowStateId
                                                        //where (tsk.ActionUserId == actionUserId && acsrqst.AdHocAccessRequestId.HasValue && tsk.Action == taskStatus && (!tsk.IsGroupTask || (tsk.IsGroupTask && tsk.AssignedTo == actionUserId)))
                                                    where (userGroups.Contains((Int32)tsk.ActionGroupId.Value) // && tsk.IsGroupTask && (tsk.AssignedTo == null || (tsk.AssignedTo != null && tsk.AssignedTo != actionUserId)) 
                                                    && tsk.Action == taskStatus)
                                                    select new PresentationTask
                                                    {
                                                        TaskId = tsk.Id,
                                                        RequestTypeId = tsk.RequestTypeId,
                                                        TaskSource = DI.Chakra.Infrastructure.Constants.BulkRequest,
                                                        AttestationId = tsk.AttestationId,
                                                        AttestationName = string.Empty,
                                                        AttestationCampaignId = tsk.AttestationCampaignId,
                                                        CampaignName = String.Empty,
                                                        EventName = blkReq.EndPoint.DisplayName,
                                                        ApplicationType = blkReq.EndPoint.ApplicationType,
                                                        CreatedDate = tsk.CreatedDate,
                                                        IsActive = tsk.IsActive,
                                                        IsCompleted = tsk.IsCompleted,
                                                        ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                                        UserName = string.Empty,
                                                        AdHocAccessRequestId = null,
                                                        AccessRequestID = blkReq.Id,
                                                        RequestedBy = blkReq.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.AdhocRequest ? (rqsteduser != null ? rqsteduser.DisplayName : "System") : "System",
                                                        AssignedTo = tsk.AssignedTo,
                                                        AssignedBy = tsk.AssignedBy,
                                                        AccountType = "User",
                                                        IsDelegated = tsk.IsDelegated,
                                                        IsGroup = tsk.IsGroupTask,
                                                        RequestedFor = "Bulk Request",
                                                        RequestForDept = rqsteruser.NOrganisation != null ? rqsteruser.NOrganisation.Name : "",
                                                        RequestForDesignation = rqsteruser.NBusinessRole != null ? rqsteruser.NBusinessRole.Description : "",
                                                        AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                                        AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                                        AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                                        StartDate = blkReq.ActivationDate,
                                                        EndDate = blkReq.DeactivationDate,
                                                        RequestedDate = blkReq.CreatedDate,
                                                        Type = ((int)blkReq.AdhocActionType == 1 ? "Add New Account" : ((int)blkReq.AdhocActionType == 2) ? "Amendment Add" : ((int)blkReq.AdhocActionType == 3 ? "Amendment Remove" : ((int)blkReq.AdhocActionType == 4 ? "Delete Account" : "None"))),
                                                        StatusType = tsk.Action.ToString(),
                                                        RequestReason = blkReq.Comments, //acsrqst.Comments ,
                                                        DelegatedBy = tsk.DelegatedBy,
                                                        DelegatedComment = tsk.DelegationComments,
                                                        ActionUser = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : "") : actionString,
                                                        ActionUserADID = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.UserName : "") : "",
                                                        BulkReqDetails = blkReq.BulkRequestDetails,
                                                        EntitlementXML = blkReq.EntitlementDetail
                                                    }).ToList();

            foreach (var item in provisioningTasksFromBulkRequest)
            {
                item.RequestedFor = string.Join(",", item.BulkReqDetails.Select(i => i.Name));
            }

            var employeeEventTasks = (from tsk in CurrentContext.Tasks
                                      join trigger in CurrentContext.EmployeeEventTriggers
                                      on tsk.EmployeeEventTriggerId equals trigger.Id
                                      join rqsteduser in CurrentContext.User
                                      on trigger.CreatedBy equals rqsteduser.UserName
                                      join rqsteruser in CurrentContext.User
                                         on trigger.UserId equals rqsteruser.Id
                                      join assignedby in CurrentContext.User
                                             on tsk.AssignedBy equals assignedby.Id into Adetail
                                      join assignedUser in CurrentContext.User
                                        on tsk.AssignedTo equals assignedUser.Id into detail
                                      from assignedUser in detail.DefaultIfEmpty()
                                      from assignedby in Adetail.DefaultIfEmpty()
                                      where (userGroups.Contains((int)tsk.ActionGroupId.Value) && tsk.IsGroupTask // && (task.AssignedTo == null || (task.AssignedTo != null && task.AssignedTo != actionUserId)) 
                                       && tsk.Action == taskStatus)
                                      select new PresentationTask
                                      {
                                          TaskId = tsk.Id,
                                          RequestTypeId = tsk.RequestTypeId,
                                          TaskSource = "Employee Creation",
                                          AttestationId = tsk.AttestationId,
                                          AttestationName = string.Empty,
                                          EventName = trigger.EventType == EmployeeEvents.NonEmployeeCreation ? "Non Employee Creation" : (trigger.EventType == EmployeeEvents.NonEmployeeExtension ? "Non Employee Extension / Shortening" : string.Empty),
                                          CreatedDate = tsk.CreatedDate,
                                          IsActive = tsk.IsActive,
                                          IsCompleted = tsk.IsCompleted,
                                          ReponseTimeInDays = 0,
                                          UserName = string.Empty,
                                          AdHocAccessRequestId = null,
                                          AccessRequestID = null,
                                          RequestedBy = rqsteduser.DisplayName,
                                          AssignedTo = tsk.AssignedTo,
                                          AssignedBy = tsk.AssignedBy,
                                          AccountType = AccountType.None.ToString(),
                                          IsDelegated = tsk.IsDelegated,
                                          IsGroup = tsk.IsGroupTask,
                                          RequestedFor = rqsteruser.DisplayName,
                                          AssignedToName = assignedUser.DisplayName,
                                          AssignedByName = assignedby.DisplayName,
                                          StatusType = tsk.Action.ToString(),
                                          //StartDate = tsk.AdHocAccessRequest.ValidFrom,
                                          //EndDate = tsk.AdHocAccessRequest.ValidTo,
                                          RequestedDate = trigger.CreatedDate,
                                          Type = trigger.EventType == EmployeeEvents.NonEmployeeCreation ? "Non Employee Creation" : (trigger.EventType == EmployeeEvents.NonEmployeeExtension ? "Non Employee Extension / Shortening" : string.Empty)
                                      }).ToList();

            tasks = tasks.Union(employeeEventTasks);
            tasks = tasks.Union(provisioningTasksFromBulkRequest);
            tasks = tasks.OrderByDescending(rvw => rvw.CreatedDate).ThenBy(rvw1 => rvw1.IsCompleted);

            return tasks.ToList();
        }

        public IList<PresentationTask> GetSubmittedAttestationTaskDetailsByActionUserId(long actionUserId)
        {
            return (from task in CurrentContext.Tasks
                    where task.IsCompleted
                    join attestattion in CurrentContext.Attestations
                        on task.AttestationId equals attestattion.Id
                    join attCampaign in CurrentContext.AttestationCampaigns
                                    on task.AttestationCampaignId equals attCampaign.Id
                    join wfConfig in CurrentContext.WorkflowConfigurations
                        on attestattion.WorkflowConfigurationId equals wfConfig.Id
                    from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).DefaultIfEmpty()
                        //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == task.WorkflowStateId
                    where task.ActionUserId == actionUserId
                    select new PresentationTask
                    {
                        TaskId = task.Id,
                        RequestTypeId = task.RequestTypeId,
                        AttestationId = task.AttestationId,
                        AttestationName = attCampaign.CampaignName,
                        EventName = string.Empty,
                        CreatedDate = task.CreatedDate,
                        IsActive = task.IsActive,
                        IsCompleted = task.IsCompleted,
                        ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                        UserName = string.Empty,
                        SubmittedDate = task.LastModifiedDate.Value,
                        StatusType = task.Action.ToString(),
                    }).ToList();
        }


        public void CreateTriggers()
        {
            foreach (var filePath in Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory + "\\SqlScripts\\Triggers\\", "*.sql"))
            {
                CurrentContext.Database.ExecuteSqlCommand(File.ReadAllText(filePath));
                CurrentContext.SaveChanges();
            }
        }

        public List<long> GetGroupId(long actionUserId)
        {
            List<long> userGroups = CurrentContext.User.Include("Groups").FirstOrDefault(item => item.Id == actionUserId).Groups.Select(item => item.Id).ToList();
            return userGroups;
        }

        public List<PresentationTask> GetAllExecutorTask()
        {
            List<PresentationTask> finalOutput = new List<PresentationTask>();
            var userExecutorTask = (from task in CurrentContext.Tasks.Include("WorkFlowConfiguration")
                                    join user in CurrentContext.User on task.ActionUserId equals user.Id
                                    join acsrqst in CurrentContext.AccessRequest.Include("Requester")
                                    on task.AccessRequestId equals acsrqst.Id
                                    join adhocrqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                                    on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                                    join rqsteduser in CurrentContext.User
                                       on adhocrqst.RequesterUserId equals rqsteduser.Id into requester
                                    from rqsteduser in requester.DefaultIfEmpty()
                                    join rqsteruser in CurrentContext.User.Include("NBusinessRole").Include("NOrganisation")
                                       on adhocrqst.RequesteeUserId equals rqsteruser.Id
                                    where task.IsActionOnlyRequiredTask && task.IsCompleted == false &&
                                    task.Action == StatusType.Pending && (!task.IsGroupTask || (task.IsGroupTask && task.IsDelegated))
                                    select new PresentationTask
                                    {
                                        TaskId = task.Id,
                                        RequestTypeId = task.RequestTypeId,
                                        AccessRequestID = acsrqst.Id,
                                        WorkFlowConfigurationId = task.WorkflowConfigurationId,
                                        EventName = adhocrqst.AdhocActionType.ToString() + " - " + adhocrqst.EndPoint.DisplayName,
                                        AdHocAccessRequestId = adhocrqst.Id,
                                        StatusType = ((StatusType)task.Action).ToString(),
                                        Type = task.Approver,
                                        CreatedDate = task.CreatedDate,
                                        AssignedToName = user.DisplayName,
                                        AssignedToADID = user.UserName,
                                        IsDelegated = task.IsDelegated,
                                        DelegatedBy = task.Delegated.DisplayName,
                                        IsGroup = task.IsGroupTask,
                                        GroupName = task.IsGroupTask ? task.GroupName : string.Empty,
                                        RequestedDate = adhocrqst.CreatedDate,
                                        TaskName = adhocrqst.AdhocActionType.ToString() + " - " + adhocrqst.EndPoint.DisplayName,
                                        RequestedBy = rqsteduser.DisplayName,
                                        RequestedFor = rqsteruser.DisplayName,
                                        RequestForDept = rqsteruser.NOrganisation != null ? rqsteruser.NOrganisation.Name : "",
                                        RequestForDesignation = rqsteruser.NBusinessRole != null ? rqsteruser.NBusinessRole.Description : "",
                                        EndDate = DbFunctions.AddDays(task.CreatedDate,
                                                  (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault() == null ? 1 :
                                                    task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().ResponseTimeInDays
                                                  //+ (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().IsEscalationRequired ?
                                                  //task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().DaysToWaitAfterEscalation : 0)
                                                  )),
                                    }).ToList();

            if (userExecutorTask != null && userExecutorTask.Count > 0)
                finalOutput.AddRange(userExecutorTask);

            var groupExecutorTask = (from task in CurrentContext.Tasks.Include("WorkFlowConfiguration")
                                     join grp in CurrentContext.Group on task.ActionGroupId equals grp.Id
                                     join assignedUser in CurrentContext.User on task.AssignedTo equals assignedUser.Id into grpAssignedUser
                                     from assignedUser in grpAssignedUser.DefaultIfEmpty()
                                     join acsrqst in CurrentContext.AccessRequest
                                     on task.AccessRequestId equals acsrqst.Id
                                     join adhocrqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                                     on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                                     join rqsteduser in CurrentContext.User
                                      on adhocrqst.RequesterUserId equals rqsteduser.Id into requester
                                     from rqsteduser in requester.DefaultIfEmpty()
                                     join rqsteruser in CurrentContext.User.Include("NBusinessRole").Include("NOrganisation")
                                        on adhocrqst.RequesteeUserId equals rqsteruser.Id
                                     where task.IsActionOnlyRequiredTask && task.IsCompleted == false &&
                                     task.Action == StatusType.Pending && task.IsGroupTask && !task.IsDelegated
                                     select new PresentationTask
                                     {
                                         TaskId = task.Id,
                                         RequestTypeId = task.RequestTypeId,
                                         AccessRequestID = acsrqst.Id,
                                         WorkFlowConfigurationId = task.WorkflowConfigurationId,
                                         EventName = adhocrqst.AdhocActionType.ToString() + " - " + adhocrqst.EndPoint.DisplayName,
                                         AdHocAccessRequestId = adhocrqst.Id,
                                         StatusType = ((StatusType)task.Action).ToString(),
                                         Type = task.Approver,
                                         CreatedDate = task.CreatedDate,
                                         AssignedToName = task.AssignedTo.HasValue ? (assignedUser != null ? assignedUser.DisplayName : "") : string.Empty,
                                         AssignedToADID = task.AssignedTo.HasValue ? (assignedUser != null ? assignedUser.UserName : "") : string.Empty,
                                         IsDelegated = task.IsDelegated,
                                         DelegatedBy = task.Delegated.DisplayName,
                                         IsGroup = task.IsGroupTask,
                                         GroupName = task.GroupName,
                                         RequestedDate = adhocrqst.CreatedDate,
                                         TaskName = adhocrqst.AdhocActionType.ToString() + " - " + adhocrqst.EndPoint.DisplayName,
                                         RequestedBy = rqsteduser.DisplayName,
                                         RequestedFor = rqsteruser.DisplayName,
                                         RequestForDept = rqsteruser.NOrganisation != null ? rqsteruser.NOrganisation.Name : "",
                                         RequestForDesignation = rqsteruser.NBusinessRole != null ? rqsteruser.NBusinessRole.Description : "",
                                         EndDate = DbFunctions.AddDays(task.CreatedDate,
                                                   (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault() == null ? 1 :
                                                    task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().ResponseTimeInDays
                                                   //+ (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().IsEscalationRequired ?
                                                   //task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().DaysToWaitAfterEscalation : 0)
                                                   )),
                                     }).ToList();

            if (groupExecutorTask != null && groupExecutorTask.Count > 0)
                finalOutput.AddRange(groupExecutorTask);

            return finalOutput;
        }

        public IList<PresentationTask> GetCampaignTaskDetail(long AttestationCampaignId)
        {
            try
            {
                var outputForUserTask = (from task in CurrentContext.Tasks.Include("WorkFlowConfiguration")
                                         join user in CurrentContext.User on task.ActionUserId equals user.Id
                                         join attestattion in CurrentContext.Attestations
                                              on task.AttestationId equals attestattion.Id
                                         join attCampaign in CurrentContext.AttestationCampaigns
                                                         on task.AttestationCampaignId equals attCampaign.Id
                                         where task.AttestationCampaignId == AttestationCampaignId && !task.IsGroupTask

                                         select new PresentationTask
                                         {
                                             UserName = user.DisplayName,
                                             AttestationId = task.AttestationId,
                                             IsCompleted = task.IsCompleted,
                                             Action = task.Action.ToString(),
                                             TaskId = task.Id,
                                             AttestationCampaignId = task.AttestationCampaignId,
                                             AttestationName = attestattion.AttestationName,
                                             Approver = task.Approver,
                                             CampaignName = attCampaign.CampaignName,
                                             RequestedDate = task.CreatedDate,
                                             SubmittedDate = task.IsCompleted ? task.LastModifiedDate : null,
                                             //EndDate = EntityFunctions.AddDays(DateTime.Now,
                                             //(task.WorkFlowConfigurationDetail.ResponseTimeInDays + (task.WorkFlowConfigurationDetail.IsEscalationRequired ? task.WorkFlowConfigurationDetail.DaysToWaitAfterEscalation : 0))),
                                             EndDate = EntityFunctions.AddDays(task.CreatedDate,
                                             (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).Any() ? task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().ResponseTimeInDays : 1 +
                                             (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).Any() ? (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().IsEscalationRequired ?
                                             task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().DaysToWaitAfterEscalation : 0) : 0))),
                                             Level = task.WorkflowStateId,
                                         }



                        ).Distinct().ToList();

                var outputForGrpTask = (from task in CurrentContext.Tasks.Include("WorkFlowConfiguration")
                                        join grpUser in CurrentContext.User on task.AssignedTo equals grpUser.Id
                                        join attestattion in CurrentContext.Attestations
                                             on task.AttestationId equals attestattion.Id
                                        join attCampaign in CurrentContext.AttestationCampaigns
                                                        on task.AttestationCampaignId equals attCampaign.Id
                                        where task.AttestationCampaignId == AttestationCampaignId && task.IsGroupTask

                                        select new PresentationTask
                                        {
                                            UserName = grpUser.DisplayName,
                                            AttestationId = task.AttestationId,
                                            IsCompleted = task.IsCompleted,
                                            Action = task.Action.ToString(),
                                            TaskId = task.Id,
                                            AttestationCampaignId = task.AttestationCampaignId,
                                            AttestationName = attestattion.AttestationName,
                                            Approver = task.Approver,
                                            CampaignName = attCampaign.CampaignName,
                                            RequestedDate = task.CreatedDate,
                                            SubmittedDate = task.IsCompleted ? task.LastModifiedDate : null,
                                            //          EndDate = EntityFunctions.AddDays(DateTime.Now,
                                            //(task.WorkFlowConfigurationDetail.ResponseTimeInDays + (task.WorkFlowConfigurationDetail.IsEscalationRequired ? task.WorkFlowConfigurationDetail.DaysToWaitAfterEscalation : 0))),
                                            EndDate = EntityFunctions.AddDays(task.CreatedDate,
                                                       (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Any() ? task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().ResponseTimeInDays : 0 +
                                            (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Any() ? (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().IsEscalationRequired ?
                                            task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().DaysToWaitAfterEscalation : 0) : 0))),
                                            Level = task.WorkflowStateId,
                                        }



                        ).Distinct().ToList();

                var outputForGrpUnAssignedTask = (from task in CurrentContext.Tasks.Include("WorkFlowConfiguration")
                                                  join attestattion in CurrentContext.Attestations
                                                          on task.AttestationId equals attestattion.Id
                                                  join attCampaign in CurrentContext.AttestationCampaigns
                                                          on task.AttestationCampaignId equals attCampaign.Id
                                                  where task.AttestationCampaignId == AttestationCampaignId && task.IsGroupTask && !task.AssignedTo.HasValue

                                                  select new PresentationTask
                                                  {
                                                      UserName = "",
                                                      AttestationId = task.AttestationId,
                                                      IsCompleted = task.IsCompleted,
                                                      Action = task.Action.ToString(),
                                                      TaskId = task.Id,
                                                      AttestationCampaignId = task.AttestationCampaignId,
                                                      AttestationName = attestattion.AttestationName,
                                                      Approver = task.Approver,
                                                      CampaignName = attCampaign.CampaignName,
                                                      RequestedDate = task.CreatedDate,
                                                      SubmittedDate = task.IsCompleted ? task.LastModifiedDate : null,
                                                      //          EndDate = EntityFunctions.AddDays(DateTime.Now,
                                                      //(task.WorkFlowConfigurationDetail.ResponseTimeInDays + (task.WorkFlowConfigurationDetail.IsEscalationRequired ? task.WorkFlowConfigurationDetail.DaysToWaitAfterEscalation : 0))),
                                                      EndDate = EntityFunctions.AddDays(task.CreatedDate,
                                                                 (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Any() ? task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().ResponseTimeInDays : 1 +
                                                      (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Any() ? (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().IsEscalationRequired ?
                                                      task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().DaysToWaitAfterEscalation : 0) : 0))),
                                                      Level = task.WorkflowStateId,
                                                  }



                        ).Distinct().ToList();

                var output = outputForUserTask.Union(outputForGrpTask).Union(outputForGrpUnAssignedTask).Distinct().ToList();
                output = output.OrderBy(a => a.IsCompleted).ThenBy(a => a.EndDate).ThenByDescending(a => a.Level).ToList();
                return output;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public List<PresentationTask> GetAllExecutorTaskReport()
        {
            return (from task in CurrentContext.Tasks.Include("User")
                    join acsrqst in CurrentContext.AccessRequest
                    on task.AccessRequestId equals acsrqst.Id
                    join adhocrqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                    on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                    where task.IsActionOnlyRequiredTask &&
                    (task.Action == StatusType.Pending || task.Action == StatusType.Approved)
                    select new PresentationTask
                    {
                        TaskId = task.Id,
                        RequestTypeId = task.RequestTypeId,
                        AccessRequestID = acsrqst.Id,
                        WorkFlowConfigurationId = task.WorkflowConfigurationId,
                        EventName = adhocrqst.EndPoint.DisplayName,
                        StatusType = ((StatusType)task.Action).ToString(),
                        Type = task.Approver,
                        CreatedDate = task.CreatedDate,
                        AssignedToName = task.ActionUser.DisplayName,
                        AssignedToADID = task.ActionUser.UserName,
                        DelegatedBy = task.Delegated.DisplayName,
                        IsGroup = task.IsGroupTask,
                    }).Distinct().ToList();
        }

        public List<PresentationTask> GetAllExecutorTaskByDate(DateTime? fromDate, DateTime? toDate, StatusType statusType)
        {
            return (from task in CurrentContext.Tasks.Include("User")
                    join acsrqst in CurrentContext.AccessRequest
                    on task.AccessRequestId equals acsrqst.Id
                    join adhocrqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                    on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                    where task.IsActionOnlyRequiredTask && task.Action == statusType &&
                    task.CreatedDate >= fromDate && task.CreatedDate <= toDate
                    select new PresentationTask
                    {
                        TaskId = task.Id,
                        RequestTypeId = task.RequestTypeId,
                        AccessRequestID = acsrqst.Id,
                        WorkFlowConfigurationId = task.WorkflowConfigurationId,
                        EventName = adhocrqst.EndPoint.DisplayName,
                        StatusType = ((StatusType)task.Action).ToString(),
                        Type = task.Approver,
                        CreatedDate = task.CreatedDate,
                        AssignedToName = task.ActionUser.DisplayName,
                        DelegatedBy = task.Delegated.DisplayName,
                        IsGroup = task.IsGroupTask,
                    }).Distinct().ToList();
        }

        public List<PresentationTask> GetAllExecutorTaskByDateTime(DateTime? fromDate, DateTime? toDate)
        {
            return (from task in CurrentContext.Tasks.Include("User")
                    join acsrqst in CurrentContext.AccessRequest
                    on task.AccessRequestId equals acsrqst.Id
                    join adhocrqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                    on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                    where task.IsActionOnlyRequiredTask &&
                    (task.Action == StatusType.Pending || task.Action == StatusType.Approved) &&
                    task.CreatedDate >= fromDate && task.CreatedDate <= toDate
                    select new PresentationTask
                    {
                        TaskId = task.Id,
                        RequestTypeId = task.RequestTypeId,
                        AccessRequestID = acsrqst.Id,
                        WorkFlowConfigurationId = task.WorkflowConfigurationId,
                        EventName = adhocrqst.EndPoint.DisplayName,
                        StatusType = ((StatusType)task.Action).ToString(),
                        Type = task.Approver,
                        CreatedDate = task.CreatedDate,
                        AssignedToName = task.ActionUser.DisplayName,
                        DelegatedBy = task.Delegated.DisplayName,
                        IsGroup = task.IsGroupTask,
                    }).Distinct().ToList();
        }
        public List<PresentationTask> GetAllExecutorTaskByStatusType(StatusType statusType)
        {
            return (from task in CurrentContext.Tasks.Include("User")
                    join acsrqst in CurrentContext.AccessRequest
                    on task.AccessRequestId equals acsrqst.Id
                    join adhocrqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                    on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                    where task.IsActionOnlyRequiredTask &&
                    task.Action == statusType
                    select new PresentationTask
                    {
                        TaskId = task.Id,
                        RequestTypeId = task.RequestTypeId,
                        AccessRequestID = acsrqst.Id,
                        WorkFlowConfigurationId = task.WorkflowConfigurationId,
                        EventName = adhocrqst.EndPoint.DisplayName,
                        StatusType = ((StatusType)task.Action).ToString(),
                        Type = task.Approver,
                        CreatedDate = task.CreatedDate,
                        AssignedToName = task.ActionUser.DisplayName,
                        DelegatedBy = task.Delegated.DisplayName,
                        IsGroup = task.IsGroupTask,
                    }).Distinct().ToList();
        }

        public List<Task> GetEmployeeTaskByRequestState(long triggerId, byte stateId)
        {
            return CurrentContext.Tasks.Where(item => item.EmployeeEventTriggerId == triggerId && item.WorkflowStateId == stateId).ToList();
        }

        public List<Task> GetAccessRequestTaskByRequestState(long requestId, byte stateId)
        {
            return CurrentContext.Tasks.Where(item => item.AccessRequestId == requestId && item.WorkflowStateId == stateId).ToList();
        }

        public List<Task> GetBulkRequestTaskByRequestState(long bulkRequestId, byte stateId)
        {
            return CurrentContext.Tasks.Where(item => item.BulkRequestId == bulkRequestId && item.WorkflowStateId == stateId).ToList();
        }
        public IList<PresentationTask> GetTaskDetailsByActionUserIdAndTaskStatusForBulkAssign(long actionUserId, StatusType taskStatus, long groupId)
        {
            //List<long> userGroups = CurrentContext.User.Include("Groups").FirstOrDefault(item => item.Id == actionUserId).Groups.Select(item => item.Id).ToList();
            List<long> userGroups = new List<long>() { groupId };
            var actionnUser = CurrentContext.User.Where(a => a.Id == actionUserId).FirstOrDefault();
            var actionString = (actionnUser != null ? actionnUser.DisplayName : "");
            var attestationTasks = (from task in CurrentContext.Tasks
                                    join attestattion in CurrentContext.Attestations
                                        on task.AttestationId equals attestattion.Id
                                    join attCampaign in CurrentContext.AttestationCampaigns
                                        on task.AttestationCampaignId equals attCampaign.Id
                                    join user in CurrentContext.Group
                                        on task.ActionGroupId equals user.Id
                                    join assignedby in CurrentContext.User
                                           on (long)task.AssignedBy equals assignedby.Id into Adetail
                                    join assignedUser in CurrentContext.User
                                      on (long)task.AssignedTo equals assignedUser.Id into detail
                                    from assignedUser in detail.DefaultIfEmpty()
                                    from assignedby in Adetail.DefaultIfEmpty()
                                    join wfConfig in CurrentContext.WorkflowConfigurations
                                        on attestattion.WorkflowConfigurationId equals wfConfig.Id
                                    from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).DefaultIfEmpty()
                                        //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                    where (userGroups.Contains((int)task.ActionGroupId.Value) && task.IsGroupTask && !task.AssignedBy.HasValue && !task.AssignedTo.HasValue// && (task.AssignedTo == null || (task.AssignedTo != null && task.AssignedTo != actionUserId)) 
                                    && task.Action == taskStatus)
                                    select new PresentationTask
                                    {
                                        TaskId = task.Id,
                                        RequestTypeId = task.RequestTypeId,
                                        TaskSource = "Attestation",
                                        AttestationId = task.AttestationId,
                                        AttestationName = attCampaign.Attestation.AttestationName,
                                        AttestationCampaignId = task.AttestationCampaignId,
                                        CampaignName = attCampaign.CampaignName,
                                        EventName = attCampaign.CampaignName,
                                        CreatedDate = task.CreatedDate,
                                        IsActive = task.IsActive,
                                        IsCompleted = task.IsCompleted,
                                        ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                        UserName = string.Empty,
                                        AdHocAccessRequestId = null,
                                        AccessRequestID = null,
                                        RequestedBy = "System",
                                        AssignedTo = task.AssignedTo,
                                        AssignedBy = task.AssignedBy,
                                        DelegatedBy = task.DelegatedBy,
                                        AccountType = AccountType.None.ToString(),
                                        IsDelegated = task.IsDelegated,
                                        IsGroup = task.IsGroupTask,
                                        RequestedFor = user.GroupName,
                                        AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                        AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                        AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                        StartDate = task.CreatedDate,
                                        EndDate = DbFunctions.AddDays(task.CreatedDate,
                                  (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault() == null ? 1 :
                                  task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().ResponseTimeInDays
                                  //+ (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().IsEscalationRequired ?
                                  //task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().DaysToWaitAfterEscalation : 0)
                                  )),
                                        RequestedDate = task.CreatedDate,
                                        Type = attCampaign.CampaignName,
                                        ActionUser = (task.IsGroupTask && !task.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : "") : actionString,
                                        ActionUserADID = (task.IsGroupTask && !task.IsDelegated) ? (assignedUser != null ? assignedUser.UserName : "") : "",
                                        StatusType = task.Action.ToString(),
                                    }).Distinct().ToList();
            var employeeEventTasks = (from tsk in CurrentContext.Tasks
                                      join trigger in CurrentContext.EmployeeEventTriggers
                                      on tsk.EmployeeEventTriggerId equals trigger.Id
                                      join rqsteduser in CurrentContext.User
                                      on trigger.CreatedBy equals rqsteduser.UserName
                                      join rqsteruser in CurrentContext.User
                                         on trigger.UserId equals rqsteruser.Id
                                      join assignedby in CurrentContext.User
                                             on tsk.AssignedBy equals assignedby.Id into Adetail
                                      join assignedUser in CurrentContext.User
                                        on tsk.AssignedTo equals assignedUser.Id into detail
                                      from assignedUser in detail.DefaultIfEmpty()
                                      from assignedby in Adetail.DefaultIfEmpty()
                                      where (
                                      tsk.ActionUserId == actionUserId && tsk.EmployeeEventTriggerId.HasValue && (tsk.Action == taskStatus) && tsk.IsGroupTask && !tsk.AssignedBy.HasValue && !tsk.AssignedTo.HasValue)
                                      select new PresentationTask
                                      {
                                          TaskId = tsk.Id,
                                          RequestTypeId = tsk.RequestTypeId,
                                          TaskSource = "Employee Creation",
                                          AttestationId = tsk.AttestationId,
                                          AttestationName = string.Empty,
                                          EventName = trigger.EventType == EmployeeEvents.NonEmployeeCreation ? "Non Employee Creation" : (trigger.EventType == EmployeeEvents.NonEmployeeExtension ? "Non Employee Extension / Shortening" : string.Empty),
                                          CreatedDate = tsk.CreatedDate,
                                          IsActive = tsk.IsActive,
                                          IsCompleted = tsk.IsCompleted,
                                          ReponseTimeInDays = 0,
                                          UserName = string.Empty,
                                          AdHocAccessRequestId = null,
                                          AccessRequestID = null,
                                          RequestedBy = rqsteduser.DisplayName,
                                          AssignedTo = tsk.AssignedTo,
                                          AssignedBy = tsk.AssignedBy,
                                          AccountType = AccountType.None.ToString(),
                                          IsDelegated = tsk.IsDelegated,
                                          IsGroup = tsk.IsGroupTask,
                                          RequestedFor = rqsteruser.DisplayName,
                                          AssignedToName = assignedUser.DisplayName,
                                          AssignedByName = assignedby.DisplayName,
                                          StatusType = tsk.Action.ToString(),
                                          //StartDate = tsk.AdHocAccessRequest.ValidFrom,
                                          //EndDate = tsk.AdHocAccessRequest.ValidTo,
                                          RequestedDate = trigger.CreatedDate,
                                          Type = trigger.EventType == EmployeeEvents.NonEmployeeCreation ? "Non Employee Creation" : (trigger.EventType == EmployeeEvents.NonEmployeeExtension ? "Non Employee Extension / Shortening" : string.Empty)
                                      }).ToList();

            var provisioningTasks = (from tsk in CurrentContext.Tasks
                                     join acsrqst in CurrentContext.AccessRequest
                                     on tsk.AccessRequestId equals acsrqst.Id
                                     join adhocrqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                                     on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                                     join rqsteduser in CurrentContext.User
                                         on adhocrqst.RequesterUserId equals rqsteduser.Id into requester
                                     from rqsteduser in requester.DefaultIfEmpty()
                                     join rqsteruser in CurrentContext.User.Include("NBusinessRole").Include("NOrganisation")
                                        on adhocrqst.RequesteeUserId equals rqsteruser.Id
                                     join assignedby in CurrentContext.User
                                            on (long)tsk.AssignedBy equals assignedby.Id into Adetail
                                     join assignedUser in CurrentContext.User
                                       on (long)tsk.AssignedTo equals assignedUser.Id into detail
                                     from assignedUser in detail.DefaultIfEmpty()
                                     from assignedby in Adetail.DefaultIfEmpty()
                                     join wfConfig in CurrentContext.WorkflowConfigurations
                                     on tsk.WorkflowConfigurationId equals wfConfig.Id
                                     from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == tsk.WorkflowStateId).DefaultIfEmpty()
                                         //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == tsk.WorkflowStateId
                                     where (userGroups.Contains((Int32)tsk.ActionGroupId.Value) && acsrqst.AdHocAccessRequestId.HasValue && tsk.IsGroupTask && !tsk.AssignedBy.HasValue && !tsk.AssignedTo.HasValue// && (tsk.AssignedTo == null || (tsk.AssignedTo != null && tsk.AssignedTo != actionUserId)) 
                                     && tsk.Action == taskStatus)
                                     select new PresentationTask
                                     {
                                         TaskId = tsk.Id,
                                         RequestTypeId = tsk.RequestTypeId,
                                         TaskSource = "Access Request",
                                         AttestationId = tsk.AttestationId,
                                         AttestationName = string.Empty,
                                         ApplicationType = adhocrqst.EndPoint.ApplicationType,
                                         AttestationCampaignId = tsk.AttestationCampaignId,
                                         CampaignName = String.Empty,
                                         EventName = adhocrqst.EndPoint.DisplayName,
                                         CreatedDate = tsk.CreatedDate,
                                         IsActive = tsk.IsActive,
                                         IsCompleted = tsk.IsCompleted,
                                         ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                         UserName = string.Empty,
                                         AdHocAccessRequestId = adhocrqst.Id,
                                         AccessRequestID = acsrqst.Id,
                                         RequestedBy = acsrqst.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.AdhocRequest ? (rqsteduser != null ? rqsteduser.DisplayName : "System") : "System",
                                         AssignedTo = tsk.AssignedTo,
                                         AssignedBy = tsk.AssignedBy,
                                         DelegatedBy = tsk.DelegatedBy,
                                         AccountType = adhocrqst.AccountType.ToString(),
                                         IsDelegated = tsk.IsDelegated,
                                         IsGroup = tsk.IsGroupTask,
                                         RequestedFor = rqsteruser.DisplayName,
                                         RequestForDept = rqsteruser.NOrganisation != null ? rqsteruser.NOrganisation.Name : "",
                                         RequestForDesignation = rqsteruser.NBusinessRole != null ? rqsteruser.NBusinessRole.Description : "",
                                         AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                         AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                         AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                         StartDate = adhocrqst.ValidFrom,
                                         EndDate = adhocrqst.ValidTo,
                                         RequestedDate = adhocrqst.CreatedDate,
                                         Type = ((int)adhocrqst.AdhocActionType == 1 ? "Add New Account" : ((int)adhocrqst.AdhocActionType == 2) ? "Amendment Add" : ((int)adhocrqst.AdhocActionType == 3 ? "Amendment Remove" : ((int)adhocrqst.AdhocActionType == 4 ? "Delete Account" : "None"))),
                                         ActionUser = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : "") : actionString,
                                         StatusType = tsk.Action.ToString(),
                                         EntitlementXML = adhocrqst.EntitlementDetail
                                     }).Distinct().ToList();


            var tasks = attestationTasks.Union(employeeEventTasks).Union(provisioningTasks);
            var provisioningTasksFromBulkRequest = (from tsk in CurrentContext.Tasks
                                                    join blkReq in CurrentContext.BulkRequests.Include("EndPoint")
                                                    on tsk.BulkRequestId equals blkReq.Id
                                                    join rqsteduser in CurrentContext.User
                                                    on blkReq.RequestedBy equals rqsteduser.Id into requester
                                                    from rqsteduser in requester.DefaultIfEmpty()
                                                    join rqsteruser in CurrentContext.User.Include("NBusinessRole").Include("NOrganisation")
                                                    on blkReq.SelectedSupervisor equals rqsteruser.Id
                                                    join assignedby in CurrentContext.User
                                                           on (long)tsk.AssignedBy equals assignedby.Id into Adetail
                                                    join assignedUser in CurrentContext.User
                                                      on (long)tsk.AssignedTo equals assignedUser.Id into detail
                                                    from assignedUser in detail.DefaultIfEmpty()
                                                    from assignedby in Adetail.DefaultIfEmpty()
                                                    join wfConfig in CurrentContext.WorkflowConfigurations
                                                    on tsk.WorkflowConfigurationId equals wfConfig.Id
                                                    from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == tsk.WorkflowStateId).DefaultIfEmpty()
                                                        //where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId //&& wfConfigDetail.SequenceNumber == tsk.WorkflowStateId
                                                        //where (tsk.ActionUserId == actionUserId && acsrqst.AdHocAccessRequestId.HasValue && tsk.Action == taskStatus && (!tsk.IsGroupTask || (tsk.IsGroupTask && tsk.AssignedTo == actionUserId)))
                                                    where (userGroups.Contains((Int32)tsk.ActionGroupId.Value) && tsk.IsGroupTask && !tsk.AssignedBy.HasValue && !tsk.AssignedTo.HasValue//&& (tsk.AssignedTo == null || (tsk.AssignedTo != null && tsk.AssignedTo != actionUserId)) 
                                                    && tsk.Action == taskStatus)
                                                    select new PresentationTask
                                                    {
                                                        TaskId = tsk.Id,
                                                        RequestTypeId = tsk.RequestTypeId,
                                                        TaskSource = DI.Chakra.Infrastructure.Constants.BulkRequest,
                                                        ApplicationType = blkReq.EndPoint.ApplicationType,
                                                        AttestationId = tsk.AttestationId,
                                                        AttestationName = string.Empty,
                                                        AttestationCampaignId = tsk.AttestationCampaignId,
                                                        CampaignName = String.Empty,
                                                        EventName = blkReq.EndPoint.DisplayName,
                                                        CreatedDate = tsk.CreatedDate,
                                                        IsActive = tsk.IsActive,
                                                        IsCompleted = tsk.IsCompleted,
                                                        ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                                        UserName = string.Empty,
                                                        AdHocAccessRequestId = null,
                                                        AccessRequestID = blkReq.Id,
                                                        RequestedBy = blkReq.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.AdhocRequest ? (rqsteduser != null ? rqsteduser.DisplayName : "System") : "System",
                                                        AssignedTo = tsk.AssignedTo,
                                                        AssignedBy = tsk.AssignedBy,
                                                        AccountType = "User",
                                                        IsDelegated = tsk.IsDelegated,
                                                        IsGroup = tsk.IsGroupTask,
                                                        RequestedFor = "Bulk Request",
                                                        RequestForDept = rqsteruser.NOrganisation != null ? rqsteruser.NOrganisation.Name : "",
                                                        RequestForDesignation = rqsteruser.NBusinessRole != null ? rqsteruser.NBusinessRole.Description : "",
                                                        AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                                        AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                                        AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                                        StartDate = blkReq.ActivationDate,
                                                        EndDate = blkReq.DeactivationDate,
                                                        RequestedDate = blkReq.CreatedDate,
                                                        Type = ((int)blkReq.AdhocActionType == 1 ? "Add New Account" : ((int)blkReq.AdhocActionType == 2) ? "Amendment Add" : ((int)blkReq.AdhocActionType == 3 ? "Amendment Remove" : ((int)blkReq.AdhocActionType == 4 ? "Delete Account" : "None"))),
                                                        StatusType = tsk.Action.ToString(),
                                                        RequestReason = blkReq.Comments, //acsrqst.Comments ,
                                                        DelegatedBy = tsk.DelegatedBy,
                                                        DelegatedComment = tsk.DelegationComments,
                                                        ActionUser = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : "") : actionString,
                                                        ActionUserADID = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.UserName : "") : "",
                                                        BulkReqDetails = blkReq.BulkRequestDetails,
                                                        EntitlementXML = blkReq.EntitlementDetail
                                                    }).ToList();

            foreach (var item in provisioningTasksFromBulkRequest)
            {
                item.RequestedFor = string.Join(",", item.BulkReqDetails.Select(i => i.Name));
            }
            tasks = tasks.Union(provisioningTasksFromBulkRequest);
            tasks = tasks.OrderByDescending(rvw => rvw.CreatedDate).ThenBy(rvw1 => rvw1.IsCompleted);

            return tasks.ToList();
        }
        public bool IsTaskExistsForAccessRequestAttestationRemediation(long accessrequestId)
        {
            return CurrentContext.Tasks.FirstOrDefault(i => i.AccessRequestId == accessrequestId && i.AttestationCampaignId.HasValue) == null;
        }

        public List<Task> GetAllPendingAttestationTask()
        {
            var task = (from tsk in CurrentContext.Tasks
                    join wf in CurrentContext.WorkflowConfigurations on tsk.WorkflowConfigurationId equals wf.Id
                    join wfd in CurrentContext.WorkflowConfigurationDetails on wf.Id equals wfd.WorkflowConfigurationId
                    where tsk.AttestationId != null && tsk.AttestationCampaignId != null && tsk.Action == StatusType.Pending && tsk.WorkflowStateId == wfd.SequenceNumber && (wfd.IsReminderRequired || wfd.IsEscalationRequired)
                    select tsk.Id
                    );
            return CurrentContext.Tasks.Include(i => i.WorkFlowConfiguration).Include("WorkFlowConfiguration.WorkflowConfigurationDetails").Where
                (i => task.Contains(i.Id)).ToList();
        }
        public List<Task> GetAllPendingProvisioningTaskForReminder()
        {
            var task = (from tsk in CurrentContext.Tasks
                        join wf in CurrentContext.WorkflowConfigurations on tsk.WorkflowConfigurationId equals wf.Id
                        join wfd in CurrentContext.WorkflowConfigurationDetails on wf.Id equals wfd.WorkflowConfigurationId
                        where tsk.AttestationId == null && tsk.AttestationCampaignId == null && tsk.Action == StatusType.Pending && tsk.WorkflowStateId == wfd.SequenceNumber && (wfd.IsReminderRequired || wfd.IsEscalationRequired)
                        select tsk.Id
                    );
            return CurrentContext.Tasks.Include(i => i.WorkFlowConfiguration).Include("WorkFlowConfiguration.WorkflowConfigurationDetails").Where
                (i => task.Contains(i.Id)).ToList();
        }
        public PresentationGridPagedData<EntityModel.Presentation.PresentationTask> GetPagedResult(DateTime? dtFrom, DateTime? dtTo, int startIndex, int pageSize, string filterExpression, string sortExpression, long requestFor = 0, long endpointSystemId = 0, StatusType statusType = StatusType.None, long actionUserId = 0)
        {
            PresentationGridPagedData<EntityModel.Presentation.PresentationTask> results = new PresentationGridPagedData<EntityModel.Presentation.PresentationTask>();
            var currentTask = from tsk in CurrentContext.Tasks
                              where statusType == StatusType.None ? true : tsk.Action == statusType
                              select tsk;
            var userQuery = from reqFor in CurrentContext.User
                            where (requestFor == 0 ? true : reqFor.Id == requestFor)
                            select reqFor;
            
            IQueryable<AccessRequest> currentAccessRequest = from accessReq in CurrentContext.AccessRequest
                                                             where (requestFor == 0 ? true : accessReq.UserId == requestFor)
                                                             && (endpointSystemId > 0) ? accessReq.EndpointSystemId == endpointSystemId : true
                                                             select accessReq;
            IQueryable<PresentationTask> attestationTasksForUser = null;
            IQueryable<PresentationTask> attestationTasksForGroup = null;
            IQueryable<PresentationTask> attestationTasks = null;
            IQueryable<PresentationTask> tasks = null;
            if (requestFor == 0)
            {

                attestationTasksForUser = (from task in currentTask
                                           join attestattion in CurrentContext.Attestations
                                               on task.AttestationId equals attestattion.Id
                                           join attCampaign in CurrentContext.AttestationCampaigns
                                               on task.AttestationCampaignId equals attCampaign.Id
                                           join user in CurrentContext.User
                                               on task.ActionUserId equals user.Id
                                           join assignedby in CurrentContext.User
                                                  on (long)task.AssignedBy equals assignedby.Id into Adetail
                                           join assignedUser in CurrentContext.User
                                             on (long)task.AssignedTo equals assignedUser.Id into detail
                                           from assignedUser in detail.DefaultIfEmpty()
                                           from assignedby in Adetail.DefaultIfEmpty()
                                           join wfConfig in CurrentContext.WorkflowConfigurations
                                               on attestattion.WorkflowConfigurationId equals wfConfig.Id
                                           from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).DefaultIfEmpty()
                                           where !task.IsGroupTask && (!dtFrom.HasValue || task.CreatedDate >= dtFrom.Value) && (!dtTo.HasValue || task.CreatedDate <= dtTo.Value)
                                           && (actionUserId > 0 ? (((task.ActionUserId == actionUserId && !task.IsGroupTask) || ((task.IsGroupTask && task.AssignedTo == actionUserId)))) : true)
                                           select new PresentationTask
                                           {
                                               TaskId = task.Id,
                                               RequestTypeName = "Attestation",
                                               RequestTypeId = task.RequestTypeId,
                                               TaskSource = "Attestation",
                                               AttestationId = task.AttestationId,
                                               AttestationName = attestattion.AttestationName,
                                               AttestationCampaignId = task.AttestationCampaignId,
                                               CampaignName = attCampaign.CampaignName,
                                               EventName = attCampaign.CampaignName,
                                               CreatedDate = task.CreatedDate,
                                               IsActive = task.IsActive,
                                               IsCompleted = task.IsCompleted,
                                               ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                               AdHocAccessRequestId = 0,
                                               RequestedBy = "System",
                                               AssignedTo = task.AssignedTo,
                                               AssignedBy = task.AssignedBy,
                                               AccountType = AccountType.None.ToString(),
                                               IsDelegated = task.IsDelegated,
                                               IsDelegatedByUser = task.IsDelegated ? "Yes" : "No",
                                               IsGroup = task.IsGroupTask,
                                               RequestedFor = attestattion.AttestationName,
                                               AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                               AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                               AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                               StartDate = task.CreatedDate,
                                               EndDate = task.IsCompleted ? task.ActionDate : DbFunctions.AddDays(task.CreatedDate,
                                         (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault() == null ? 1 :
                                  task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().ResponseTimeInDays
                                         )),
                                               TaskAccessRequest = DI.Chakra.Infrastructure.Enums.AccessRequestStatus.None,
                                               AccessRequestID = null,
                                               Type = attCampaign.CampaignName,
                                               StatusType = task.Action.ToString(),
                                               RequestReason = string.Empty,
                                               DelegatedBy = task.DelegatedBy,
                                               DelegatedComment = task.DelegationComments,
                                               UserName = user.UserName,
                                               Approver = task.Approver,
                                               ActionUser = user.DisplayName,
                                               ActionUserADID = user.UserName,
                                               RequestedDate = task.CreatedDate,
                                               EntitlementXML = null,
                                               ApplicationType = ApplicationType.None,
                                               GroupName = null,
                                               CompletionDate = task.ActionDate
                                           });


                attestationTasksForGroup = (from task in currentTask
                                            join attestattion in CurrentContext.Attestations
                                                on task.AttestationId equals attestattion.Id
                                            join attCampaign in CurrentContext.AttestationCampaigns
                                                on task.AttestationCampaignId equals attCampaign.Id
                                            join grp in CurrentContext.Group
                                                on task.ActionGroupId equals grp.Id
                                            join assignedby in CurrentContext.User
                                                   on (long)task.AssignedBy equals assignedby.Id into Adetail
                                            join assignedUser in CurrentContext.User
                                              on (long)task.AssignedTo equals assignedUser.Id into detail
                                            join actionuser in CurrentContext.User
                                             on task.ActionUserId equals actionuser.Id into actionUser
                                            from assignedUser in detail.DefaultIfEmpty()
                                            from assignedby in Adetail.DefaultIfEmpty()
                                            from actionuser in actionUser.DefaultIfEmpty()
                                            join wfConfig in CurrentContext.WorkflowConfigurations
                                                on attestattion.WorkflowConfigurationId equals wfConfig.Id
                                            from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).DefaultIfEmpty()
                                            where task.IsGroupTask && (!dtFrom.HasValue || task.CreatedDate >= dtFrom.Value) && (!dtTo.HasValue || task.CreatedDate <= dtTo.Value) &&
                                            //2802 - fixed
                                            (actionUserId > 0 ? ((task.ActionUserId == actionUserId && !task.IsGroupTask) || (task.IsGroupTask && task.AssignedTo == actionUserId)) : true)
                                            select new PresentationTask
                                            {
                                                TaskId = task.Id,
                                                RequestTypeName = "Attestation",
                                                RequestTypeId = task.RequestTypeId,
                                                TaskSource = "Attestation",
                                                AttestationId = task.AttestationId,
                                                AttestationName = attestattion.AttestationName,
                                                AttestationCampaignId = task.AttestationCampaignId,
                                                CampaignName = attCampaign.CampaignName,
                                                EventName = attCampaign.CampaignName,
                                                CreatedDate = task.CreatedDate,
                                                IsActive = task.IsActive,
                                                IsCompleted = task.IsCompleted,
                                                ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                                AdHocAccessRequestId = 0,
                                                RequestedBy = "System",
                                                AssignedTo = task.AssignedTo,
                                                AssignedBy = task.AssignedBy,
                                                AccountType = AccountType.None.ToString(),
                                                IsDelegated = task.IsDelegated,
                                                IsDelegatedByUser = task.IsDelegated ? "Yes" : "No",
                                                IsGroup = task.IsGroupTask,
                                                RequestedFor = attestattion.AttestationName,
                                                AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                                AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                                AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                                    StartDate = task.CreatedDate,
                                                    EndDate = task.IsCompleted ? task.ActionDate : DbFunctions.AddDays(task.CreatedDate,
                                  (task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault() == null ? 1 :
                                  task.WorkFlowConfiguration.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == task.WorkflowStateId).FirstOrDefault().ResponseTimeInDays

                                  )),
                                                TaskAccessRequest = DI.Chakra.Infrastructure.Enums.AccessRequestStatus.None,
                                                AccessRequestID = null,
                                                Type = attCampaign.CampaignName,
                                                StatusType = task.Action.ToString(),
                                                RequestReason = string.Empty,
                                                DelegatedBy = task.DelegatedBy,
                                                DelegatedComment = task.DelegationComments,
                                                UserName = task.Approver,
                                                Approver = task.Approver,
                                                ActionUser = task.IsDelegated ? actionuser.DisplayName : (assignedUser != null ? assignedUser.DisplayName : grp != null ? grp.GroupName : ""),
                                                ActionUserADID = task.IsDelegated ? actionuser.UserName : (assignedUser != null ? assignedUser.UserName : ""),
                                                RequestedDate = task.CreatedDate,
                                                EntitlementXML = null,
                                                ApplicationType = ApplicationType.None,
                                                GroupName = null,
                                                CompletionDate = task.ActionDate
                                            });

                attestationTasks = attestationTasksForGroup.Union(attestationTasksForUser);
                if (endpointSystemId > 0)
                {
                    var attEnd = CurrentContext.AttestationEndpoints.Where(i => i.EndpointSystemId == endpointSystemId).Select(i => i.AttestationId).Distinct();
                    attestationTasks = attestationTasks.Where(i => attEnd.Contains(i.AttestationId.Value));
                }
            }
            var provisioningTasks = (from tsk in currentTask
                                     join acsrqst in currentAccessRequest
                                     on tsk.AccessRequestId equals acsrqst.Id
                                     join adhocrqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                                     on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                                     join rqsteduser in CurrentContext.User
                                        on adhocrqst.RequesterUserId equals rqsteduser.Id into requester
                                     from rqsteduser in requester.DefaultIfEmpty()
                                     join rqsteruser in userQuery//.Include("NBusinessRole").Include("NOrganisation")
                                        on adhocrqst.RequesteeUserId equals rqsteruser.Id
                                     join actionuser in CurrentContext.User
                                     on (long)tsk.ActionUserId equals actionuser.Id into actionUser
                                     from actionuser in actionUser.DefaultIfEmpty()
                                     join assignedby in CurrentContext.User
                                           on (long)tsk.AssignedBy equals assignedby.Id into Adetail
                                     join assignedUser in CurrentContext.User
                                       on (long)tsk.AssignedTo equals assignedUser.Id into detail
                                     from assignedUser in detail.DefaultIfEmpty()
                                     from assignedby in Adetail.DefaultIfEmpty()
                                     join wfConfig in CurrentContext.WorkflowConfigurations
                                     on tsk.WorkflowConfigurationId equals wfConfig.Id
                                     from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == tsk.WorkflowStateId).DefaultIfEmpty()
                                     join actiongroup in CurrentContext.Group
                                     on (long)tsk.ActionGroupId equals actiongroup.Id into actionGroup
                                     from actiongroup in actionGroup.DefaultIfEmpty()

                                     where 1 == 1 && (!dtFrom.HasValue || tsk.CreatedDate >= dtFrom.Value) && (!dtTo.HasValue || tsk.CreatedDate <= dtTo.Value)
                                     && (actionUserId > 0 ? ((tsk.ActionUserId == actionUserId && !tsk.IsGroupTask) || (tsk.IsGroupTask && tsk.AssignedTo == actionUserId)): true)
                                     select new PresentationTask
                                     {
                                         TaskId = tsk.Id,
                                         RequestTypeName = "Provisioning",
                                         RequestTypeId = tsk.RequestTypeId,
                                         TaskSource = tsk.AttestationCampaignId != null ? "Attestation Remediation" : "Access Request",
                                         AttestationId = tsk.AttestationId,
                                         AttestationName = string.Empty,
                                         AttestationCampaignId = tsk.AttestationCampaignId,
                                         CampaignName = String.Empty,
                                         EventName = adhocrqst.EndPoint.DisplayName,
                                         CreatedDate = tsk.CreatedDate,
                                         IsActive = tsk.IsActive,
                                         IsCompleted = tsk.IsCompleted,
                                         ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                         AdHocAccessRequestId = adhocrqst.Id,
                                         RequestedBy = acsrqst.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.AdhocRequest ? (rqsteduser != null ? rqsteduser.DisplayName : "System") : "System",
                                         AssignedTo = tsk.AssignedTo,
                                         AssignedBy = tsk.AssignedBy,
                                         AccountType = adhocrqst.AccountType.ToString(),
                                         IsDelegated = tsk.IsDelegated,
                                         IsDelegatedByUser = tsk.IsDelegated ? "Yes" : "No",
                                         IsGroup = tsk.IsGroupTask,
                                         RequestedFor = rqsteruser.DisplayName,
                                         AssignedToName = string.Empty,
                                         AssignedToADID = string.Empty,
                                         AssignedByName = String.Empty,
                                         StartDate = acsrqst.ActivationDate,
                                         EndDate = acsrqst.DeactivationDate,
                                         TaskAccessRequest = acsrqst.RequestStatus,
                                         AccessRequestID = acsrqst.Id,
                                         Type = ((int)adhocrqst.AdhocActionType == 1 ? "Add New Account" : ((int)adhocrqst.AdhocActionType == 2) ? "Amendment Add" : ((int)adhocrqst.AdhocActionType == 3 ? "Amendment Remove" : ((int)adhocrqst.AdhocActionType == 4 ? "Delete Account" : "None"))),
                                         StatusType = tsk.Action.ToString(),
                                         RequestReason = adhocrqst.Comments, //acsrqst.Comments ,
                                         DelegatedBy = tsk.DelegatedBy,
                                         DelegatedComment = tsk.DelegationComments,
                                         UserName = rqsteduser.UserName,
                                         Approver = tsk.Approver,
                                         ActionUser = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : tsk.Approver) : actionuser.DisplayName,
                                         ActionUserADID = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.UserName : "") : actionuser.UserName,
                                         RequestedDate = acsrqst.CreatedDate,
                                         EntitlementXML = adhocrqst.EntitlementDetail,
                                         ApplicationType = acsrqst.ApplicationType,
                                         GroupName = actiongroup.GroupName,
                                         CompletionDate = tsk.ActionDate
                                     }).Distinct();

            var provisioningTasksFromBulkRequest = (from tsk in currentTask
                                                    join blkReq in CurrentContext.BulkRequests.Include("EndPoint")
                                                    on tsk.BulkRequestId equals blkReq.Id
                                                    join blkReqDetail in CurrentContext.BulkRequestDetails on blkReq.Id equals blkReqDetail.BulkRequestId
                                                    join rqsteduser in CurrentContext.User
                                                    on blkReq.RequestedBy equals rqsteduser.Id into requester
                                                    from rqsteduser in requester.DefaultIfEmpty()
                                                    join rqsteruser in CurrentContext.User//.Include("NBusinessRole").Include("NOrganisation")
                                                    on blkReq.SelectedSupervisor equals rqsteruser.Id
                                                    join assignedby in CurrentContext.User
                                                           on (long)tsk.AssignedBy equals assignedby.Id into Adetail
                                                    join assignedUser in CurrentContext.User
                                                      on (long)tsk.AssignedTo equals assignedUser.Id into detail
                                                    from assignedUser in detail.DefaultIfEmpty()
                                                    from assignedby in Adetail.DefaultIfEmpty()
                                                    join wfConfig in CurrentContext.WorkflowConfigurations
                                                    on tsk.WorkflowConfigurationId equals wfConfig.Id
                                                    join actionUser in CurrentContext.User on tsk.ActionUserId equals actionUser.Id into actUser
                                                    from actionUser in actUser.DefaultIfEmpty()
                                                    from wfConfigDetail in wfConfig.WorkflowConfigurationDetails.Where(a => a.SequenceNumber == tsk.WorkflowStateId).DefaultIfEmpty()
                                                    where ((requestFor == 0 ? true : blkReqDetail.UserId == requestFor)
                              && (endpointSystemId > 0) ? blkReq.EndpointSystemId == endpointSystemId : true) &&
                                                     (!dtFrom.HasValue || tsk.CreatedDate >= dtFrom.Value) && (!dtTo.HasValue || tsk.CreatedDate <= dtTo.Value)
                                                     && (requestFor == 0 ? true : blkReq.BulkRequestDetails.Select(i => i.UserId).Contains(requestFor))
                                                     && (actionUserId > 0 ? (((tsk.ActionUserId == actionUserId && !tsk.IsGroupTask) || ((tsk.IsGroupTask && tsk.AssignedTo == actionUserId)))) : true)
                                                    select new PresentationTask
                                                    {
                                                        TaskId = tsk.Id,
                                                        RequestTypeName = "Bulk Request",
                                                        RequestTypeId = tsk.RequestTypeId,
                                                        TaskSource = DI.Chakra.Infrastructure.Constants.BulkRequest,
                                                        AttestationId = tsk.AttestationId,
                                                        AttestationName = string.Empty,
                                                        AttestationCampaignId = tsk.AttestationCampaignId,
                                                        CampaignName = String.Empty,
                                                        EventName = blkReq.EndPoint.DisplayName,
                                                        CreatedDate = tsk.CreatedDate,
                                                        IsActive = tsk.IsActive,
                                                        IsCompleted = tsk.IsCompleted,
                                                        ReponseTimeInDays = wfConfigDetail == null ? 1 : wfConfigDetail.ResponseTimeInDays,
                                                        AdHocAccessRequestId = null,
                                                        RequestedBy = blkReq.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.AdhocRequest ? (rqsteduser != null ? rqsteduser.DisplayName : "System") : "System",
                                                        AssignedTo = tsk.AssignedTo,
                                                        AssignedBy = tsk.AssignedBy,
                                                        AccountType = "User",
                                                        IsDelegated = tsk.IsDelegated,
                                                        IsDelegatedByUser = tsk.IsDelegated ? "Yes" : "No",
                                                        IsGroup = tsk.IsGroupTask,
                                                        RequestedFor = blkReqDetail.Name,
                                                        AssignedToName = (assignedUser != null ? assignedUser.DisplayName : ""),
                                                        AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                                        AssignedByName = (assignedby != null ? assignedby.DisplayName : ""),
                                                        StartDate = blkReq.ActivationDate,
                                                        EndDate = blkReq.DeactivationDate,
                                                        TaskAccessRequest = blkReq.RequestStatus,
                                                        AccessRequestID = blkReq.Id,
                                                        Type = ((int)blkReq.AdhocActionType == 1 ? "Add New Account" : ((int)blkReq.AdhocActionType == 2) ? "Amendment Add" : ((int)blkReq.AdhocActionType == 3 ? "Amendment Remove" : ((int)blkReq.AdhocActionType == 4 ? "Delete Account" : "None"))),
                                                        StatusType = tsk.Action.ToString(),
                                                        RequestReason = blkReq.Comments, //acsrqst.Comments ,
                                                        DelegatedBy = tsk.DelegatedBy,
                                                        DelegatedComment = tsk.DelegationComments,
                                                        UserName = string.Empty,
                                                        Approver = tsk.Approver,
                                                        ActionUser = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : tsk.Approver) : actionUser.DisplayName,
                                                        ActionUserADID = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.UserName : "") : "",
                                                        RequestedDate = blkReq.CreatedDate,
                                                        EntitlementXML = blkReq.EntitlementDetail,
                                                        ApplicationType = blkReq.EndPoint.ApplicationType,
                                                        GroupName = null,
                                                        CompletionDate = tsk.ActionDate
                                                    });


            IQueryable<PresentationTask> nonEmployeeEventTasksQuery = null;
            if (endpointSystemId == 0)
            {
                var empTrigger = CurrentContext.EmployeeEventTriggers.Where(i => i.EventType == EmployeeEvents.NonEmployeeExtension || i.EventType == EmployeeEvents.NonEmployeeCreation);
                currentTask = currentTask.Where(tsk => tsk.EmployeeEventTriggerId.HasValue && (!dtFrom.HasValue || tsk.CreatedDate >= dtFrom.Value) && (!dtTo.HasValue || tsk.CreatedDate <= dtTo.Value));

                nonEmployeeEventTasksQuery = (from trigger in empTrigger
                                              join tsk in currentTask on trigger.Id equals tsk.EmployeeEventTriggerId
                                              join rqsteduser in CurrentContext.User
                                              on trigger.CreatedBy equals rqsteduser.UserName
                                              join rqsteruser in userQuery
                                                 on trigger.UserId equals rqsteruser.Id
                                              join actionuser in CurrentContext.User
                                                on (long)tsk.ActionUserId equals actionuser.Id into actionUser
                                              from actionuser in actionUser.DefaultIfEmpty()
                                              join assignedby in CurrentContext.User
                                                     on tsk.AssignedBy equals assignedby.Id into Adetail
                                              join assignedUser in CurrentContext.User
                                                on tsk.AssignedTo equals assignedUser.Id into detail
                                              from assignedUser in detail.DefaultIfEmpty()
                                              from assignedby in Adetail.DefaultIfEmpty()
                                              where (actionUserId > 0 ? ((tsk.ActionUserId == actionUserId && !tsk.IsGroupTask) || ((tsk.IsGroupTask && tsk.AssignedTo == actionUserId))):true)
                                              select new PresentationTask
                                              {
                                                  TaskId = tsk.Id,
                                                  RequestTypeName = "Non Employee",
                                                  RequestTypeId = tsk.RequestTypeId,
                                                  TaskSource = "Employee Creation",
                                                  AttestationId = tsk.AttestationId,
                                                  AttestationName = string.Empty,
                                                  AttestationCampaignId = null,
                                                  CampaignName = null,
                                                  EventName = trigger.EventType == EmployeeEvents.NonEmployeeCreation ? "Non Employee Creation" : (trigger.EventType == EmployeeEvents.NonEmployeeExtension ? "Non Employee Extension / Shortening" : string.Empty),
                                                  CreatedDate = tsk.CreatedDate,
                                                  IsActive = tsk.IsActive,
                                                  IsCompleted = tsk.IsCompleted,
                                                  ReponseTimeInDays = 0,
                                                  AdHocAccessRequestId = null,
                                                  RequestedBy = rqsteduser.DisplayName,
                                                  AssignedTo = tsk.AssignedTo,
                                                  AssignedBy = tsk.AssignedBy,
                                                  AccountType = AccountType.None.ToString(),
                                                  IsDelegated = tsk.IsDelegated,
                                                  IsDelegatedByUser = tsk.IsDelegated ? "Yes" : "No",
                                                  IsGroup = tsk.IsGroupTask,
                                                  RequestedFor = rqsteruser.DisplayName,
                                                  AssignedToName = assignedUser.DisplayName,
                                                  AssignedToADID = (assignedUser != null ? assignedUser.UserName : ""),
                                                  AssignedByName = assignedby.DisplayName,
                                                  StartDate = null,
                                                  EndDate = null,
                                                  TaskAccessRequest = DI.Chakra.Infrastructure.Enums.AccessRequestStatus.None,
                                                  AccessRequestID = null,
                                                  Type = trigger.EventType == EmployeeEvents.NonEmployeeCreation ? "Non Employee Creation" : (trigger.EventType == EmployeeEvents.NonEmployeeExtension ? "Non Employee Extension / Shortening" : string.Empty),
                                                  StatusType = tsk.Action.ToString(),
                                                  RequestReason = string.Empty,
                                                  DelegatedBy = tsk.DelegatedBy,
                                                  DelegatedComment = tsk.DelegationComments,
                                                  UserName = string.Empty,
                                                  Approver = tsk.Approver,
                                                  ActionUser = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.DisplayName : tsk.Approver) : actionuser.DisplayName,
                                                  ActionUserADID = (tsk.IsGroupTask && !tsk.IsDelegated) ? (assignedUser != null ? assignedUser.UserName : "") : actionuser.UserName,
                                                  RequestedDate = trigger.CreatedDate,
                                                  EntitlementXML = null,
                                                  ApplicationType = ApplicationType.None,
                                                  GroupName = null,
                                                  CompletionDate = tsk.ActionDate
                                              });
            }
            //foreach (var item in provisioningTasksFromBulkRequest)
            //{
            //    item.RequestedFor = string.Join(",", item.BulkReqDetails.Select(i => i.Name));
            //}
            if (attestationTasks != null)
            {
                tasks = attestationTasks.Union(provisioningTasks).Union(provisioningTasksFromBulkRequest);
            }
            else
            {
                tasks = provisioningTasks.Union(provisioningTasksFromBulkRequest);
            }

            if (nonEmployeeEventTasksQuery != null)
            {
                tasks = tasks.Union(nonEmployeeEventTasksQuery);
            }
            tasks = tasks.Distinct();

            if (!string.IsNullOrWhiteSpace(filterExpression))
            {
                tasks = tasks.Where(filterExpression);
            }

            if (!string.IsNullOrWhiteSpace(sortExpression))
            {
                tasks = tasks.OrderBy(sortExpression);
            }
            else
            {
                tasks = tasks.OrderByDescending(rvw => rvw.CreatedDate).ThenBy(rvw1 => rvw1.IsCompleted);
            }
            results.TotalCount = tasks.Count();
            if(pageSize == -1)
            {
                results.Data = tasks.ToList();
            }
            else
            {
                results.Data = tasks.Skip(startIndex).Take(pageSize).ToList();
            }
            
            return results;
        }
    }
}