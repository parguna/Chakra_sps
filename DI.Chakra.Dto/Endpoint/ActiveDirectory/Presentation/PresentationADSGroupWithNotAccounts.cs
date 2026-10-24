using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationADSGroupWithNoAccounts
    {
        public long GroupId { get; set; }

        public long EndPointSystemId { get; set; }

        public string cn { get; set; }

        public string description { get; set; }

        public string member { get; set; }

        public string distinguishedName { get; set; }

        public string instanceType { get; set; }

        public Nullable<System.DateTime> whenCreated { get; set; }

        public Nullable<System.DateTime> whenChanged { get; set; }

        public Nullable<long> uSNCreated { get; set; }

        public Nullable<long> uSNChanged { get; set; }

        public string name { get; set; }

        public string objectGUID { get; set; }

        public string objectSid { get; set; }

        public string sAMAccountName { get; set; }

        public string sAMAccountType { get; set; }

        public string groupType { get; set; }

        public string groupTypeDesc { get; set; }

        public string objectCategory { get; set; }

        public string nTSecurityDescriptor { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public int? Version { get; set; }

        public bool? CanRequest { get; set; }

        public string EntitlementName { get; set; }
        
        public string EntitlementDesc { get; set; }
        
        public EntitlementType EntitlementType { get; set; }

    }
}
