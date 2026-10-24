using DI.Chakra.DomainModel;
using DI.Chakra.Logger;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.Shared
{
    class FLINS : Common.FLINSBase
    {

        public FLINS(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(endpoint, currentRequest,idmsLogger)
        {
        }
    }
}
