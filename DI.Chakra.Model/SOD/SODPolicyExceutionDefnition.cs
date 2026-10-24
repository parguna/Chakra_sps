using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.SOD
{
    [Serializable]
    [Table("DI_M_SODPolicyExceutionDefinition")]
    public class SODPolicyExceutionDefinition
    {
        [Key]
        public long Id { get; set; }

        public Nullable<long> CompliancePolicyId { get; set; }
        public string UniqueRunCode { get; set; }

        public SODOutputType OutputType { get; set; }

        public Nullable<System.DateTime> RunDate { get; set; }
        public Nullable<bool> RunAtObjectLevel { get; set; }

        [ForeignKey("CompliancePolicyId")]
        public virtual CompliancePolicy CompliancePolicy { get; set; }

        public virtual List<SODEndPointPolicySelection> EndpointSelection { get; set; }

        public virtual List<SODException> Exceptions { get; set; }

        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
