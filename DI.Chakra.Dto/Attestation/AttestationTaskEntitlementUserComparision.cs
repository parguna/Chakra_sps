using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class AttestationTaskEntitlementUserComparision : DtoBase
    {
        //As TaskId
        public long TaskId { get; set; }
        public string Entitlement { get; set; }
        public string EntitlementDescription { get; set; }
        public EntitlementType EntitlementType { get; set; }
        public long SnapshotActionId { get; set; }
        public string Name { get; set; }
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

    [DataContract]
    public class ViewUserEntitlementData
    {
        [DataMember]
        public long rowID { get; set; }
        [DataMember]
        public string Entitlement { get; set; }
        [DataMember]
        public string EntitlementDescription { get; set; }
        [DataMember]
        public string EntitlementType { get; set; }
        [DataMember]
        public string SystemName { get; set; }
        [DataMember]
        public long EndpointId { get; set; }
        [DataMember]
        public Dictionary<string, KeyValueData> EmployeeNames { get; set; }
    }
    [DataContract]
    public class KeyValueData
    {
        [DataMember]
        public string EmployeeName { get; set; }
        [DataMember]
        public string EmployeeNo { get; set; }
        [DataMember]
        public long AccountId { get; set; }
        [DataMember]
        public bool Exists { get; set; }
        [DataMember]
        public bool Approve { get; set; }
        [DataMember]
        public bool Revoke { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public bool IsActionable { get; set; }
    }
    [DataContract]
    public class FilterData
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string AdditionalInfo { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string EndpointName { get; set; }
        [DataMember]
        public long EndpointId { get; set; }
        [DataMember]
        public bool AccountStatus { get; set; }
        [DataMember]
        public bool IsActionable { get; set; }
        [DataMember]
        public string EntitlementType { get; set; }
    }
    [DataContract]
    public class TaskFilter
    {
        [DataMember]
        public long TaskId { get; set; }
        [DataMember]
        public int FilterType { get; set; }
        [DataMember]
        public List<long> EpIds { get; set; }
        [DataMember]
        public List<long> UserIds { get; set; }
        [DataMember]
        public AccountType AccountType { get; set; } = AccountType.User;
        [DataMember]
        public string AccountName { get; set; }
        [DataMember]
        public long UserId { get; set; }
    }
    [DataContract]
    public class DataTableSearch
    {
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public bool Regex { get; set; }
        [DataMember]
        public List<long> EndpointIds { get; set; }
        [DataMember]
        public AccountType AccountType { get; set; } = AccountType.None;
    }
    [DataContract]
    public class UserEntitlementComparisionResult
    {
        [DataMember]
        public int draw { get; set; }
        [DataMember]
        public int sEcho { get; set; }
        [DataMember]
        public int recordsTotal { get; set; }

        [DataMember]
        public int recordsFiltered { get; set; }
        [DataMember]
        public int iTotalRecords { get; set; }
        [DataMember]
        public int iTotalDisplayRecords { get; set; }
        [DataMember]
        public List<ViewUserEntitlementData> data { get; set; }
        [DataMember]
        public List<ViewUserEntitlementData> aaData { get; set; }
    }
    [Serializable]
    public class APIAttestationSearch
    {
        public long TaskId { get; set; }
        public int Draw { get; set; }
        public int Start { get; set; } 
        public int Length { get; set; }
        public string Value { get; set; }
        public bool Regex { get; set; }
        public AccountType AccountType { get; set; }
        public List<long> EndpointIds { get; set; }
    }
    [DataContract]
    public class AttestationTaskInformation
    {
        [DataMember]
        public string Approver { get; set; }
        [DataMember]
        public string SubmittedDate { get; set; }
        [DataMember]
        public string RequestedDate { get; set; }

        [DataMember]
        public string AttestationDescription { get; set; }
        [DataMember]
        public string AttestationType { get; set; }
        [DataMember]
        public string AttestationName { get; set; }
        [DataMember]
        public string Level { get; set; }
        [DataMember]
        public string CampaignName { get; set; }

        [DataMember]
        public string Status { get; set; }
    }
    
    public class DTORequestParameter
    {
        [DataMember]
        public long TaskId { get; set; }
        [DataMember]
        public long EndpointId { get; set; }
        [DataMember]
        public long UserId { get; set; }
        [DataMember]
        public string Entitlement { get; set; }
        [DataMember]
        public string UserName { get; set; }
        public AccountType AccountType { get; set; }
        [DataMember]
        public string AccessType { get; set; }
        [DataMember]
        public long ActionUserId { get; set; }
        [DataMember]
        public int Level { get; set; }
        [DataMember]
        public string EntitlementType { get; set; }
    }
}
