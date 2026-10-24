using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class NProvisioningField : DtoBase
    {
        public long EndPointSystemTypeId { get; set; }
        public long EndPointSystemId { get; set; }

        public AccountType AccountType { get; set; }

        public string FieldName { get; set; }

        public string Description { get; set; }

        public string Value { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public EndPoint EndPoint { get; set; }
        public EndPointSystemType EndpointSystemType { get; set; }

        public string ValueDisplayName { get; set; }
    }
}
