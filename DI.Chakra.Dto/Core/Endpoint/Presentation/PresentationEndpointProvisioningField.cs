using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationEndpointProvisioningField
    {
        public long Id { get; set; }
        public long RowID { get; set; }
        public Nullable<long> ParentRowID { get; set; }
        public long EndPointSystemId { get; set; }
        public long EndPointSystemTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool EndPointHasValues { get; set; }

    }
}
