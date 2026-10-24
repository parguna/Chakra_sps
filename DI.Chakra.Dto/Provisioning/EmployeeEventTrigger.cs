using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class EmployeeEventTrigger : DtoBase
    {
        
        public long UserId { get; set; }
        public EmployeeEvents EventType { get; set; }
        public DateTime EffectiveDate { get; set; }
        public Nullable<DateTime> OldEffectiveDate { get; set; }
        public bool IsRequestCreated { get; set; }
        //public bool IsExecuted { get; set; }
        public bool IsAdminCheckRequired { get; set; }
        //public bool IsRejoiningUser { get; set; }
        public string UserDetails { get; set; }
        public string OldUserDetails { get; set; }
        public bool IsActive { get; set; }
        public AccessRequestStatus RequestStatus { get; set; }
        public string Comments { get; set; }
        public bool IsMailSent { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }
        public User User { get; set; }
        public Nullable<DateTime> ActualEventDate { get; set; }
        public int EventSequence { get; set; } = 1;
    }

    public class DeptChangedUserGridData
    {
        public long UserId { get; set; }
        public string DisplayName { get; set; }
        public string EmployeeNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string CostCenter { get; set; }
        public string Department { get; set; }
        public string Division { get; set; }
        public string Section { get; set; } 
        public long? Organisation  { get; set; }
        public DateTime EffectiveDate { get; set; } 
        public string ErrorMessage { get; set; }
        public string UserDetail { get; set; }
    }


    [Serializable]
    public class FailedRequestGridData
    {
        public long AccessRequestId { get; set; }
        public string RequestSourceType { get; set; }
        public Nullable<long> AdHocAccessRequestId { get; set; }
        public int UserId { get; set; }
        public string SystemName { get; set; }
        public string DisplayName { get; set; }
        public string Company { get; set; }
        public string CostCenter { get; set; }
        public string Department { get; set; }
        public string Division { get; set; }
        public string Section { get; set; }
        public string Remarks { get; set; }
        public string EmployeeID { get; set; }
        public DateTime RequestDate { get; set; }
        public AccessRequestSource RequestSource { get; set; }
        public string AccessRequestSourceText
        {
            get
            {
                return RequestSource.ToString();
            }
        }
    }

    [Serializable]
    public class MyRequestsGridData
    { 
        public long AdHocAccessRequestId { get; set; }
        public long AccessRequestID { get; set; }
        public string Name { get; set; }
        public string RequestSource { get; set; }
        public string Remarks { get; set; }
        public string RequestType { get; set; }
        public System.DateTime RequestedDate { get; set; }
        public System.DateTime? ExecutionDate { get; set; }
        public AccessRequestStatus RequestStatus { get; set; }
        public string Comments { get; set; }
        public string RequesterName { get; set; }
        public string RequesteeName { get; set; }
        public AdHocActionType AdhocActionType { get; set; }
        public ApplicationType EntitlementDetailType { get; set; }
        public long? EndpointSystemId { get; set; }
        public long? BulkRequestId { get; set; }
        public EndPoint EndPoint { get; set; }
        public long UserID { get; set; }
        public bool IsAdhocRequest { get; set; }
        public bool IsBulkRequest { get; set; }
        public bool IsUndoEventRequest { get; set; }
        public DateTime? EndDate { get; set; }

        public DateTime? StartDate { get; set; }
        public System.DateTime? ValidFrom { get; set; }
        public System.DateTime? ValidTo { get; set; }
        public AccountType AccountType { get; set; }
        public string AccountTypeText
        {
            get
            {

                return AccountType.ToString();
            }
        }
        public string RequestStatusText {
            get
            {

                return RequestStatus.ToString();
            }
        }
        public string AdhocActionTypeText
        {
            get
            {
                return AdhocActionType.ToString();
            }
        }
    }
}
