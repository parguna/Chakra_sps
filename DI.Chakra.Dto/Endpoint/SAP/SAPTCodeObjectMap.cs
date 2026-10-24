using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class SAPTCodeObjectMap : DtoBase
    {
        public string ObjectType { get; set; }

        public string FieldValue { get; set; }

        public string ObjectCode { get; set; }

        public string FieldCode { get; set; }

        public long EndPointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }


        public int? Version { get; set; }
    }
}
