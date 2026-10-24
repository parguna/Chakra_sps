using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class AccessRequest : DtoBase
    {


        public Nullable<long> RequesterId { get; set; }

        public long UserId { get; set; }

        public string Description { get; set; }

        public Nullable<long> AdHocAccessRequestId { get; set; }

        public Nullable<long> EndpointSystemId { get; set; }

        public Nullable<long> EmployeeEventTriggerId { get; set; }

        public DI.Chakra.Infrastructure.Enums.AccessRequestStatus RequestStatus { get; set; }

        public DI.Chakra.Infrastructure.Enums.AccessRequestProgress ProgressStatus { get; set; }

        public DI.Chakra.Infrastructure.Enums.AccessRequestSource RequestSource { get; set; }

        public Nullable<System.DateTime> ActivationDate { get; set; }

        public Nullable<System.DateTime> DeactivationDate { get; set; }

        public Nullable<System.DateTime> ExecutionTime { get; set; }

        public string Remarks { get; set; }

        public string ApproverType { get; set; }

        public Nullable<long> AttestationCampaignId { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public Guid TransactionId { get; set; }

        public bool IsCurrentLevelApproved { get; set; }

        public string Comments { get; set; }

        public string AdditionalInfo { get; set; }

        public long? ParentRequestId { get; set; }

        public bool IsParentRequest { get; set; }

        public bool IsDependentRequest { get; set; }

        public bool IsDependentRequestCreated { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public AccountType AccountType { get; set; }
        public Nullable<long> AttestationReviewId { get; set; }
        public AdHocAccessRequest AdHocAccessRequest { get; set; }
        public User User { get; set; }
        public User Requester { get; set; }
        public EndPoint Application { get; set; }
        public string InterimIds { get; set; }
        public string RESPONSETXID { get; set; }
        public Nullable<long> BulkRequestId { get; set; }
        public Nullable<long> EventUndoRequestId { get; set; }
        public string ConflictsRemarks { get; set; }

        public string OrgCompany { get; set; }
        public string OrgDepartment { get; set; }
        public string OrgDivision { get; set; }
        public string OrgBranch { get; set; }
        public string OrgSection { get; set; }
        public string OrgUnit { get; set; }
    }
}
