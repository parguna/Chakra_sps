using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
     [Table("DI_M_WorkflowConfigurationDetail")]
    public class WorkflowConfigurationDetail : EntityBase,ICreatable,IUpdatable
    {
        public long WorkflowConfigurationId { get; set; }

        public byte SequenceNumber { get; set; }

        public ApprovalUserType ApprovalUserType { get; set; }

        public int Approver { get; set; }

        public int ResponseTimeInDays { get; set; }

        public bool IsActionRequired { get; set; }

        public bool RequiresApprovalFromAll { get; set; }

        public bool IsMockGroup { get; set; }

        public bool IsReminderRequired { get; set; }

        public Nullable<int> ReminderIntervalInDays { get; set; }

        public bool IsEscalationRequired { get; set; }

        public Nullable<int> DaysToWaitAfterEscalation { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [ForeignKey("WorkflowConfigurationId")]
        public WorkflowConfiguration WorkflowConfigurations { get; set; }
    }
}
