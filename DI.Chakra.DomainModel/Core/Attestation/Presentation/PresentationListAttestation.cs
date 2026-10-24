using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationListAttestation : Attestation
    {
        public long AttestationId { get; set; }

        public string Status { get; set; }

        public Nullable<System.DateTime> RunDate { get; set; }

        public string AttesationCreatedBy { get; set; }

        public int ResponseTimeInDays { get; set; }

        public Occurence occurence { get; set; }

        public string EndPointSystem { get; set; }

        public string AttestationRemediation { get; set; }

        public static IList<PresentationListAttestation> GetAllByEnpointSystemTypeId(int endpointSystemTypeId)
        {
                IList<PresentationListAttestation> attestationList = new List<PresentationListAttestation>();

                IList<PresentationListAttestation> tempAttestationListList;

                if ((endpointSystemTypeId == 0) || (endpointSystemTypeId == -1))
                {
                    using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
                    {
                        tempAttestationListList = ORMapList(new AttestationRepository(dataAccessTransaction).GetAllAttestationList(endpointSystemTypeId));
                    }
                }
                else
                {
                    using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
                    {
                        tempAttestationListList = ORMapList(new AttestationRepository(dataAccessTransaction).GetAllAttestationList(endpointSystemTypeId));
                    }
                }

                Dictionary<long, long> uniqueStore = new Dictionary<long, long>();
                foreach (PresentationListAttestation Gridlist in tempAttestationListList)
                {
                    if (!uniqueStore.ContainsKey(Gridlist.AttestationId))
                    {
                        uniqueStore.Add(Gridlist.AttestationId, 0);
                        attestationList.Add(Gridlist);
                    }
                }
                return attestationList;
        }

        public static PresentationListAttestation ORMap(EntityModel.Presentation.PresentationListAttestation presentationListAttestationEntityModel)
        {
            if (presentationListAttestationEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationListAttestation, PresentationListAttestation>(presentationListAttestationEntityModel);
        }

        public static List<PresentationListAttestation> ORMapList(IList<EntityModel.Presentation.PresentationListAttestation> presentationListAttestationEntityModel)
        {
            return presentationListAttestationEntityModel.Select(ORMap).ToList();
        }
    }
}
