using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    [NotMapped]
    public class PresentationAttestationReviewPageSAP_Level1
    {
        public string sapLevel1DetailId { get; set; }

        public string sapLevel1Detail { get; set; }

        public string sapLevel1DetailDesc { get; set; }

        public string sapLevel1DetailType { get; set; }
    }
}
