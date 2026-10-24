using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationADSOUs
    {
        public long OuId { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public Nullable<DateTime> whenCreated { get; set; }

        public Nullable<DateTime> whenChanged { get; set; }

        public string HostName { get; set; }

        public string Institution { get; set; }
    }
}
