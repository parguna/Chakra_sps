using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationAdHocRequest:AdHocAccessRequest
    {
        public AdHocAccessRequest AdHocAccessRequests { get; set; }
    }
}
