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
    public class PresentationReportProcess:Report
    {

        public long ProcessId { get; set; }
        
        public Occurence occurence { get; set; }
        public string Status { get; set; }
        public string StartDateTime { get; set; }
        public string ErrorMessage { get; set; }
    }
}
