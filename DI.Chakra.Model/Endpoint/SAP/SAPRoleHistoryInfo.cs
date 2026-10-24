using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Model.SAP
{
  public  class SAPRoleHistoryInfo : IAggregateRoot
    {
        public string ROLE_CODE { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string ROLE_DESC { get; set; }
        public string TargetSysId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? Version { get; set; }


        public static SAPRoleHistoryInfo CreateSAPRoleHistoryInfo()
        {
            SAPRoleHistoryInfo SAPRoleHistoryInfo = new SAPRoleHistoryInfo();

            return SAPRoleHistoryInfo;
        }
    }
}
