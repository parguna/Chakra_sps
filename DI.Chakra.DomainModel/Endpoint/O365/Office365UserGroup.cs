using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class Office365UserGroup : IAggregateRoot
    {
        public long Office365GroupInfo_GroupId { get; set; }
        public long Office365UserInfo_UserId { get; set; }
        public long EndpointSystemId { get; set; }      
    }
}
