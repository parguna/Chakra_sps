using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Serializable]

    [Table("DI_M_OUChangeSnapshot")]
    public class OUChangeSnapshot
    {
        [Key]
        public long Id { get; set; }
        public string BatchId { get; set; }
        public long UserId { get; set; }
        public long EndpointSystemId { get; set; }
        public long EndpointUserId { get; set; }
        public EntitlementType EntitlementType { get; set; }
        public string Entitlement { get; set; }
        public DateTime EnteredDate { get; set; }
    }
}
