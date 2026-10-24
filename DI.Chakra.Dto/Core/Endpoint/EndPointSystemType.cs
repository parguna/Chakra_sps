using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class EndPointSystemType : DtoBase
    {
        public EndpointType EndpointSystemTypeName { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public Nullable<bool> IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public string ObjectName { get; set; }

        public bool CanBeTrustedSource { get; set; }

        public  List<EndPoint> EndPoints { get; set; }

        public  List<EndpointObjects> EndpointObjects { get; set; }
    }
}
