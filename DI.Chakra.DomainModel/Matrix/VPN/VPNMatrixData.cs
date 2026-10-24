using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class VPNMatrixData
    {
        public VPNMatrixEnviornmentType EnviornmentType { get; set; }
        public string ADGroup { get; set; }
        public string Name { get; set; }
        public VPNMatrixType VPNMatrixType { get; set; }
    }
}
