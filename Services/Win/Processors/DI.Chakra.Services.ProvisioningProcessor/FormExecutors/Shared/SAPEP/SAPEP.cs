using DI.Chakra.Connector; 
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.ProvisioningUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.Shared
{
    class SAPEP : Common.SAPEPBase
    {

        public SAPEP(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(endpoint, currentRequest,idmsLogger)
        {
            Endpoint = endpoint; CurrentRequest = currentRequest;
        }
        

    }
}
