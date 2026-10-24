using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationRequestActionUsers
    {
        public long? TaskId { get; set; }
        public long? RequestId { get; set; }
        public string ActionUser { get; set; }
        public long? ActionUserId { get; set; }

    }
}
