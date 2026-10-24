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
    public class AttestationReview : DomainModelBase<AttestationReview, EntityModel.AttestationReview>
    {
        public long UserId { get; set; }

        public string Comments { get; set; }

        public string SubmittedBy { get; set; }

        public Nullable<System.DateTime> SubmittedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public string ActionId { get; set; }

        public string ActionTakenBy { get; set; }

        public string ActionComments { get; set; }

        public int HistoryId { get; set; }

        public bool IsChanged { get; set; }

        public bool HasRisk { get; set; }
        public AttestationRemediationStatus RemediationStatus { get; set; }

        public long ReviewerId;

        public bool IsGroupTask;

        public List<ReviewEndpointSystem> ReviewEndpointSystems { get; set; }

        public User User { get; set; }

        public static AttestationReview GetByIdIncludeAttestationReview(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.AttestationReview>(dataAccessTransaction)
                    .GetById(id, 
                    attest => attest.User, 
                    attest => attest.ReviewEndpointSystems, 
                    attest => attest.ReviewEndpointSystems
                        .Select(x=>x.AttestationReviewEndpointSystem.AttestationReviewEndpointSystemL2Details)));
            }
        }

        public static AttestationReview GetPendingRemediationStatusIncludingUser()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.AttestationReview>(dataAccessTransaction)
                    .Get(attest => attest.RemediationStatus == AttestationRemediationStatus.Pending,
                    attest => attest.User, 
                    attest => attest.ReviewEndpointSystems, 
                    attest => attest.ReviewEndpointSystems
                        .Select(x => x.AttestationReviewEndpointSystem.AttestationReviewEndpointSystemL2Details)).FirstOrDefault());
            }
        }

        public static List<AttestationReview> GetPendingRemediations()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new AttestationRepository(dataAccessTransaction).GetPendingRemediations());
            }
        }

    }
}
