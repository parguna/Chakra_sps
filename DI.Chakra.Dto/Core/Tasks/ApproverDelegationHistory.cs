using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class ApproverDelegationHistory : DtoBase
    {
        public long AccessRequestId { get; set; }

        public string ApproverName { get; set; }

        public string Level { get; set; }

        public string Remarks { get; set; }

        public string ApproverRole { get; set; }

        public DateTime DelegatedDateTime { get; set; }
        public bool IsActionOnlyRequired { get; set; }
        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public string LastModifiedBy { get; set; }
    }
}
