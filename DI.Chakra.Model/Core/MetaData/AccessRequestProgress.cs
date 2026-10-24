using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace DI.Chakra.Model
{
    [Serializable]

    [Table("DI_M_AccessRequestProgress")]
    public class AccessRequestProgressClass  
    {
        [Key]
        public AccessRequestProgress Id { get; set; } 
        public string Description { get; set; } 
        public bool IsActive { get; set; } 
    }
}
