using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class BulkRequestRepository : RepositoryBase<BulkRequest>
    {
        public BulkRequestRepository(DataAccessTransaction transaction) : base(transaction) {
        }
        
        public AdHocAccessRequest GetRemoveAdocRequest(DateTime validTo, AdHocActionType adhocActionType, long userId)
        {
            var adHocAccessRequest = (from accessRequests in this.CurrentContext.AccessRequest
                                      join adhocAccessRequests in this.CurrentContext.AdHocAccessRequests
                                      on accessRequests.AdHocAccessRequestId equals adhocAccessRequests.Id
                                      where (((accessRequests.ProgressStatus == AccessRequestProgress.WaitingforApproval) || (accessRequests.ProgressStatus == AccessRequestProgress.ReadytoExecute) ||
                                      (accessRequests.ProgressStatus == AccessRequestProgress.Executing)) && (adhocAccessRequests.AdhocActionType == adhocActionType) &&
                                      (adhocAccessRequests.IsÉxpiryHandled == false) && (adhocAccessRequests.RequesteeUserId == userId))
                                      select adhocAccessRequests
                        ).FirstOrDefault();
            
            return adHocAccessRequest;
        }

        public BulkRequest GetRequestsToExecute()
        {
            try
            {
                return this.CurrentContext.BulkRequests.Include("BulkRequestDetails").Include("EndPoint").
                    FirstOrDefault(item => item.RequestStatus == AccessRequestStatus.Granted &&
                        item.ProgressStatus == AccessRequestProgress.ReadytoExecute);
            }
            catch
            {
                throw;
            }
        }

        public List<BulkRequest> GetSubmittedBulkRequests()
        {
            try
            {
                return CurrentContext.BulkRequests.Include("BulkRequestDetails").Where(item => item.RequestStatus == AccessRequestStatus.Submitted &&
                        item.ProgressStatus == AccessRequestProgress.Submitted).ToList();
            }
            catch
            {
                throw;
            }
        }

        public  List<BulkRequest> GetAllBulkRequestsToExecute()
        {
            try
            {
                return this.CurrentContext.BulkRequests.Include("BulkRequestDetails").Include("EndPoint").
                    Where(item => item.RequestStatus == AccessRequestStatus.Granted &&
                        item.ProgressStatus == AccessRequestProgress.ReadytoExecute).OrderBy(i => i.Id).ToList();
            }
            catch
            {
                throw;
            }
        }
    }
}
