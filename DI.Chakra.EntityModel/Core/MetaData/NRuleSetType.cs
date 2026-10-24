using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_NRulesetType")]
    public class NRulesetType : EntityBase, ICreatable, IUpdatable
    {
        public string RulesetType { get; set; }

        public string Description { get; set; }

        public long Score { get; set; }

        public bool IsActive { get; set; }        

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
