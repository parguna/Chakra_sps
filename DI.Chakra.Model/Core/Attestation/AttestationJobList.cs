using System;
using System.Collections.Generic;
using DI.Chakra.Infrastructure.Enums;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.List
{
    public class AttestationJobList
    {
        public long AttestationProcessId { get; set; }
        public string ProcessName { get; set; }
        public string Description { get; set; }
        public Occurence occurence { get; set; }
        public string AttestationName { get; set; }
        public string Status { get; set; }
        public string StartDateTime { get; set; }
    }
}
