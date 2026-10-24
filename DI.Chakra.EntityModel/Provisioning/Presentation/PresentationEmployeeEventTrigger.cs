using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationEmployeeEventTrigger
    {
        public long TriggerId { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string ADID { get; set; }
        public string EmployeeId { get; set; }
        public EmployeeEvents EventType { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string OldEffectiveDate { get; set; }
        public bool IsRequestCreated { get; set; }
        public bool IsAdminCheckRequired { get; set; }
        public string UserDetails { get; set; }
        public string OldUserDetails { get; set; }
        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Comments { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }
        public Nullable<DateTime> ActualEventDate { get; set; }
    }
}
