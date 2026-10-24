using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class AttributeMappingColumns : DtoBase
    {
        public string DiColumn { get; set; }

        public string Tgcolumn { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreateDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public Nullable<long> EndpointSystemId { get; set; }

        public Nullable<long> EndpointSystemTypeId { get; set; }

        public EndPoint EndPoint { get; set; }

        public EndPointSystemType EndPointSystemType { get; set; }
    }
}
