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
    public class AccessRequestSourceChartDataRepository : RepositoryBase<AccessRequestSourceChartData>
    {
        public AccessRequestSourceChartDataRepository(DataAccessTransaction transaction) : base(transaction) { }


        public IList<AccessRequestSourceChartData> GetSeriesDataByAccessRequestSource(AccessRequestSource RequestSource)
        {

            IList<AccessRequestSourceChartData> chartData = new List<AccessRequestSourceChartData>();

            var query = from AR in CurrentContext.AccessRequest
                        join U in CurrentContext.User
                        on AR.UserId equals U.Id
                        where AR.RequestSource == RequestSource
                        select new AccessRequestSourceChartData
                        {
                            Description = AR.Description,
                            UserName = U.UserName,
                            RequestStatus = AR.RequestStatus,
                            ProgressStatus = AR.ProgressStatus
                        };

            chartData = query.ToList();

            return chartData;
        }

        public IList<AccessRequestSourceChartData> GetAccessRequestSourceByMonth(int Year, int Month, AccessRequestSource RequestSource)
        {

            IList<AccessRequestSourceChartData> chartData = new List<AccessRequestSourceChartData>();

            var query = from AR in CurrentContext.AccessRequest
                        join U in CurrentContext.User
                        on AR.UserId equals U.Id
                        where AR.RequestSource == RequestSource
                        && ((AR.ActivationDate.Value != null && AR.ActivationDate.Value.Year == Year && AR.ActivationDate.Value.Month == Month) ||
                        (AR.ExecutionTime.HasValue && AR.ExecutionTime.Value.Year == Year && AR.ExecutionTime.Value.Month == Month) ||
                        (AR.CreatedDate.Year == Year && AR.CreatedDate.Month == Month))
                        select new AccessRequestSourceChartData
                        {
                            Description = AR.Description,
                            UserName = U.UserName,
                            RequestStatus = AR.RequestStatus,
                            ProgressStatus = AR.ProgressStatus
                        };

            chartData = query.ToList();

            return chartData;
        }
    }
}
