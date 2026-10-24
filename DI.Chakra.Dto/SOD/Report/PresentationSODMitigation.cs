using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class PresentationSODMitigation
    {
        public string MitigationCode { get; set; }
        public string MitigationDescription { get; set; }
        public string Document { get; set; }
        public Nullable<DateTime> DocumentDate { get; set; }
        public IEnumerable<string> RiskCode { get; set; }
    }
}
