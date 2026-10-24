using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_AuditConfigurationDetail")]
    public class AuditConfigurationDetail : EntityBase, ICreatable, IUpdatable
    {
        public long? TableId { get; set; }

        public string PropertyName { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        [ForeignKey("TableId")]
        public AuditConfiguration AuditLog { get; set; }

    }
}
