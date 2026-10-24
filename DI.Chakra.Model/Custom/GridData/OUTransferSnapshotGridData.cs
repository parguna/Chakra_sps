using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class OUTransferSnapshotGridData
    {
        public long SnapshotId { get; set; }
        public string UserName { get; set; }
        public string EmployeeNo { get; set; }
        public string EmailID { get; set; }
        public long EndpointSystemTypeID { get; set; }
        public string EndpointSystemType { get; set; }
        public string EndpointsystemName { get; set; }
        public long EndpointUserID { get; set; }
        public string AccountName { get; set; }
        public EntitlementType EntitlementType { get; set; }
        public string Entitlement { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
