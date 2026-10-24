using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure
{
    public class ProvisioningResult
    {
        public bool Result { get; set; }
        public string Message { get; set; }
        public string Result_TX_ID { get; set; }
        public bool IsADIDFailed { get; set; }

        public bool ADPasswordSetFailed { get; set; }
    }
}
