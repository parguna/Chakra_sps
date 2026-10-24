using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;

using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationAttestation
    {
        public long? Id { get; set; }
        public string AttestationName { get; set; }

        public string AttestationDescription { get; set; }

        public bool RequiresAttestationRemediation { get; set; }

        public string WorkFlow { get; set; }

        public DateTime? CreatedDate { get; set; }

        public static IList<PresentationAttestation> GetAttestations()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new PresentationAttestationRepository(dataAccessTransaction).GetAttestations());
            }
        }
        public static PresentationAttestation ORMap(EntityModel.Presentation.PresentationAttestation presentationAttestation)
        {
            if (presentationAttestation == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationAttestation, PresentationAttestation>(presentationAttestation);
        }

        public static List<PresentationAttestation> ORMapList(IList<EntityModel.Presentation.PresentationAttestation> presentationAttestation)
        {
            return presentationAttestation.Select(ORMap).ToList();
        }
    }
}
