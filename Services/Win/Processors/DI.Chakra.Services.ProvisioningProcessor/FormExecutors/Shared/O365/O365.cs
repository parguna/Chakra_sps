using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Services.ProvisioningProcessor.FormExecutors.Common;
using System;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.Shared
{
    public class O365 : O365Base
    {
        public O365(EndPoint endpoint, AccessRequest currentRequest,IdmsLogger idmsLogger) : base(endpoint, currentRequest, idmsLogger)
        {
            Endpoint = endpoint;
            CurrentRequest = currentRequest;
        }

        public override bool Execute(ProvisioningParamCollection fields, DomainModel.User user)
        {
            bool result = base.Execute(fields, user);
            return result;
        }      
    }
}
