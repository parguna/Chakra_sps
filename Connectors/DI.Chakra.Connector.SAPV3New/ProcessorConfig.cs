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
using DI.Chakra.Connector.SAPBase;

namespace DI.Chakra.Connector.SAPV3
{
    public class ProcessorConfig
    {
        public DateTime DeltaDate { get; set; }
        public bool IsDelta { get; set; }
        public long EndpointId { get; set; }
        public int insertThreshold { get; set; }
        public Sapv3Util sapUtil { get; set; }
        public Process Process { get; set; }
    }
}
