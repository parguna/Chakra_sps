using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure.Enums
{
    public enum VPNMatrixEnviornmentType : int
    {
        Production = 1,
        QA = 2,
        Development = 3,
        ADGroup = 4
    }
    public enum VPNMatrixType : int
    {
        Application = 23,
        Project = 24
    }
    public enum VPNType : int
    {
        [Description("SSLVPN Only")]
        SSLVPNOnly=1,
        VDI
    }
    
}
