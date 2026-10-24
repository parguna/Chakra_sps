using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAPHANA_ObjectValues")]
    public class SAPHANAObjectValues : EntityBase,ICreatable,IUpdatable
    {
        public string SapObjectType { get; set; }

        public string SapObjectCode { get; set; }

        public String SapOkFlag { get; set; }

        public string ModificationID { get; set; }

        public string OrgName { get; set; }

        public long EndPointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public int? Version { get; set; }
    }
}
