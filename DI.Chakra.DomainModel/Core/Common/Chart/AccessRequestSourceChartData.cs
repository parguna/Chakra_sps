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
    public class AccessRequestSourceChartData : DomainModelBase<AccessRequestSourceChartData, EntityModel.AccessRequestSourceChartData>
    {
        public string UserName { get; set; }
        public string Description { get; set; }
        public AccessRequestStatus RequestStatus { get; set; }
        public AccessRequestProgress ProgressStatus { get; set; }


        public static IList<AccessRequestSourceChartData> GetSeriesDataByAccessRequestSource(AccessRequestSource RequestSource)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                return ORMapList(new AccessRequestSourceChartDataRepository(transaction).GetSeriesDataByAccessRequestSource(RequestSource));
            }
        }

        public static IList<AccessRequestSourceChartData> GetAccessRequestSourceByMonth(int Year, int Month, AccessRequestSource RequestSource)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                return ORMapList(new AccessRequestSourceChartDataRepository(transaction).GetAccessRequestSourceByMonth(Year,Month,RequestSource));
            }
        }

    }
}
