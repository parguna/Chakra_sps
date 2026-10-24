using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel;

namespace DI.Chakra.Dto
{
    public class AuditConfigurationDetail : DtoBase
    {
        public long? TableId { get; set; }

        public string PropertyName { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

    }
}
