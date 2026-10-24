using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_NOVA_SoDExceptionsMatrix")]
    public class NOVASoDExceptionsMatrix: EntityBase, ICreatable, IUpdatable
    {
        public long EndPointSystemId { get; set; }
        public string Exceptions { get; set; }
        public string AffectedUsers { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }
}
