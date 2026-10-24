using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SMSBadgeAccessLevel : IAggregateRoot
    {
        public int BadgeKey { get; set; }
        public int AccessLevelId { get; set; }
    }
}
