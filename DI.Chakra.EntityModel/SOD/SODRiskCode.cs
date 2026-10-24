using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{

    [Table("DI_M_SODRiskCode")]
    public class SODRiskCode : EntityBase,ICreatable,IUpdatable
    {
        public string RiskCode { get; set; }

        public string RiskDescription { get; set; }

        public Nullable<long> RiskLevelId { get; set; }

        public int Status { get; set; }

        public Nullable<long> UserId { get; set; }

        public Nullable<long> RiskScoreId { get; set; }

        [ForeignKey("RiskLevelId")]
        public SODRiskLevel sodrisklevel { get; set; }

        [ForeignKey("RiskScoreId")]
        public SODRiskScore sodriskscore { get; set; }

        [ForeignKey("UserId")]
        public User user { get; set; }

        public List<SODMitigation> Mitigations { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
