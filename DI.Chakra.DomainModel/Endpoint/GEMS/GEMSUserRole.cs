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

    public class GEMSUserRole
    { 
        public long GEMSUserInfo_Id { get; set; }
        public long GEMSRoleInfo_Id { get; set; }
        public long EndpointSystemId { get; set; }
    }

}

