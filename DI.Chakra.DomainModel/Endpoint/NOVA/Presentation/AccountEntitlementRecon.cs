using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel 
{
    public class AccountEntitlementRecon
    {
        public AccountEntitlementRecon()
        {
            Entitlement = new List<EntitlementReconcilation>();
        }
        public AccountReconcilation AccountRecon { get; set; }

        public List<EntitlementReconcilation> Entitlement { get; set; }

    }
}
