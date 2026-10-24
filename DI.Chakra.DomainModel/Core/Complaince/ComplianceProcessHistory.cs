using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class ComplianceProcessHistory:DomainModelBase<ComplianceProcessHistory, EntityModel.ComplianceProcessHistory>
    {

        public int ProcessId { get; set; }

        public int HistoryId { get; set; }

        public DateTime? ProcessStartAt { get; set; }

        public DateTime? ProcessEndAt { get; set; }

        public string Status { get; set; }

        public string Message { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public string RecordStatus { get; set; }

    }
}
