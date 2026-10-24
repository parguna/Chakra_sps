using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
 public class PresentationStandingOrder
    {
        public long Id { get; set; }
        public long? EndpointSystemId { get; set; }

        public string SubSystemName { get; set; }

        public string SONumber { get; set; }

        public bool HasAttachments { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public string Comments { get; set; }
 
        public System.DateTime CreatedDate { get; set; }

        public byte[] AttachmentFile { get; set; }


    }
}
