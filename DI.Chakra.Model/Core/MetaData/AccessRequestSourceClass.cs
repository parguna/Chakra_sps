using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_AccessRequestSource")]

    public class AccessRequestSourceClass
    {
        [Key]
        public AccessRequestSource Id { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } 
    }
}
