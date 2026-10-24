using DI.Chakra.DomainModel;
using DI.Chakra.Logger;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.Shared
{
    class SPLocal : Common.ADBase
    {
        public SPLocal(EndPoint endpoint, AccessRequest currentRequest,IdmsLogger idmsLogger) : base(endpoint, currentRequest, idmsLogger)
        {
        }
    }
}
