using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class InhouseSystemsGridData
    {
        public long UserId { get; set; }
        public long? DIUserID { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string NRIC { get; set; }
        public string Email { get; set; }
        public string EmployeeId { get; set; }
        public int InhouseSystemId { get; set; }
        public Nullable<int> EndpointSystemId { get; set; }
        public string SystemName { get; set; }
        public string ApplicationOwner { get; set; }
        public string Permission { get; set; }
        public string SystemCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsLocked { get; set; }
        public bool IsDeleted { get; set; }
        public AccountType AccountType { get; set; }
        public LockScenario Lockscenario { get; set; }
        
        
        public System.DateTime? AccountCreatedDate { get; set; }
        public System.DateTime? LastLoginDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public string IsMapped { get; set; }
        public string Owner { get; set; }
    }
}
