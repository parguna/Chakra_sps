using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.Model
{
    [Serializable]
    public class FailedRequestGridData
    {
        public long AccessRequestId { get; set; }
        public Nullable<long> AdHocAccessRequestId { get; set; }
        public long UserId { get; set; }
        public string SystemName { get; set; }
        public string DisplayName { get; set; }
        public string Company { get; set; }
        public string CostCenter { get; set; }
        public string Department { get; set; }
        public string Division { get; set; }
        public string Section { get; set; }
        public string Remarks { get; set; }
        public string EmployeeID { get; set; }
        public DateTime RequestDate { get; set; }
        public AccessRequestSource RequestSource { get; set; }
    }
}
