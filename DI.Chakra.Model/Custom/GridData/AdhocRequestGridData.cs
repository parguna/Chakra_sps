using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Model.GridData
{
    public class AdhocRequestGridData
    {
        public int AdHocAccessRequestId { get; set; }
        public string TenantId { get; set; }
        public int RequesterUserId { get; set; }
        public int RequesteeUserId { get; set; }
        public EndpointType EndpointSystemType { get; set; }
        public long EndpointSystemId { get; set; }
        public string EndpoindSystemName { get; set; }
        public AdHocActionType AdhocActionType { get; set; }
        public bool IsSelfRequest { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string Comments { get; set; }
        public string EntitlementDetail { get; set; }

        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
