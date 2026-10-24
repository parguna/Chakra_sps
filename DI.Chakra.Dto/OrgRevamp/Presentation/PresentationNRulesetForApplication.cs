using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public  class PresentationNRulesetForApplication : NRulesetForApplication
    {
        public string RuleCode { get; set; }
        public string EndpointSystemName { get; set; }
    }
}
