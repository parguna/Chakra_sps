using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class PresentationSODPolicyExecutionDefinition
    {
        public string PolicyCode { get; set; }
        public string PolicyDescription { get; set; }
        public string UniqueRunCode { get; set; }
        public SODOutputType OutputType { get; set; }
        public bool RunAtObjectLevel { get; set; }
    }
}
