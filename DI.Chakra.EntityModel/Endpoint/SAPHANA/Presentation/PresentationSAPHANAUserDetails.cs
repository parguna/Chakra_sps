using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationSAPHANAUserDetails
    {
        public long Id { get; set; }
        public long EndpointSystemId { get; set; }
        public string UserLogonName { get; set; }
    }
}
