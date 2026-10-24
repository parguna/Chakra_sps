using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_SODRiskScore")]
    public class SODRiskScore : EntityBase,ICreatable,IUpdatable
    {
        public int RiskScore { get; set; }

        public string RiskScoreDescription { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
