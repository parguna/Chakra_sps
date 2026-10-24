using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_NOrganisation")]
    public class NOrganisation : EntityBase,ICreatable, IUpdatable
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ParentOrgCode { get; set; }

        public Nullable<long> ParentOrgId { get; set; }

        public Nullable<long> ScopeId { get; set; }
        
        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
    
        [ForeignKey("ScopeId")]
        public virtual NOrganisationScope OrganisationScope { get; set; }
    }
}
