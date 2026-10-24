using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAP_FIELDVALUEMAPPING")]
    public class SAPFieldValueMapping : EntityBase,ICreatable,IUpdatable
    {
        public string RoleCode { get; set; }

        public string FieldCode { get; set; }

        public string FromValue { get; set; }

        public string ToValue { get; set; }

        public long EndPointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public int? Version { get; set; }
    }
}
