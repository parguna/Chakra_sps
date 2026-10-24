using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class SMSCompany : DtoBase, IEndpointEntitlementInfo
    {
        public long EndPointSystemId { get; set; }
        public int CompanyKey { get; set; }
        public string Name { get; set; }
        public int SegmentID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public EndPoint EndPoint { get; set; }
        public string EntitlementName { get; set; }
        public string EntitlementDesc { get; set; }
        public Infrastructure.Enums.EntitlementType EntitlementType { get; set; }

    }
}
