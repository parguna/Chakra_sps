using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class PresentationReviewerRepository : RepositoryBase<PresentationReviewer>
    {
        public PresentationReviewerRepository(DataAccessTransaction transaction) : base(transaction) { }

        public IList<PresentationReviewer> GetAttestationReviewers(long AttestationId)
        {
            List<PresentationReviewer> reviewers = new List<PresentationReviewer>();
            var distinctActionUserIds = CurrentContext.Tasks.Where(t => t.AttestationId == AttestationId).Select(t => t.ActionUserId).Distinct().ToList();
            foreach(long id in distinctActionUserIds)
            {
                var PresentationReviewer =  (from att in CurrentContext.Attestations
                        join tsk in CurrentContext.Tasks
                        on att.Id equals tsk.AttestationId
                        join usr in CurrentContext.User
                        on tsk.ActionUserId equals usr.Id
                        where att.Id == AttestationId && tsk.ActionUserId == id 
                        select new PresentationReviewer
                        {
                            TaskId = tsk.Id,
                            DisplayName = usr.DisplayName,
                            ReviewerName = usr.UserName,
                            EmailId = usr.EmailId
                        }).ToList().LastOrDefault();

                reviewers.Add(PresentationReviewer);
            }
            return reviewers;
        }

        public IList<PresentationReviewer> GetAttestationRejectedReviewers(long AttestationId)
        {
            List<PresentationReviewer> reviewers = new List<PresentationReviewer>();
            var distinctActionUserIds = CurrentContext.Tasks.Where(t => t.AttestationId == AttestationId).Select(t => t.ActionUserId).Distinct().ToList();
            foreach (long id in distinctActionUserIds)
            {
                var PresentationReviewer = (from att in CurrentContext.Attestations
                                            join tsk in CurrentContext.Tasks
                                            on att.Id equals tsk.AttestationId
                                            join usr in CurrentContext.User
                                            on tsk.ActionUserId equals usr.Id
                                            where att.Id == AttestationId && tsk.ActionUserId == id
                                            select new PresentationReviewer
                                            {
                                                TaskId = tsk.Id,
                                                DisplayName = usr.DisplayName,
                                                ReviewerName = usr.UserName,
                                                EmailId = usr.EmailId
                                            }).ToList().LastOrDefault();
                if(CheckTaskIsRejected(PresentationReviewer.TaskId))
                    reviewers.Add(PresentationReviewer);
            }
            return reviewers;
        }

        private bool CheckTaskIsRejected(long TaskId)
        {
            bool isRejected = false;

            isRejected = CurrentContext.AttestationSnapshotActions.Where(t => t.TaskId == TaskId).Any(x=>x.Revoke == true);
            
            return isRejected;
        }
    }
}
