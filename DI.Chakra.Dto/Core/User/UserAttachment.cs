using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class UserAttachment:DtoBase
    {
        public long UserId { get; set; }
        public string FileName { get; set; }
        public byte[] AttachmentFile { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public User Users { get; set; }

        public bool IsAmendAttachment { get; set; }
    }
}
