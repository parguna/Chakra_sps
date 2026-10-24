using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    public class SAPEPUserGroups
    {
        public long SAPEPUserInfo_Id { get; set; }
        public long SAPEPGroupInfo_Id { get; set; }
        public long EndpointSystemId { get; set; }

        public short Version { get; set; }
    }
}
