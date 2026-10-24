using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.Dto.Presentation
{
    [Serializable]
    public class PresentationMyRequestData : IIDMSReportPropertyExtractor
    {
        public long Id
        {
            get
            {
                if (BulkRequestId.HasValue && BulkRequestId > 0)
                {
                    return BulkRequestId.Value;
                }
                else
                {
                    return AdHocAccessRequestId;
                }
            }
        }
        public long AdHocAccessRequestId { get; set; }
        public long AccessRequestID { get; set; }
        public string Name { get; set; }
        public string RequestType { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? ExecutionDate { get; set; }
        public DI.Chakra.Infrastructure.Enums.AccessRequestStatus RequestStatus { get; set; }
        public string RequestStatusString { get { return RequestStatus.GetDisplayAs(); } }
        public AccessRequestProgress ProgressStatus { get; set; }
        public string ProgressStatusString
        {
            get
            {
                if (RequestSource == Constants.NonEmployee && RequestStatus == AccessRequestStatus.Requested)
                    return AccessRequestProgress.WaitingforApproval.GetDescription();
                if (RequestSource == Constants.NonEmployee && RequestStatus == AccessRequestStatus.Granted)
                    return AccessRequestProgress.Completed.GetDescription();
                return RequestSource == Constants.NonEmployee ? RequestStatus.GetDescription() : ProgressStatus.GetDescription(); //this.ProgressStatus.ToString();
            }
        }
        public string RequestSource { get; set; }
        public string Status { get; set; }
        public string Comments { get; set; }
        public string RequesterName { get; set; }
        public string RequesteeName { get; set; }
        public AdHocActionType AdhocActionType { get; set; }
        public long? EndpointSystemId { get; set; }
        public EndPoint EndPoint { get; set; }
        public AdHocAccessRequest AdHocAccessRequests { get; set; }
        public AccountType AccountType { get; set; }
        public string AccountTypeString { get { return AccountType.GetDescription(); } }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public long UserID { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? StartDate { get; set; }
        public string ErrorMessage { get; set; }
        public long? ParentrequestId { get; set; }
        public string RequesteeUserName { get; set; }
        public string RequesterUserName { get; set; }
        public string RequesteeEmployeeId { get; set; }
        public string RequesteeDepartment { get; set; }
        public bool IsAdhocRequest { get; set; }
        public bool IsBulkRequest { get; set; }
        public bool IsUndoEventRequest { get; set; }
        public string Entitlement { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public string EntitlementString { get; set; }
        public string ServiceAccountComplianceString { get; set; }
        public long? BulkRequestId { get; set; }
        public string Remarks { get; set; }
        public string AccessRequestComments { get; set; }
        public string ConflictRemarks { get; set; }
        public string GetReportPropertyValue(string propertyName, string format)
        {
            switch (propertyName)
            {
                case "requestid": return AccessRequestID.ToString() ?? string.Empty;
                case "progressstatus": return ProgressStatusString ?? string.Empty;
                case "status": return RequestStatusString ?? string.Empty;
                case "applicationname": return Name ?? string.Empty;
                case "requestsource": return RequestSource ?? string.Empty;
                case "accounttype": return AccountTypeString ?? string.Empty;
                case "requesttype": return RequestType ?? string.Empty;
                case "requestfor": return RequesteeName ?? string.Empty;
                case "requestby": return RequesterName ?? string.Empty;
                case "entitlementstring": return Utility.DeserializeEntitlement(Entitlement, ApplicationType) ?? string.Empty;
                case "requestdate": return RequestedDate.ToString(format) ?? string.Empty;
                case "startdate": return ValidFrom?.ToString(format) ?? string.Empty;
                case "enddate": return ValidTo?.ToString(format) ?? string.Empty;
                case "executiontime": return ExecutionDate?.ToString(format) ?? string.Empty;
                default:
                    return string.Empty;
            }
        }
    }
}
