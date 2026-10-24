using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.EntityModel;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Repository
{
    public class SchedulerRepository : RepositoryBase<Scheduler>
    {
        public SchedulerRepository(DataAccessTransaction transaction) : base(transaction) { }

        public Scheduler GetDiscoveryScheduler(long discoveryProcessId)
        {           
            return (from discovery in CurrentContext.DiscoveryProcess
                        join pr in CurrentContext.Process
                        on discovery.Id equals pr.ProcessTypeKeyId
                        join s in CurrentContext.Scheduler
                        on pr.Scheduler.Id equals s.Id
                        where pr.ProcessType == ProcessType.Discovery
                        && discovery.Id == discoveryProcessId
                    select s).ToList().FirstOrDefault();
        }

        public Scheduler GetComplianceScheduler(long complianceProcessId)
        {
            return(from compliance in CurrentContext.ComplianceProcess
                        join pr in CurrentContext.Process
                                on compliance.Id equals pr.ProcessTypeKeyId
                        join s in CurrentContext.Scheduler
                                on pr.Scheduler.Id equals s.Id
                                where pr.ProcessType == ProcessType.Compliance
                                && compliance.Id == complianceProcessId
                   select s).ToList().FirstOrDefault();

        }

        public Scheduler GetAttestationScheduler(long attestationProcessId)
        {
            return (from attestation in CurrentContext.AttestationProcesses
                    join pr in CurrentContext.Process
                    on attestation.Id equals pr.ProcessTypeKeyId
                    join s in CurrentContext.Scheduler
                    on pr.Scheduler.Id equals s.Id
                    where pr.ProcessType == ProcessType.Attestation
                    && attestation.Id == attestationProcessId
                    select s).ToList().FirstOrDefault();

        }

    }
}
