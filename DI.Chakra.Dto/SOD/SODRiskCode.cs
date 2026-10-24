using System;
using System.Collections.Generic;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class SODRiskCode : DtoBase
    {
        public string RiskCode { get; set; }

        public string RiskDescription { get; set; }

        public Nullable<long> RiskLevelId { get; set; }

        public int Status { get; set; }

        public Nullable<long> UserId { get; set; }

        public Nullable<long> RiskScoreId { get; set; }

        public SODRiskLevel sodrisklevel { get; set; }

        public SODRiskScore sodriskscore { get; set; }

        public User user { get; set; }

        public List<SODMitigation> Mitigations { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
