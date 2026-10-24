using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
   [Serializable]
    public class TaskDelegation : DtoBase
    {

        public long ActionUser { get; set; }

        public long DelegatedTo { get; set; }

        public Nullable<DateTime> StartDate { get; set; }

        public Nullable<DateTime> EndDate { get; set; }

        public bool DelegateAttestationTask { get; set; }

        public bool DelegateProvisioningTask { get; set; }

        public bool DelegateSODTask { get; set; }

        public bool IsActive { get; set; }

        public string DelegationComments { get; set; }
        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public User DelegatedToUser { get; set; }

        public string DelegateToUserName { get;  set; }
        public string DelegatedFromUserName { get; set; }
    }
}
