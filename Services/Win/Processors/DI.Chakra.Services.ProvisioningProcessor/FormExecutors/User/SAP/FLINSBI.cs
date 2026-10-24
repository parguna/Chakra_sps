using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Linq;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.User
{
    class FLINSBI : Common.SAPT1UserBase
    {

        public FLINSBI(EndPoint endpoint, AccessRequest currentRequest,IdmsLogger idmsLogger) : base(endpoint, currentRequest, idmsLogger)
        {
            Endpoint = endpoint; CurrentRequest = currentRequest;
        }

        protected override string ConstructUserName(DomainModel.User user)
        {
            string UserName = null;
            if (user == null)
            {
                throw new Exception("DI User is NULL in ConstructUserName");

            }
            var accountRecons = AccountReconcilation.GetByAccountTypeAndApplicationType(user.Id, AccountType.User, ApplicationType.ADSingaporePowerLocal);
            if (accountRecons != null && accountRecons.Count > 0)
            {
                var account = accountRecons.FirstOrDefault(u => u.IsLocked == false && u.IsDeleted == false);
                if (account != null)
                    UserName = account.AccountName;
            }
            if (string.IsNullOrWhiteSpace(UserName) && user.FullTimeEmployee)
            {
                var str = $"The user does not have AD User account in {UtilityMethods.GetEnumAttribute<Description>(ApplicationType.ADSingaporePowerLocal)}. Thus IDMS cannot be proceed provisioning the request - {CurrentRequest.Id}.";
                var ex = new Exception(str);
                WriteProvisioningLog(LogLevelType.Exception, str, ex);
                throw ex;
            }

            if (string.IsNullOrWhiteSpace(UserName) && !user.FullTimeEmployee)
            {
                UserName = base.ConstructUserName(user);
            }

            return UserName;
        }

    }
}
