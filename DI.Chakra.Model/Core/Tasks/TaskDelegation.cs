using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{

    [Table("DI_M_TaskDelegation")]
    [Serializable]
    public class TaskDelegation : IAggregateRoot
    {
        public TaskDelegation()
        {
        }

        [Key]
        public long Id { get; set; }
        public long ActionUser { get; set; }
        public long DelegatedTo { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public bool DelegateAttestationTask { get; set; }
        public bool DelegateProvisioningTask { get; set; }
        public bool DelegateSODTask { get; set; }
        public bool IsActive { get; set; }
        public string DelegationComments { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
        [ForeignKey("DelegatedTo")]
        public virtual User DelegatedToUser { get; set; }

        public string DelegateToUserName
        {
            get
            {
                return this.DelegatedToUser.DisplayName;
            }
        }
    }

}
