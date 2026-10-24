using DI.Chakra.DomainModel;
using DI.Chakra.Logger;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.Secondary
{
    class SAPLocal : Common.ADBase
    {
        public SAPLocal(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(endpoint, currentRequest, idmsLogger)
        {
        }


    }
}
