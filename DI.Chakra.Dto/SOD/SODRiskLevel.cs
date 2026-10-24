using System;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class SODRiskLevel : DtoBase
    {
        public string RiskLevelDescription { get; set; }

        public string RiskLevelCode { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
