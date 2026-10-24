using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel 
{

    [Table("DI_SAP_Position")]
    public class SAPPosition:EntityBase,ICreatable,IUpdatable
    {
        public long EndPointSystemId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public DateTime? BEGDA { get; set; }

        public DateTime? ENDDA { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public int? Version { get; set; }

    }
}
