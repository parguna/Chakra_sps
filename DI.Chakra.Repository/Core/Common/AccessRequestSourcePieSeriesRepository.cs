using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DI.Chakra.Repository
{
    public class AccessRequestSourcePieSeriesRepository : RepositoryBase<AccessRequestSourcePieSeries>
    {
        public AccessRequestSourcePieSeriesRepository(DataAccessTransaction transaction) : base(transaction) { }

        public IList<AccessRequestSourcePieSeries> GetPieChartData()
        {
            IList<AccessRequestSourcePieSeries> chartData = new List<AccessRequestSourcePieSeries>();
            var query = from UQ in this.CurrentContext.AccessRequest
                        where UQ.RequestSource != null
                        group UQ by UQ.RequestSource into g
                        let count = g.Count()
                        select new AccessRequestSourcePieSeries
                        {
                            Y = count,
                            Name = g.Key
                        };

            chartData = query.ToList();


            return chartData;
        }

        public IList<AccessRequestSourcePieSeries> GetPieChartDataUser(long UserId)
        {
            IList<AccessRequestSourcePieSeries> chartData = new List<AccessRequestSourcePieSeries>();
            var query = from UQ in this.CurrentContext.AccessRequest
                        where UQ.RequestSource != null && UQ.RequesterId == UserId
                        group UQ by UQ.RequestSource into g
                        let count = g.Count()
                        select new AccessRequestSourcePieSeries
                        {
                            Y = count,
                            Name = g.Key
                        };

            chartData = query.ToList();


            return chartData;
        }

    }
}
