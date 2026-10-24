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
    [Table("DI_M_SODMitigation")]
    public class SODMitigation
    {
        [Key]
        public long Id { get; set; }
        public string MitigationCode { get; set; }
        public string MitigationDescription { get; set; }
        public string MitigationSteps { get; set; }
        public Nullable<long> ApproverUserId { get; set; }
        public Nullable<int> ApproverGroupId { get; set; }
        public Nullable<long> MonitorUserId { get; set; }
        public Nullable<int> MonitorGroupId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string DocumentName { get; set; }
        public string DocumentUploadedPath { get; set; }
        public Nullable<DateTime> DocumentUploadedDate { get; set; }


        public virtual List<SODRiskCode> RiskCodes { get; set; }
        [ForeignKey("ApproverUserId")]
        public virtual User Approver { get; set; }
        [ForeignKey("MonitorUserId")]
        public virtual User Reveiwer { get; set; }
    }
}
