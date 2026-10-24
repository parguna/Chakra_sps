using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_Attachment")]
    public class Attachment : EntityBase, ICreatable, IUpdatable
    {
        public long RequestTypeId { get; set; }
        
        public Nullable<long> TaskId { get; set; }

        public long AccessRequestId { get; set; }

        public string ContentType { get; set; }

        public string FileName { get; set; }

        public byte[] AttachmentFile { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public string LastModifiedBy { get; set; }
        public string AccessRequestSource { get; set; }
    }
}
