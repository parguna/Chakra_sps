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
    public class SMSUserAccessLevel
    {
        public long SMSUserInfo_Id { get; set; }
        public long SMSAccessLevel_Id { get; set; }
    }
}
