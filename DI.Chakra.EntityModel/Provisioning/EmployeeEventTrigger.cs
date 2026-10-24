using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_EmployeeEventTrigger")]
    public class EmployeeEventTrigger : EntityBase,ICreatable,IUpdatable
    {
        
        public long UserId { get; set; }
        public EmployeeEvents EventType { get; set; }
        public DateTime EffectiveDate { get; set; }
        public Nullable<DateTime> OldEffectiveDate { get; set; }
        public bool IsRequestCreated { get; set; }
        //public bool IsExecuted { get; set; }
        public bool IsAdminCheckRequired { get; set; }
        //public bool IsRejoiningUser { get; set; }
        public string UserDetails { get; set; }
        public string OldUserDetails { get; set; }
        public bool IsActive { get; set; }
        public AccessRequestStatus RequestStatus { get; set; }
        public string Comments { get; set; }
        public bool IsMailSent { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; } 
        public Nullable<long> UserEventId { get; set; }
        public Nullable<DateTime> ActualEventDate { get; set; }
        public int EventSequence { get; set; } = 1;

    }
}
