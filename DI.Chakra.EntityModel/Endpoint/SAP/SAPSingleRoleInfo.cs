using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAP_SINGLEROLEINFO")]
    public class SAPSingleRoleInfo :EntityBase,ICreatable,IUpdatable
    {
        public string SingleRole_Code { get; set; }

        public string SingleRole_Desc { get; set; }

        public long EndPointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public int? Version { get; set; }

        public  IList<SAPTCodeInfo> SAPTCodes { get; set; }

        public  IList<SAPCompositeRoleInfo> SAPCompositeRoleInfos { get; set; }

        
    }
}
