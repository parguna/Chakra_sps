using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationAttestation : EntityBase
    {
        public string AttestationName { get; set; }

        public string AttestationDescription { get; set; }

        public bool RequiresAttestationRemediation { get; set; }

        public string WorkFlow { get; set; }

        public DateTime? CreatedDate { get; set; }

 
    }
}
