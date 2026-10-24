using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class AttestationcompletedSourcePieSeries : DomainModelBase<AttestationcompletedSourcePieSeries, EntityModel.AttestationcompletedSourcePieSeries>
    {

        public StatusType Name { get; set; }
        public decimal Y { get; set; }

        public static IList<AttestationcompletedSourcePieSeries> GetPieChartData(string campaignName)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new AttestationCompletedPieSeriesRepository(dataAccessTransaction).GetPieChartData(campaignName));
            }
        }

    }
}
