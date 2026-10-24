using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Model.SAP
{
    [TableInfo("DI_SAP_GROUPHISTORYINFO", "DI_SAP_GROUPHISTORYINFO")]
    [Table("DI_SAP_GROUPHISTORYINFO")]
    public class SAPGroupHistoryInfo : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public Nullable<int> HistoryId { get; set; }
        public string TenantId { get; set; }
        public Nullable<int> EndPointSystemId { get; set; }
        public string GROUP_CODE { get; set; }
        public string GROUP_DESC { get; set; }
        public Nullable<int> TargetSysId { get; set; } // string
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public int? Version { get; set; }


        public static SAPGroupHistoryInfo CreateSAPGroupInfo()
        {
            SAPGroupHistoryInfo SAPGroupInfo = new SAPGroupHistoryInfo();

            return SAPGroupInfo;
        }
    }
}
