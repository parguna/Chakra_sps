using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    [Serializable]
    public class PresentationUserAccount
    {
        public long AcountReconcilationId { get; set; }

        public long EndpointUserId { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }

        public Nullable<DateTime> LastLogonDate { get; set; }

        public AccountType AccountType { get; set; }

        public EndpointType EndPointSystemType { get; set; }

        public string EndpointSystemName { get; set; }

        public long EndpointSystemId { get; set; }

        public string Reserved1 { get; set; }

        public string Reserved2 { get; set; }

        public string Reserved3 { get; set; }

        public string AccountStatus { get; set; }
        public bool AccountLocked { get; set; }
        public string AccountLockedstring {
            get
            {
                return this.AccountLocked ? "Yes" : "No";
            }
        }
        public bool IsAllowPasswordReset { get; set; }

        public bool IsAllowAccountUnlock { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public bool AllowReset { get; set; }
    }
}
