using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure.CustomObjects
{
    public class InhouseUserCustomInfo
    {
        public long UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<KeyValuePair<string, string>> Entitlements { get; set; }
        public long EndpointSystemId { get; set; }
        public string CreatedBy { get; set; }
    }
}
