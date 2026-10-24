using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_AccessRequest")]
    public class AccessRequest : EntityBase, ICreatable, IUpdatable
    {


        public Nullable<long> RequesterId { get; set; }

        public long UserId { get; set; }

        public string Description { get; set; }

        public Nullable<long> AdHocAccessRequestId { get; set; }

        public Nullable<long> EndpointSystemId { get; set; }

        public Nullable<long> EmployeeEventTriggerId { get; set; }
        public Nullable<long> EventUndoRequestId { get; set; }

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

        //user snapshot
        public string OrgCompany { get; set; }
        public string OrgDepartment { get; set; }
        public string OrgDivision { get; set; }
        public string OrgBranch { get; set; }
        public string OrgSection { get; set; }
        public string OrgUnit { get; set; }


        [ForeignKey("AdHocAccessRequestId")]
        public AdHocAccessRequest AdHocAccessRequest { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("RequesterId")]
        public User Requester { get; set; }

        [ForeignKey("EndpointSystemId")]
        public EndPoint Application { get; set; }

        public string RESPONSETXID { get; set; }

        public Nullable<long> BulkRequestId { get; set; }

        [ForeignKey("EventUndoRequestId")]
        public EventUndoRequest EventUndoRequest { get; set; }
        public string ConflictsRemarks { get; set; }
    }
}
