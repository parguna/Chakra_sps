using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{

    public class PresentationExceptionReconData
    {
        public string EndpointSystemType { get; set; }

        public string EndpointSystemName { get; set; }

        public string AccountName { get; set; }

        public string DisplayName { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string ExceptionType { get; set; }

        public long? UserId { get; set; }

        public int? Version { get; set; }

        public long EndpointSystemId { get; set; }

        public long EndpointUserId { get; set; }
    }
}
