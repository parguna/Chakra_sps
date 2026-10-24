using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class UserAttachment : DomainModelBase<UserAttachment, EntityModel.UserAttachment>
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
        public static List<UserAttachment> GetByUserId(long userId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.UserAttachment>(dataAccessTransaction).Get(attach =>attach.UserId == userId).ToList());
            }
        }
    }
}
