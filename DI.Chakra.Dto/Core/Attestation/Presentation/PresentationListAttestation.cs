using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationListAttestation : Attestation, IIDMSReportPropertyExtractor
    {
        public long AttestationId { get; set; }

        public string Status { get; set; }

        public Nullable<System.DateTime> RunDate { get; set; }

        public string AttesationCreatedBy { get; set; }

        public int ResponseTimeInDays { get; set; }

        public Occurence occurence { get; set; }

        public string EndPointSystem { get; set; }
        public string AttestationRemediation { get; set; }
        public string SpilitCampaignScope { get
            {
                return SplitCampaignUsing.ToString();
            }
        }
        public string GetReportPropertyValue(string propertyName, string format)
        {
            switch (propertyName)
            {
                case "EndPointSystem": return EndPointSystem;
                case "AttestationDescription": return this.AttestationDescription;
                case "AttestationName": return this.AttestationName;
                case "AttestationRemediation": return AttestationRemediation;
                case "SplitCampaignUsing": return SplitCampaignUsing.ToString();
                case "SpilitCampaignScope": return SpilitCampaignScope;
                case "CreatedDate":
                    {
                        if(!string.IsNullOrWhiteSpace(format))
                            return CreatedDate.ToString(format);
                        return CreatedDate.ToString("dd-MM-yyyy hh:mm tt");
                    }
                case "LastModifiedDate":
                    {
                        if (LastModifiedDate.HasValue)
                        {
                            if (!string.IsNullOrWhiteSpace(format))
                                return LastModifiedDate.Value.ToString(format);
                            return LastModifiedDate.Value.ToString("dd-MM-yyyy hh:mm tt");
                        }
                        return "";
                    }
                case "LastModifiedBy": return LastModifiedBy??"";
                case "CreatedBy": return CreatedBy;
                case "AttesationCreatedBy":
                    return AttesationCreatedBy;
                default:return "";
            }
        }
    }
}
