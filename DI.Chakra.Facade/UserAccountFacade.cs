using DI.Chakra.DataAccess.Implementation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Facade
{
    public class UserAccountFacade
    {
        private UserAccountDA UserAccountDA = new UserAccountDA();
        private ReconcilationFacade reconFacade = new ReconcilationFacade();

        public IList<UserAccount> GetUserAccountDetail(long UserID)
        {
            try
            {
                IList<AccountReconcilation> lst = reconFacade.GetAccountReconcilations(UserID).ToList();

                return (from acnt in lst
                    select new UserAccount
                    {
                        AcountReconcilationId = acnt.Id,
                        EndpointUserId = acnt.EndpointUserId,
                        UserName = acnt.AccountName,
                        FullName = acnt.FullName,
                        LastLogonDate = null,
                        AccountType = acnt.AccountType,
                        EndpointSystemId = acnt.EndpointSystemId,
                        EndPointSystemType = acnt.EndPoint.EndPointSystemType.EndpointSystemTypeName,
                        EndpointSystemName = acnt.EndPoint.DisplayName

                    }).ToList();
                        

                
            }
            catch
            {
                throw;
            }
        }
        public IList<UserEntitlement> GetEntitlementDetail(long id)
        {
            try
            {
                ReconcilationFacade reconFacade = new ReconcilationFacade();

                AccountReconcilation recon = reconFacade.GetAccountReconcilationRecordById(id);

                if (recon == null)
                    return null;

                return UserAccountDA.GetEntitlementDetail(recon.EndpointUserId, recon.EndPoint.EndPointSystemType.EndpointSystemTypeName);
            }
            catch
            {
                throw;
            }
        }
        public IList<UserEntitlement> GetEntitlementDetail(long EndpointUserId, EndpointType ApplicationType)
        {
            try
            {
                return UserAccountDA.GetEntitlementDetail(EndpointUserId, ApplicationType);
            }
            catch
            {
                throw;
            }
        }

        public IList<UserAccount> GetUserAccountDetailByApplication(int ApplicationID)
        {
            try
            {
                return UserAccountDA.GetUserAccountDetailByApplication(ApplicationID);
            }
            catch
            {
                throw;
            }
        }
        public IList<UserAccount> GetUserAccountDetailByApplicationAUId(int ApplicationID, int EndpointUserId)
        {
            try
            {
                return UserAccountDA.GetUserAccountDetailByApplication(ApplicationID).Where(ua => ua.EndpointUserId == EndpointUserId).ToList();
            }
            catch
            {
                throw;
            }
        }
        public AccountDetail GetAccountDetailByEndPointUserID(int EndpointUserId, EndpointType ApplicationType)
        {
            try
            {
                return UserAccountDA.GetAccountDetailByEndPointUserID(EndpointUserId, ApplicationType);
            }
            catch
            {
                throw;
            }
        }

        public IList<AccountDetail> GetPwdExpiryUsers()
        {
            try
            {
                return UserAccountDA.GetPwdExpiryUsers();
            }
            catch
            {
                throw;
            }
        }

        
    }
}
