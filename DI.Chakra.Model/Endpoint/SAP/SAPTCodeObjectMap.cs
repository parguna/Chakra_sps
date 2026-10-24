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
    [Table("DI_SAP_TCodeObject")]
    public class SAPTCodeObjectMap : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public string ObjectType { get; set; }
        public string FieldValue { get; set; }
        public string ObjectCode { get; set; }
        public string FieldCode { get; set; }

        public long EndPointSystemId { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string TenantId { get; set; }
        public int? Version { get; set; }

    }
}
