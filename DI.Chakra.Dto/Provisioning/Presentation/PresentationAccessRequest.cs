using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.Dto.Presentation
{
    [Serializable]
    public class PresentationAccessRequest
    {
        public string Id
        {
            get
            {
                return (AdHocAccessRequestId == null || AdHocAccessRequestId == 0) ? string.Empty : AdHocAccessRequestId.ToString();
            }
        }
        public long? AdHocAccessRequestId { get; set; }
        public long? TriggerId { get; set; }
        public string DisplayName { get; set; }
        public string ADID { get; set; }
        public string RequestSource { get; set; }
        public AccessRequestProgress ProgressStatus { get; set; }
        public string ProgressStatusString
        {
            get
            {
                return UtilityMethods.GetEnumAttribute<DisplayAs>(ProgressStatus); //this.ProgressStatus.ToString();
            }
        }
        public string EndpointsystemName { get; set; }
        public DateTime? ExecutionTime { get; set; }
        public DateTime? RequestDate { get; set; }
        public string EndpointName { get; set; }
        public long AccessRequestID { get; set; }
        public string Name { get; set; }
        public string RequestType { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? ExecutionDate { get; set; }
        public DI.Chakra.Infrastructure.Enums.AccessRequestStatus RequestStatus { get; set; }
        public string Status { get { return UtilityMethods.GetEnumAttribute<DisplayAs>(RequestStatus); } }
        public string Comments { get; set; }
        public AdHocActionType AdhocActionType { get; set; }
        public long? EndpointSystemId { get; set; }
        public EndPoint EndPoint { get; set; }
        public AdHocAccessRequest AdHocAccessRequests { get; set; }
        public AccountType AccountType { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public long UserID { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? StartDate { get; set; }
        public string ErrorMessage { get; set; }
        public long? ParentrequestId { get; set; }
        public string RequesteeUserName { get; set; }
        public string RequesterUserName { get; set; }
        public bool IsAdhocRequest { get; set; }
        public bool IsBulkRequest { get; set; }
        public bool IsUndoEventRequest { get; set; }
        public string Entitlement { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public string EntitlementString { get; set; }
        public string AccountTypeString { get { return AccountType.GetDescription(); } }

        public string GetReportPropertyValue(string propertyName, string format)
        {
            switch (propertyName)
            {
                case "requestid": return AccessRequestID.ToString() ?? string.Empty;
                case "triggerid": return TriggerId.ToString() ?? string.Empty;
                case "progressstatus": return ProgressStatusString ?? string.Empty;
                case "status": return ProgressStatusString ?? string.Empty;
                case "applicationname": return Name ?? string.Empty;
                case "requestsource": return RequestSource ?? string.Empty;
                case "accounttype": return AccountTypeString ?? string.Empty;
                case "requesttype": return RequestType ?? string.Empty;
                case "requestfor": return RequesteeUserName ?? string.Empty;
                case "requestby": return RequesterUserName ?? string.Empty;
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
