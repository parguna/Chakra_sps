using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DI.Chakra.Dto
{
    public class Tenant : DtoBase
    {
        public string TenantId { get; set; }

        public string TenantName { get; set; }

        public string TenantDescription { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> CreateDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
