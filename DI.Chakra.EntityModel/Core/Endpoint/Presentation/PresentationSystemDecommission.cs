using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{

   public class PresentationSystemDecommission
    {
        public long Id { get; set; }

        public long EndpointSystemId { get; set; }

        public DateTime DecommissionedDate;

        public string DecommissionedBy { get; set; }

        public string Comments { get; set; }

        public bool Status { get; set; }

    }
}
