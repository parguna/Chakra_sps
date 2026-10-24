using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    [NotMapped]
    public class PresentationRequestActionUsers
    {
        public long? TaskId { get; set; }
        public long? RequestId { get; set; }
        public string ActionUser { get; set; }
        public long? ActionUserId { get; set; }

    }
}
