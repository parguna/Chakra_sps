using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_EndpointHierarchy")]
    public class EndpointHierarchy : EntityBase, ICreatable, IUpdatable
    {        
        public EndpointType EndpointType { get; set; }

        public Nullable<long> EndpointId { get; set; }

        public EntitlementType EntitlementType { get; set; }

        public long EndpointObjectId { get; set; }

        public bool IsActionable { get; set; }

        public bool IsAssociatedWithUser { get; set; }

        public Nullable<long> ParentId { get; set; }

        public long Level { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        
    }
}
