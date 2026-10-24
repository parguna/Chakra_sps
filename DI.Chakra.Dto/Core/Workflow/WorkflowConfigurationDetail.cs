using DI.Chakra.Infrastructure.Enums;
using System;


namespace DI.Chakra.Dto
{
    [Serializable]
    public class WorkflowConfigurationDetail : DtoBase
    {
        public long WorkflowConfigurationId { get; set; }

        public byte SequenceNumber { get; set; }

        public ApprovalUserType ApprovalUserType { get; set; }

        public long Approver { get; set; }

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

        public WorkflowConfiguration WorkflowConfigurations { get; set; }
    }
}
