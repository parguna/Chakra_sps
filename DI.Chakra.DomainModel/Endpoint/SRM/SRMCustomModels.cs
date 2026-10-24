using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SRMRoleData
    {
        public string AccountName { get; set; }

        public string SRPRoles { get; set; }
        public string SRPEnablerRoles { get; set; }

        public string SUSRoles { get; set; }

        public string FLINSBIRoles { get; set; }
        public string FLINSBIPOrgRoles { get; set; }
        public string FLINSBIPGrpRoles { get; set; }

        public string ADGroups { get; set; }
    }

    public class SRMUserRoleMappingData
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string BusinessRole { get; set; }
        public string POrg { get; set; }
        public string PGrp { get; set; }
    }

    public class SRMRolePOrgData
    {
        public string EnablerRule { get; set; }
        public string BusinessRole { get; set; }
        public string POrg { get; set; }
        public string PGrp { get; set; }
    }
}
