using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [NotMapped]
    public class NOrganisationPresentation : EntityBase
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ParentOrgCode { get; set; }

        public Nullable<long> ParentOrgId { get; set; }

        public long ScopeId { get; set; }
        
        public bool IsActive { get; set; }
        
        [ForeignKey("ScopeId")]
        public virtual NOrganisationScope OrganisationScope { get; set; }

        public List<NOrganisationRelationship> OrganisationRelationship { get; set; }
    }
}
