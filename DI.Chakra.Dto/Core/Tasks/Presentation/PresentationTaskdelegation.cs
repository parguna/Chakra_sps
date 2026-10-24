using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationTaskdelegation : TaskDelegation
    {
        public long DelegationId { get; set; }

        public string ActionUserName { get; set; }

        public long DelegatedToUserId { get; set; }

        public string DelegatedToUserName { get; set; }

        public bool IsAdvance { get; set; }

        public string DelegateProvisioningTaskString
        {
            get
            {
                return DelegateProvisioningTask ? "Yes" : "No";
            }
        }
        public string DelegateAttestationTaskString
        {
            get
            {
                return DelegateAttestationTask ? "Yes" : "No";
            }
        }
        public string IsActiveString
        {
            get
            {
                return IsActive ? "Yes" : "No";
            }
        }
    }
}
