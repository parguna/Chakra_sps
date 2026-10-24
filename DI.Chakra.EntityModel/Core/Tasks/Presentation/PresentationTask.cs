using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel.Presentation
{
    [NotMapped]
    public class PresentationTask
    {
        public PresentationTask()
        {
            this.BulkReqDetails = new List<BulkRequestDetail>();
        }
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
        public ApplicationType ApplicationType { get; set; }
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
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public Nullable<long> WorkFlowConfigurationId { get; set; }
        public long Level { get; set; }
        public string RequestType { get; set; }
        public string TaskName { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        public string ActionUser { get; set; }
        public string GroupName { get; set; }
        public string AssignedToADID { get; set; }
        public string ActionUserADID { get; set; }
        public string RequestForSupDisplayName { get; set; }
        public string RequestForDept { get; set; }
        public string RequestForDesignation { get; set; }
        public string TaskSource { get; set; }
        public List<BulkRequestDetail> BulkReqDetails { get; set; }
        public string EntitlementXML { get; set; }
        public string Entitlement { get; set; }
        public string IsDelegatedByUser { get; set; }
        public string RequestTypeName { get; set; }
    }
}
