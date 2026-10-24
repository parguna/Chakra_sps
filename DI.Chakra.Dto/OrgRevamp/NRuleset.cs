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
    public class NRuleset : DtoBase
    {
        public long RuleId { get; set; }


        public string Description { get; set; }
        public string Code { get; set; }

        public long TypeId { get; set; }

        public string Value { get; set; }
        public string ValueDescription { get; set; }
        public string Operator { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }       

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public NRulesetType RuleSetType { get; set; }

        public List<NRuleset> SelectedRuleSets { get; set; }
    }
}
