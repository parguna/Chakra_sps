using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class UsersEndPointSystemDetailsGridData
    {
        public int Id { get; set; }
        public string EndpointSystemTypeName { get; set; }
        public string HostName { get; set; }
        public long EndPointSystemId { get; set; } // DI_END_UserInfoEndPointSystemId
        public string UserName { get; set; }
        public string TenantId { get; set; }
        public string DisplayName { get; set; }
        public int UserId { get; set; } // DI_M_EndpointUserId
    }
}
