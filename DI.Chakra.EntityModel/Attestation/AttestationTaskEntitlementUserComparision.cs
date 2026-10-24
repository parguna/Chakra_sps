using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("vwAttestationTaskEntitlementUserComparision")]
    public class AttestationTaskEntitlementUserComparision : EntityBase
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
    [NotMapped]
    public class AttestationCampaignEntitlementUserComparision
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
    public class PresentationUserEntitlementData
    {
        public string EmployeeName { get; set; }
        public string Entitlement { get; set; }
        public string EntitlementDescription { get; set; }
        public string SystemName { get; set; }
        public long EndpointId { get; set; }
        public long? UserId { get; set; }
        public string EmployeeId { get; set; }
        public bool Approved { get; set; }
        public bool Revoked { get; set; }
        public string UserName { get; set; }
        public bool IsActionable { get; set; }
        public EntitlementType EntitlementType { get; set; }
    }

    [NotMapped]
    public class FilterData
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string EndpointName { get; set; }
        /// <summary>
        /// Possible value: EmployeeId
        /// </summary>
        public string AdditionalInfo { get; set; }
        public string UserName { get; set; }
        public long EndpointId { get; set; }
        public bool AccountStatus { get; set; }
        public bool IsActionable { get; set; }
        public string EntitlementType { get; set; }
    }
    [NotMapped]
    public class ViewUserEntitlementData
    {
        public long rowID { get; set; }
        public string Entitlement { get; set; }
        public string EntitlementDescription { get; set; }
        public string EntitlementType { get; set; }
        public string SystemName { get; set; }
        public long EndpointId { get; set; }
        public Dictionary<string, KeyValueData> EmployeeNames { get; set; }
    }
    [NotMapped]
    public class KeyValueData
    {
        public string EmployeeName { get; set; }
        public string EmployeeNo { get; set; }
        public long AccountId { get; set; }
        public bool Exists { get; set; }
        public bool Approve { get; set; }
        public bool Revoke { get; set; }
        public string UserName { get; set; }
        public bool IsActionable { get; set; }
    }
}
