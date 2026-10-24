using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;
using DI.Chakra.Services.ProvisioningProcessor.FormExecutors.Common;
using System;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.Secondary
{
    public class GEMS : GEMSBase
    {
        public GEMS(EndPoint endpoint, AccessRequest currentRequest,IdmsLogger idmsLogger) : base(endpoint, currentRequest,idmsLogger)
        {
            Endpoint = endpoint;
            CurrentRequest = currentRequest;
        }

        public override bool Execute(ProvisioningParamCollection fields, DomainModel.User user)
        {
            if (CurrentRequest.AdHocAccessRequest.AdhocActionType == Infrastructure.Enums.AdHocActionType.NewAccount)
            {
                var gemsDetail = (AdHocGEMSRequest)CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();
                UserName = ConstructUserName(gemsDetail.AdHocGEMSSharedAccount.AccountName);
            }
            bool result = base.Execute(fields, user);
            return result;
        }
    }
}
