using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class UserGridFilterInfo : DomainModelBase<UserGridFilterInfo, EntityModel.UserGridFilterInfo>
    {   
        public string PagePath { get; set; }

        public string GridId { get; set; }

        public long UserId { get; set; }

        public string Operand { get; set; }

        public string ColumnName { get; set; }

        public string FilterValue { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
       
        public  User Users { get; set; }

        public static UserGridFilterInfo GetFilterByUserId(long userId, string gridId, string pagePath,string columnName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.UserGridFilterInfo>(dataAccessTransaction).Get(m => m.UserId == userId && m.PagePath == pagePath && m.GridId == gridId && m.ColumnName== columnName).FirstOrDefault());
            }
        }

        public static List<UserGridFilterInfo> GetFilterByUserId(long userId, string gridId, string pagePath)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.UserGridFilterInfo>(dataAccessTransaction).Get(m => m.UserId == userId && m.PagePath == pagePath && m.GridId == gridId).ToList());
            }
        }
        public static void Delete(long userId, string pagePath, string gridId,string columnName)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                UserGridFilterInfo info = ORMap(new RepositoryBase<EntityModel.UserGridFilterInfo>(dataAccessTransaction).Get(m => m.UserId == userId && m.PagePath == pagePath && m.GridId == gridId && m.ColumnName == columnName).FirstOrDefault());
                if (info != null)
                {
                    UserGridFilterInfo.Delete(info.Id);
                }
                dataAccessTransaction.Commit();
            }
        }

        public static string Getfiltervalue(long userId, string gridId, string pagePath)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.UserGridFilterInfo>(dataAccessTransaction).Get(m => m.UserId == userId && m.PagePath == pagePath && m.GridId == gridId).Select(c=>c.FilterValue).FirstOrDefault();
            }
        }

        public static void ClearFilterByUserId(long userId)
        {
            var query = $"delete DI_M_UserGridFilterInfo where UserId = {userId}";
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                new RepositoryBase<EntityModel.UserGridFilterInfo>(dataAccessTransaction).ExecuteQuery(query);
                dataAccessTransaction.Commit();
            }
        }
    }
}
