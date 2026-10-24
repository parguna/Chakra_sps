using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.Dto.Presentation
{
    [Serializable]
    public class PresentationTask : IIDMSReportPropertyExtractor
    {
        public long Id
        {
            get
            {
                return TaskId;
            }
        }
        public long TaskId { get; set; }
        public byte RequestTypeId { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string NRIC { get; set; }
        public Nullable<long> AttestationId { get; set; }
        public Nullable<long> AttestationCampaignId { get; set; }
        public string Approver { get; set; }
        public string CampaignName { get; set; }
        public string AttestationName { get; set; }
        public string EventName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ReponseTimeInDays { get; set; }
        public bool IsActive { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string RequestedBy { get; set; }
        public Nullable<long> AdHocAccessRequestId { get; set; }
        public Nullable<long> AccessRequestID { get; set; }
        public Nullable<long> AssignedTo { get; set; }
        public Nullable<long> AssignedBy { get; set; }
        public string RequestedFor { get; set; }
        public string Action { get; set; }
        public string AccountType { get; set; }
        public bool IsDelegated { get; set; }
        public string AssignedToName { get; set; }
        public string AssignedByName { get; set; }

        public string DelegatedBy { get; set; }
        public string DelegatedComment { get; set; }
        public bool IsGroup { get; set; }
        public DI.Chakra.Infrastructure.Enums.AccessRequestStatus TaskAccessRequest { get; set; }
        public DateTime? RequestedDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string Type { get; set; }
        public string StatusType { get; set; }
        public string RequestReason { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Entitlement { get; set; }
        public string EntitlementXML { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public string RequestType { get; set; }
        public string IsDelegate { get; set; }
        public string TaskName { get; set; }
        public long Level { get; set; }
        public Nullable<long> WorkFlowConfigurationId { get; set; }
        public string ActionUser { get; set; }
        public string GroupName { get; set; }
        public string AssignedToADID { get; set; }
        public string ActionUserADID { get; set; }
        public string RequestForSupDisplayName { get; set; }
        public string RequestForDept { get; set; }
        public string RequestForDesignation { get; set; }
        public string TaskSource { get; set; }
        public string IsDelegatedByUser { get; set; }
        public string StartDateToShort
        {

            get
            {
                if (this.StartDate.HasValue)
                {
                    return this.StartDate.Value.ToShortDateString();
                }
                else return string.Empty;
            }
        }
        public string EndDateShort {
            get
            {
                if (this.EndDate.HasValue)
                {
                    return this.EndDate.Value.ToShortDateString();
                }
                else return string.Empty;
            }
        }
        public string RequestTypeName
        {
            get; set;
        }
        public string GetReportPropertyValue(string propertyName, string format)
        {
            switch (propertyName)
            {
                case "accessrequestid": return AccessRequestID.HasValue ? AccessRequestID.Value.ToString() : "";
                case "taskid": return TaskId.ToString() ?? string.Empty;
                case "eventname": return EventName ?? string.Empty;
                case "tasksource": return TaskSource ?? string.Empty;
                case "accounttype": return AccountType ?? string.Empty;
                case "type": return Type ?? string.Empty;
                case "requesttype": return RequestType ?? string.Empty;
                case "requestedfor": return RequestedFor ?? string.Empty;
                case "requestedby": return RequestedBy ?? string.Empty;
                case "isdelegate": return IsDelegate ?? string.Empty;
                case "entitlement": return Entitlement ?? string.Empty;
                case "requesteddate": return RequestedDate!=null? RequestedDate.Value.ToString(format): string.Empty;
                case "createddate": return CreatedDate.ToString(format);
                case "startdatetoshort": return StartDateToShort?? string.Empty;
                case "enddateshort": return EndDateShort ?? string.Empty;
                case "statustype": return StatusType ?? string.Empty;
                case "actionuser": return ActionUser ?? string.Empty;
                case "assignedtoname": return AssignedToName ?? string.Empty;
                case "assignedbyname": return AssignedByName ?? string.Empty;
                case "delegatedby": return DelegatedBy ?? string.Empty;
                case "completiondate": return CompletionDate?.ToString(format);
                case "requesttypename":
                    return RequestTypeName ?? string.Empty;
                default:
                    return string.Empty;
            }
        }
    }
}
