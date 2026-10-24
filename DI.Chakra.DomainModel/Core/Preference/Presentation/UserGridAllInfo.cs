using AutoMapper;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel.Presentation
{
    public class UserGridAllInfo : DomainModelBase<UserGridAllInfo, EntityModel.Presentation.UserGridAllInfo>
    {
        public string PagePath { get; set; }
        public string GridId { get; set; }
        public long UserId { get; set; }
        public UserGridInfo UserGridInfo { get; set; }
        public List<UserGridFilterInfo> UserGridFilterInfo { get; set; }
        public List<UserGridColumnInfo> UserGridColumnInfo { get; set; }


        public static UserGridAllInfo GetByUserId(long userId, string gridId, string pagePath)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                UserGridAllInfo userGridInfo = new UserGridAllInfo();
                userGridInfo.UserId = userId;
                userGridInfo.GridId = gridId;
                userGridInfo.PagePath = pagePath;
                userGridInfo.UserGridInfo = UserGridInfo.GetByUserId(userId, gridId, pagePath);
                userGridInfo.UserGridFilterInfo = DomainModel.UserGridFilterInfo.GetFilterByUserId(userId, gridId, pagePath);
                userGridInfo.UserGridColumnInfo = DomainModel.UserGridColumnInfo.GetcolumnsByUserId(userId, gridId, pagePath);
                return userGridInfo;
            }
        }


    }
}
