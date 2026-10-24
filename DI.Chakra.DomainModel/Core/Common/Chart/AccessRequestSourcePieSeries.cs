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
    public class AccessRequestSourcePieSeries : DomainModelBase<AccessRequestSourcePieSeries, EntityModel.AccessRequestSourcePieSeries>
    {
        public AccessRequestSource Name { get; set; }
        public decimal Y { get; set; }

        public static IList<AccessRequestSourcePieSeries> GetPieChartData()
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                return ORMapList(new AccessRequestSourcePieSeriesRepository(transaction).GetPieChartData());
            }
        }

        public static IList<AccessRequestSourcePieSeries> GetPieChartDataUser(long UserId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                return ORMapList(new AccessRequestSourcePieSeriesRepository(transaction).GetPieChartDataUser(UserId));
            }
        }
    }
}
