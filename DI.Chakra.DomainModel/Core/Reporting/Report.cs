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
    public class Report: DomainModelBase<Report, EntityModel.Report>
    {
        public ReportType Type { get; set; }
        public string Format { get; set; }
        public Process process { get; set; }
        public string Name { get; set; }
        public List<ReportAttributes> ReportAttributes { get; set; }
        public string ReportUrl { get; set; }
        public long UserId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
        public string MailTo { get; set; }
        public string MailCC { get; set; }
        public static Report GetByReportName(string reportName)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.Report>(dataAccessTransaction).Get(report => report.Name == reportName).FirstOrDefault());
            }
        }

        public static long  GetReportId(string reportName)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.Report>(dataAccessTransaction).Get(report => report.Name == reportName).FirstOrDefault()).Id;
            }
        }

        public static Report GetReportIncludingAttributes(long reportId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                Report rpt;

                rpt = ORMap(new RepositoryBase<EntityModel.Report>(dataAccessTransaction).Get(report => report.Id == reportId, report=>report.ReportAttributes).FirstOrDefault());
                if (rpt != null)
                {
                    rpt.process = Process.GetByProcessType(rpt.Id, ProcessType.Reporting);
                   
                }
                return rpt;
            }
        }


        public static List<Report> GetReportList(string userName)//Need to match UserName
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.Report>(dataAccessTransaction).GetAll(report => report.ReportAttributes, report => report.process).ToList());
            }
        }

        public static void DeleteReportProcess(long processId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                List<ReportAttributes> attributes = DomainModel.ReportAttributes.GetByReportId(processId);
                DomainModel.ReportAttributes.DeleteList(dataAccessTransaction, attributes);
                Report.Delete(dataAccessTransaction, processId);
                    Process process = Process.GetByProcessTypeKeyId(processId, Infrastructure.Enums.ProcessType.Reporting);
                    if (process != null)
                    {
                    Process.Delete(dataAccessTransaction, process.Id);
                    }
                    Scheduler scheduler = Scheduler.GetById(process.Scheduler_SchedulerId);
                    if (scheduler != null)
                    {
                    Scheduler.Delete(dataAccessTransaction, scheduler.Id);
                    }
                dataAccessTransaction.Commit();
            }
        }


        public override long Save(string currentUser)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                if (this.Id == 0)
                {
                    base.Save(transaction);                
                
                }
                else
                {
                    new ReportingProcessRepository(transaction).Update(ORUnMap());
                    this.process.Save(transaction);
                    this.process.Scheduler.Save(transaction);
                }

                transaction.Commit();
            }
            return 0;
        }
    }
}
