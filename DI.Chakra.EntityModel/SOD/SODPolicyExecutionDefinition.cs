using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_SODPolicyExceutionDefinition")]
    public class SODPolicyExecutionDefinition : EntityBase,ICreatable,IUpdatable
    {
        public Nullable<long> CompliancePolicyId { get; set; }

        public string UniqueRunCode { get; set; }

        public SODOutputType OutputType { get; set; }

        public Nullable<System.DateTime> RunDate { get; set; }

        public Nullable<bool> RunAtObjectLevel { get; set; }

        [ForeignKey("CompliancePolicyId")]
        public CompliancePolicy CompliancePolicy { get; set; }

        public List<SODEndPointPolicySelection> EndpointSelection { get; set; }

        public List<SODException> Exceptions { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
