using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Dto.Presentation
{
    [Serializable]
   public class PresentationAttetstationCampaign: IIDMSReportPropertyExtractor
    {
        public long AttestationCampaignId { get; set; }
        public long AttestationID { get; set; }
        public string AttestationName { get; set; }
        public string AttestationType { get; set; }
        public Guid WorkflowInstanceID { get; set; }

        public string CampaignName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public AttestationDefaultReviewStatus DefaultReviewStatus { get; set; }

        public string Comments { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<System.DateTime> CloseDate { get; set; }
        public int Status { get; set; }
        public int Level { get; set; }
        public long TotalItem { get; set; }
        public string OverallStatus { get; set; }
        public long CampaignSplit { get; set; }
        public string SnapshotFileName { get; set; }
        public string SnapshotFilePath { get; set; }
        public bool RequiresAttestationRemediation { get; set; }
        public Nullable<System.DateTime> SubmittedDate { get; set; }
        public Int32 DaysToComplete {
            get
            {
                Int32 days = 0;
                if (CloseDate.HasValue)
                {
                    var daysToComplete = (CloseDate - DateTime.Now).Value.TotalDays;

                    days = Convert.ToInt32(daysToComplete) + 1;
                }
                return days;
            }
        }
        public string GetReportPropertyValue(string propertyName, string format)
        {
            switch (propertyName)
            {
                case "AttestationCampaignId": return AttestationCampaignId.ToString();
                case "AttestationName": return AttestationName ?? "";
                case "CampaignName": return CampaignName ?? "";
                case "Description": return Description ?? "";
                case "CreatedDate": if (!string.IsNullOrWhiteSpace(format))
                    {
                        return CreatedDate.ToString(format);
                    }
                    else return CreatedDate.ToString("dd/MM/yyyy");
                case "CloseDate":
                    if (!string.IsNullOrWhiteSpace(format) && CloseDate.HasValue)
                    {
                        return CloseDate.Value.ToString(format);
                    }
                    else
                    {
                        return CloseDate?.ToString("dd/MM/yyyy");
                    }
                case "EndDate":
                    if (!string.IsNullOrWhiteSpace(format) && EndDate.HasValue)
                    {
                        return EndDate.Value.ToString(format);
                    }
                    else
                    {
                        return EndDate?.ToString("dd/MM/yyyy");
                    }
                case "OverallStatus": return OverallStatus ?? "";
                case "CampaignSplit": return CampaignSplit.ToString() ?? ""; 
                case "CreatedBy":
                    return CreatedBy;
                case "RequiresAttestationRemediation": return RequiresAttestationRemediation.ToString() ?? "";
                case "DaysToComplete":
                    return "Days To Complete: "+ DaysToComplete.ToString() ?? "";
                case "Level":
                    return Level.ToString() ?? "";
                case "TotalItem":
                    return TotalItem.ToString() ?? "";
                case "SubmittedDate":
                    if (!string.IsNullOrWhiteSpace(format) && SubmittedDate.HasValue)
                    {
                        return SubmittedDate.Value.ToString(format);
                    }
                    else
                    {
                        return SubmittedDate?.ToString("dd/MM/yyyy");
                    }
                default:
                    return "";
            }
        }
    }
}
