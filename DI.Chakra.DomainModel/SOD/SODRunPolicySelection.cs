using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SODRunPolicySelection : DomainModelBase<SODRunPolicySelection, EntityModel.SODRunPolicySelection>
    {
        public int SODRunPolicyId { get; set; }

        public string EndpointTypeName { get; set; }

        public string LevelName { get; set; }

        public string LevelValue { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}
