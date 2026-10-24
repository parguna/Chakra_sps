using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.Dto.Presentation
{
    [Serializable]
    public class PresentationEmployeeEventTrigger: IIDMSReportPropertyExtractor
    {
        public long Id
        {
            get
            {
                return TriggerId;
            }
        }
        public long TriggerId { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string ADID { get; set; }
        public string EmployeeId { get; set; }
        public EmployeeEvents EventType { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string OldEffectiveDate { get; set; }
        public bool IsRequestCreated { get; set; }
        public bool IsAdminCheckRequired { get; set; }
        public string UserDetails { get; set; }
        public string OldUserDetails { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }
        public string EmployeeEvent
        {
            get
            {
                return EventType.GetDescription();
            }
        }
        public string RejoiningUser { get; set; }
        public string AdminCheckRequired { get; set; }
        public string RequestCreated { get; set; }
        public string Active { get; set; }
        public bool HasAttachment { get; set; }
        public bool HasAmendAttachment { get; set; }
        public string Comments { get; set; }
        public Nullable<DateTime> ActualEventDate { get; set; }
        public string GetReportPropertyValue(string propertyName, string format)
        {
            var returnValue = string.Empty;
            switch (propertyName)
            {
                case "TriggerId":
                    returnValue = TriggerId.ToString() ?? "";
                    break;
                case "UserId":
                    returnValue = UserId.ToString() ?? "";
                    break;
                case "UserName":
                    returnValue = UserName ?? "";
                    break;
                case "ADID":
                    returnValue = ADID ?? "";
                    break;
                case "EmployeeId":
                    returnValue = EmployeeId ?? "";
                    break;
                case "EventType":
                    returnValue = EventType.GetDescription() ?? "";
                    break;
                case "EffectiveDate":
                    returnValue = !string.IsNullOrWhiteSpace(format)? EffectiveDate.ToString(format):"";
                    break;
                case "CreatedDate":
                    returnValue = !string.IsNullOrWhiteSpace(format) ? CreatedDate.ToString(format) : "";
                    break;
                case "CreatedBy":
                    returnValue = CreatedBy??"";
                    break;
                case "LastModifiedDate":
                    returnValue = !string.IsNullOrWhiteSpace(format) && LastModifiedDate.HasValue ? LastModifiedDate?.ToString(format) : "";
                    break;
                case "LastModifiedBy":
                    returnValue = LastModifiedBy ?? "";
                    break;
                case "OldEffectiveDate":
                    returnValue = OldEffectiveDate??"";
                    break;
                case "IsActive":
                    returnValue = IsActive.GetBoolToString();
                    break;
                case "IsRequestCreated":
                    returnValue = IsRequestCreated.GetBoolToString();
                    break;
                case "IsAdminCheckRequired":
                    returnValue = IsAdminCheckRequired.GetBoolToString();
                    break;
                case "Comments":
                    returnValue = this.Comments??"";
                    break;
                case "ActualEventDate":
                    returnValue = !string.IsNullOrWhiteSpace(format) && ActualEventDate.HasValue ? ActualEventDate?.ToString(format) : "";
                    break;
                default:
                    break;
            }
            return returnValue;
        }
    }
}
