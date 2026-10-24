using DI.Chakra.Infrastructure;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class GlobalAnswer : DomainModelBase<GlobalAnswer, EntityModel.GlobalAnswer>
    {
        public long UserId { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public User User { get; set; }

        public static List<GlobalAnswer> GetByUserId(long userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<GlobalAnswer> globalAnswer= ORMapList(new RepositoryBase<EntityModel.GlobalAnswer>(dataAccessTransaction)
                       .Get(ga => ga.UserId == userId).ToList());

                foreach (var item in globalAnswer)
                {
                    item.Answer = RSAHelper.DecryptData(item.Answer);
                }

                return globalAnswer;

            }
        }

        public static bool HasAnyQuestionByUserId(long userId, string question) 
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.GlobalAnswer>(dataAccessTransaction)
                    .Any(globalAnswer => globalAnswer.UserId == userId && globalAnswer.Question == question);
            }
        }

        public static GlobalAnswer GetsecurityquestionById(long userId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.GlobalAnswer>(dataAccessTransaction).GetById(userId));
            }
        }
    }
}
