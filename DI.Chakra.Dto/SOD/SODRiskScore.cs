using System;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class SODRiskScore : DtoBase
    {
        public int RiskScore { get; set; }

        public string RiskScoreDescription { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
