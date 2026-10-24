using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class PresentationAttestationEndPointRepository : RepositoryBase<PresentationAttestationEndPoint>
    {
        public PresentationAttestationEndPointRepository(DataAccessTransaction transaction) : base(transaction) { }
        
        public IList<PresentationAttestationEndPoint> GetAttestationEndPoints(long AttestationId)
        {
            return (from att in CurrentContext.Attestations
                    join ep in CurrentContext.AttestationEndpoints
                    on att.Id equals ep.AttestationId
                    where att.Id == AttestationId
                    select new PresentationAttestationEndPoint
                    {
                        Id = att.Id,
                        ApplicationName = ep.EndPoint.DisplayName,
                        TargetSystem = ep.EndpointSystemType.ToString()
                    }).ToList();
        }
    }
}
