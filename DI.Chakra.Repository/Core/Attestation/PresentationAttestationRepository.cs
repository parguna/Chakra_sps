using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class PresentationAttestationRepository : RepositoryBase<PresentationAttestation>
    {
        public PresentationAttestationRepository(DataAccessTransaction transaction) : base(transaction) { }
        
        public IList<PresentationAttestation> GetAttestations()
        {
            return (from att in CurrentContext.Attestations
                    join wf in CurrentContext.WorkflowConfigurations
                    on att.WorkflowConfigurationId equals wf.Id
                    select new PresentationAttestation
                    {
                        Id = att.Id,
                        AttestationDescription = att.AttestationDescription,
                        AttestationName = att.AttestationName,
                        CreatedDate = att.CreatedDate,
                        RequiresAttestationRemediation = att.RequiresAttestationRemediation,
                        WorkFlow = wf.WorkflowName
                    }).ToList();
        }
    }
}
