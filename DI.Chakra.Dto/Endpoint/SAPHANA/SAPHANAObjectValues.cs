using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class SAPHANAObjectValues:DtoBase
    {
        public string SapObjectType { get; set; }

        public string SapObjectCode { get; set; }

        public String SapOkFlag { get; set; }

        public string ModificationID { get; set; }

        public string OrgName { get; set; }

        public long EndPointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }


        public int? Version { get; set; }
    }
}
