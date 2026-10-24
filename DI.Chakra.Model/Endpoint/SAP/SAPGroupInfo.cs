using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Model.SAP
{
    [TableInfo("DI_SAP_GROUPINFO", "DI_SAP_GROUPINFO")]
    [Table("DI_SAP_GROUPINFO")]
    public class SAPGroupInfo : IAggregateRoot , IEndpointEntitlementInfo
    {
        [Key]
        public long Id { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string GROUP_CODE { get; set; }
        public string GROUP_DESC { get; set; }
        public Nullable<int> TargetSysId { get; set; } // string
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public int? Version { get; set; }

        public virtual IList<SAPUserinfo> SAPUserinfos { get; set; }
        public string EntitlementName
        {
            get { return GROUP_CODE; }
        }

        public string EntitlementDesc
        {
            get { return GROUP_DESC; }
        }
        public EntitlementType EntitlementType
        {
            get { return EntitlementType.Group; }
        }
        public SAPGroupInfo()
        {
            SAPUserinfos = new List<SAPUserinfo>();
        }

        public static SAPGroupInfo CreateSAPGroupInfo()
        {
            SAPGroupInfo SAPGroupInfo = new SAPGroupInfo();
            return SAPGroupInfo;
        }
    }

}
