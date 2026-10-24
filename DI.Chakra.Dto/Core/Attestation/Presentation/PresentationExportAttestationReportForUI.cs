using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    [Serializable]
    public class PresentationExportAttestationReportForUI
    {
        public string AttestationName { get; set; }
        public string CampaignName { get; set; }
        public string EndpointSystemName { get; set; }

        public string EndpointType { get; set; }       

        public byte[] excelFile { get; set; }
    }
}
