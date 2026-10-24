using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System;

namespace DI.Chakra.Repository
{
    public class PresentationADSActiveAccountsWithInActiveUserRepository : RepositoryBase<PresentationADSActiveAccountsWithInActiveUser>
    {
        public PresentationADSActiveAccountsWithInActiveUserRepository(DataAccessTransaction transaction) : base(transaction) { }

        public List<PresentationADSActiveAccountsWithInActiveUser> GetADSActiveAccountsWithInActiveUser(long endpointSystemId)
        {
            try
            {
                var lstOfInactiveUsers = (from account in CurrentContext.AccountReconcilations
                                          join user in CurrentContext.User.Include(i => i.NOrganisation) on account.UserId equals user.Id
                                          join endpointUser in CurrentContext.ADSUserInfos on account.EndpointUserId equals endpointUser.Id
                                          where account.EndpointSystemId == endpointSystemId &&
                                          !endpointUser.IsAccountDisabled &&
                                          !endpointUser.IsAccountLocked &&
                                          !account.IsDeleted &&
                                          !account.IsLocked &&
                                          !(bool)user.IsActive
                                          select new PresentationADSActiveAccountsWithInActiveUser
                                          {
                                              UserId = user.Id,
                                              UserName = user.UserName,
                                              DisplayName = user.DisplayName,
                                              TerminatedDate = user.TerminationDate,
                                              UserIsActive = user.IsActive,
                                              EmailId = user.EmailId,
                                              OrganisationName = user.NOrganisationId.HasValue ? user.NOrganisation.Name : "",
                                              EndPointSystemId = account.EndpointSystemId,
                                              sAMAccountName = endpointUser.sAMAccountName,
                                              AccountIsActive = !(endpointUser.IsAccountDisabled || endpointUser.IsAccountLocked),
                                              displayName = endpointUser.displayName,
                                              department = endpointUser.department,
                                              whenCreated = endpointUser.whenCreated,
                                              lastLogon = endpointUser.lastLogon,
                                              pwdLastSet = endpointUser.pwdLastSet,
                                              accountExpires = endpointUser.accountExpires,
                                              IsAccountDisabled = endpointUser.IsAccountDisabled,
                                              IsAccountLocked = endpointUser.IsAccountLocked,
                                          }
                        ).ToList();

                return lstOfInactiveUsers;
            }
            catch  (Exception ex)
            {

            }

            return new List<PresentationADSActiveAccountsWithInActiveUser>();
        }
    }
}
