using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel 
{
    [NotMapped]
    public class AccountEntitlementRecon
    { 
        public AccountReconcilation AccountRecon  { get; set; } 
        
        public List<EntitlementReconcilation> Entitlement { get; set; }
        
    }
}
