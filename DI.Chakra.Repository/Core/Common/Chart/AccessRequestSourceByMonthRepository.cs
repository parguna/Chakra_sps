using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class AccessRequestSourceByMonthRepository : RepositoryBase<AccessRequestProgressStatusByMonth>
    {
        public AccessRequestSourceByMonthRepository(DataAccessTransaction transaction) : base(transaction) { }

        public IList<AccessRequestSourceByMonth> GetAccessRequestSource(int Year)
        {
            IList<AccessRequestSourceByMonth> chartData = new List<AccessRequestSourceByMonth>();

            var query = from UQ in CurrentContext.AccessRequest
                        where UQ.CreatedDate.Year == Year && UQ.RequestSource != null 
                        group UQ by new { UQ.CreatedDate.Month, UQ.RequestSource } into g
                        let count = g.Count()
                        select new AccessRequestSourceByMonth
                        {
                            Y = count,
                            Name = g.Key.RequestSource,
                            LabelName = g.Key.Month
                        };

              chartData = query.ToList();

             return chartData;
        }

        public IList<AccessRequestSourceByMonth> GetAccessRequestSourceForUser(int Year, long UserId)
        {
            IList<AccessRequestSourceByMonth> chartData = new List<AccessRequestSourceByMonth>();

            var query = from UQ in CurrentContext.AccessRequest
                        where UQ.CreatedDate.Year == Year && UQ.RequestSource != null && UQ.RequesterId == UserId
                        group UQ by new { UQ.CreatedDate.Month, UQ.RequestSource } into g
                        let count = g.Count()
                        select new AccessRequestSourceByMonth
                        {
                            Y = count,
                            Name = g.Key.RequestSource,
                            LabelName = g.Key.Month
                        };

            chartData = query.ToList();

            return chartData;
        }
    }
}
