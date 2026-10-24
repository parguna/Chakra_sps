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
    public class AttestationProcessRepository : RepositoryBase<AttestationProcess>
    {
        public AttestationProcessRepository(DataAccessTransaction transaction) : base(transaction) { }
        
        public override void Update(AttestationProcess attprocess)
        {
            AttestationProcess dbDiscoveryProcess = this.GetById(attprocess.Id);
            base.Update(attprocess);
        }

        public List<AttestationProcess> GetAttestationProcessByEndpointType(EndpointType endPointType)
        {
            var NonConnectedAttestationEndPointList = (from attestation in CurrentContext.Attestations
                    where attestation.AttestationEndpoint.Any(m => m.EndpointSystemType == endPointType)
                    select attestation.Id).ToList();
            return CurrentContext.AttestationProcesses.Where(m => NonConnectedAttestationEndPointList.Contains(m.AttestationId.Value)).ToList();
        }

        public bool IsCampaignRunningForAttestation(long attestationId)
        {
            return (from attProcess in CurrentContext.AttestationProcesses
                    join attest in CurrentContext.Attestations on attProcess.AttestationId equals attest.Id
                    join process in CurrentContext.Process.Where(i => i.ProcessType == ProcessType.Attestation) on attProcess.Id equals process.ProcessTypeKeyId
                    where attest.Id == attestationId && attProcess.WorkflowStatus == "InProgress"
                    select attProcess
                    ).Any();
        }
    }
}