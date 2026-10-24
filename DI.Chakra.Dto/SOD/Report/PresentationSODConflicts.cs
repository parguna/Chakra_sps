using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class PresentationSODConflicts
    {
        public string SODConflictCode { get; set; }
        public string SODConflictDescription { get; set; }
        public Nullable<int> SodConflictType { get; set; }
        public string Function1 { get; set; }
        public string Function2 { get; set; }
        public string RiskCode { get; set; }
    }
}
