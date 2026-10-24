using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class AttestationScope : DomainModelBase<AttestationScope, EntityModel.AttestationScope>
    {
        public long AttestationId { get; set; }

        public Nullable<long> OrganisationScopeId { get; set; }

        public string AttestationScopeName { get; set; }

        public Nullable<long> AttestationScopeValue { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public Attestation Attestation { get; set; }

        public NOrganisation NOrganisation { get; set; }

        public string AttestationScopeValueDesc { get; set; }

        public static IList<AttestationScope> GetByAttestationId(long attestationId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AttestationScope>(dataAccessTransaction)
                    .Get(attatn => attatn.AttestationId == attestationId).ToList());
            }
        }
    }
}
