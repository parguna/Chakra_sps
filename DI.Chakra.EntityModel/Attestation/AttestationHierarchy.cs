using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_AttestationHierarchy")]
    public class AttestationHierarchy : EntityBase, ICreatable, IUpdatable
    {
        public Nullable<long> AttestationId { get; set; }
        public Nullable<long> EndpointHierarchyId { get; set; }
        public long EndpointId { get; set; }
        public bool IsActive { get; set; }
        public bool IsVisible { get; set; }
        public bool IsActionAllowed { get; set; }
        public AttestationDefaultReviewStatus DefaultAttestationAction { get; set; }
        public bool IsActionExtendable { get; set; }
        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [ForeignKey("EndpointHierarchyId")]
        public EndpointHierarchy EndpointHierarchy { get; set; }
    }
}
