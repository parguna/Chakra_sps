using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    [Serializable]
    public class PresentationExportAttestationEntitlementReviewDetail
    {
        public string AttestationName { get; set; }
        public string CampaignName { get; set; }
        public string EndpointSystemName { get; set; }

        public string EndpointType { get; set; }

        public string UserName { get; set; }
        public string Name { get; set; }

        public string Detail { get; set; }

        public string AccountType { get; set; }

        public string EntitlementName { get; set; }

        public string EntitlementType { get; set; }

        public bool Approved { get; set; }

        public bool Rejected { get; set; }

        public string Comments { get; set; }
        public string Reserved1 { get; set; }
        public string Reserved2 { get; set; }
        public string Reserved3 { get; set; }
        public string Reserved4 { get; set; }
        public string Reserved5 { get; set; }
        public string Reserved6 { get; set; }
        public string Reserved7 { get; set; }
        public string Reserved8 { get; set; }
        public string Reserved9 { get; set; }
        public string Reserved10 { get; set; }
        public string ActionReserved1 { get; set; }
        public string ActionReserved2 { get; set; }
        public string ActionReserved3 { get; set; }
        public long TaskId { get; set; }
        public long Level { get; set; }
        public bool IsChanged { get; set; }
        public bool IsActionable { get; set; }
        public bool IsVisible { get; set; }
        public string CreatedBy { get; set; }

        public string LastModifiedBy { get; set; }

        public byte[] excelFile { get; set; }
    }
}
