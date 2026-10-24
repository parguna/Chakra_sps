using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{ 
    [Table("DI_SAP_SectionMappingInfo")]
    public class SAPSectionMappingInfo : EntityBase, ICreatable, IUpdatable
    {
        public string HRMSSectionCode { get; set; }
        public string SAPSectionCode { get; set; }
        public string VBUKR { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
