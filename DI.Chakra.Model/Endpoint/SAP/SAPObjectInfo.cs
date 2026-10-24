using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.SAP
{
    [Table("DI_SAP_Objects")]
    public class SAPObjectInfo : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }

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
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string TenantId { get; set; }
        public int? Version { get; set; }


    }
}
