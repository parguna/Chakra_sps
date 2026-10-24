using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAP_AuthorizationProfile")]
    public class SAPAuthorizationProfile:EntityBase,ICreatable,IUpdatable
    {
        public string ProfileCode { get; set; }

        public string ObjectCode { get; set; }

        public string AuthorizationCode { get; set; }

        public long EndPointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public  int?  Version { get; set; }
    }
}
