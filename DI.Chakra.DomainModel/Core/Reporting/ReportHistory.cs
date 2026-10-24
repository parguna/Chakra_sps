using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class ReportHistory : DomainModelBase<ReportHistory, EntityModel.ReportHistory>
    {
        public long ReportId { get; set; }
        public Report ReportSchedule { get; set; }
        public byte[] Report { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public ReportStatus Status { get; set; }

        public string CreatedBy { get; set; }

        public string LastModifiedBy { get; set; }

     

        public static List<ReportHistory> GetReportHistoryByReportId(long reportId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.ReportHistory>(dataAccessTransaction).Get(report => report.ReportId == reportId).ToList());
            }
        }

        public static ReportHistory GetByIdIncludeReport(long id)  
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.ReportHistory>(transaction)
                    .GetById(id, report => report.Report));
            }
        }
    }
}
