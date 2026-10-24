using System;
using System.Collections.Generic;
using DI.Chakra.EntityModel;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class UserGridInfo : DomainModelBase<UserGridInfo, EntityModel.UserGridInfo>
    {
        public string PagePath { get; set; }

        public string GridId { get; set; }

        public long UserId { get; set; }

        public string SortDirection { get; set; }

        public string SortColumnName { get; set; }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
      
        public  User Users { get; set; }

        public static UserGridInfo GetByUserId(long userId, string gridId, string pagePath)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.UserGridInfo>(dataAccessTransaction).Get(m => m.UserId == userId && m.PagePath == pagePath && m.GridId == gridId).LastOrDefault());
            }
        }

        public static void Delete(long UserId, string PagePath, string GridId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                UserGridInfo info = ORMap(new RepositoryBase<EntityModel.UserGridInfo>(dataAccessTransaction).Get(m => m.UserId == m.UserId && m.PagePath == m.PagePath && m.GridId == m.GridId).LastOrDefault());
                if (info != null)
                {
                    UserGridInfo.Delete(info.Id);
                }
                dataAccessTransaction.Commit();
            }
        }

    }
}
