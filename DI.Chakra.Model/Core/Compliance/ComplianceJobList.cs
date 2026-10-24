using DI.Chakra.Model.SOD;
using System;
using System.Collections.Generic;
using DI.Chakra.Infrastructure.Enums;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class ComplianceJobList
    {
        public long ComplianceProcessId { get; set; }
        public string ProcessName { get; set; }
        public string Description { get; set; }
        public Occurence occurence { get; set; }
        public string Status { get; set; }
        public string StartDateTime { get; set; }
        public string PolicyName { get; set; }
    }
}
