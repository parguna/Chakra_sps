using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class SkypeForBusinessResponseGroupInfo:DtoBase
    {
        public SkypeForBusinessResponseGroupInfo()
        {
            this.SkypeForBusinessUserInfo = new List<SkypeForBusinessUserInfo>();
        }
        public string OwnerPool { get; set; }
        public string AgentsByUri { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ParticipationPolicy { get; set; }
        public string AgentAlertTime { get; set; }
        public string RoutingMethod { get; set; }
        public string DistributionGroupAddress { get; set; }
        public string Identity { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public List<SkypeForBusinessUserInfo> SkypeForBusinessUserInfo { get; set; }

        public EndPoint EndPoint { get; set; }

        public string EntitlementName
        {
            get
            {
                return Name;
            }
        }

        public string EntitlementDesc
        {
            get
            {
                return Name;
            }
        }

        public EntitlementType EntitlementType
        {
            get
            {
                return EntitlementType.ResponseGroup;
            }
        }
    }
}
