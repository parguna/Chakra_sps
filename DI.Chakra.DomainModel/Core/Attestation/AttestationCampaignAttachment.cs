using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class AttestationCampaignAttachment : DomainModelBase<AttestationCampaignAttachment, EntityModel.AttestationCampaignAttachment>
    {        
        public Nullable<long> AttestationCampaignId { get; set; }
        
        public string ContentType { get; set; }

        public string FileName { get; set; }

        public byte[] AttachmentFile { get; set; }

        public string FileSavePath { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public string LastModifiedBy { get; set; }
        
        public AttestationCampaign AttestationCampaign { get; set; }

        public static List<AttestationCampaignAttachment> GetByAttestationCampaignId(long attestationCampaignId)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AttestationCampaignAttachment>(tran)
                    .Get(a => a.AttestationCampaignId == attestationCampaignId).ToList());
            }
        }

        public static AttestationCampaignAttachment GetAttachmentById(long attachmentId, string fileName)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AttestationCampaignAttachment>(tran)
                    .Get(a => a.Id == attachmentId && a.FileName == fileName).FirstOrDefault());
            }
                
        }

        public static long GetAttachmentCount(long attestationCampaignId)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return (new RepositoryBase<EntityModel.AttestationCampaignAttachment>(tran)
                    .Count(a => a.AttestationCampaignId == attestationCampaignId));
            }
        }
    }
}
