using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_ProvisioningConfiguration")]
    public class ProvisioningConfiguration : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }

       // public UserNameSource UserNameSource { get; set; }

        public int UserNameGeneratorId { get; set; }

    }
}
