using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    public class PresentationVPNUserInfo
    {
        public long UserId { get; set; }
        public long EndpointUserId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public int EmployeeType { get; set; }
        public string OneKeyTokenId { get; set; }
    }
}
