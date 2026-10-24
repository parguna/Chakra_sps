using System;
using System.Collections.Generic;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using SAP.Middleware.Connector; 

namespace DI.Chakra.Connector.SAPHANA
{
    public class SAPHANAProcessorConfig
    {
        public DateTime DeltaDate { get; set; }
        public bool IsDelta { get; set; }
        public long EndpointId { get; set; }
        public int insertThreshold { get; set; }
        public SAPHANAUtil sapUtil { get; set; }
        public Process Process { get; set; }
    }
}
