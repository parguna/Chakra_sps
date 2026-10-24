using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
     [Table("DI_M_EndpointSystemType")]
    public class EndPointSystemType : EntityBase,ICreatable,IUpdatable
    {
        public EndpointType EndpointSystemTypeName { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public Nullable<bool> IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public string ObjectName { get; set; }

        public bool CanBeTrustedSource { get; set; }

        public  IList<EndPoint> EndPoints { get; set; }

        public  IList<EndpointObjects> EndpointObjects { get; set; }
    }
}
