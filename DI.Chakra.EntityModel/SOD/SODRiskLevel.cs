using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{

    [Table("DI_M_SODRiskLevel")]
    public class SODRiskLevel : EntityBase,ICreatable,IUpdatable
    {
        public string RiskLevelDescription { get; set; }

        public string RiskLevelCode { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
