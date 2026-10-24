using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel.Presentation
{
    [NotMapped]
    public class PresentationDiscoveryProcess : DiscoveryProcess
    {
        public long DiscoveryProcessId { get; set; }
        public Occurence occurence { get; set; }
        public string EndPointSystem { get; set; }
        public string Status { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string ErrorMessage { get; set; }

        public string SchedulerDateTime { get; set; }

        public DateTime? LastRunDate { get; set; }

    }
}
