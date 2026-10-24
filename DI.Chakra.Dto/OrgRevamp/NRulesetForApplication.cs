using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Dto;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class NRulesetForApplication : DtoBase
    {
        public long RuleId { get; set; }

        public long EndpointSystemId { get; set; }

        public string AccountType { get; set; }


        public string CreatedBy { get; set; }

        public DateTime  CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public NRuleset Ruleset { get; set; }

        public EndPoint EndPoint { get; set; }
    }
}
