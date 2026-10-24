using AutoMapper;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationReviewer
    {
        public string ReviewerName { get; set; }
        public string DisplayName { get; set; }
        public string EmailId { get; set; }

        public long TaskId { get; set; }

        public static List<PresentationReviewer> GetAttestationReviewers(long AttestationId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new PresentationReviewerRepository(dataAccessTransaction).GetAttestationReviewers(AttestationId));
            }
        }
        public static List<PresentationReviewer> GetAttestationRejectedReviewers(long AttestationId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new PresentationReviewerRepository(dataAccessTransaction).GetAttestationRejectedReviewers(AttestationId));
            }
        }
        public static PresentationReviewer ORMap(EntityModel.Presentation.PresentationReviewer presentationOrganisation)
        {
            if (presentationOrganisation == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationReviewer, PresentationReviewer>(presentationOrganisation);
        }

        public static List<PresentationReviewer> ORMapList(IList<EntityModel.Presentation.PresentationReviewer> presentationOrganisation)
        {
            return presentationOrganisation.Select(ORMap).ToList();
        }
    }
}
