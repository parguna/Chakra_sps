using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class ReportAttributes: DomainModelBase<ReportAttributes, EntityModel.ReportAttributes>
    {
        public long ReportId { get; set; }
        public Report ReportSchedule { get; set; }
        public string ParamKey { get; set; }
        public string ParamValue { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }


        public static List<ReportAttributes> GetByReportId(long reportId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.ReportAttributes>(dataAccessTransaction).Get(report => report.ReportId == reportId).ToList());
            }
        }
    }
}
