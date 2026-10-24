using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DI.Chakra.Model 
{
    
    [Serializable]
    [Table("DI_M_EndpointSystemType")]
    public partial class EndPointSystemType: IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public EndpointType EndpointSystemTypeName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public string ObjectName { get; set; }
        public virtual IList<EndPoint> EndPoints { get; set; }
        public virtual IList<EndpointObjects> EndpointObjects { get; set; }
    }
}
