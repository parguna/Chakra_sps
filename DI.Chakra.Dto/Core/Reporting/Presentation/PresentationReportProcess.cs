using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationReportProcess:Report
    {
        public long ProcessId { get; set; }
        public Occurence occurence { get; set; }
        public string Status { get; set; }
        public DateTime? StartDateTime { get; set; }
        public string ErrorMessage { get; set; }
    }
}
