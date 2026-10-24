using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
namespace DI.Chakra.Repository
{
  public  class AttestationCompletedPieSeriesRepository : RepositoryBase<AttestationcompletedSourcePieSeries>
    {
        public AttestationCompletedPieSeriesRepository(DataAccessTransaction transaction) : base(transaction) { }
       
        public IList<AttestationcompletedSourcePieSeries> GetPieChartData(string campaignName)
        {
            IList<AttestationcompletedSourcePieSeries> chartData = new List<AttestationcompletedSourcePieSeries>();
            var query = from UQ in this.CurrentContext.Tasks
                        join campaign in this.CurrentContext.AttestationCampaigns on UQ.AttestationCampaignId equals campaign.Id
                        where UQ.AttestationId != null && campaign.CampaignName == campaignName
                        group UQ by UQ.Action into g
                        let count = g.Count()
                        select new AttestationcompletedSourcePieSeries
                        {
                            Y = count,
                            Name = g.Key
                        };

            chartData = query.ToList();


            return chartData;
        }
    }
}
