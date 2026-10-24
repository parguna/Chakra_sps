using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class SAPHANARoleInfo : DtoBase, IEndpointEntitlementInfo
    {
        

        public long EndPointSystemId { get; set; }

        public string ROLE_CODE { get; set; }

        public string ROLE_DESC { get; set; }

        public string TargetSysId { get; set; }

        public string RoleType { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
        public List<SAPHANAUserinfo> SAPUserinfo { get; set; }

        public int? Version { get; set; }

        public string EntitlementName { get; set; }

        public string EntitlementDesc { get; set; }

        public EntitlementType EntitlementType { get; set; }

        public bool? CanRequest { get; set; }
        public string UserDefinedDesc { get; set; }

    }
}
