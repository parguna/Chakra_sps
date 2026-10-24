using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class AttestationScope : DtoBase
    {
        public long Id { get; set; }
        public long AttestationId { get; set; }

        public Nullable<long> OrganisationScopeId { get; set; }

        public string AttestationScopeName { get; set; }

        public Nullable<long> AttestationScopeValue { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public Attestation Attestation { get; set; }

        public NOrganisation NOrganisation { get; set; }

        public long AttestationScopeValueId { get; set; }

        public string AttestationScopeValueDesc { get; set; } 
    }
}
