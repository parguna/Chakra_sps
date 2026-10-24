using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;

namespace DI.Chakra.DomainModel
{
    public class UserImage : DomainModelBase<UserImage,EntityModel.UserImage>
    {
        public byte[] Image { get; set; }

        public long? UserId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public static UserImage GetImageByUserId(long userId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap( new RepositoryBase<EntityModel.UserImage>(dataAccessTransaction).
                    Get(userImage => userImage.UserId == userId).FirstOrDefault());
            }
        }
    }
}
