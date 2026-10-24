using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class SRMHangingRoleInfo : DtoBase
    {
        public long EndPointSystemId { get; set; }
        public string AccountName { get; set; }
        public AccountType AccountType { get; set; }
        public string ADGroups { get; set; }
        public string BIRoles { get; set; }
        public string SUPRoles { get; set; }
        public string SRPRoles { get; set; }
        public string Reserved1 { get; set; }
        public string Reserved2 { get; set; }
        public string Reserved3 { get; set; }
        public string Reserved4 { get; set; }
        public string Reserved5 { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
