using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_NRuleset")]
    public class NRuleset : EntityBase, ICreatable, IUpdatable
    {
        public long RuleId { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }
        public long TypeId { get; set; }

        public string Value { get; set; }

        [NotMapped]
        public string ValueDescription { get; set; }

        public string Operator { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [ForeignKey("TypeId")]
        public NRulesetType RuleSetType { get; set; }
    }
}
