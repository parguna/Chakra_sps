using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAPHANA_Objects")]
    public class SAPHANAObjectInfo : EntityBase,ICreatable,IUpdatable
    {
        public string SapObjectCode { get; set; }

        public string FieldCode1 { get; set; }

        public string FieldCode2 { get; set; }

        public string FieldCode3 { get; set; }

        public string FieldCode4 { get; set; }

        public string FieldCode5 { get; set; }

        public string FieldCode6 { get; set; }

        public string FieldCode7 { get; set; }

        public string FieldCode8 { get; set; }

        public string FieldCode9 { get; set; }

        public string FieldCode0 { get; set; }

        public long EndPointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public int? Version { get; set; }

    }
}
