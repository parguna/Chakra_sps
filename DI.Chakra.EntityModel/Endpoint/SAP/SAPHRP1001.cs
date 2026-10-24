using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAP_SAPHRP1001")]
    public class SAPHRP1001 : EntityBase ,ICreatable,IUpdatable
    {
        public long EndpointSystemId { get; set; }

        public string ActualObjectType { get; set; }
        public string ActualObjectCode { get; set; }

        public string RelatedObjectType { get; set; }

        public string RelatedObjectCode { get; set; }

        public string BeginDate { get; set; }

        public string EndDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }


    }
}
