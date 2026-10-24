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
    public class AttestationEndpoint : DomainModelBase<AttestationEndpoint, EntityModel.AttestationEndpoint>
    {
        public long AttestationId { get; set; }

        public EndpointType EndpointSystemType { get; set; }

        public long EndpointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public Attestation Attestation { get; set; }

        public EndPoint EndPoint { get; set; }

        public static IList<AttestationEndpoint> GetByAttestationId(long attestationId)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AttestationEndpoint>(tran)
                    .Get(attatn => attatn.AttestationId == attestationId, attatn => attatn.EndPoint).ToList());
            }
        }

        public static IList<AttestationEndpoint> GetByAttestationIdIncludeEndpoint(long attestationId)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AttestationEndpoint>(tran)
                    .Get(attatn => attatn.AttestationId == attestationId, true).ToList());
            }
        }

        public static List<string> GetAttestationEndpoints()
        {

            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new AttestationRepository(dataAccessTransaction).GetAttestationEndpoints();
            }
        }

        public static List<string> GetAttestationEndpointsForActiveCampaign()
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return new AttestationRepository(tran).GetAttestationEndpointsForActiveCampaign();
            }
        }

        public static List<string> GetAttestationByEndpointName(string endPointTypeName)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return new AttestationRepository(tran).GetAttestationByEndpointName(endPointTypeName);
            }
        }
    }
}
