using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    public class PresentationResponseGroupMember
    {
        public string ResponseGroupName { get; set; }
        public string MemberPrincipalName { get; set; }
        public string ADID { get; set; }
        public Nullable<bool> IsMasked { get; set; }
        public long EndpointSystemId { get; set; }
        public long UserId { get; set; }
    }
    public class PresentationResponseGroupUserMember
    {
        public string UserPrincipalName { get; set; }
        public string LastName { get; set; }
        public string LineURI { get; set; }

    }
    public class PresentationSkypeUserInfo
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string AccountName { get; set; }
        public string LineURI { get; set; }
        public string UserTelephoneNumber { get; set; }
        public EmployeeType EmployeeType { get; set; }
    }
}
