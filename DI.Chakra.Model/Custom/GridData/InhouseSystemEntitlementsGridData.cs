using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class InhouseSystemEntitlementsGridData
    {
        public int InhouseSystemEntitlementId { get; set; }
        public int InhouseSystemsId { get; set; }
        public long EndPointSystemID { get; set; }
        public string InhouseSystemsCode { get; set; }
        public string SystemName { get; set; }
        public string ApplicationOwner { get; set; }
        public string EntitlementName { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public bool CanRequestMultiple { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
