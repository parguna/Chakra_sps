using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DI.Chakra.Dto.Presentation
{
    public class PresentationADSUserToGroups
    {
        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public Nullable<DateTime> AccountCreatedDate { get; set; }

        public Nullable<DateTime> PasswordExpirationDate { get; set; }

        public Nullable<DateTime> LastLoginDate { get; set; }

        public string Disabled { get; set; }

        public string Locked { get; set; }

        public string Email { get; set; }

        public string Department { get; set; }

        public string GroupName { get; set; }

        public string GroupDescription { get; set; }

        public string GroupOU { get; set; }

        public string UserOU { get; set; }
        public string TotalUsage { get; set; }
        public string UsageInPercentage { get; set; }
    }
}
