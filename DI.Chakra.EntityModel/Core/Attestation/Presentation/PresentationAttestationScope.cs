using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationAttestationScope : EntityBase
    {
        public long Id { get; set; } 
        public long AttestationId { get; set; }
        public long OrganisationScopeId { get; set; }
        public string OrganisationScopeName { get; set; } 
        public string AttestationScopeName { get; set; }
        public Nullable<long> AttestationScopeValue { get; set; }
        public string AttestationScopeValueDesc { get; set; } 
    }
}
