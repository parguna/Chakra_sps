using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_AttributeMapping")]
    public class AttributeMappingColumns : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public string DiColumn { get; set; }
        public string Tgcolumn { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public Nullable<long> EndpointSystemId { get; set; }
        public Nullable<long> EndpointSystemTypeId { get; set; }

        [ForeignKey("EndpointSystemId")]
        public virtual EndPoint EndPoint { get; set; }

        [ForeignKey("EndpointSystemTypeId")] 
        public virtual EndPointSystemType EndPointSystemType { get; set; }
    }
}
