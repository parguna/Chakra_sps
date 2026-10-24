using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{

    [Table("DI_M_SODMitigation")]
    public class SODMitigation : EntityBase,ICreatable,IUpdatable
    {
        public string MitigationCode { get; set; }

        public string MitigationDescription { get; set; }

        public string MitigationSteps { get; set; }

        public Nullable<long> ApproverUserId { get; set; }

        public Nullable<long> ApproverGroupId { get; set; }

        public Nullable<long> MonitorUserId { get; set; }

        public Nullable<long> MonitorGroupId { get; set; }

        public Nullable<bool> IsActive { get; set; }

        public string DocumentName { get; set; }

        public string DocumentUploadedPath { get; set; }

        public Nullable<DateTime> DocumentUploadedDate { get; set; }
        
        public List<SODRiskCode> RiskCodes { get; set; }

        [ForeignKey("ApproverUserId")]
        public User Approver { get; set; }

        [ForeignKey("MonitorUserId")]
        public User Reveiwer { get; set; }


        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }


    }
}
