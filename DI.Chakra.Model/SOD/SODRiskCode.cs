using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_SODRiskCode")]
    public class SODRiskCode
    {
        [Key]
        public long Id { get; set; }
        public string RiskCode { get; set; }
        public string RiskDescription { get; set; }
        public Nullable<long> RiskLevelId { get; set; }
        public int Status { get; set; }
        public Nullable<long> UserId { get; set; }
        public Nullable<long> RiskScoreId { get; set; }
        [ForeignKey("RiskLevelId")]
        public virtual SODRiskLevel sodrisklevel { get; set; }
        [ForeignKey("RiskScoreId")]
        public virtual SODRiskScore sodriskscore { get; set; }
        [ForeignKey("UserId")]
        public virtual User user { get; set; }

        public virtual List<SODMitigation> Mitigations { get; set; }

        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
