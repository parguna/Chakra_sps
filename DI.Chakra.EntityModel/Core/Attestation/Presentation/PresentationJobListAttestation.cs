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
    public class PresentationJobListAttestation 
    {
        public long AttestationProcessId { get; set; }
        public string ProcessName { get; set; }
        public string Description { get; set; }
        public string occurence { get; set; }
        public string AttestationName { get; set; }
        public string Status { get; set; }
        public DateTime? StartDateTime { get; set; }
        public string ErrorMessage { get; set; }

        public string SchedulerDateTime { get; set; }

        public DateTime? LastRunDate { get; set; }
    }
}
