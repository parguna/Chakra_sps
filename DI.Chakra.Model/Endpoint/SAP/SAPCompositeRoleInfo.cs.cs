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
    [TableInfo("DI_SAP_COMPOSITEROLEINFO", "DI_SAP_COMPOSITEROLEINFO")]
    [Table("DI_SAP_COMPOSITEROLEINFO")]
    public class SAPCompositeRoleInfo : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public string CompositeRole_Code { get; set; }
        public string CompositeRole_Desc { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public int? Version { get; set; }


     
        public virtual IList<SAPSingleRoleInfo> SAPSingleRoleInfos { get; set; }

        
        public SAPCompositeRoleInfo()
        {
           
            SAPSingleRoleInfos = new List<SAPSingleRoleInfo>();
        }
    }
}
