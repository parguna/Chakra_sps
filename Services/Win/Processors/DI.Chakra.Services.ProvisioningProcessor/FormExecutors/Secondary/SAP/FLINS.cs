using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.Secondary
{
    class FLINS : Common.FLINSBase
    {

        public FLINS(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(endpoint, currentRequest,idmsLogger)
        {
        }

        public override bool Transfer(DomainModel.User user, ProvisioningParamCollection fields)
        {
            try
            {
                var sapUsrs = SAPUserinfo.GetActiveAccounts(user.Id, Endpoint.Id, CurrentRequest.AccountType);

                foreach (SAPUserinfo sapUsr in sapUsrs)
                {
                    this.TakeOUSnapshot(user, sapUsr, EmployeeEvents.OUChange);

                    bool success = this.LockUser(sapUsr, LockScenario.OUChange);
                    if (success)
                        WriteProvisioningLog(LogLevelType.Info, string.Format("User {0} modified in SAP.", sapUsr.UserLogonName), null);
                    else
                        WriteProvisioningLog(LogLevelType.Error, string.Format("Error while locking User {0} in SAP.", sapUsr.UserLogonName), null);
                }

                return true;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing the transfer request. {0}", ex.Message), ex);
                return false;
            }
        }


    }
}
