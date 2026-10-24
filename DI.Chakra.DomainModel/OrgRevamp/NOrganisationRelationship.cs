using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public partial class NOrganisationRelationship : DomainModelBase<NOrganisationRelationship, EntityModel.NOrganisationRelationship>
    {
        public long OrganisationId { get; set; }

        public long ScopeId { get; set; }

        public string Value { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public NOrganisation Organisation { get; set; }

        public NOrganisationScope OrganisationScope { get; set; }
    }
}
