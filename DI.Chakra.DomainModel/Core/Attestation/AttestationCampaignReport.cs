using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class AttestationCampaignReport : DomainModelBase<AttestationCampaignReport, EntityModel.AttestationCampaignReport>
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

        public static List<AttestationCampaignReport> GetByAttestationCampaignId(long attestationCampaignId)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AttestationCampaignReport>(tran)
                    .Get(a => a.AttestationCampaignId == attestationCampaignId).ToList());
            }
                
        }

        public static AttestationCampaignReport GetReportByType(long attestationCampaignId, AttestationReportType reportType)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AttestationCampaignReport>(tran)
                    .Get(a => a.AttestationCampaignId == attestationCampaignId && a.ReportType == reportType).FirstOrDefault());
            }
        }
    }
}