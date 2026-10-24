using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{

    public class GEMSRoleInfo : DtoBase, IEndpointEntitlementInfo
    {
        public long EndPointSystemId { get; set; }
        public string Name { get; set; }
        public string Display_Name { get; set; }
        public string Description { get; set; }
        public Nullable<int> Extension_ID { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public IList<GEMSUserInfo> GEMSUserInfos { get; set; }
        public EndPoint EndPoint { get; set; }
        public string EntitlementName { get; set; }
        public string EntitlementDesc { get; set; }
        public EntitlementType EntitlementType { get; set; }
    }
}
