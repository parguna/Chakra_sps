using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public abstract class IdBase
    {
        public long Id { get; set; }
    }
}
