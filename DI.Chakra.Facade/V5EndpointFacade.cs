using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Facade
{
    public class V5EndpointFacade
    {

        public static IList<IEndpointUserInfo> GetEndpointUserInfoFromAccountRecon(IList<AccountReconcilation> listAccountRecon)
        {
            return listAccountRecon.Select(accountReconcilation => GetEndpointUserInfo(accountReconcilation.Id))
                                   .Where(endpointUserInfo => endpointUserInfo != null)
                                   .ToList();
        }
        public static IEndpointUserInfo GetEndpointUserInfoFromAccountRecon(AccountReconcilation accountRecon)
        {
            return GetEndpointUserInfo(accountRecon.Id);
                                   
        }
        public static IEndpointUserInfo GetEndpointUserInfo(long reconId)
        {
            AccountReconcilation acctRecon = new ReconcilationFacade().GetAccountReconcilationRecordById(reconId);

            return new ReconcilationFacade().GetEndpointUserInfo((int)acctRecon.EndpointSystemId, acctRecon.EndpointUserId);
        }
    }
}