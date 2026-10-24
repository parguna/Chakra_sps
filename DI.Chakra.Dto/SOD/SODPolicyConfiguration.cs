using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
   public  class SODPolicyConfiguration:DtoBase
    {
       
        public long RiskLevelId { get; set; }

        public string Operator { get; set; }

        public long RiskScore { get; set; }

        public string RiskScoreOperator { get; set; }
      
        public SODRiskLevel SODRiskLevel { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
