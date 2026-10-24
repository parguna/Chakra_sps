using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [NotMapped]
    public class PresentationAccountEntitlementAuditTrial:EntityBase
    {
        public long EndPointSystemId { get; set; }
        public string EndpointName { get; set; }
        public string DisplayName { get; set; }
        public string AccountType { get; set; }
        public string UserName { get; set; }
        public string Permission { get; set; }
        public string Remarks { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
