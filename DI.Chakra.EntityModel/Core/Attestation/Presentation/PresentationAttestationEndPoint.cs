using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationAttestationEndPoint : EntityBase
    {
        public string TargetSystem { get; set; }

        public string ApplicationName { get; set; }
    }
}
