using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAPHANA_COMPOSITEROLEINFO")]
    public class SAPHANACompositeRoleInfo : EntityBase,ICreatable,IUpdatable
    {
        public string CompositeRole_Code { get; set; }

        public string CompositeRole_Desc { get; set; }

       public long EndPointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }

        public string ModifiedBy { get; set; }

        public Nullable<System.DateTime> ModifiedDate { get; set; }

        public int? Version { get; set; }

        public  IList<SAPHANASingleRoleInfo> SAPSingleRoleInfos { get; set; }

        
        





    }
}
