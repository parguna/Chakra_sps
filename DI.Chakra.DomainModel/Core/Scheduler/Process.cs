using System;
using System.Collections.Generic;
using System.Linq;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using DI.Chakra.Repository;

namespace DI.Chakra.DomainModel
{
    public class Process : DomainModelBase<Process, EntityModel.Process>
    {
        public ProcessType ProcessType { get; set; }

        public long ProcessTypeKeyId { get; set; }

        public Nullable<DateTime> ProcessStartAt { get; set; }

        public Nullable<DateTime> ProcessEndAt { get; set; }

        public Nullable<DateTime> LastRunOn { get; set; }

        public string Status { get; set; }

        public string ErrorMessage { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public long Scheduler_SchedulerId { get; set; }

        public Scheduler Scheduler { get; set; }

        public int DiscoveryInstance { get; set; }

        public static Process GetByProcessId(long ProcessId) 
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.Process>(dataAccessTransaction)
                    .Get(item => item.ProcessTypeKeyId == ProcessId && item.ProcessType == ProcessType.Discovery).FirstOrDefault());
            }
        }
  
        public static Process GetByProcessType(long ProcessId, ProcessType processType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.Process>(dataAccessTransaction)
                    .Get(item => item.ProcessTypeKeyId == ProcessId && item.ProcessType == processType, item => item.Scheduler).
                    FirstOrDefault());
            }
        }

        public static List<Process> GetCurrentJobs(ProcessType processType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new ProcessRepository(dataAccessTransaction).GetCurrentJobs(processType).ToList());
            }
        }

        public override long Save(string currentUser)
        {
            bool isNew = this.IsNew;

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                base.Save(dataAccessTransaction);
                if (!isNew && this.Scheduler != null)
                {
                   this.Scheduler.Save(dataAccessTransaction);
                }
                dataAccessTransaction.Commit();
            }
            return 0;
        }

        public static Process GetByProcessTypeKeyId(long processTypeKeyId, ProcessType processType)  
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.Process>(dataAccessTransaction)
                    .Get(item => item.ProcessTypeKeyId == processTypeKeyId && item.ProcessType == processType).FirstOrDefault());
            }
        } 

       
        public override void Delete()
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                try {
                    Scheduler scheduler = Scheduler.GetAttestationScheduler(this.ProcessTypeKeyId);
                    Delete(this.Id);
                    if (scheduler != null)
                        scheduler.Delete();
                    tranaction.Commit();
                }
                catch(System.Exception ex)
                {
                    throw;
                }
                
            }
        }

        public static bool DeleteAllNavigation(long attestationProcessId, Infrastructure.Enums.ProcessType processType,string currentUser)
        {
            var Process = GetByProcessType(attestationProcessId, processType);
            var AttesationProcess = AttestationProcess.GetById(attestationProcessId);
            AttesationProcess.ProcessName = null;
            AttesationProcess.Description = null;
            AttesationProcess.Save(currentUser);
            Process.Delete();
            return true;
        }
    }
}
