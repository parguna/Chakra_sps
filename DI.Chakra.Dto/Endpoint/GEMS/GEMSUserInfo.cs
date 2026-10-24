using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class GEMSUserInfo : DtoBase, IEndpointUserInfo
    {
        public long EndPointSystemId { get; set; }
        public string USER_NAME { get; set; }
        public string DISPLAY_NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string EMAIL { get; set; }
        public Nullable<DateTime> DATEMODIFIED { get; set; }
        public string LASTUSER { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public bool IsDummyEntry { get; set; }
        public List<GEMSRoleInfo> GEMSRoles { get; set; }
        public EndPoint EndPoint { get; set; }
        public string AccountName { get; set; }
        public string AccountPassword { get; set; }
        public string DisplayName { get; set; }
        public List<IEndpointEntitlementInfo> Entitlements { get; set; }

    }
}
