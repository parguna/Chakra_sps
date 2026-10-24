using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class IACMGroupInfo : DtoBase, IEndpointEntitlementInfo
    {
        public long EndPointSystemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public EndPoint EndPoint { get; set; }
        public List<IACMUserInfo> Users { get; set; }

        public string UserDefinedDisplayName { get; set; }

        public string UserDefinedDesc { get; set; }

        public bool? CanRequest { get; set; }

        public long? Level1Approver { get; set; }

        public long? Level2Approver { get; set; }

        public bool? IsLevel1Group { get; set; }

        public bool? IsLevel2Group { get; set; }

        public string EntitlementName { get; set; }

        public string EntitlementDesc { get; set; }

        public EntitlementType EntitlementType { get; set; }
    }
}
