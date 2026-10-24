using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
   [NotMapped]
   public  class PresentationNRulesetForApplication : NRulesetForApplication
    {
        public string RuleCode { get; set; }

        public string EndpointSystemName { get; set; }
        
    }
}
