using DI.Chakra.EntityModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class AccessRequestProgressStatusByMonthRepository : RepositoryBase<AccessRequestProgressStatusByMonth>
    {
      public AccessRequestProgressStatusByMonthRepository(DataAccessTransaction transaction) : base(transaction) { }

        public IQueryable<AccessRequestProgressStatusByMonth> GetAccessRequestProgressStatus(int Year, int Month)
        {

               return   from UQ in CurrentContext.AccessRequest
                        where ((UQ.ActivationDate.Value != null && UQ.ActivationDate.Value.Year == Year && UQ.ActivationDate.Value.Month == Month) ||
                        (UQ.ExecutionTime.HasValue && UQ.ExecutionTime.Value.Year == Year && UQ.ExecutionTime.Value.Month == Month) ||
                        ( UQ.CreatedDate.Year == Year && UQ.CreatedDate.Month == Month))
                        group UQ by new { UQ.ProgressStatus } into g
                        let count = g.Count()
                        select new AccessRequestProgressStatusByMonth
                        {
                            Y = count,
                            Name = g.Key.ProgressStatus,
                            LabelName = g.Key.ProgressStatus
                        };
        }

        public IQueryable<AccessRequestProgressStatusByMonth> GetAccessRequestProgressStatusByUser(int Year, int Month, long UserId)
        {

            return from UQ in CurrentContext.AccessRequest
                   where ((UQ.ActivationDate.Value != null && UQ.ActivationDate.Value.Year == Year && UQ.ActivationDate.Value.Month == Month) ||
                        (UQ.ExecutionTime.HasValue && UQ.ExecutionTime.Value.Year == Year && UQ.ExecutionTime.Value.Month == Month) ||
                        (UQ.CreatedDate.Year == Year && UQ.CreatedDate.Month == Month)) && UQ.RequesterId == UserId
                   group UQ by new { UQ.ProgressStatus } into g
                   let count = g.Count()
                   select new AccessRequestProgressStatusByMonth
                   {
                       Y = count,
                       Name = g.Key.ProgressStatus,
                       LabelName = g.Key.ProgressStatus
                   };
        }
    }
}
