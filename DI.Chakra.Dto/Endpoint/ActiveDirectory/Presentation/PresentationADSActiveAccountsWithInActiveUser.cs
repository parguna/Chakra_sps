using System;


namespace DI.Chakra.Dto.Presentation
{

    public class PresentationADSActiveAccountsWithInActiveUser
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public DateTime? TerminatedDate { get; set; }
        public bool UserIsActive { get; set; }
        public string EmailId { get; set; }
        public string OrganisationName { get; set; }

        public long EndPointSystemId { get; set; }

        public string sAMAccountName { get; set; }
        public bool AccountIsActive { get; set; }

        public string displayName { get; set; }

        public string department { get; set; }

        public DateTime? whenCreated { get; set; }

        public DateTime? lastLogon { get; set; }

        public DateTime? pwdLastSet { get; set; }

        public DateTime? accountExpires { get; set; }

        public bool IsAccountDisabled { get; set; }

        public bool IsAccountLocked { get; set; }
    }
}
