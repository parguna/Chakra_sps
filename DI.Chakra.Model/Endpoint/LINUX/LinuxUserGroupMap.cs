using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class LinuxUserGroupMap:IAggregateRoot
    {
        public long LinuxUserInfo_Id { get; set; }
        public long LinuxGroupInfo_Id { get; set; }
        public long EndpointSystemId { get; set; }
        public int? Version { get; set; }
    }
}
