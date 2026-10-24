using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class PresentationSODRiskCode
    {
        public string RiskCode { get; set; }
        public string RiskDescription { get; set; }
        public string RiskLevel { get; set; }
        public int RiskScore { get; set; }
        public string RiskCodeOwner { get; set; }
    }
}
