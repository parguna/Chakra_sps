using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class OracleEBSUserRoleReport
    {
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public Nullable<DateTime> CreatedAt { get; set; }
        public Nullable<DateTime> ExpiryAt { get; set; }
    }
}
