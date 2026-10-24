using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationAdHocRequest
    {
        public AdHocAccessRequest adHocAccessRequests { get; set; }

        public AccessRequest accessRequests { get; set; }
    }
}
