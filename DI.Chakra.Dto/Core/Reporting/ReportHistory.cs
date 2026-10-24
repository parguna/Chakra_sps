using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class ReportHistory: DtoBase
    {
        public long ReportId { get; set; }
        public Report ReportSchedule { get; set; }
        public byte[] Report { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public ReportStatus Status { get; set; }
        public string CreatedBy { get; set; }

        public string LastModifiedBy { get; set; }

        
    }
}
