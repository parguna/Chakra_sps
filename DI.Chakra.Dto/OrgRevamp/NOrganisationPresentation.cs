
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class NOrganisationPresentation : DtoBase
    {

        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ParentOrgCode { get; set; }

        public Nullable<long> ParentOrgId { get; set; }

        public long ScopeId { get; set; }
        
        public bool IsActive { get; set; }
        
        public virtual NOrganisationScope OrganisationScope { get; set; }

        public List<NOrganisationRelationship> OrganisationRelationship { get; set; }

        
    }
}
