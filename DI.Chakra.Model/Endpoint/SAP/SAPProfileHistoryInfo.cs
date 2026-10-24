using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Model.SAP
{
    public class SAPProfileHistoryInfo : IAggregateRoot
    {
        public int SAP_PROFILEID { get; set; }
        public int HistoryId { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string PROFILE_CODE { get; set; }
        public string PROFILE_DESC { get; set; }
        public string TargetSysId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? Version { get; set; }


        public static SAPProfileHistoryInfo CreateSAPProfileHistoryInfo()
        {
            SAPProfileHistoryInfo SAPProfileHistoryInfo = new SAPProfileHistoryInfo();

            return SAPProfileHistoryInfo;
        }
    }
}
