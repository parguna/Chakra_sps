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
    [Table("DI_SAP_TCODEINFO")]
    public class SAPTCodeInfo
    {
        [Key]
        public long Id { get; set; }
        public string TCODE_Code { get; set; }
        public string TCODE_Desc { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public int? Version { get; set; }

        
        public virtual IList<SAPSingleRoleInfo> SAPSingleRoleInfos { get; set; }

        public SAPTCodeInfo()
        {
            SAPSingleRoleInfos = new List<SAPSingleRoleInfo>();
        }
        [NotMapped]
        public string Role { get; set; }
    }
}
