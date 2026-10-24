using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_AttributeMapping")]
    public class AttributeMappingColumns : EntityBase, IUpdatable,ICreatable
    {
        public string DiColumn { get; set; }

        public string Tgcolumn { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public Nullable<long> EndpointSystemId { get; set; }

        public Nullable<long> EndpointSystemTypeId { get; set; }

        [ForeignKey("EndpointSystemId")]
        public EndPoint EndPoint { get; set; }

        [ForeignKey("EndpointSystemTypeId")]
        public EndPointSystemType EndPointSystemType { get; set; }
    }
}
