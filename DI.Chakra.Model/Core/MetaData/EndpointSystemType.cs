using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace DI.Chakra.Model
{
    [Serializable]

    [Table("DI_M_EndpointSystemType")]
    public class EndpointSystemType
    {
        [Key]
        public EndpointSystemType Id { get; set; }
        public string Description { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}
