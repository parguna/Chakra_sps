using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    [NotMapped]
    public class PresentationADSPrinterInfo : ADSPrinterInfo
    {
        public string DisplayName { get; set; } 
    }
}
