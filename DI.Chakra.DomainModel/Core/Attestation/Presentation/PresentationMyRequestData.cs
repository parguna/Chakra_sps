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
    public class PresentationMyRequestData
    {
        public long AdHocAccessRequestId { get; set; }

        public long AccessRequestID { get; set; }

        public string Name { get; set; }

        public string RequestType { get; set; }

        public DateTime RequestedDate { get; set; }

        public DateTime? ExecutionDate { get; set; }

        public AccessRequestStatus RequestStatus { get; set; }

        public AccessRequestProgress ProgressStatus { get; set; }

        public string Status { get; set; }

        public string Comments { get; set; }

        public string RequesterName { get; set; }

        public string RequesteeName { get; set; }

        public AdHocActionType AdhocActionType { get; set; }
        public AccessRequestSource AccessRequestSource { get; set; }
        public EndpointType EndpointType { get; set; }

        public long? EndpointSystemId { get; set; }

        public EndPoint EndPoint { get; set; }

        public AdHocAccessRequest AdHocAccessRequests { get; set; }

        public AccountType AccountType { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public long UserID { get; set; }

        public string ErrorMessage { get; set; }
        public string RequesteeUserName { get; set; }
        public string RequesterUserName { get; set; }
        public string RequesteeEmployeeId { get; set; }
        public string RequesteeDepartment { get; set; }
        public bool IsAdhocRequest { get; set; }
        public bool IsBulkRequest { get; set; }
        public bool IsUndoEventRequest { get; set; }
        public string Entitlement { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public string RequestSource { get; set; }
        public string ServiceAccountComplianceString { get; set; }
        public string Remarks { get; set; }
        public string ConflictRemarks { get; set; }
        //public string Entitlement
        //{
        //    get
        //    {
        //        string Entitle = DeserializeEntitlement(this.AdHocAccessRequestId);

        //        if (!string.IsNullOrEmpty(Entitle))
        //            return Entitle.Replace(",", ", ").Replace("  ", " ").Trim();
        //        return "";
        //    }
        //}
        public long? BulkRequestId { get; set; }
        public string AccessRequestComments { get; set; }
        public string ExecuteDateString()
        {
            return (this.ExecutionDate.HasValue) ? ExecutionDate.Value.ToString("dd-MM-yyyy") : "";
        }
        //Switch case revamped
        //public string DeserializeEntitlement(long? AdhocRequestId)
        //{
        //    if (AdhocRequestId.HasValue)
        //    {
        //        using (DataAccessTransaction dataAccessTransaction=new DataAccessTransaction ())
        //        {
        //            AdHocAccessRequest adhoxReq=AdHocAccessRequest.GetById((long)AdhocRequestId);
        //            return EndPointFactory.GetEndPoint(adhoxReq.EndpointSystemType).GetEntitlementDetail(adhoxReq.EntitlementDetail).Entitlement;
        //        }
        //    }
        //    return "";
        //}

        //private string JoinedEntitlementAdHocVPNRequestDetail(List<AdHocVPNRequestDetail> AdhocReqDetails)
        //{
        //    string strItems = "";
        //    foreach (var items in AdhocReqDetails)
        //    {
        //        strItems += (strItems == "") ? items.ApplicationName : "," + items.ApplicationName;
        //    }
        //    return strItems;
        //}


        public DateTime? EndDate { get; set; }

        public DateTime? StartDate { get; set; }

        public static PresentationMyRequestData ORMap(EntityModel.Presentation.PresentationMyRequestData presentationMyRequestGridData)
        {
            if (presentationMyRequestGridData == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationMyRequestData, PresentationMyRequestData>(presentationMyRequestGridData);
        }

        public static List<PresentationMyRequestData> ORMapList(IList<EntityModel.Presentation.PresentationMyRequestData> presentationMyRequestGridData)
        {
            return presentationMyRequestGridData.Select(ORMap).ToList();
        }

        public static PresentationGridPagedData<PresentationMyRequestData> GetByActionUserId(long actionUserId, int startRowIndex, int maximumRows, string filterExpression, string sortExpression)
        {
            var response = new PresentationGridPagedData<PresentationMyRequestData>();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var dbResult = new AttestationRepository(dataAccessTransaction).GetByActionUserId(actionUserId, startRowIndex, maximumRows, filterExpression, sortExpression);
                response.TotalCount = dbResult.TotalCount;
                response.Data = ORMapList(dbResult.Data);
                return response;
            }
        }

        public static IList<PresentationMyRequestData> GetOnlyEventTriggedAccessRequests()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new AttestationRepository(dataAccessTransaction).GetOnlyEventTriggedAccessRequests());
            }
        }

        public static PagedResults<PresentationMyRequestData> GetByActionUserIdPaging(long actionUserId, int pageSize, int pageNumber)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapListPaging(new AttestationRepository(dataAccessTransaction).GetByActionUserIdPaging(actionUserId, pageSize, pageNumber));
            }
        }        

        public static IList<PresentationMyRequestData> GetByActionUserDepartment(long actionUserId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new AttestationRepository(dataAccessTransaction).GetByActionUserDepartment(actionUserId));
            }
        }

        public static PagedResults<PresentationMyRequestData> GetByActionUserDepartmentPaging(long actionUserId, int pageSize, int pageNumber)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapListPaging(new AttestationRepository(dataAccessTransaction).GetByActionUserDepartmentPaging(actionUserId, pageSize, pageNumber));
            }
        }

        public static IList<PresentationMyRequestData> GetByRequesterId(List<long> requesterIDs)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new AttestationRepository(dataAccessTransaction).GetByRequesterId(requesterIDs));
            }
        }

        public static PresentationMyRequestData GetByAdhocId(long requestId, bool deepLoad)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new AttestationRepository(dataAccessTransaction).GetByRequestId(requestId, deepLoad));
            }
        }

        public static IList<PresentationMyRequestData> GetByProgressStatus(DI.Chakra.Infrastructure.Enums.AccessRequestProgress progressStatus)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                IList<PresentationMyRequestData> requestList = ORMapList(new AttestationRepository(dataAccessTransaction).GetByProgressStatus(progressStatus));
                foreach (PresentationMyRequestData request in requestList)
                {
                    request.ErrorMessage = ServiceLog.GetByAccessRequestId(request.AccessRequestID);
                }
                return requestList;
            }
        }
        public static PagedResults<PresentationMyRequestData> ORMapListPaging(PagedResults<EntityModel.Presentation.PresentationMyRequestData> presentationMyRequestGridData)
        {
            var results = presentationMyRequestGridData.Results;
            var mappedResults = ORMapList(results);

            return new PagedResults<PresentationMyRequestData>()
            {
                PageNumber = presentationMyRequestGridData.PageNumber,
                PageSize = presentationMyRequestGridData.PageSize,
                TotalNumberOfPages = presentationMyRequestGridData.TotalNumberOfPages,
                TotalNumberOfRecords = presentationMyRequestGridData.TotalNumberOfRecords,
                Results = mappedResults
            };
        }
    }
}
