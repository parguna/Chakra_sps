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
    public class AdhocRequestAccountTyebyMonthRepository : RepositoryBase<AdhocRequestAccountTyebyMonth>
    {
        public AdhocRequestAccountTyebyMonthRepository(DataAccessTransaction transaction) : base(transaction) { }


        public IEnumerable<AdhocRequestAccountTyebyMonth> GetAccessRequestAccountType(int Year, int Month)
        {
            return from adhoc in CurrentContext.AdHocAccessRequests
                   join ar in CurrentContext.AccessRequest on adhoc.Id equals ar.AdHocAccessRequestId
                   where ((ar.ActivationDate.Value != null && ar.ActivationDate.Value.Year == Year && ar.ActivationDate.Value.Month == Month) ||
                        (ar.ExecutionTime.HasValue && ar.ExecutionTime.Value.Year == Year && ar.ExecutionTime.Value.Month == Month) ||
                        (ar.CreatedDate.Year == Year && ar.CreatedDate.Month == Month))
                   group ar by new { adhoc.AccountType, adhoc.AdhocActionType } into g
                   let count = g.Count()
                   select new AdhocRequestAccountTyebyMonth
                   {
                       Y = count,
                       Name = g.Key.AdhocActionType,
                       LabelName = g.Key.AccountType
                   };
        }

        public IEnumerable<AdhocRequestAccountTyebyMonth> GetUserAccessRequestByAccountType(int Year, int Month, long UserId)
        {
            return from adhoc in CurrentContext.AdHocAccessRequests
                   join ar in CurrentContext.AccessRequest on adhoc.Id equals ar.AdHocAccessRequestId
                   where ((ar.ActivationDate.Value != null && ar.ActivationDate.Value.Year == Year && ar.ActivationDate.Value.Month == Month) ||
                        (ar.ExecutionTime.HasValue && ar.ExecutionTime.Value.Year == Year && ar.ExecutionTime.Value.Month == Month) ||
                        (ar.CreatedDate.Year == Year && ar.CreatedDate.Month == Month)) && ar.RequesterId == UserId
                   group ar by new { adhoc.AccountType, adhoc.AdhocActionType } into g
                   let count = g.Count()
                   select new AdhocRequestAccountTyebyMonth
                   {
                       Y = count,
                       Name = g.Key.AdhocActionType,
                       LabelName = g.Key.AccountType
                   };
        }
    }
}
