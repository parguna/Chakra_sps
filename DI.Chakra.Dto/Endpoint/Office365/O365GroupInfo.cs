using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class Office365GroupInfo : DtoBase, IEndpointEntitlementInfo
    {
        public Office365GroupInfo()
        {
            O365Users = new List<Office365UserInfo>();
        }
        public long EndPointSystemId { get; set; }
        public string displayName { get; set; }
        public string description { get; set; }
        public string member { get; set; }
        public string distinguishedName { get; set; }
        public string instanceType { get; set; }
        public DateTime? whenCreated { get; set; }
        public DateTime? whenChanged { get; set; }
        public string name { get; set; }
        public string objectSid { get; set; }
        public string sAMAccountName { get; set; }
        public string sAMAccountType { get; set; }
        public string groupType { get; set; }
        public string groupTypeDesc { get; set; }
        public string objectCategory { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public List<Office365UserInfo> O365Users { get; set; }

        public EndPoint EndPoint { get; set; }

        public string EntitlementName { get; set; }

        public string EntitlementDesc { get; set; }
        public EntitlementType EntitlementType { get; set; }
    }
}
