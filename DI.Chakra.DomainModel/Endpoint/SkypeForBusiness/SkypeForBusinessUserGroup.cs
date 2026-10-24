using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class SkypeForBusinessUserGroup : IAggregateRoot
    {
        public long SkypeForBusinessResponseGroupInfo_GroupId { get; set; }
        public long SkypeForBusinessUserInfo_UserId { get; set; }
        public long EndpointSystemId { get; set; }      
    }
}
