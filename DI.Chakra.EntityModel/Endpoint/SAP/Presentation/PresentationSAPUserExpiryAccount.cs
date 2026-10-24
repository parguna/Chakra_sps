using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationSAPUserExpiryAccount
    {
        public long UserId { get; set; }
        public string UserLogonName { get; set; }
        public string DisplayName { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Department { get; set; }
        public string Pers_No { get; set; }
        public string Function0 { get; set; }
        public Nullable<DateTime> lastLogon { get; set; }
        public string IsActive { get; set; }
        public string PasswordExpirationDate { get; set; }
        public string E_Mail { get; set; }
        public string Role { get; set; }
        public string HostName { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public string UserType { get; set; }
        public bool IsUserLocked { get; set; }
        public string IsMapped { get; set; }
        public string Owner { get; set; }
        public string costcenter { get; set; }
        public string IsLocked { get; set; }
        public LockScenario LockScenario { get; set; }
    }
}
