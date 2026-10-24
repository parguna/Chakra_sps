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
    public class UserExclusion : DomainModelBase<UserExclusion, EntityModel.UserExclusion>
    {

        public long UserId { get; set; }
        public string UserName { get; set; }
        public EmployeeEvents EventType { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public User Users { get; set; }
        public string FileName { get; set; }
        public byte[] AttachmentFile { get; set; }
        public string ContentType { get; set; }
        public string Remarks { get; set; }
        public bool IsDeleted { get; set; }
        public static void SaveUserList(List<UserExclusion> userExclusionList, string currentUser)
        {
            using (var transaction = new DataAccessTransaction(currentUser))
            {

                List<UserExclusion> dbExclusionList = UserExclusion.GetAll();

                if (dbExclusionList.Count > 0)
                {
                    List<UserExclusion> removedExclusionDetails = dbExclusionList.Except(userExclusionList, new IdBaseEqualityComparer<UserExclusion>()).ToList();

                    removedExclusionDetails.ForEach(prd =>
                    {
                        prd.Delete(transaction);
                    }
                    );
                }
                userExclusionList.ForEach(prd => prd.Save(transaction));
                transaction.Commit();
            }
        }

        public static UserExclusion GetUserExclusionByUserId(long userId, EmployeeEvents eventType)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.UserExclusion>(transaction).Get(userEx => userEx.UserId == userId && userEx.EventType == eventType && userEx.IsDeleted == false).FirstOrDefault());
            }
        }

        public static List<UserExclusion> GetAllUserExclusionByUserId(long userId)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.UserExclusion>(transaction).Get(userEx => userEx.UserId == userId && userEx.IsDeleted == false).ToList());
            }
        }

        public static List<UserExclusion> GetAllUserExclusionByUserIdWithDate(long userId)
        {
            using (var transaction = new DataAccessTransaction())
            {
                var now = DateTime.Now.Date;
                return ORMapList(new RepositoryBase<EntityModel.UserExclusion>(transaction).Get(userEx => userEx.UserId == userId &&
                userEx.EffectiveDate.HasValue && userEx.EffectiveDate.Value > now && userEx.IsDeleted == false).ToList());                
            }
        }

        public static List<UserExclusion> GetUserExclusionListByUserId(long userId, EmployeeEvents eventType)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.UserExclusion>(transaction).Get(userEx => userEx.UserId == userId && userEx.EventType == eventType && userEx.IsDeleted == false).ToList());
            }
        }

    }
}
