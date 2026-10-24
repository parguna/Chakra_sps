using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class AttestationCampaignAttachment : DtoBase
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
    }
}