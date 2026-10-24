using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DI.Chakra.EntityModel
{

    [Table("vwAttestationTaskHistoryEntitlementUserComparision")]
    public class AttestationTaskHistoryEntitlementUserComparision : EntityBase
    {
        public long TaskId { get; set; }
        public string Entitlement { get; set; }
        public string EntitlementDescription { get; set; }
        public EntitlementType EntitlementType { get; set; }
        public long SnapshotActionId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string EmployeeNo { get; set; }
        public EmployeeType EmployeeType { get; set; }
        public DateTime? TerminationDate { get; set; }
        public long UserId { get; set; }
        public AccountType AccountType { get; set; }
        public long EndpointSystemId { get; set; }
        public string EndpointSystemName { get; set; }
        public EndpointType EndpointType { get; set; }
        public bool Approve { get; set; }
        public bool Revoke { get; set; }
        public bool IsActionable { get; set; }
    }
}
