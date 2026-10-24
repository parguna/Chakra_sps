using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class AccountEntitlementAuditTrial: DomainModelBase<AccountEntitlementAuditTrial, EntityModel.AccountEntitlementAuditTrial>
    {
        public long EndPointSystemId { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string AccountType { get; set; }
        public string Permission { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
