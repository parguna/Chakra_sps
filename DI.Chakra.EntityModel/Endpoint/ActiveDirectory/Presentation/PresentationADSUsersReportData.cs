using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationADSUsersReportData
    {
        public long UserId { get; set; }

        public string sAMAccountName { get; set; }

        public string displayName { get; set; }

        public string department { get; set; }

        public string whenCreated { get; set; }

        public string PasswordExpirationDate { get; set; }

        public string lastLogon { get; set; }

        public string pwdLastSet { get; set; }

        public string accountExpires { get; set; }

        public string logonCount { get; set; }

        public bool IsAccountDisabled { get; set; }

        public bool IsAccountLocked { get; set; }

        public string cn { get; set; }

        public string HostName { get; set; }

        public string Institution { get; set; }

        public string IsMapped { get; set; }

        public string Owner { get; set; }

        public string physicalDeliveryOfficeName { get; set; }

        public string company { get; set; }

        public string pager { get; set; }

        public string businessCategory { get; set; }

        public string division { get; set; }

        public string mail { get; set; }

        public string title { get; set; }

        public string IsLocked { get; set; }

        public LockScenario LockScenario { get; set; }

        public string OU { get; set; }
    }
}
