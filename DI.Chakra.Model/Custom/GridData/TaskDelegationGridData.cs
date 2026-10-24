
using System;
namespace DI.Chakra.Model
{
    public class TaskDelegationGridData
    {
        public long DelegationId { get; set; }
        public long ActionUser { get; set; }
        public string ActionUserName { get; set; }
        public long DelegatedToUserId { get; set; }
        public string DelegatedToUserName { get; set; }
        public string DelegationComments { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public bool DelegateAttestationTask { get; set; }
        public bool DelegateProvisioningTask { get; set; }
        public bool DelegateSODTask { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdvance { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
