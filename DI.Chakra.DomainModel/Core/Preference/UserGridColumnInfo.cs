using System;
using System.Collections;
using System.Collections.Generic;
using DI.Chakra.EntityModel;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class UserGridColumnInfo : DomainModelBase<UserGridColumnInfo, EntityModel.UserGridColumnInfo>
    {
        public string PagePath { get; set; }

        public string GridId { get; set; }

        public long UserId { get; set; }

        public string ColumnName { get; set; }

        public int OrderIndex { get; set; }

        public User Users { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public static List<UserGridColumnInfo> GetcolumnsByUserId(long userId, string gridId, string pagePath)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.UserGridColumnInfo>(dataAccessTransaction).Get(m => m.UserId == userId && m.PagePath == pagePath && m.GridId == gridId).ToList());
            }
        }

        public static void Delete(long userId, string pagePath, string gridId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<UserGridColumnInfo> info = ORMapList(new RepositoryBase<EntityModel.UserGridColumnInfo>(dataAccessTransaction).Get(m => m.UserId == userId && m.PagePath == pagePath && m.GridId == gridId).ToList());
                if (info != null)
                {
                    UserGridColumnInfo.DeleteList(info);
                }
                dataAccessTransaction.Commit();
            }
        }
    }
}
