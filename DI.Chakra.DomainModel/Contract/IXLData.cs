using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public interface IXLData
    {
        long Id { get; set; }

        DateTime? ValidFrom { get; set; }

        DateTime? ValidTo { get; set; }

        bool IsValid { get; set; }

        string ValidationMessage { get; set; }
        
    }
}
