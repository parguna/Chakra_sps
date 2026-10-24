using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model.SAP
{
    [Table("DI_SAP_SINGLEROLEINFO")]
    public class SAPSingleRoleInfo : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public string SingleRole_Code { get; set; }
        public string SingleRole_Desc { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        public int? Version { get; set; }

     
        public virtual IList<SAPTCodeInfo> SAPTCodes { get; set; }
        public virtual IList<SAPCompositeRoleInfo> SAPCompositeRoleInfos { get; set; }

        public SAPSingleRoleInfo()
        {
            SAPTCodes = new List<SAPTCodeInfo>();
            SAPCompositeRoleInfos = new List<SAPCompositeRoleInfo>();
        }
    }
}
