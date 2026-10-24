using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model.SAP
{
    [Table("DI_SAP_AuthObjectMap")]
    public class SAPAuthorizationToObject : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }

        public string AuthorizationCode { get; set; }
        public string ObjectCode { get; set; }
        public string FieldCode { get; set; }
        public string FromValue { get; set; }
        public string ToValue { get; set; }

        public long EndPointSystemId { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string TenantId { get; set; }
        public int? Version { get; set; }


    }
}
