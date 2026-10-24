using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Serializable]

    [Table("DI_M_AccessRequestHistory")] 
    public class AccessRequestHistory
    {

        [Key]
        public long Id { get; set; }
        public int AccessRequestId { get; set; }
        public string TenantId { get; set; }
        public int RequesterId { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
        public Nullable<int> ProvisioningRoleId { get; set; }
        public AccessRequestStatus RequestStatus { get; set; }
        public AccessRequestProgress ProgressStatus { get; set; }
        public AccessRequestSource RequestSource { get; set; }
        public Nullable<System.DateTime> ActivationDate { get; set; }
        public Nullable<System.DateTime> DeactivationDate { get; set; }
        public Nullable<System.DateTime> ExecutionTime { get; set; }
        public string Remarks { get; set; }
        public string ApproverType { get; set; }
        public Nullable<bool> IsRequestedToRemove { get; set; }
        public Nullable<bool> IsNewUser { get; set; }
        public Nullable<int> NewUserId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public bool IsCurrentLevelApproved { get; set; }
        public Guid TransactionId { get; set; }
        public string Comments { get; set; }

    }
}
