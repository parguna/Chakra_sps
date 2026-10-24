using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class IACMUserInfo : DtoBase, IEndpointUserInfo
    {
        public long EndPointSystemId { get; set; }

        public string NRIC { get; set; }
        public string EmployeeId { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ADID { get; set; }

        public DateTime? AccountCreatedDate { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime? LockedDate { get; set; }

        public bool IsDisabled { get; set; }

        public DateTime? DisabledDate { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedDate { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public virtual List<IACMRoleInfo> Roles { get; set; }

        public virtual List<IACMGroupInfo> Groups { get; set; }

        public EndPoint EndPoint { get; set; }
        
        public string AccountName { get; set; }

        public string AccountPassword { get; set; }

        public List<IEndpointEntitlementInfo> Entitlements { get; set; }
    }
}
