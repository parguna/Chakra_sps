using DI.Chakra.Infrastructure.Enums;
using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.ComponentModel;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class AuditConfiguration : DtoBase
    {
        public string TableName { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public List<AuditConfigurationDetail> AuditConfigurationDetails { get; set; }
    }
}
