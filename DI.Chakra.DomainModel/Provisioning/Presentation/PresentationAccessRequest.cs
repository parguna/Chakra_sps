using AutoMapper;
using DI.Chakra.DomainModel.Core;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationAccessRequest
    {
        public long Id { get; set; }
        public long? AdHocAccessRequestId { get; set; }
        public long? TriggerId { get; set; }
        public string DisplayName { get; set; }
        public string ADID { get; set; }
        public string RequestSource { get; set; }
        public AccessRequestProgress ProgressStatus { get; set; }
        public string EndpointsystemName { get; set; }
        public DateTime? ExecutionTime { get; set; }

        public DateTime? RequestDate { get; set; }
        public string EndpointName { get; set; }

        public long AccessRequestID { get; set; }
        public string Name { get; set; }
        public string RequestType { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? ExecutionDate { get; set; }
        public AccessRequestStatus RequestStatus { get; set; }
        public string Status { get; set; }
        public string Comments { get; set; }

        public AdHocActionType AdhocActionType { get; set; }
        public long? EndpointSystemId { get; set; }
        public EndPoint EndPoint { get; set; }
        public AdHocAccessRequest AdHocAccessRequests { get; set; }
        public AccountType AccountType { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public long UserID { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? StartDate { get; set; }
        public string ErrorMessage { get; set; }
        public long? ParentrequestId { get; set; }
        public string RequesteeUserName { get; set; }
        public string RequesterUserName { get; set; }

        public bool IsAdhocRequest { get; set; }
        public bool IsBulkRequest { get; set; }
        public bool IsUndoEventRequest { get; set; }
        public string Entitlement { get; set; }
        public ApplicationType ApplicationType { get; set; }
        
        public static List<PresentationAccessRequest> GetByProgressStatus(DI.Chakra.Infrastructure.Enums.AccessRequestProgress status, bool isAdminMode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new AccessRequestRepository(dataAccessTransaction).GetByProgressStatus(status, isAdminMode));
            }
        }

        public static List<PresentationAccessRequest> GetAllByProgressStatus(DI.Chakra.Infrastructure.Enums.AccessRequestProgress progressStatus, long requestedFor, DateTime? dtFrom, DateTime? dtTo, long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new AccessRequestRepository(dataAccessTransaction).GetAllByProgressStatus(progressStatus, requestedFor, dtFrom, dtTo, endpointSystemId));
            }
        }

        public static PresentationAccessRequest ORMap(EntityModel.Presentation.PresentationAccessRequest presentationAccessRequest)
        {
            if (presentationAccessRequest == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationAccessRequest, PresentationAccessRequest>(presentationAccessRequest);
        }

        public static List<PresentationAccessRequest> ORMapList(IList<EntityModel.Presentation.PresentationAccessRequest> presentationAccessRequest)
        {
            return presentationAccessRequest.Select(ORMap).ToList();
        }

        public static List<PresentationAccessRequest> GetAccessRequestForReportExtraction()
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                var currentDate = DateTime.Now.AddDays(-1);
                DateTime? dtFrom = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 0, 0, 0); 
                DateTime? dtTo = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 23, 59, 59);
                return ORMapList(new AccessRequestRepository(dataAccessTransaction).GetAccessRequestForReportExtraction(dtFrom, dtTo));
            }
        }
    }
}
