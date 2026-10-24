using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationADSOUsReportData
    {
        public long OuId { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public string whenCreated { get; set; }

        public string whenChanged { get; set; }

        public string HostName { get; set; }

        public string Institution { get; set; }
    }
}
