using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{

    public class SODPolicy : DtoBase
    {
        public string SODPolicyName { get; set; }

        public string SODPolicyDescription { get; set; }

        public long PolicyTypeId { get; set; }

        public int BindType { get; set; }

        public  ComplianceType policytype { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}
