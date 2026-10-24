using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    [NotMapped]
    public class PresentationComplianceProcess : ComplianceProcess
    {
        public long ComplianceProcessId { get; set; }
        public Occurence occurence { get; set; }
        public string Status { get; set; }
        public DateTime? StartDateTime { get; set; }
        public string PolicyName { get; set; }

        public string SchedulerDateTime { get; set; }
    }
}
