using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_SODPolicyConfiguration")]
    public class SODPolicyConfiguration: EntityBase, ICreatable, IUpdatable
    {
        public long RiskLevelId { get; set; }

        public string Operator { get; set; }

        public long RiskScore { get; set; }

        public string RiskScoreOperator { get; set; }
     
        [ForeignKey("RiskLevelId")]
        public SODRiskLevel SODRiskLevel { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
