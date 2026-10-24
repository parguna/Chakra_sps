using DI.Chakra.DomainModel;
using DI.Chakra.Logger;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.Secondary
{
    class PI : Common.SAPT1SpecialBase
    {

        public PI(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(endpoint, currentRequest,idmsLogger)
        {
            Endpoint = endpoint; CurrentRequest = currentRequest;
        }

    }
}
