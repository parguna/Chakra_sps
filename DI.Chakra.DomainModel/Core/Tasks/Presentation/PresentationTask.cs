using AutoMapper;
using DI.Chakra.DomainModel.Core;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationTask
    {
        public long TaskId { get; set; }

        public byte RequestTypeId { get; set; }

        public long UserId { get; set; }

        public string UserName { get; set; }

        public string NRIC { get; set; }

        public Nullable<long> AttestationId { get; set; }

        public Nullable<long> AttestationCampaignId { get; set; }
        public string Approver { get; set; }
        public string CampaignName { get; set; }

        public string AttestationName { get; set; }

        public string EventName { get; set; }

        public DateTime CreatedDate { get; set; }

        public int ReponseTimeInDays { get; set; }

        public bool IsActive { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public string RequestedBy { get; set; }

        public Nullable<long> AdHocAccessRequestId { get; set; }

        public Nullable<long> AccessRequestID { get; set; }

        public Nullable<long> AssignedTo { get; set; }

        public Nullable<long> AssignedBy { get; set; }

        public string RequestedFor { get; set; }

        public string Action { get; set; }

        public string AccountType { get; set; }

        public bool IsDelegated { get; set; }

        public string AssignedToName { get; set; }

        public string AssignedByName { get; set; }

        public string DelegatedBy { get; set; }

        public string DelegatedComment { get; set; }

        public bool IsGroup { get; set; }

        public DI.Chakra.Infrastructure.Enums.AccessRequestStatus TaskAccessRequest { get; set; }

        public DateTime? RequestedDate { get; set; }

        public DateTime? CompletionDate { get; set; }

        public string Type { get; set; }

        public string StatusType { get; set; }

        public string RequestReason { get; set; }
        public long Level { get; set; }

        public Nullable<long> WorkFlowConfigurationId { get; set; }

        public string IsDelegate
        {
            get
            {
                if (IsDelegated)
                    return "Yes";
                else
                    return "No";
            }
        }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string Description { get; set; }
        public string Comments { get; set; }
        //public string Entitlement
        //{
        //    get
        //    {
        //        return DeserializeEntitlement(this.AdHocAccessRequestId.Value);
        //    }
        //}

        public int RequesterId { get; set; }
        public int RequesteeId { get; set; }
        public string ActionUser { get; set; }
        public string GroupName { get; set; }
        public string AssignedToADID { get; set; }
        public string ActionUserADID { get; set; }
        public string RequestForSupDisplayName { get; set; }
        public string RequestForDept { get; set; }
        public string RequestForDesignation { get; set; }
        public string TaskSource { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public string EntitlementXML { get; set; }
        //public string DeserializeEntitlement(long AdhocRequestId)
        //{
        //    if (AdhocRequestId > 0)
        //    {
        //        AdHocAccessRequest adHocAccessRequest = AdHocAccessRequest.GetById(AdhocRequestId);
        //        if (!String.IsNullOrEmpty(adHocAccessRequest.EntitlementDetail))
        //            return EndPointFactory.GetEndPoint(adHocAccessRequest.EndpointSystemType).GetEntitlementDetail(adHocAccessRequest.EntitlementDetail).Entitlement;
        //    }
        //    return "";
        //}

        //private string JoinedEntitlementAdHocVPNRequestDetail(List<AdHocVPNRequestDetail> adHocVPNRequestDetail)
        //{
        //    string strItems = "";
        //    foreach (var items in adHocVPNRequestDetail)
        //    {
        //        strItems += (strItems == "") ? items.ApplicationName : "," + items.ApplicationName;
        //    }
        //    return strItems;
        //}

        public string IsDelegatedByUser { get; set; }
        public string RequestType
        {
            get
            {
                return UtilityMethods.GetEnumAttribute<Description>((RequestType)this.RequestTypeId);
            }
        }
        public string RequestTypeName
        {
              get; set;
        }
        public string TaskName
        {
            get
            {
                if (this.AttestationId.HasValue)
                {
                    return this.CampaignName;
                }
                else 
                {
                    return this.EventName;
                }
                //Commented
                //else if (Type == "Non Employee Creation" || Type == "Non Employee Extension / Shortening")
                //{
                    
                //}
                //else if (!string.IsNullOrWhiteSpace(this.TaskSource) && TaskSource.Equals(DI.Chakra.Infrastructure.Constants.BulkRequest,StringComparison.InvariantCultureIgnoreCase))
                //{
                //    return this.EventName;
                //}
                //return string.Empty;
            }
        }


        public static PresentationTask ORMap(EntityModel.Presentation.PresentationTask presentationTaskGridDataEntityModel)
        {
            if (presentationTaskGridDataEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationTask, PresentationTask>(presentationTaskGridDataEntityModel);
        }

        public static List<PresentationTask> ORMapList(IList<EntityModel.Presentation.PresentationTask> presentationTaskGridDataEntityModels)
        {
            return presentationTaskGridDataEntityModels.Select(ORMap).ToList();
        }
               
        public static List<PresentationTask> GetAutoCompletedTaskDetails(long actionUserId, DateTime fromDate)
        {
            List<PresentationTask> attestationTasks = new List<PresentationTask>();
            List<PresentationTask> provisioningTasks = new List<PresentationTask>();

            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                if (actionUserId != 0)
                {
                    attestationTasks = ORMapList(new TaskRepository(dataAccessTransaction).GetAttestationTasksByActionUserId(actionUserId, fromDate));

                    provisioningTasks = ORMapList(new TaskRepository(dataAccessTransaction).GetProvisioningTasksByActionUserId(actionUserId, fromDate));
                }
                else
                {
                    attestationTasks = ORMapList(new TaskRepository(dataAccessTransaction).GetAttestationTasks(fromDate));

                    provisioningTasks = ORMapList(new TaskRepository(dataAccessTransaction).GetProvisioningTasks(fromDate));
                }

                return attestationTasks.Union(provisioningTasks).OrderByDescending(rvw => rvw.CreatedDate).ToList();

            }
        }

        public static List<PresentationTask> GetPendingTaskDetails(long actionUserId, DateTime fromDate)
        {
            List<PresentationTask> attestationTasks = new List<PresentationTask>();
            List<PresentationTask> provisioningTasks = new List<PresentationTask>();

            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                if (actionUserId != 0)
                {
                    attestationTasks = ORMapList(new TaskRepository(dataAccessTransaction).GetPendingAttestationTasksByActionUserId(actionUserId, fromDate));

                    provisioningTasks = ORMapList(new TaskRepository(dataAccessTransaction).GetPendingProvisioningTasksByActionUserId(actionUserId, fromDate));
                }
                else
                {
                    attestationTasks = ORMapList(new TaskRepository(dataAccessTransaction).GetPendingAttestationTasks(fromDate));

                    provisioningTasks = ORMapList(new TaskRepository(dataAccessTransaction).GetPendingProvisioningTasks(fromDate));
                }
                return attestationTasks.Union(provisioningTasks).OrderByDescending(rvw => rvw.CreatedDate).ToList();
            }
        }

        public static List<PresentationTask> GetCompletedTaskDetails(long actionUserId, DateTime fromDate)
        {
            List<PresentationTask> attestationTasks = new List<PresentationTask>();
            List<PresentationTask> provisioningTasks = new List<PresentationTask>();

            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                if (actionUserId != 0)
                {
                    attestationTasks = ORMapList(new TaskRepository(dataAccessTransaction).GetCompletedAttestationTasksByActionUserId(actionUserId, fromDate));

                    provisioningTasks = ORMapList(new TaskRepository(dataAccessTransaction).GetCompletedProvisioningTasksByActionUserId(actionUserId, fromDate));
                }
                else
                {
                    attestationTasks = ORMapList(new TaskRepository(dataAccessTransaction).GetCompletedAttestationTasks(fromDate));

                    provisioningTasks = ORMapList(new TaskRepository(dataAccessTransaction).GetCompletedProvisioningTasks(fromDate));
                }
                return attestationTasks.Union(provisioningTasks).OrderByDescending(rvw => rvw.CreatedDate).ToList();
            }

        }

        public static List<PresentationTask> GetByStatus(StatusType taskStatus, DateTime? dtFrom, DateTime? dtTo, long requestFor = 0, ApplicationType applicationType = ApplicationType.None)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new TaskRepository(dataAccessTransaction).GetByStatus(taskStatus, dtFrom, dtTo, requestFor, applicationType));
            }
        }

        public static List<PresentationTask> GetAll(DateTime? dtFrom, DateTime? DtTo, long requestFor = 0, long endpointSystemId = 0, StatusType statusType = DI.Chakra.Infrastructure.Enums.StatusType.None)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new TaskRepository(dataAccessTransaction).GetAll(dtFrom,DtTo, requestFor, endpointSystemId, statusType));
            }
        }

        public static List<PresentationTask> GetAllExecutorTask()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                List<PresentationTask> pendingTasksList = ORMapList(new TaskRepository(dataAccessTransaction).GetAllExecutorTask());                
                return pendingTasksList;
            }
        }

        public static List<PresentationTask> GetAllExecutorTaskReport(DateTime? fromDate, DateTime? toDate, StatusType statusType)
        {
            List<PresentationTask> pendingTasksList = new List<PresentationTask>();

            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                toDate = fromDate.HasValue && toDate.HasValue == false ? DateTime.Now : toDate;
                if (fromDate.HasValue && toDate.HasValue && statusType.ToString() != "None")
                {
                    pendingTasksList = ORMapList(new TaskRepository(dataAccessTransaction).GetAllExecutorTaskByDate(fromDate, toDate, statusType));
                }

                else if (fromDate.HasValue && toDate.HasValue)
                {
                    pendingTasksList = ORMapList(new TaskRepository(dataAccessTransaction).GetAllExecutorTaskByDateTime(fromDate, fromDate));

                }

                else if (statusType.ToString() != "None")
                {
                    pendingTasksList = ORMapList(new TaskRepository(dataAccessTransaction).GetAllExecutorTaskByStatusType(statusType));
                }
                else
                {
                    pendingTasksList = ORMapList(new TaskRepository(dataAccessTransaction).GetAllExecutorTaskReport());
                }

                foreach (PresentationTask tsk in pendingTasksList)
                {
                    Task pendingTask = Task.GetPendingTask(tsk.AccessRequestID.Value);
                    if (pendingTask != null)
                    {
                        DateTime? actionDate = pendingTask.ActionDate;
                        int responseDays = pendingTask.WorkFlowConfiguration.WorkflowConfigurationDetails.LastOrDefault().ResponseTimeInDays;
                        tsk.CompletionDate = actionDate.HasValue ? actionDate.Value.AddDays(responseDays) : pendingTask.LastModifiedDate.Value.AddDays(responseDays);
                    }
                    else
                    {
                        List<WorkflowConfigurationDetail> workflowDetail = WorkflowConfigurationDetail.GetByWorkflowConfigurationId(tsk.WorkFlowConfigurationId.Value);
                        foreach (WorkflowConfigurationDetail wrkflw in workflowDetail)
                        {
                            tsk.CompletionDate = tsk.CreatedDate.AddDays(wrkflw.ResponseTimeInDays);
                        }
                    }
                }
            }
            return pendingTasksList;
        }

        public static List<PresentationTask> GetByActionUserId(long actionUserId, DateTime? dtFrom, DateTime? dtTo, long endpointSystemId = 0)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new TaskRepository(dataAccessTransaction).GetByActionUserId(actionUserId, dtFrom, dtTo, endpointSystemId));
            }
        }

        public static List<PresentationTask> GetTaskDetailsByTaskStatus(long actionUserId, StatusType taskStatus, DateTime? dtFrom, DateTime? dtTo, long endpointSystemId = 0)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new TaskRepository(dataAccessTransaction).GetTaskDetailsByTaskStatus(actionUserId, taskStatus, dtFrom, dtTo, endpointSystemId));
            }
        }

        public static List<PresentationTask> GetGroupTaskDetailsByActionUserId(long actionUserId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new TaskRepository(dataAccessTransaction).GetGroupTaskDetailsByActionUserId(actionUserId));
            }
        }
        public static PresentationGridPagedData<PresentationTask> GetGroupTaskDetailsByActionUserId(long actionUserId,StatusType? type, int startRowIndex, int maximumRows, string filterExpression, string sortExpression, long requestFor = 0, DateTime? startDate=null, DateTime? endDate=null,long endpointId=0)
        {
            var response = new PresentationGridPagedData<PresentationTask>();
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                var dbResult = new TaskRepository(dataAccessTransaction).GetGroupTaskDetailsByActionUserId(actionUserId, type, startRowIndex, maximumRows, filterExpression, sortExpression, requestFor,startDate,endDate,endpointId);
                response.TotalCount = dbResult.TotalCount;
                response.Data = ORMapList(dbResult.Data);
                return response;
            }
        }
        public static List<PresentationTask> GetAllGroupTask(long actionUserId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new TaskRepository(dataAccessTransaction).GetAllGroupTask(actionUserId));
            }
        }

        public static List<PresentationTask> GetGroupTaskDetailsByTaskStatus(long actionUserId, StatusType taskStatus)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new TaskRepository(dataAccessTransaction).GetGroupTaskDetailsByTaskStatus(actionUserId, taskStatus));
            }
        }

        public static List<PresentationTask> GetSubmittedAttestationTaskDetailsByActionUserId(long actionUserId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new TaskRepository(dataAccessTransaction).GetSubmittedAttestationTaskDetailsByActionUserId(actionUserId));
            }
        }

        public static List<PresentationTask> GetCampaignTaskDetail(long AttestationCampaignId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new TaskRepository(dataAccessTransaction).GetCampaignTaskDetail(AttestationCampaignId));
            }
        }
        public static List<PresentationTask> GetTaskDetailsByActionUserIdAndTaskStatusForBulkAssign(long actionUserId, StatusType taskStatus, long groupId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new TaskRepository(dataAccessTransaction).GetTaskDetailsByActionUserIdAndTaskStatusForBulkAssign(actionUserId, taskStatus, groupId));
            }
        }
        public static PresentationGridPagedData<Presentation.PresentationTask> GetPagedResult(DateTime? dtFrom, DateTime? dtTo, int startIndex, int pageSize, string filterExpression, string sortExpression,  long requestFor = 0, long endpointSystemId = 0, StatusType statusType = Infrastructure.Enums.StatusType.None, long actionUserId = 0)
        {
            PresentationGridPagedData<Presentation.PresentationTask> gridPagedData = new PresentationGridPagedData<PresentationTask>();
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                var data = new TaskRepository(dataAccessTransaction).GetPagedResult(dtFrom, dtTo, startIndex, pageSize,filterExpression,sortExpression, requestFor, endpointSystemId, statusType, actionUserId);
                gridPagedData.Data = ORMapList(data.Data);
                gridPagedData.TotalCount = data.TotalCount;
                return gridPagedData;
            }
        }
        public static string TaskWithDraw(long taskId, string comments)
        {
            string result = string.Empty;
            try
            {
                if (taskId > 0)
                {
                    var tTask = Task.GetById(taskId);
                    if(tTask == null)
                    {
                        result = $"Given Task:{taskId} does not exists.";
                    }
                    else
                    {
                        AccessRequestStatus accessRequestStatus = AccessRequestStatus.None;
                        if (tTask.AccessRequestId.HasValue)
                        {
                            accessRequestStatus = AccessRequest.GetById(tTask.AccessRequestId.Value).RequestStatus;

                        }else if (tTask.BulkRequestId.HasValue)
                        {
                            accessRequestStatus = BulkRequest.GetById(tTask.BulkRequestId.Value).RequestStatus;
                        }else if (tTask.EmployeeEventTriggerId.HasValue)
                        {
                            accessRequestStatus = EmployeeEventTrigger.GetById(tTask.EmployeeEventTriggerId.Value).RequestStatus;
                        }
                        
                        if (accessRequestStatus == AccessRequestStatus.Withdrawn || accessRequestStatus == AccessRequestStatus.Rejected)
                        {
                            if (tTask.Action == Infrastructure.Enums.StatusType.Pending || tTask.Action == Infrastructure.Enums.StatusType.NotInitiated ||
                                tTask.Action == Infrastructure.Enums.StatusType.None)
                            {
                                using (var dataAccessTransaction = new DataAccessTransaction(false))
                                {
                                    tTask.ActionDate = DateTime.Now;
                                    tTask.IsCompleted = true;
                                    tTask.Comments = !string.IsNullOrWhiteSpace(comments) ? comments : "Request is withdrawn by the user.";
                                    tTask.IsActive = false;
                                    tTask.Action = accessRequestStatus == AccessRequestStatus.Withdrawn ? Infrastructure.Enums.StatusType.Withdrawn : Infrastructure.Enums.StatusType.Rejected;
                                    tTask.Save(MasterConfigHelper.ServiceUser);
                                    result = $"{Constants.taskStatusMessageWithdraw}: {tTask.Action}";
                                }
                            }
                            else
                            {
                                result = $"Task Status:{tTask.Action} cannot be withdrawn";
                            }
                        }
                        else
                        {
                            result = $"Task can be withdrawn/Rejected only for Rejected or Withdrawn request";
                        }
                    }
                }
                else
                {
                    result = $"Task does not exists TaskId:{taskId}";
                }
            }
            catch (Exception ex)
            {
                result = $"Error occured: {ex.Message}";
            }
            return result;
        }

        public static string ReinitiateRequest(RequestType requestType, long requestId)
        {
            var query = string.Empty;
            try
            {
                if (requestType == Infrastructure.Enums.RequestType.Provisioning)
                {
                    var accessRequest = AccessRequest.GetById(requestId);
                    if (accessRequest != null && accessRequest.ProgressStatus == AccessRequestProgress.WaitingforApproval)
                    {
                        query = query + $"DELETE FROM DI_M_TASK WHERE  ACCESSREQUESTID = {requestId}; Update DI_M_AccessRequest Set RequestStatus = 6, ProgressStatus = 9 where Id = {requestId}";
                    }
                    else
                    {
                        return "Only Accessrequest Status: WaitingforApproval can be reinitiated for approval.";
                    }
                }
                else if (requestType == Infrastructure.Enums.RequestType.BulkRequest)
                {
                    var bulkRequest = BulkRequest.GetById(requestId);
                    if (bulkRequest != null && bulkRequest.ProgressStatus == AccessRequestProgress.WaitingforApproval)
                        query = $"DELETE FROM DI_M_TASK WHERE  BULKREQUESTID = {requestId}; Update DI_M_BulkRequest Set RequestStatus = 6, ProgressStatus = 9 where Id = {requestId}";
                    else
                    {
                        return "Only Bulkrequest Status: WaitingforApproval can be reinitiated for approval.";
                    }
                }

                if (!string.IsNullOrWhiteSpace(query))
                {
                    if(Task.ExecuteQuery(query))
                        return "SUCCESS";
                }
            }
            catch (Exception ex)
            {
                query = $"Error occured while Reinitiate the request ";
            }
            return query;
        }
    }
}
