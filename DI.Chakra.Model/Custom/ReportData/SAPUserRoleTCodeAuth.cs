using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class SAPUserRoleTCodeAuth
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Department { get; set; }
        public bool? IsActive { get; set; }
        public string RoleCode { get; set; }
        public string TCode { get; set; }
        public string AuthorizationCode { get; set; }
        public string ProfileCode { get; set; }

    }
}
