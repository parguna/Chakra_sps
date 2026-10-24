using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Services.ProvisioningProcessor.FormExecutors.Common;
using System;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.Service
{
    public class GEMS : GEMSBase
    {
        public GEMS(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(endpoint, currentRequest, idmsLogger)
        {
            Endpoint = endpoint;
            CurrentRequest = currentRequest;
        }

        public override bool Execute(ProvisioningParamCollection fields, DomainModel.User user)
        {
            try
            {
                WriteProvisioningLog(LogLevelType.DebugStep, $"GEMS Provisioning service account started for user {user?.KnownAs ?? ""}");

                if (CurrentRequest.AdHocAccessRequest.AdhocActionType == Infrastructure.Enums.AdHocActionType.NewAccount)
                {
                    var gemsDetail = (AdHocGEMSRequest)CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();
                    UserName = ConstructUserName(gemsDetail.AdHocGEMSSharedAccount.AccountName);
                }
                bool result = base.Execute(fields, user);
                return result;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"Error in GEMS Provisioning service account for user {user?.KnownAs ?? ""}", ex);
                throw ex;
            }
        }
        public override bool OffBoard(DomainModel.User user, ProvisioningParamCollection fields)
        {
            return true;
        }
        public override bool Propagate(DomainModel.User user, ProvisioningParamCollection fields)
        {
            return true;
        }

        public override bool ReOrg(DomainModel.User user, ProvisioningParamCollection fields)
        {
            return true;
        }

        public override bool Transfer(DomainModel.User user, ProvisioningParamCollection fields)
        {
            return true;
        }

        public override bool Extension(DomainModel.User user, ProvisioningParamCollection fields, AccountType accountType, long accountReconId = 0)
        {
            return true;
        }

        public override bool UndoEvent(DomainModel.User user, ProvisioningParamCollection fields)
        {
            return true;
        }
    }
}
