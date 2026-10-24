using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationApprovers : AccessRequest
    {
        public int ApproverLevelId { get; set; }

        public string ApproverName { get; set; }

        public StatusType Action { get; set; }

        public System.DateTime ActionDate { get; set; }

        public byte WorkflowState { get; set; }

        public static IList<PresentationApprovers> GetByAccessRequestId(long accessRequestId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new AccessRequestRepository(dataAccessTransaction).GetByAccessRequestId(accessRequestId));
            }
        }

        public static PresentationApprovers ORMap(EntityModel.Presentation.PresentationApprovers presentationApproversEntityModel)
        {
            if (presentationApproversEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationApprovers, PresentationApprovers>(presentationApproversEntityModel);
        }

        public static List<PresentationApprovers> ORMapList(IList<EntityModel.Presentation.PresentationApprovers> presentationApproversEntityModel)
        {
            return presentationApproversEntityModel.Select(ORMap).ToList();
        }
    }


    public class FailedRequestGridData
    {
        public long AccessRequestId { get; set; }
        public string RequestSourceType { get; set; }
        public Nullable<long> AdHocAccessRequestId { get; set; }
        public long UserId { get; set; }
        public string SystemName { get; set; }
        public string DisplayName { get; set; }
        public string Company { get; set; }
        public string CostCenter { get; set; }
        public string Department { get; set; }
        public string Division { get; set; }
        public string Section { get; set; }
        public string Remarks { get; set; }
        public string EmployeeID { get; set; }
        public DateTime RequestDate { get; set; }
        public AccessRequestSource RequestSource { get; set; }
        public string AdditionalInfo { get; set; }
    }

    public class MyRequestsGridData
    {
        public long AdHocAccessRequestId { get; set; }
        public long AccessRequestID { get; set; }
        public string Name { get; set; }
        public string RequestSource { get; set; }
        public string Remarks { get; set; }
        public string RequestType { get; set; }
        public System.DateTime RequestedDate { get; set; }
        public System.DateTime? ExecutionDate { get; set; }
        public AccessRequestStatus RequestStatus { get; set; }
        public string Comments { get; set; }
        public string RequesterName { get; set; }
        public string RequesteeName { get; set; }
        public AdHocActionType AdhocActionType { get; set; }
        public ApplicationType EntitlementDetailType { get; set; }
        public long? EndpointSystemId { get; set; }
        public long? BulkRequestId { get; set; }
        public EndPoint EndPoint { get; set; }
        public long UserID { get; set; }
        public bool IsAdhocRequest { get; set; }
        public bool IsBulkRequest { get; set; }
        public bool IsUndoEventRequest { get; set; }
        public DateTime? EndDate { get; set; }

        public DateTime? StartDate { get; set; }
        public System.DateTime? ValidFrom { get; set; }
        public System.DateTime? ValidTo { get; set; }
        public AccountType AccountType { get; set; }
        public string AdditionalInfo { get; set; }
    }
}
