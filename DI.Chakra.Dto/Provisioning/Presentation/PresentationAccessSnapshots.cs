using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    [Serializable]
    public class PresentationAccessSnapshots
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string EmailID { get; set; }
        public string EndpointsystemName { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public string Entitlement { get; set; }
        public string EntitlementType { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string ProvisionedThrough { get; set; }
        public bool IsResolved { get; set; }
        public string ResolvedBy { get; set; }
        public string ResolvedStatus { get; set; }
        public DateTime? LastLogonDate { get; set; }
        public DateTime? ValidFromDate { get; set; }
        public DateTime? ValidToDate { get; set; }
        public long? AccessRequestId { get; set; }
    }
}
