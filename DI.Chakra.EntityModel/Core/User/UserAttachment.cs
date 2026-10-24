using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_UserAttachment")]
    public class UserAttachment : EntityBase, ICreatable, IUpdatable
    {
        public long UserId { get; set; }
        public string FileName { get; set; }
        public byte[] AttachmentFile { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [ForeignKey("UserId")]
        public User Users { get; set; }
        public bool IsAmendAttachment { get; set; }
    }
}
