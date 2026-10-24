using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class AttestationsByGroupAndRoleLevelReportData
    {
        public string Userr { get; set; }
        public string EndpointUserName { get; set; }
        public string DisplayName { get; set; }
        public string HostName { get; set; }
        public string EndpointSystemTypeName { get; set; }
        public EntitlementType AccessType { get; set; }
        public string AccessName { get; set; } // GroupName
        public string SubmittedBy { get; set; }
        public Nullable<DateTime> SubmittedDate { get; set; }
        public string RequestedAction { get; set; }
        public string Comments { get; set; }
        public long UserId { get; set; }
        public long EndpointSystemId { get; set; }
        public string EndpointSystemName { get; set; }
        public long EndpointSystemTypeId { get; set; }
        public long WorkflowConfigurationId { get; set; }
        public int Level { get; set; }
    }
}
