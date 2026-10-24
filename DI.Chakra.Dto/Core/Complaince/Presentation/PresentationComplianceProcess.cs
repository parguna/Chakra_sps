using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationComplianceProcess : ComplianceProcess
    {
        public long ComplianceProcessId { get; set; }
        public Occurence occurence { get; set; }
        public string Status { get; set; }
        public string StartDateTime { get; set; }
        public string PolicyName { get; set; }

        public string SchedulerDateTime { get; set; }
    }
}
