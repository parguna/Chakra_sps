using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_ProvisioningNameConstructor")]
    public class ProvisioningNameConstructor : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }

        public BusinessLogicSource BusinessLogicSource { get; set; }

        public int BusinessLogicId { get; set; }

    }
}
