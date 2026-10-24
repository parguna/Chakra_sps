using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Repository
{
    public class PresentationAccessSnapshotsRepository : RepositoryBase<PresentationAccessSnapshots>
    {
        public PresentationAccessSnapshotsRepository(DataAccessTransaction transaction) : base(transaction) { }


        public IList<PresentationAccessSnapshots> GetByUserActionUserId(long actionUserId)
        {
            IList<PresentationAccessSnapshots> data = new List<PresentationAccessSnapshots>();

            if (actionUserId > 0)
            {
                var account = (from accRecon in CurrentContext.AccountReconcilations
                               join Endpnt in CurrentContext.EndPoint
                                   on accRecon.EndpointSystemId equals Endpnt.Id
                               join usr in CurrentContext.User
                                    on accRecon.UserId equals usr.Id
                               where accRecon.UserId == actionUserId
                               select new PresentationAccessSnapshots
                               {
                                   UserName = accRecon.AccountName,
                                   DisplayName = usr.DisplayName,
                                   EmailID = usr.EmailId,
                                   EndpointsystemName = Endpnt.DisplayName,
                                   AccountName = accRecon.AccountName,
                                   AccountType = accRecon.AccountType.ToString(),
                                   Entitlement = string.Empty,
                                   EntitlementType = EntitlementType.None.ToString(),
                                   CreatedDate = accRecon.CreatedDate,
                                   IsDeleted = accRecon.IsDeleted,
                                   ProvisionedThrough = accRecon.ProvisionedThrough,
                                   IsResolved = accRecon.IsResolved,
                                   ResolvedBy = accRecon.ResolvedBy,
                                   ResolvedStatus = accRecon.ResolvedStatus.ToString(),
                                   LastLogonDate = accRecon.LastLogonDate,
                                   ValidFromDate = accRecon.ValidFromDate,
                                   ValidToDate = accRecon.ValidToDate,
                                   AccessRequestId = accRecon.AccessRequestId,

                               }).ToList();

                var entitlement = (from accRecon in CurrentContext.AccountReconcilations
                               join entRecon in CurrentContext.EntitlementReconcilations
                                   on accRecon.Id equals entRecon.AccountReconcilationId
                               join Endpnt in CurrentContext.EndPoint
                                   on accRecon.EndpointSystemId equals Endpnt.Id
                               join usr in CurrentContext.User
                                    on accRecon.UserId equals usr.Id
                               where accRecon.UserId == actionUserId
                               select new PresentationAccessSnapshots
                               {
                                   UserName = accRecon.AccountName,
                                   DisplayName = usr.DisplayName,
                                   EmailID = usr.EmailId,
                                   EndpointsystemName = Endpnt.DisplayName,
                                   AccountName = accRecon.AccountName,
                                   AccountType = accRecon.AccountType.ToString(),
                                   Entitlement = entRecon.Entitlement,
                                   EntitlementType = ((EntitlementType)entRecon.EntitlementType).ToString(),
                                   CreatedDate = entRecon.EnteredDate,
                                   IsDeleted = entRecon.IsDeleted,
                                   ProvisionedThrough = entRecon.ProvisionedThrough,
                                   IsResolved = entRecon.IsResolved,
                                   ResolvedBy = entRecon.ResolvedBy,
                                   ResolvedStatus = entRecon.ResolvedStatus.ToString(),
                                   LastLogonDate = null,
                                   ValidFromDate = entRecon.ValidFromDate,
                                   ValidToDate = entRecon.ValidToDate,
                                   AccessRequestId = entRecon.AccessRequestId,

                               }).ToList();

                data = account;
                data = account.Union(entitlement).ToList();

                data = data.OrderByDescending(a => a.CreatedDate).ThenBy(a => a.AccountName).ToList();
            }

            return data;
        }
    }
}
