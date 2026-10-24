using System;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;

namespace DI.Chakra.DomainModel
{
    public class Scheduler : DomainModelBase<Scheduler, EntityModel.Scheduler>
    {   
        public string ProcessName { get; set; }
 
        public DateTime StartDate { get; set; }

        public System.TimeSpan StartTime { get; set; }

        public Nullable<Occurence> Occurence { get; set; }

        public Nullable<int> ReccurEveryDays { get; set; }

        public Nullable<int> ReccurEveryWeeks { get; set; }

        public string WeekDay { get; set; }

        public Nullable<int> ReccurEveryMonths { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public static Scheduler GetDiscoveryScheduler(long discoveryProcessId)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new SchedulerRepository(transaction).GetDiscoveryScheduler(discoveryProcessId));
            }
        }

        public static Scheduler GetComplianceScheduler(long complianceProcessId)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new SchedulerRepository(transaction).GetComplianceScheduler(complianceProcessId));
            }
        }

        public static Scheduler GetAttestationScheduler(long attestationProcessId)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new SchedulerRepository(transaction).GetAttestationScheduler(attestationProcessId));
            }
        }

    }
}
