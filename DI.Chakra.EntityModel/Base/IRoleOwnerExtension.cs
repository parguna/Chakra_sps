using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    public interface IRoleOwnerExtension
    {        
        string UserDefinedDisplayName { get; set; }

        string UserDefinedDesc { get; set; }

        bool? CanRequest { get; set; }

        Nullable<long> Level1Approver { get; set; }

        Nullable<long> Level2Approver { get; set; }

        Nullable<bool> IsLevel1Group { get; set; }

        Nullable<bool> IsLevel2Group { get; set; }
    }
}
