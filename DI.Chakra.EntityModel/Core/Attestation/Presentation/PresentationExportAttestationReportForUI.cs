using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationExportAttestationReportForUI : EntityBase
    {
        public string AttestationName { get; set; }
        public string CampaignName { get; set; }
        public string EndpointSystemName { get; set; }

        public string EndpointType { get; set; }       

        public byte[] excelFile { get; set; }
    }
}
