using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_NRulesetForApplication")]
    public class NRulesetForApplication : EntityBase, ICreatable, IUpdatable
    {
        public long RuleId { get; set; }

        public long EndpointSystemId { get; set; }

        public string AccountType { get; set; }


        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [ForeignKey("EndpointSystemId")]
        public EndPoint EndPoint { get; set; }
    }
}
