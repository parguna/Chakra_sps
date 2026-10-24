using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public class AdhocSkypeForBusinessRequest : IEntitlementDetail
    {
        public string DisplayAccountName { get; set; }
        public AdhocSkypeForBusinessRequest()
        {
        }
        public long EndpointUserId { get; set; }
        public List<ResponseGroup> SkypeForBusinessResponseGroups { get; set; } = new List<ResponseGroup>();
        public EntitlementType EntitlementType { get; set; }
        public string Extension { get; set; }
        public string DisplayCurrentLineURI { get; set; }
        public string LineUri { get; set; }
        public string VoicePolicy { get; set; }
        public string VoiceRoutingPolicy { get; set; }
        public string DialPlan { get; set; }
        public string RegistrarPoolName { get; set; }
        public Nullable<bool> MoveToOnline { get; set; } = null;
        public EntitlementType UserType { get; set; } = EntitlementType.None;

        [Obsolete]
        public ISharedAccountDetail SharedAccountDetail
        {
            get;
        }
        public string Entitlement
        {
            get
            {

                string strItems = "";
                if (EntitlementType == EntitlementType.ResponseGroup)
                {
                    if (SkypeForBusinessResponseGroups != null)
                    {
                        strItems = string.Join(",", SkypeForBusinessResponseGroups.Select(i => i.Name));
                    }
                    return strItems;
                }
                else
                {
                    strItems = EntitlementType.ToString();
                }
                return strItems;
            }
        }
    }
    [Serializable]
    public class ResponseGroup
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Identity { get; set; }
        public bool Mask { get; set; }
        public string ADID { get; set; }
    }
}
