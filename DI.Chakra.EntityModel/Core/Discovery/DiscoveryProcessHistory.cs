using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_DiscoveryProcessHistory")]
    public class DiscoveryProcessHistory : EntityBase, ICreatable, IUpdatable
    {
        public int ProcessId { get; set; }

        public int HistoryId { get; set; }

        public long EndpointSystemId { get; set; }

        public DateTime? ProcessStartAt { get; set; }

        public DateTime? ProcessEndAt { get; set; }

        public string Status { get; set; }

        public string Message { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string RecordStatus { get; set; }
    }
}
