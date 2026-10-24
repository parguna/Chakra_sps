using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_Task")]
    public class Task : EntityBase, ICreatable, IUpdatable
    {
        public byte RequestTypeId { get; set; }

        public Nullable<long> AttestationId { get; set; }

        public Nullable<long> AccessRequestId { get; set; }
        public Nullable<long> BulkRequestId { get; set; }
        public Nullable<long> EmployeeEventTriggerId { get; set; }

        public Nullable<long> SubTaskId { get; set; }

        public Nullable<long> ActionUserId { get; set; }

        public Nullable<long> ActionGroupId { get; set; }

        public byte WorkflowStateId { get; set; }

        public long WorkflowConfigurationId { get; set; }

        public Guid WorkflowInstanceId { get; set; }

        public bool IsEscalated { get; set; }

        public bool IsNotResponded { get; set; }

        public bool IsCompleted { get; set; }

        public bool IsActive { get; set; }

        public Nullable<DateTime> ActionDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public StatusType Action { get; set; }

        public string Approver { get; set; }

        public string Comments { get; set; }

        public string Remarks { get; set; }

        public bool IsDelegated { get; set; }

        public Nullable<long> DelegatedFrom { get; set; }

        public string DelegatedBy { get; set; }

        public string DelegationComments { get; set; }

        public Nullable<DateTime> DelegatedDate { get; set; }

        public bool IsActionOnlyRequiredTask { get; set; }

        public bool IsGroupTask { get; set; }

        public bool IsLocked { get; set; }

        public Nullable<long> AssignedTo { get; set; }

        public Nullable<long> AssignedBy { get; set; }

        public string GroupName { get; set; }

        public AccessRequest AccessRequest { get; set; }
        public Nullable<long> AttestationCampaignId { get; set; }
        [ForeignKey("AttestationCampaignId")]
        public AttestationCampaign AttestationCampaign { get; set; }

        [ForeignKey("SubTaskId")]
        public SubTask SubTask { get; set; }

        [ForeignKey("ActionUserId")]
        public User ActionUser { get; set; }

        [ForeignKey("DelegatedFrom")]
        public User Delegated { get; set; }

        [ForeignKey("AttestationId")]
        public Attestation Attestation { get; set; }

        [ForeignKey("WorkflowConfigurationId")]
        public WorkflowConfiguration WorkFlowConfiguration { get; set; }

        [ForeignKey("EmployeeEventTriggerId")]
        public EmployeeEventTrigger EmployeeEventTrigger { get; set; }

        [ForeignKey("BulkRequestId")]
        public BulkRequest BulkRequest { get; set; }
    }
}
