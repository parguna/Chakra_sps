using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class SSIDSRolelnfo : DtoBase, IEndpointEntitlementInfo
    {
        public SSIDSRolelnfo()
        {
            this.SSIDSUserInfos = new List<SSIDSUserInfo>();
        }
        public long EndPointSystemId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public IList<SSIDSUserInfo> SSIDSUserInfos { get; set; }

        public EndPoint EndPoint { get; set; }

        public string EntitlementName
        {
            get { return Convert.ToString(this.RoleName); }
        }

        public string EntitlementDesc
        {
            get { return this.Description; }
        }
        public EntitlementType EntitlementType
        {
            get { return EntitlementType.Role; }
        }
    }
}
