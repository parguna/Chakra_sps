using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;
using DI.Chakra.Services.ProvisioningProcessor.FormExecutors.Common;
using System;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.Service
{
    public class SRM : SRMBase
    {
        public SRM(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(endpoint, currentRequest,idmsLogger)
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
