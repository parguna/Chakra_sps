using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    [NotMapped]
    public class PresentationListAttestation : Attestation
    {
        public long AttestationId { get; set; }

        public string Status { get; set; }

        public Nullable<System.DateTime> RunDate { get; set; }

        public string AttesationCreatedBy { get; set; }

        public int ResponseTimeInDays { get; set; }

        public Occurence occurence { get; set; }

        public string EndPointSystem { get; set; }

        public string AttestationRemediation { get; set; } 
    }
}
