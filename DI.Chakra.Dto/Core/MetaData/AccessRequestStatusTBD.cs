using DI.Chakra.Infrastructure.Enums;
using System;

using System.Xml.Serialization;

namespace DI.Chakra.Dto
{
    public class AccessRequestStatusTBD : DtoBase  
    {
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}
