using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    [Serializable]
    public class AccountDetail
    {
        public string LoginName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; } //Role , Group, Profile
        public AccountType AccountType { get; set; }
        public bool IsMappedAccount { get; set; }
        public Nullable<DateTime> LastLogonDate { get; set; }
        public string Version { get; set; }
        public long? DIUserID { get; set; }
        public string UnMappedMapTo { get; set; }
        public bool IsPrimaryAccount { get; set; }
        public long EndpointUserId { get; set; }
        public long EndpointSystemId { get; set; }

        public string ApplicationType { get; set; }
        public string Application { get; set; }

    }
}
