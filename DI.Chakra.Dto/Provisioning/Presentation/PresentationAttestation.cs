using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationAttestation
    {
        public long? Id { get; set; }
        public string AttestationName { get; set; }

        public string AttestationDescription { get; set; }

        public bool RequiresAttestationRemediation { get; set; }

        public string WorkFlow { get; set; }

        public DateTime? CreatedDate { get; set; }

    }
}
