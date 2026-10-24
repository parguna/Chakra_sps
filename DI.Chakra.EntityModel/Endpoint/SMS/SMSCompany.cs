using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SMS_Company")]
    public class SMSCompany : EntityBase, ICreatable, IUpdatable
    {
        public long EndPointSystemId { get; set; }
        public int CompanyKey { get; set; }
        public string Name { get; set; }
        public int SegmentID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPoint { get; set; }

    }
}
