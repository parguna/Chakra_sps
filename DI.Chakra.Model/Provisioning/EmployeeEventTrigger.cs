using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_EmployeeEventTrigger")]
    public class EmployeeEventTrigger
    {
    
        [Key]
        public long Id { get; set; }
        public string TenantId { get; set; }
        public long UserId { get; set; }
        public EmployeeEvents EventType { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsRequestCreated { get; set; }
        //public bool IsExecuted { get; set; }
        public bool IsAdminCheckRequired { get; set; }
        public bool IsRejoiningUser { get; set; }
        public string UserDetails { get; set; }
        public string OldUserDetails { get; set; }
        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        
    }
}
