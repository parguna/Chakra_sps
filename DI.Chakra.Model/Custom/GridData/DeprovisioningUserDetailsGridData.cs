using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class DeprovisioningUserDetailsGridData
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Department { get; set; }
        public string BusinessRole { get; set; }
        public string TenantId { get; set; }
        public long UserId { get; set; } 
    }
}
