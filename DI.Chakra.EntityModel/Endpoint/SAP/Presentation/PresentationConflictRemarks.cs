using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationConflictRemarks
    {
        public string EndpointName { get; set; }
        public string RequestedRole { get; set; }
        public string ConflictedRole { get; set; }
        public string ConflictRemarks { get; set; }
        public string Source { get; set; }
    }
}
