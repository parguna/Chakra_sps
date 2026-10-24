using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class PresentationUserBusinessRole
    {
        public string RoleName { get; set; }
        public string Value { get; set; }
        public List<PresentationSRMPOrg> POrgs { get; set; } = new List<PresentationSRMPOrg>();
    }
    public class PresentationSRMPOrg
    {
        public PresentationSRMPOrg(string OrgName, string Value)
        {
            this.OrgName = OrgName;
            this.Value = Value;
        }
        public string OrgName { get; set; }
        public string Value { get; set; }
        public List<PresentationSRMPGroup> PGroups { get; set; } = new List<PresentationSRMPGroup>();
    }
    public class PresentationSRMPGroup
    {
        public PresentationSRMPGroup(string PGrpName, string Value)
        {
            this.PGrpName = PGrpName;
            this.Value = Value;
        }
        public string PGrpName { get; set; }
        public string Value { get; set; }
    }
}
