using AutoMapper;
using DI.Chakra.Infrastructure;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationAttestationScope
    {
        public long Id { get; set; }
        public long AttestationId { get; set; }
        public long OrganisationScopeId { get; set; }
        public string OrganisationScopeName { get; set; }
        public string AttestationScopeName { get; set; }
        public long AttestationScopeValue { get; set; }
        public string AttestationScopeValueDesc { get; set; }


        public static List<PresentationAttestationScope> GetAttestationScopeByAttestationId(long attestationId)  
        {
            List<PresentationAttestationScope> lstattScope = new List<PresentationAttestationScope>();
            Attestation attestation = Attestation.GetByIdIncludingAttestationProcesses(attestationId);

            if (attestation != null)
            {
                foreach (AttestationScope attScope in attestation.AttestationScopes)
                {
                        if ((attScope.AttestationScopeName != AttestationScopeEnum.IACMGroup.ToString()) &&
                           (attScope.AttestationScopeName != AttestationScopeEnum.User.ToString()) &&
                           (attScope.AttestationScopeName != AttestationScopeEnum.RO.ToString()))
                        {
                            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
                            {
                                lstattScope.Add(ORMap(new PresentationAttestationScopeRepository(dataAccessTransaction).GetOrganisationScope(attScope.Id, Convert.ToInt32(attScope.OrganisationScopeId == null ? 0 : attScope.OrganisationScopeId))));
                            }
                        }
                        if (attScope.AttestationScopeName == AttestationScopeEnum.IACMGroup.ToString())
                        {
                            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
                            {
                                lstattScope.Add(ORMap(new PresentationAttestationScopeRepository(dataAccessTransaction).
                                GetIACMGroupScope(attScope.Id, Convert.ToInt32(attScope.AttestationScopeValue == null ? 0 : attScope.AttestationScopeValue))));
                            }
                            
                        }
                        if (attScope.AttestationScopeName == AttestationScopeEnum.User.ToString())
                        {
                            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
                            {
                                lstattScope.Add(ORMap(new PresentationAttestationScopeRepository(dataAccessTransaction)
                                    .GetUserScope(attScope.Id, Convert.ToInt32(attScope.AttestationScopeValue == null ? 0 : attScope.AttestationScopeValue))));
                            }
                            
                        }
                        if (attScope.AttestationScopeName == AttestationScopeEnum.RO.ToString())
                        {
                            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
                            {
                                lstattScope.Add(ORMap(new PresentationAttestationScopeRepository(dataAccessTransaction)
                                .GetManagerScope(attScope.Id, Convert.ToInt32(attScope.AttestationScopeValue == null ? 0 : attScope.AttestationScopeValue))));
                            }
                            
                        }
                }
            }    
            return lstattScope;
        }

        public static List<PresentationAttestationScope> GetOrganisationHeadList(long attestationId)
        {
            List<PresentationAttestationScope> lstattScope = new List<PresentationAttestationScope>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                lstattScope = ORMapList(new PresentationAttestationScopeRepository(dataAccessTransaction).GetOrganisationHeadList(attestationId));
                return lstattScope;
            }
        }

        public static PresentationAttestationScope ORMap(EntityModel.Presentation.PresentationAttestationScope presentationAttestationReview)
        {
            if (presentationAttestationReview == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationAttestationScope, PresentationAttestationScope>(presentationAttestationReview);
        }

        public static List<PresentationAttestationScope> ORMapList(IList<EntityModel.Presentation.PresentationAttestationScope> presentationAttestationReview)
        {
            return presentationAttestationReview.Select(ORMap).ToList();
        }
    }
}
