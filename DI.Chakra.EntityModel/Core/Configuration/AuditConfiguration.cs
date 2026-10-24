using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_AuditConfiguration")]
    public class AuditConfiguration : EntityBase, ICreatable, IUpdatable
    {
        public string TableName { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        [DataMember]
        public List<AuditConfigurationDetail> AuditConfigurationDetails { get; set; }
    }
}
