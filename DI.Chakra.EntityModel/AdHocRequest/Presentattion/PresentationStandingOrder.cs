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
   public class PresentationStandingOrder : EntityBase
    {

        public long? EndpointSystemId { get; set; }

        public string SubSystemName { get; set; }

        public string Description { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string AdditionalInfo { get; set; }
        public byte[] AttachmentFile { get; set; }

    }
}
