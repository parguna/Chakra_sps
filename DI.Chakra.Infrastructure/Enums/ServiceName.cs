using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure.Enums
{
    public enum  ServiceName: int
    {
        [Description("DIChakraProvisioningService")]
        DIChakraProvisioningService_CUST= 1,
         [Description("DIChakraDiscoveryService")]
        DIChakraDiscoveryService_CUST= 2,
         [Description("DIChakraAttestationService")]
        DIChakraAttestationService_CUST = 3,
         [Description("DIChakraEmailingService")]
        DIChakraEmailingService_CUST = 4,
         [Description("DIChakraComplianceService")]
        DIChakraComplianceService_CUST = 5,
         [Description("DIChakraMiscellaneousService")]
        DIChakraMiscellaneousService_CUST = 6,
         [Description("DIChakraReportingService")]
        DIChakraReportingService_CUST=7,
         [Description("DIChakraAdHocJobService")]
        DIChakraAdHocJobService_CUST = 8

    }
}
