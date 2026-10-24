using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_AccountEntitlementAuditTrial")]
    public class AccountEntitlementAuditTrial : EntityBase, ICreatable, IUpdatable
    {
        public long EndPointSystemId { get; set; }
        public string DisplayName { get; set; }
        [MaxLength(512)]
        public string UserName { get; set; }
        public string Permission { get; set; }
        [MaxLength(32)]
        public string AccountType { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
