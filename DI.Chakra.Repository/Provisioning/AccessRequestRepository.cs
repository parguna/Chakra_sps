using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace DI.Chakra.Repository
{
    public class AccessRequestRepository : RepositoryBase<AccessRequest>
    {


        public AccessRequestRepository(DataAccessTransaction transaction) : base(transaction) { }


        public string CheckForDuplicateAccessRequest(ApplicationType applicationType, long endpointSystemId, AdHocActionType actiontype, AccountType accountType, long userId)
        {

            var tt = (from acsrqst in CurrentContext.AccessRequest
                      join rqst in CurrentContext.AdHocAccessRequests
                      on acsrqst.AdHocAccessRequestId equals rqst.Id
                      where acsrqst.EndpointSystemId.HasValue &&
                            acsrqst.EndpointSystemId.Value == endpointSystemId &&
                            rqst.RequesteeUserId == userId &&
                            rqst.AccountType == accountType &&
                            rqst.AdhocActionType == actiontype &&
                            (acsrqst.RequestStatus != AccessRequestStatus.Rejected || acsrqst.RequestStatus != AccessRequestStatus.Withdrawn || acsrqst.RequestStatus != AccessRequestStatus.None) &&
                            acsrqst.ProgressStatus != AccessRequestProgress.Withdrawn
                      select "Access Request Id:" + acsrqst.Id + " (AdHoc Access Request Id :" + rqst.Id + ") ").ToList();


            if (accountType == AccountType.User)
            {
                var tt1 = (from acsrqst in CurrentContext.AccessRequest
                           join blk in CurrentContext.BulkRequests
                           on acsrqst.AdHocAccessRequestId equals blk.Id
                           join blkDetails in CurrentContext.BulkRequestDetails
                           on blk.Id equals blkDetails.BulkRequestId
                           where acsrqst.EndpointSystemId.HasValue &&
                                 acsrqst.EndpointSystemId.Value == endpointSystemId &&
                                 blkDetails.UserId == userId &&
                                 blk.AdhocActionType == actiontype &&
                                 (acsrqst.RequestStatus != AccessRequestStatus.Rejected || acsrqst.RequestStatus != AccessRequestStatus.Withdrawn || acsrqst.RequestStatus != AccessRequestStatus.None) &&
                            acsrqst.ProgressStatus != AccessRequestProgress.Withdrawn
                           select "Access Request Id:" + acsrqst.Id + " (Bulk Access Request Id :" + blk.Id + ") ").ToList();

                tt = tt.Union(tt1).ToList();
            }

            if (tt.Count > 0)
                return string.Join(",", tt);
            return string.Empty;
        }

        public IList<MyRequestsGridData> GetAdhocRequestsByProgressStatus(AccessRequestProgress progressStatus)
        {
            try
            {

                IList<MyRequestsGridData> requestList = new List<MyRequestsGridData>();
                DateTime CurrentDate = DateTime.Now;
                if(progressStatus == AccessRequestProgress.WorkflowNotFound)
                {
                    requestList = (from rqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                                   join acssrqst in CurrentContext.AccessRequest.Include("User").Include("AdHocAccessRequest")
                                   on rqst.Id equals acssrqst.AdHocAccessRequestId
                                   join rusr in CurrentContext.User
                                   on rqst.RequesteeUserId equals rusr.Id into allusr
                                   from alusr in allusr.DefaultIfEmpty()
                                   join rqust in CurrentContext.User
                                   on rqst.RequesterUserId equals rqust.Id into allrqst
                                   from alrqst in allrqst.DefaultIfEmpty()
                                   where acssrqst.ProgressStatus == progressStatus
                                   select new MyRequestsGridData()
                                   {
                                       AdHocAccessRequestId = rqst.Id,
                                       RequestSource = (acssrqst.BulkRequestId == null && acssrqst.AttestationCampaignId == null) ? "Access Request" : acssrqst.AttestationCampaignId == null ? "Bulk Request" : "Attestation Remediation",
                                       Name = (rqst.EndPoint == null ? "" : rqst.EndPoint.DisplayName),
                                       RequestedDate = rqst.CreatedDate,
                                       ExecutionDate = acssrqst.ExecutionTime,
                                       RequestType = ((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? ((int)rqst.AdhocActionType == 2 ? "Amendment-Add" : "Amendment-Remove") : ((int)rqst.AdhocActionType == 1 ? "New Account" : "Delete Account"),
                                       Comments = rqst.Comments,
                                       RequestStatus = acssrqst.RequestStatus,
                                       AccessRequestID = acssrqst.Id,
                                       RequesteeName = alusr.DisplayName,
                                       RequesterName = alrqst.DisplayName,
                                       AccountType = rqst.AccountType,
                                       ValidFrom = rqst.ValidFrom,
                                       ValidTo = rqst.ValidTo,
                                       UserID = acssrqst.UserId,
                                       EntitlementDetailType = rqst.ApplicationType,
                                       AdditionalInfo = acssrqst.AdditionalInfo
                                   }).ToList();
                }
                else
                {
                    requestList = (from rqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                                   join acssrqst in CurrentContext.AccessRequest.Include("User").Include("AdHocAccessRequest")
                                   on rqst.Id equals acssrqst.AdHocAccessRequestId
                                   join rusr in CurrentContext.User
                                   on rqst.RequesteeUserId equals rusr.Id into allusr
                                   from alusr in allusr.DefaultIfEmpty()
                                   join rqust in CurrentContext.User
                                   on rqst.RequesterUserId equals rqust.Id into allrqst
                                   from alrqst in allrqst.DefaultIfEmpty()
                                   where acssrqst.ProgressStatus == progressStatus && (acssrqst.RequestSource == AccessRequestSource.AdhocRequest || acssrqst.RequestSource == AccessRequestSource.AttestationRemediation)
                                   select new MyRequestsGridData()
                                   {
                                       AdHocAccessRequestId = rqst.Id,
                                       RequestSource = (acssrqst.BulkRequestId == null && acssrqst.AttestationCampaignId == null) ? "Access Request" : acssrqst.AttestationCampaignId == null ? "Bulk Request" : "Attestation Remediation",
                                       Name = (rqst.EndPoint == null ? "" : rqst.EndPoint.DisplayName),
                                       RequestedDate = rqst.CreatedDate,
                                       ExecutionDate = acssrqst.ExecutionTime,
                                       RequestType = ((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? ((int)rqst.AdhocActionType == 2 ? "Amendment-Add" : "Amendment-Remove") : ((int)rqst.AdhocActionType == 1 ? "New Account" : "Delete Account"),
                                       Comments = rqst.Comments,
                                       RequestStatus = acssrqst.RequestStatus,
                                       AccessRequestID = acssrqst.Id,
                                       RequesteeName = alusr.DisplayName,
                                       RequesterName = alrqst.DisplayName,
                                       AccountType = rqst.AccountType,
                                       ValidFrom = rqst.ValidFrom,
                                       ValidTo = rqst.ValidTo,
                                       UserID = acssrqst.UserId,
                                       EntitlementDetailType = rqst.ApplicationType,
                                       AdditionalInfo = acssrqst.AdditionalInfo
                                   }).ToList();
                }
                
                if (progressStatus == AccessRequestProgress.Executing)
                {
                    var blkRequestList = (from rqst in CurrentContext.BulkRequests.Include("EndPoint")
                                   join blkDetail in CurrentContext.BulkRequestDetails on rqst.Id equals blkDetail.BulkRequestId
                                   join rusr in CurrentContext.User
                                   on blkDetail.UserId equals rusr.Id into allusr
                                   from alusr in allusr.DefaultIfEmpty()
                                   join rqust in CurrentContext.User
                                   on rqst.RequestedBy equals rqust.Id into allrqst
                                   from alrqst in allrqst.DefaultIfEmpty()
                                   where rqst.ProgressStatus == progressStatus
                                   select new MyRequestsGridData()
                                   {
                                       AdHocAccessRequestId = rqst.Id,
                                       RequestSource = "Bulk Request",
                                       Name = (rqst.EndPoint == null ? "" : rqst.EndPoint.DisplayName),
                                       RequestedDate = rqst.CreatedDate,
                                       ExecutionDate = rqst.ExecutionTime,
                                       RequestType = ((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? ((int)rqst.AdhocActionType == 2 ? "Amendment-Add" : "Amendment-Remove") : ((int)rqst.AdhocActionType == 1 ? "New Account" : "Delete Account"),
                                       Comments = rqst.Comments,
                                       RequestStatus = rqst.RequestStatus,
                                       AccessRequestID = rqst.Id,
                                       RequesteeName = alusr.DisplayName,
                                       RequesterName = alrqst.DisplayName,
                                       AccountType = AccountType.User,
                                       ValidFrom = rqst.ActivationDate,
                                       ValidTo = rqst.DeactivationDate,
                                       UserID = blkDetail.UserId,
                                       EntitlementDetailType = rqst.ApplicationType,
                                       AdditionalInfo = rqst.AdditionalInfo
                                   }).ToList();

                    requestList = requestList.Union(blkRequestList).ToList();

                    requestList = requestList.Where(item => item.ExecutionDate.HasValue && (CurrentDate - item.ExecutionDate.Value).TotalMinutes > 30).ToList();
                }

                return requestList.OrderByDescending(i=> i.RequestedDate).ToList();

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<FailedRequestGridData> GetFailedSystemRequests(AccessRequestProgress progressStatus)
        {
            try
            {

                return (from request in CurrentContext.AccessRequest.Include("User")
                        join ep in CurrentContext.EndPoint
                            on request.EndpointSystemId equals ep.Id into detail
                        from ep in detail.DefaultIfEmpty()
                        where request.ProgressStatus == progressStatus && request.RequestSource != AccessRequestSource.AttestationRemediation
                        && request.RequestSource != AccessRequestSource.AdhocRequest && request.BulkRequestId == null && request.AttestationCampaignId == null
                        && request.AdHocAccessRequestId == null
                        select new FailedRequestGridData()
                        {
                            UserId = request.UserId,
                            RequestSourceType = "Access Request",
                            DisplayName = request.User.DisplayName,
                            Company = request.User.CompanyCode + " - " + request.User.CompanyName,
                            CostCenter = request.User.CostCenter,
                            Department = request.User.DepartmentCode + " - " + request.User.DepartmentName,
                            Division = request.User.DivisionCode + " - " + request.User.DivisionName,
                            Section = request.User.SectionCode + " - " + request.User.SectionName,
                            AccessRequestId = request.Id,
                            SystemName = ep.DisplayName,
                            Remarks = request.Remarks,
                            EmployeeID = request.User.EmployeeId,
                            RequestDate = request.ExecutionTime.Value,
                            RequestSource = request.RequestSource,
                            AdditionalInfo = request.AdditionalInfo
                        }).OrderByDescending(i=> i.RequestDate).ToList();

            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<PresentationApprovers> GetByAccessRequestId(long accessRequestId)
        {
            return (from adhocrqst in CurrentContext.AdHocAccessRequests
                    join acssrqst in CurrentContext.AccessRequest
                    on adhocrqst.Id equals acssrqst.AdHocAccessRequestId
                    join tsk in CurrentContext.Tasks
                    on acssrqst.Id equals tsk.AccessRequestId
                    join usr in CurrentContext.User
                    on tsk.ActionUserId equals usr.Id
                    where adhocrqst.Id == accessRequestId && (tsk.IsCompleted || tsk.Action == StatusType.Rejected)
                    select new PresentationApprovers
                    {
                        ApproverName = usr.DisplayName,
                        ApproverType = tsk.Approver,
                        WorkflowState = tsk.WorkflowStateId,
                        RequestStatus = acssrqst.RequestStatus,
                        Action = tsk.Action,
                        ActionDate = tsk.LastModifiedDate != null ? tsk.LastModifiedDate.Value : tsk.CreatedDate,
                        Comments = tsk.Comments,
                    }).ToList();
        }



        public bool IsBulkRequestHasPendingChildRequests(int bulkRequestId)
        {
            return CurrentContext.AccessRequest.Any(item => item.BulkRequestId == bulkRequestId &&
                                                           (item.ProgressStatus == AccessRequestProgress.WaitingforApproval
                                                                   || item.ProgressStatus == AccessRequestProgress.Submitted));
        }
        public List<PresentationAccessRequest> GetByProgressStatus(DI.Chakra.Infrastructure.Enums.AccessRequestProgress status, bool isAdminMode)
        {
            var returnlist = (
                    from acssrqst in CurrentContext.AccessRequest
                    join endpoint in CurrentContext.EndPoint
                    on acssrqst.EndpointSystemId equals endpoint.Id
                    join usr in CurrentContext.User.Include("NOrganisation")
                    on acssrqst.UserId equals usr.Id
                    where (status == DI.Chakra.Infrastructure.Enums.AccessRequestProgress.None ? 1 == 1 : acssrqst.ProgressStatus == status)
                    //&& (!isAdminMode ? (acssrqst.AdHocAccessRequestId) != null :
                    // (acssrqst.RequestSource != AccessRequestSource.AttestationRemediation || acssrqst.RequestSource == AccessRequestSource.None))
                    orderby acssrqst.Id descending
                    select new PresentationAccessRequest
                    {
                        DisplayName = usr.DisplayName,
                        ADID = usr.UserName,
                        Id = acssrqst.Id,
                        AdHocAccessRequestId = acssrqst.AdHocAccessRequestId,
                        EndpointsystemName = endpoint.EndPointSystemType.EndpointSystemTypeName.ToString(),
                        ProgressStatus = acssrqst.ProgressStatus,
                        RequestSource = acssrqst.RequestSource.ToString(),
                        ExecutionTime = acssrqst.ExecutionTime,
                        AccountType = acssrqst.AccountType,
                        EndpointName = endpoint.DisplayName,
                        RequestDate = acssrqst.CreatedDate
                    }).ToList();

            returnlist.RemoveAll(item => (item.ProgressStatus == AccessRequestProgress.WaitingForSync || item.ProgressStatus == AccessRequestProgress.Executing) && (DateTime.Now - Convert.ToDateTime(item.ExecutionTime)).TotalMinutes < 30);
            return returnlist;
        }

        public List<PresentationAccessRequest> GetAllByProgressStatus(DI.Chakra.Infrastructure.Enums.AccessRequestProgress progressStatus, long requestedFor, DateTime? dtFrom, DateTime? dtTo, long endpointSystemId)
        {
            var currentRequests = from req in CurrentContext.AccessRequest
                                  where ((progressStatus == AccessRequestProgress.None ? true : req.ProgressStatus == progressStatus) && ((endpointSystemId > 0) ? req.EndpointSystemId == endpointSystemId : true))
                                  select req;

            var requestee = from reqFor in CurrentContext.User
                            where (requestedFor == 0 ? true : reqFor.Id == requestedFor)
                            select reqFor;

            var bulkrequest = from blkReq in CurrentContext.BulkRequests
                              join blkDetail in CurrentContext.BulkRequestDetails on blkReq.Id equals blkDetail.BulkRequestId
                              where ((requestedFor == 0 ? true : blkDetail.UserId == requestedFor) &&
                                    (endpointSystemId > 0 ? blkReq.EndpointSystemId == endpointSystemId : true) &&
                                    (progressStatus == AccessRequestProgress.None ? true : blkReq.ProgressStatus == progressStatus))
                              select blkReq;

            var provrequests = (from rqst in CurrentContext.AdHocAccessRequests.Include("EndPoint") //.Include("MiscSystem")
                                join acssrqst in currentRequests
                                on rqst.Id equals acssrqst.AdHocAccessRequestId
                                join usr in CurrentContext.User
                                on acssrqst.RequesterId equals usr.Id
                                join rusr in requestee
                                on rqst.RequesteeUserId equals rusr.Id
                                where ((acssrqst.RequestSource == AccessRequestSource.AdhocRequest || acssrqst.RequestSource == AccessRequestSource.NonEmployeeExtension) && ((!dtFrom.HasValue || acssrqst.CreatedDate >= dtFrom.Value) && (!dtTo.HasValue || acssrqst.CreatedDate <= dtTo.Value)))
                                select new PresentationAccessRequest()
                                {
                                    AdHocAccessRequestId = rqst.Id,
                                    Name = rqst.EndPoint != null ? rqst.EndPoint.DisplayName : "",
                                    RequestSource = acssrqst.AttestationCampaignId != null ? Constants.AttestationRemediation : Constants.AccessRequest,
                                    RequestedDate = acssrqst.CreatedDate,
                                    ExecutionDate = acssrqst.ExecutionTime,
                                    RequestType = ((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? ((int)rqst.AdhocActionType == 2 ? "Amendment-Add" : "Amendment-Remove") : ((int)rqst.AdhocActionType == 1 ? "New Account" : "Delete Account"),
                                    Comments = rqst.Comments,
                                    RequestStatus = acssrqst.RequestStatus,
                                    ProgressStatus = acssrqst.ProgressStatus,
                                    AccessRequestID = acssrqst.Id,
                                    RequesteeUserName = rusr.DisplayName,
                                    RequesterUserName = acssrqst.RequestSource == AccessRequestSource.AdhocRequest ? usr.DisplayName : "System",
                                    AccountType = rqst.AccountType,
                                    ValidFrom = rqst.ValidFrom,
                                    ValidTo = rqst.ValidTo,
                                    IsAdhocRequest = true,
                                    IsBulkRequest = false,
                                    IsUndoEventRequest = false,
                                    ApplicationType = rqst.ApplicationType,
                                    Entitlement = rqst.EntitlementDetail,
                                    TriggerId = null,
                                });
            var eventRequests = (from rqst in CurrentContext.EmployeeEventTriggers
                                 join acssrqst in currentRequests on rqst.Id equals acssrqst.EmployeeEventTriggerId
                                 join ep in CurrentContext.EndPoint on acssrqst.EndpointSystemId equals ep.Id
                                join usr in requestee
                                on rqst.UserId equals usr.Id
                                where rqst.IsRequestCreated && ((acssrqst.RequestSource == AccessRequestSource.OnBoarding || acssrqst.RequestSource == AccessRequestSource.OffBoarding ||
                                acssrqst.RequestSource == AccessRequestSource.OUChange ||
                                acssrqst.RequestSource == AccessRequestSource.Rehire || acssrqst.RequestSource == AccessRequestSource.ReOrganisation
                                || acssrqst.RequestSource == AccessRequestSource.UndoEvent
                                ) && ((!dtFrom.HasValue || acssrqst.CreatedDate >= dtFrom.Value) && (!dtTo.HasValue || acssrqst.CreatedDate <= dtTo.Value)))
                                select new PresentationAccessRequest()
                                {
                                    AdHocAccessRequestId = null,
                                    Name = ep != null ? ep.DisplayName : "",
                                    RequestSource = acssrqst.RequestSource.ToString(),
                                    RequestedDate = acssrqst.CreatedDate,
                                    ExecutionDate = acssrqst.ExecutionTime,
                                    RequestType = acssrqst.RequestSource == AccessRequestSource.OnBoarding ? Constants.OnBoarding :
                                    acssrqst.RequestSource == AccessRequestSource.OffBoarding ? Constants.OffBoarding :
                                    acssrqst.RequestSource == AccessRequestSource.ReOrganisation ? Constants.ReOrganisation :
                                    acssrqst.RequestSource == AccessRequestSource.Propagation ? Constants.Propagation :
                                    acssrqst.RequestSource == AccessRequestSource.OUChange ? Constants.OUChange :
                                    acssrqst.RequestSource == AccessRequestSource.Rehire ? Constants.Rehire:
                                    acssrqst.RequestSource == AccessRequestSource.UndoEvent ? Constants.UndoEvent:"",
                                    Comments = acssrqst.Comments,
                                    RequestStatus = acssrqst.RequestStatus,
                                    ProgressStatus = acssrqst.ProgressStatus,
                                    AccessRequestID = acssrqst.Id,
                                    RequesteeUserName = usr.DisplayName,
                                    RequesterUserName = "System",
                                    AccountType = AccountType.User,
                                    ValidFrom = acssrqst.ActivationDate,
                                    ValidTo = acssrqst.DeactivationDate,
                                    IsAdhocRequest = true,
                                    IsBulkRequest = false,
                                    IsUndoEventRequest = false,
                                    ApplicationType = acssrqst.ApplicationType,
                                    Entitlement = null,
                                    TriggerId = rqst.Id
                                });
            var bulkRequest = (from rqst in bulkrequest
                               join usr in CurrentContext.User
                               on rqst.RequestedBy equals usr.Id
                               where ((!dtFrom.HasValue || rqst.CreatedDate >= dtFrom.Value) && (!dtTo.HasValue || rqst.CreatedDate <= dtTo.Value))
                               select new PresentationAccessRequest()
                               {
                                   AdHocAccessRequestId = rqst.Id,
                                   Name = rqst.EndPoint != null ? rqst.EndPoint.DisplayName : "",
                                   RequestSource = Constants.BulkRequest,
                                   RequestedDate = rqst.CreatedDate,
                                   ExecutionDate = rqst.ExecutionTime,
                                   RequestType = ((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? ((int)rqst.AdhocActionType == 2 ? "Amendment-Add" : "Amendment-Remove") : ((int)rqst.AdhocActionType == 1 ? "New Account" : "Delete Account"),
                                   Comments = rqst.Comments,
                                   RequestStatus = rqst.RequestStatus,
                                   ProgressStatus = rqst.ProgressStatus,
                                   AccessRequestID = rqst.Id,
                                   RequesteeUserName = string.Empty,
                                   RequesterUserName = rqst.RequestSource == AccessRequestSource.AdhocRequest ? usr.DisplayName : "System",
                                   AccountType = AccountType.User,
                                   ValidFrom = rqst.ActivationDate,
                                   ValidTo = rqst.DeactivationDate,
                                   IsAdhocRequest = false,
                                   IsBulkRequest = true,
                                   IsUndoEventRequest = false,
                                   ApplicationType = rqst.ApplicationType,
                                   Entitlement = rqst.EntitlementDetail,
                                   TriggerId = null,
                               });

            var UndoEventRequest = (from acssrqst in currentRequests
                                    join endpoint in CurrentContext.EndPoint on acssrqst.EndpointSystemId equals endpoint.Id
                                    join rqst in CurrentContext.AdHocAccessRequests on acssrqst.AdHocAccessRequestId equals rqst.Id into adhocReq
                                    from accReq in adhocReq.DefaultIfEmpty()
                                    join usr in requestee
                                    on acssrqst.UserId equals usr.Id
                                    join rusr in CurrentContext.User
                                    on acssrqst.RequesterId equals rusr.Id
                                    where
                                    ((acssrqst.RequestSource == AccessRequestSource.UndoEvent)
                                      && (!dtFrom.HasValue || acssrqst.CreatedDate >= dtFrom.Value)
                                      && (!dtTo.HasValue || acssrqst.CreatedDate <= dtTo.Value))
                                    select new PresentationAccessRequest()
                                    {
                                        AdHocAccessRequestId = accReq.Id,
                                        Name = endpoint.DisplayName,
                                        RequestSource = acssrqst.RequestSource.ToString(),
                                        RequestedDate = acssrqst.CreatedDate,
                                        ExecutionDate = acssrqst.ExecutionTime,
                                        RequestType = accReq != null ? (((int)accReq.AdhocActionType == 2 || (int)accReq.AdhocActionType == 3) ? ((int)accReq.AdhocActionType == 2 ? "Amendment-Add" : "Amendment-Remove") : ((int)accReq.AdhocActionType == 1 ? "New Account" : "Delete Account")):"None",
                                        Comments = accReq.Comments,
                                        RequestStatus = acssrqst.RequestStatus,
                                        ProgressStatus = acssrqst.ProgressStatus,
                                        AccessRequestID = acssrqst.Id,
                                        RequesteeUserName = usr.DisplayName,
                                        RequesterUserName = acssrqst.RequestSource == AccessRequestSource.AdhocRequest ? rusr.DisplayName : "System",
                                        AccountType = accReq != null ? accReq.AccountType : AccountType.User, 
                                        ValidFrom = accReq.ValidFrom,
                                        ValidTo = accReq.ValidTo,
                                        IsAdhocRequest = false,
                                        IsBulkRequest = false,
                                        IsUndoEventRequest = true,
                                        ApplicationType = endpoint.ApplicationType,
                                        Entitlement = accReq != null ? accReq.EntitlementDetail : "",
                                        TriggerId = null
                                    });

            List<AccessRequestStatus> reqStatus = new List<AccessRequestStatus>();

            if (progressStatus == AccessRequestProgress.WaitingforApproval)
            {
                reqStatus.Add(AccessRequestStatus.Requested);
            }
            if (progressStatus == AccessRequestProgress.RequestRejected)
            {
                reqStatus.Add(AccessRequestStatus.Rejected);
            }
            if (progressStatus == AccessRequestProgress.Withdrawn)
            {
                reqStatus.Add(AccessRequestStatus.Withdrawn);
            }
            if (progressStatus == AccessRequestProgress.Submitted)
            {
                reqStatus.Add(AccessRequestStatus.Submitted);
            }
            if (progressStatus == AccessRequestProgress.Completed)
            {
                reqStatus.Add(AccessRequestStatus.Granted);
                reqStatus.Add(AccessRequestStatus.Approved);
            }
            if(progressStatus == AccessRequestProgress.WorkflowNotFound)
            {
                reqStatus.Add(AccessRequestStatus.Submitted);
            }
            var employeeEvents = (from trigger in CurrentContext.EmployeeEventTriggers
                                  join rusr in CurrentContext.User
                                  on trigger.UserId equals rusr.Id
                                  join rqust in CurrentContext.User
                                  on trigger.CreatedBy equals rqust.UserName
                                  where (requestedFor == 0 ? true : (trigger.UserId == requestedFor)) && ((trigger.EventType == EmployeeEvents.NonEmployeeExtension || trigger.EventType == EmployeeEvents.NonEmployeeCreation)
                                        && (!dtFrom.HasValue || trigger.CreatedDate >= dtFrom.Value) && (!dtTo.HasValue || trigger.CreatedDate <= dtTo.Value) && (progressStatus == AccessRequestProgress.None || reqStatus.Contains(trigger.RequestStatus)))
                                  select new PresentationAccessRequest()
                                  {
                                      AdHocAccessRequestId = trigger.Id,
                                      Name = "Non Employee",
                                      RequestSource = Constants.NonEmployee,
                                      RequestedDate = trigger.CreatedDate,
                                      ExecutionDate = null,
                                      RequestType = trigger.EventType == EmployeeEvents.NonEmployeeCreation ? "Non Employee Creation" : (trigger.EventType == EmployeeEvents.NonEmployeeExtension ? "Non Employee Extension / Shortening" : string.Empty),
                                      Comments = trigger.Comments,
                                      RequestStatus = trigger.RequestStatus,
                                      ProgressStatus = progressStatus,
                                      AccessRequestID = trigger.Id,
                                      RequesteeUserName = rusr.DisplayName,
                                      RequesterUserName = rqust.DisplayName,
                                      AccountType = AccountType.None,
                                      ValidFrom = null,
                                      ValidTo = null,
                                      IsAdhocRequest = false,
                                      IsBulkRequest = false,
                                      IsUndoEventRequest = false,
                                      ApplicationType = ApplicationType.None,
                                      Entitlement = string.Empty,
                                      TriggerId = trigger.Id,
                                  });

            var requests = provrequests.Union(eventRequests).Union(bulkRequest).Union(UndoEventRequest).Distinct().OrderByDescending(x => x.RequestedDate).ToList();

            if (endpointSystemId == 0)
            {
                requests = requests.Union(employeeEvents).Distinct().OrderByDescending(x => x.RequestedDate).ToList();
            }

            var bulk = requests.Where(i => i.IsBulkRequest).Select(i => i.AdHocAccessRequestId);

            var t = CurrentContext.BulkRequestDetails.Select(i => new { i.BulkRequestId, i.Name }).Where(x => bulk.Contains(x.BulkRequestId)).GroupBy(i => i.BulkRequestId).ToList().ToDictionary(g => g.Key, g => string.Join(",", g.Select(i => i.Name)));

            foreach (var item in requests)
            {
                if (item.IsBulkRequest)
                {
                    item.RequesteeUserName = t.ContainsKey(item.AdHocAccessRequestId.Value) ? t[item.AdHocAccessRequestId.Value] : string.Empty;
                }
            }

            return requests;
        }

        public List<PresentationAccessRequest> GetAccessRequestForReportExtraction(DateTime? dtFrom, DateTime? dtTo)
        {
            var currentRequests = from req in CurrentContext.AccessRequest
                                  where (req.ProgressStatus == AccessRequestProgress.Completed || req.ProgressStatus == AccessRequestProgress.ManuallyResolved)
                                  && req.RequestSource == AccessRequestSource.AdhocRequest
                                  select req;

            return (from rqst in CurrentContext.AdHocAccessRequests.Include("EndPoint") //.Include("MiscSystem")
                    join acssrqst in currentRequests
                    on rqst.Id equals acssrqst.AdHocAccessRequestId
                    join usr in CurrentContext.User
                    on acssrqst.UserId equals usr.Id
                    where
                    (rqst.AdhocActionType == AdHocActionType.NewAccount || rqst.AdhocActionType == AdHocActionType.DeleteAccount) &&
                    ((acssrqst.RequestSource == AccessRequestSource.AdhocRequest) && acssrqst.ExecutionTime != null && ((!dtFrom.HasValue || acssrqst.ExecutionTime >= dtFrom.Value) && (!dtTo.HasValue || acssrqst.ExecutionTime <= dtTo.Value)))
                    select new PresentationAccessRequest()
                    {
                        Id = acssrqst.Id,
                        EndpointName = rqst.EndPoint != null ? rqst.EndPoint.DisplayName : "",
                        RequestType = ((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? ((int)rqst.AdhocActionType == 2 ? "Amendment-Add" : "Amendment-Remove") : ((int)rqst.AdhocActionType == 1 ? "New Account" : "Delete Account"),
                        RequesteeUserName = usr.DisplayName,
                        RequesterUserName = usr.UserName,
                        ExecutionDate = acssrqst.ExecutionTime,
                        AccessRequestID = acssrqst.Id,
                        EndpointSystemId = acssrqst.EndpointSystemId,
                        AccountType = rqst.AccountType,
                        ApplicationType = rqst.ApplicationType,
                        Entitlement = rqst.EntitlementDetail,
                        AdHocAccessRequests = rqst,
                        RequestSource = acssrqst.RequestSource.ToString()
                    }).ToList();
        }
    }
}
