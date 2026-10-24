using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.Dto
{
    public class AttestationCampaignReport : DtoBase
    {
        public Nullable<long> AttestationCampaignId { get; set; }

        public string AttestationCampaignName { get; set; }

        /// <summary>
        /// Type of Report e.g. PDF, CSV, Excel-Flat, Excel-Hierarchical
        /// </summary>
        public AttestationReportType ReportType { get; set; }

        public string ContentType { get; set; }

        public string FileName { get; set; }

        public byte[] AttachmentFile { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }
    }
}