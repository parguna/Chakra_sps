using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Model.LotusNotes
{
    
    public class LotusUserGroupMap : IAggregateRoot
    {
        public long LotusUserInfo_UserId { get; set; }
        public long LotusGroupInfo_Id { get; set; }
        public long EndpointSystemId { get; set; }
        public int? Version { get; set; }
       
    }
}
